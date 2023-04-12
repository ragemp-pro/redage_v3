var text = "";
var canback = "";

let isInterface = false;
global.openDialog = () => {
    mp.gui.emmit(`window.router.setPopUp("PopupConfirm", {title: "${translateText("Подтверждение")}", text: "${text}"});`);
    mp.gui.cursor.visible = true;
    isInterface = false;    
    if (!global.menuOpened) {
        global.menuOpen(true);
        isInterface = true;
        global.isPopup = true;
    }
    //mp.events.call('startScreenEffect', "MenuMGHeistIn", 1, true);
}

global.closeDialog = () => {
    mp.gui.emmit('window.router.setPopUp()');

    if (isInterface)
        global.menuClose();
    else if (isOpenPopupList)
        mp.events.callRemote('popup.list.callback', null);

    isInterface = false;
    global.isPopup = false;
    isOpenPopupList = false;
    //mp.events.call('stopScreenEffect', "MenuMGHeistIn");
}

gm.events.add('openDialog', (cback, ctext) => {
    // Добавить title!!!
    canback = cback;
    text = ctext;
    global.openDialog();
})

gm.events.add('client:OnDialogCallback', (state) => {
    if (canback == 'tuningbuy') mp.events.call('client.custom.sbuy', state);
    else mp.events.callRemote('dialogCallback', canback, state);
    global.closeDialog();
})

//

gm.events.add('openHospitalDialog', (ctext) => {
    mp.gui.emmit(`window.router.setPopUp("PopupDeath", {title: "${translateText("Подтверждение")}", text: "${ctext}"});`);
    mp.gui.cursor.visible = true;
    isInterface = false;    
    if (!global.menuOpened) {
        global.menuOpen(true);
        gm.discord('Общается с врачами в больнице');
        isInterface = true;
        global.isPopup = true;
    }
})

gm.events.add('client:OnHospitalDialogCallback', (state) => {
    mp.events.callRemote('server:OnHospitalDialogCallback', state);
    global.closeDialog();
})


//list

let isOpenPopupList = false;

gm.events.add('popup.list.open', (header, list) => {
    mp.gui.emmit(`window.router.setPopUp("PopupSelect", {title: '${header}', elements: '${list}'});`);
    mp.gui.cursor.visible = true;
    isInterface = false;
    isOpenPopupList = true;
    if (!global.menuOpened) {
        global.menuOpen(true);
        isInterface = true;
        global.isPopup = true;
    }
})

gm.events.add('popup.list.selected', (listItem) => {
    mp.events.callRemote('popup.list.callback', listItem);
    isOpenPopupList = false;
    global.closeDialog ();
})

