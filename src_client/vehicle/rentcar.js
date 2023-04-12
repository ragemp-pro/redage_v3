gm.events.add('client.rentcar.open', (json) => {
    if (global.menuCheck()) return;
    global.menuOpen();

    json = JSON.parse(json);
    let cars= [];
    json.forEach((item) => {
        cars.push({
            Id: item[0],
            Model: item[1],
            Price: item[2],
            IsJob: item[3],
        })
    });

    cars = JSON.stringify(cars);
    mp.gui.emmit(`window.router.setView("PlayerRentCar", '${cars}');`);
    gm.discord(translateText("Хочет арендовать транспорт"));
})

gm.events.add('client.rentcar.exit', () => {
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
});

let lastRentCar = 0;

gm.events.add('client.rentcar.buy', (carId, colorId, hour) => {
    if (new Date().getTime() - lastRentCar < 1000) {
        mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
        return;
    }
    lastRentCar = new Date().getTime();
    mp.events.callRemote('server.rentcar.buy', carId, colorId, hour);
    mp.events.call('client.rentcar.exit');
});

let selectRentData = {
    marker: undefined,
    blip: undefined,
    colshape: undefined,
    vehicle: undefined,
    vehicleId: undefined,
    z: 1.2
}

gm.events.add('client.rentcar.point', (vehicleId) => {
    ClearRentCarData ();
    selectRentData.vehicleId = vehicleId;
});


gm.events.add('client.rentcar.func', (func) => {
    mp.events.callRemote('server.rentcar.func', func);
});

gm.events.add("vehicleStreamIn", (entity) => {
    if (!selectRentData.vehicle && entity && entity.remoteId === selectRentData.vehicleId) {
        selectRentData.vehicle = entity;

        selectRentData.zDefault = getVehicleHeight (entity) + 1;

        mp.events.call("createBlip", "rentcar", translateText("Аренда"), 225, entity.position, 1.25, 4);
        //mp.events.call("client.showWithPicture", translateText("Арендодатель"), "Mors Mutual Insurance", translateText("Спасибо за использование наших услуг. Транспортное средство можно найти на парковке. Хорошего дня!"), "CHAR_MP_MORS_MUTUAL");
        mp.game.audio.playSoundFrontend(-1, "Boss_Message_Orange", "GTAO_Boss_Goons_FM_Soundset", true);

        if (!global.localplayer["NewUser"])
            mp.events.call('createWaypoint', entity.position.x, entity.position.y);
        else
            mp.events.call('setIntoVehicle', entity, -1);
    }
});

const ClearRentCarData = () => {   
    if (selectRentData.marker !== undefined) {
        selectRentData.marker.destroy();
        selectRentData.marker = undefined;
    }
    
    if (selectRentData.blip !== undefined) {
        selectRentData.blip.setRoute(false);
        selectRentData.blip.destroy();
        selectRentData.blip = undefined;
    }

    if (selectRentData.colshape !== undefined) {
        selectRentData.colshape.destroy();
        selectRentData.colshape = undefined;
    }

    selectRentData.zDefault = 1;
    selectRentData.z = 0.2;

    selectRentData.vehicle = undefined;
    selectRentData.vehicleId = undefined;

    mp.events.call("deleteBlip", "rentcar");

}

gm.events.add("render", () => {
    if (!global.loggedin) return;
    if (selectRentData.vehicleId && selectRentData.vehicle && mp.vehicles.exists(selectRentData.vehicle)) {
        const position = global.localplayer.position;
        const dist = mp.game.system.vdist(position.x, position.y, position.z, selectRentData.vehicle.position.x, selectRentData.vehicle.position.y, selectRentData.vehicle.position.z);
        if (dist < 75.0) {
            if (selectRentData.marker !== undefined) {
                selectRentData.marker.destroy();
                selectRentData.marker = undefined;
            }

            selectRentData.z += 0.0015;
            if (selectRentData.z > 0.3)
                selectRentData.z = 0.2;

            selectRentData.marker = mp.markers.new(2, new mp.Vector3(selectRentData.vehicle.position.x, selectRentData.vehicle.position.y, selectRentData.vehicle.position.z + selectRentData.zDefault +  selectRentData.z), 0.3, {
                visible: true,
                color: [255, 255, 255, 185],
                rotation: new mp.Vector3(180, 0, 0),
                dimension: global.localplayer.dimension
            });
        }
    } else if (selectRentData.vehicleId && selectRentData.vehicle && !mp.vehicles.exists(selectRentData.vehicle)) {
        ClearRentCarData ();
    }
});


gm.events.add("playerEnterVehicle", (vehicle, seat) => {
    if (selectRentData.vehicle && mp.vehicles.exists(selectRentData.vehicle) && selectRentData.vehicle.handle === vehicle.handle) {
        ClearRentCarData ();
    }
});

gm.events.add("client.showWithPicture", (title, sender, message, notifPic, icon = 0, flashing = false, textColor = -1, bgColor = -1, flashColor = [255, 255, 255, 200]) => {
    if (textColor > -1) {
        mp.game.invoke("0x39BBF623FC803EAC", textColor);
    }

    if (bgColor > -1) {
        mp.game.invoke("0x92F0DA1E27DB96DC", bgColor);
    }

    if (flashing) {
        mp.game.ui.setNotificationFlashColor(flashColor[0], flashColor[1], flashColor[2], flashColor[3]);
    }

    mp.game.ui.setNotificationTextEntry("CELL_EMAIL_BCON");

    for (let i = 0, msgLen = message.length; i < msgLen; i += 50) {
        mp.game.ui.addTextComponentSubstringPlayerName(message.substr(i, Math.min(50, message.length - i)));
    }
    
    mp.game.ui.setNotificationMessage(notifPic, notifPic, flashing, icon, title, sender);
    mp.game.ui.drawNotification(false, true);
});