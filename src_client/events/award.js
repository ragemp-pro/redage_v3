let IsCompensation = false;

gm.events.add("client.everydayawards", async (isInit, RealDay, json, WeekTime, donate) => {
    try
    {
		if (global.menuCheck() && !global.gamemenu) return;
        IsCompensation = isInit;
        mp.gui.emmit(`window.gameMenuView ("EverydayReward");`);
        gm.discord(translateText("Изучает ежедневные награды"));
        await global.wait(50);       
		mp.gui.emmit(`window.events.callEvent("cef.everydayreward.init", ${RealDay}, '${json}', ${WeekTime}, ${donate})`);
        if (!global.gamemenu) global.binderFunctions.GameMenuOpen ();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/award", "client.everydayawards", e.toString());
    }
});

gm.events.add("client.everydayawards.close", () => {
    try
    {
        if (IsCompensation)
            mp.events.callRemote('IsCompensation');

        IsCompensation = false;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/award", "client.everydayawards.close", e.toString());
    }
});

gm.events.add("client.everydayawards.take", () => {
    mp.events.callRemote('server.everydayaward.take')
    global.binderFunctions.GameMenuClose ()
});

gm.events.add("client.everydayawards.checkbox", () => {
    mp.events.callRemote('server.everydayaward.checkbox')
});

gm.events.add("client.everydayawards.open", () => {
    mp.events.callRemote('server.everydayaward.open', false)
});

