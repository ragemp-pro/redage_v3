global.isAdmin = false;
global.adminLVL = 0;

mp.events.addDataHandler("ALVL", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) {
			global.adminLVL = Number (value);
			global.isAdmin = global.adminLVL != 0;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "ALVL", e.toString());
	}
});

mp.events.addDataHandler("REDNAME", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0) {
			if (value)
				entity.setAlpha(100);
			else
				entity.setAlpha(255);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "REDNAME", e.toString());
	}
});

global.fractionId = 0;



mp.events.addDataHandler("fraction", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) {
			global.fractionId = value;
			mp.events.call('client.charStore.FractionID', value);
			if (!Number (value)) {
				mp.events.call('client.charStore.FractionLVL', 0);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "fraction", e.toString());
	}
});

global.isLeader = false;

mp.events.addDataHandler("leader", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) {
			mp.events.call('client.charStore.IsLeader', value);
			global.isLeader = value;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "leader", e.toString());
	}
});

mp.events.addDataHandler("vmuted", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) {
			mp.gui.emmit(`window.hudStore.isMute (${value})`);
			global.binderFunctions.disableVoice();
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "vmuted", e.toString());
	}
});


global.organizationId = 0;

mp.events.addDataHandler("organization", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) {
			global.organizationId = value;
			mp.events.call('client.charStore.OrganizationID', value);
			if (!Number (value)) {
				mp.events.call('client.charStore.OrganizationLVL', 0);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "organization", e.toString());
	}
});

global.isDeath = false;

mp.events.addDataHandler("InDeath", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity === global.localplayer) {
			global.isDeath = value;
		}

		if (entity !== global.localplayer)
			gm.createPlayerBlip (entity);

		else {
			const deadAnim = [
				"dead_a",
				"dead_b",
				"dead_c",
				"dead_d",
				"dead_f",
				"dead_e",
				"dead_g",
				"dead_h",
			][Math.floor(8 * Math.random())];

			const deathInterval = setInterval(async () => {
				if (!mp.players.local.getVariable("InDeath")) {
					gm.stopAnimation(localplayer, "dead", deadAnim);
					clearInterval(deathInterval)
					return;
				}

				if (!mp.players.local.isPlayingAnim("dead", deadAnim, 3))
					gm.playAnimation (localplayer, "dead", deadAnim, 1 / 0.3, gm.animationFlags.Loop);

			}, 100);
		}


	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "InDeath", e.toString());
	}
});

global.isDeaf = false;

mp.events.addDataHandler("isDeaf", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) 
		{
			global.isDeaf = value;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "isDeaf", e.toString());
	}
});

global.isDemorgan = false;

gm.events.add('client.demorgan', (value) => {
	global.isDemorgan = value;
});

global.isArmor = false;

gm.events.add('client.isArmor', (value) => {
	global.isArmor = value;
});

const WalkStylesData = [
	null,
    "move_m@brave",
    "move_m@confident",
    "move_m@drunk@verydrunk",
    "move_m@shadyped@a",
    "move_m@sad@a",
    "move_f@sexy@a",
    "move_m@fire",
    "MOVE_M@FEMME@",
    "MOVE_M@TOUGH_GUY@", /* - Ковбой */
    "move_m@money",
    "anim@move_m@grooving@",
    "FEMALE_FAST_RUNNER",
    "move_p_m_one",
    "MOVE_M@DRUNK@SLIGHTLYDRUNK",
    "move_p_m_zero_janitor",
    "MOVE_M@POSH@",
    "ANIM_GROUP_MOVE_BALLISTIC",
    "move_f@flee@a",
    "move_f@fat@a",
    "move_f@injured",
    "clipset@move@trash_fast_turn",
    "move_f@arrogant@a",
    "move_f@scared",
    "move_p_m_zero_slow",
    "anim@move_m@security_guard",
    "move_ped_crouched"
];

global.wait(5000).then(() => {
	WalkStylesData.forEach((name) => {
		if (name != null)
			mp.game.streaming.requestClipSet(name);
	})
});

mp.events.addDataHandler("WalkStyle", (entity, value, oldValue) => {
	global.SetWalkStyle (entity, value);
});

global.SetWalkStyle = (entity, index) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0) {
			if (index === null) index = entity.getVariable('WalkStyle');
			if (index == 0 || WalkStylesData [index] == null) entity.resetMovementClipset(0.0);
			else entity.setMovementClipset(WalkStylesData [index], 0.0);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "SetWalkStyle", e.toString());
	}
}


const FacialClipsetData = [
	null,
    "mood_aiming_1",
    "mood_angry_1",
    "mood_drunk_1",
    "mood_happy_1",
    "mood_injured_1",
    "mood_stressed_1", /* - Злость */
    "mood_sleeping_1",
    "mood_knockout_1",
    "electrocuted_1",
    "eating_1",
    "drinking_1",
    "mood_sulk_1",
    "coughing_1",
    "shocked_1",
    "shocked_2",
    "effort_1",
    "effort_3",
    "pain_1",
    "pain_2",
    "pain_3",
    "smoking_inhale_1",
    "smoking_exhale_1",
    "smoking_hold_1"
];

mp.events.addDataHandler("FacialClipset", (entity, value, oldValue) => {
	global.SetFacialClipset (entity, value);
});

global.SetFacialClipset = (entity, index) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0) {
			if (index === null) index = entity.getVariable('FacialClipset');
			if (index == 0 || FacialClipsetData [index] == null) entity.clearFacialIdleAnimOverride(0.0);
			else mp.game.invoke('0xFFC24B988B938B38', entity.handle, FacialClipsetData [index], 0);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "SetFacialClipset", e.toString());
	}
}

gm.events.add("playerStreamIn", (entity) => {
	global.SetWalkStyle(entity, null);
	global.SetFacialClipset(entity, null);
});

mp.events.addDataHandler("AGM", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.remoteId === global.localplayer.remoteId) {
			value = Boolean (value);
			global.admingm = value;
			global.localplayer.setInvincible(value);
			mp.game.graphics.notify(value ? 'GM: ~g~Enabled' : 'GM: ~r~Disabled');
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "AGM", e.toString());
	}
});