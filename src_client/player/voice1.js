//Рация
let MaxRange = 12.0;
let isRadio = false;
let isVoice = false;

const listernType = {
	None: 0,
	Talk: 1,
	Phone: 2,
	Radio: 3,
}

global.binderFunctions.enableRadioVoice = () => {
	try
	{
		if (
			global.chatActive || 
			global.localplayer['vmuted'] ||
			!global.loggedin || 
			isVoice || 
			global.reportactive ||
			global.startedMafiaGame && global.mafiaGameProcess == 1 ||
			global.startedMafiaGame && global.mafiaGameProcess == 3 ||
			global.isDeaf ||
			global.menuCheck() && !global.walkieTalkieMenu && !global.mafiaMenuActive
		) return;	
		//else if (!global.organizationId && !global.fractionId)
		//	return;	
		mp.events.callRemote("addRadio");
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "global.binderFunctions.enableVoice", e.toString());
	}
}

let PlayerIdsToListernRadio = [];
gm.events.add('StartRadio', (json) => {
	try
	{
		isRadio = true;
		mp.voiceChat.muted = false;
		mp.events.call("PlayAnimToKey", global.localplayer, true, "radio");
		mp.gui.emmit(`window.hudStore.radio (1)`);
		mp.events.call("sounds.playInterface", "radio/on", 0.02, false);
		global.localplayer.playFacialAnim("mic_chatter", "mp_facial");
		PlayerIdsToListernRadio = JSON.parse (json);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "AddRadio", e.toString());
	}
});


global.binderFunctions.disableRadioVoice = () => {
	try
	{
		if (
			!global.loggedin || 
			mp.voiceChat.muted ||
			!isRadio) return;		
		isRadio = false;	 
		mp.events.callRemote("disableRadio");
		//mp.events.call("PlayAnimToKey", global.localplayer, false, "radio");
		mp.voiceChat.muted = true;
		mp.gui.emmit(`window.hudStore.radio (0)`);
		mp.events.call("sounds.playInterface", "radio/off", 0.02, false);
		global.localplayer.playFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");
		//'
		let player;
		PlayerIdsToListernRadio.forEach((playerId) => {
			player = mp.players.atRemoteId(playerId);
			if (player)
				g_voiceMgr.remove(player, false);
		});
		PlayerIdsToListernRadio = []
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "global.binderFunctions.disableVoice", e.toString());
	}
}

gm.events.add('AddRadio', (playerId) => {
	try
	{
		if (global.MaxVolumeRadio > 0) {
			let name = "";
			const player = mp.players.atRemoteId(playerId);
			if (player) {
				if (global.pplMuted [player.name] || global.pplMutedMe [player.name]) return false;
				else if (player.isListening == listernType.Phone) return false;
				else if (global.isDeaf && !player['ALVL']) return false;
				else if (global.FirstLVLMute && player['DMGDisable']) return false;

				g_voiceMgr.remove(player, false);

				player.isListening = listernType.Radio;
				player.voice3d = false;
				player.voiceVolume = global.getSoundRadioVolume(100);
				//mp.events.call("PlayAnimToKey", player, true, "radio");
				name = global.getName(player);
				mp.gui.emmit(`window.voice.add('${name}', ${playerId})`);
				mp.gui.emmit(`window.events.callEvent('cef.walkietalkie.addPlayerInfo', '${name}', ${playerId})`);
				mp.game.audio.playSoundFrontend(-1, "Start_Squelch", "CB_RADIO_SFX", true);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "AddRadio", e.toString());
	}
});

gm.events.add('RemoveRadio', (playerId) => {
	try
	{
		mp.gui.emmit(`window.voice.dell(${playerId})`);
		mp.gui.emmit(`window.events.callEvent('cef.walkietalkie.removePlayerInfo', ${playerId})`);
		const player = mp.players.atRemoteId(playerId);
		if (player) {
			mp.events.call("PlayAnimToKey", player, false, "radio");
			player.isListening = listernType.None;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "AddRadio", e.toString());
	}
});

gm.events.add('LeaveRadio', () => {
	mp.players.forEach((player) => {
		if (player && player.isListening == listernType.Radio)  {
				
			mp.gui.emmit(`window.voice.dell(${player.remoteId})`);
			mp.gui.emmit(`window.events.callEvent("cef.walkietalkie.removePlayerInfo", ${player.remoteId})`);
			mp.events.callRemote("remove_voice_listener", player);
		}
	});

});

gm.events.add('UpdateVoiceRadio', () => {
	mp.players.forEach((player) => {
		if (player && player.isListening == listernType.Radio)  {
			player.voiceVolume = global.getSoundRadioVolume (100);
		}
	});
});

global.getSoundRadioVolume = (volume) => {
	return parseFloat(MaxVolumeRadio * 0.01 * (volume * 0.01));
}

//////////////////////////////////////////////////////////////////////////

gm.events.add('openWalkieTalkieMenu', async (isGovFactionPlayer, walkieTalkieFrequency) => {
	try
	{		
		if (!global.loggedin || global.chatActive || global.editing || global.isDemorgan || global.menuCheck() || global.localplayer.getVariable("InDeath")) return;
		if (!global.walkieTalkieMenu) {			
			mp.gui.emmit(`window.hudStore.isWalkietalkie (true)`);
			mp.gui.emmit(`window.events.callEvent("cef.walkietalkie.updateMainInfo", '${global.Keys[global.userBinder[50].keyCode]}', ${isGovFactionPlayer}, ${walkieTalkieFrequency})`);			
			global.menuOpen(true);
			mp.discord.update('Настраивает рацию', `на RedAge под ID ${global.localplayer.remoteId}`);
			global.walkieTalkieMenu = true;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/voice", "openWalkieTalkieMenu", e.toString());
    }
});

gm.events.add('closeWalkieTalkieMenu', () => {
	try
	{
		if (global.walkieTalkieMenu === true) {
			mp.gui.emmit(`window.hudStore.isWalkietalkie (false)`);
			global.menuClose();
			global.walkieTalkieMenu = false;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/voice", "closeWalkieTalkieMenu", e.toString());
    }
});

gm.events.add('UpdateWalkieTalkieVolume_client', (value) => {
	try
	{
		global.MaxVolumeRadio = value;
		mp.events.call('UpdateVoiceRadio');
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/voice", "UpdateWalkieTalkieVolume_client", e.toString());
    }
});

gm.events.add('UpdateWalkieTalkieFrequency_client', (value) => {
	try
	{
		mp.events.callRemote('UpdateWalkieTalkieFrequency', value)
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/voice", "UpdateWalkieTalkieFrequency_client", e.toString());
    }
});

gm.events.add('PressedDangerButton_client', () => {
	try
	{
		if (global.fractionId == 7 || global.fractionId == 9 || global.fractionId == 18) {
			mp.events.callRemote('PressedDangerButton');
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "PressedDangerButton_client", e.toString());
	}
});

gm.events.add('StartDangerButtonSound_client', (_url) => {
	try
	{
		if (global.MaxVolumeRadio > 0) {
			mp.events.call('sounds.playInterface', _url, global.getSoundRadioVolume (80));
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "player/voice", "StartDangerButtonSound_client", e.toString());
	}
});

let eventsRefresh = new Date().getTime();
global.binderFunctions.takeWalkieTalkie = () => {
    try
    {
		if (!global.loggedin || global.chatActive || global.editing || global.menuCheck()) return;
		if (!global.walkieTalkieMenu) {
			if (new Date().getTime() - eventsRefresh < 1000) {
                mp.events.call('notify', 4, 9, "Слишком быстро.", 1500);
                return;
            }
        
            eventsRefresh = new Date().getTime();

			mp.events.callRemote("TakeWalkieTalkie_server");
		} else {
			mp.events.call('closeWalkieTalkieMenu');
		}
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/voice", "global.binderFunctions.takeWalkieTalkie", e.toString());
    }
}

//////////////////////////////////////////////////////////////////////////

global.binderFunctions.enableVoice = () => {
	try
	{
		if (
			global.phoneMute ||
			global.chatActive || 
			global.localplayer['vmuted'] ||
			!global.loggedin || 
			isRadio || 
			global.reportactive ||
			global.startedMafiaGame && global.mafiaGameProcess == 1 ||
			global.startedMafiaGame && global.mafiaGameProcess == 3 ||
			global.isDeaf
		) return;
		if (mp.voiceChat.muted) {
			isVoice = true;
			mp.voiceChat.muted = false;
			mp.gui.emmit(`window.hudStore.microphone (1)`);
			global.localplayer.playFacialAnim("mic_chatter", "mp_facial");
		} else
			global.binderFunctions.disableVoice (false);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "global.binderFunctions.enableVoice", e.toString());
	}
}

global.binderFunctions.disableVoice = (isBtn = true) => {
	try
	{
		if (
			!global.loggedin || 
			mp.voiceChat.muted
		) return;
		isVoice = false;
		mp.voiceChat.muted = true;
		mp.gui.emmit(`window.hudStore.microphone (0)`);
		global.localplayer.playFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "global.binderFunctions.disableVoice", e.toString());
	}
}

let voiceReloadTime = 0;
global.binderFunctions.voiceReload = () => {
	try
	{
		if (voiceReloadTime >= new Date().getTime()) return;
		voiceReloadTime = new Date().getTime() + 2000;
		mp.gui.chat.push("Вы успешно перезагрузили голосовой чат для себя.");
		mp.voiceChat.cleanupAndReload(true, true, true);
		global.pplMuted = [];
		mp.events.callRemote("unmuteAll");
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "global.binderFunctions.voiceReload", e.toString());
	}
}

gm.events.add('v_reload', () => {
	try
	{
		mp.voiceChat.cleanupAndReload(true, true, true);
		global.pplMuted = [];
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "v_reload", e.toString());
	}
});

let localPos = global.localplayer.position;
let playerPos = global.localplayer.position;

let PhoneTalkPlayer = null; 

mp.events.addDataHandler("PhoneTalk", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity === global.localplayer) {
			if (value) {
				mp.events.call("client.phone.callAccept");//Подтверждамем звонок

				let player = mp.players.atRemoteId(value - 1);
				if (player) {
					PhoneTalkPlayer = player; 

					g_voiceMgr.remove(player, false);

					player.isListening = listernType.Phone;

					player.voice3d = false;
					player.voiceVolume = global.getSoundVolume (100);
				}
			} else {
				if (PhoneTalkPlayer && mp.players.exists (PhoneTalkPlayer)) {
					mp.events.callRemote("remove_voice_listener", PhoneTalkPlayer);
					PhoneTalkPlayer.isListening = listernType.None;
				}
				PhoneTalkPlayer = null;
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "PhoneTalk", e.toString());
	}
});

gm.events.add('playerStartTalking', (player) => {
	try
	{		
		if (player && mp.players.exists(player) && player.type === 'player') {
			/*if (player.isListening = listernType.Phone) {
				player.voiceVolume = global.getSoundVolume (100);
			} else if (player.isListening = listernType.Radio) {	
				player.voiceVolume = global.getSoundRadioVolume (100);
			} else  {
				if (player.isListening = listernType.None) player.voiceVolume = 0.0; 
				else player.voice3d = true;
			}*/
			player.playFacialAnim("mic_chatter", "mp_facial");
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "playerStartTalking", e.toString());
	}
});

gm.events.add('playerStopTalking', (player) => {
	try
	{
		if (player && mp.players.exists(player) && player.type === 'player') 
			player.playFacialAnim("mood_normal_1", "facials@gen_male@variations@normal");
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "playerStopTalking", e.toString());
	}
});

gm.events.add('clientSyncHandle', (status) => {
	try
	{
		if (!status) {
			mp.players.forEach((player) => {
				if (player && player.isListening == listernType.Talk)  {
					g_voiceMgr.remove(player, false);
					mp.events.callRemote("remove_voice_listener_sync", player);
				}
			});
		} else {			
			mp.players.forEach((player) => {
				if (player && player.isListening == listernType.Talk)  {
					g_voiceMgr.remove(player, true);
				}
			});
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/voice", "clientSyncHandle", e.toString());
	}
});

let g_voiceMgr = {
    listeners: [],

    add: function (player, notify) {
		try
		{
			if (this.listeners.indexOf(player) === -1) 
			{
				player.voiceVolume = 0.0;
				player.voice3d = true;
				player.isListening = listernType.Talk;

				if (notify && !PlayerIdsToListernRadio.includes(player.remoteId)) 
					mp.events.callRemote("add_voice_listener", player);

				this.listeners.push(player);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/voice", "add", e.toString());
		}
    },

    remove: function (player, removefrom) {
		try
		{
			let index = this.listeners.indexOf(player);
			if (index !== -1) 
			{
				this.listeners.splice(index, 1);
				player.voiceVolume = 0.0;
				player.isListening = listernType.None;

				if (removefrom && !PlayerIdsToListernRadio.includes(player.remoteId))
					mp.events.callRemote("remove_voice_listener", player);
					
				return true;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/voice", "remove", e.toString());
		}
		return false;
    }
};

gm.events.add("playerQuit", (player) => {
    if (player.isListening) 
		g_voiceMgr.remove(player, false);
});

// Я разрешаю слушать себя пользователям

const IsVoiceToPlayer = (player) => {
	try 
	{
        if (!player) return false;
        else if (player.handle === 0) return false;
        else if (player === global.localplayer) return false;
        else if (player.isListening) return false;		
        else if (global.pplMuted [player.name] || global.pplMutedMe [player.name]) return false;
		else if (player["InDeath"]) return false;
		else if (global.isDeaf && !player['ALVL']) return false;
		else if (!global.isAdmin && player['isDeaf']) return false;
		else if (global.FirstLVLMute && player['DMGDisable']) return false;
        else if (global.localplayer['VoiceZone'] && !player['ALVL'] && global.localplayer['VoiceZone'] !== player['VoiceZone']) return false;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/voice", "IsVoiceToPlayer", e.toString());
        return false;
    }
    return true;
}

const IsVoiceListenerToPlayer = (player) => {
	try {
        if (!player) return false;
		else if (global.isDeath) return false;
        else if (player.handle === 0) return false;
        else if (!player.isListening) return false;
        else if (global.pplMuted [player.name] || global.pplMutedMe [player.name]) return false;
		else if (player["InDeath"]) return false;
		else if (global.isDeaf && !player['ALVL']) return false;
		else if (!global.isAdmin && player['isDeaf']) return false;
		else if (global.FirstLVLMute && player['DMGDisable']) return false;
        else if (global.localplayer['VoiceZone'] && !player['ALVL'] && global.localplayer['VoiceZone'] !== player['VoiceZone']) return false;
	}
	catch (e) 
	{
        mp.events.callRemote("client_trycatch", "player/voice", "IsVoiceListenerToPlayer", e.toString());
        return false;
	}
    return true;
}

global.getSoundVolume = (volume) => {
	return Number(parseFloat(Number(global.MaxVoiceVolume) * 0.01 * (Number(volume) * 0.01)).toFixed(2));
}

mp.events.addDataHandler("VoiceDist", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player') {
			entity.voiceDist = Number (value);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "ALVL", e.toString());
	}
});


const voiceUpdate = () => {
	try {

        localPos = global.localplayer.position;
		
		if (!global.isDeath) {
			mp.players.forEachInStreamRange(player => {
				if (IsVoiceToPlayer (player)) 
				{
					playerPos = player.position;
					const dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);

					if (global.localplayer.voiceDist && dist <= global.localplayer.voiceDist) 
						g_voiceMgr.add(player, true);
					else if (player.voiceDist && dist <= player.voiceDist) 
						g_voiceMgr.add(player, true);
					else if (dist <= MaxRange) 
						g_voiceMgr.add(player, true);
				}
			});
		}
        
		g_voiceMgr.listeners.forEach((player) => {
			if (IsVoiceListenerToPlayer (player)) 
			{
				playerPos = player.position;
				const dist = mp.game.system.vdist(playerPos.x, playerPos.y, playerPos.z, localPos.x, localPos.y, localPos.z);
				
				if (global.localplayer.voiceDist && dist <= global.localplayer.voiceDist) {
					
					if (dist <= MaxRange) 
						player.voiceVolume = global.getSoundVolume(100 * (1 - (dist / MaxRange)));
					else 
						player.voiceVolume = 0;
				}
				else if (player.voiceDist && dist <= player.voiceDist) 
					player.voiceVolume = global.getSoundVolume(100 * (1 - (dist / player.voiceDist)));
				else if (dist <= MaxRange) 
					player.voiceVolume = global.getSoundVolume(100 * (1 - (dist / MaxRange)));
				else 
					g_voiceMgr.remove(player, true);
			}
			else 
				g_voiceMgr.remove(player, true);
		});

	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/voice"] < 60000) return;
		global.trycatchtime["player/voice"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/voice", "voiceUpdate", e.toString());
	}
}

gm.events.add(global.renderName ["350ms"], voiceUpdate);