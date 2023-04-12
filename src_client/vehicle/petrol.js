// PETROL //

gm.events.add('openPetrol', () => {
	try
	{
		if (global.menuCheck()) return;
		global.menuOpen();
		mp.gui.emmit(`window.router.setView("PlayerGasStation");`);
		gm.discord(translateText("Заправляется"));
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('petrol', (data) => {
	try
	{
		mp.events.callRemote('petrol', data);
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "petrol", e.toString());
	}
});

gm.events.add('petrol.full', () => {
	try
	{
		mp.events.callRemote('petrol', 9999);
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "petrol.full", e.toString());
	}
});

gm.events.add('petrol.gov', () => {
	try
	{
		mp.events.callRemote('petrol', 99999);
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "petrol.gov", e.toString());
	}
});

gm.events.add('closePetrol', () => {
	try
	{
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "closePetrol", e.toString());
	}
});