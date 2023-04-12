
let isInterface = false;

global.input = {
    head: "",
    desc: "",
    len: "",
    cBack: "",
    set: function (h, d, l, c) {
        this.head = h, this.desc = d;
        this.len = l, this.cBack = c;
        //if (global.menuCheck()) return;
        mp.gui.emmit(
            `window.router.setPopUp("PopupInput", {title: '${this.head}', plholder: '${this.desc}', length: ${this.len}});`
        );
        mp.gui.cursor.visible = true;
        isInterface = false;
        if (!global.menuCheck ()) {
            global.menuOpen();
            isInterface = true;
            global.isPopup = true;
        }
    },
    open: function () {
        if (global.menuCheck()) return;
        mp.gui.emmit(`window.router.setPopUp("PopupInput")`);
        global.menuOpen();
        //mp.events.call('startScreenEffect', "MenuMGHeistIn", 1, true);
    },
    close: function () {
        mp.gui.emmit(`window.router.setPopUp()`);
        if (isInterface) 
            global.menuClose();
        isInterface = false;
        global.isPopup = false;
        //mp.events.call('stopScreenEffect', "MenuMGHeistIn");
    }
};

gm.events.add('input', (text) => {
    try
    {
        if (input.cBack == "") return;

        if (input.cBack == "join_private_lobby")
            mp.events.call('airsoft_joinPrivateLobby_client', 2, text);
        else if (input.cBack == "mafia_join_private_lobby")
            mp.events.call('mafia_joinPrivateLobby_client', 2, text);
        else if (input.cBack == "tanks_join_private_lobby")
            mp.events.call('tanks_joinPrivateLobby_client', 2, text);
        else if (input.cBack == "setCruise")
            mp.events.call('setCruiseSpeed', text);
        else if (input.cBack == "boombox")
            mp.events.callRemote('setFirstBoomboxURL', text);
        else if (input.cBack == "update_boombox_url")
            mp.events.callRemote('updateBoomboxURL', text);
        else if (input.cBack == 'take_frac_ammo')
            mp.events.callRemote('server.takeFractionAmmo', text);
        else if (input.cBack == "sendReportFromClientInput")
            mp.events.callRemote('sendReportFromClient', text);
        else 
            mp.events.callRemote('inputCallback', input.cBack, text);
        
            input.cBack = "";
        input.close();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/input", "input", e.toString());
    }
});

gm.events.add('openInput', (h, d, l, c) => {
    //if (global.menuCheck()) return;
    input.set(h, d, l, c);
    //input.open();
})

gm.events.add('closeInput', () => {
    input.close();
})