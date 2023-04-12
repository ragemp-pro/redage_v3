gm.events.add('client.animationStore.animFavorites', (json) => {
    mp.gui.emmit(`window.animationStore.initAnimFavorites('${json}')`);
});

gm.events.add('client.animationStore.animBind', (json) => {
    mp.gui.emmit(`window.animationStore.initAnimBind('${json}')`);
});


let toggled = false;
let isPlay = false;

global.binderFunctions.c_animation = async () => {
    if (!toggled) return;
    mp.gui.emmit(`window.router.setHud();`)
    toggled = false;    
    await global.wait(50);
    global.menuClose();
}

global.binderFunctions.o_animation = (isCef = false) => {// Animations selector
    if (!isCef)
        return;
    else if (
        !global.loggedin || 
        global.chatActive || 
        global.editing ||
        global.cuffed ||
        global.menuCheck() || 
        global.isDeath === true || 
		global.isDemorgan ||
        global.localplayer.isInAnyVehicle(false)) return;
    mp.gui.emmit(`window.router.setView('PlayerAnimations', ${global.ANTIANIM});`);
    global.menuOpen(true);
    toggled = true;
};

gm.events.add('client.animation.open', () => {
	global.binderFunctions.o_animation(true);
    gm.discord(translateText("Изучает список анимаций"));
});

gm.events.add('client.animation.play', (item) => {
    try
    {
        if (!global.loggedin || 
            global.chatActive || 
            global.editing ||
            global.cuffed ||
            global.isDeath === true || 
            global.isDemorgan ||
            global.startedMining === true ||
            global.startedMafiaGame ||
            global.localplayer.isInAnyVehicle(false)) return;
        else if (global.ANTIANIM && !isPlay) 
            return;

        mp.events.callRemote("server.animation.play", item, true);
        mp.gui.emmit(`window.UpdateButtonText('hud__icon-Anim', '${translateText('Чтобы сбросить анимацию, нажмите "Пробел" дважды.')}');`);
        global.binderFunctions.c_animation ();
        gm.discord('ФЛЕКСИТ');
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/animation", "client.animation.play", e.toString());
    }
});

gm.events.add('client.animation.stop', () => {
    try
    {
        if (!isPlay || global.startedMafiaGame)
            return;
        if (!global.ANTIANIM) {
            global.binderFunctions.o_animation(true);
            return;
        }
        mp.gui.emmit(`window.UpdateButtonText('', '');`);
        mp.events.callRemote("server.animation.play", -1, true);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/animation", "client.animation.stop", e.toString());
    }
});

gm.events.add('client.animation.favorite', (json) => {
	mp.events.callRemote("server.animation.favorite", json);
});

gm.events.add('client.animation.bind', (json) => {
	mp.events.callRemote("server.animation.bind", json);
});

gm.events.add('client.animation.isPlayer', (toggled) => {
    isPlay = toggled;
});

gm.events.add("playerEnterVehicle", (vehicle, seat) => {
    try 
	{
        global.localplayer.setHelmet(false);
        
        if (isPlay == 1) {
            mp.gui.emmit(`window.UpdateButtonText('', '');`);
	        mp.events.callRemote("server.animation.play", -1, false);
        }
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/animation", "playerStartEnterVehicle", e.toString());
	}
});