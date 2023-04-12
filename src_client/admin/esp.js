global.binderFunctions.esp = () => {
	try 
	{
		if (!loggedin || global.chatActive || global.isAdmin !== true) return;
		if(global.esptoggle == 4) global.esptoggle = 0;
		else global.esptoggle++;
		if(global.esptoggle == 0) mp.game.graphics.notify('ESP: ~r~Disabled');
		else if (global.esptoggle == 1) mp.game.graphics.notify('ESP: ~g~Only Players');
		else if (global.esptoggle == 2) mp.game.graphics.notify('ESP: ~g~Only Vehicles');
		else if (global.esptoggle == 3) mp.game.graphics.notify('ESP: ~g~Players & Vehicles');
		else if (global.esptoggle == 4) mp.game.graphics.notify('ESP: ~g~Furniture & Items');
		mp.events.callRemote('saveEspState', global.esptoggle);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/esp", "binderFunctions", e.toString());
	}
	
};

gm.events.add('setEspState', function (state) {
	try
	{
		global.esptoggle = state;
		if(state == 0) mp.game.graphics.notify('ESP: ~r~Disabled');
		else if (state == 1) mp.game.graphics.notify('ESP: ~g~Only Players');
		else if (state == 2) mp.game.graphics.notify('ESP: ~g~Only Vehicles');
		else if (state == 3) mp.game.graphics.notify('ESP: ~g~Players & Vehicles');
		else if (state == 4) mp.game.graphics.notify('ESP: ~g~Furniture & Items');
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/esp", "setEspState", e.toString());
	}
	
});

gm.events.add('CheckMyVList', function (argument) {
	try 
	{
		mp.gui.chat.push("=== VOICE LIST ===");
		mp.players.forEachInStreamRange(player => {
			if(player.isVoiceActive) mp.gui.chat.push("[" + player.remoteId + "] " + player.name);
		});
		mp.gui.chat.push("=== VOICE LIST ===");
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "admin/esp", "CheckMyVList", e.toString());
	}
	
});

gm.events.add("render", () => {
	if (!loggedin || global.isAdmin !== true) return;
	if (global.esptoggle >= 1) {
		//global.GetFps ();
		const localPosition = global.localplayer.position;
		let position;
		if(global.esptoggle == 1 || global.esptoggle == 3) {
			mp.players.forEachInStreamRange(player => {
				try {
					if (player.handle !== 0 && player !== global.localplayer) {
						position = player.position;
						let playerALVL = player['ALVL'];
						if (!playerALVL)
							playerALVL = 0;
						if(global.adminLVL == 9 || global.adminLVL >= Number (playerALVL)) {
							if (player.isVoiceActive) {
								mp.game.graphics.drawText(`VOICE`, [position.x, position.y, position.z+1.8], {
									scale: [0.3, 0.3],
									outline: true,
									color: [0, 255, 0, 255],
									font: 4
								});
							}

							let espc = [255, 255, 255, 255];

							const fractionId = player['fraction'];
							if (playerALVL) espc = [255, 0, 0, 255];
							else if (fractionId == 1) espc = [34, 139, 34, 255];
							else if (fractionId == 2) espc = [186, 85, 211, 255];
							else if (fractionId == 3) espc = [240, 230, 140, 255];
							else if (fractionId == 4) espc = [65, 105, 225, 255];
							else if (fractionId == 5) espc = [255, 82, 82, 255];
							else if (fractionId == 6) espc = [173, 255, 47, 255];
							else if (fractionId == 7) espc = [0, 0, 255, 255];
							else if (fractionId == 8) espc = [247, 69, 132, 255];
							else if (fractionId == 9) espc = [0, 0, 255, 255];
							else if (fractionId == 10) espc = [186, 155, 0, 255];
							else if (fractionId == 11) espc = [10, 127, 140, 255];
							else if (fractionId == 12) espc = [139, 0, 0, 255];
							else if (fractionId == 13) espc = [169, 169, 169, 255];
							else if (fractionId == 14) espc = [139, 69, 19, 255];
							else if (fractionId == 15) espc = [255, 145, 0, 255];
							else if (fractionId == 18) espc = [0, 0, 255, 255];

							//mp.game.graphics.drawLine(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z, espc[0], espc[1], espc[2], espc[3]);

							let nameTag = `${player.name} (${player.remoteId})\n${Math.round (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z))} M | ${player.getHealth()} HP | ${player.getArmour()} AR`;

							//if (player.getVariable("IS_MEDIA") == true)
								//nameTag += '\n~g~MEDIA'

							mp.game.graphics.drawText(nameTag, [position.x, position.y, position.z+1.5], {
								scale: [0.3, 0.3],
								outline: true,
								color: espc,
								font: 4
							});
						} else if (!playerALVL) {
							mp.game.graphics.drawText(`${player.name} (${player.remoteId})\n${Math.round (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z))} M | ${player.getHealth()} HP | ${player.getArmour()} AR`, [position.x, position.y, position.z+1.5], {
								scale: [0.3, 0.3],
								outline: true,
								color: [0, 255, 0, 255],
								font: 4
							});
						}
					}
				} catch(e) {
					mp.game.graphics.drawText(`[ESP-ERROR] Cant render player ${player.name} (${player.remoteId})`, [0.20, 0.75], {
						font: 0,
						color: [255, 255, 255, 185],
						scale: [0.35, 0.35],
						outline: true
					});
				}
			});
		}
		if(global.esptoggle == 2 || global.esptoggle == 3) {
			mp.vehicles.forEachInStreamRange(vehicle => {
				if (vehicle.handle !== 0 && vehicle !== global.localplayer.vehicle) {
					position = vehicle.position;
					try {
						mp.game.graphics.drawText(`${mp.game.vehicle.getDisplayNameFromVehicleModel(vehicle.model)} (${vehicle.remoteId}) | ${Natives.GET_VEHICLE_NUMBER_PLATE_TEXT (vehicle.handle)}\n${Math.round (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z))} M | ${parseInt(vehicle.getEngineHealth())} HP | ${parseInt(vehicle.getSpeed()*3.6)} KMH`, [position.x, position.y, position.z-0.5], {
							scale: [0.3, 0.3],
							outline: true,
							color: [255, 255, 255, 150],
							font: 4
						});
					} catch(e) {
						mp.game.graphics.drawText(`${mp.game.vehicle.getDisplayNameFromVehicleModel(vehicle.model)} (${vehicle.remoteId}) | ${Natives.GET_VEHICLE_NUMBER_PLATE_TEXT (vehicle.handle)}\n${Math.round (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z))} M`, [position.x, position.y, position.z-0.5], {
							scale: [0.3, 0.3],
							outline: true,
							color: [255, 255, 255, 150],
							font: 4
						});
					}
				}
			});
		}
		if(global.esptoggle == 4) {
			mp.objects.forEachInStreamRangeItems(object => {
				if (object && object.doesExist() && object.type == "object") {
					position = object.position;
					if (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z) < 100) {
						try {
							if (object['dropData']) {
								mp.game.graphics.drawText(`Item (${object.remoteId})\n${object.model} | ${Math.round (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z))} M`, [position.x, position.y, position.z-0.1], {
									scale: [0.3, 0.3],
									outline: true,
									color: [196, 196, 196, 255],
									font: 4
								});
							}
							else if (object.getVariable('furniture')) {
								mp.game.graphics.drawText(`Furniture\n${object.model} | ${Math.round (mp.game.system.vdist(position.x, position.y, position.z, localPosition.x, localPosition.y, localPosition.z))} M`, [position.x, position.y, position.z-0.1], {
									scale: [0.3, 0.3],
									outline: true,
									color: [196, 196, 196, 255],
									font: 4
								});
							}
						} catch(e) {
							mp.game.graphics.drawText(`[ESP-ERROR] Cant render object`, [position.x, position.y, position.z-0.1], {
								scale: [0.3, 0.3],
								outline: true,
								color: [196, 196, 196, 255],
								font: 4
							});
						}
					}
				}
			});
		}
	}
});