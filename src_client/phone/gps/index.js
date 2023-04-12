const
    clientName = "client.phone.gps.",
    rpcName = "rpc.phone.gps.",
    serverName = "server.phone.gps.";

let routes = [];

let routesToName = {};

gm.events.add(clientName + "init", (json) => {
    json = JSON.parse (json);
    routes = [];

    json.forEach((category) => {
        let gpsCategory = {};

        gpsCategory.name = category[0];
        gpsCategory.icon = category[1];

        let content = [];
        category[2].forEach((item) => {
            let gpsItem = {};

            gpsItem.name = item[0];

            if (typeof item[1] === "object" && typeof item[1][0] === "object") {
                let gpsCategoryPosList = [];
                let gpsCategorySubNameList = [];

                let isSub = false;

                item[1].forEach((pos) => {
                    gpsCategoryPosList.push(new mp.Vector3(pos[0], pos[1], 0))

                    if (pos[2]) {
                        isSub = true;
                        gpsCategorySubNameList.push(pos[2])
                    }
                });
                gpsItem.posList = gpsCategoryPosList;

                if (isSub) {
                    gpsItem.isSub = true;
                    gpsItem.subList = gpsCategorySubNameList;
                }

            } else {
                gpsItem.pos = new mp.Vector3(item[1][0], item[1][1], 0);
            }

            routesToName [gpsItem.name] = gpsItem;
            content.push(gpsItem);
        })

        gpsCategory.content = content;
        routes.push(gpsCategory);
    })
});

gm.events.add("gps.name", (name) => {
    setRouterToName (name);
});

const setRouterToName = (name) => {
    if (routesToName [name]) {
        let pos = routesToName [name].pos;

        const playerPos = mp.players.local.position;

        if (routesToName [name].posList) {
            pos = getNearest (routesToName [name].posList, playerPos);
        }
        mp.events.call('createWaypoint', pos.x, pos.y);
    }
}


rpc.register(rpcName + "getRoutes", () => {

    let returnList = [];

    routes.forEach((route) => {
        returnList.push({
            name: route.name,
            icon: route.icon,
        })
    })


    return JSON.stringify (returnList);
});

const getNearest = (posList, playerPos) => {
    let returnPos = null;

    posList.forEach((pos) => {
        if (returnPos === null)
            returnPos = pos;
        else if (global.vdist2(pos, playerPos, true) < global.vdist2(returnPos, playerPos, true))
            returnPos = pos;
    });

    return returnPos;
}

let selectedSubCategory = -1;

rpc.register(rpcName + "getList", (index) => {
    selectedSubCategory = -1;

    const route = routes [index];

    let returnList = {};

    returnList.name = route.name;
    returnList.icon = route.icon;
    returnList.content = [];

    const playerPos = mp.players.local.position;

    route.content.forEach((route, index) => {

        let item = {};
        item.name = route.name;
        item.pos = route.pos;
        item.isSub = route.isSub;

        if (route.posList) {
            item.pos = getNearest (route.posList, playerPos);
        }

        item.dist = Math.round(global.vdist2(item.pos, playerPos, true));

        returnList.content.push(item)
    })

    return JSON.stringify (returnList);
});

rpc.register(rpcName + "getSubList", (item) => {
    const route = routes [item.index];

    let returnList = {};

    returnList.name = route.name;
    returnList.icon = route.icon;
    returnList.content = [];

    const playerPos = mp.players.local.position;

    const content = route.content[item.id];

    selectedSubCategory = item.id;

    content.posList.forEach((pos, index) => {
        let item = {};
        item.name = content.name + " #" + content.subList [index];
        item.pos = pos;

        item.dist = Math.round(global.vdist2(item.pos, playerPos, true));

        returnList.content.push(item)
    })

    return JSON.stringify (returnList);
});


rpc.register(rpcName + "getItem", (item) => {
    const route = routes [item.index];
    const content = selectedSubCategory !== -1 ? route.content[selectedSubCategory] : route.content[item.id];

    let returnList = {};

    returnList.icon = route.icon;
    returnList.name = content.name;
    returnList.pos = content.pos;

    const playerPos = mp.players.local.position;

    if (selectedSubCategory !== -1) {
        returnList.pos = content.posList [item.id];
        returnList.name = content.name + " #" + content.subList [item.id];
    } else if (content.posList) {
        returnList.pos = getNearest(content.posList, playerPos);
    }

    returnList.dist = Math.round(global.vdist2(returnList.pos, playerPos, true));

    return JSON.stringify (returnList);
});

Natives.SET_WAYPOINT_OFF = () => mp.game.invoke('0xA7E4E2D361C2627F');

gm.events.add("gps.setPoint", (json) => {
    json = JSON.parse(json);

    mp.events.call("gps.clearPoint");

    Natives.SET_WAYPOINT_OFF ();

    global.gps = mp.blips.new(162, new mp.Vector3(json.x, json.y, 0), {
        'name': json.name,
        'color': 5,
        'shortRange': false
    });
    global.gps.setRoute(true);
    global.gps.setRouteColour(5);

    mp.game.audio.setGpsActive(true);

    //mp["events"]["callRemote"]("gps.server.setPoint", JSON["stringify"](json))
})

gm.events.add("gps.clearPoint", () => {

    if (global.gps) {
        global.gps.setRoute(false);
        global.gps.destroy();
    }
    global.gps = null;
})

gm.events.add("gps.pointDefault", (name) => {

    mp.events.callRemote("gps.pointDefault", name);
})

