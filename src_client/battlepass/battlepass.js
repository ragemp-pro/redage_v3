
const
    clientName = "client.battlepass.",
    rpcName = "rpc.battlepass.",
    serverName = "server.battlepass.";

const battlepassData = {};

const maxCountAwards = 6;
battlepassData.awards = [];
battlepassData.awardsPremium = [];

battlepassData.lvl = 0;
battlepassData.exp = 0;
battlepassData.isPremium = false;

//Те которые надо выполнить

battlepassData.tasksDay = [];
battlepassData.tasksWeek = [];

//Награды которые забрал

battlepassData.tookReward = [];
battlepassData.tookRewardPremium = [];

//

battlepassData.missionsTask = [];
battlepassData.missionDataTasks = [];
battlepassData.missionDataSelect = 0;

gm.events.add(clientName + "open", () => {

    global.binderFunctions.GameMenuClose ();

    let isInit = battlepassData.awards.length > 0;

    mp.events.callRemote(serverName + "open", isInit);
});

let isBattlePassOpen = false;
gm.events.add(clientName + "show", async (lvl, exp, isPremium, tasksDay, tasksWeek, tookReward, tookRewardPremium, awards, awardsPremium, time, missionsTask, missionDataTasks, missionDataSelect) => {

    await global.awaitMenuCheck ();

    if (isBattlePassOpen)
        return;

    battlepassData.lvl = lvl;
    battlepassData.exp = exp;
    battlepassData.isPremium = isPremium;
    //
    battlepassData.tasksDay = JSON.parse (tasksDay);
    battlepassData.tasksWeek = JSON.parse (tasksWeek);
    //
    battlepassData.tookReward = JSON.parse (tookReward);
    battlepassData.tookRewardPremium = JSON.parse (tookRewardPremium);

    if (awards && typeof awards === "string")
        battlepassData.awards = JSON.parse (awards);

    if (awardsPremium && typeof awardsPremium === "string")
        battlepassData.awardsPremium = JSON.parse (awardsPremium);

    battlepassData.time = time;

    //

    if (missionsTask && typeof missionsTask === "string")
        battlepassData.missionsTask = JSON.parse (missionsTask);

    battlepassData.missionDataTasks = JSON.parse (missionDataTasks);
    battlepassData.missionDataSelect = missionDataSelect;

    //

    mp.gui.emmit(`window.router.setView("PlayerBattlePass");`);
    gm.discord(translateText("Изучает боевой пропуск"));
    isBattlePassOpen = true;
    global.menuOpen()
});

gm.events.add(clientName + "close", () => {
    if (!isBattlePassOpen)
        return;

    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
    
    isBattlePassOpen = false;
});

//

rpc.register(rpcName + "getAwardsCount", () => {
    return battlepassData.awards.length;
});

const getAwards = (page) => {
    const awards = Array.from(battlepassData.awards);
    const awardsPremium = Array.from(battlepassData.awardsPremium);

    let data = [];

    for (let i = 0; i < maxCountAwards; i++) {
        const index = (maxCountAwards * page) + i;
        data.push({
            index: index,
            usual: {
                taked: battlepassData.tookReward.includes(index),
                ...awards [index]
            },
            premium: {
                taked: battlepassData.tookRewardPremium.includes(index),
                ...awardsPremium [index]
            },
        })
    }

    return data;
}

rpc.register(rpcName + "getAwards", (page) => {
    return JSON.stringify (getAwards (page));
});

const isAllAwardsTaked = () => {
    const awards = Array.from(battlepassData.awards);
    const awardsPremium = Array.from(battlepassData.awardsPremium);

    let isAllTaked = false;

    for (let i = 0; i < awards.length; i++) {

        if (battlepassData.lvl > i) {
            if (awards [i].Type >= 0 && !battlepassData.tookReward.includes(i)) {
                isAllTaked = true;
                break;
            }
            if (battlepassData.isPremium && awardsPremium [i].Type >= 0 && !battlepassData.tookRewardPremium.includes(i)) {
                isAllTaked = true;
                break;
            }
        }
    }

    return isAllTaked;
}

rpc.register(rpcName + "isAllAwardsTaked", () => {
    return isAllAwardsTaked ();
});
//

rpc.register(rpcName + "getLvl", () => {
    return battlepassData.lvl;
});

rpc.register(rpcName + "getExp", () => {
    return battlepassData.exp;
});

//

rpc.register(rpcName + "getMinutesAddExp", () => {
    return battlepassData.time;
});

//
rpc.register(rpcName + "getPremium", () => {
    return battlepassData.isPremium;
});

//

rpc.register(rpcName + "getTasksDay", () => {
    return JSON.stringify (battlepassData.tasksDay);
});

rpc.register(rpcName + "getTasksWeek", () => {
    return JSON.stringify (battlepassData.tasksWeek);
});

//

gm.events.add(clientName + "takeAll", () => {//+
    mp.events.callRemote(serverName + "takeAll");
});

gm.events.add(clientName + "take", (index, isPrem) => {//+
    mp.events.callRemote(serverName + "take", index, isPrem);
});

gm.events.add(clientName + "takeSuccess", (tookReward, tookRewardPremium) => {//+
    battlepassData.tookReward = JSON.parse (tookReward);
    battlepassData.tookRewardPremium = JSON.parse (tookRewardPremium);

    //

    mp.gui.emmit(`window.listernEvent ('battlePassTakeSuccess');`);
    mp.gui.emmit(`window.listernEvent ('isAllAwardsTaked');`);
});

//

gm.events.add(clientName + "buyPremium", () => {//+
    mp.events.call("client.donatepack.open", 0);
    //mp.events.callRemote(serverName + "buyPremium");
});

gm.events.add(clientName + "buyPremiumSuccess", () => {//+
    battlepassData.isPremium = true;

    //

    mp.gui.emmit(`window.listernEvent ('battlePassBuyPremiumSuccess');`);
});

//

gm.events.add(clientName + "buyLvl", (index) => {
    mp.events.callRemote(serverName + "buyLvl", index);
});

gm.events.add(clientName + "updateLvlAndExp", (lvl, exp, isClear) => {
    battlepassData.lvl = lvl;
    battlepassData.exp = exp;
    //
    mp.gui.emmit(`window.listernEvent ('battlePassUpdateLvlAndExp');`);
    //
    mp.gui.emmit(`window.listernEvent ('isAllAwardsTaked');`);
    //
    if (isClear) {
        battlepassData.tasksDay = [];
        battlepassData.tasksWeek = [];
    }
});


//Миссии

const statusData = {
    closed: 0,
    selected: 1,
    done: 2,
    //active: 3
}

const maxCountMissions = 20;

const getMissions = (page) => {

    let missionDataTasks = {};

    battlepassData.missionDataTasks.forEach((data) => {
        missionDataTasks [data.Index] = data;
    });

    const missions = Array.from(battlepassData.missionsTask);

    let missionsTask = [];

    for (let i = 0; i < maxCountMissions; i++) {
        const index = (maxCountMissions * page) + i;
        const data = missions [index];
        if (typeof data === "object") {
            const missionData = missionDataTasks [data.id];
            let status = statusData.closed;

            if (battlepassData.missionDataSelect === data.id)
                status = statusData.selected;
            else if (typeof missionData === "object" && missionData.IsReward)
                status = statusData.done;
            //else if (typeof missionData === "object")
            //    status = statusData.active;

            missionsTask.push({
                ...data,
                isDone: typeof missionData === "object" ? missionData.IsDone : false,
                count: typeof missionData === "object" ? missionData.Count : 0,
                status: status
            })
        }
    }

    return missionsTask;
}


rpc.register(rpcName + "getMissions", (page) => {
    return JSON.stringify (getMissions (page));
});

gm.events.add(clientName + "setMissions", (id) => {
    mp.events.callRemote(serverName + "setMissions", id);
});

gm.events.add(clientName + "updateMissions", (id, missionDataTasks, missionsTask) => {
    battlepassData.missionDataSelect = id;
    battlepassData.missionDataTasks = JSON.parse (missionDataTasks);

    if (missionsTask && typeof missionsTask === "string")
        battlepassData.missionsTask = JSON.parse (missionsTask);

    //
    mp.gui.emmit(`window.listernEvent ('updateMissions');`);
});

const missionImage = "https://cloud.redage.net/cloud/inventoryItems/items/bp.png";

gm.events.add(clientName + "missionComplite", (title, desc) => {
    mp.gui.emmit(`window.missionComplite ('${title}', '${desc}', '${translateText("Молодец, ты справился!")}', '${missionImage}');`);
    mp.events.call("sounds.playInterface", "cloud/sound/missionComplite.ogg", 0.005);
});

gm.events.add(clientName + "skip", () => {
    mp.events.callRemote(serverName + "skip");
});
