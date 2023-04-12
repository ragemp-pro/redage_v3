const lumberjackJob = {
    treeIndex: -1,
    treeStage: 0,
    treePosition: undefined,
    treeHP: 100,
    interval: undefined,
    restartTimeout: undefined,
    process_count: 0
};

gm.events.add("lumberjackJob_setTreeInfo", (index, stage, position, hp) => {
    try
    {
        lumberjackJob.treeIndex = index;
        lumberjackJob.treeStage = stage;
        lumberjackJob.treePosition = position;
        lumberjackJob.treeHP = hp;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/lumberjack", "lumberjackJob_setTreeInfo", e.toString());
    }
});

gm.events.add("lumberjackJob_updateTreeInfo", (index, stage, hp) => {
    try
    {
        if (lumberjackJob.treeIndex == index) {
            lumberjackJob.treeStage = stage;
            lumberjackJob.treeHP = hp;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/lumberjack", "lumberjackJob_updateTreeInfo", e.toString());
    }
});

gm.events.add("lumberjackJob_clearInfo", () => {
    lumberjackJob.treeIndex = -1;
    lumberjackJob.treeStage = 0;
    lumberjackJob.treePosition = undefined;
    lumberjackJob.treeHP = 100;

    if (lumberjackJob.interval !== undefined) {
        clearInterval(lumberjackJob.interval);
        lumberjackJob.interval = undefined;
    }

    if (lumberjackJob.restartTimeout !== undefined) {
        clearTimeout(lumberjackJob.restartTimeout);
        lumberjackJob.restartTimeout = undefined;
    }
});

gm.events.add("lumberjackJob_startProcess", () => {
    try
    {
        if (lumberjackJob.treeIndex == -1) return;

        global.startedMining = true;

        global.localplayer.freezePosition(true);
        global.menuOpened = true; // чтобы запретить открывать инвентарь и биндер
        gm.discord(translateText("Рубит деревья в лесу"));

        mp.events.callRemote('Lumberjack_StartAnimCutTree');

        if (lumberjackJob.interval !== undefined) {
            clearInterval(lumberjackJob.interval);
            lumberjackJob.interval = undefined;
        }

        if (lumberjackJob.restartTimeout !== undefined) {
            clearTimeout(lumberjackJob.restartTimeout);
            lumberjackJob.restartTimeout = undefined;
        }

        lumberjackJob.process_count = 0;

        lumberjackJob.interval = setInterval(() => {
            if (global.localplayer.vehicle || global.cuffed || global.isDeath === true || global.isDemorgan) {
                mp.events.call('lumberjackJob_stopProcess');
                return;
            }

            lumberjackJob.process_count += 1000;

            if (lumberjackJob.treeHP - 25 >= 0) {
                lumberjackJob.treeHP -= 25;
            }

            if (lumberjackJob.process_count >= 4000) {
                global.startedMining = false;
                lumberjackJob.process_count = 0;
                lumberjackJob.treeHP = 0;
                
                if (lumberjackJob.interval !== undefined) {
                    clearInterval(lumberjackJob.interval);
                    lumberjackJob.interval = undefined;
                }

                //global.localplayer.freezePosition(false);
                //global.menuOpened = false; // чтобы запретить открывать инвентарь и биндер

                mp.events.callRemote('Lumberjack_CutTree', lumberjackJob.treeIndex);

                lumberjackJob.restartTimeout = setTimeout(() => {
                    mp.events.call('lumberjackJob_startProcess');
                }, 15500);
            }
        }, 3300);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/lumberjack", "lumberjackJob_startProcess", e.toString());
    }
});

gm.events.add("lumberjackJob_stopProcess", () => {
    try
    {
        global.startedMining = false;
        lumberjackJob.process_count = 0;
        
        if (lumberjackJob.interval !== undefined) {
            clearInterval(lumberjackJob.interval);
            lumberjackJob.interval = undefined;
        }

        if (lumberjackJob.restartTimeout !== undefined) {
            clearTimeout(lumberjackJob.restartTimeout);
            lumberjackJob.restartTimeout = undefined;
        }

        global.localplayer.freezePosition(false);
        global.menuOpened = false; // чтобы запретить открывать инвентарь и биндер
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/lumberjack", "lumberjackJob_startProcess", e.toString());
    }
});

mp.keys.bind(0x45, true, () => {
    if (lumberjackJob.treeIndex == -1 || global.localplayer.vehicle || global.cuffed || global.isDeath === true || global.isDemorgan) {
        return;
    }

    if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 1000) {
        if (lumberjackJob.treeStage == 0 && !global.startedMining) mp.events.callRemote("Lumberjack_HitTree", lumberjackJob.treeIndex);
        else if (global.startedMining) {
            mp.events.call("lumberjackJob_stopProcess");
            mp.events.callRemote("Lumberjack_StopCutTree", lumberjackJob.treeIndex);
        }
        else if (lumberjackJob.treeStage == 1) mp.events.callRemote("Lumberjack_TakeTimber", lumberjackJob.treeIndex);
    }
    
    global.anti_flood = new Date().getTime();
});

gm.events.add("render", () => {
    try {
        if (lumberjackJob.treeIndex !== -1 && lumberjackJob.treePosition !== undefined && lumberjackJob.treeStage != 2) {
            mp.game.controls.disableControlAction(0, global.Inputs.ATTACK, true);
            mp.game.controls.disableControlAction(0, global.Inputs.VEH_ATTACK, true);
            mp.game.controls.disableControlAction(0, global.Inputs.VEH_ATTACK2, true);
            mp.game.controls.disableControlAction(0, global.Inputs.VEH_PASSENGER_ATTACK, true);
            mp.game.controls.disableControlAction(0, global.Inputs.MELEE_ATTACK_LIGHT, true);
            mp.game.controls.disableControlAction(0, global.Inputs.MELEE_ATTACK_HEAVY, true);
            mp.game.controls.disableControlAction(0, global.Inputs.MELEE_ATTACK_ALTERNATE, true);
            mp.game.controls.disableControlAction(0, global.Inputs.ATTACK2, true);
            mp.game.controls.disableControlAction(0, global.Inputs.MELEE_ATTACK1, true);
            mp.game.controls.disableControlAction(0, global.Inputs.MELEE_ATTACK2, true);

            if (lumberjackJob.treeStage == 0) {
                drawTreeBar(1, lumberjackJob.treePosition.x, lumberjackJob.treePosition.y, lumberjackJob.treePosition.z, 0.075, 0.015, lumberjackJob.treeHP);
            } else {
                drawTreeBar(2, lumberjackJob.treePosition.x, lumberjackJob.treePosition.y, lumberjackJob.treePosition.z, 0.075, 0.015, lumberjackJob.treeHP);
            }
        }
    }
    catch (e)
    {
        if (new Date().getTime() - global.trycatchtime["player/lumberjack"] < 5000) return;
        global.trycatchtime["player/lumberjack"] = new Date().getTime();
        mp.events.callRemote("client_trycatch", "player/lumberjack", "render", e.toString());
    }
});

function drawTreeBar(state, x, y, z, width, height, health) {
    let pos = mp.game.graphics.world3dToScreen2d(new mp.Vector3(x, y, z));
    if (pos == undefined || pos.x == undefined || pos.y == undefined) return;

    if (state == 1) {
        mp.game.graphics.drawRect(pos.x, pos.y, width, height, 0, 0, 0, 200);

        let delta_pos = ((health * width) / 100) / 2;
        if (health == 100) mp.game.graphics.drawRect(pos.x, pos.y, ((health * width) / 100), height, 0, 200, 0, 200);
        else if (health >= 75) mp.game.graphics.drawRect(pos.x - 0.0375 + delta_pos, pos.y, ((health * width) / 100), height, 0, 200, 0, 200);
        else if (health < 75 && health >= 45) mp.game.graphics.drawRect(pos.x - 0.0375 + delta_pos, pos.y, ((health * width) / 100), height, 255, 255, 0, 200);
        else if (health < 45) mp.game.graphics.drawRect(pos.x - 0.0375 + delta_pos, pos.y, ((health * width) / 100), height, 215, 55, 55, 200);
    }

    mp.game.graphics.drawText(state == 1 ? translateText('{0}HP\nНажмите один раз E чтобы рубить', health) : "E", [pos.x, pos.y - 0.01], {
        font: 0,
        color: [255, 255, 255, 185],
        scale: [0.25, 0.25],
        outline: true
    });
}