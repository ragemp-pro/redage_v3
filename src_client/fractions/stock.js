// STOCK //!
gm.events.add('openStock', (data) => {
	try 
	{
		if (global.menuCheck()) return;
		mp.gui.emmit(`window.router.setView("FractionsStock", '${data}');`);
		gm.discord(translateText("На фракционном складе"));
		global.menuOpen();
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "fractions/stock", "openStock", e.toString());
	}
});

gm.events.add('closeStock', () => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "fractions/stock", "closeStock", e.toString());
	}
});

gm.events.add('stockTake', (index) => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);

		switch (index) {
			case 0: //cash
				mp.events.callRemote('setStock', "money");
				global.input.set(translateText("Взять деньги"), translateText("Введите кол-во денег"), 10, "take_stock");
				break;
			case 1: //healkit
				mp.events.callRemote('setStock', "medkits");
				global.input.set(translateText("Взять аптечки"), translateText("Введите кол-во аптечек"), 10, "take_stock");
				break;
			case 2: //weed
				mp.events.callRemote('setStock', "drugs");
				global.input.set(translateText("Взять наркотики"), translateText("Введите кол-во наркоты"), 10, "take_stock");
				break;
			case 3: //mats
				mp.events.callRemote('setStock', "mats");
				global.input.set(translateText("Взять маты"), translateText("Введите кол-во матов"), 10, "take_stock");
				break;
			case 4: //weapons stock
				mp.events.callRemote('openWeaponStock');
				break;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "fractions/stock", "stockTake", e.toString());
	}
});

gm.events.add('stockPut', (index) => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);

		switch (index) {
			case 3: //mats
				mp.events.callRemote('setStock', "mats");
				global.input.set(translateText("Положить маты"), translateText("Введите кол-во матов"), 10, "put_stock");
				break;
			case 0: //cash
				mp.events.callRemote('setStock', "money");
				global.input.set(translateText("Положить деньги"), translateText("Введите кол-во денег"), 10, "put_stock");
				break;
			case 1: //healkit
				mp.events.callRemote('setStock', "medkits");
				global.input.set(translateText("Положить аптечки"), translateText("Введите кол-во аптечек"), 10, "put_stock");
				break;
			case 2: //weed
				mp.events.callRemote('setStock', "drugs");
				global.input.set(translateText("Положить наркотики"), translateText("Введите кол-во наркоты"), 10, "put_stock");
				break;
			case 4: //weapons stock
				mp.events.callRemote('openWeaponStock');
				break;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "fractions/stock", "stockPut", e.toString());
	}
});

gm.events.add('stockExit', () => {
	try 
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud()`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "fractions/stock", "stockExit", e.toString());
	}
});