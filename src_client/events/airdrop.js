const 
    covercrate_model = ["ex_prop_adv_case_sm", "ex_prop_adv_case_sm_02", "ex_prop_adv_case_sm_03", "ex_prop_adv_case_sm_flash", "ex_prop_adv_case_sm_flash"],
    parachute_model = "p_cargo_chute_s";
let globalFightData = {};

const lodDist = 1000;

gm.events.add("client.fight.create", async (id, propId, position, time) => {
    try
    {
        mp.events.call("client.fight.dell", id);
        await global.loadModel(covercrate_model [propId]);
        await global.loadModel(parachute_model);

        const fightData = {};
        //fightData.crateSpawn = c;
        const posZ = -0.19;
        fightData.object = mp.game.object.createObject(mp.game.joaat(covercrate_model [propId]), position.x, position.y, position.z + (27.5 * time), true, true, true);
        Natives.SET_ENTITY_LOD_DIST (fightData.object, lodDist);
        Natives.ACTIVATE_PHYSICS (fightData.object);
        Natives.SET_DAMPING (fightData.object, 2, 3063 - 3062.1);
        Natives.SET_ENTITY_VELOCITY (fightData.object, 0, 0, posZ);
        Natives.SET_ENTITY_INVINCIBLE (fightData.object, true);
        Natives.SET_ENTITY_PROOFS (fightData.object, true, true, true, true, true, true, true, true, true);

        fightData.parachute = mp.game.object.createObject(mp.game.joaat(parachute_model), position.x, position.y, position.z + (27.5 * time), true, true, true);
        Natives.SET_ENTITY_LOD_DIST (fightData.parachute, lodDist);
        Natives.SET_ENTITY_VELOCITY (fightData.parachute, 0, 0, posZ);

        fightData.sound = Natives.GET_SOUND_ID ();
        mp.game.audio.playSoundFromEntity(fightData.sound, "Crate_Beeps", fightData.object, "MP_CRATE_DROP_SOUNDS", true, 0);
        Natives.ATTACH_ENTITY_TO_ENTITY (fightData.parachute, fightData.object, 0, 0, 0, 3860.35 - 3860, 0, 0, 0, false, false, false, false, 2, true);
        Natives.FREEZE_ENTITY_POSITION (fightData.object, false);
        
        globalFightData[id] = fightData;

        setTimeout(() => {
            const _fightData = globalFightData[id];
            if (_fightData) {
                if (_fightData.parachute && Natives.DOES_ENTITY_EXIST (_fightData.parachute)) {
                    mp.game.object.deleteObject(_fightData.parachute);
                    _fightData.parachute = null;
                }
                const shape = mp.colshapes.newSphere(
                    position.x, position.y, position.z,
                    2.5,
                    100
                );
        
                shape.FightId = id;
                globalFightData[id].share = shape;

                position.z -= 0.3;
                mp.events.call("client.particleEffect", "core", "exp_grd_flare", position, 30000);
            }
        }, 1000 * (8.33 * time));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airdrop", "client.fight.create", e.toString());
    }
});

gm.events.add("client.fight.dell", (id) => {
    try
    {
        const fightData = globalFightData[id];
        if (fightData) {
            if (fightData.object && Natives.DOES_ENTITY_EXIST (fightData.object)) 
                mp.game.object.deleteObject(fightData.object);
            if (fightData.parachute && Natives.DOES_ENTITY_EXIST (fightData.parachute)) 
                mp.game.object.deleteObject(fightData.parachute);
            if (fightData.sound) {
                mp.game.audio.stopSound(fightData.sound);
                mp.game.audio.releaseSoundId(fightData.sound);
            }
            if (fightData.share) {
                fightData.share.destroy();
            }
            delete globalFightData[id];
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airdrop", "client.fight.dell", e.toString());
    }
});

gm.events.add("airdrop.updateTeamsInfo", (stats) => {
    try
    {
        stats = JSON.parse(stats);

        let data = [
            {
                name: stats[0] ? `${stats[0].Key} (${stats[0].Value.TeammatesInZone})` : translateText("Нет"),
                score: stats[0] ? stats[0].Value.TeamFrags : 0
            },
            {
                name: stats[1] ? `${stats[1].Key} (${stats[1].Value.TeammatesInZone})` : translateText("Нет"),
                score: stats[1] ? stats[1].Value.TeamFrags : 0
            },
            {
                name: stats[2] ? `${stats[2].Key} (${stats[2].Value.TeammatesInZone})` : translateText("Нет"),
                score: stats[2] ? stats[2].Value.TeamFrags : 0
            }
        ];

        mp.gui.emmit(`window.airsoftFunctions(4, '${JSON.stringify(data)}');`);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airdrop", "airdrop.updateTeamsInfo", e.toString());
    }
});

global.selectFightId = -1;
gm.events.add('playerEnterColshape', (shape) => {
    if (shape && shape.FightId !== undefined && global.selectFightId === -1) {
        mp.events.call('hud.oEnter', 1);
        global.selectFightId = shape.FightId;
        global.canHackAirdrop = false;
        mp.events.callRemote("CheckAirdropLockStatus", global.selectFightId);
    }
});

gm.events.add('playerExitColshape', (shape) => {
    if (shape && shape.FightId !== undefined && global.selectFightId !== -1 && shape.FightId === global.selectFightId) {
        mp.events.call('hud.cEnter');
        global.selectFightId = -1;
        global.canHackAirdrop = false;
    }
});
// Взлом аирдропа:

const airdropLock = {
    interval: undefined,
    health: 100
};

gm.events.add("client.updateAirdropHackStatus", (status, health) => {
    global.canHackAirdrop = status;
    airdropLock.health = health;
});

gm.events.add('airdrop_hackStatus', (state) => {
    if (state == 1) {
        global.localplayer.freezePosition(true);

        if (airdropLock.interval !== undefined) {
            clearInterval(airdropLock.interval);
            airdropLock.interval = undefined;
        }

        airdropLock.interval = setInterval(() => {
            airdropLock.health -= 1;
            
            if (airdropLock.health <= 0) {
                airdropLock.health = 0;

                if (airdropLock.interval !== undefined) {
                    clearInterval(airdropLock.interval);
                    airdropLock.interval = undefined;
                }

                global.canHackAirdrop = false;
                global.localplayer.freezePosition(false);
                mp.events.callRemote('AirdropChangeLockStatus', global.selectFightId, airdropLock.health);
            }
        }, 1000);
    }
    else if (state == 2) {
        if (airdropLock.interval !== undefined) {
            clearInterval(airdropLock.interval);
            airdropLock.interval = undefined;
        }

        global.localplayer.freezePosition(false);
        mp.events.callRemote('AirdropChangeLockStatus', global.selectFightId, airdropLock.health);
    }
});

gm.events.add("render", () => {
    if (global.selectFightId !== -1 && global.canHackAirdrop === true && mp.game.controls.isControlJustPressed(0, 38)) {
        mp.events.callRemote('server.fight.player.start.hack', global.selectFightId);
    }

    if (global.canHackAirdrop === true && mp.game.controls.isControlJustReleased(0, 38)) {
        mp.events.call('airdrop_hackStatus', 2);
    }
});

// Создание зоны:

const airdrop = {
    zone_marker: undefined,
    zone_shape: undefined,
    blipId: false
};

gm.events.add("client.airdropZone.create", (position, radius, color, airdrop_minutes) => {
    try
    {
        if (!airdrop.zone_marker) {
            airdrop.zone_marker = mp.markers.new(28, new mp.Vector3(parseFloat(position.x), parseFloat(position.y), parseFloat(position.z) - 1.25), 125.0, {
                visible: true,
                color: [255, 0, 0, 90],
                dimension: -1
            });
        }

        if (!airdrop.zone_shape) {
            airdrop.zone_shape = mp.colshapes.newSphere(parseFloat(position.x), parseFloat(position.y), parseFloat(position.z), 125.0, -1);
        }

        if (!airdrop.blipId) {
            airdrop.blipId = mp.game.ui.addBlipForRadius(parseFloat(position.x), parseFloat(position.y), parseFloat(position.z), parseFloat(radius));
            nativeInvoke ("SET_BLIP_SPRITE", airdrop.blipId, 9);
            nativeInvoke ("SET_BLIP_ALPHA", airdrop.blipId, 100);
            nativeInvoke ("SET_BLIP_COLOUR", airdrop.blipId, color);
        }

        mp.events.call("hud.event", "AirDrop", "Внимание!", `AirDrop будет сброшен через ${airdrop_minutes} мин`, "png", 30 * 1000);

        //_subTitle, _title, _desc, _image, timeWait
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airdrop", "client.airdropZone.create", e.toString());
    }
});

gm.events.add("client.blipZone.remove", () => {
    try
    {
        clearAirdropZoneInfo();

        if (airdrop.blipId) {
            mp.game.ui.removeBlip (airdrop.blipId);
            airdrop.blipId = false;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airdrop", "client.blipZone.remove", e.toString());
    }
});

gm.events.add('playerEnterColshape', (shape) => {
    if (shape === airdrop.zone_shape) {
        global.inAirdropZoneShape = true;
        mp.events.callRemote('AirdropChangePlayerDimension', 1);
    }
});

gm.events.add('playerExitColshape', (shape) => {
    if (shape === airdrop.zone_shape) {
        global.inAirdropZoneShape = false;
        mp.events.callRemote('AirdropChangePlayerDimension', 2);
    }
});

function clearAirdropZoneInfo() {
    if (airdrop.zone_marker !== undefined) {
        airdrop.zone_marker.destroy();
        airdrop.zone_marker = undefined;
    }

    if (airdrop.zone_shape !== undefined) {
        airdrop.zone_shape.destroy();
        airdrop.zone_shape = undefined;
    }

    if (airdrop.blipId) {
        mp.game.ui.removeBlip (airdrop.blipId);
        airdrop.blipId = false;
    }
    
    if (global.inAirdropZoneShape === true && global.dimension === 100) {
        global.inAirdropZoneShape = false;
        mp.events.callRemote('AirdropChangePlayerDimension', 2);
    }

    mp.gui.emmit(`window.airsoftFunctions(0);`);
}