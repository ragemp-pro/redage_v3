
require("./job");


const
    clientName = "client.phone.mech.",
    rpcName = "rpc.phone.mech.",
    serverName = "server.phone.mech.";


let orderMech = {};

const onCancelOrder = () => {

    const shape = orderMech.shape;

    orderMech = {};

    if (typeof shape !== "undefined")
        shape.destroy();

    mp.events.call('deleteBlip', "mech");
    mp.events.call('deleteBlip', "mechCircle");
}
const onSelectOrder = () => orderMech = {
    isOrder: true
};

rpc.register(rpcName + "getOrder", () => {
    return JSON.stringify(orderMech)
});

const onUpdateOrder = () => {

    mp.gui.emmit(`window.listernEvent ('phone.mech.load');`);

    if (orderMech.pos) {

        mp.events.call("createBlip", "mech", translateText("Такси"), 636, orderMech.pos, 1.25, 5);

        if (typeof orderMech.shape === "undefined") {
            const playerPos = mp.players.local.position;

            mp.events.call("createBlip", "mechCircle", "", 9, playerPos, false, 5, 155, undefined, 15);

            orderMech.shape = mp.colshapes.newSphere(playerPos.x, playerPos.y, playerPos.z, 15);
        }
    }
}

gm.events.add('playerExitColshape', (shape) => {
    if (orderMech && orderMech.shape && typeof orderMech.shape !== "undefined" && shape && shape === orderMech.shape) {
        mp.events.call(clientName + "cancel");
    }
});

gm.events.add(clientName + "order", () => {
    if (!orderMech.isOrder) {
        mp.events.callRemote(serverName + "order");
    }
});

gm.events.add(clientName + "successOrder", () => {
    onSelectOrder ();

    onUpdateOrder ();
});

gm.events.add(clientName + "cancel", () => {
    if (orderMech.isOrder) {
        mp.events.callRemote(serverName + "cancel");
    }
});

gm.events.add(clientName + "successCancel", () => {
    onCancelOrder ();

    onUpdateOrder ();
});

gm.events.add(clientName + "updateOrder", (driver, number, posX, posY, posZ) => {
    if (orderMech.isOrder) {
        orderMech.driver = driver;
        orderMech.number = number;
        orderMech.pos = new mp.Vector3(posX, posY, posZ);

        onUpdateOrder ();
    }
});

gm.events.add(clientName + "updatePosOrder", (posX, posY, posZ) => {
    if (orderMech.isOrder) {
        orderMech.pos = new mp.Vector3(posX, posY, posZ);

        onUpdateOrder ();
    }
});

rpc.register(rpcName + "getMenu", () => {

    if (global.isMechOrder)
        return "Driver"
    else if (global.isInitMechList)
        return "List"

    return "Client"
});



///global.isInitMechList
















/*

let isOpenMenumech = false;

gm.events.add(clientName + "open", () => {
    if (Natives.IS_WAYPOINT_ACTIVE ())
        Natives._DELETE_WAYPOINT ();

    isOpenMenumech = true;
});

gm.events.add(clientName + "close", () => {
    isOpenMenumech = false;
});























gm.events.add("render", () => {
    if (!global.loggedin || true)
        return;

    if (!global.isPhoneOpen)
        return;

    if (!isOpenMenumech)
        return;

    if (Natives.IS_WAYPOINT_ACTIVE ()) {
        let returnPosition = global.GetWaypointCoords ();
        if(returnPosition !== null)
            mp.gui.emmit(`window.listernEvent ('phonemechPos', '${JSON.stringify(returnPosition)}');`);
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