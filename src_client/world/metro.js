const StationsData = [
    {
        station: "LSIA Terminal",
        id: 0,
        position: new mp.Vector3(-1102.43, -2730.98, -7.41),
        final: true
    },    
    {
        station: "LSIA Terminal",
        id: 1,
        position: new mp.Vector3(-1062.819, -2697.82, -7.41),
        final: false
    },    
    {
        station: "LSIA Parking",
        id: 2,
        position: new mp.Vector3(-878.79, -2317.05, -11.73),
        final: false
    },    
    {
        station: "LSIA Parking",
        id: 3,
        position: new mp.Vector3(-885.77, -2314.26, -7.41),
        final: false
    },
    {
        station: "Puerto Del Sol",
        id: 4,
        position: new mp.Vector3(-541.01, -1288.4, 26.9),
        final: false
    },
    {
        station: "Puerto Del Sol",
        id: 5,
        position: new mp.Vector3(-544.82, -1286.46, 26.9),
        final: false
    },
    {
        station: "Strawberry",
        id: 6,
        position: new mp.Vector3(279, -1207.1, 38.89),
        final: false
    },
    {
        station: "Strawberry",
        id: 7,
        position: new mp.Vector3(279.6, -1201.62, 38.89),
        final: false
    },
    {
        station: "Burton",
        id: 8,
        position: new mp.Vector3(-290.45, -331.06, 10.06),
        final: false
    },
    {
        station: "Burton",
        id: 9,
        position: new mp.Vector3(-298.05, -332.06, 10.06),
        final: false
    },
    {
        station: "Portola Drive",
        id: 10,
        position: new mp.Vector3(-816.3, -134.11, 19.95),
        final: false
    },
    {
        station: "Portola Drive",
        id: 11,
        position: new mp.Vector3(-812.45, -140.36, 19.95),
        final: false
    },
    {
        station: "Del Perro",
        id: 12,
        position: new mp.Vector3(-1355.43, -464.75, 15.04),
        final: false
    },
    {
        station: "Del Perro",
        id: 13,
        position: new mp.Vector3(-1349.18, -461.38, 15.04),
        final: false
    },
    {
        station: "Little Seoul",
        id: 14,
        position: new mp.Vector3(-502.46, -676.59, 11.8),
        final: false
    },
    {
        station: "Little Seoul",
        id: 15,
        position: new mp.Vector3(-502.76, -669.37, 11.8),
        final: false
    },
    {
        station: "Pillbox South",
        id: 16,
        position: new mp.Vector3(-217.11, -1038.77, 30.14),
        final: false
    },
    {
        station: "Pillbox South",
        id: 17,
        position: new mp.Vector3(-214.17, -1040.12, 30.13),
        final: false
    },
    {
        station: "Davis",
        id: 18,
        position: new mp.Vector3(110.09, -1718.53, 30.11),
        final: false
    },
    {
        station: "Davis",
        id: 19,
        position: new mp.Vector3(118.91, -1730.56, 30.11),
        final: true
    }
]

const centerShapeStation = mp.colshapes.newSphere(178.289, -1779.05, 29.08, 10, -1);

let metroData = {};
metroData.enterColshapes = [];
metroData.exitColshapes = [];
metroData.isUsingMetro = false;

let selectMetro,
    endStation = false;
let isMetroOpen = false;
const CreateShape = () => {

    try {
        StationsData.forEach((stationData) => {
            if (!stationData.final) {
                const enterShape = mp.colshapes.newSphere(stationData.position.x, stationData.position.y, stationData.position.z, 1, -1);
                enterShape.name = stationData.station;
                enterShape.cid = stationData.id;
                enterShape.pos = stationData.position;
                enterShape.isfinal = stationData.final;
                mp.markers.new(2, new mp.Vector3(stationData.position.x, stationData.position.y, stationData.position.z), 1, [52, 152, 219, 255]);
                metroData.enterColshapes.push(enterShape);
            }
            const exitShape = mp.colshapes.newSphere(stationData.position.x, stationData.position.y, stationData.position.z, 6.5, -1);
            exitShape.name = stationData.station;
            exitShape.cid = stationData.id;
            exitShape.pos = stationData.position;
            exitShape.isfinal = stationData.final;
            metroData.exitColshapes.push(exitShape);
        });
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "CreateShape", e.toString());
    }
}

CreateShape ();

gm.events.add("metroEnter", () => {
    try {
        if (metroData.enterColshapes.includes(metroData.metroColshape)) {
            if (isMetroOpen) {
                mp.events.call("client.metro.close");
            } else {
                if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || global.menuCheck() || (global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0)) return;
                isMetroOpen = true;
                mp.gui.emmit(`window.router.setView("PlayerMetro", '${metroData.metroColshape.name}');`);
                global.menuOpen();
            }
        } else if (endStation)
            mp.events.call("metro.clear");
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "metroEnter", e.toString());
    }
});

gm.events.add("playerEnterColshape", (shape) => {
    try {
        if (!global.loggedin) 
            return;
        if (metroData.enterColshapes.includes(shape)) {
            metroData.metroColshape = shape;
            mp.events.call('hud.oEnter', "buyMetro");
        }
        if (metroData.exitColshapes.includes(shape) && isStart) {
            selectMetro = shape;
            StopMetro(shape.isFinal);
        }
        if (centerShapeStation === shape && metroData.isUsingMetro)
            StopMetro(true);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "playerEnterColshape", e.toString());
    }
});
gm.events.add("playerExitColshape", (shape) => {
    try {
        if (!global.loggedin) 
            return;

        if (metroData.enterColshapes.includes(shape)) {
            metroData.metroColshape = null;
            mp.events.call("client.metro.close");
        }
        if (metroData.exitColshapes.includes(shape)) {
            selectMetro = null;
            endStation = false;
            mp.events.call('hud.cEnter');
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "playerExitColshape", e.toString());
    }
});
gm.events.add("client.metro.close", (isBrowers = false) => {
    mp.gui.emmit(`window.router.setHud();`)
    global.menuClose()
    isMetroOpen = false;
    if (!isBrowers)
        mp.events.call('hud.cEnter');
});

gm.events.add("client.metro.buyTicket", async (station, Increase) => {
    if (!metroData.metroColshape)
        return;
    else if (metroData.metroColshape.name == station) {
        mp.events.call('notify', 4, 9, translateText("Вы и так находитесь на данной станции!"), 3000);
        return;
    }
    mp.events.callRemote("metro.server.buyTicket", station, Increase);
});

let LastStation = ""; 

gm.events.add("metro.server.buyTicketSuccess", async (station) => {
    try {
        LastStation = station; 
        mp.events.call("client.metro.close");
        const serverStationData = StationsData.find((sData) => sData.station === station),
            clientStationData = StationsData.filter((sData) => sData.station === metroData.metroColshape.name && !sData.final);
        const stationData = ((serverStationData && serverStationData.id >= metroData.metroColshape.cid) || clientStationData.length == 1) ? clientStationData[0] : clientStationData[1];
        metroData.isUsingMetro = true;
        StartMetro(stationData);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "buyTicketSuccess", e.toString());
    }
});

global.isSartMetro = false;
let isStart = false;
let metroTrain = false;

const StartMetro = async (stationData) => {
    try {
        if (!stationData) {
            metroData.isUsingMetro = false;
            return;
        }
        
        global.isSartMetro = true;
        gm.discord(translateText("Едет на метро"));
        mp.events.call("setTraffic", 3);
        
        DestroyMetro ();
        Natives.DELETE_ALL_TRAINS ();

        // Подгрузка нужных моделей
        //await global.loadModel("s_m_m_lsmetro_01");
        await global.loadModel("metrotrain");

        // Создание нужного состава поезда метро
        //metroTrain = mp.game.vehicle.createMissionTrain(24, stationData.position.x, stationData.position.y, stationData.position.z, true);
        metroTrain = mp.game.vehicle.createMissionTrain(28, stationData.position.x, stationData.position.y, stationData.position.z, true);

        global.localplayer.setIntoVehicle(metroTrain, 0);
        if (metroTrain) {
            Natives.SET_TRAIN_SPEED (metroTrain, 14.5);
            Natives.SET_TRAIN_CRUISE_SPEED (metroTrain, 14.5);
        }

        await global.wait(1000);
        isStart = true;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "StartMetro", e.toString());
    }
}
const StopMetro = async (isEnd) => {
    try {
        if (metroTrain) {
            Natives.SET_TRAIN_SPEED (metroTrain, 0);
            Natives.SET_TRAIN_CRUISE_SPEED (metroTrain, 0);
        }
        await global.wait(1000);
        endStation = true;
        if (isEnd || LastStation === selectMetro.name)
            mp.events.call("metro.clear");
        else {
            mp.events.call('notify', 2, 9, translateText("Вы на станции {0}. Это еще не Ваша станция, но можете выйти здесь.", selectMetro.name), 9000);
            mp.events.call('hud.oEnter', "exitMetro");
            await global.wait(10000);
            if (metroTrain) {
                Natives.SET_TRAIN_SPEED (metroTrain, 14.5);
                Natives.SET_TRAIN_CRUISE_SPEED (metroTrain, 14.5);
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "StopMetro", e.toString());
    }
};

const DestroyMetro = () => {    
    if (metroTrain) {
        mp.game.vehicle.deleteMissionTrain(metroTrain)
        Natives.DELETE_ALL_TRAINS ();
    }
}

gm.events.add("metro.clear", async () => {
    try {
        metroData.isUsingMetro = false;
        global.isSartMetro = false;
        isStart = false;
        endStation = false;
        await global.wait(500);
        DestroyMetro ();
        if (selectMetro)
            global.localplayer.position = selectMetro.pos;
        mp.events.callRemote("metro.server.exit");
        selectMetro = null;
        LastStation = null;
        mp.events.call("setTraffic", 0);
        mp.events.call("cleartraffic");
        mp.events.call('hud.cEnter');
        //mp.events.callRemote("metro.server.stop");
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/other", "clear", e.toString());
    }
});

gm.events.add("render", () => {
    if (metroData.isUsingMetro)
        mp.game.controls.disableControlAction(0, 75, true);
});