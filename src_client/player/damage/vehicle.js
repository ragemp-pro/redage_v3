gm.events.add("client.vehicle.setEngineHealth", () => {
    //entity.setBodyHealth(-999);
    //entity.setEngineHealth(-999);
    //entity.setHealth(-999);
});

const isSomebodyInVehicle = (vehicle) => {
    const r = vehicle.getMaxNumberOfPassengers();
    for (let i = -1; i < r; i += 1) {
        const playerHandle = vehicle.getPedInSeat(i);
        if (0 === playerHandle)
            continue;
        if (mp.players.atHandle(playerHandle))
            return true;
    }
    return false;
}

gm.events.add("vehicleStreamIn", (entity) => {
    entity.setInvincible(!isSomebodyInVehicle(entity))
    entity.isDamageTime = -1;
});

setInterval(() => {
    mp.vehicles.forEachInStreamRange((vehicle) => {
        if (!vehicle.handle)
            return;

        const isPassengers = isSomebodyInVehicle(vehicle);

        if (typeof vehicle.isDamageTime === "undefined")
            vehicle.isDamageTime = -1;

        if (!isPassengers) {
            if (++vehicle.isDamageTime === 15) {
                vehicle.setInvincible(true);
            }
        } else if (vehicle.isDamageTime !== 0) {
            vehicle.setInvincible(false);
            vehicle.isDamageTime = 0;
        }

        const
            getEngineHealth = vehicle.getEngineHealth(),
            getBodyHealth = vehicle.getBodyHealth();

        if (getEngineHealth < 0)
            vehicle.setEngineHealth(0);

        if (getBodyHealth < 0)
            vehicle.setBodyHealth(0);
    });
}, 1000);