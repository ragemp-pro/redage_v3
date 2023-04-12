const
    clientName = "client.policecomputer.",
    rpcName = "rpc.policecomputer.",
    serverName = "server.policecomputer.";

gm.events.add(clientName + "open", async () => {
    await global.awaitMenuCheck ();
    global.menuOpen();
    mp.gui.emmit(
        `window.router.setView("FractionsBortovoi")`
    );
    mp.discord.update('Изучает боттовой компьютер', `на RedAge под ID ${global.localplayer.remoteId}`);
});

gm.events.add(clientName + "close", async () => {
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
});

gm.events.add(clientName + "init", (json) => {
    personsArray = JSON.parse(json);
});

rpc.register(rpcName + "searchPerson", (name) => {
    const advert = adsArray.find(el => el.ID == addID)
    if (advert) {
        if(advert.Editor && advert.Editor.length && advert.Editor !== global.localplayer.name)
            return false;

        mp.events.callRemote(serverName + "take", addID);
        selectedID = addID;
        return JSON.stringify (advert);
    }
    return false;
});