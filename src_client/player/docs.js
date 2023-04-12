// DOCS //
let showDocs = false;

gm.events.add('passport', (data) => {
    try
    {
        if (global.menuCheck() && !global.isBSearchActive || showDocs) return;
        mp.gui.emmit(`window.router.setView("PlayerPassport", ${data})`);
        gm.discord(translateText("Смотрит паспорт"));
        showDocs = true;
        global.menuOpen();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/docs", "passport", e.toString());
    }
});

gm.events.add('licenses', (data) => {
    try
    {
        if (global.menuCheck() && !global.isBSearchActive || showDocs) return;
        mp.gui.emmit(`window.router.setView("PlayerLicense", ${data})`);
        gm.discord(translateText("Смотрит лицензии"));
        
        showDocs = true;
        global.menuOpen();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/docs", "licenses", e.toString());
    }
});

gm.events.add('dochide', () => {
    try
    {
        if (!global.isBSearchActive) {
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
        } else {
            mp.gui.emmit(`window.router.setView("FractionsBSearch", ${global.isBSearchActive});`);        
        }
        showDocs = false;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/docs", "dochide", e.toString());
    }
});

gm.events.add('client.docs', (pageId, data) => {
    try
    {
        if (global.menuCheck() && !global.isBSearchActive || showDocs) return;
        mp.gui.emmit(`window.router.setView("PlayerDocumets", {page: '${pageId}', data: ${data}})`);
        gm.discord(translateText("Смотрит документы"));
        showDocs = true;
        global.menuOpen();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/docs", "client.docs", e.toString());
    }
});

global.CloseDocs = () => {
    if (!showDocs) return;
    if (!global.isBSearchActive) {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud();`);
    } else {
        mp.gui.emmit(`window.router.setView("FractionsBSearch", ${global.isBSearchActive});`);
    }
    showDocs = false;
}