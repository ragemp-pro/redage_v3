const
    clientName = "client.businessmanage.",
    rpcName = "rpc.businessmanage.",
    serverName = "server.businessmanage.";

let businessStats = {}
let businessStock = []
let businessOrders = []

let getTopProd = []
let getTopPlayers = []

gm.events.add(clientName + "open", async (jsonStats, jsonStock, jsonOrders) => {
    businessStats = JSON.parse(jsonStats);
    businessStock = JSON.parse(jsonStock);
    businessOrders = JSON.parse(jsonOrders);
    await global.awaitMenuCheck ();
    global.menuOpen();
    mp.gui.emmit(
        `window.router.setView("BusinessManage")`
    );
    gm.discord(translateText("Управляет своим бизнесом"));
});

rpc.register(rpcName + "getStats", () => {
	return JSON.stringify (businessStats);
});

rpc.register(rpcName + "getOrders", () => {
	return JSON.stringify (businessOrders);
});

rpc.register(rpcName + "getStocks", () => {
	return JSON.stringify (businessStock);
});

gm.events.add(clientName + "getHistory", () => {
    mp.events.callRemote(serverName + 'gethistory');
})

gm.events.add(clientName + "setHistory", (jsonProd, jsonUser) => {
    json = JSON.parse(json);

    json.forEach((data) => {
        getTopProd.push({
            name: data[4],
            price: data[5]
        })
        getTopPlayers.push({
            uuid: (data[3]).toString(36),
            name: data[4],
            price: data[5]
        })
    })

    mp.gui.emmit(`window.listernEvent.updateBusinessHistory ();`);

})

rpc.register(rpcName + "getTopProd", () => {
    return JSON.stringify (getTopProd);
});




























gm.events.add('client.businessmanage.cancelOrder', (id) => {
	try
	{
        if(id) mp.events.callRemote('server.businessmanage.cancelOrder', id);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.cancelOrder", e.toString());
    }
})

gm.events.add('client.businessmanage.makeOrder', (id) => {
	try
	{
        if(id) mp.events.callRemote('server.businessmanage.makeOrder', id);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.makeOrder", e.toString());
    }
})

gm.events.add('client.businessmanage.changePrice', (id, price) => {
	try
	{
        if(id) mp.events.callRemote('server.businessmanage.changePrice', id);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.changePrice", e.toString());
    }
})

gm.events.add('client.businessmanage.fillStocks', (id) => {
	try
	{
        if(id) mp.events.callRemote('server.businessmanage.fillStocks', id);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.fillStocks", e.toString());
    }
})

gm.events.add('client.businessmanage.sellBiz', () => {
	try
	{
        if(id) mp.events.callRemote('server.businessmanage.sellBiz');
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/businessmanage", "client.businessmanage.sellBiz", e.toString());
    }
})