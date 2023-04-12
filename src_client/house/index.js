gm.events.add('client.parking.open', async () => {
	try
	{
        await global.awaitMenuCheck ();
        //

        global.menuOpen();
        mp.gui.emmit(
            `window.router.setView("HouseMenu")`
        );

	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "house/index", "client.house.open", e.toString());
	}
});

gm.events.add('client.parking.close', () => {//+
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
})

gm.events.add('client.garage.parking', (number, place) => {//+
    mp.events.callRemote('server.garage.parking', number, place);
});

//gm.events.add('client.parking.confirm', (sqlId, place) => {//+
//    mp.gui.emmit(`window.events.callEvent("cef.parking.confirm", ${sqlId}, ${place})`);
//});

gm.events.add('client.parking.updateCar', (json) => {//+
    mp.gui.emmit(`window.events.callEvent("cef.parking.carsData", '${json}')`);
    mp.gui.emmit(`window.events.callEvent("cef.parking.confirm")`);
});

gm.events.add('client.vehicle.action', (number, action) => {//+
    if (action === "sell")
        mp.events.call('client.house.close');

    mp.events.callRemote('server.vehicle.action', number, action);
});

gm.events.add('client.garage.update', () => {//+
    mp.events.call('client.house.close');
    mp.events.callRemote('server.garage.update');
});

//

gm.events.add('client.houseinfo.open', (data) => {//+
    if (global.menuCheck()) return;

    global.menuOpen();
    gm.discord(translateText("Присматривает дом"));
    
    mp.gui.emmit(
        `window.router.setView("HouseBuy", '${data}')`
    );
});

gm.events.add('client.houseinfo.close', () => {//+
    global.menuClose();
    mp.gui.emmit(`window.router.setHud()`);
});

gm.events.add('client.houseinfo.action', (action) => {//+
    mp.events.call('client.houseinfo.close');
    mp.events.callRemote('server.houseinfo.action', action);
});

//////////////////////

gm.events.add('client.furniture.open', (json) => {//+
    if (global.menuCheck()) return;
    global.menuOpen();
    gm.discord(translateText("Присматривает мебель"));
    mp.gui.emmit(
        `window.router.setView("HouseFurniture", '${json}')`
    );
});

gm.events.add('client.furniture.buy', (name, type) => {//+
    mp.events.callRemote('server.furniture.buy', name, type);
});

gm.events.add('client.furniture.close', () => {//+
    global.menuClose();
    mp.gui.emmit(`window.router.setHud()`);
});

//


gm.events.add('client.vehicleair.open', (json) => {//+
    if (global.menuCheck()) return;
    global.menuOpen();
    gm.discord(translateText("В магазине вертолётов"));
    mp.gui.emmit(
        `window.router.setView("VehicleAir", '${json}')`
    );
});

gm.events.add('client.vehicleair.exit', () => {//+
    mp.gui.emmit(`window.router.setHud();`);
    global.menuClose();
})