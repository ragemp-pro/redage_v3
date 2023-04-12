gm.events.add('eventsMenuShow', () => {
    try
    {
        if (global.menuCheck() || global.eventsMenuActive) return;
        mp.gui.emmit(`window.router.setView("GamesOtherMain")`);
        global.menuOpen();
        global.eventsMenuActive = true;
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "events/eventsMenu", "eventsMenuShow", e.toString());
    }
});

gm.events.add('eventsMenuHide', () => {
    try
    {
        if (!global.eventsMenuActive) return;
        mp.gui.emmit(`window.router.setHud();`);
        global.menuClose();
        global.eventsMenuActive = false;
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "events/eventsMenu", "eventsMenuHide", e.toString());
    }
});

gm.events.add('selectEventClient', (index) => {
    mp.events.call('eventsMenuHide');
    mp.events.callRemote('selectEventServer', index);
});