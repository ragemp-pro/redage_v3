
require("./job");


const
    clientName = "client.phone.taxi.",
    rpcName = "rpc.phone.taxi.",
    serverName = "server.phone.taxi.";


let orderTaxi = {};

const onCancelOrder = () => {
    const shape = orderTaxi.shape;

    orderTaxi = {};

    if (typeof shape !== "undefined")
        shape.destroy();


    mp.events.call('deleteBlip', "taxi");
    mp.events.call('deleteBlip', "taxiCircle");
}
const onSelectOrder = () => orderTaxi = {
    isOrder: true
};

rpc.register(rpcName + "getOrder", () => {
    return JSON.stringify(orderTaxi)
});

const onUpdateOrder = () => {

    mp.gui.emmit(`window.listernEvent ('phone.taxi.load');`);

    if (orderTaxi.pos) {

        mp.events.call("createBlip", "taxi", translateText("Такси"), 56, orderTaxi.pos, 1.25, 5);

        if (typeof orderTaxi.shape === "undefined") {
            const playerPos = mp.players.local.position;

            mp.events.call("createBlip", "taxiCircle", "", 9, playerPos, false, 5, 155, undefined, 15);

            orderTaxi.shape = mp.colshapes.newSphere(playerPos.x, playerPos.y, playerPos.z, 15);
        }
    }
}

gm.events.add('playerExitColshape', (shape) => {
    if (orderTaxi && typeof orderTaxi.shape !== "undefined" && shape && shape === orderTaxi.shape) {
        mp.events.call(clientName + "cancel");
    }
});

gm.events.add(clientName + "order", () => {
    if (!orderTaxi.isOrder) {
        mp.events.callRemote(serverName + "order");
    }
});

gm.events.add(clientName + "successOrder", () => {
    onSelectOrder ();

    onUpdateOrder ();
});

gm.events.add(clientName + "cancel", () => {
    if (orderTaxi.isOrder) {
        mp.events.callRemote(serverName + "cancel");
    }
});

gm.events.add(clientName + "successCancel", () => {
    onCancelOrder ();

    onUpdateOrder ();
});

gm.events.add(clientName + "updateOrder", (driver, number, posX, posY, posZ) => {
    if (orderTaxi.isOrder) {
        orderTaxi.driver = driver;
        orderTaxi.number = number;
        orderTaxi.pos = new mp.Vector3(posX, posY, posZ);

        onUpdateOrder ();
    }
});

gm.events.add(clientName + "updatePosOrder", (posX, posY, posZ) => {
    if (orderTaxi.isOrder) {
        orderTaxi.pos = new mp.Vector3(posX, posY, posZ);

        onUpdateOrder ();
    }
});

//Таксометр

let isOpenCounter = false;
gm.events.add(clientName + "openCounter", (name, isDriver) => {
    isOpenCounter = {
        name: name,
        isDriver: isDriver,
        price: 0,
        pos: global.localplayer.position
    };

    mp.gui.emmit(`window.listernEvent ('phone.taxi.getMenu');`);
    mp.gui.emmit(`window.hudStore.isTaxiCounter (true)`);
});

gm.events.add(clientName + "updateCounter", (price) => {
    isOpenCounter.price += price;
    isOpenCounter.pos = global.localplayer.position;

    mp.gui.emmit(`window.listernEvent ('phone.taxi.updateCounter');`);
    mp.gui.emmit(`window.listernEvent ('hud.taxi.updateCounter');`);

});


rpc.register(rpcName + "getCounter", () => {
    return JSON.stringify(isOpenCounter)
});

gm.events.add(clientName + "closeCounter", () => {
    isOpenCounter = false;
    mp.gui.emmit(`window.listernEvent ('phone.taxi.getMenu');`);
    mp.gui.emmit(`window.hudStore.isTaxiCounter (false)`);
});




rpc.register(rpcName + "getMenu", () => {
    if (isOpenCounter)
        return "Counter";
    else if (global.isTaxiOrder)
        return "Driver"
    else if (global.isInitTaxiList)
        return "List"

    return "Client"
});



///global.isInitTaxiList
















/*

let isOpenMenuTaxi = false;

gm.events.add(clientName + "open", () => {
    if (Natives.IS_WAYPOINT_ACTIVE ())
        Natives._DELETE_WAYPOINT ();

    isOpenMenuTaxi = true;
});

gm.events.add(clientName + "close", () => {
    isOpenMenuTaxi = false;
});























gm.events.add("render", () => {
    if (!global.loggedin || true)
        return;

    if (!global.isPhoneOpen)
        return;

    if (!isOpenMenuTaxi)
        return;

    if (Natives.IS_WAYPOINT_ACTIVE ()) {
        let returnPosition = global.GetWaypointCoords ();
        if(returnPosition !== null)
            mp.gui.emmit(`window.listernEvent ('phoneTaxiPos', '${JSON.stringify(returnPosition)}');`);
    }

});

rpc.register(rpcName + "getItem", () => {
    let returnList = {};

    const playerPos = mp.players.local.position;

    returnList.icon = "clubs";
    returnList.name = global.getStreetName(playerPos.x, playerPos.y, playerPos.z) + " | " + global.getAreaName(playerPos.x, playerPos.y, playerPos.z);
    returnList.pos = playerPos;

    returnList.dist = Math.round(global.vdist2(returnList.pos, playerPos, true));

    return JSON.stringify (returnList);
});*/