// AUTO SHOP //
let autoColors = [translateText("Черный"), translateText("Белый"), translateText("Красный"), translateText("Оранжевый"), translateText("Желтый"), translateText("Зеленый"), translateText("Голубой"), "Синий", translateText("Фиолетовый")];
let autoModels = null;

let buyMetodName = "";

let colors = {};
colors[translateText("Черный")] = [0, 0, 0];
colors[translateText("Белый")] = [225, 225, 225];
colors[translateText("Красный")] = [230, 0, 0];
colors[translateText("Оранжевый")] = [255, 115, 0];
colors[translateText("Желтый")] = [240, 240, 0];
colors[translateText("Зеленый")] = [0, 230, 0];
colors[translateText("Голубой")] = [0, 205, 255];
colors["Синий"] = [0, 0, 230];
colors[translateText("Фиолетовый")] = [190, 60, 165];

let auto = {
    model: null,
    color: null,
    entity: null,
}

let selectSpawn = 0;

const spawnCar = [
	[-973.6858, -3005.5195, 14.035086, 60.0],
]

global.AirCarList = [
	mp.game.joaat("akula"),
	mp.game.joaat("annihilator"),
	mp.game.joaat("annihilator2"),
	mp.game.joaat("buzzard"),
	mp.game.joaat("buzzard2"),
	mp.game.joaat("cargobob"),
	mp.game.joaat("cargobob2"),
	mp.game.joaat("cargobob3"),
	mp.game.joaat("cargobob4"),
	mp.game.joaat("frogger"),
	mp.game.joaat("frogger"),
	mp.game.joaat("havok"),
	mp.game.joaat("hunter"),
	mp.game.joaat("maverick"),
	mp.game.joaat("savage"),
	mp.game.joaat("seasparrow"),
	mp.game.joaat("seasparrow2"),
	mp.game.joaat("seasparrow3"),
	mp.game.joaat("skylift"),
	mp.game.joaat("supervolito"),
	mp.game.joaat("supervolito2"),
	mp.game.joaat("swift"),
	mp.game.joaat("swift2"),
	mp.game.joaat("valkyrie"),
	mp.game.joaat("valkyrie2"),
	mp.game.joaat("volatus"),
]


const getSpawn = (hashModel) => {

	let isAir = global.AirCarList.includes(hashModel);
	/*AirCarList.forEach((model) => {
		if (hashModel === mp.game.joaat(model))
			isAir = true;
	})*/

	if (!isAir) {
		isAir = selectSpawn !== 0;
		selectSpawn = 0;
	} else {
		isAir = selectSpawn !== 1;
		selectSpawn = 1;
	}

	return isAir;
}

gm.events.add('auto', async (act, index) => {
	try
	{
		if (auto.entity == null || !mp.vehicles.exists(auto.entity)) return;
		switch (act) {
			case "model":
				auto.model = autoModels[index].modelName;
				await global.loadModel(auto.model).then(
					response => {
						if (response) {
							auto.entity.model = mp.game.joaat(auto.model);
							//const isUpdate = getSpawn (mp.game.joaat(auto.model));
							const posZ = mp.game.gameplay.getGroundZFor3dCoord(spawnCar [selectSpawn] [0], spawnCar [selectSpawn] [1], spawnCar [selectSpawn] [2], 0.0, false);
							auto.entity.setCoordsNoOffset(spawnCar [selectSpawn] [0], spawnCar [selectSpawn] [1], posZ + 0.45, false, false, false);
							auto.entity.setRotation(0, 0, spawnCar [selectSpawn] [3], 2, true);
							auto.entity.setOnGroundProperly();
							auto.entity.setForwardSpeed(0);
							//if (isUpdate) {
							//	global.cameraManager.stopCamera ();
							//	global.createCamera ("autoshop", auto.entity);
							//}
						}
					}
				);
				break;
			case "color":
				auto.color = autoColors[index];
				auto.entity.setCustomPrimaryColour(colors[autoColors[index]][0], colors[autoColors[index]][1], colors[autoColors[index]][2]);
				auto.entity.setCustomSecondaryColour(colors[autoColors[index]][0], colors[autoColors[index]][1], colors[autoColors[index]][2]);
				break;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "auto", e.toString());
	}
});

gm.events.add('buyAuto', (id) => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		mp.events.callRemote(buyMetodName, auto.model, auto.color, id);
		destroyAutoShop ();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "buyAuto", e.toString());
	}
});

gm.events.add('closeAuto', () => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();    
		mp.events.callRemote('carroomCancel');
		destroyAutoShop ();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "closeAuto", e.toString());
	}
});

gm.events.add('testDrive', (type) => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime(); 
		mp.events.callRemote('testDrive', auto.model, auto.color, type);
		destroyAutoShop ();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "testDrive", e.toString());
	}
});

gm.events.add('startTestDrive', async (vehicle) => {
	try
	{
		if (!vehicle) return;
		await global.IsLoadEntity (vehicle);//TODO
		mp.events.call('notify', 1, 9, translateText("Вы получили транспортное средство на тест-драйв. Тест-драйв будет окончен через 2 минуты или при выходе из транспортного средства."), 10000);
		if (vehicle && vehicle.handle !== 0)
			global.localplayer.setIntoVehicle(vehicle.handle, -1);
		//global.FadeScreen (false, 1500);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "startTestDrive", e.toString());
	}
});

gm.events.add('setIntoVehicle', async (vehicle, seat, callRemote = false) => {
	try
	{
		await global.IsLoadEntity (vehicle);
		if (vehicle && vehicle.handle !== 0) {
			global.localplayer.setIntoVehicle(vehicle.handle, seat);
			if (callRemote && typeof callRemote === "string")
				mp.events.callRemote(callRemote);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "setIntoVehicle", e.toString());
	}
});

const destroyAutoShop = () => {
	try
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
		global.cameraManager.stopCamera ();
		spectateInfo = false;
		if (auto.entity == null) return;
		auto.entity.destroy();
		auto.entity = null;
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "destroyAutoShop", e.toString());
	}
};

let spectateInfo = false;

gm.events.add('openAuto', async (models, prices, gosPrices, bagageSlots, _buyMetodName, isDonate = false) => {
	try
	{
		if (global.menuCheck()) return;
		buyMetodName = _buyMetodName;
		mp.gui.emmit(`window.router.setView("BusinessAutoShop");`);
		gm.discord(translateText("В автосалоне"));
		models = JSON.parse(models);
		prices = JSON.parse(prices);
		gosPrices = JSON.parse(gosPrices);
		bagageSlots = JSON.parse(bagageSlots);
		autoModels = [];
		models.forEach((value, index) => {
			autoModels = [
				...autoModels, {
					index: index,
					modelName: value,
					price: prices[index],
					gosPrice: gosPrices[index],
					speed: (mp.game.vehicle.getVehicleModelMaxSpeed(mp.game.joaat(value)) * 3.6).toFixed(1),
					boost: (mp.game.vehicle.getVehicleModelAcceleration(mp.game.joaat(value))).toFixed(2),
					seat: Math.round(mp.game.vehicle.getVehicleModelMaxNumberOfPassengers(mp.game.joaat(value))),
					invslots: bagageSlots[index]
				}
			];
		});
		//const isUpdate = getSpawn (mp.game.joaat(autoModels[0].modelName));
		global.localplayer.position = new mp.Vector3(spawnCar [selectSpawn] [0], spawnCar [selectSpawn] [1], spawnCar [selectSpawn] [2]);
		await global.wait(50);
		mp.gui.emmit(`window.authShop.data('${JSON.stringify(autoModels)}', ${isDonate});`);
		auto.entity = mp.vehicles.new(mp.game.joaat(autoModels[0].modelName), new mp.Vector3(spawnCar [selectSpawn] [0], spawnCar [selectSpawn] [1], spawnCar [selectSpawn] [2]), {
			heading: spawnCar [selectSpawn] [3],
			numberPlate: 'AUTOROOM',
			alpha: 255,
			color: [[0, 0, 0], [0, 0, 0]],
			locked: false,
			engine: false,
			dimension: localplayer.dimension
		});
		auto.entity.setInvincible(true);
		await global.IsLoadEntity (auto.entity);//TODO
		auto.entity.setInvincible(true);

		if (auto.entity && auto.entity.handle !== 0)
			global.localplayer.setIntoVehicle(auto.entity.handle, -1);

		auto.entity.freezePosition(true);

		auto.entity.setRotation(0, 0, spawnCar [selectSpawn] [3], 2, true);
		auto.entity.setDirtLevel(0.0);
		
		auto.color = autoColors[0];
		auto.model = autoModels[0].modelName;
		global.createCamera ("autoshop", auto.entity);
		global.menuOpen();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/autoshop", "openAuto", e.toString());
	}
});