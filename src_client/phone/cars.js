const
    clientName = "client.phone.cars.",
    rpcName = "rpc.phone.cars.",
    serverName = "server.phone.cars.";

let vehiclesList = [];
let inGarage = false;
let isOwner = false;

gm.events.add(clientName + "load", () => {
    mp.events.callRemote(serverName + "load");
});


const headerName = (isSell) => {
    if (typeof isSell !== "number" && !isSell) {
        return isOwner ? translateText("Подселённого") : translateText("Домовладельца");
    }

    return translateText("Личный");
}

let filterData = [];


gm.events.add(clientName + "init", (vehiclesJson, _inGarage, _isOwner) => {
    filterData = [];

    inGarage = _inGarage;
    isOwner = _isOwner;

    vehiclesJson = JSON.parse(vehiclesJson);

    vehiclesList = [];
    vehiclesJson.forEach((item) => {
        if (item[0] === "rent") {
            vehiclesList.push({
                isRent: true,
                model: item[1],
                number: item[2],
                date: item[3],
                rentPrice: item[4],
                isJob: item[5],
                header: translateText("Аренда"),
            })

            if (!filterData.includes(translateText("Аренда")))
                filterData.push(translateText("Аренда"))
        } else {
            vehiclesList.push({
                sqlId: item[0],
                model: item[1],
                number: item[2],
                isCarGarage: item[3],
                place: item[4],
                ticket: item[5],
                isAir: item[6],
                isCreate: item[7],
                color: item[8],
                sell: item[9],
                header: headerName (item[9])
            })
            if (!filterData.includes(headerName (item[9])))
                filterData.push(headerName (item[9]))
        }
    });

    mp.gui.emmit(`window.listernEvent ('phoneCarsLoad')`)
});

gm.events.add(clientName + "error", () => {
    mp.gui.emmit(`window.listernEvent ('phoneCarsLoad')`)
});

rpc.register(rpcName + "filterData", () => {
    return JSON.stringify(filterData);
});

rpc.register(rpcName + "getCarsList", () => {
    return JSON.stringify(vehiclesList);
});

rpc.register(rpcName + "inGarage", () => {
    return !!inGarage;
});

rpc.register(rpcName + "isOwner", () => {
    return !!isOwner;
});