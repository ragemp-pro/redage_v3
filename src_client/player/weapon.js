gm.events.add('client.weapon.give', (weaponHash, ammo, isReload = false, ItemId = 0) => {
	try
	{
		weaponHash = parseInt(weaponHash);
		ammo = parseInt(ammo);
		ammo = ammo >= 9999 ? 9999 : ammo;
		
		const player = global.localplayer.handle;
		if (!isReload) mp.game.invoke(getNative("REMOVE_ALL_PED_WEAPONS"), player, true);
		else ammo += global.weaponData.ammo;
		//
		mp.game.invoke(getNative("SET_PED_AMMO"), player, weaponHash, 0);
		//
		mp.game.invoke(getNative("SET_AMMO_IN_CLIP"), player, weaponHash, 0);
		//
		global.weaponData.weapon = weaponHash;
		global.weaponData.group = mp.game.weapon.getWeapontypeGroup(global.weaponData.weapon);
		global.weaponData.isMelee = [global.weaponGroup.Unarmed, global.weaponGroup.Melee].includes(global.weaponData.group);
		global.weaponData.ammo = ammo;
		
		mp.gui.emmit(`window.hudStore.weaponItemId (${ItemId})`);
		mp.gui.emmit(`window.hudStore.clipSize (${mp.game.weapon.getWeaponClipSize(weaponHash)})`);

		if (!isReload) 
		{
			mp.gui.emmit(`window.hudStore.ammo (${ammo})`);
			mp.game.invoke(getNative("GIVE_WEAPON_TO_PED"), player, weaponHash, ammo, false, true);
			return;
		}
		mp.game.invoke(getNative("MAKE_PED_RELOAD"), player);
		setTimeout(() => 
		{
			if (global.weaponData.weapon != weaponHash) return;
			mp.gui.emmit(`window.hudStore.ammo (${ammo})`);
			mp.game.invoke(getNative("GIVE_WEAPON_TO_PED"), player, weaponHash, ammo, false, true);
		}, 1000);
		
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weapon", "client.weapon.give", e.toString());
    }
});

global.IsWeaponSniper = function() {
	if(global.weaponData.weapon == 100416529 || global.weaponData.weapon == 205991906 || global.weaponData.weapon == -952879014 || global.weaponData.weapon == 177293209 || global.weaponData.weapon == 1785463520) return true;
	return false;
}

global.binderFunctions.playerReload = () => {// R key
	try {
		if (!global.loggedin || global.chatActive || new Date().getTime() - global.lastCheck < 1000 || mp.gui.cursor.visible) return;
		//var current = global.getCurrentWeapon();
		if (global.weaponData.weapon == -1569615261 || global.weaponData.weapon == 911657153) return;
		if (mp.game.weapon.getWeaponClipSize(global.weaponData.weapon) == global.weaponData.ammo) return;
		mp.events.callRemote("server.weapon.reload", global.pInt (global.weaponData.ammo));
		global.lastCheck = new Date().getTime();
	} catch(e) { }
};

global.binderFunctions.changeweap_1 = () => {// 1 key
    if (!global.binocularsEnabled && (!global.loggedin || global.chatActive || global.menuCheck () || mp.gui.cursor.visible || global.isDemorgan == true || global.attachedtotrunk || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0))) return;

	if (!global.antiFlood("changeweap", 2000))
		return;

	updateWeaponShotAmmo ();
	mp.events.callRemote('changeweap', 1, global.pInt (global.weaponData.ammo));
    global.lastCheck = new Date().getTime();
};

global.binderFunctions.changeweap_2 = () => {// 2 key
    if (!global.binocularsEnabled && (!global.loggedin || global.chatActive || global.menuCheck () || mp.gui.cursor.visible || global.isDemorgan == true || global.attachedtotrunk || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0))) return;

	if (!global.antiFlood("changeweap", 2000))
		return;

	updateWeaponShotAmmo ();
	mp.events.callRemote('changeweap', 2);
    global.lastCheck = new Date().getTime();
};

global.binderFunctions.changeweap_3 = () => {// 3 key
    if (!global.binocularsEnabled && (!global.loggedin || global.chatActive || global.menuCheck () || mp.gui.cursor.visible || global.isDemorgan == true || global.attachedtotrunk || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0))) return;

	if (!global.antiFlood("changeweap", 2000))
		return;

	updateWeaponShotAmmo ();
	mp.events.callRemote('changeweap', 3);
    global.lastCheck = new Date().getTime();
};

global.binderFunctions.changeweap_4 = () => {// 4 key
    if (!global.binocularsEnabled && (!global.loggedin || global.chatActive || global.menuCheck () || mp.gui.cursor.visible || global.isDemorgan == true || global.attachedtotrunk || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0))) return;

	if (!global.antiFlood("changeweap", 2000))
		return;

	updateWeaponShotAmmo ();
	mp.events.callRemote('changeweap', 4);
    global.lastCheck = new Date().getTime();
};
global.binderFunctions.changeweap_5 = () => {// 5 key
    if (!global.binocularsEnabled && (!global.loggedin || global.chatActive || global.menuCheck () || mp.gui.cursor.visible || global.isDemorgan == true || global.attachedtotrunk || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0))) return;

	if (!global.antiFlood("changeweap", 500))
		return;

	updateWeaponShotAmmo ();
	mp.events.callRemote('changeweap', 5);
    global.lastCheck = new Date().getTime();
};

gm.events.add('client.weapon.take', (sendback) => {
	try
	{
		if(sendback && global.pInt (global.weaponData.ammo) > 0)
			mp.events.callRemote("server.weapon.ammoin", global.weaponData.weapon, global.pInt (global.weaponData.ammo));

		global.weaponData.ammo = 0;
		global.weaponData.weapon = -1569615261;
		global.weaponData.group = mp.game.weapon.getWeapontypeGroup(global.weaponData.weapon);
		global.weaponData.isMelee = [global.weaponGroup.Unarmed, global.weaponGroup.Melee].includes(global.weaponData.group);
		mp.gui.emmit(`window.hudStore.ammo (0)`);
		mp.gui.emmit(`window.hudStore.clipSize (0)`);
		mp.game.invoke(getNative("REMOVE_ALL_PED_WEAPONS"), global.localplayer.handle, true);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weapon", "client.weapon.take", e.toString());
    }
});

let countWeaponShotAmmo = 0;

const sendWeaponShotAmmo = (gm.createTimeout)(() => {
	updateWeaponShotAmmo ();
}, 1000);

gm.events.add('playerWeaponShot', function (targetPosition, targetEntity) {
	try
	{
		if (global.weaponData.weapon == 911657153) return;
		if (global.weaponData.ammo <= 0) return;
		
		mp.game.cam.shakeGameplayCam('SMALL_EXPLOSION_SHAKE', 0.005);
		global.weaponData.ammo--;
		mp.gui.emmit(`window.hudStore.ammo (${global.weaponData.ammo})`);
		
		if (global.weaponData.ammo <= 0) 
		{
			global.weaponData.ammo = 0;
			localplayer.taskSwapWeapon(false);
			mp.gui.emmit(`window.hudStore.ammo (0)`);
			mp.gui.emmit(`window.hudStore.clipSize (0)`);
		}

		if (!global.binocularsEnabled && (!global.loggedin || global.chatActive || global.menuCheck () || mp.gui.cursor.visible || global.isDemorgan == true || global.attachedtotrunk || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0)))
			return;

		countWeaponShotAmmo++;
		sendWeaponShotAmmo ();
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weapon", "playerWeaponShot", e.toString());
    }
});

const updateWeaponShotAmmo = () => {
	if (!countWeaponShotAmmo)
		return;

	mp.events.callRemote('server.weaponShot', countWeaponShotAmmo);
	countWeaponShotAmmo = 0;
}

gm.events.add("removeAllWeapons", function () {
	try
	{
		global.weaponData.ammo = 0;
		global.weaponData.weapon = -1569615261;
		global.weaponData.group = mp.game.weapon.getWeapontypeGroup(global.weaponData.weapon);
		global.weaponData.isMelee = [global.weaponGroup.Unarmed, global.weaponGroup.Melee].includes(global.weaponData.group);
		mp.gui.emmit(`window.hudStore.ammo (0)`);
		mp.gui.emmit(`window.hudStore.clipSize (0)`);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weapon", "removeAllWeapons", e.toString());
    }
});

gm.events.add("playerDeath", function (player, reason, killer) {
	try
	{
		if (!global.loggedin) return;
        else if (player !== global.localplayer) return;
		global.weaponData.ammo = 0;
		global.weaponData.weapon = -1569615261;
		global.weaponData.group = mp.game.weapon.getWeapontypeGroup(global.weaponData.weapon);
		global.weaponData.isMelee = [global.weaponGroup.Unarmed, global.weaponGroup.Melee].includes(global.weaponData.group);
		mp.gui.emmit(`window.hudStore.ammo (0)`);
		mp.gui.emmit(`window.hudStore.clipSize (0)`);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weapon", "playerDeath", e.toString());
    }
});

var resistStages = {
    0: 0.0,
    1: 0.05,
    2: 0.07,
    3: 0.1,
};

gm.events.add("setResistStage", function (stage) {
    mp.game.player.setMeleeWeaponDefenseModifier(0.25 + resistStages[stage]);
    mp.game.player.setWeaponDefenseModifier(1.3 + resistStages[stage]);
});