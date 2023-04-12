const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";

let newsList = []
let newsPriceOneSymbol = 0

gm.events.add(clientName + "loadNews", () => {
    mp.events.callRemote(serverName + "loadNews");
});

gm.events.add(clientName + "initNews", (priceOneSymbol, json) => {
    newsPriceOneSymbol = priceOneSymbol;

    newsList = [];

    json = JSON.parse(json);

    json.forEach((item) => {
        newsList.push({
            name: item [1],
            number: item [2],
            text: item [3],
            link: item [4],
            time: item [7],
            type: item [5],
            isPremium: item [6]
        })
    });


    mp.gui.emmit(`window.listernEvent ('phoneNewsLoad');`);
});


rpc.register(rpcName + "getNews", () => {
    return JSON.stringify(newsList.reverse());
});

rpc.register(rpcName + "newsPriceOneSymbol", () => {
    return newsPriceOneSymbol;
});

gm.events.add(clientName + "successNews", () => {
    mp.gui.emmit(`window.listernEvent ('adSuccess');`);
})

gm.events.add(clientName + "addNews", (text, link, type, isPremium) => {
    mp.events.callRemote(serverName + "addNews", text, link, type, isPremium);
});
