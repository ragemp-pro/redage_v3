let isFractionMenu = false;

global.binderFunctions.open_Table = () => {
    if (global.menuCheck()) return;

    if (global.fractionId !== 0 && global.organizationId !== 0)
        global.OpenCircle(translateText("Открыть планшет"), 0);
    else if (global.fractionId === 15 || (global.fractionId === 6 && global.isLeader))
        global.OpenCircle(translateText("Открыть планшет"), 0);
    else if (global.fractionId !== 0) {
        mp.gui.emmit(`window.gameMenuView ("Fractions");`);
        if (!global.gamemenu)
            global.binderFunctions.GameMenuOpen ();
    }
    else if (global.organizationId !== 0) {
        mp.gui.emmit(`window.gameMenuView ("Organization");`);
        if (!global.gamemenu)
            global.binderFunctions.GameMenuOpen ();
    }
}

gm.events.add('client.table.open', async (usersList, vehiclesList, boardList, settings, defaultAccess, access, updateInfo, clothesList, isOrgTable = false) => {
    try 
    {
        //if (global.menuCheck()) return;
        await global.awaitMenuCheck ();
        global.menuOpen();
        mp.gui.emmit(
            `window.router.setView("FractionsMenu", {usersList: '${usersList}', vehiclesList: '${vehiclesList}', boardList: '${boardList}', settings: '${settings}', defaultAccess: '${defaultAccess}', access: '${access}', updateInfo: '${updateInfo}', clothesList: '${clothesList}', isOrgTable: '${isOrgTable}'})`
        );
        isFractionMenu = true;
        gm.discord(translateText("Изучает фракционный планшет"));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/mats", "matsOpen", e.toString());
    }
});

global.closeFractionTableMenu = () => {
    if (!isFractionMenu)
        return;
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
    isFractionMenu = false;
}

gm.events.add('client.table.rank', (isUp, name) => {//+
    mp.events.callRemote('server.table.rank', isUp, name);
});

gm.events.add('client.table.irank', (rank, name) => {//+
    mp.events.callRemote('server.table.irank', rank, name);
});

gm.events.add('client.table.call', (text, name) => {//+
    mp.events.callRemote('server.table.call', text, name);
});

gm.events.add('client.table.uninvite', (name) => {//+
    mp.events.callRemote('server.table.uninvite', name);
});

gm.events.add('client.table.invite', (text) => {//+
    mp.events.callRemote('server.table.invite', text);
});

gm.events.add('client.table.fracad', (text) => {//+
    mp.events.callRemote('server.table.fracad', text);
});

gm.events.add('client.table.ufracad', (index, text) => {//+
    mp.events.callRemote('server.table.ufracad', index, text);
});

gm.events.add('client.table.dfracad', (index) => {//+
    mp.events.callRemote('server.table.dfracad', index);
});

gm.events.add('client.table.gps', (number) => {//+
    mp.events.callRemote('server.table.gps', number);
});

gm.events.add('client.table.evacuation', (number) => {//+
    mp.events.callRemote('server.table.evacuation', number);
});

gm.events.add('client.table.gethistory', (name, pageId) => {
    mp.events.callRemote('server.table.gethistory', name, pageId);
});

gm.events.add('client.table.logs', (json) => {
    mp.gui.emmit(`window.events.callEvent("cef.table.hget", '${json}')`);
});

gm.events.add('client.table.vrank', (isUp, number) => {//+
    mp.events.callRemote('server.table.vrank', isUp, number);
});

gm.events.add('client.table.clothingSetRank', (isUp, name, gender) => {
    mp.events.callRemote('server.table.clothingSetRank', isUp, name, gender);
});

gm.events.add('client.table.startEditClothingSet', (oldName, newName, gender) => {
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.startEditClothingSet', oldName, newName, gender);
});

gm.events.add('client.table.editClothingSet', (dictionary, id, texture) => {
    mp.events.callRemote('server.table.editClothingSet', dictionary, id, texture);
});





gm.events.add('client.table.createrank', (rankName) => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.createrank', rankName);
});

gm.events.add('client.table.editrank', (index, rankName) => {//+
    mp.events.callRemote('server.table.editrank', index, rankName);
});

gm.events.add('client.table.accessdelete', (index, accessIndex) => {//+
    mp.events.callRemote('server.table.accessdelete', index, accessIndex);
});

gm.events.add('client.table.accessadd', (index, accessIndex) => {//+
    mp.events.callRemote('server.table.accessadd', index, accessIndex);
});

gm.events.add('client.table.event', (index) => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.event', index);
});




gm.events.add('client.table.dellorg', () => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.dellorg');
});



gm.events.add('client.table.dellrank', (index) => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.dellrank', index);
});

gm.events.add('client.table.sellcar', (number) => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.sellcar', number);
});

gm.events.add('client.table.leave', () => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.leave');
});

gm.events.add('client.table.defaultrank', () => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.defaultrank');
});

gm.events.add('client.table.defaultvrank', () => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.defaultvrank');
});

gm.events.add('client.table.upgrade', (type) => {//+
    mp.events.callRemote('server.table.upgrade', type);
});

gm.events.add('client.table.tuning', () => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.tuning');
});

gm.events.add('client.table.dron', () => {//+
    global.closeFractionTableMenu ();
    mp.events.callRemote('server.table.dron');
});

gm.events.add('client.table.reprimand', (uuid, name, text) => {
    mp.events.callRemote('server.table.reprimand', uuid, name, text);
});