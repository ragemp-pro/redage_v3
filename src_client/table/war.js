let isOpenWar = false;
let isInterface = false;

gm.events.add("client.openWar", (title, owner) => {
    if (isOpenWar) return;

    let popupData = {
        title: title,
        owner: owner,
        minHour: 14
    }

    mp.gui.emmit(`window.router.setPopUp("PopupWar", \`${JSON.stringify (popupData)}\`)`);
    isOpenWar = true;
    mp.gui.cursor.visible = true;
    if (!global.menuOpened) {
        global.menuOpen(true);
        isInterface = true;
        global.isPopup = true;
    }


});

gm.events.add("client.war", (typeBattle, composition, weaponsCategory, day, hour, min) => {
    if (!global.antiFlood("server.war", 250))
        return;

    mp.events.callRemote("server.war", typeBattle, composition, weaponsCategory, day, hour, min);
});

gm.events.add("client.closeWar", () => {
    if (!isOpenWar)
        return;

    mp.gui.emmit('window.router.setPopUp()');

    if (isInterface)
        global.menuClose();

    isInterface = false;
    isOpenWar = false;
    global.isPopup = false;

});

//

let captureZone = {};
gm.events.add("createWarZone", (id, posX, posY, posZ, range) => {
    mp.events.call("client.destroyWarZone", id);
    //
    mp.events.call('createWaypoint', posX, posY);
    const position = new mp.Vector3(posX, posY, posZ);

    mp.events.call("createBlip", `captureZone_${id}`, "", 9, position, false, 1, 155, undefined, range);

    captureZone[id] = {
        type: 28,
        position: position,
        scale: range,
        color: [255, 0, 0, 90]
    }
});

gm.events.add("render", () => {
    if (!global.loggedin)
        return;

    if (!captureZone)
        return;

    const markers = Object.values(captureZone);

    if (!markers.length)
        return;

    markers.forEach((marker) => {

        mp.game.graphics.drawMarker(marker.type, marker.position.x, marker.position.y, marker.position.z,
            0, 0, 0,
            0, 0, 0,
            marker.scale, marker.scale, marker.scale,
            marker.color[0], marker.color[1], marker.color[2], marker.color[3],
            false, false, 2, false, null, null, false);
    })
});

gm.events.add("destroyWarZone", (id) => {
    mp.events.call('deleteBlip', `captureZone_${id}`);

    if (captureZone [id])
        delete captureZone [id];

});

//

let warZone = {
    time: 0,
    intervalId : null
}

const warType =  {
    Gangs: 0,
    Mafia: 1,
    OrgWarZone: 2,
    OrgHouse: 3
}

global.weaponCategory = 0;

gm.events.add("inWarZone", (type, gripType, attackName, protectingName, time, attackingCount, protectingCount, weaponsCategory, attackingPlayersInZone, protectingPlayersInZone) => {

    global.weaponCategory = weaponsCategory;

    let title = ""

    switch (type) {
        case warType.Gangs:
            title = "Война за территорию"
            break;
        case warType.Mafia:
            title = "Война за бизнес"
            break;
        case warType.OrgWarZone:
            title = "Война за зону влияния"
            break;
        case warType.OrgHouse:
            title = "Захват дома"
            break;
    }

    warZone.time = time;
    timeWarZone ();
    warZone.intervalId = setInterval(() => {
        warZone.time--;
        timeWarZone ();
    }, 1000);

    mp.gui.emmit(`window.listernEvent ('hud.war.open', ${type}, ${gripType}, ${weaponsCategory}, '${title}', '${attackName}', '${protectingName}');`);

    mp.events.call('countWarZone', attackingCount, protectingCount);
    mp.events.call('playersWarZone', attackingPlayersInZone, protectingPlayersInZone);
});

gm.events.add("outWarZone", (id, isDestroy) => {

    if (isDestroy)
        mp.events.call('destroyWarZone', id);

    mp.gui.emmit(`window.listernEvent ('hud.war.close');`);

    if (warZone.intervalId !== null)
        clearInterval(warZone.intervalId);

    warZone.intervalId = null;
    warZone.time = 0;
    global.weaponCategory = 0;
});

const timeWarZone = () => {
    const minutes = Math.trunc(warZone.time / 60);
    const seconds = warZone.time % 60;

    const timeText = `${global.formatIntZero(minutes, 2)}:${global.formatIntZero(seconds, 2)}`

    mp.gui.emmit(`window.listernEvent ('hud.war.update', 'time', '${timeText}');`);

}

gm.events.add("playersWarZone", (attackingPlayersInZone, protectingPlayersInZone) => {

    mp.gui.emmit(`window.listernEvent ('hud.war.update', 'playersCount', ${attackingPlayersInZone}, ${protectingPlayersInZone});`);

});

gm.events.add("countWarZone", (attackingCount, protectingCount, killerId, victimId, weapon) => {

    mp.gui.emmit(`window.listernEvent ('hud.war.update', 'count', ${attackingCount}, ${protectingCount});`);

    if (typeof killName !== "undefined" && !global.adminLVL)
        mp.events.call('hud.kill', killerId, victimId, weapon);
});

let warZoneInfo = []

gm.events.add("initWarZone", async (json, isSend = false) => {
    if (!isSend) {
        warZoneInfo = JSON.parse(json);
    } else if (warZoneInfo.length) {
        for (const item of warZoneInfo) {
            mp.events.call('infoWarZone', true, item[0], item[1], item[2], item[3], item[4], item[5]);
            await global.wait(1000 * 10);
        }

        mp.events.call('infoWarZone', false);
    }

});

gm.events.add("infoWarZone", async (visible, isAttack, mapName, selectTypeBattle, time, selectComposition, selectWeaponsCategory, posX, posY) => {

    if (visible === -1) {
        if (posX || posY)
            mp.events.call('createWaypoint', posX, posY);
        else
            nativeInvoke ("_START_SCREEN_EFFECT", 'MP_SmugglerCheckpoint', 2000, true);
        //
        mp.game.audio.playSoundFrontend(-1, "Zone_Team_Capture", "DLC_Apartments_Drop_Zone_Sounds", true);
        mp.gui.emmit(`window.listernEvent ('hud.war.info', true, ${isAttack}, '${mapName}', ${selectTypeBattle}, '${time}', ${selectComposition}, ${selectWeaponsCategory});`);
        await global.wait(1000 * 10);
        mp.gui.emmit(`window.listernEvent ('hud.war.info', false);`);
        return;
    }

    mp.gui.emmit(`window.listernEvent ('hud.war.info', ${visible}, ${isAttack}, '${mapName}, ${selectTypeBattle}, '${time}', ${selectComposition}, ${selectWeaponsCategory});`);
});