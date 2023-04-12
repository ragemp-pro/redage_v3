global.requestAnimDict = (animDictionary, isDeleteDict = false) => new Promise(async (resolve, reject) => {
    if (mp.game.streaming.hasAnimDictLoaded(animDictionary))
        return resolve(true);
    mp.game.streaming.requestAnimDict(animDictionary);

    let time = 0;
    while (!mp.game.streaming.hasAnimDictLoaded(animDictionary)) {
        if (time > 5000)
            return resolve(translateText("Ошибка requestAnimDict. Dict: ") + animDictionary);
        time++;        
        await global.wait (0);
    }

    if (!isDeleteDict)
        onClearLoadData("requestAnimDict", animDictionary);

    return resolve(true);
});

let loadsData = {
    requestAnimDict: {},
    requestNamedPtfxAsset: {},
};

const onClearLoadData = (name, dict) => {
    const loadData = loadsData[name][dict];
    if (loadData) {
        //loadData.cancel();
        //delete loadsData[name][dict];
		return;
    }

    loadsData[name][dict] = global.wait(60000).then(() => {
        clearLoadData(name, dict);
        delete loadsData[name][dict];
    });
};

const clearLoadData = (name, dict) => {
    if ("string" == typeof dict) {
        switch (name) {
            case "requestAnimDict":
                mp.game.streaming.removeAnimDict(dict);
                break;
            case "requestNamedPtfxAsset":
                mp.game.streaming.removeNamedPtfxAsset(dict);
                break;
        }
    }
};


global.ANTIANIM = false;

const setAnim = (entity, data) => {
	try 
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0 && !entity.vehicle) {
			//if (global.GetGender (entity) === -1) return;
			//let isStream = false;
			if (!data) {
				data = entity.getVariable("ANIM_USE");
				//isStream = false;
			}
			if(data != undefined && data != "null") 
			{
				data = data.split("|");
				if (data && data.length >= 3) {
					if (!mp.game.streaming.doesAnimDictExist(data[0])) return;
					global.requestAnimDict(data[0]).then(async () => {
						if (entity.handle === global.localplayer.handle)
							global.ANTIANIM = true;
						entity.taskPlayAnim (data[0], data[1], 2.0, entity.handle === global.localplayer.handle ? 8 : 0, -1, Number (data[2]), 0, false, false, false);
						mp.game.streaming.removeAnimDict(data[0]);
						if (data[3] && entity.handle === global.localplayer.handle)
							mp.attachments.addLocal (data[3]);
					});
					return;
				}
			}
			if (entity.handle === global.localplayer.handle) {
				mp.gui.emmit(`window.UpdateButtonText('', '');`);
				global.ANTIANIM = false;
			}
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/animation", "setAnim", e.toString());
	}
}

mp.events.addDataHandler("ANIM_USE", (entity, value, oldValue) => {
    setAnim (entity, value);
});

gm.events.add("playerStreamIn", (entity) => {
	setAnim(entity, null);
});

gm.events.add('client.setAnim', (entity) => {
	setAnim (entity, null);
});

gm.events.add('taskPlayAnim', async (entity, animDict, animName, flag, clearTasks, attachmentName) => {
	try {
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0 && !entity.vehicle) {
			if (!mp.game.streaming.doesAnimDictExist(animDict)) return;
			global.requestAnimDict(animDict).then(async () => {
				entity.taskPlayAnim (animDict, animName, 2.0, entity.handle === global.localplayer.handle ? 8 : 0, -1, Number (flag), 0, false, false, false);
				mp.game.streaming.removeAnimDict(animDict);
				if (entity.handle === global.localplayer.handle) {
					global.ANTIANIM = true;
					if (attachmentName) mp.attachments.addLocal (attachmentName);
	
				}
				const endTime = mp.game.entity.getEntityAnimDuration(animDict, animName);
				if (endTime > 0) {
					await global.wait(1000 * endTime - 10);
					if (entity.isPlayingAnim(animDict, animName, 3)) {
						if (clearTasks) {
							entity.stopAnimTask(animDict, animName, 3);
							entity.clearTasksImmediately();
						} else {
							entity.stopAnimTask(animDict, animName, 3);
							//nativeInvoke ("CLEAR_PED_SECONDARY_TASK", global.localplayer.handle);
							//if (entity.handle === global.localplayer.handle || entity.isInAnyVehicle(false)) entity.clearTasksImmediately();
						}
					} else {						
						if (entity.handle === global.localplayer.handle || entity.isInAnyVehicle(false)) entity.clearTasksImmediately();
					}
				}
				
				if (entity.handle === global.localplayer.handle) {
					mp.events.callRemote("OffAnim");
					global.ANTIANIM = false;
					mp.gui.emmit(`window.UpdateButtonText('', '');`);
					if (attachmentName) mp.attachments.removeLocal (attachmentName);
				}
			}).catch(error => console.log("ANIM ERR: " + error))
		}
	}	
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/animation", "gm.events.add('taskPlayAnim'", e.toString());
	}
});



const PlayerAnimList = {
    "mineJob": {
        animDictionary: "anim@heists@box_carry@",
        animationName: "idle",
		flag: 49,
		playbackRate: 0,
		attachmentName: "radio"
    },
    "radio": {
        animDictionary: "random@arrests",
        animationName: "generic_radio_enter",
		flag: 50,
		playbackRate: 2.0,
		//attachmentName: "radio"
    },
    "pushVeh": {
        animDictionary: "missfinale_c2ig_11",
        animationName: "pushcar_offcliff_m",
		flag: 35,
		playbackRate: 0
    },
	"vape": {
        animDictionary: "mp_player_inteat@burger",
        animationName: "mp_player_int_eat_burger",
		flag: 49,
		playbackRate: 0,
		attachmentName: "vape"
    },
	"handshake": { 
        animDictionary: "mp_ped_interaction",
        animationName: "handshake_guy_a",
		flag: 39,
		playbackRate: 0,
		attachmentName: "handshake" /// ???
    },
	"drink": {
        animDictionary: "amb@world_human_drinking@beer@male@idle_a",
        animationName: "idle_c",
		flag: 49,
		playbackRate: 0,
		attachmentName: "drink" /// ???
    },
	"healthkit": {
        animDictionary: "amb@code_human_wander_texting_fat@female@enter",
        animationName: "enter",
		flag: 49,
		playbackRate: 0,
		attachmentName: "healthkit"
    },
	"smokeloop": {
        animDictionary: "anim@heists@humane_labs@finale@keycards",
        animationName: "ped_a_enter_loop",
		flag: 49,
		playbackRate: 0,
		attachmentName: "smokeloop" /// ???????
    },
	"bong": {
        animDictionary: "amb@code_human_in_car_mp_actions@smoke@std@rps@base",
        animationName: "idle_c",
		flag: 49,
		playbackRate: 0,
		attachmentName: "bong"
    },
	"rose": {
        animDictionary: "anim@heists@humane_labs@finale@keycards",
        animationName: "ped_b_enter_loop",
		flag: 49,
		playbackRate: 0,
		attachmentName: "rose"
    },
	"barbell": {
        animDictionary: "amb@world_human_muscle_free_weights@male@barbell@idle_a",
        animationName: "idle_a",
		flag: 49,
		playbackRate: 0,
		attachmentName: "barbell"
    },
	"binoculars": {
        animDictionary: "oddjobs@hunter",
        animationName: "binoculars_loop",
		flag: 49,
		playbackRate: 0,
		attachmentName: "binoculars"
    },
	"umbrella": {
        animDictionary: "anim@heists@humane_labs@finale@keycards",
        animationName: "ped_b_enter_loop",
		flag: 49,
		playbackRate: 0,
		attachmentName: "umbrella"
    },
	"mic": {
        animDictionary: "anim@heists@humane_labs@finale@keycards",
        animationName: "ped_b_enter_loop",
		flag: 49,
		playbackRate: 0,
		attachmentName: "mic"
    },
	"guitar": {
        animDictionary: "amb@lo_res_idles@",
        animationName: "world_human_musician_guitar_lo_res_base",
		flag: 49,
		playbackRate: 0,
		attachmentName: "guitar"
    },
	"camera": {
        animDictionary: "misscarsteal4@meltdown",
        animationName: "_rehearsal_camera_man",
		flag: 49,
		playbackRate: 0,
		attachmentName: "camera"
    },
	"eat": {
        animDictionary: "amb@code_human_wander_eating_donut@male@idle_a",
        animationName: "idle_b",
		flag: 49,
		playbackRate: 0,
		attachmentName: "eat" // ??????????
    },
	"joint": {
        animDictionary: "amb@code_human_wander_smoking@male@idle_a",
        animationName: "idle_a",
		flag: 49,
		playbackRate: 0,
		attachmentName: "joint"
    },
	"cola": {
        animDictionary: "amb@code_human_wander_drinking@male@idle_a",
        animationName: "idle_c",
		flag: 49,
		playbackRate: 0,
		attachmentName: "cola" // ???????
    },
	"enterpet": {
        animDictionary: "amb@world_human_sunbathe@male@back@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "enterpet" // ????????
    },
	"creatorcam": {
        animDictionary: "amb@world_human_stand_guard@male@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "creatorcam" // ??????????
    },
	"sit": {
        animDictionary: "switch@michael@sitting",
        animationName: "idle",
		flag: 39,
		playbackRate: 0,
		attachmentName: "sit"
    },
	"lezhat": {
        animDictionary: "amb@world_human_sunbathe@male@back@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "lezhat"
    },
	"olezhat": {
        animDictionary: "missheistfbi3b_ig8_2",
        animationName: "cower_loop_victim",
		flag: 39,
		playbackRate: 0,
		attachmentName: "olezhat"
    },
	"pguitar": {
        animDictionary: "amb@world_human_musician@guitar@male@base",
        animationName: "base",
		flag: 49,
		playbackRate: 0,
		attachmentName: "pguitar"
    },
	"pbongo": {
        animDictionary: "amb@world_human_musician@bongos@male@base",
        animationName: "base",
		flag: 49,
		playbackRate: 0,
		attachmentName: "pbongo"
    },
	"jim": {
        animDictionary: "amb@prop_human_seat_muscle_bench_press@idle_a",
        animationName: "idle_a",
		flag: 39,
		playbackRate: 0,
		attachmentName: "jim"
    },
	"tyag": {
        animDictionary: "amb@prop_human_muscle_chin_ups@male@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "tyag"
    },
	"jimstoya": {
        animDictionary: "amb@world_human_muscle_free_weights@male@barbell@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "jimstoya"
    },
	"press": {
        animDictionary: "amb@world_human_sit_ups@male@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "press"
    },
	"otjim": {
        animDictionary: "amb@world_human_push_ups@male@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "otjim"
    },
	"playeguitar": {
        animDictionary: "amb@world_human_musician@guitar@male@base",
        animationName: "base",
		flag: 49,
		playbackRate: 0,
		attachmentName: "playeguitar"
    },
	"vzlommeb": {
        animDictionary: "amb@code_human_wander_texting@male@base",
        animationName: "static",
		flag: 39,
		playbackRate: 0,
		attachmentName: "vzlommeb"
    },
	"bagazhnik": {
        animDictionary: "timetable@floyd@cryingonbed@base",
        animationName: "base",
		flag: 33,
		playbackRate: 0,
		attachmentName: "bagazhnik"
    },
	"vzlomsafe": {
        animDictionary: "mini@safe_cracking",
        animationName: "idle_base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "vzlomsafe"
    },
	"repaircar": {
        animDictionary: "anim@amb@garage@chassis_repair@",
        animationName: "base_amy_skater_01",
		flag: 39,
		playbackRate: 0,
		attachmentName: "repaircar"
    },
	"revive": {
        animDictionary: "amb@medic@standing@tendtodead@idle_a",
        animationName: "idle_a",
		flag: 49,
		playbackRate: 0,
		attachmentName: "revive"
    },
	"airdropvzlom": {
        animDictionary: "mp_weapons_deal_sting",
        animationName: "crackhead_bag_loop",
		flag: 39,
		playbackRate: 0,
		attachmentName: "vzlomsafe"
    },
	"pickup_snowball": {
        animDictionary: "anim@mp_snowball",
        animationName: "pickup_snowball",
		flag: 39,
		playbackRate: 0,
		attachmentName: "pickup_snowball"
    },
	"dead": {
       // animDictionary: "????????????????",
       // animationName: "??????????????????",
		flag: 39,
		playbackRate: 0,
		attachmentName: "dead"
    },
	"arresting": {
        animDictionary: "mp_arresting",
        animationName: "idle",
		flag: 49,
		playbackRate: 0,
		attachmentName: "arresting"
    },
	"vzlomhouse": {
        animDictionary: "mini@safe_cracking",
        animationName: "idle_base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "vzlomhouse"
    },
	"itemraise": {
        animDictionary: "random@domestic",
        animationName: "pickup_low",
		flag: 39,
		playbackRate: 0,
		attachmentName: "itemraise"
    },
	"electric": {
        animDictionary: "amb@prop_human_movie_studio_light@base",
        animationName: "base",
		flag: 39,
		playbackRate: 0,
		attachmentName: "electric"
    },
	"gopostal": {
        animDictionary: "anim@heists@narcotics@trash",
        animationName: "drop_side",
		flag: -1,
		playbackRate: 0,
		attachmentName: "gopostal"
    },
	"lumberjack": {
        animDictionary: "melee@large_wpn@streamed_core",
        animationName: "car_side_attack_a",
		flag: 1,
		playbackRate: 0,
		attachmentName: "lumberjack"
    },
	"miner": {
        animDictionary: "melee@large_wpn@streamed_core",
        animationName: "car_side_attack_a",
		flag: 1,
		playbackRate: 0,
		attachmentName: "miner"
    },
	"cuff": {
        animDictionary: "mp_arresting",
        animationName: "idle",
		flag: 49,
		playbackRate: 0,
		attachmentName: "lumberjack"
    },
	"phonecall": {
        animDictionary: "anim@cellphone@in_car@ds",
        animationName: "cellphone_call_listen_base",
		flag: 49,
		playbackRate: 0,
		attachmentName: "phonecall"
    },
	"casino1": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "sit_enter_left",
		flag: 3,
		playbackRate: 0,
		attachmentName: "casino1"
    },
	"casino2": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "idle_a",
		flag: 3,
		playbackRate: 0,
		attachmentName: "casino2"
    },
	"casinobet": {
        animDictionary: "anim_casino_b@amb@casino@games@blackjack@player",
        animationName: "place_bet_small",
		flag: 3,
		playbackRate: 0,
		attachmentName: "casinobet"
    },
	"casinobet1": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "idle_var_01",
		flag: 3,
		playbackRate: 0,
		attachmentName: "casinobet1"
    },
	"animationgameend": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        //animationName: "???",
		flag: 3,
		playbackRate: 0,
		attachmentName: "animationgameend"
    },
	"exitcasino": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "sit_exit_left",
		flag: 39,
		playbackRate: 0,
		attachmentName: "exitcasino"
    },
	"sitting": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "sit_enter_left_side",
		flag: 3,
		playbackRate: 0,
		attachmentName: "sitting"
    },
	"sitting1": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "idle_a",
		flag: 3,
		playbackRate: 0,
		attachmentName: "sitting1"
    },
	"sitting2": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "sit_exit_left",
		flag: 3,
		playbackRate: 0,
		attachmentName: "sitting2"
    },
/*	"spin": {
        animDictionary: "anim_casino_a@amb@casino@games@slots@male OR anim_casino_a@amb@casino@games@slots@female",
        animationName: "press_betone_a",
		flag: 49,
		playbackRate: 0,
		attachmentName: "sitting"
    }, */
	"spin2": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "idle_a",
		flag: 3,
		playbackRate: 0,
		attachmentName: "spin2"
    },
	/* "spin3": {
        animDictionary: "anim_casino_a@amb@casino@games@slots@male OR anim_casino_a@amb@casino@games@slots@female",
        animationName: "win_spinning_wheel",
		flag: 49,
		playbackRate: 0,
		attachmentName: "spin3"
    },*/
	"spinenter": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "sit_enter_left_side",
		flag: 3,
		playbackRate: 0,
		attachmentName: "spinenter"
    },
	"spinleft": {
        animDictionary: "anim_casino_b@amb@casino@games@shared@player@",
        animationName: "sit_exit_left",
		flag: 39,
		playbackRate: 0,
		attachmentName: "spinleft"
    },
	"eatpet": {
        animDictionary: "mp_weapons_deal_sting",
        animationName: "crackhead_bag_loop",
		flag: 39,
		playbackRate: 0,
		attachmentName: "eatpet"
    },
	"phonecall": {
        animDictionary: "anim@cellphone@in_car@ds",
        animationName: "cellphone_call_listen_base",
		flag: 49,
		playbackRate: 0,
		//attachmentName: "phonecall"
    },
	"microphone": {
        animDictionary: "anim@heists@humane_labs@finale@keycards",
        animationName: "ped_b_enter_loop",
		flag: 49,
		playbackRate: 0,
		attachmentName: "microphone"
    },
    "sit": {
        animDictionary: "switch@michael@sitting", 
        animationName: "idle",
		flag: 35,
		playbackRate: 0,
		freeze: true,
		collision: true,
    },
}

gm.events.add('PlayAnimToKey', (entity, status, key) => {
    setAnimToKey (entity, status, key);
});


mp.events.addDataHandler("AnimToKey", (entity, value, oldValue) => {
	if (entity && mp.players.exists(entity) && entity.type === 'player') {
    	setAnimToKey (entity, -1, value);
		entity.AnimToKey = value;
	}
});

gm.events.add("playerStreamIn", (entity) => {
	setAnimToKey(entity, -1, -1);
});

const setAnimToKey = (entity, status, key) => {
    try {
        if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0 && !entity.vehicle) {
			if (!PlayerAnimList [key] && !PlayerAnimList [entity.AnimToKey])
				return;

			if (key != -1 && !PlayerAnimList [key] && PlayerAnimList [entity.AnimToKey]) 
				status = false;

			if (!PlayerAnimList [key])
				key = entity.AnimToKey;

			if (status == -1) {
				if (PlayerAnimList [key])
					status = true;
				else
					status = false;
			}

            if (status) {
				if (PlayerAnimList[key].collision) 
					entity.setCollision(false, true);

				if (PlayerAnimList[key].freeze) 
					entity.freezePosition(true);

                global.requestAnimDict(PlayerAnimList[key].animDictionary).then(async () => {

					entity.taskPlayAnim (PlayerAnimList[key].animDictionary, PlayerAnimList[key].animationName, 2.0, entity.handle === global.localplayer.handle ? 8 : 0, -1, PlayerAnimList[key].flag, 0, false, false, false);
                    mp.game.streaming.removeAnimDict(PlayerAnimList[key].animDictionary);                  

                    if (entity.handle === global.localplayer.handle) {
                        global.ANTIANIM = true;
						if (PlayerAnimList[key].attachmentName) 
							mp.attachments.addLocal (PlayerAnimList[key].attachmentName);
                    }
                })
            } else {
				if (PlayerAnimList[key].collision) 
					entity.setCollision(true, true);

				if (PlayerAnimList[key].freeze) 
					entity.freezePosition(false);

                if (entity.isPlayingAnim(PlayerAnimList[key].animDictionary, PlayerAnimList[key].animationName, 3)) {
                    entity.stopAnimTask(PlayerAnimList[key].animDictionary, PlayerAnimList[key].animationName, 3);
                    //entity.clearTasksImmediately();
                }

                if (entity.handle === global.localplayer.handle) {
                    global.ANTIANIM = false;
					mp.gui.emmit(`window.UpdateButtonText('', '');`);
					if (PlayerAnimList[key].attachmentName) mp.attachments.removeLocal (PlayerAnimList[key].attachmentName);
                }
            }
        }
    }
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/animation", "PlayAnimToKey", e.toString());
	}
}




/*
let indexAnim = 0;
let auto = 0;

gm.events.add('test.taskPlayAnim', async () => {
    mp.events.call('setTimeCmd', 12, 0, 0);
    mp.events.call('SetWeather', 0);

	auto = mp.vehicles.new(mp.game.joaat("cadctsv"), new mp.Vector3(-1553.7766, 83.61818, 8.099368), {
		heading: 136.96025 + 180,
		numberPlate: 'REDAGE',
		alpha: 255,
		color: [[207, 31, 33], [207, 31, 33]],
		locked: false,
		engine: false,
		dimension: 0
	});
	global.localplayer.setAlpha(0);
	auto.setCustomPrimaryColour(207, 31, 33);
	auto.setCustomSecondaryColour(207, 31, 33);
	auto.freezePosition(true);
	auto.setRotation(0, 0, 136.96025 + 180, 2, true);
    auto.position = new mp.Vector3(-1553.7766, 83.61818, 8.099368);
	//global.createCamera ("autoshop", auto);
});*/