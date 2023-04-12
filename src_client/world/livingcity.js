/*
gm.events.add("vehicleStreamOut", (entity) => 
{
	let vehNumber = entity.getNumberPlateText();
	if (!vehNumber.includes('LC0')) return;
	
	vehNumber = vehNumber.replace('LC0', '');
	let pedHandle = parseInt(vehNumber);
	if (isNaN(pedHandle) || pedHandle == 0) return;
	
	const pedEntity = mp.peds.atRemoteId(pedHandle);
	if (pedEntity == null) return;
	
	mp.events.callRemote("LivingCity_PedStreamOut", pedEntity);
});
*/
gm.events.add("entityControllerChange", async (entity, newController) => 
{
	if (!entity || !mp.peds.exists(entity)) return;
	if (!newController || newController.handle != global.localplayer.handle) return;
	
	let vehValue = entity.getVariable('LCNPC');
	if (!vehValue || vehValue == 0) return;
	
	let tries = 0;
	let vehicleEntity = mp.vehicles.atRemoteId(vehValue);
	if (!vehicleEntity) return;
	
	while(!vehicleEntity.handle || !entity.handle) 
	{
		await mp.game.waitAsync(100);
		if (tries++ >= 10) return;
	}
	entity.setIntoVehicle(vehicleEntity.handle, -1);
	entity.taskVehicleDriveWander(vehicleEntity.handle, 13, 536871307);
});