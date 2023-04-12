const
    clientName = "client.frac.main.",
    rpcName = "rpc.frac.main.",
    serverName = "server.frac.main.";

gm.events.add(clientName + "mainLoad", () => {
    logs = [];
    mp.events.callRemote(serverName + "mainLoad");
});

let leaderData = {};
let membersOnlineStats = {};
let stats = {};
let info = {};
let board = {};

gm.events.add(clientName + "mainInit", (_leaderData, _membersOnlineStats, _stats, _info, _board, _ranksName, _defaultAccess, _departmentsTag) => {
    table.setRanksName(_ranksName);
    table.setDefaultAccess(_defaultAccess);
    table.setDepartmentsTag(_departmentsTag);
    //
    leaderData = JSON.parse(_leaderData);
    membersOnlineStats = table.getOnline(_membersOnlineStats);
    stats = table.getStats(_stats);
    info = table.getInfo(_info);
    board = JSON.parse(_board);
    global.tableInFocus = false;

    mp.gui.emmit(`window.listernEvent ('table.online', '${JSON.stringify(membersOnlineStats)}');`);
    mp.gui.emmit(`window.listernEvent ('table.tableInfo', '${JSON.stringify(info)}');`);
    mp.gui.emmit(`window.listernEvent ('table.mainInit');`);
});

gm.events.add(clientName + "inputFocus", (toggled) => {
    global.tableInFocus = toggled;
    global.menuOpen(!inFocus);
});

rpc.register(rpcName + "getLeader", () => {
    return JSON.stringify(table.getMember(leaderData));
});


rpc.register(rpcName + "getSettings", () => {
    return JSON.stringify(info);
});

rpc.register(rpcName + "getStats", () => {
    return JSON.stringify(stats);
});

rpc.register(rpcName + "getStock", () => {
    return JSON.stringify({
        isAccessStock: info.isAccessStock,
        isStock: info.isStock,
        isGunAccessStock: info.isGunAccessStock,
        isGunStock: info.isGunStock,
    });
});

rpc.register(rpcName + "isOrgUpgrate", () => {
    return info.orgUpgrate;
});

//Table

rpc.register(rpcName + "getBoard", () => {
    return JSON.stringify(table.getBoard(board));
});

gm.events.add(clientName + "getBoard", (index) => {
    mp.events.callRemote(serverName + "getBoard", index);
});

gm.events.add(clientName + "setBoard", (_board) => {
    board = JSON.parse(_board);

    mp.gui.emmit(`window.listernEvent ('table.board');`);
});

gm.events.add(clientName + "addBoard", (title, text) => {
    if (!global.antiFlood("table_addBoard", 1000))
        return;

    mp.events.callRemote(serverName + "addBoard", title, text);
});

gm.events.add(clientName + "updateBoard", (id, title, text) => {
    if (!global.antiFlood("table_updateBoard", 1000))
        return;

    mp.events.callRemote(serverName + "updateBoard", id, title, text);
});

gm.events.add(clientName + "deleteBoard", (id) => {
    if (!global.antiFlood("table_deleteBoard", 1000))
        return;

    mp.events.callRemote(serverName + "deleteBoard", id);
});


//Upgrate

gm.events.add(clientName + "getUpgrate", () => {
    mp.events.callRemote(serverName + "getUpgrate");
});

gm.events.add(clientName + "setUpgrate", (_upgrate) => {
    mp.gui.emmit(`window.listernEvent ('table.upgrate', '${_upgrate}');`);
});

gm.events.add(clientName + "buyUpgrate", (type) => {
    if (!global.antiFlood("table.buyUpgrate", 1000))
        return;

    mp.events.callRemote(serverName + "buyUpgrate", type);
});

//updateStock

gm.events.add(clientName + "updateStock", () => {
    if (!global.antiFlood("table.updateStock", 1000))
        return;

    mp.events.callRemote(serverName + "updateStock");
});


gm.events.add(clientName + "updateGunStock", () => {
    if (!global.antiFlood("table.updateGunStock", 1000))
        return;

    mp.events.callRemote(serverName + "updateGunStock");
});

gm.events.add(clientName + "isStock", (value) => {
    info.isStock = value;
    mp.gui.emmit(`window.listernEvent ('table.isStock');`);
});

gm.events.add(clientName + "isGunStock", (value) => {
    info.isGunStock = value;
    mp.gui.emmit(`window.listernEvent ('table.isStock');`);
});


// Logs

const logsMax = 20;
let logs = [];

gm.events.add(clientName + "logs", (_logs) => {

    _logs = table.getLogs(_logs);

    logs = [
        ...logs,
        ..._logs
    ]

    mp.gui.emmit(`window.listernEvent ('table.logs');`);
});

gm.events.add(clientName + "getLog", (uuid, isStock, text, pageId) => {
    if (!global.antiFlood("table.getLog", 250))
        return;

    if (pageId == -1)
        pageId = Math.floor (logs.length / logsMax);

    if (!pageId)
        logs = [];

    mp.events.callRemote(serverName + "getLog", uuid, isStock, text, pageId);
});

rpc.register(rpcName + "isLog", () => {
    return info.isLog;
});

rpc.register(rpcName + "getLogs", () => {
    return JSON.stringify(logs);
});

//Members

gm.events.add(clientName + "membersLoad", () => {
    mp.events.callRemote(serverName + "membersLoad");
});

gm.events.add(clientName + "members", (_member, _members) => {
    _member = table.getMember(_member);
    _members = table.getMembers(_members);

    mp.gui.emmit(`window.listernEvent ('table.members', \`${JSON.stringify(_member)}\`, \`${JSON.stringify(_members)}\`, \`${JSON.stringify(membersOnlineStats)}\`);`);
});

gm.events.add(clientName + "updateMember", (_member) => {
    _member = table.getMember(_member);

    mp.gui.emmit(`window.listernEvent ('table.updateMember', \`${JSON.stringify(_member)}\`);`);
});



gm.events.add(clientName + "invitePlayer", (name) => {
    if (!global.antiFlood("table.invitePlayer", 250))
        return;
    mp.events.callRemote(serverName + "invitePlayer", name);
});

gm.events.add(clientName + "deletePlayer", (uuid) => {
    if (!global.antiFlood("table.deletePlayer", 250))
        return;
    mp.events.callRemote(serverName + "deletePlayer", uuid);
});

gm.events.add(clientName + "addPlayerScore", (uuid, value) => {
    if (!global.antiFlood("table.addPlayerScore", 250))
        return;
    mp.events.callRemote(serverName + "addPlayerScore", uuid, value);
});

gm.events.add(clientName + "reprimand", (uuid, name, text) => {
    if (!global.antiFlood("table.addPlayerScore", 250))
        return;

    mp.events.callRemote(serverName + "reprimand", uuid, name, text);
});

gm.events.add(clientName + "updateMemberRankAccess", (uuid, json) => {
    mp.events.callRemote(serverName + "updateMemberRankAccess", uuid, json);
});

gm.events.add(clientName + "updatePlayerRank", (uuid, rank) => {
    if (!global.antiFlood("table.updatePlayerRank", 250))
        return;
    mp.events.callRemote(serverName + "updatePlayerRank", uuid, rank);
});

gm.events.add(clientName + "defaultRanks", () => {
    if (!global.antiFlood("table.defaultRanks", 250))
        return;
    mp.events.callRemote(serverName + "defaultRanks");
});


//updateMemberRankAccess








//
gm.events.add(clientName + "rankAccessLoad", (id) => {
    mp.events.callRemote(serverName + "rankAccessLoad", id);
});

gm.events.add(clientName + "rankAccessInit", (_access, _lock) => {
    const access = table.getAccess(_access, _lock);
    mp.gui.emmit(`window.listernEvent ('table.rankAccess', \`${JSON.stringify(access)}\`);`);
});

rpc.register(rpcName + "getRanks", () => {
    return JSON.stringify(table.ranksName);
});

gm.events.add(clientName + "setRanks", (_ranksName) => {
    table.setRanksName(_ranksName);
    mp.gui.emmit(`window.listernEvent ('table.ranks');`);
});


gm.events.add(clientName + "updateRanksId", (json) => {
    mp.events.callRemote(serverName + "updateRanksId",  json);
});

gm.events.add(clientName + "updateRankAccess", (id, json) => {
    mp.events.callRemote(serverName + "updateRankAccess", id, json);
});

gm.events.add(clientName + "createRank", (name, score) => {
    mp.events.callRemote(serverName + "createRank", name, score);
});
gm.events.add(clientName + "removeRank", (id) => {
    mp.events.callRemote(serverName + "removeRank", id);
});

gm.events.add(clientName + "updateRankName", (id, name) => {
    mp.events.callRemote(serverName + "updateRankName", id, name);
});

gm.events.add(clientName + "updateRankScore", (id, score) => {
    mp.events.callRemote(serverName + "updateRankScore", id, score);
});

//depart

gm.events.add(clientName + "departmentsLoad", () => {
    mp.events.callRemote(serverName + "departmentsLoad");
});

gm.events.add(clientName + "departmentsInit", (_departments) => {
    _departments = table.getDepartments(_departments);

    mp.gui.emmit(`window.listernEvent ('table.departments', \`${JSON.stringify(_departments)}\`);`);
});

gm.events.add(clientName + "createDepartment", (name, tag) => {
    if (!global.antiFlood("table.createDepartment", 1000))
        return;

    mp.events.callRemote(serverName + "createDepartment", name, tag);
});

gm.events.add(clientName + "removeDepartment", (id) => {
    if (!global.antiFlood("table.removeDepartment", 1000))
        return;

    mp.events.callRemote(serverName + "removeDepartment", id);
});


rpc.register(rpcName + "getDepartments", () => {
    return JSON.stringify(Object.values(table.departmentsTag));
});

gm.events.add(clientName + "departmentLoad", (id) => {
    mp.events.callRemote(serverName + "departmentLoad", id);
});

const departmentRankId = {
    chief: 3,
    zam1: 2,
    zam2: 1,
}

gm.events.add(clientName + "departmentInit", (_department, _members, _membersOnlineStats) => {
    _members = table.getMembers(_members);
    _department = JSON.parse(_department);

    let
        chief = "Нет",
        zam1 = "Нет",
        zam2 = "Нет";

    _members.forEach((member) => {
        if (member.departmentRank === departmentRankId.chief)
            chief = member.name;
        if (member.departmentRank === departmentRankId.zam1)
            zam1 = member.name;
        if (member.departmentRank === departmentRankId.zam2)
            zam2 = member.name;
    })

    let department = {
        id: _department[0],
        name: _department[1],
        tag: _department[2],
        date: _department[3],
        playerCount: _department[4],
        ranks: table.getDepartmentRanks(_department[5]),
        isSettings: _department[6],
        myRank: _department[7],
        chief: chief,
        zam1: zam1,
        zam2: zam2,
    };

    _membersOnlineStats = table.getOnline(_membersOnlineStats);

    mp.gui.emmit(`window.listernEvent ('table.department', \`${JSON.stringify(department)}\`, \`${JSON.stringify(_members)}\`, \`${JSON.stringify(_membersOnlineStats)}\`);`);
});

gm.events.add(clientName + "departmentRankLoad", (departmentId) => {
    mp.events.callRemote(serverName + "departmentRankLoad", departmentId);
});

gm.events.add(clientName + "departmentRankInit", (_ranks) => {
    _ranks = table.getDepartmentRanks(_ranks);

    mp.gui.emmit(`window.listernEvent ('table.departmentRankInit', \`${JSON.stringify(_ranks)}\`);`);
});

gm.events.add(clientName + "departmentRankAccessLoad", (departmentId, rank) => {
    mp.events.callRemote(serverName + "departmentRankAccessLoad", departmentId, rank);
});



gm.events.add(clientName + "setLeadersDepartment", (departmentId, json) => {
    if (!global.antiFlood("table.setLeadersDepartment", 1000))
        return;

    mp.events.callRemote(serverName + "setLeadersDepartment", departmentId, json);
});


gm.events.add(clientName + "invitePlayerDepartment", (departmentId, uuid) => {
    if (!global.antiFlood("table.invitePlayerDepartment", 1000))
        return;

    mp.events.callRemote(serverName + "invitePlayerDepartment", departmentId, uuid);
});

gm.events.add(clientName + "deletePlayerDepartment", (departmentId, uuid) => {
    if (!global.antiFlood("table.deletePlayerDepartment", 1000))
        return;

    mp.events.callRemote(serverName + "deletePlayerDepartment", departmentId, uuid);
});

gm.events.add(clientName + "updateDepartment", (departmentId, name, tag) => {
    mp.events.callRemote(serverName + "updateDepartment", departmentId, name, tag);
});

gm.events.add(clientName + "updateDepartmentRankName", (departmentId, id, name) => {
    mp.events.callRemote(serverName + "updateDepartmentRankName", departmentId, id, name);
});

gm.events.add(clientName + "updateDepartmentRankAccess", (jsonId, json) => {//+

    if (jsonId && jsonId.split('|') && jsonId.split('|').length > 0) {
        jsonId = jsonId.split('|');

        mp.events.callRemote(serverName + "updateDepartmentRankAccess", Number (jsonId[0]), Number (jsonId[1]), json);
    }
});

//vehicle


gm.events.add(clientName + "vehiclesLoad", () => {
    mp.events.callRemote(serverName + "vehiclesLoad");
});

gm.events.add(clientName + "vehicles", (isVehicleUpdateRank, _vehicles) => {

    _vehicles = table.getVehicles(_vehicles);

    mp.gui.emmit(`window.listernEvent ('table.vehicles', ${isVehicleUpdateRank}, \`${JSON.stringify(_vehicles)}\`);`);
});

gm.events.add(clientName + "evacuation", (number) => {
    if (!global.antiFlood("table.evacuation", 500))
        return;

    mp.events.callRemote(serverName + "evacuation", number);
});

gm.events.add(clientName + "gps", (number) => {
    if (!global.antiFlood("table.gps", 500))
        return;

    mp.events.callRemote(serverName + "gps", number);
});

gm.events.add(clientName + "updateVehicleRank", (number, rank) => {
    if (!global.antiFlood("table.updateVehicleRank", 1000))
        return;

    mp.events.callRemote(serverName + "updateVehicleRank", number, rank);
});


//Clothes


gm.events.add(clientName + "clothesLoad", () => {
    mp.events.callRemote(serverName + "clothesLoad");
});

gm.events.add(clientName + "clothes", (json) => {
    json = JSON.parse(json);

    let clothes = [];

    json.forEach((item) => {
        clothes.push({
            gender: item[0],
            name: item[1],
            rank: item[2],
            rankName: table.getRankName(item[2])
        })
    })

    mp.gui.emmit(`window.listernEvent ('table.clothes', \`${JSON.stringify(clothes)}\`);`);
});



gm.events.add(clientName + "clothesUpdate", (name, newName, rank, gender) => {
    global.binderFunctions.GameMenuClose ();
    mp.events.callRemote(serverName + "clothesUpdate", name, newName, rank, gender);
});


gm.events.add(clientName + "avatar", (url) => {
    mp.events.callRemote(serverName + "avatar", url);
});


gm.events.add(clientName + "leave", () => {
    global.binderFunctions.GameMenuClose ();
    mp.events.callRemote(serverName + "leave");
});

//Tasks

gm.events.add(clientName + "tasksMyLoad", () => {
    mp.events.callRemote(serverName + "tasksMyLoad");
});

gm.events.add(clientName + "tasksLoad", () => {
    mp.events.callRemote(serverName + "tasksLoad");
});

gm.events.add(clientName + "tasks", (_tasks) => {

    _tasks = table.getTasks(_tasks);

    mp.gui.emmit(`window.listernEvent ('table.tasks', \`${JSON.stringify(_tasks)}\`);`);
});

//missions

gm.events.add(clientName + "missionsLoad", () => {
    mp.events.callRemote(serverName + "missionsLoad");
});

gm.events.add(clientName + "missions", (_missions) => {
    _missions = table.getMissions(_missions);

    mp.gui.emmit(`window.listernEvent ('table.missions', \`${JSON.stringify(_missions)}\`);`);
});

gm.events.add(clientName + "missionUse", (index) => {
    if (!global.antiFlood("table.missionUse", 500))
        return;

    mp.events.callRemote(serverName + "missionUse", index);
});
