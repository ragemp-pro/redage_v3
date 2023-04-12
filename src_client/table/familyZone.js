gm.events.add("client.familyZoneOpen", () => {
    if (!global.antiFlood("server.familyZoneOpen", 250))
        return;

    mp.events.callRemote("server.familyZoneOpen");
});

let isOpenFamilyZones = false;

gm.events.add("client.familyZones", (_zones, _topNames, _warsJson) => {

    global.binderFunctions.GameMenuClose ();

    let zones = [];

    _zones = JSON.parse(_zones);
    _zones.forEach((item) => {
        zones.push({
            id: item [0],
            owner: item [1],
            color: item [2],
            isWar: item [3],
            attackingColor: item [4],
            protectingColor: item [5]
        })
    });

    let wars = [];
    _warsJson = JSON.parse(_warsJson);
    _warsJson.forEach((item) => {
        wars.push({
            isAttack: item [0],
            text: item [1],
            gripType: item [2],
            weaponsCategory: item [3],
            time: item [4],
        })
    });

    let viewData = {
        zones: zones,
        topNames: JSON.parse(_topNames),
        wars: wars,
    }

    mp.gui.emmit(`window.router.setView("FractionsWar", \`${JSON.stringify (viewData)}\`)`);
    global.menuOpen();
    isOpenFamilyZones = true;
});

gm.events.add("client.familyZoneClose", () => {
    if (!isOpenFamilyZones)
        return;

    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
    isOpenFamilyZones = false;
});

gm.events.add("client.familyZoneAttack", (id) => {
    if (!global.antiFlood("server.familyZoneAttack", 250))
        return;

    mp.events.callRemote("server.familyZoneAttack", id);
});