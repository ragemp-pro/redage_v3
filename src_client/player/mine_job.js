const mine_job = {
	markers: [],
    colshapes: [],

    gov_markers: [],
    gov_colshapes: [],

    gov_warehouse_marker: mp.markers.new(1, new mp.Vector3(-595.855, 2094.609, 131.4356 - 4.5), 5.25, { visible: true, dimension: 0, color: [255, 255, 255, 220] }),
    gov_warehouse_shape: mp.colshapes.newSphere(-595.855, 2094.609, 131.4356, 5),
    
    can_press: -1,
    gov_place: false,
    interval: undefined,

    mining_seconds: 0,
    mining_needful_time: 11,

    ore_carry_status: false
};

const mine_places = [
    { x: 135, y: -365, z: 42 },
    { x: 138, y: -353, z: 42 },
    { x: 144, y: -358, z: 42 },
    { x: 118, y: -443, z: 40 },
    { x: 113, y: -434, z: 40 },
    { x: 120, y: -434, z: 40 },
    { x: 99, y: -388, z: 40 },
    { x: 107, y: -387, z: 40 },
    { x: 102, y: -393, z: 40 }

];
const gov_mine_place = [
    { x: -588.0355, y: 2058.172, z: 130.7677 }
];

for (let i = 0; i < mine_places.length; i++) {
    mine_job.markers.push(
        mp.markers.new(1, new mp.Vector3(mine_places[i].x, mine_places[i].y, mine_places[i].z - 5.25), 6, {
            visible: true,
            dimension: 0,
            color: [255, 255, 255, 220] 
        })
    );
    
    mine_job.colshapes.push(
        mp.colshapes.newSphere(mine_places[i].x, mine_places[i].y, mine_places[i].z, 6)
    );
}

for (let i = 0; i < gov_mine_place.length; i++) {
    mine_job.gov_markers.push(
        mp.markers.new(1, new mp.Vector3(gov_mine_place[i].x, gov_mine_place[i].y, gov_mine_place[i].z - 5.25), 6, {
            visible: true,
            dimension: 0,
            color: [255, 255, 255, 220] 
        })
    );
    
    mine_job.gov_colshapes.push(
        mp.colshapes.newSphere(gov_mine_place[i].x, gov_mine_place[i].y, gov_mine_place[i].z, 6)
    );
}

mp.game.entity.createModelHide(-596.0465, 2088.56, 130.587, 3, mp.game.joaat('prop_mineshaft_door'), true);

gm.events.add('playerEnterColshape', (shape) => {
    for (let i = 0; i < mine_job.colshapes.length; i++) {
        if (shape === mine_job.colshapes[i]) {
            mine_job.can_press = i;
            mine_job.gov_place = false;
            mine_job.mining_seconds = 0;

            mp.gui.emmit(`window.events.callEvent("cef.hud.game.open", 69, 1, 8, false)`);

            if (mine_job.interval !== undefined) {
                clearInterval(mine_job.interval);
                mine_job.interval = undefined;
            }
        }
    }

    for (let i = 0; i < mine_job.gov_colshapes.length; i++) {
        if (shape === mine_job.gov_colshapes[i]) {
            if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 3000) {
                mp.events.callRemote('PlayerEnteredMineShape', i);
            }
                
            global.anti_flood = new Date().getTime();
        }
    }

    if (shape === mine_job.gov_warehouse_shape && mine_job.ore_carry_status === true) {
        mp.events.callRemote('PlayerPutGovResources');
    }
});

gm.events.add('playerExitColshape', (shape) => {
    for (let i = 0; i < mine_job.colshapes.length; i++) {
        if (shape === mine_job.colshapes[i]) {
            mine_job.can_press = -1;
            mine_job.gov_place = false;

            mp.gui.emmit(`window.events.callEvent("cef.hud.game.close")`);

            if (global.startedMining === true) {
                mp.events.call('mineJob_stopMining');
            }
        }
    }

    for (let i = 0; i < mine_job.gov_colshapes.length; i++) {
        if (shape === mine_job.gov_colshapes[i]) {
            mine_job.can_press = -1;
            mine_job.gov_place = false;

            mp.gui.emmit(`window.events.callEvent("cef.hud.game.close")`);

            if (global.startedMining === true) {
                mp.events.call('mineJob_stopMining');
            }
        }
    }
});

gm.events.add('confirmPlayerEnteredMineShape', (zone) => {
    try {
        mine_job.can_press = zone;
        mine_job.gov_place = true;
        mine_job.mining_seconds = 0;
        
        mp.gui.emmit(`window.events.callEvent("cef.hud.game.open", 69, 1, 8, false)`);
        
        if (mine_job.interval !== undefined) {
            clearInterval(mine_job.interval);
            mine_job.interval = undefined;
        }
    }
	catch (e) 
	{
		if (new Date().getTime() - global.trycatchtime["player/mine_job"] < 5000) return;
		global.trycatchtime["player/mine_job"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/mine_job", "confirmPlayerEnteredMineShape", e.toString());
	}
});

gm.events.add('mineJob_updateToolInfo', (type) => {
    try {
        if (type == 1) { // Обычная кирка
            mine_job.mining_needful_time = 15;
        }
        else if (type == 2) { // Усиленная кирка
            mine_job.mining_needful_time = 13;
        }
        else if (type == 3) { // Профессиональная кирка
            mine_job.mining_needful_time = 11;
        }
    }
	catch (e) 
	{
		if (new Date().getTime() - global.trycatchtime["player/mine_job"] < 5000) return;
		global.trycatchtime["player/mine_job"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/mine_job", "mineJob_updateToolInfo", e.toString());
	}
});

gm.events.add('mineJob_updateOreCarryStatus', (status) => {
    mine_job.ore_carry_status = status;
});

gm.events.add('mineJob_startMining', () => {
    try {
        global.startedMining = true;

        global.localplayer.freezePosition(true);
        global.menuOpened = true; // чтобы запретить открывать инвентарь и биндер
        gm.discord(translateText("Копает руду на шахте"));
        
        mp.gui.emmit(`window.events.callEvent("cef.hud.game.close")`);
        mp.gui.emmit(`window.events.callEvent("cef.hud.game.open", 69, 1, 8, false)`);

        if (mine_job.interval !== undefined) {
            clearInterval(mine_job.interval);
            mine_job.interval = undefined;
        }

        mine_job.mining_seconds = 0;

        mine_job.interval = setInterval(() => {
            if (global.localplayer.vehicle || global.cuffed || global.isDeath === true || global.isDemorgan) {
                mp.events.call('mineJob_stopMining');
                return;
            }

            mine_job.mining_seconds += 0.5;

            mp.gui.emmit(`window.events.callEvent("cef.hud.game.updateProgress", ${(mine_job.mining_seconds + 0.5) / mine_job.mining_needful_time})`);
            
            if (mine_job.mining_seconds >= mine_job.mining_needful_time) {
                mine_job.mining_seconds = 0;
                mp.gui.emmit(`window.events.callEvent("cef.hud.game.updateProgress", ${0 / mine_job.mining_needful_time})`);
                
                if (mine_job.gov_place === true) {
                    global.startedMining = false;

                    if (mine_job.interval !== undefined) {
                        clearInterval(mine_job.interval);
                        mine_job.interval = undefined;
                    }

                    global.localplayer.freezePosition(false);
                    global.menuOpened = false; // чтобы запретить открывать инвентарь и биндер
                    
                    mp.events.callRemote('PlayerMinedGovResources');
                } else {
                    mp.events.callRemote('PlayerMinedResources');
                }
            }
        }, 500);
    }
	catch (e) 
	{
		if (new Date().getTime() - global.trycatchtime["player/mine_job"] < 5000) return;
		global.trycatchtime["player/mine_job"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/mine_job", "mineJob_startMining", e.toString());
	}
});

mp.keys.bind(0x45, true, () => {
    try {
        if (mine_job.can_press > -1 && !global.startedMining && !mine_job.ore_carry_status && !global.menuCheck ()) {
            if (global.localplayer.vehicle) {
                if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 3000) {
                    mp.events.call('notify', 1, 9, translateText("Вы должны выйти из транспорта."), 3000);
                }
                
                global.anti_flood = new Date().getTime();
                return;
            }

            if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 500) {
                global.GetItems(PickaxeItems);
            }
            
            global.anti_flood = new Date().getTime();
        }

        if (mine_job.can_press > -1 && global.startedMining === true) {
            mp.events.call('mineJob_stopMining');
        };
    }
	catch (e) 
	{
		if (new Date().getTime() - global.trycatchtime["player/mine_job"] < 3000) return;
		global.trycatchtime["player/mine_job"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/mine_job", "mp.keys.bind_0x45", e.toString());
	}
});

gm.events.add('mineJob_stopMining', () => {
    try {
        global.startedMining = false;

        if (mine_job.interval !== undefined) {
            clearInterval(mine_job.interval);
            mine_job.interval = undefined;
        }

        global.localplayer.freezePosition(false);
        global.menuOpened = false; // чтобы запретить открывать инвентарь и биндер
        mp.events.callRemote('PlayerFinishedMining');

        mp.gui.emmit(`window.events.callEvent("cef.hud.game.updateProgress", ${0 / mine_job.mining_needful_time})`);
    }
	catch (e) 
	{
		if (new Date().getTime() - global.trycatchtime["player/mine_job"] < 5000) return;
		global.trycatchtime["player/mine_job"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/mine_job", "mineJob_stopMining", e.toString());
	} 
});

const PickaxeItems = [236, 235, 234];

gm.events.add("render", () => {
    /*if (mine_job.can_press > -1 && mp.game.controls.isControlJustPressed(0, 38) && !mine_job.ore_carry_status && !global.menuCheck ()) {
        if (global.localplayer.vehicle) {
            if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 3000) {
                mp.events.call('notify', 1, 9, translateText("Вы должны выйти из транспорта."), 3000);
            }

            global.anti_flood = new Date().getTime();
            return;
        }

        if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 1000) {
            //mp.events.callRemote('PlayerStartedMining');
            global.GetItems(PickaxeItems);
        }

        global.anti_flood = new Date().getTime();
    }

    if (mine_job.can_press > -1 && mp.game.controls.isControlJustReleased(0, 38) && global.startedMining === true) {
        mp.events.call('mineJob_stopMining');
    }*/

    if (mine_job.ore_carry_status === true) {
        if (global.localplayer.isFalling() || global.localplayer.isCuffed() || global.localplayer.isFatallyInjured() || global.localplayer.isShooting() || global.localplayer.isSwimming() || global.localplayer.isClimbing()) {
            if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 3000) {
                mp.events.call('notify', 1, 9, translateText("Вы уронили добытый ресурс."), 3000);
                mp.events.callRemote('PlayerStopCarryOre');
            }

            global.anti_flood = new Date().getTime();
        }
    }
});

const isPickaxe = (rItemId) => {
    let toggled = false;
    PickaxeItems.forEach(itemId => {
        if (rItemId.includes(itemId))
            toggled = true;
    });
    return toggled;
}

gm.events.add('client.inventory.GetItem', (rItemId, toggled) => {
    try {
        rItemId = JSON.parse(rItemId);
        if (isPickaxe(rItemId) && toggled) {
            if (rItemId.includes(PickaxeItems[0])) { // Профессиональная кирка
                mine_job.mining_needful_time = 11;
            }
            else if (rItemId.includes(PickaxeItems[1])) { // Усиленная кирка
                mine_job.mining_needful_time = 13;
            }
            else if (rItemId.includes(PickaxeItems[2])) { // Обычная кирка
                mine_job.mining_needful_time = 15;
            }
            else {
                return mp.events.call('notify', 1, 9, translateText("У Вас нет кирки."), 3000);
            }
            mp.events.callRemote('PlayerStartedMining');
        }
        else {
            mp.events.call('notify', 1, 9, translateText("У Вас нет кирки."), 3000);
        }
    }
	catch (e) 
	{
		//mp.events.callRemote("client_trycatch", "player/mine_job", "GetItem", e.toString());
	} 
});

//

gm.events.add('resourceSell_playerSell', (state, index, amount) => {
    if (state == 0) mp.events.callRemote("PlayerSellOres", index, amount);
    else if (state == 1) mp.events.callRemote("PlayerSellTrees", index, amount);
});

gm.events.add('resourceSell_openMenu', (state, json) => {
    if (global.menuCheck()) return;
    global.menuOpen();

    // todo delete
    if (state == 1) {
        json = JSON.stringify({0: {Price: 9, ItemId: 245}, 1: {Price: 19, ItemId: 246}, 2: {Price: 30, ItemId: 247}});
    }

    mp.gui.emmit(`window.router.setView("PlayerOresSale", [${state}, '${json}']);`);
    gm.discord(translateText("Продаёт руду"));
});

gm.events.add('resourceSell_closeMenu', () => {
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
});