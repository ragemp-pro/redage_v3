const
    clientName = "client.phone.forbes.",
    rpcName = "rpc.phone.forbes.",
    serverName = "server.phone.forbes.";


gm.events.add(clientName + "load", () => {
    mp.events.callRemote(serverName + "load");
});

const forbesType = {
    House: 0,
    Biz: 1,
    Vehicle: 2
}

let forbesData = [];
gm.events.add(clientName + "init", (json) => {
    forbesData = [];

    json = JSON.parse(json);
    json.forEach(item => {
        const newItem = {};

        newItem.Name = item [0];
        newItem.Money = item [1];
        newItem.SumMoney = item [2];
        newItem.Lvl = item [3];
        newItem.IsShowForbes = item [4];

        let list = {
            houses: [],
            biz: [],
            vehicles: [],
        }

        item [5].forEach((itemList) => {

            let nameType = "houses";

            if (itemList [2] === forbesType.Biz)
                nameType = "biz";
            else if (itemList [2] === forbesType.Vehicle)
                nameType = "vehicles";


            list [nameType].push({
                Name: itemList [0],
                Money: itemList [1],
            })
        });

        newItem.houses = list.houses;
        newItem.biz = list.biz;
        newItem.vehicles = list.vehicles;

        forbesData.push (newItem);
    });


    mp.gui.emmit(`window.listernEvent ('phone.forbes.load');`);
});

rpc.register(rpcName + "getList", () => {
    let list = []

    forbesData.forEach((item) => {
        list.push({
            Name: item.Name,
            Money: item.SumMoney,
            IsShowForbes: item.IsShowForbes
        })
    });

    return JSON.stringify (list);
});

rpc.register(rpcName + "getId", (id) => {
    return JSON.stringify (forbesData[id]);
});

