let ticketOpen = false;

gm.events.add('client.ticket.open', (json) => {
	try
	{
		if (global.menuCheck()) return;
        json = JSON.parse (json)

        json.model = mp.game.vehicle.getDisplayNameFromVehicleModel(json.model);
		global.menuOpen();
        ticketOpen = true;
		mp.gui.emmit(`window.router.setView("FractionsTicket", '${JSON.stringify (json)}');`, 1);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('client.ticket.close', () => {
	try
	{
        if (!ticketOpen)
            return;
        ticketOpen = false;
		global.menuClose();
		mp.gui.emmit(`window.router.setHud();`);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('client.ticket.end', (ticketText, ticketPrice, cameraLink, isEvac) => {
	try
	{
        if (!ticketOpen)
            return;
            
        mp.events.callRemote('server.ticket.end', ticketText, ticketPrice, cameraLink, isEvac);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('client.ticket.myOpen', (json) => {
	try
	{
		if (global.menuCheck()) return;
		global.menuOpen();
        ticketOpen = true;

		json = JSON.parse (json);

		let newData = [];

		json.forEach(data => {
			if (!isNaN(Number (data.Model)))
				data.Model = mp.game.vehicle.getDisplayNameFromVehicleModel(Number (data.Model));

			newData.push(data)
		})

		mp.gui.emmit(`window.router.setView("PlayerTickets", '${JSON.stringify(newData)}');`, 1);
		gm.discord(translateText("Оплачивает штрафы"));
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('client.ticket.payment', (autoId) => {
	try
	{
        if (!ticketOpen)
            return;
            
        mp.events.callRemote('server.ticket.payment', autoId);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "openPetrol", e.toString());
	}
});

gm.events.add('client.ticket.addRender', () => {
	mp.events.add("render", OnTicketRender);
});

gm.events.add('client.ticket.removeRender', () => {
	mp.events.remove("render", OnTicketRender);
});

const OnTicketRender = () => {
	try
	{
		if (!global.loggedin) return;
            
        const pPos = global.localplayer.position;
		let 
			vPos,
			dist;
		mp.vehicles.forEachInStreamRange(vehicle => {
			if (vehicle.handle !== 0 && vehicle !== global.localplayer.vehicle && vehicle.isTicket) {
				if (!vehicle.isOnScreen())
					return;
				vPos = vehicle.position;
				dist = global.vdist2(pPos, vPos);

				if (dist < 35) {
					vPos.z += getVehicleHeight (vehicle) + 1;
					vPos = mp.game.graphics.world3dToScreen2d(vPos);
                    global.DrawSprite("redage_textures_001", "ticket", [255, 255, 255, 255], vPos.x, vPos.y);

				}
			}
		});
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/petrol", "OnTicketRender", e.toString());
	}
}