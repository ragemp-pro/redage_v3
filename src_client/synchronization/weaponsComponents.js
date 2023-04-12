const setComponentsToPlayer = (entity, data) => {
	try 
	{
		if (entity && mp.players.exists(entity) && entity.type === 'player' && entity.handle !== 0) {
			if (!data) data = entity.getVariable("weaponComponents");
			if (data && data != "undefined" && data != "null" && data.split("|")) {
				let [weaponHash, indexData] = data.split("|");
				weaponHash = parseInt(weaponHash);
				if (!global.ComponentsData [weaponHash]) return;
				else if (!global.ComponentsData [weaponHash].Components) return;
				entity.__weaponHash = weaponHash;
		
				indexData = (indexData && indexData.length > 0) ? JSON.parse(indexData) : [];
			
				// don't touch this or you will have a bad time
				if (entity.handle != global.localplayer.handle) entity.giveWeapon(weaponHash, -1, true);

				for (let index of indexData) addComponentToPlayer(entity, weaponHash, index);
				mp.game.invoke("0xADF692B254977C0C", entity.handle, weaponHash >> 0, true);
			} else {
				removeAllComponentFromPlayer(entity);
			}
		}
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["synchronization/weaponsComponents"] < 5000) return;
		global.trycatchtime["synchronization/weaponsComponents"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "synchronization/weaponsComponents", "setComponentsToPlayer", e.toString());
	}
}

const addComponentToPlayer = (player, weaponHash, componentHash) => {
	try
	{
		if (player && mp.players.exists(player) && player.type === 'player') 
		{
			if (!player.hasOwnProperty("__weaponComponentData")) player.__weaponComponentData = new Set();

			player.__weaponComponentData.add(componentHash);
			mp.game.invoke("0xD966D51AA5B28BB9", player.handle, weaponHash >> 0, componentHash >> 0);
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/weaponsComponents", "addComponentToPlayer", e.toString());
	}
}

const removeAllComponentFromPlayer = (player) => {
	try
	{
		if (player && mp.players.exists(player) && player.type === 'player') 
		{
			if (!player.hasOwnProperty("__weaponHash")) return;
			if (!player.hasOwnProperty("__weaponComponentData")) return;

			for (let component of player.__weaponComponentData) mp.game.invoke("0x1E8BE90C74FB4C09", player.handle, player.__weaponHash >> 0, component >> 0);
			player.__weaponComponentData = new Set();
			delete player.__weaponComponentData;
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/weaponsComponents", "removeAllComponentFromPlayer", e.toString());
	}
}

mp.events.addDataHandler("weaponComponents", (entity, value, oldValue) => {
    setComponentsToPlayer (entity, value);
});


gm.events.add("playerStreamIn", (entity) => {
	setComponentsToPlayer (entity, null);
});


gm.events.add("playerStreamOut", (entity) => {
	if (entity && entity.hasOwnProperty("__weaponComponentData")) {
		entity.__weaponComponentData = new Set();
		delete entity.__weaponComponentData;
	}
});