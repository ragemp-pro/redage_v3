let isOpenRieltagency = false;

gm.events.add('client.rieltagency.open', (buyPrice, houseData, allHouse, businessData, allBusiness) => {

    if (global.menuCheck()) return;
    isOpenRieltagency = true;
    global.menuOpen();
    gm.discord(translateText("В риэлторском агентстве"));
    mp.gui.emmit(
        `window.router.setView("HouseRielt", {buyPrice: ${buyPrice}, houseData: '${houseData}', allHouse: ${allHouse}, businessData: '${businessData}', allBusiness: ${allBusiness}})`
    );
});

gm.events.add('client.rieltagency.close', (isServerRemote = true) => {
    if (!isOpenRieltagency)
        return;

    if (isServerRemote)
        mp.events.callRemote('server.rieltagency.close');

    isOpenRieltagency = false;

    global.menuClose();
    mp.gui.emmit(`window.router.setHud()`);
});

gm.events.add('client.rieltagency.buy', (id, type) => {
    if (!isOpenRieltagency)
        return;
        
    mp.events.call('client.rieltagency.close');
    mp.events.callRemote('server.rieltagency.buy', id, type);
});

gm.events.add('client.rieltagency.addRange', (houseData, businessData) => {
    if (!isOpenRieltagency)
        return;
        
	mp.gui.emmit(`window.events.callEvent("cef.rieltagency.addHouse", '${houseData}')`);
	mp.gui.emmit(`window.events.callEvent("cef.rieltagency.addBusiness", '${businessData}')`);
});

let rieltagencyBlips = {}
gm.events.add('client.rieltagency.addBlip', (blipId, color, id, posX, posY, name) => {
    mp.events.call('client.rieltagency.delBlip', blipId, id);

    rieltagencyBlips [`${blipId}_${id}`] = mp.blips.new(blipId, new mp.Vector3(posX, posY), { alpha: 255, color: color, name: name });
    mp.events.call('createWaypoint', posX, posY);
});

gm.events.add('client.rieltagency.delBlip', (blipId, id) => {
    if (rieltagencyBlips [`${blipId}_${id}`]) {
        rieltagencyBlips [`${blipId}_${id}`].destroy();
        delete rieltagencyBlips [`${blipId}_${id}`];
    }
});