const
    clientName = "client.phone.mechjob.",
    rpcName = "rpc.phone.mechjob.",
    serverName = "server.phone.mechjob.";


let selectedOrders = {};
let orders = []
global.isInitMechList = false;

gm.events.add(clientName + "init", (json) => {
    orders = [];

    const playerPos = global.localplayer.position;

    json = JSON.parse(json);
    json.forEach((item) => {

        let newItem = {};

        newItem.id = item[0];
        newItem.name = item[1];
        newItem.pos = new mp.Vector3(item[2], item[3], item[4]);
        newItem.dist = Math.round(global.vdist2(newItem.pos, playerPos, true));
        newItem.aStreet = global.getStreetName(newItem.pos.x, newItem.pos.y, newItem.pos.z);
        newItem.aArea = global.getAreaName(newItem.pos.x, newItem.pos.y, newItem.pos.z);
        newItem.area = global.getStreetName(newItem.pos.x, newItem.pos.y, newItem.pos.z) + " - " + global.getAreaName(newItem.pos.x, newItem.pos.y, newItem.pos.z);

        orders.push(newItem);
    });

    global.isInitMechList = true;
    mp.gui.emmit(`window.listernEvent ('phone.mech.getMenu');`);
})




gm.events.add(clientName + "jobEnd", () => {
    orders = [];

    global.isInitMechList = false;
    mp.gui.emmit(`window.listernEvent ('phone.mech.getMenu');`);
})

gm.events.add(clientName + "add", (id, name, posX, posY, posZ) => {
    if (!global.isInitMechList)
        return;

    const playerPos = mp.players.local.position;

    let newItem = {};

    newItem.id = id;
    newItem.name = name;
    newItem.pos = new mp.Vector3(posX, posY, posZ);
    newItem.dist = Math.round(global.vdist2(newItem.pos, playerPos, true));
    newItem.aStreet = global.getStreetName(newItem.pos.x, newItem.pos.y, newItem.pos.z);
    newItem.aArea = global.getAreaName(newItem.pos.x, newItem.pos.y, newItem.pos.z);
    newItem.area = global.getStreetName(newItem.pos.x, newItem.pos.y, newItem.pos.z) + " - " + global.getAreaName(newItem.pos.x, newItem.pos.y, newItem.pos.z);

    orders.push(newItem);

    mp.gui.emmit(`window.listernEvent ('phone.mechjob.update');`);

    mp.gui.chat.push(translateText("!{#00a86b}[ДИСПЕТЧЕР]: !{#ffffff}Игрок {0} вызвал автомеханика !{#ffcc00}({1}м)!{#ffffff}. Откройте телефон чтобы принять вызов", name, newItem.dist));
    mp.events.call('phone.notify', 333, translateText("Появился новый заказ! :)"), 4);
});

gm.events.add(clientName + "dell", (id) => {
    if (!global.isInitMechList)
        return;

    const index = orders.findIndex(o => o.id === id);

    if (orders [index])
        orders.splice(index, 1);

    mp.gui.emmit(`window.listernEvent ('phone.mechjob.update');`);
});

gm.events.add(clientName + "take", (id) => {
    if (!global.isInitMechList)
        return;

    const index = orders.findIndex(o => o.id === id);

    if (orders [index])
        mp.events.callRemote(serverName + "take", id);
});

gm.events.add(clientName + "cancel", () => {
    mp.events.callRemote("server.phone.mech.cancel");
});


gm.events.add(clientName + "successCancel", () => {
    selectedOrders = {};
    global.isMechOrder = false;
    mp.gui.emmit(`window.listernEvent ('phone.mechjob.load');`);
});


gm.events.add(clientName + "load", () => {
    mp.events.callRemote(serverName + "load");
});

global.isMechOrder = false;

gm.events.add(clientName + "initSelect", (_selectedOrders, isTake) => {
    _selectedOrders = JSON.parse(_selectedOrders);

    selectedOrders = {};

    const playerPos = mp.players.local.position;

    if (_selectedOrders && _selectedOrders.length) {
        selectedOrders.name = _selectedOrders[0];
        global.isMechOrder = true;

        selectedOrders.pos = new mp.Vector3(_selectedOrders[1], _selectedOrders[2], _selectedOrders[3]);
        if (isTake)
            mp.events.call('createWaypoint', selectedOrders.pos.x, selectedOrders.pos.y);
        selectedOrders.dist = Math.round(global.vdist2(selectedOrders.pos, playerPos, true));

        selectedOrders.aStreet = global.getStreetName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z);
        selectedOrders.aArea = global.getAreaName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z);
        selectedOrders.area = global.getStreetName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z) + " - " + global.getAreaName(selectedOrders.pos.x, selectedOrders.pos.y, selectedOrders.pos.z);
    }

    mp.gui.emmit(`window.listernEvent ('phone.mechjob.load');`);
});


rpc.register(rpcName + "getList", () => {
    return JSON.stringify(orders);
});

rpc.register(rpcName + "getSelect", () => {
    return JSON.stringify(selectedOrders);
});