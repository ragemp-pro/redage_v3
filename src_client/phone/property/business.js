const
    clientName = "client.phone.business.",
    rpcName = "rpc.phone.business.",
    serverName = "server.phone.business.";

gm.events.add(clientName + "load", (bizId) => {
    mp.events.callRemote(serverName + "load", bizId);

    clearJsonStats ();
});

let stats = {}
let stocks = []
let orders = []

gm.events.add(clientName + "init", (statsJson, stocksJson, ordersJson) => {
    statsJson = JSON.parse(statsJson);

    stats = {
        type: statsJson[0],
        tax: statsJson[1],
        cash: statsJson[2],
        pribil: statsJson[3],
        zatratq: statsJson[4],
        whCount: statsJson[5],
        whMaxCount: statsJson[6],
        whPriceMaxCount: statsJson[7],
        sellPrice: statsJson[8],
    };

    stocksJson = JSON.parse(stocksJson);

    stocks = [];
    stocksJson.forEach((item) => {
        stocks.push({
            name: item[0],
            count: item[1],
            maxCount: item[2],
            price: item[3],
            otherPrice: item[4],
            defaultPrice: item[5],
            minPrice: item[6],
            maxPrice: item[7],
            itemId: item[8],
            productType: item[9]
        });
    });

    ordersJson = JSON.parse(ordersJson);

    orders = [];
    ordersJson.forEach((item) => {

        const stockIndex = stocks.findIndex(s => s.name === item[1]);
        const stock = stocks [stockIndex];
        if (stock) {
            const price = stock.otherPrice > 0 ? stock.otherPrice : stock.defaultPrice;

            orders.push({
                uid: item[0],
                name: item[1],
                count: item[2],
                //
                price: price * item[2],
                itemId: stock.itemId,
                productType: stock.productType

            });

            stocks [stockIndex].isOrder = true;
            stocks [stockIndex].uidOrder = item[0];
        }
    });

    mp.gui.emmit(`window.listernEvent ('phoneBusinessInit');`);
    mp.gui.emmit(`window.listernEvent ('phoneBusinessUpdate');`);
});

rpc.register(rpcName + "getProducts", () => {
    return JSON.stringify(stocks);
});

rpc.register(rpcName + "getProduct", (productName) => {
    return JSON.stringify(stocks.find(s => s.name === productName));
});

rpc.register(rpcName + "getStats", () => {
    return JSON.stringify(stats);
});

rpc.register(rpcName + "getType", () => {
    return stats.type;
});

gm.events.add(clientName + "extraCharge", (name, value) => {
    mp.events.callRemote(serverName + "extraCharge", name, value);
});

gm.events.add(clientName + "cExtraCharge", (name, value) => {
    const stockIndex = stocks.findIndex(s => s.name === name);

    if (stocks [stockIndex]) {
        stocks [stockIndex].price = value;
        mp.gui.emmit(`window.listernEvent ('phoneBusinessUpdate');`);
    }
});

gm.events.add(clientName + "maxProducts", () => {
    mp.events.callRemote(serverName + "maxProducts");
});

//Order

rpc.register(rpcName + "getOrders", () => {
    return JSON.stringify(orders);
});

rpc.register(rpcName + "getOrder", (orderName) => {
    return JSON.stringify(orders.find(s => s.name === orderName));
});

gm.events.add(clientName + "addOrder", (name, value) => {
    mp.events.callRemote(serverName + "addOrder", name, value);
});

gm.events.add(clientName + "cAddOrder", (uid, name, count) => {
    const stockIndex = stocks.findIndex(s => s.name === name);
    const stock = stocks [stockIndex];
    if (stock) {
        const price = stock.otherPrice > 0 ? stock.otherPrice : stock.defaultPrice;

        orders.push({
            uid: uid,
            name: name,
            count: count,
            //
            price: price * count,
            itemId: stock.itemId,
            productType: stock.productType

        });

        stocks [stockIndex].isOrder = true;
        stocks [stockIndex].uidOrder = uid;

        stats.whPriceMaxCount -= price * (stock.maxCount - stock.count);

        mp.gui.emmit(`window.listernEvent ('phoneBusinessUpdate');`);
    }
});

gm.events.add(clientName + "cancelOrder", (uid) => {
    const orderIndex = orders.findIndex(s => s.uid === uid);
    if (orders [orderIndex])
        mp.events.callRemote(serverName + "cancelOrder", uid);
});

gm.events.add(clientName + "successCancel", (uid) => {
    const orderIndex = orders.findIndex(s => s.uid === uid);

    if (orders [orderIndex]) {
        const stockIndex = stocks.findIndex(s => s.name === orders [orderIndex].name);

        const stock = stocks [stockIndex];
        if (stock) {
            const price = stock.otherPrice > 0 ? stock.otherPrice : stock.defaultPrice;

            stats.whPriceMaxCount += price * (stock.maxCount - stock.count);

            delete stocks [stockIndex].isOrder;
            delete stocks [stockIndex].uidOrder;
        }

        orders.splice(orderIndex, 1);
        mp.gui.emmit(`window.listernEvent ('phoneBusinessUpdate');`);
    }
});

//Stats

let jsonStats = "",
    jsonProducts = "",
    jsonUsers = "";

const clearJsonStats = () => {
    jsonStats = "";
    jsonProducts = "";
    jsonUsers = "";
}

gm.events.add(clientName + "loadStats", () => {

    if (!jsonStats || jsonStats.length <= 1)
        mp.events.callRemote(serverName + "loadStats");
    else
        initStats ();
});

gm.events.add(clientName + "initStats", (_jsonStats, _jsonProducts, _jsonUsers) => {
    jsonStats = _jsonStats;

    _jsonProducts = JSON.parse(_jsonProducts);

    let data = [];
    _jsonProducts.forEach((time, timeId) => {
        time.forEach((item) => {
            const stock = stocks.find(s => s.name === item[0]);
            if (stock) {
                data.push({
                    name: stock.name,
                    price: item[1],
                    productType: stock.productType,
                    itemId: stock.itemId,
                    timeId: timeId
                });
            }
        })
    })

    jsonProducts = JSON.stringify(data);

    data = [];

    _jsonUsers = JSON.parse(_jsonUsers);

    _jsonUsers.forEach((time, timeId) => {
        time.forEach((item) => {
            data.push({
                uuid: item[0],
                price: item[1],
                timeId: timeId
            });
        })
    })

    jsonUsers = JSON.stringify(data);

    initStats ();
});

const initStats = () => {

    mp.gui.emmit(`window.listernEvent ('phoneBusinessStatsInit', '${jsonStats}', '${jsonProducts}', '${jsonUsers}');`);
}

gm.events.add(clientName + "sell", () => {
    mp.events.call('client.phone.close');
    mp.events.callRemote(serverName + "sell");
});