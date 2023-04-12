global.menuOpened = true;
let timeMenuPause = 0;

global.menuCheck = () => {
    if (global.menuOpened || mp.game.ui.isPauseMenuActive() || global.isBind || global.isSartMetro/* || timeMenuPause > Date.now()*/) return true;
    return false;
};

global.awaitMenuCheck = () => new Promise(async (resolve, reject) => {
    try {
        if (!global.menuCheck())
            return resolve();
        let d = 0;
        while (global.menuCheck()) {
            if (d > 5000) return resolve(translateText("Ошибка awaitMenuCheck."));
            d++;
            await global.wait (0);
        }
        return resolve();
    } 
    catch (e) 
    {
		mp.events.callRemote("client_trycatch", "player/menus", "render", e.toString());
        resolve();
    }
});

let isControls = false;
global.menuOpen = function (_isControls = false) {
    mp.gui.cursor.visible = true;

    if (_isControls)
        mp.gui.cursor.show(false, true);

    isControls = _isControls;
    global.menuOpened = true;

    //mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
}

global.menuClose = function () {
    mp.gui.cursor.show(true, true);

    mp.gui.cursor.visible = false;
    global.discordDefault ()
    global.menuOpened = false;
    isControls = false;
    global.isPopup = false;
}
/*
gm.events.add(global.renderName ["1s"], () => {
    if (mp.gui.cursor.visible && mp.game.ui.isPauseMenuActive()) {
        mp.gui.cursor.show(true, true);
        mp.gui.cursor.visible = false;
        mp.game.ui.setPauseMenuActive(false);

        mp.gui.emmit(`console.log ('renderName - ${mp.gui.cursor.visible} - ${mp.game.ui.isPauseMenuActive()}')`);
    }
});
*/

gm.events.add("render", () => {
    if (global.menuOpened && mp.game.ui.isPauseMenuActive())
        mp.game.ui.setPauseMenuActive(false);
    else if (global.ANTIANIM)
        mp.game.controls.disableControlAction(0, global.Inputs.LOOK_BEHIND, true);

    if (isControls) {
        mp.game.controls.disableAllControlActions(0);

        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_LR, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_UD, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_UP_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_DOWN_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_LEFT_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.MOVE_RIGHT_ONLY, true);
        mp.game.controls.enableControlAction(0, global.Inputs.JUMP, true);

        mp.game.controls.enableControlAction(0, global.Inputs.VEH_MOVE_LR, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_MOVE_UD, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_ACCELERATE, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_BRAKE, true);
        mp.game.controls.enableControlAction(0, global.Inputs.VEH_HANDBRAKE, true);

        if (mp.keys.isDown(global.Keys.VK_RBUTTON)) {
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LR, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UD, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_UP_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_DOWN_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_LEFT_ONLY, true);
            mp.game.controls.enableControlAction(2, global.Inputs.LOOK_RIGHT_ONLY, true);
        }
    }
});

/*gm.events.add("playerQuit", (player, exitType, reason) => {
    if (player.name === global.localplayer.name) {
        global.menuClose();
    }
});*/