mp.events.add('entityStreamIn', (entity) => {
    if (entity) {
        if (entity.type === 'player' && mp.players.exists(entity)) {
            entity.setLodDist(global.getLodDist (global.DistancePlayer));
            mp.events.call('pPlayerStreamIn', entity);
            mp.events.call('playerStreamIn', entity);
        } else if (entity.type === 'vehicle' && mp.vehicles.exists(entity)) {
            entity.setLodDist(global.getLodDist (global.DistanceVehicle));
            mp.events.call('vehicleStreamIn', entity);
        } else if (entity.type === 'ped' && mp.peds.exists(entity)) {
            entity.setLodDist(global.getLodDist (global.DistancePlayer));
            mp.events.call('pedStreamIn', entity);
        } else if (entity.type === 'object' && mp.objects.exists(entity)) {
            mp.events.call('objectStreamIn', entity);
        }
    }
});

mp.events.add('entityStreamOut', (entity) => {
    if (entity) {
        if (entity.type === 'player' && mp.players.exists(entity)) {
            mp.events.call('playerStreamOut', entity);
        } else if (entity.type === 'vehicle' && mp.vehicles.exists(entity)) {
            mp.events.call('vehicleStreamOut', entity);
        } else if (entity.type === 'ped' && mp.peds.exists(entity)) {
            mp.events.call('pedStreamOut', entity);
        } else if (entity.type === 'object' && mp.objects.exists(entity)) {
            mp.events.call('objectStreamOut', entity);
        }
    }
});

mp.peds.newLegacy = (hash, position, heading, streamIn, dimension) => {
    let ped = mp.peds.new(hash, position, heading, dimension);
    ped.streamInHandler = streamIn;
    return ped;
};

mp.events.add("pedStreamIn", entity => {
    if (entity.streamInHandler) {
        entity.streamInHandler(entity);
    }
});

mp.vehicles.newLegacy = (hash, position, parameters, streamIn) => {
    let vehicle = mp.vehicles.new(hash, position, parameters);
    vehicle.streamInHandler = streamIn;
    return vehicle;
};

mp.events.add("vehicleStreamIn", entity => {
    if (entity.streamInHandler) {
        entity.streamInHandler(entity);
    }
});