let trafficState = false;

mp.keys.bind(36, true, function () {
    trafficState = !trafficState;

    if (trafficState) {
        mp.game.streaming.setPedPopulationBudget(3);
        mp.game.streaming.setVehiclePopulationBudget(3);
    } else {
        mp.game.streaming.setPedPopulationBudget(0);
        mp.game.streaming.setVehiclePopulationBudget(0);

        const pos = global.localplayer.position;
        mp.game.gameplay.clearAreaOfPeds(pos.x, pos.y, pos.z, 10000, 1);
        mp.game.gameplay.clearAreaOfVehicles(pos.x, pos.y, pos.z, 10000, false, false, false, false, false, 0);
    }
});


mp.events.add('render', ()=>{

    if(!trafficState){
        return;
    }

    mp.game.ped.setPedDensityMultiplierThisFrame(3);
    mp.game.vehicle.setVehicleDensityMultiplierThisFrame(3);
});