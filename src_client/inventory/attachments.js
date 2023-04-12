mp.attachments = {
	attachments: {},

	addFor: async function(entity, id)
	{
		try
		{
			if(!this.attachments)
				this.attachments = {};

			if(this.attachments.hasOwnProperty(id))
			{
				if(!entity.__attachmentObjects) 
					entity.__attachmentObjects = {};

				if(!entity.__attachmentObjects.hasOwnProperty(id))
				{
					let attInfo = this.attachments[id];

					let object = mp.objects.new(attInfo.model, entity.position, {
						dimension: -1
					});

					await global.IsLoadEntity (object);
					
					if (object && object.handle && entity && entity.handle) {

						object.attachTo(entity.handle,
							(typeof(attInfo.boneName) === 'string') ? entity.getBoneIndexByName(attInfo.boneName) : entity.getBoneIndex(attInfo.boneName),
							attInfo.offset.x, attInfo.offset.y, attInfo.offset.z, 
							attInfo.rotation.x, attInfo.rotation.y, attInfo.rotation.z, 
							false, false, false, false, 2, attInfo.fixedRot);
							
						if (entity.type === 'vehicle')
							object.setLodDist(global.getLodDist (global.DistanceVehicle));
						else 
							object.setLodDist(global.getLodDist (global.DistancePlayer));	

						entity.__attachmentObjects[id] = object;					
					}
					else if(mp.objects.exists(object)) 
						object.destroy();
					
					//if(id == "scubatank" && entity == global.localplayer) global.localplayer.setMaxTimeUnderwater(400);
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "addFor", e.toString());
		}
	},
	
	removeFor: function(entity, id)
	{
		try
		{
			if(!entity.__attachmentObjects)
				entity.__attachmentObjects = {};

			if(entity.__attachmentObjects.hasOwnProperty(id))
			{
				let obj = entity.__attachmentObjects[id];
				delete entity.__attachmentObjects[id];
				
				if(mp.objects.exists(obj))
				{
					obj.destroy();
				}
				//if(id == "scubatank" && entity == global.localplayer) global.localplayer.setMaxTimeUnderwater(10);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "removeFor", e.toString());
		}
	},
	
	initFor: function(entity)
	{
		try
		{
			for(let attachment of entity.__attachments)
			{
				mp.attachments.addFor(entity, attachment);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "initFor", e.toString());
		}
	},
	
	shutdownFor: function(entity)
	{
		try
		{
			for(let attachment in entity.__attachmentObjects)
			{
				mp.attachments.removeFor(entity, attachment);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "shutdownFor", e.toString());
		}
	},
	
	register: function(id, model, boneName, offset, rotation, fixedRot = true)
	{
		try
		{
			if(typeof(id) === 'string')
			{
				id = mp.game.joaat(id);
			}
			
			if(typeof(model) === 'string')
			{
				model = mp.game.joaat(model);
			}

			if(!this.attachments)
				this.attachments = {};

			if(!this.attachments.hasOwnProperty(id))
			{
				if(mp.game.streaming.isModelInCdimage(model))
				{
					this.attachments[id] = {
						id: id,
						model: model,
						offset: offset,
						rotation: rotation,
						boneName: boneName,
						fixedRot: fixedRot
					};
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "register", e.toString());
		}
	},
	
	unregister: function(id) 
	{
		try
		{
			if(typeof(id) === 'string')
			{
				id = mp.game.joaat(id);
			}

			if(!this.attachments)
				this.attachments = {};

			if(this.attachments.hasOwnProperty(id))
			{
				this.attachments[id] = undefined;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "unregister", e.toString());
		}
	},
	
	addLocal: function(attachmentName)
	{
		try
		{
			if(typeof(attachmentName) === 'string')
			{
				attachmentName = mp.game.joaat(attachmentName);
			}
			
			let entity = global.localplayer;
			
			if(!entity.__attachments || entity.__attachments.indexOf(attachmentName) === -1)
			{
				mp.events.callRemote("staticAttachments.Add", String (attachmentName));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "addLocal", e.toString());
		}
	},
	
	removeLocal: function(attachmentName)
	{
		try
		{
			if(typeof(attachmentName) === 'string')
			{
				attachmentName = mp.game.joaat(attachmentName);
			}
			
			let entity = global.localplayer;
			
			if(entity.__attachments && entity.__attachments.indexOf(attachmentName) !== -1)
			{
				mp.events.callRemote("staticAttachments.Remove", String (attachmentName));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "inventory/attachments", "removeLocal", e.toString());
		}
	},
	
	getAttachments: function()
	{
		return Object.assign({}, this.attachments);
	}
};

global.IsLoadEntity = entity => new Promise(async (resolve, reject) => {
	try {
		if (entity && entity.doesExist() && entity.handle !== 0)
			return resolve(true);
        let d = 0;
		while (!entity || !entity.doesExist() || entity.handle === 0) {
            if (d > 1000) return resolve(translateText("Ошибка IsLoadEntity."));
            d++;
            await mp.game.waitAsync(10);
        }
        return resolve(true);
    } 
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "IsLoadEntity", e.toString());
		resolve();
	}
});

global.IsSeatVehicle = () => new Promise(async (resolve, reject) => {
	try {
		if (global.localplayer.vehicle)
			return resolve(true);
		let d = 0;
		while (!global.localplayer.vehicle) {
			if (d > 1000) return resolve(translateText("Ошибка IsSeatVehicle."));
			d++;
			await mp.game.waitAsync(10);
		}
		return resolve(true);
	}
	catch (e)
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "IsSeatVehicle", e.toString());
		resolve();
	}
});


gm.events.add("playerStreamIn", (entity) => {
	if (entity && entity.__attachments)
		mp.attachments.initFor(entity);
});

gm.events.add("playerStreamOut", (entity) => {
	if (entity && entity.__attachmentObjects)
		mp.attachments.shutdownFor(entity);
});

mp.events.addDataHandler("attachmentsData", (entity, data) =>
{
	try
	{
		let newAttachments = (data.length > 0) ? JSON.parse(data) : [];

		if (entity.handle !== 0) 
		{	
			let oldAttachments = entity.__attachments;	
		
			if(!oldAttachments)
			{
				oldAttachments = [];
				entity.__attachmentObjects = {};
			}
			
			// process outdated first
			for(let attachment of oldAttachments)
			{
				if(newAttachments.indexOf(attachment) === -1)
				{
					mp.attachments.removeFor(entity, attachment);
				}
			}
			
			// then new attachments
			for(let attachment of newAttachments)
			{
				if(oldAttachments.indexOf(attachment) === -1)
				{
					mp.attachments.addFor(entity, attachment);
				}
			}
		}
		entity.__attachments = newAttachments;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "attachmentsData", e.toString());
	}
});

gm.events.add("playerReady", () => {
    try {
		let data;
		let atts;
		mp.players.forEach(player =>
		{
			data = player.getVariable("attachmentsData");
			
			if(data && data.length > 0)
			{
				atts = (data.length > 0) ? JSON.parse(data) : [];
				
				if(!atts) 
					atts = [];

				player.__attachments = atts;
				player.__attachmentObjects = {};
			}
		});
    }
    catch (e) 
    {
		mp.events.callRemote("client_trycatch", "inventory/attachments", "playerReady", e.toString());
    }
});

function addItems()
{	
	mp.attachments.register(`spec1`, 1657647215, 'bodyshell', new mp.Vector3(0, 0, 0.0), new mp.Vector3(0, 0, 180));//ch_prop_casino_drone_02a
	mp.attachments.register(`spec2`, 442185650, 'bodyshell', new mp.Vector3(0, 0, 0.0), new mp.Vector3(0, 0, 180));//xs_prop_arena_drone_02
	mp.attachments.register(`spec3`, -388213579, 'bodyshell', new mp.Vector3(0, 0, 0.0), new mp.Vector3(0, 0, 180));//ba_prop_battle_cameradrone
	//mp.attachments.register(`spec1`, 1657647215, 'bodyshell', new mp.Vector3(0, 0, 0.0), new mp.Vector3(0, 0, 180));//ch_prop_casino_drone_02a
	//mp.attachments.register(`spec2`, 442185650, 'bodyshell', new mp.Vector3(0, 0, 0.0), new mp.Vector3(0, 0, 180));//xs_prop_arena_drone_02
	//mp.attachments.register(`spec3`, -388213579, 'bodyshell', new mp.Vector3(0, 0, 0.0), new mp.Vector3(0, 0, 180));//ba_prop_battle_cameradrone

	mp.attachments.register("beer", "prop_beer_stzopen", 57005, new mp.Vector3(0.13, -0.13, -0.07), new mp.Vector3(-80, 0, 0), true);
	mp.attachments.register("burger", "prop_cs_burger_01", 57005, new mp.Vector3(0.14,0.00,-0.06), new mp.Vector3(0, 0, 0), true);
	mp.attachments.register("hotdog", "prop_cs_hotdog_01", 57005, new mp.Vector3(0.146,0.0,-0.035), new mp.Vector3(-33.28,-153.187,203.25), true);
	mp.attachments.register("pizza", "prop_pizza_box_02", 18905, new mp.Vector3(-0.094,0.094,0.085), new mp.Vector3(148.3,0.0,-22.58), true);
	mp.attachments.register("sandwich", "prop_sandwich_01", 57005, new mp.Vector3(0.144,0.0,-0.03), new mp.Vector3(128.75,4.2,-59.33), true);
	mp.attachments.register("crisps", "prop_cs_crisps_01", 18905, new mp.Vector3(0.045,0.08,0.03), new mp.Vector3(93.02,22.13,-156.58), true);
	mp.attachments.register("joint", "prop_sh_joint_01", 57005, new mp.Vector3(0.175,0.0,0.01), new mp.Vector3(0.17,66.85,148.33), true);
    mp.attachments.register("ecola", "prop_ecola_can", 60309, new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), true);
	mp.attachments.register("sprunk", "apa_prop_cs_plastic_cup_01", 57005, new mp.Vector3(0.15,-0.09,-0.05), new mp.Vector3(-85.4, 0, 0), true);
    mp.attachments.register("guitar", "prop_acc_guitar_01", 24818, new mp.Vector3(-0.1, 0.31, 0.1), new mp.Vector3(10, -20, 150), true);
	mp.attachments.register("bongo", "prop_bongos_01", 24818, new mp.Vector3(-0.14,0.2,0.2), new mp.Vector3(51.74,150.41,-16.1), true);
	mp.attachments.register("press1", "prop_barbell_100kg", 57005, new mp.Vector3(0.146,0.34,0.025), new mp.Vector3(0.0,10.3,-101.08), true);
	mp.attachments.register("press2", "prop_curl_bar_01", 36029, new mp.Vector3(0.043,-0.107,0.237), new mp.Vector3(-4.41,-114.86,99.58), true);
	mp.attachments.register("elguitar", "prop_el_guitar_01", 24818, new mp.Vector3(-0.045,0.29,0.13), new mp.Vector3(11.73,-14.83,164.166), true);
    mp.attachments.register("cuffs", "p_cs_cuffs_02_s", 0x188e, new mp.Vector3(-0.02, 0.063, 0), new mp.Vector3(0x4b, 0, 0x4c), true);
	mp.attachments.register("moneybag", "prop_money_bag_01", 0x49d9, new mp.Vector3(0.55, 0.02, 0), new mp.Vector3(0, -90, 0), true);
	mp.attachments.register("postalobj", "prop_drug_package_02", 60309, new mp.Vector3(0.03, 0, 0.02), new mp.Vector3(0, 0, 50), true);
	mp.attachments.register("phonecall", "redagephone",  6286, new mp.Vector3(0.06, 0.01, -0.02), new mp.Vector3(80, -10, 110), true);
	mp.attachments.register('microphone', 'prop_microphone_02', 60309, new mp.Vector3(0.06715794, 0.03628302, -0.00216622), new mp.Vector3(243.8641, -12.80466, 10.23078), true);
	mp.attachments.register('vape', 'ba_prop_battle_vape_01', 18905, new mp.Vector3(0.11999999999989086, 0, 0.030000000000654836), new mp.Vector3(-180, 90, -20), true);

 
	mp.attachments.register("umbrella", 'p_amb_brolly_01', 57005, new mp.Vector3(0.09479946, 0.013351775, -0.020646578), new mp.Vector3(-76.90267, 5.92244, -32.74062), true),
	mp.attachments.register('rose', 'prop_single_rose', 57005, new mp.Vector3(0.13973124, 0.09400548, -0.008136311), new mp.Vector3(-86.43276, 0, -29.57296), true);
	mp.attachments.register("news_camera", "prop_v_cam_01", 28422, new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), true),
	mp.attachments.register("news_mic", 'p_ing_microphonel_01', 57005, new mp.Vector3(0.13055836, 0.07557731, -0.0057103653), new mp.Vector3(-83.314026, 7.7800093, -24.884037), true),
	mp.attachments.register("electric_guitar", "prop_el_guitar_01", 24818, new mp.Vector3(-0.1, 0.31, 0.1), new mp.Vector3(10, -20, 150), true),
	mp.attachments.register("binoculars", "prop_binoc_01", 60309, new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), true),
	mp.attachments.register("clipboard", "p_amb_clipboard_01", 60309, new mp.Vector3(0, 0, 0), new mp.Vector3(0, 0, 0), true),
	mp.attachments.register("bong", "prop_bong_01", 28422, new mp.Vector3(0.04849681, -0.044438273, -0.057797566), new mp.Vector3(-73.746086, 42.461754, 0.12613341), true),
	mp.attachments.register("teddy", "v_ilev_mr_rasberryclean", 24817, new mp.Vector3(-0.2, 0.46, -0.016), new mp.Vector3(-180, 90, 0), true);
	mp.attachments.register('barbell', 'prop_barbell_02', 57005, new mp.Vector3(0.07116705, -0.12621467, -0.22997302), new mp.Vector3(17.429564, 124.60293, 84.8938), true);


	mp.attachments.register('mine_pickaxe', 'prop_tool_pickaxe', 6286, new mp.Vector3(0.048031863, -0.025162484, -0.023895439), new mp.Vector3(-77.863785, 0, -5.8967414), true);
	mp.attachments.register('mine_rock', 'prop_rock_5_smash2', 6286, new mp.Vector3(0.12810344, 0.0496148, -0.24910454), new mp.Vector3(-85.96926, -111.396194, 11.526477), true);
	mp.attachments.register('work_axe', 'prop_ld_fireaxe', 6286, new mp.Vector3(0.063695654, 0.038963683, 0), new mp.Vector3(74.704636, -7.9448185, -172.23056), true);
	
	
	mp.attachments.register('ball', 'w_am_baseball', 17188, new mp.Vector3(0.120, 0.010, 0.010), new mp.Vector3(5.0, 150.0, 0.0), true);
	mp.attachments.register('vehicleNumber', 'p_num_plate_02', 57005, new mp.Vector3(0.2573055, 0.06773748, 0.014193673), new mp.Vector3(277.4668, -3.8297656, 0.867015), true);

	mp.attachments.register('neonstick', 'prop_parking_wand_01', 57005, new mp.Vector3(0.12700854, -0.019614343, -0.016310496), new mp.Vector3(-86.301476, 0, 0), true);
	mp.attachments.register('neonstickr', 'prop_parking_wand_01', 60309, new mp.Vector3(0.05823519, -0.025888821, 0.032461915), new mp.Vector3(-116.43026, 1.0740396, 18.005377), true);

	mp.attachments.register('glowstick', 'ba_prop_battle_glowstick_01', 60309, new mp.Vector3(0.07497374, 0.05300144, 0.0041695107), new mp.Vector3(69.2257, 0, 0), true);
	mp.attachments.register('glowstickr', 'ba_prop_battle_glowstick_01', 57005, new mp.Vector3(0.12962468, 0.051268853, 0), new mp.Vector3(-63.400352, 2.1992705, -6.131192), true);
	

	//petData.ball.attachTo(petData.entity.handle, petData.entity.getBoneIndex(17188), 0.120, 0.010, 0.010, 5.0, 150.0, 0.0, true, true, false, true, 1, true);



}

function addWeapons()
{
	let weapons = 
	[
		[ "Pistol", 1467525553, 0 ],
		[ "VintagePistol", -1124046276, 0 ],
		[ "APPistol", 905830540, 0 ],
		[ "CombatPistol", 403140669, 0 ],
		[ "Revolver", 914615883, 0 ],
		[ "SNSPistol", 339962010, 0 ],
		[ "HeavyPistol", 1927398017, 0 ],
		[ "Pistol50", -178484015, 0 ],
		[ "NavyRevolver", 2200574582, 0 ],
		[ "CeramicPistol", 3924381353, 0 ],
		[ "MarksmanPistol", 4191177435, 0 ],
		[ "FlareGun", 1349014803, 0 ],
		[ "DoubleAction", 2050882666, 0 ],
		[ "PistolMk2", 995074671, 0 ],
		[ "SNSPistolMk2", 4221916961, 0 ],
		[ "RevolverMk2", 4065179617, 0 ],
		[ "Glock", 651271362, 0],
		
		[ "CombatPDW", -1393014804, 1 ],
		[ "MicroSMG", -1056713654, 1 ],
		[ "SMG", -500057996, 1 ],
		[ "MiniSMG", -972823051, 1 ],
		[ "MachinePistol", -331545829, 1 ],
		[ "AssaultSMG", -473574177, 1 ],
		[ "RayCarbine", 377247090, 1 ],
		[ "MG", 2238602894, 1 ],
		[ "CombatMG", 3555572849, 1 ],
		[ "Gusenberg", 574348740, 1 ],
		[ "SMGMk2", 2547423399, 1 ],
		[ "CombatMGMk2", 2969831089, 1 ],
		
		[ "CarbineRifle", 1026431720, 2 ],
		[ "AssaultRifle", 273925117, 2 ],
		[ "SpecialCarbine", -1745643757, 2 ],
		[ "MarksmanRifle", -1711248638, 2 ], 
		[ "AdvancedRifle", 2587382322, 2 ], 
		[ "BullpupRifle", 3006407723, 2 ], 
		[ "CompactRifle", 1931114084, 2 ], 
		[ "AssaultRifleMk2", 1762764713, 2 ], 
		[ "CarbineRifleMk2", 1520780799, 2 ], 
		[ "SpecialCarbineMk2", 2379721761, 2 ], 
		[ "BullpupRifleMk2", 1415744902, 2 ], 
		//[ "MilitaryRifle", -1658906650, 2 ],
		[ "TacticalRifle", 3520460075, 2 ],
		[ "HeavyRifle", 1493691718, 2 ],
		[ "CombatRifle", 3673305557, 2 ],
		
		
		[ "PumpShotgun", 689760839, 3 ],
		[ "HeavyShotgun", -1209868881, 3 ],
		[ "AssaultShotgun", 1255410010, 3 ],
		[ "BullpupShotgun", -1598212834, 3 ],
		[ "SawnOffShotgun", 3619125910, 3 ],
		[ "Musket", 1652015642, 3 ],
		[ "DoubleBarrelShotgun", 222483357, 3 ],
		[ "SweeperShotgun", 1380588314, 3 ],
		[ "PumpShotgunMk2", 3194406291, 3 ],
		[ "CombatShotgun", 487013001, 3 ]
	];
	
	let offset = new mp.Vector3(0.0, 0.0, 0.0);
	let rotation = new mp.Vector3(0.0, 0.0, 0.0);
	
	for(let weap of weapons)
	{
		let bone = 0;
		
		switch (weap[2])
		{
			case 0:
				bone = 51826;
				offset = new mp.Vector3(0.02, 0.06, 0.1);
				rotation = new mp.Vector3(-100.0, 0.0, 0.0);
				break;

			case 1:
				bone = 58271;
				offset = new mp.Vector3(0.08, 0.03, -0.1);
				rotation = new mp.Vector3(-80.77, 0.0, 0.0);
				break;

			case 2:
				bone = 24818;
				offset = new mp.Vector3(-0.1, -0.15, 0.11);
				rotation = new mp.Vector3(-180.0, 0.0, 0.0);
				break;

			case 3:
				bone = 24818;
				offset = new mp.Vector3(-0.1, -0.15, -0.13);
				rotation = new mp.Vector3(0.0, 0.0, 3.5);
				break;
		}
		mp.attachments.register(weap[0], weap[1], bone, offset, rotation);
	}
}

function initMenu()
{
	addItems();
	addWeapons();
}

initMenu();

/*
let attachEditor = {
	attachment: false,
	offset: new mp.Vector3(),
	rotation: new mp.Vector3(),
	model: "lr_prop_carkey_fob",
	boneName: 60309
}

gm.events.add('objecteditor:start', (model, boneName, offset = new mp.Vector3(), rotation = new mp.Vector3(), fixedRot = true) => {
	try
	{
		if (global.menuCheck()) return;
		global.menuOpen();
		
		let object = mp.objects.new(mp.game.joaat(model), global.localplayer.position);
					
		object.attachTo(global.localplayer.handle,
			(typeof(boneName) === 'string') ? global.localplayer.getBoneIndexByName(boneName) : global.localplayer.getBoneIndex(boneName),
			offset.x, offset.y, offset.z, 
			rotation.x, rotation.y, rotation.z, 
			false, false, false, false, 2, fixedRot);

		
		attachEditor = {
			attachment: true,
			offset: offset,
			rotation: rotation,
			model: mp.game.joaat(model),
			boneName: boneName,
			fixedRot: fixedRot,
			object: object
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "objecteditor:start", e.toString());
	}
});


var res = mp.game.graphics.getScreenActiveResolution(0, 0);

let MOVE_SENSITIVTY = 50;
let ROT_SENSITIVITY = 800;

let oldPos;
let oldRot;
let mode = 'Move';
let curBtn;
let oldcursorPos = [0, 0];

let xbox;
let ybox;
let zbox;
let switchbox;
let groundbox;
let cancelbox;
let savebox;

new class extends debugRender {
    constructor() {
        super("r_inventory_attachments");
    }
    render () {
		if (attachEditor.attachment && mp.objects.exists(attachEditor.object)) {
			const attachment = new mp.Vector3((attachEditor.object.position.x + attachEditor.offset.y), (attachEditor.object.position.y + attachEditor.offset.z), (attachEditor.object.position.z - attachEditor.offset.x));

			mp.game.graphics.drawLine(attachment.x - 1.0, attachment.y, attachment.z, attachment.x + 1.0, attachment.y, attachment.z, 0, 0, 255, 255);
			mp.game.graphics.drawLine(attachment.x, attachment.y - 1.0, attachment.z, attachment.x, attachment.y + 1.0, attachment.z, 255, 0, 0, 255);
			mp.game.graphics.drawLine(attachment.x, attachment.y, attachment.z - 1.0, attachment.x, attachment.y, attachment.z + 1.0, 0, 255, 0, 255);

			xbox = mp.game.graphics.world3dToScreen2d(attachment.x + 1.5, attachment.y, attachment.z);
			ybox = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y + 1.5, attachment.z);
			zbox = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y, attachment.z + 1.5);
			switchbox = mp.game.graphics.world3dToScreen2d(attachment.x - 0.8, attachment.y - 0.8, attachment.z);
			if(switchbox != undefined) {
				groundbox = {x: switchbox.x+0.065, y: switchbox.y};
				cancelbox = {x: switchbox.x+0.13, y: switchbox.y};
				savebox = {x: switchbox.x+0.195, y: switchbox.y};
			} else {
				cancelbox = undefined, savebox = undefined;
			}

			if(xbox != undefined) {
				mp.game.graphics.drawRect(xbox.x, xbox.y, 0.015, 0.026, 0, 0, 255, 255);
				mp.game.graphics.drawText('X', [xbox.x, xbox.y-0.015], { 
					font: 2, 
					color: [255, 255, 255, 255], 
					scale: [0.5, 0.5], 
					outline: false
				});
			}
			if(ybox != undefined) {
				mp.game.graphics.drawRect(ybox.x, ybox.y, 0.015, 0.026, 255, 0, 0, 255);
				mp.game.graphics.drawText('Y', [ybox.x, ybox.y-0.016], { 
					font: 2, 
					color: [255, 255, 255, 255], 
					scale: [0.5, 0.5], 
					outline: false
				});
			}
			if(zbox != undefined) {
				mp.game.graphics.drawRect(zbox.x, zbox.y, 0.015, 0.026, 0, 255, 0, 255);
				mp.game.graphics.drawText('Z', [zbox.x, zbox.y-0.016], { 
					font: 2, 
					color: [255, 255, 255, 255], 
					scale: [0.5, 0.5], 
					outline: false
				});
			}
			if(switchbox != undefined) {
				mp.game.graphics.drawRect(switchbox.x, switchbox.y, 0.06, 0.026, 255, 255, 255, 255);
				mp.game.graphics.drawRect(groundbox.x, groundbox.y, 0.06, 0.026, 255, 255, 255, 255);
				mp.game.graphics.drawRect(cancelbox.x, cancelbox.y, 0.06, 0.026, 255, 255, 255, 255);
				mp.game.graphics.drawRect(savebox.x, savebox.y, 0.06, 0.026, 255, 255, 255, 255);
				mp.game.graphics.drawText(mode == 'Move' ? 'Rotate' : 'Move', [switchbox.x, switchbox.y-0.016], { 
					font: 0, 
					color: [0, 0, 0, 255], 
					scale: [0.4, 0.4], 
					outline: false
				});
				mp.game.graphics.drawText('Ground', [groundbox.x, groundbox.y-0.016], { 
					font: 0, 
					color: [0, 0, 0, 255], 
					scale: [0.4, 0.4], 
					outline: false
				});
				mp.game.graphics.drawText('Cancel', [cancelbox.x, cancelbox.y-0.016], { 
					font: 0, 
					color: [0, 0, 0, 255], 
					scale: [0.4, 0.4], 
					outline: false
				});
				mp.game.graphics.drawText('Save', [savebox.x, savebox.y-0.016], { 
					font: 0, 
					color: [0, 0, 0, 255], 
					scale: [0.4, 0.4], 
					outline: false
				});
			}

			let pos = mp.gui.cursor.position;
			let cursorDir = {x: pos[0]-oldcursorPos[0], y: pos[1]-oldcursorPos[1]};
			cursorDir.x /= res.x;
			cursorDir.y /= res.y;

			if(curBtn == 'x') { 
				let mainPos = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y, attachment.z);
				let refPos;
				if(mode == 'Move') {
					refPos = mp.game.graphics.world3dToScreen2d(attachment.x+1, attachment.y, attachment.z);
				} else {
					refPos = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y+1, attachment.z);
				}
				if(mainPos == undefined || refPos == undefined) return;
				var screenDir = {x: refPos.x-mainPos.x, y: refPos.y-mainPos.y};
				var magnitude = cursorDir.x*screenDir.x + cursorDir.y*screenDir.y;
				if(mode == 'Move') {
					attachEditor.offset = new mp.Vector3(attachEditor.offset.x, attachEditor.offset.y, attachEditor.offset.z+magnitude*MOVE_SENSITIVTY);
				} else {
					attachEditor.rotation = new mp.Vector3(attachEditor.rotation.x, attachEditor.rotation.y, attachEditor.rotation.z+cursorDir.x*ROT_SENSITIVITY*0.2); //Here direction can be determined by just x axis of mouse, hence the *0.2
				}
				UpdateObj ();
				
			} else if(curBtn == 'y') {
				let mainPos = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y, attachment.z);
				let refPos;
				if(mode == 'Move') {
					refPos = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y+1, attachment.z);
				} else {
					refPos = mp.game.graphics.world3dToScreen2d(attachment.x+1, attachment.y, attachment.z);
				}
				if(mainPos == undefined || refPos == undefined) return;
				var screenDir = {x: refPos.x-mainPos.x, y: refPos.y-mainPos.y};
				var magnitude = cursorDir.x*screenDir.x + cursorDir.y*screenDir.y;
				if(mode == 'Move') {
					attachEditor.offset = new mp.Vector3(attachEditor.offset.x, attachEditor.offset.y+magnitude*MOVE_SENSITIVTY, attachEditor.offset.z);
				} else {
					attachEditor.rotation = new mp.Vector3(attachEditor.rotation.x, attachEditor.rotation.y+magnitude*ROT_SENSITIVITY, attachEditor.rotation.z);
				}
				
				UpdateObj ();
				
			} else if(curBtn == 'z') {
				let mainPos = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y, attachment.z);
				let refPos = mp.game.graphics.world3dToScreen2d(attachment.x, attachment.y, attachment.z+1);
				if(mainPos == undefined || refPos == undefined) return;
				var screenDir = {x: refPos.x-mainPos.x, y: refPos.y-mainPos.y};
				var magnitude = cursorDir.x*screenDir.x + cursorDir.y*screenDir.y;
				if(mode == 'Move') {
					attachEditor.offset = new mp.Vector3(attachEditor.offset.x+magnitude*MOVE_SENSITIVTY, attachEditor.offset.y, attachEditor.offset.z);
				} else {
					attachEditor.rotation = new mp.Vector3(attachEditor.rotation.x-magnitude*ROT_SENSITIVITY, attachEditor.rotation.y, attachEditor.rotation.z);
				}		
				UpdateObj ();	
			}
			oldcursorPos = pos;
		}
	}
};

const UpdateObj = () => {
	try
	{
		if (!attachEditor.attachment) return;
		let obj = attachEditor.object;


		delete attachEditor.object;

		if (mp.objects.exists(obj))
		{
			obj.destroy();
		}
		obj = mp.objects.new(attachEditor.model, global.localplayer.position);
		
		obj.attachTo(global.localplayer.handle,
			(typeof(attachEditor.boneName) === 'string') ? global.localplayer.getBoneIndexByName(attachEditor.boneName) : global.localplayer.getBoneIndex(attachEditor.boneName),
			attachEditor.offset.x, attachEditor.offset.y, attachEditor.offset.z, 
			attachEditor.rotation.x, attachEditor.rotation.y, attachEditor.rotation.z, 
			false, false, false, false, 2, attachEditor.fixedRot);

		attachEditor.object = obj;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "UpdateObj", e.toString());
	}
}

gm.events.add('click', (x, y, upOrDown, leftOrRight, relativeX, relativeY, worldPosition, hitEntity) => {
	try
	{
		if(!attachEditor.attachment) return;
    
		let mouseRel = {x: x/res.x, y: y/res.y};

		if (upOrDown == 'up') {
			curBtn = '';
		} else if (upOrDown == 'down') {
			if(xbox != undefined && mouseRel.x >= xbox.x-0.01 && mouseRel.x <= xbox.x+0.009 && mouseRel.y >= xbox.y-0.015 && mouseRel.y <= xbox.y+0.009) {
				curBtn = 'x';
			} else if(ybox != undefined && mouseRel.x >= ybox.x-0.01 && mouseRel.x <= ybox.x+0.009 && mouseRel.y >= ybox.y-0.015 && mouseRel.y <= ybox.y+0.009) {
				curBtn = 'y';
			} else if(zbox != undefined && mouseRel.x >= zbox.x-0.01 && mouseRel.x <= zbox.x+0.009 && mouseRel.y >= zbox.y-0.015 && mouseRel.y <= zbox.y+0.009) {
				curBtn = 'z';
			} else if(switchbox != undefined && mouseRel.x >= switchbox.x-0.03 && mouseRel.x <= switchbox.x+0.03 && mouseRel.y >= switchbox.y-0.015 && mouseRel.y <= switchbox.y+0.009) {
				switchMode();
			} else if(groundbox != undefined && mouseRel.x >= groundbox.x-0.03 && mouseRel.x <= groundbox.x+0.03 && mouseRel.y >= groundbox.y-0.015 && mouseRel.y <= groundbox.y+0.009) {
				groundObject();
			} else if(cancelbox != undefined && mouseRel.x >= cancelbox.x-0.03 && mouseRel.x <= cancelbox.x+0.03 && mouseRel.y >= cancelbox.y-0.015 && mouseRel.y <= cancelbox.y+0.009) {
				cancel();
			} else if(savebox != undefined && mouseRel.x >= savebox.x-0.03 && mouseRel.x <= savebox.x+0.03 && mouseRel.y >= savebox.y-0.015 && mouseRel.y <= savebox.y+0.009) {
				saveChanges();
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "click", e.toString());
	}
});

function switchMode() {
	if(!attachEditor.attachment) return;
    mode = (mode == 'Move' ? 'Rotation' : 'Move');
}

function groundObject() {
	if(!attachEditor.attachment) return;
    global.menuClose();
}

function cancel() {
	try
	{
		if(!attachEditor.attachment) return;
		global.menuClose();
		
		let obj = attachEditor.object
		delete attachEditor.object;
		if (mp.objects.exists(obj)) obj.destroy();
		
		attachEditor = {
			attachment: false,
			offset: new mp.Vector3(),
			rotation: new mp.Vector3(),
			model: "lr_prop_carkey_fob",
			boneName: 60309
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "cancel", e.toString());
	}
}

function saveChanges() {
	try
	{
		if(!attachEditor.attachment) return;
		mp.events.callRemote('staticAttachments.Save', attachEditor.object.model, JSON.stringify(attachEditor.offset), JSON.stringify(attachEditor.rotation));
		global.menuClose();
		
		let obj = attachEditor.object
		delete attachEditor.object;
		if (mp.objects.exists(obj)) obj.destroy();
		
		attachEditor = {
			attachment: false,
			offset: new mp.Vector3(),
			rotation: new mp.Vector3(),
			model: "lr_prop_carkey_fob",
			boneName: 60309
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "saveChanges", e.toString());
	}
}*/


let Toggled = false;

let EditEntity;

let UseModel = "";

let BoneId = 0;
let BoneIndex = 0;

let SelectedButton = "";

let SelectedMod = false;

let IsDowned = false;

let attachOffset = new mp.Vector3();
let attachRotation = new mp.Vector3();

let MouseSensitivity = 50.0;

let MouseRotSensitivity = 800.0;

let LastCursorPos;

let Buttons = {
	x: [0.0, 0.0, 0.0, 0.0],
	y: [0.0, 0.0, 0.0, 0.0],
	z: [0.0, 0.0, 0.0, 0.0],
};

mp.keys.bind(global.Keys.VK_RETURN, false, () => Save());
mp.keys.bind(global.Keys.VK_R, false, () => ChangeMode());
mp.keys.bind(global.Keys.VK_ESCAPE, false, () => DisableEditor());

function Save() {
	if (Toggled)
	{
		/*Vector3 bonePos = global.localplayer.getBoneCoords(BoneId, 0.0, 0.0, 0.0);
		Vector3 objPos = EditEntity.position;
		Vector3 objRot = EditEntity.GetRotation(2);

		RAGE.Chat.Output(bonePos.ToString());
		RAGE.Chat.Output(objPos.ToString());
		Vector3 attachOffset = objPos - bonePos;
		RAGE.Chat.Output(attachOffset.ToString());

		RAGE.Game.Entity.AttachEntityToEntity(EditEntity.handle, global.localplayer.handle, BoneIndex, attachOffset.x, attachOffset.y, attachOffset.z, objRot.x, objRot.y, objRot.z, true, false, false, false, 0, true);
		*/

		//RAGE.Chat.Output($"attachOffset {attachOffset}");
		//RAGE.Chat.Output($"attachOffset {attachRotation}");
		//BoneIndex, attachOffset.x, attachOffset.y, attachOffset.z, attachRotation.x, attachRotation.y, attachRotation.z
		mp.events.callRemote('staticAttachments.Save', UseModel, BoneId, JSON.stringify (attachOffset), JSON.stringify (attachRotation));
		DisableEditor();
	}
}

function ChangeMode() {
	if (Toggled)
	{
		SelectedMod = !SelectedMod;
	}
}

gm.events.add('objecteditor:start', (model, boneId) => {
	OnToggle(true, model, boneId)
});

function OnToggle(toggle, model, boneId) {
	if (toggle)
		EnableEditor(model, boneId);
	else
		DisableEditor();
}

const EnableEditor = async (model, boneId) => {
	try 
	{
		if (global.menuCheck()) return;
		Toggled = true;

		UseModel = model;

		BoneId = boneId;
		BoneIndex = (typeof(boneId) === 'string') ? global.localplayer.getBoneIndexByName(boneId) : global.localplayer.getBoneIndex(boneId);

		if (mp.objects.exists(EditEntity)) EditEntity.destroy();

		EditEntity = mp.objects.new(mp.game.joaat(model), global.localplayer.position, new mp.Vector3());

		await global.IsLoadEntity (EditEntity);

		EditEntity.setCollision(false, false);
		EditEntity.setNoCollision(global.localplayer.handle, false);

		attachOffset = new mp.Vector3();
		attachRotation = new mp.Vector3(); 

		EditEntity.attachTo(global.localplayer.handle, BoneIndex, attachOffset.x, attachOffset.y, attachOffset.z, attachRotation.x, attachRotation.y, attachRotation.z, true, false, false, false, 0, true);

		global.menuOpen();
		global.dropEditor = true;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "EnableEditor", e.toString());
	}
}

function DisableEditor() {
	if (!Toggled)
		return;
	Toggled = false;
	global.menuClose();
	global.dropEditor = false;
	if (mp.objects.exists(EditEntity)) EditEntity.destroy();
	UseModel = "";
}

/*
let mainPos = mp.game.graphics.world3dToScreen2d(selObj.position.x, selObj.position.y, selObj.position.z);
let refPos;
if(mode == 'Move') {
	refPos = mp.game.graphics.world3dToScreen2d(selObj.position.x+1, selObj.position.y, selObj.position.z);
} else {
	refPos = mp.game.graphics.world3dToScreen2d(selObj.position.x, selObj.position.y+1, selObj.position.z);
}
if(mainPos == undefined || refPos == undefined) return;
var screenDir = {x: refPos.x-mainPos.x, y: refPos.y-mainPos.y};
var magnitude = cursorDir.x*screenDir.x + cursorDir.y*screenDir.y;

*/
global.GetMagnitudeOffset = (pos, cursorDirX, cursorDirY, mainScreenX, mainScreenY, offsetX = 0.0, offsetY = 0.0, offsetZ = 0.0) => {
	try 
	{
		const refScreen = mp.game.graphics.world3dToScreen2d(pos.x + offsetX, pos.y + offsetY, pos.z + offsetZ);

		const screenDirX = refScreen.x - mainScreenX;
		const screenDirY = refScreen.y - mainScreenY;
		return cursorDirX * screenDirX + cursorDirY * screenDirY;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "GetMagnitudeOffset", e.toString());
	}
}

global.DrawAxis = (name, start, end, color) => {
	try 
	{
		if (start && end && start.x !== undefined && start.y !== undefined && start.z !== undefined && end.x !== undefined && end.y !== undefined && end.z !== undefined) {

			mp.game.graphics.drawLine(start.x, start.y, start.z, end.x, end.y, end.z, color[0], color[1], color[2], color[3]);

			const screen = mp.game.graphics.world3dToScreen2d(end.x, end.y, end.z);
	
			if (screen && screen.x !== undefined && screen.y !== undefined) {
				const buttonWidth = 0.01;
				const buttonHeight = 0.02;
				
				mp.game.graphics.drawRect(screen.x, screen.y, buttonWidth, buttonHeight, color[0], color[1], color[2], color[3]);
		
				mp.game.graphics.drawText(name, [screen.x, screen.y - 0.0115], { 
					font: 0, 
					color: [255, 255, 255, 255], 
					scale: [0.3, 0.3], 
					outline: false
				});
		
				return [screen.x, screen.y - 0.0115];
			}
		}
		return false;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/attachments", "DrawAxis", e.toString());
	}
}

gm.events.add("render", () => {
	if (!global.loggedin) return;
	if (Toggled && EditEntity != null && EditEntity.handle != 0) {
		const cursorPos = mp.gui.cursor.position;

		const activeResolution = mp.game.graphics.getScreenActiveResolution(0, 0);

		const
			resX = activeResolution.x,
			resY = activeResolution.y;


		const xStart = EditEntity.getOffsetFromInWorldCoords(-0.6, 0.0, 0.0);
		const xEnd = EditEntity.getOffsetFromInWorldCoords(0.6, 0.0, 0.0);
		Buttons["x"] = DrawAxis(!SelectedMod ? "X" : "RX", xStart, xEnd, [153, 153, 204, (SelectedButton == "x" ? 255 : 150)]);

		const yStart = EditEntity.getOffsetFromInWorldCoords(0.0, -0.6, 0.0);
		const yEnd = EditEntity.getOffsetFromInWorldCoords(0.0, 0.6, 0.0);
		Buttons["y"] = DrawAxis(!SelectedMod ? "Y" : "RY", yStart, yEnd, [190, 143, 143, (SelectedButton == "y" ? 255 : 150)]);

		const zStart = EditEntity.getOffsetFromInWorldCoords(0.0, 0.0, -0.6);
		const zEnd = EditEntity.getOffsetFromInWorldCoords(0.0, 0.0, 0.6);
		Buttons["z"] = DrawAxis(!SelectedMod ? "Z" : "RZ", zStart, zEnd, [140, 180, 139, (SelectedButton == "z" ? 255 : 150)]);

		mp.game.controls.enableControlAction(0, 237, true);

		if (mp.game.controls.isControlPressed(0, 237)) {
			if (!IsDowned) {

				IsDowned = true;

				if (SelectedButton == "") {
					const x = cursorPos[0] / resX;
					const y = cursorPos[1] / resY;
					for (let key in Buttons) {
						const position = Buttons [key];
						const dist = mp.game.system.vdist(x, y, 0.0, position[0], position[1], 0.0);
						if (0.015 >= dist)
						{
							SelectedButton = key;
						}
					}
				}
			}
		} else {
			IsDowned = false;
			SelectedButton = "";
		}

		if (SelectedButton != "")
		{
			const position = EditEntity.position;
			//const rotation = EditEntity.GetRotation(2);

			const cursorDirX = (cursorPos[0] - LastCursorPos[0]) / resX;
			const cursorDirY = (cursorPos[1] - LastCursorPos[1]) / resY;

			const screen = mp.game.graphics.world3dToScreen2d(position.x, position.y, position.z);

			const
				mainScreenX = screen.x,
				mainScreenY = screen.y;

			switch (SelectedButton)
			{
				case "x":
					{
						const magnitude = GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, -1.0);
						if (!SelectedMod)
							attachOffset.x += (magnitude * MouseSensitivity);
						else
							attachRotation.x += (magnitude * MouseRotSensitivity);
					}
					break;
				case "y":
					{
						const magnitude = GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0.0, -1.0);
						if (!SelectedMod)
							attachOffset.y += (magnitude * MouseSensitivity);
						else
							attachRotation.y += (magnitude * MouseRotSensitivity);
					}
					break;
				case "z":
					{
						const magnitude = GetMagnitudeOffset(position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0.0, -1.0);
						if (!SelectedMod)
							attachOffset.z += (magnitude * MouseSensitivity);
						else
							attachRotation.z += (magnitude * MouseRotSensitivity);
					}
					break;
			}

			// EditEntity.position = position;
			// EditEntity.SetRotation(rotation.x, rotation.y, rotation.z, 2, false);
			EditEntity.attachTo(global.localplayer.handle, BoneIndex, attachOffset.x, attachOffset.y, attachOffset.z, attachRotation.x, attachRotation.y, attachRotation.z, true, false, false, false, 0, true);
		}

		if (!mp.keys.isDown(global.Keys.VK_SPACE)) {
			mp.game.controls.disableAllControlActions(0);
			mp.gui.cursor.visible = true;
		} else {
			mp.gui.cursor.visible = false;
		}

		LastCursorPos = cursorPos;
	}
});

global.ToggleMovementControls = () => {
	mp.game.controls.disableControlAction(0, global.Inputs.MOVE_UP_ONLY, true);
	mp.game.controls.disableControlAction(0, global.Inputs.MOVE_DOWN_ONLY, true);
	mp.game.controls.disableControlAction(0, global.Inputs.MOVE_LEFT_ONLY, true);
	mp.game.controls.disableControlAction(0, global.Inputs.MOVE_RIGHT_ONLY, true);

	mp.game.controls.disableControlAction(0, global.Inputs.VEH_MOVE_UP_ONLY, true);
	mp.game.controls.disableControlAction(0, global.Inputs.VEH_MOVE_DOWN_ONLY, true);
	mp.game.controls.disableControlAction(0, global.Inputs.VEH_MOVE_LEFT_ONLY, true);
	mp.game.controls.disableControlAction(0, global.Inputs.VEH_MOVE_RIGHT_ONLY, true);

	mp.game.controls.disableControlAction(0, global.Inputs.JUMP, true);
}

global.ToggleFightControls = () => {
	mp.game.controls.disableControlAction(0, global.Inputs.VEH_AIM, true);
	mp.game.controls.disableControlAction(0, global.Inputs.VEH_ATTACK, true);
	mp.game.controls.disableControlAction(0, global.Inputs.VEH_ATTACK2, true);
}