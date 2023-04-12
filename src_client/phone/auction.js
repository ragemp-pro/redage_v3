const
    clientName = "client.phone.auction.",
    rpcName = "rpc.phone.auction.",
    serverName = "server.phone.auction.";

gm.events.add(clientName + "load", () => {
    mp.events.callRemote(serverName + "load");
});

gm.events.add(clientName + "close", () => {
    mp.events.callRemote(serverName + "close");
});

const getData = (item) => {
    let betsList = [];

    item [11].forEach((betItem) => {
        betsList.push({
            name: betItem[0],
            bet: betItem[1],
        })
    });

    return {
        id: item [0],
        type: item [1],
        betCount: item [2],
        title: item [3],
        text: item [4],
        image: item [5],
        createPrice: item [6],
        lastPrice: item [7],
        createName: item [8],
        createUuid: item [9],
        time: item [10],
        betsList: betsList,
        lastBet: item [12]
    }
}

const unJson = (json) => {
    let returnJson = [];

    json = JSON.parse(json);

    json.forEach((item) => {
        returnJson.push(getData (item))
    });

    return returnJson;
}

let myAuctions = []
let auctions = []

gm.events.add(clientName + "init", (_myAuctions, _auctions) => {
    myAuctions = unJson (_myAuctions);
    auctions = unJson (_auctions);

    mp.gui.emmit(`window.listernEvent ('auction.load');`);
});

//

let selectCategory = 0;

gm.events.add(clientName + "selectCategory", (index) => {
    selectCategory = index;
});


gm.events.add(clientName + "getItem", (index) => {
    mp.events.callRemote(serverName + "getItem", index);
});

rpc.register(rpcName + "getCategory", () => {
    return selectCategory;
});

let items = []
gm.events.add(clientName + "setItem", (json) => {
    items = []

    json = JSON.parse(json);
    json.forEach((item) => {
        items.push({
            id: item [0],
            name: item [1],
        })
    })

    mp.gui.emmit(`window.listernEvent ('auction.load');`);
    //mp.gui.emmit(`window.listernEvent ('auction.view', 'Main');`);
});

rpc.register(rpcName + "getItem", () => {
    return JSON.stringify(items);
});

//

let selectId = -1;

gm.events.add(clientName + "setItemId", (id) => {
    selectId = id;
});

gm.events.add(clientName + "add", (text, cameraLink, price) => {
    mp.events.callRemote(serverName + "add", selectCategory, selectId, text, cameraLink, price);
    mp.gui.emmit(`window.listernEvent ('auction.view', 'Main');`);
    selectId = -1;
});

//

rpc.register(rpcName + "getMyList", () => {
    return JSON.stringify(myAuctions);
});

//

rpc.register(rpcName + "getList", () => {
    return JSON.stringify([
        ...myAuctions.filter(a => a.type === selectCategory),
        ...auctions.filter(a => a.type === selectCategory)
    ]);
});

rpc.register(rpcName + "getListItem", () => {
    let auction = myAuctions.find(a => a.id === selectId);
    if (auction) {
        selectCategory = auction.type;
        return JSON.stringify(auction);
    }

    auction = auctions.find(a => a.id === selectId);
    if (auction) {
        selectCategory = auction.type;
        return JSON.stringify(auction);
    }

    mp.gui.emmit(`window.listernEvent ('auction.view', 'Main');`);
    return false;
});

//


gm.events.add(clientName + "bet", (id, price) => {
    mp.events.callRemote(serverName + "bet", id, price);
});


//

gm.events.add(clientName + "addMyItem", (json) => {
    json = JSON.parse(json);
    json = getData (json);

    myAuctions.push(json)

    mp.gui.emmit(`window.listernEvent ('auction.updateMyList');`);

    if (json.type === selectCategory && json.id === selectId)
        mp.gui.emmit(`window.listernEvent ('auction.updateItem');`);
    else if (selectId == -1) {
        selectId = json.id;
        selectCategory = json.type;
        mp.gui.emmit(`window.listernEvent ('auction.view', 'ListItem');`);
    }
});


gm.events.add(clientName + "addItem", (json) => {
    json = JSON.parse(json);
    auctions.push(getData (json))

    mp.gui.emmit(`window.listernEvent ('auction.updateList');`);
});


gm.events.add(clientName + "updateItem", (json) => {
    json = JSON.parse(json);

    json = getData (json);

    let index = myAuctions.findIndex(a => a.id === json.id);
    if (typeof myAuctions [index] === "object")
        myAuctions [index] = json;

    index = auctions.findIndex(a => a.id === json.id);
    if (typeof auctions [index] === "object")
        auctions [index] = json;

    if (json.type === selectCategory && json.id === selectId)
        mp.gui.emmit(`window.listernEvent ('auction.updateItem');`);
});

gm.events.add(clientName + "delItem", (id, isMain) => {

    let item = {};

    let index = myAuctions.findIndex(a => a.id === id);
    if (typeof myAuctions [index] === "object") {
        item = myAuctions [index];
        myAuctions.splice(index, 1);
        mp.gui.emmit(`window.listernEvent ('auction.updateMyList');`);
    }

    index = auctions.findIndex(a => a.id === id);
    if (typeof auctions [index] === "object") {
        item = auctions [index];
        auctions.splice(index, 1);
        mp.gui.emmit(`window.listernEvent ('auction.updateList');`);
    }

    if (!isMain && item.type === selectCategory && item.id === selectId) {
        selectId = -1;
        mp.gui.emmit(`window.listernEvent ('auction.view', 'Main');`);
    }
});