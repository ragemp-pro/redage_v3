let petData = {
	entity: null,
	blip: null,
	attack: null,
	attackPet: null,
	ballPosition: null,
	ballHandle: null,
	ballThrown: false,
	toFollow: null,
	toSniff: null,
	toSniffTimer: 0,
	ball: null,
	isStop: false,
	meTp: false,
	followType: 0
}

const SetFollowPet = (entity) => {
	try 
	{		
		if (entity && mp.peds.exists(entity) && entity.type === 'ped' && entity.handle !== 0) {
			if (entity.getVariable("isPet")) {
				entity.isPet = true;

				/*entity.setCanBeDamaged(true);
				 entity.setInvincible(true);
				 entity.setOnlyDamagedByPlayer(true);
				 entity.setProofs(true, true, true, true, true, true, true, true);
				 entity.setAsMission(true, true);*/


				/*entity.setCombatAbility(100);
				entity.setCombatRange(1);
				entity.setCombatMovement(3);
				entity.setCombatAttributes(46, true);
				entity.setCombatAttributes(5, true);
				entity.setFleeAttributes(0.0, false);*/
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/clothes", "SetFollowPet", e.toString());
	}
}

const petToPng = {
	1462895032: "cat.png",
	351016938: "chop.png",
	1318032802: "husky.png",
	1125994524: "poodle.png",
	1832265812: "pug.png",
	882848737: "retriever.png",
	2506301981: "rottweiler.png",
	1126154828: "shepherd.png",
	2910340283: "westy.png",
	2971380566: "pig.png",
	3462393972: "boar.png",
	307287994: "panther.png",
	3877461608: "bpanther.png",
	1682622302: "coyote.png"
}

gm.events.add('client.initPet', (ped, health, petName) => {
	try {
		if (ped && mp.peds.exists(ped) && ped.type === 'ped') {
			//ped.freezePosition(true);
			ped.freeze = true; //pet state
			ped.sit = false; //pet state
			ped.sleep = false; //pet state
			ped.gettingBall = false; //pet state
			mp.gui.emmit(`window.charStore.charIsPet ('${petToPng [ped.model]}')`);
			mp.gui.emmit(`window.events.callEvent("cef.pet.health", ${health})`);
			mp.gui.emmit(`window.hudStore.animalName ('${petName}')`);
			//ped.taskFollowToOffsetOf(global.localplayer.handle, 0, 0, 0, 1, -1, 1, true);
			petData.entity = ped;
			petData.model = ped.model;
			//
			petData.blip = mp.blips.new(463, new mp.Vector3(), { alpha: 255, color: 75, name: translateText("Питомец") });
			petData.blip.setCoords(ped.position);
			//
			petData.followType = 0;
			//
			updateMenu ();
		}
	}
	catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/petSystem", "initPet", e.toString());
    }
});

gm.events.add('client.pet.health', (health) => {
	if (petData.entity && mp.peds.exists(petData.entity)) {
		mp.gui.emmit(`window.events.callEvent("cef.pet.health", ${health})`);
	}
});

gm.events.add("pedStreamIn", (entity) => {
	SetFollowPet (entity);
});

const Config = {
    Job: 'police',
    //Command = 'policedog', -- set to false if you dont want to have a command
    Model: 351016938,
	TpDistance: 50.0,
	Freeze: {
		dict: 'creatures@rottweiler@amb@world_dog_sitting@exit',
		anim: 'exit'
	},
	Sit: {
		dict: 'creatures@rottweiler@amb@world_dog_sitting@base',
		anim: 'base'
	},
	Sleep: {
		dict: 'creatures@rottweiler@amb@sleep_in_kennel@',
		anim: 'sleep_in_kennel'
	},
    //Drugs = {'weed', 'cocaine', 'meth'}, -- add all drugs here for the dog to detect
}

let isOpenUsePet = false;
gm.events.add('client.pet.isUse', (toggled) => {
	isOpenUsePet = toggled;
});

gm.events.add("render", () => {
	if (!global.loggedin) return;
	/*if (isOpenUsePet && petData.entity && mp.peds.exists(petData.entity) && mp.keys.isDown(global.Keys.VK_RBUTTON)) {
		mp.gui.chat.push("asd - " + pos.x, pos.y, pos.z, rot);
		global.OnPetEditor (petData.entity.model, data, (pos, rot, index) => {
			mp.gui.chat.push("asd - " + pos.x, pos.y, pos.z, rot);
			//mp.events.callRemote('server.object.finish', data [index], pos.x, pos.y, pos.z, rot, 1800000);
		})
	}*/

	if (petData.blip && petData.entity && petData.entity.doesExist()) {
		petData.blip.setCoords(petData.entity.getCoords(true));
	}

	const localPos = global.localplayer.position;
	const graphics = mp.game.graphics;

	mp.peds.forEachInStreamRange((ped) => {
		if (ped && ped.type === "ped") {
			const petName = ped.getVariable("petName");
			if (petName && petName.length > 0) {
				const pedPosition = ped.getCoords(true);
				const dist = mp.game.system.vdist(pedPosition.x, pedPosition.y, pedPosition.z, localPos.x, localPos.y, localPos.z);
				if (dist < 15.0) {
					const getBoneCoords = ped.getBoneCoords(12844, 0, 0, 0);
					const _getScale = GetScale (dist, 25);
					let scale = 0.3 * _getScale;

					graphics.drawText(petName, [
						getBoneCoords.x,
						getBoneCoords.y,
						getBoneCoords.z + (1 - _getScale) + (0.35)
					], {
						'font': 0,
						'color': [255,
							255,
							255,
							255],
						'scale': [
							scale,
							scale
						],
						'outline': true
					});
				}
			}
		}
	});
});

let updateDimensionAntiFlood = 0;

const OnTimer = async () => {
	try {
        if (!global.loggedin) return;
		let petExist = mp.peds.exists(petData.entity);
		if (!petData.entity && !petExist) return;
		if (petData.entity && !petExist) 
		{
			if (petData.blip) petData.blip.destroy();
			
			petData = 
			{
				entity: null,
				blip: null,
				attack: null
			}
	
			mp.gui.emmit(`window.charStore.charIsPet (false)`);
			mp.gui.emmit(`window.hudStore.isAnimal (false)`);
			mp.gui.emmit(`window.SetBindToKey (-1)`);
			return;
		}
		if (petData.entity.isInMeleeCombat() && PetNoAttack.includes (petData.entity.model)) 
		{
			ClearAttackInterval (true);
		} 
		else if (petData.entity.isInMeleeCombat() && petData.entity.isInCombat(global.localplayer.handle)) 
		{	
			ClearAttackInterval (true);
		} 
		else if (petData.attack === null && petData.attackPet === null && petData.entity.isInMeleeCombat()) 
		{
			mp.players.forEachInStreamRange((player) => {
				if (petData.entity.isInCombat(player.handle)) {	
					petData.attack = player;
					updateMenu ();
				}
			});
			mp.peds.forEachInStreamRange((pet) => {
				if (petData.entity.isInCombat(pet.handle)) {	
					petData.attackPet = pet;
					updateMenu ();
				}
			});
			if (petData.attack === null && petData.attackPet === null) {
				ClearAttackInterval(true);
			}
		} 
		
		else if (global.localplayer.isInAnyVehicle(true) && petData.entity.isInAnyVehicle(false)) 
		{	
			petData.entity.freeze = false;
			petData.followType = 0;
			return;
		} 
		else if (petData.meTp && updateDimensionAntiFlood < new Date().getTime()) 
		{
			petData.meTp = false;
			petData.entity.freeze = false;
			petData.followType = 0;
			mp.events.callRemote('server.pet.topos');
			updateDimensionAntiFlood = new Date().getTime() + 10000;
			//OnTimer ();
		}
		else if (petData.attack !== null) 
		{	
			if (isAddPlayerToList (petData.attack)) {
				petData.entity.taskCombat(petData.attack.handle, 0, 16);
			} else 
				ClearAttackInterval ();
		} 
		else if (petData.attackPet !== null) 
		{
			if (isAddPedToList (petData.attackPet)) {
				petData.entity.taskCombat(petData.attackPet.handle, 0, 16);
			} else 
				ClearAttackInterval ();
		}	
		else if (petData.entity.gettingBall && petData.ballPosition) 
		{	
			const pedPosition = petData.entity.getCoords(true);
			const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, petData.ballPosition.x, petData.ballPosition.y, petData.ballPosition.z, true);
			
			if (!Natives.DOES_ENTITY_EXIST (petData.ballHandle)) {
				petData.entity.freeze = false;
				petData.followType = 0;
				petData.ballPosition = null;
				petData.ballHandle = null;
				petData.entity.gettingBall = false;
				petData.ballThrown = false; 
			}
			else if (distance <= 1.5) {
				petData.entity.freeze = false;
				petData.followType = 0;

				mp.events.callRemote('server.pet.dellball', petData.entity, petData.ballPosition.x, petData.ballPosition.y, petData.ballPosition.z);

				//Natives.SET_ENTITY_AS_MISSION_ENTITY (petData.ballHandle, true, true);
				//mp.game.object.deleteObject(petData.ballHandle);
				//Natives.DELETE_OBJECT (petData.ballHandle);

				petData.ballPosition = null;
				petData.ballHandle = null;
				petData.entity.gettingBall = false;
				petData.ballThrown = true; 
			} else {
				let speed;
				if(distance > 5)
					speed = 5;
				else 
					speed = 1;

				petData.entity.taskGoToCoordAndAimAtHatedEntitiesNearCoord(petData.ballPosition.x, petData.ballPosition.y, petData.ballPosition.z + 0.4, petData.ballPosition.x, petData.ballPosition.y, petData.ballPosition.z + 0.4, speed, false, 0, 0, false, 0, false, petData.model);
			}
		} 
		else if (!petData.entity.freeze) 
		{
			if(petData.toFollow !== null) 
			{
				if (isAddPlayerToList (petData.toFollow)) onToFollow (petData.toFollow, "follow");
				else ClearAttackInterval ();
			} 
			else if(petData.toSniff !== null) 
			{
				if (isAddPlayerToList (petData.toSniff)) onToFollow (petData.toSniff, "sniff");
				else ClearAttackInterval ();
			}
			else if(!petData.entity.gettingBall && petData.attack === null && petData.attackPet === null) onToFollow (global.localplayer, "me");
		}
	}
	catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/petSystem", "OnTimer", e.toString());
    }
}

setInterval(() => {
	OnTimer ();
}, 100 * 5)

const onToFollow = (player, type) => {
	try {
		
		if (!petData.entity || !mp.peds.exists(petData.entity))
			return;

		const pedCoords = petData.entity.getCoords(false);
		const pedPosition = new mp.Vector3(pedCoords.x, pedCoords.y, pedCoords.z);//petData.entity.getCoords(true);
		const distance = global.vdist2(pedPosition, player.position);
		const playerDimension = global.localplayer.dimension;
		const pedDimension = petData.entity.dimension;
		
		if (distance <= 2.0 && pedDimension === playerDimension) {
			if (petData.isStop || petData.entity.isInAnyVehicle(false))
				return;
	
			petData.isStop = true;
	
			petData.followType = 0;
			//petData.entity.setKeepTask(false);
			//petData.entity.freeze = true;
			freezePet(petData.entity);
			//updateMenu ();
	
			if (type == "me" && petData.ballThrown) {
				petData.ballThrown = false; 
				mp.events.callRemote('server.pet.getBall', petData.entity);
	
				if (petData.ball && mp.objects.exists(petData.ball)) 
					petData.ball.destroy();
	
			} else if (type == "sniff") {
				petData.entity.freeze = true;

				if (petData.toSniffTimer != 0)
					clearTimeout(petData.toSniffTimer);

				petData.toSniffTimer = setTimeout (() => {
					petData.toSniffTimer = 0;
					petData.entity.freeze = false;
					ClearAttackInterval ();
					mp.events.callRemote('server.pet.sniff', player);
				}, 2500);
			}
		} else {
			petData.isStop = false;
			let pedInVehicle = petData.entity.isInAnyVehicle(false);
			//if we get too far from the pet, it stops and waits for us sitting down
			if (playerDimension == 0 && distance <= 10 && player.isInAnyVehicle(true) && !pedInVehicle) 
			{
				petData.entity.freeze = true;
				mp.events.call('client.pet.setInVehicle');
				return;
			} else if (playerDimension == 0 && !player.isInAnyVehicle(true) && pedInVehicle) {
				petData.entity.taskLeaveVehicle(petData.entity.getVehicleIsIn(false), 16);
				petData.followType = 0;
				return;
			} else if (!petData.ballThrown && distance > 40 && !pedInVehicle && !global.fly.flying) {
				petData.entity.freeze = true;
				petData.entity.clearTasks();
				petData.entity.clearTasksImmediately();	
				freezePet(petData.entity);
				petData.meTp = true;
			}
			else taskToPlayer (player);
		}
	}
	catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/petSystem", "onToFollow", e.toString());
    }
}

const taskToPlayer = (player) => {
	if (!petData.entity.isInAnyVehicle(false)/* && petData.followType == 0*/) {

		const pedPosition = new mp.Vector3(petData.entity.getCoords(false).x, petData.entity.getCoords(false).y, petData.entity.getCoords(false).z);//petData.entity.getCoords(true);
		const distance = global.vdist2(pedPosition, player.position);

		let speed = 1;

		if (distance > 8)
			speed = 10;
		else if (distance > 6)
			speed = 5;
		else if (distance > 4)
			speed = 3;

		petData.entity.taskGoToCoordAndAimAtHatedEntitiesNearCoord(player.position.x, player.position.y, player.position.z, player.position.x, player.position.y, player.position.z, speed, false, parseFloat(0), parseFloat(0), false, 0, false, petData.model);
		petData.followType = 1;
	}
}


//Function to freeze pet
function freezePet(pet) {
	try {
		if (pet) {
			//pet.clearTasks();
			//pet.clearTasksImmediately();
			global.requestAnimDict(Config.Freeze.dict).then(async () => {
				pet.taskPlayAnim (Config.Freeze.dict, Config.Freeze.anim, 1, 1.0, -1, 0, 1.0, false, false, false);
			});
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "freezePet", e.toString());
	}
}

//Function to sit down/get up pet
function sitPet(pet, status, msg = true) {
	try {
		if (pet) {
			if (status) {
				global.requestAnimDict(Config.Sit.dict).then(async () => {
					pet.taskPlayAnim (Config.Sit.dict, Config.Sit.anim, 1, 1.0, -1, 1, 1.0, false, false, false);
				});
	
				if (msg) 
					mp.events.call('notify', 4, 9, translateText("Питомец сел."), 3000);
			} else {
				global.requestAnimDict(Config.Freeze.dict).then(async () => {
					pet.taskPlayAnim (Config.Freeze.dict, Config.Freeze.anim, 1, 1.0, -1, 0, 1.0, false, false, false);
				});
	
				if (msg) 
					mp.events.call('notify', 4, 9, translateText("Питомец встал."), 3000);
			}
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "sitPet", e.toString());
	}
}

//Function to sleep/wake up pet
function sleepPet(pet, status, msg = true) {
	try {
		if (pet) {
			if (status) {
				global.requestAnimDict(Config.Sleep.dict).then(async () => {
					pet.taskPlayAnim (Config.Sleep.dict, Config.Sleep.anim, 1, 1.0, -1, 1, 1.0, false, false, false);
				});
	
				   if(msg) 
					   mp.events.call('notify', 4, 9, translateText("Питомец лёг."), 3000);			
			} else {
				global.requestAnimDict(Config.Freeze.dict).then(async () => {
					pet.taskPlayAnim (Config.Freeze.dict, Config.Freeze.anim, 1, 1.0, -1, 0, 1.0, false, false, false);
				});
				
				if(msg)
					mp.events.call('notify', 4, 9, translateText("Питомец встал."), 3000);			
			}
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "sleepPet", e.toString());
	}
}
//Function to find ball
function findBall(pet) {
	try {
		if (pet) {
			const pedPosition = pet.getCoords(true);
			let ball = mp.game.object.getClosestObjectOfType(pedPosition.x, pedPosition.y, pedPosition.z, 100.0, mp.game.joaat('w_am_baseball'), false, true, true);
			if(ball && weaponData.weapon != mp.game.joaat('weapon_ball')) {
				petData.ballHandle = ball;
				Natives.FREEZE_ENTITY_POSITION (ball, true);
				petData.ballPosition = Natives.GET_ENTITY_COORDS(ball, false);
				pet.gettingBall = true;
			}
			//else mp.game.ui.notifications.showWithPicture("Mascota", "", "Primero debes tirar la pelota", "CHAR_CHOP", 7, true);     
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "findBall", e.toString());
	}
}

gm.events.add('client.pet.dellball', (xPos, yPos, zPos) => {
	try {
		let ball = mp.game.object.getClosestObjectOfType(xPos, yPos, zPos, 100.0, mp.game.joaat('w_am_baseball'), false, true, true);
		if (ball) {
			Natives.SET_ENTITY_AS_MISSION_ENTITY (ball, true, true);
			mp.game.object.deleteObject(ball);
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "dellball", e.toString());
	}
});

//Следовать или не следовать за мной
gm.events.add('client.pet.follow', () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		const pedPosition = petData.entity.getCoords(true);
		const localPedPosition = global.localplayer.position;
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, localPedPosition.x, localPedPosition.y, localPedPosition.z, true);
		if (distance >= 7.0) 
		{
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
		
		ClearAttackInterval ();
	
		petData.entity.freeze = !petData.entity.freeze;
		petData.entity.sit = false;
		petData.entity.sleep = false;
	
		if (!petData.entity.freeze) mp.events.call('notify', 4, 9, translateText("Питомец будет следовать за Вами."), 3000);
		else 
		{
			freezePet(petData.entity);
			mp.events.call('notify', 4, 9, translateText("Питомец будет ждать тут."), 3000);
		}
	
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "follow", e.toString());
	}
});

gm.events.add('client.pet.unFreeze', () => 
{
	if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	petData.entity.freeze = false;
	updateMenu ();
});

gm.events.add('client.pet.sit', async () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		const pedPosition = petData.entity.getCoords(true);
		gm.discord(translateText("Играет с питомцем"));
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 10.0) {
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		} else if (petData.entity.sleep) {
			mp.events.call('notify', 4, 9, translateText("Чтобы выполнить это действие, питомец не должен лежать."), 3000);
			return;
		}
	
		ClearAttackInterval ();
		
		petData.entity.freeze = true;
		petData.entity.sit = !petData.entity.sit;
	
		sitPet (petData.entity, petData.entity.sit);
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "initPet", e.toString());
	}
});


gm.events.add('client.pet.getBall', async () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		const pedPosition = petData.entity.getCoords(true);
		gm.discord(translateText("Играет с питомцем"));
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 15.0) {
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
	
		ClearAttackInterval ();
	
		petData.entity.freeze = false;
		petData.entity.sleep = false;
		petData.entity.sit = false;
		petData.followType = 0;
	
		findBall (petData.entity);
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "getBall", e.toString());
	}
});

gm.events.add('client.pet.sleep', async () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		gm.discord(translateText("Играет с питомцем"));
		const pedPosition = petData.entity.getCoords(true);
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 10.0) {
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
	
		ClearAttackInterval ();
	
		petData.entity.freeze = false;
		petData.entity.sit = false;
		petData.entity.sleep = !petData.entity.sleep;
		petData.followType = 0;
	
		sleepPet (petData.entity, petData.entity.sleep);
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "sleep", e.toString());
	}
});

const ClearAttackInterval = (isAttack = false) => {
	try {
		if (petData.attack !== null || petData.attackPet !== null || isAttack) {
			//petData.entity.clearTasks();
			//petData.entity.clearTasksImmediately();
			//freezePet(petData.entity);



			//

			petData.attack = null;
			petData.attackPet = null;
			updateMenu ();
			petData.followType = 0;

			petData.entity.clearTasksImmediately();
			taskToPlayer (global.localplayer);
		} 
		if (petData.toFollow !== null) {
			freezePet(petData.entity);
			petData.toFollow = null;
			updateMenu ();
			petData.followType = 0;
		} 
		if (petData.toSniff !== null) {

			if (petData.toSniffTimer != 0)
				clearTimeout(petData.toSniffTimer);

			freezePet(petData.entity);
			petData.toSniff = null;
			petData.toSniffTimer = 0;
			updateMenu ();
			petData.followType = 0;
			petData.entity.freeze = false;
		} 
		if (petData.entity.gettingBall || petData.ballThrown) {
			petData.entity.freeze = false;
			petData.followType = 0;
			petData.ballPosition = null;
			petData.ballHandle = null;
			petData.entity.gettingBall = false;
			petData.ballThrown = false; 
		}
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "ClearAttackInterval", e.toString());
	}
}

gm.events.add('client.pet.attack', (playerId) => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	
		const player = mp.players.atRemoteId(playerId);
		if (!player) return;
		else if (petData.entity.isInCombat(player.handle)) return;
		else if (player.vehicle) return;
		const pedPosition = petData.entity.getCoords(true);
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 35.0) {
			//mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
	
		petData.entity.freeze = false;
		petData.attack = player;
		petData.followType = 0;
	
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "attack", e.toString());
	}
});

gm.events.add('client.pet.sniff', (playerId) => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	
		const player = mp.players.atRemoteId(playerId);
		if (!player) return;
		else if (petData.entity.isInCombat(player.handle)) return;
		else if (player.vehicle) return;
		const pedPosition = petData.entity.getCoords(true);
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 35.0) {
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
	
		petData.entity.freeze = false;
		petData.toSniff = player;
		petData.followType = 0;
	
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "sniff", e.toString());
	}
});

gm.events.add('client.pet.toFollow', (playerId) => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	
		const player = mp.players.atRemoteId(playerId);
		if (!player) return;
		else if (petData.entity.isInCombat(player.handle)) return;
		else if (player.vehicle) return;
		const pedPosition = petData.entity.getCoords(true);
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 35.0) {
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
	
		petData.entity.freeze = false;
		petData.toFollow = player;
		petData.followType = 0;
	
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "toFollow", e.toString());
	}
});

gm.events.add('client.pet.clear', () => {
	if (!petData.entity || !mp.peds.exists(petData.entity)) return;

	ClearAttackInterval ();
});


const PetNoAttack = [1462895032, 2910340283, 1125994524, 1832265812, 2971380566, 3462393972];
const PetAttack = [2506301981, 1318032802, 1126154828];
const PetSleep = [2971380566, 2506301981, 1318032802, 1126154828, 307287994, 3877461608];
const PetSit = [2971380566, 2506301981, 1318032802, 1126154828, 307287994, 3877461608];
const PetSniff = [];

const updateMenu = () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) 
			return;
			
		let commandsArray = [
			{
				name: petData.entity.freeze ? translateText("Следовать") : translateText("Ждать"),
				event: 'client.pet.follow'
			}
		]
	
		if (PetAttack.includes (petData.entity.model)) {
			commandsArray.push ({
				name: petData.attack === null ? translateText("Атаковать") : translateText("Отменить атаку"),
				event: petData.attack === null ? 'client.pet.getPlayers.attack' : 'client.pet.clear',
			});
			commandsArray.push ({
				name: petData.attackPet === null ? translateText("Атаковать Питомцев") : translateText("Отменить атаку"),
				event: petData.attackPet === null ? 'client.pet.getPet.attack' : 'client.pet.clear',
			});
		}
		
		if (PetSit.includes (petData.entity.model)) commandsArray.push ({
			name: !petData.entity.sit ? translateText("Сесть") : translateText("Встать"),
			event: 'client.pet.sit'
		});
		
		if (PetSleep.includes (petData.entity.model)) commandsArray.push ({
			name: !petData.entity.sleep ? translateText("Лечь") : translateText("Встать"),
			event: 'client.pet.sleep'
		});
	
		commandsArray = [
			...commandsArray,
			{
				name: translateText("Принести мяч"),
				event: 'client.pet.getBall'
			},
			{
				name: translateText("Обнюхать"),
				event: petData.toSniff === null ? 'client.pet.getPlayers.sniff' : 'client.pet.clear',
			},
		];

		if (PetSniff.includes (petData.entity.model)) commandsArray.push ({
			name: translateText("Преследовать"),
			event: petData.toFollow === null ? 'client.pet.getPlayers.follow' : 'client.pet.clear',
		});

		commandsArray.push ({
			name: translateText("Накормить"),
			event: 'client.pet.setEat'
		});
	
		mp.gui.emmit(`window.events.callEvent("cef.pet.menu", '${JSON.stringify (commandsArray)}')`);
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "updateMenu", e.toString());
	}
}

gm.events.add('client.pet.setEat', async () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		mp.events.callRemote("server.pet.setEat");
		gm.discord(translateText("Кормит питомца"));

	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "updateMenu", e.toString());
	}	
});
gm.events.add('client.pet.getPlayers.attack', () => {
	if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	getPlayers ("client.pet.attack");
});

gm.events.add('client.pet.getPlayers.sniff', () => {
	if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	getPlayers ("client.pet.sniff");
});

gm.events.add('client.pet.getPlayers.follow', () => {
	if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	getPlayers ("client.pet.toFollow");
});

const isAddPlayerToList = (player) => {
	try {
		if (!player || !mp.players.exists(player)) return false;
		else if (!player.handle) return false;
		else if (player == global.localplayer) return false;
		else if (player['AGM']) return false;
		else if (player["InDeath"]) return false;
		else if (player["INVISIBLE"]) return false;
		else if (player.getHealth() < 1) return false;
		else if (player.isInAnyVehicle(false)) return false;
		return true;
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "updateMenu", e.toString());
	}
	return false;
}

const getPlayers = (events) => {	
	try {
		const localPos = global.localplayer.position;
		let players = [];
		mp.players.forEachInStreamRange((player) => {
			if (isAddPlayerToList (player)) {
				let position = player.position;
				let dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
				players.push ({
					name: global.getName (player),
					dist: dist,
					pId: player.remoteId
				})
			}
		});
	
		players.sort(function (a, b) {
			if (a.dist > b.dist) {
				return 1;
			}
			if (a.dist < b.dist) {
				return -1;
			}
			// a должно быть равным b
			return 0;
		});
	
		players = players.splice (0, 9);
	
		players.push({
			name: translateText("Назад"),
			isEnd: true
		});
	
		mp.gui.emmit(`window.events.callEvent("cef.pet.player", '${JSON.stringify (players)}', '${events}')`);
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "getPlayers", e.toString());
	}
}

//Pet
gm.events.add('client.pet.getPet.attack', () => {
	if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	getPeds ("client.pet.attackPet");
});

gm.events.add('client.pet.attackPet', (pedId) => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
	
		const ped = mp.peds.atRemoteId(pedId);
		if (!ped) return;
		else if (petData.entity.isInCombat(ped.handle)) return;
		else if (ped.vehicle) return;
		const pedPosition = petData.entity.getCoords(true);
		const distance = mp.game.gameplay.getDistanceBetweenCoords(pedPosition.x, pedPosition.y, pedPosition.z, global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, true);
		if (distance >= 35.0) {
			mp.events.call('notify', 4, 9, translateText("Подойдите ближе к питомцу, чтобы отдавать ему команды."), 3000);
			return;
		}
	
		petData.entity.freeze = false;
		petData.attack = null;
		petData.attackPet = ped;
		petData.followType = 0;
	
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "attack", e.toString());
	}
});

const isAddPedToList = (ped) => {
	try {
		if (!ped || !mp.peds.exists(ped)) return false;
		else if (!ped.handle) return false;
		else if (ped == petData.entity) return false;
		else if (!ped.getVariable("isPet")) return false;
		return true;
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "updateMenu", e.toString());
	}
	return false;
}

gm.events.add('client.pet.nameChange', (newPetName) => {
	try {
		mp.gui.emmit(`window.hudStore.animalName ('${newPetName}')`);
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "nameChange", e.toString());
	}
});

const getPeds = (events) => {	
	try {
		const localPos = global.localplayer.position;
		let peds = [];
		mp.peds.forEachInStreamRange((ped) => {
			if (isAddPedToList (ped)) {
				let position = ped.position;
				let dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
				peds.push ({
					name: ped.getVariable("petName"),
					dist: dist,
					pId: ped.remoteId
				})
			}
		});
	
		peds.sort(function (a, b) {
			if (a.dist > b.dist) {
				return 1;
			}
			if (a.dist < b.dist) {
				return -1;
			}
			// a должно быть равным b
			return 0;
		});
	
		peds = peds.splice (0, 9);
	
		peds.push({
			name: translateText("Назад"),
			isEnd: true
		});
	
		mp.gui.emmit(`window.events.callEvent("cef.pet.player", '${JSON.stringify (peds)}', '${events}')`);
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "getPeds", e.toString());
	}
}

gm.events.add('client.pet.setInVehicle', () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		const vehicle = global.localplayer.vehicle;
		
		if (!vehicle || !mp.vehicles.exists(vehicle)) 
		{
			petData.entity.freeze = false;
			return;
		}
		
		gm.discord(translateText("Катается с питомцем"));
		ClearAttackInterval ();
		
		let vehMaxNumberOfPassengers = vehicle.getMaxNumberOfPassengers();
		for(var i = 0; i < vehMaxNumberOfPassengers; i++) 
		{
			if (vehicle.isSeatFree(i)) 
			{
				petData.entity.setIntoVehicle(vehicle.handle, i);
				return;
			}
		}
		mp.events.call('notify', 4, 9, translateText("Питомец не смог найти свободное место и теперь ждет новую команду!"), 7000);
		petData.entity.freeze = true;
		freezePet(petData.entity);
		petData.entity.sit = true;
		sitPet(petData.entity, petData.entity.sit, false);
		updateMenu ();
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "world/petSystem", "setInVehicle", e.toString());
	}
});

/////////////

let isOpenPetShop = false;

const Open = async (json) => {
    try
    {        
        if (global.menuCheck()) 
			return;

        global.menuOpen();
		isOpenPetShop = true;

        mp.gui.emmit(`window.router.setView("BusinessNewPetShop", '${json}');`);
		gm.discord(translateText("Присматривает себе питомца"));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/newshop/index", "client.shop.open", e.toString());
    }
}

gm.events.add('client.petshop.open', Open);


const OnPetshopBuy = (index) => {
    mp.events.callRemote('server.petshop.buy', index);
}

gm.events.add('client.petshop.buy', OnPetshopBuy);

const OnClose = () => {
    if (!isOpenPetShop)
		return;

	global.menuClose();
	mp.gui.emmit('window.router.setHud();');
	isOpenPetShop = false;
}

gm.events.add('client.petshop.close', OnClose);


gm.events.add('client.pet.follow', () => {
	try {
		if (!petData.entity || !mp.peds.exists(petData.entity)) return;
		
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/newshop/index", "client.shop.open", e.toString());
    }
});