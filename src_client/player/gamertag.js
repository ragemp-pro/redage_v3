mp.nametags.enabled = false;
global.binderFunctions.showGamertags = () => {// Включить / Выключить.
    if (!global.loggedin || global.chatActive || global.editing || global.menuCheck()) return;
    global.Name = !global.Name;
};

const eventsData = [
	{name: "ALVL"},
	{name: "AGM"},
	{name: "DMGDisable"},
	{name: "VoiceZone"},
	{name: "leader"},
	{name: "INVISIBLE", updateNameTag: true},
	{name: "HideNick", updateNameTag: true},
	{name: "InDeath"},
	{name: "AFK_STATUS"},
	{name: "vmuted"},
	{name: "isWhisper"},
	{name: "isDeaf"},
	{name: "PlayerAirsoftTeam"},
	{name: "REDNAME", updateNameTag: true},
	{name: "fraction", updateNameTag: true, updateName: true},
	{name: "organization", updateNameTag: true, updateName: true},
	{name: "IS_MASK", updateNameTag: true, updateName: true},
	{name: "NewUser"},
	{name: "SZ"},//SafeZone
	{name: "warType"},
	{name: "Old1"},
	{name: "Old2"},
	{name: "Old3"},
];

eventsData.forEach(data => {
	mp.events.addDataHandler(data.name, (entity, value, oldValue) => {
		if (entity && mp.players.exists(entity) && entity.type === 'player') {
			SetSharedData (entity, data.name, value);

			if (data.updateName) 
				SetName (entity);
	
			if (data.updateNameTag && entity.handle !== 0)
				SetNameTag (entity);
		}
	});
});

///
global.SetSharedData = (player, name, value) => {
	if (player && mp.players.exists(player) && player.type === 'player') {
		player [name] = value;
	}
}
//************************************************** */

gm.events.add('newPassport', async (entity, pass) => {
	if (entity && mp.players.exists(entity) && entity.type === 'player') {
		await global.wait (10);

		SetName (entity);

		if (entity.handle !== 0)
			SetNameTag (entity);
	}
});

gm.events.add('setFriendList', async () => {
	try {

		await global.wait (10);

		mp.players.forEach(player => {
			if (player && mp.players.exists(player)) {
				SetName (player);
	
				if (player.handle !== 0)
					SetNameTag (player);
			}
		});
	}
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", "setFriendList", e.toString());
	}
});

gm.events.add('setFriend', async () => {
	try {
		await global.wait (10);

		mp.players.forEach(player => {
			if (player && mp.players.exists(player)) {
				SetName (player);
				
				if (player.handle !== 0)
					SetNameTag (player);
			}
		});
	}
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", "setFriend", e.toString());
	}
});

gm.events.add("pPlayerStreamIn", (entity) => {
	eventsData.forEach(data => {
		SetSharedData (entity, data.name, entity.getVariable(data.name));
	});

	SetName (entity);
	SetNameTag (entity);
});

//************************************************** */

const SetName = (player) => {
	if (!player || !mp.players.exists(player))
		return;
	
	player.currentName = GetCurrentName (player, player.remoteId);
}

global.getName = (player) => {
	if (!player || !mp.players.exists(player))
		return;
	
	if (!player.currentName) 
		player.currentName = GetCurrentName (player, player.remoteId);

	return player.currentName;
}

const nameSettings = {
	scale: 0.275,
	font: 0
}

const SetNameTag = (player) => {
	try {

		if (!player || !mp.players.exists(player))
			return;

		if (player['INVISIBLE'] || player['HideNick']) {
			player.nameTag = false;
		} else {
			if (player['REDNAME']) {
				player.nameTag = `~r~${player.name.replace('_', ' ')} (${player.remoteId})`;
			} else {
				player.nameTag = global.getName (player);
			}

			player.nameWidth = ((text, font, scale) => (
				mp.game.ui.setTextEntryForWidth("STRING"),
				mp.game.ui.addTextComponentSubstringPlayerName(text),
				mp.game.ui.setTextFont(font),
				mp.game.ui.setTextScale(scale, scale),
				mp.game.ui.getTextScreenWidth(true)
			))(player.nameTag, nameSettings.font, nameSettings.scale);

			player.nameHeight = mp.game.ui.getTextScaleHeight(nameSettings.scale, nameSettings.font);

		}
	}
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", "SetNameTag", e.toString());
	}
}

global.loadTextureDict = dict => new Promise(async (resolve, reject) => {
	try {
		if (mp.game.graphics.hasStreamedTextureDictLoaded(dict))
			return resolve(true);
        mp.game.graphics.requestStreamedTextureDict(dict, false)
        let d = 0;
		while (!mp.game.graphics.hasStreamedTextureDictLoaded(dict)) {
            if (d > 5000) return resolve(translateText("Ошибка loadTextureDict. Model: ") + dict);
            //mp.game.streaming.requestModel(mp.game.joaat(requiredModel));
            d++;
            await global.wait (0);
        }
        return resolve(true);
    } 
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", "loadTextureDict", e.toString());
		resolve();
	}
});

const TextureDictLoadedData = [
	"redage_textures_001",
	"mpinventory"
]

global.wait(5000).then(() => {
	TextureDictLoadedData.forEach((textureDict) => {
		if (!mp.game.graphics.hasStreamedTextureDictLoaded(textureDict))
			global.loadTextureDict (textureDict);
	});
});

const MaxStreeamDist = 225;

gm.events.add(global.renderName ["1s"], () => {
	try {
		//const localPos = global.localplayer.position;
		mp.players.forEachInStreamRange(player => {
			//const dist = mp.game.system.vdist(player.position.x, player.position.y, player.position.z, localPos.x, localPos.y, localPos.z);

			//if (dist > MaxStreeamDist) {
			//	player.hasLosTo = false;
			//}

			if (global.localplayer.hasClearLosTo(player.handle, 17))
				player.hasLosTo = true;
			else 
				player.hasLosTo = false;
		});
	}
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", global.renderName ["1s"], e.toString());
	}
});

const defaultColor = [
	255,
	255,
	255,
	255
]

gm.events.add('render', (nametags) => {
	try {
		if (!global.loggedin) return;

		// Player gamertags
		if (global.isTagsHead) {
			nametags.forEach(nametag => {
				try {
					let [player, x, y, distance] = nametag;
					if (distance <= MaxStreeamDist) {
						if (!player.hasLosTo)
							return;
						if (!player.isOnScreen())
							return;
						if (!player.nameTag)
							return;

						DrawPlayerName(player, x, y + 0.015);
						
						y += 0.03015;

						DrawPlayerIcon(player, x, y, defaultColor);

						DrawHealthAndArmor(player, x, y + 0.005, 0.035, 0.003);						
					}
				}
				catch (e) 
				{
					if(new Date().getTime() - global.trycatchtime["player/gamertag1"] < 5000) return;
					global.trycatchtime["player/gamertag1"] = new Date().getTime();
					mp.events.callRemote("client_trycatch", "player/gamertag", "render 1", e.toString());
				}
			})
		} /*else if(isAdmin) {
			const localPos = global.localplayer.position;
			nametags.forEach(nametag => {
				try {
					let [player, x, y, distance] = nametag;
					const position = player.position;
					const dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
					DrawHealthAndArmor(player, dist);
				}
				catch (e) 
				{
					if(new Date().getTime() - global.trycatchtime["player/gamertag2"] < 5000) return;
					global.trycatchtime["player/gamertag2"] = new Date().getTime();
					mp.events.callRemote("client_trycatch", "player/gamertag", "render 2", e.toString());
				}
			})
		}*/
	}
	catch (e)
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag"] < 60000) return;
		global.trycatchtime["player/gamertag"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "render", e.toString());
	}
});


const drawSprites = [
	["redage_textures_001", "newUser"],
	["redage_textures_001", "telefon"],
	["redage_textures_001", "racia"],
	["redage_textures_001", "micro_on"],
	["redage_textures_001", "micro_off"],
	["redage_textures_001", "microphone"],
	["redage_textures_001", "quest_perform", -45],
	["redage_textures_001", "quest_take", -45],
	["redage_textures_001", "door_open", -45],
	["redage_textures_001", "door_close", -45],	
	["redage_textures_001", "death"],
	["redage_textures_001", "afk"],
	["redage_textures_001", "shepot"],
	["redage_textures_001", "mutedinamik"],
	["redage_textures_001", "chat"],
	["redage_textures_001", "friends"],
	["redage_textures_001", "admin"],
	["redage_textures_001", "ticket", -45],
	["redage_textures_001", "attack"],
	["redage_textures_001", "shield"],
	["redage_textures_001", "verify_1"],
	["redage_textures_001", "verify_2"],
	["redage_textures_001", "verify_3"],
]

const defaultSpriteSize = 0.55;
const defaultSpritesSize = {
	"door_open": 1.0,
	"door_close": 1.0,
	"ticket": 1.0,
	"quest_perform": 1.0,
	"quest_take": 1.0,
};


let drawSpritesSize = {};

gm.events.add(global.renderName ["2s"], () => {

	const activeResolution = mp.game.graphics.getScreenActiveResolution(0, 0);

	drawSprites.forEach((drawSprite) => {
		const size = defaultSpritesSize [ drawSprite [1] ] ? defaultSpritesSize [ drawSprite [1] ] : defaultSpriteSize;

		const drawSpritesResolution = mp.game.graphics.getTextureResolution(drawSprite [0], drawSprite [1]);
		drawSpritesSize [ drawSprite [1] ] = { 
			width: (size * drawSpritesResolution.x) / activeResolution.x,
			height: (size * drawSpritesResolution.y) / activeResolution.y,
			heading: drawSprite [2] ? drawSprite [2] : 0
		}
	});
});

const DrawPlayerName = (player, x, y) => {
	try {
		const textData = {};
		textData.font = 0; 
		textData.color = defaultColor;
		textData.scale = [
			0.275,
			0.275
		];
		textData.outline = true;
		//textData.centre = true;

		let nameTag = player.nameTag;

		if (global.isAdmin && player.getVariable("IS_MEDIA") == true)
			nameTag += '\n~g~MEDIA'

		mp.game.graphics.drawText(nameTag, [
			x,
			y
		], textData);

		let voiceIcon = false;
		if (player['AFK_STATUS'])
			voiceIcon = {textureDict: "redage_textures_001", textureName: "afk"};
		else if (player.isTypingInTextChat)
			voiceIcon = {textureDict: "redage_textures_001", textureName: "chat"};
		else if (!player["InDeath"]) {
			if (player.isVoiceActive && player.isListening == 2)
				voiceIcon = {textureDict: "redage_textures_001", textureName: "telefon"};
			else if (player.isVoiceActive && player.isListening == 3)
				voiceIcon = {textureDict: "redage_textures_001", textureName: "racia"};
			else if (player.isVoiceActive) {
				if (global.startedMafiaGame && global.mafiaGameProcess == 1) {
					voiceIcon = false;
				} else {
					let voiceDist = "redage_textures_001";
					let voiceName = "micro_on";
					let color = defaultColor;
			
					if (global.pplMuted[player.name] === true) {
						color = [255, 0, 0, 255];
						voiceName = "mutedinamik";
					} else if(global.pplMutedMe[player.name] === true) {			
						color = [255, 0, 0, 255];
						voiceName = "micro_off";
					} else if (player.voiceDist) {
						voiceName = "microphone";
					}
					voiceIcon = {textureDict: voiceDist, textureName: voiceName, color: color};
				}
			}
		}

		if (voiceIcon) {
			global.DrawSprite (
				voiceIcon.textureDict, 
				voiceIcon.textureName, 
				voiceIcon.color ? voiceIcon.color : defaultColor, 
				x + player.nameWidth / 2 + drawSpritesSize [voiceIcon.textureName].width / 2,
				y + player.nameHeight / 2 + drawSpritesSize [voiceIcon.textureName].height * 0.25);
		}

	}
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", "DrawPlayerName", e.toString());
	}
}

const adminColors = [
	defaultColor,
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[21, 168, 3, 255],
	[211, 126, 15, 255],
	[255, 221, 0, 255],
	[211, 126, 15, 255],
	[255, 0, 0, 255],
	[255, 0, 0, 255],
]

const GetDrawPlayerIcon = (player) => {
	try {
		let returnIcon = [];

		if (!global.localplayer["NewUser"] && player["NewUser"])
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "newUser", color: [255, 255, 255, 255]});

		if (player["InDeath"]) {
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "death", color: [255, 0, 0, 255]});
		} else if (player['vmuted']) {
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "mutedinamik", color: [255, 0, 0, 255]});
		} else if (player['isWhisper']) {
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "shepot"});
		} else if (player['isDeaf']) {
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "mutedinamik"});
		}

	if (player['Old1']) {
		returnIcon.push ({textureDict: "redage_textures_001", textureName: "verify_1"});
	} else if (player['Old2']) {
		returnIcon.push ({textureDict: "redage_textures_001", textureName: "verify_2"});
	} else if (player['Old3']) {
		returnIcon.push ({textureDict: "redage_textures_001", textureName: "verify_3"});
	}

		if (player["warType"] === 2)
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "attack", color: [255, 0, 0, 255]});
		else if (player["warType"] === 1)
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "shield", color: [0, 204, 102, 255]});

		if (global.localplayer['PlayerAirsoftTeam'] && global.localplayer['PlayerAirsoftTeam'] >= 0 && global.localplayer['PlayerAirsoftTeam'] == player['PlayerAirsoftTeam']) {
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "friends"});
		} else if (player['REDNAME']) {
			let playerALVL = Number (player['ALVL']);
			if (!playerALVL) 
				playerALVL = 0;

			returnIcon.push ({textureDict: "redage_textures_001", textureName: "admin", color: adminColors [playerALVL]});
		}
	
		/*if (player.isVoiceActive && player.isListening == 2)
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "telefon"});
		else if (player.isVoiceActive && player.isListening == 3)
			returnIcon.push ({textureDict: "redage_textures_001", textureName: "racia"});
		else if (player.isVoiceActive) {
			let voiceDist = "redage_textures_001";
			let voiceName = "micro_on";
			let color = false;
	
			if (global.pplMuted[player.name] === true) {
				color = [255, 0, 0, 255];
				voiceName = "mutedinamik";
			} else if(global.pplMutedMe[player.name] === true) {			
				color = [255, 0, 0, 255];
				voiceName = "micro_off";
			} else if (player.voiceDist) {
				voiceName = "microphone";
			}
			returnIcon.push ({textureDict: voiceDist, textureName: voiceName, color: color});
		}*/
	
	
		return returnIcon;
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag5"] < 5000) return;
		global.trycatchtime["player/gamertag5"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "GetDrawPlayerIcon", e.toString());
	}
	return [];
}

const DrawPlayerIcon = (player, x, y, color) => {
	try {
		const returnIcon = GetDrawPlayerIcon (player);
		let count = 0
		if (returnIcon && (count = returnIcon.length)) {
			if (count >= 3)
				x -= 0.015;
			else if (count >= 2)
				x -= 0.01;

			returnIcon.forEach((data) => {
				global.DrawSprite (data.textureDict, data.textureName, data.color ? data.color : color, x, y - 0.02);
				x += 0.01;
			});
		}
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag5"] < 5000) return;
		global.trycatchtime["player/gamertag5"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "DrawPlayerIcon", e.toString());
	}
}

global.DrawSprite = (textureDict, textureName, colour, x, y) => {
	const drawData = drawSpritesSize [textureName];
	if (drawData)
		mp.game.graphics.drawSprite(textureDict, textureName, x, y, drawData.width, drawData.height, drawData.heading, colour[0], colour[1], colour[2], colour[3]);
}

const DrawHealthAndArmor = (player, x, y, width, height) => {
	try {
		const isAdmin = global.isAdmin;
		if (isAdmin || mp.game.player.isFreeAimingAtEntity(player.handle) || mp.game.player.isTargettingEntity(player.handle)) {

			y += 0.0225;
			let health = player.getHealth();
			const realHealth = health;
			if (health > 100 || health < 0)
				health = 10;

			health = health <= 100 ? health / 100 : (health - 100) / 100;

			let armour = player.getArmour() / 100;
			if (armour > 100 || armour < 0)
				armour = 100;
			
			let drawColor = Array(3);

			if (realHealth >= 80)
				drawColor[0] = 0, drawColor[1] = 220, drawColor[2] = 0;
			else if (realHealth >= 20 && realHealth < 80)
				drawColor[0] = 255, drawColor[1] = 220, drawColor[2] = 0;
			else
				drawColor[0] = 255, drawColor[1] = 0, drawColor[2] = 0;

			mp.game.graphics.drawRect(x, y, width, height, drawColor[0], drawColor[1], drawColor[2], 145);
			mp.game.graphics.drawRect(x - width / 2 * (1 - health), y, width * health, height, drawColor[0], drawColor[1], drawColor[2], 200);

			if (armour > 0) {
				y += 0.007;
				mp.game.graphics.drawRect(x, y, width, height, 41, 66, 78, 255);
				mp.game.graphics.drawRect(x - width / 2 * (1 - armour), y, width * armour, height, 48, 108, 135, 200)
			}
		}
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag3"] < 5000) return;
		global.trycatchtime["player/gamertag3"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "DrawHealthAndArmor", e.toString());
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
				
				msg = msg.replace("{name}", global.getName (player));
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
		const playerFraction = player['fraction'];
		if (global.fractionId != 0 && playerFraction != 0 && global.fractionId === playerFraction) return 1;
		const playerOrganization = player['organization'];
		if (global.organizationId != 0 && playerOrganization != 0 && global.organizationId === playerOrganization) return 1;
		else if (!player['IS_MASK'] && global.passports[player.name] != undefined) return 2;
		else if (!player['IS_MASK'] && global.friends[player.name] != undefined) return 3;
		return 0;
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/gamertag6"] < 5000) return 0;
		global.trycatchtime["player/gamertag6"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/gamertag", "IsNameToPlayer", e.toString());
		return 0;
	}
}

const GetCurrentName = (player, id) => {
	try {

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
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/gametag", "GetCurrentName", e.toString());
		return "";
	}
}