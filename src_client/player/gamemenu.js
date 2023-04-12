let gamemenu_state = false;

global.binderFunctions.gamemenu = () => {
    if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk) return;

    if(!gamemenu_state && !global.menuCheck())
    {
        gamemenu_state = true;
        mp.gui.emmit(`window.router.updateStatic("PlayerGameMenu");`);
        global.menuOpen(true);
    }
    else if(gamemenu_state)
    {
        gamemenu_state = false;
        mp.gui.emmit(`window.router.setHud();`);
        global.menuClose();
    }
};