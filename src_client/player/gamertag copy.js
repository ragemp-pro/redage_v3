mp.nametags.enabled = false;
global.binderFunctions.showGamertags = () => {// Включить / Выключить.
    if (!global.loggedin || global.chatActive || global.editing || global.menuCheck()) return;
    global.Name = !global.Name;
};

const MaxStreeamDist = 200;
const MaxStreeamPlayerStatsDist = 60;

new class extends debugRender {
	constructor() {
		super("r_player_gamertagcopy");
	}

	render () {
		if (!global.loggedin) return;

		const isAdmin = global.isAdmin;
		// Player gamertags
		if (global.isTagsHead) {
			const localPos = global.localplayer.position;
			nametags.forEach(nametag => {
				try {
					let [player, x, y, distance] = nametag;
					if (MaxStreeamDist >= distance && !player.getVariable('INVISIBLE')) {
						const position = player.position;
						const dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
						if (dist < 25.0 && !player.getVariable('HideNick')) {
							DrawPlayerName(player, dist);
							DrawEspHealthArmor(player, dist, MaxStreeamPlayerStatsDist >= distance);
							DrawPlayerIcon(player, dist);
						} else if (isAdmin)
							DrawEspHealthArmor(player);
					}
				}
				catch (e) 
				{
					if(new Date().getTime() - global.trycatchtime["player/gamertag1"] < 5000) return;
					global.trycatchtime["player/gamertag1"] = new Date().getTime();
					mp.events.callRemote("client_trycatch", "player/gamertag", "render 1", e.toString());
				}
			})
		} else if(isAdmin) {
			const localPos = global.localplayer.position;
			nametags.forEach(nametag => {
				try {
					let [player, x, y, distance] = nametag;
					const position = player.position;
					const dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
					DrawEspHealthArmor(player, dist);
				}
				catch (e) 
				{
					if(new Date().getTime() - global.trycatchtime["player/gamertag2"] < 5000) return;
					global.trycatchtime["player/gamertag2"] = new Date().getTime();
					mp.events.callRemote("client_trycatch", "player/gamertag", "render 2", e.toString());
				}
			})
		}
	}
};

const width = 0.03; 
const height = 0.004;
const border = 0.001;

const DrawEspHealthArmor = (player, dist, isStream = false) => {
	try {
		const isAdmin = global.isAdmin;
		if ((!isAdmin && mp.game.player.isFreeAimingAtEntity(player.handle) && isStream) ||
			(isAdmin && (global.esptoggle == 1 || global.esptoggle == 3) && !player.getVariable('ALVL'))) {


			const getBoneCoords = player.getBoneCoords(12844, 0, 0, 0);
			const _getScale = GetScale (dist, 25);
			let pos = mp.game.graphics.world3dToScreen2d(getBoneCoords.x, getBoneCoords.y, getBoneCoords.z + (1 - _getScale) + 0.2);
			if (!pos) {
				return false;
			}

			const graphics = mp.game.graphics;

			let health = player.getHealth();
			const realHealth = health;
			let drawColor = Array(3);
			health = health <= 100 ? health / 100 : (health - 100) / 100;

			const _width = width * _getScale;
			const _height = height * _getScale;

			graphics.drawRect(pos.x, pos.y, _width + border * 2, _height + border * 2, 0, 0, 0, 200);
			graphics.drawRect(pos.x, pos.y, _width, _height, 150, 150, 150, 255);

			if (realHealth >= 80)
				drawColor[0] = 0, drawColor[1] = 220, drawColor[2] = 0;
			else if (realHealth >= 20 && realHealth < 80)
				drawColor[0] = 255, drawColor[1] = 220, drawColor[2] = 0;
			else
				drawColor[0] = 255, drawColor[1] = 0, drawColor[2] = 0;

			graphics.drawRect(pos.x - _width / 2 * (1 - health), pos.y, _width * health, _height, drawColor[0], drawColor[1], drawColor[2], 200);
			
			const armour = player.getArmour() / 100;

			if (armour > 0) {
				const y = pos.y - 0.007 * _getScale;
				graphics.drawRect(pos.x, y, _width + border * 2, _height + border * 2, 0, 0, 0, 200);
				graphics.drawRect(pos.x, y, _width, _height, 41, 66, 78, 255);
				graphics.drawRect(pos.x - _width / 2 * (1 - armour), y, _width * armour, _height, 48, 108, 135, 200);
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gamertag", "DrawEspHealthArmor", e.toString());
	}
}

let wordCount = {};

const DrawPlayerName = (player, dist) => {
	try {
		const graphics = mp.game.graphics;
		const getBoneCoords = player.getBoneCoords(12844, 0, 0, 0);
		const isAdmin = player.getVariable('REDNAME');
		let textName = '';
		let textStats = '';
		
		if (player.getVariable("InDeath")) {
			textStats = textStats + '~w~<font color="#f22e27">Бeз coзнaния</font>';
		} else if (player.getVariable('AFK_STATUS') == true) {
			textStats = `~w~<font color="#A0A0A0">На паузе</font>`;
		} else if (player.getVariable('vmuted') == true) {
			textStats = '~w~<font color="#A0A0A0">Heмoй</font>';
		} else if (player.getVariable('Is_Microphone') == true) {
			textStats = '~w~<font color="#A0A0A0">Держит микрофон</font>';
		} else if (player.getVariable('isWhisper') == true) {
			textStats = '~w~<font color="#A0A0A0">Шепчется</font>';
		} else if (player.getVariable('isDeaf') == true) {
			textStats = '~w~<font color="#A0A0A0">Не слышит</font>';
		} else if (player.isTypingInTextChat) {
			if (!wordCount [player.remoteId]) wordCount [player.remoteId] = ['', 0];
			if (new Date().getTime() - wordCount [player.remoteId][1] > 500) {
				wordCount [player.remoteId][0] += '.';
				if (wordCount [player.remoteId][0].length > 3) wordCount [player.remoteId][0] = '';
				wordCount [player.remoteId][1] = new Date().getTime();
			}
			textStats = `~w~<font color="#A0A0A0">Пишет${wordCount [player.remoteId][0]}</font>`;
		}

		const pname = player.name;
		const pid = player.remoteId;
		
		if (global.localplayer.getVariable('PlayerAirsoftTeam') >= 0 && global.localplayer.getVariable('PlayerAirsoftTeam') == player.getVariable('PlayerAirsoftTeam')) {
			textName = `[TEAMMATE] ${pname.replace('_', ' ')} (${pid})`;
		}
		else if (global.mafiaGameProcess !== undefined && global.mafiaGameProcess >= 1 && player.getVariable('mafiaGameNumber')) {
			textName = `${player.getVariable('mafiaGameNumber')}`;
		} else if (isAdmin === true) {
			textName = translateText("Aдминиcтpaтop~n~{0} ({1})");
		} else {
			textName = global.getName (player, pid);
		}

		const colors = isAdmin ? [
			255,
			0,
			0,
			255
		] : [
			255,
			255,
			255,
			255
		];
		const _getScale = GetScale (dist, 25);
		let scale = 0.3 * _getScale;

		if (global.isAdmin && player.getVariable("IS_MEDIA") == true) {
			graphics.drawText("MEDIA", [
				getBoneCoords.x,
				getBoneCoords.y,
				getBoneCoords.z + (1 - _getScale) + (0.5)
			], {
				'font': 0,
				'color': [
					255,
					0,
					0,
					255
				],
				'scale': [
					scale,
					scale
				],
				'outline': true
			});
		}
		graphics.drawText(textStats, [
			getBoneCoords.x,
			getBoneCoords.y,
			getBoneCoords.z + (1 - _getScale) + (0.4)
		], {
			'font': 0,
			'color': colors,
			'scale': [
				scale,
				scale
			],
			'outline': true
		});
		graphics.drawText(textName, [
			getBoneCoords.x,
			getBoneCoords.y,
			getBoneCoords.z + (1 - _getScale) + (0.3)
		], {
			'font': 0,
			'color': colors,
			'scale': [
				scale,
				scale
			],
			'outline': true
		});
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag4"] < 5000) return;
		global.trycatchtime["player/gamertag4"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "DrawPlayerName", e.toString());
	}
}

function DrawPlayerIcon(player, dist) {
	try {
		const getBoneCoords = player.getBoneCoords(12844, 0, 0, 0);
		const _getScale = GetScale (dist, 25);
		const pos = mp.game.graphics.world3dToScreen2d(getBoneCoords.x, getBoneCoords.y, getBoneCoords.z + (1 - _getScale) + 0.55);
		if (!pos) {
			return false;
		}
		let scale = 0.85 * _getScale;
		
		if (player.isVoiceActive && !player.getVariable("PhoneTalk")) {
			let colors = [255, 255, 255, 255];
			let VoiceDist = "redage_textures_001";
			let VoiceName = "micro_on";
			if (global.pplMuted[player.name] === true) {
				colors = [255, 0, 0, 255];
				VoiceName = "muted";
			} else if(global.pplMutedMe[player.name] === true) {			
				colors = [255, 0, 0, 255];
				VoiceName = "mutedinamik";
			}
			DrawSprite(VoiceDist, VoiceName, [scale, scale], 0, colors, pos.x, pos.y + 0.038 * _getScale);
		}
		else if (player.isVoiceActive && player.isListening == 2)
			DrawSprite("redage_textures_001", 'phone', [scale, scale], 0, [255, 255, 255, 255], pos.x, pos.y + 0.038 * _getScale);
		else if (player.isVoiceActive && player.isListening == 3)
			DrawSprite("redage_textures_001", 'racia', [scale, scale], 0, [255, 255, 255, 255], pos.x, pos.y + 0.038 * _getScale);
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag5"] < 5000) return;
		global.trycatchtime["player/gamertag5"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "DrawPlayerIcon", e.toString());
	}
}

global.DrawSprite = (dist, name, scale, heading, colour, x, y, layer) => {
	try
	{
		const resolution = mp.game.graphics.getScreenActiveResolution(0, 0),
			textureResolution = mp.game.graphics.getTextureResolution(dist, name),
			textureScale = [(scale[0] * textureResolution.x) / resolution.x, (scale[1] * textureResolution.y) / resolution.y]

		if (mp.game.graphics.hasStreamedTextureDictLoaded(dist)) 
		{
			if (typeof layer === 'number') mp.game.graphics.set2dLayer(layer);
			mp.game.graphics.drawSprite(dist, name, x, y, textureScale[0], textureScale[1], heading, colour[0], colour[1], colour[2], colour[3]);
		} 
		else mp.game.graphics.requestStreamedTextureDict(dist, true)
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag7"] < 5000) return;
		global.trycatchtime["player/gamertag7"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "global.DrawSprite", e.toString());
	}
}

global.GetScale = (realDist, maxDist) => {
	return Math.max(0.1, 1 - realDist / maxDist);
};

gm.events.add('sendRPMessage', (type, msg, players) => {

	try
	{
		//mp.gui.chat.push(msg);
		//msg = global.escapeHtml (msg);
		//mp.gui.chat.push(msg);
		var chatcolor = ``;

		players.forEach((id) => {
			var player = mp.players.atRemoteId(id);
			if (mp.players.exists(player)) {

				if (type === "chat" || type === "s") {
					let localPos = global.localplayer.position;
					let position = player.position;
					let dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
					var color = (dist < 2) ? "FFFFFF" :
						(dist < 4) ? "F7F9F9" :
							(dist < 6) ? "DEE0E0" :
								(dist < 8) ? "C5C7C7" : "ACAEAE";

					chatcolor = color;
				}
				
				msg = msg.replace("{name}", global.getName (player, id));
			}
		});

		if (type === "chat" || type === "s") msg = `!{${chatcolor}}${msg}`;

		mp.gui.chat.push(msg);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gamertag", "sendRPMessage", e.toString());
	}
});

const IsNameToPlayer = (player) => {
	try
	{
		if (global.isAdmin) return 1;
		else if (player === global.localplayer) return 1;
		const playerFraction = player.getVariable('fraction');
		if (global.fractionId != 0 && playerFraction != 0 && global.fractionId === playerFraction) return 1;
		const playerOrganization = player.getVariable('organization');
		if (global.organizationId != 0 && playerOrganization != 0 && global.organizationId === playerOrganization) return 1;
		else if (!player.getVariable('IS_MASK') && global.passports[player.name] != undefined) return 2;
		else if (!player.getVariable('IS_MASK') && global.friends[player.name] != undefined) return 3;
		return 0;
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag6"] < 5000) return 0;
		global.trycatchtime["player/gamertag6"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "DrawEspHealthArmor", e.toString());
		return 0;
	}
}

global.getName = (player, id) => {
	let name = "";
	if (!player || !mp.players.exists(player))
		return name;
	
	const IdNameToPlayer = IsNameToPlayer (player);
	if (IdNameToPlayer !== 0) {
		name = (IdNameToPlayer === 3 && global.friends[player.name] === false) ? player.name.split("_")[0] : player.name.replace('_', ' ');
		if (IdNameToPlayer === 2) name += ` (${id} | ${global.passports[player.name]})`;
		else name += ` (${id})`;
	} else {
		let gender = global.GetGender (player);
		name = (gender ? translateText("Heзнaкoмeц") : translateText("Heзнaкoмкa")) + ` (${id})`;
	}
	return name;
}