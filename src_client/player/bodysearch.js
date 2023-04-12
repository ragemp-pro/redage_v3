// BODY SEARCH //
global.isBSearchActive = false;

gm.events.add('bsearch', (act) => {

    switch (act) {
        case 1:
            mp.events.callRemote('pSelected', global.entity, translateText("Посмотреть лицензии"));
            break;
        case 2:
            mp.events.callRemote('pSelected', global.entity, translateText("Посмотреть паспорт"));
            break;
        default:
            global.isBSearchActive = false;
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            break;
    }
});

gm.events.add('bsearchOpen', (data) => {
    if (global.menuCheck()) return;
    global.menuOpen();
    global.isBSearchActive = data;
    mp.gui.emmit(`window.router.setView("FractionsBSearch", ${data});`);
})