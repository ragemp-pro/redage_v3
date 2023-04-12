const
    clientName = "client.advert.",
    rpcName = "rpc.advert.",
    serverName = "server.advert.";

let adsArray = []
let selectedID = null;
let isOpenAdvert = false;

gm.events.add(clientName + "open", async () => {
	await global.awaitMenuCheck ();
	global.menuOpen();
	mp.gui.emmit(
		`window.router.setView("FractionsWeazelNews")`
	);
	gm.discord(translateText("Изучает фракционный планшет"));
	isOpenAdvert = true;
});

gm.events.add(clientName + "close", async () => {
	if (!isOpenAdvert)
		return;

	mp.gui.emmit(`window.router.setHud();`);
	global.menuClose();

	isOpenAdvert = false;
});

let isList = false
gm.events.add(clientName + "isList", (toggled) => isList = toggled);

gm.events.add(clientName + "init", (json) => {
	adsArray = JSON.parse(json);
});

rpc.register(rpcName + "getAdsList", () => {
	if (selectedID != null) {
		mp.events.callRemote(serverName + "take", selectedID);
	}
	selectedID = null;
	return JSON.stringify (adsArray);
});

rpc.register(rpcName + "getAdsCount", () => {
	return adsArray.length;
});

rpc.register(rpcName + "getSelected", () => {
	if (selectedID != null)
		return selectedID;

	return false;
});

rpc.register(rpcName + "getAddByID", (addID) => {
	const advert = adsArray.find(el => el.ID == addID)
	if (advert) {
		if(advert.Editor && advert.Editor.length && advert.Editor !== global.localplayer.name)
			return false;

		mp.events.callRemote(serverName + "take", addID);
		selectedID = addID;
		return JSON.stringify (advert);
	}
	return false;
});

rpc.register(rpcName + "isAddByID", (addID) => {
	const advert = adsArray.find(el => el.ID == addID)
	if (advert) {
		if(advert.Editor && advert.Editor.length && advert.Editor !== global.localplayer.name)
			return false;

		return true;
	}
	return false;
});

gm.events.add(clientName + "add", (json) => {
	mp.events.call('notify', 0, 2, translateText("Пришло новое объявление!"), 3000);
	json = JSON.parse(json)
	adsArray.unshift(json);
	if (isList)
		mp.gui.emmit(`window.listernEvent ('updateListAdverts');`);
});

gm.events.add(clientName + "update", (id, name) => {
	const index = adsArray.findIndex(a => a.ID === id);
	if (adsArray[index]) {
		adsArray[index].Editor = name;
		if (isList)
			mp.gui.emmit(`window.listernEvent ('updateListAdverts');`);
	}
});

gm.events.add(clientName + "remove", (id) => {
	const index = adsArray.findIndex(a => a.ID == id);
	if (!adsArray[index]) return;
	adsArray.splice(index, 1);

	if (isList)
		mp.gui.emmit(`window.listernEvent ('updateListAdverts');`);
});

// Advert menu
global.advertsactive = false;

gm.events.add(clientName + "delete", (index, a) => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		mp.events.callRemote(serverName + "delete", index, a);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/advert", "client.advert.delete", e.toString());
    }
})

gm.events.add(clientName + "send", (index, a) => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		mp.events.callRemote(serverName + "send", index, a);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/advert", "client.advert.send", e.toString());
    }
})

gm.events.add('client.advert.logs', (json) => {
    mp.gui.emmit(`window.events.callEvent("cef.advert.logs", '${json}')`);
});


gm.events.add('client.advert.phone', (index) => {
	try
	{
		if(new Date().getTime() - global.lastCheck < 50) return; 
		global.lastCheck = new Date().getTime();
		mp.events.callRemote('server.advert.phone', index);
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "fractions/advert", "client.advert.phone", e.toString());
    }
})