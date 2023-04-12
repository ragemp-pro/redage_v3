const localplayer = mp.players.local;
gm.events.add(global.renderName ["5s"], () => {
    const vehicle = localplayer.vehicle;

    if (localplayer.mileageClass) {
        if (!(vehicle && vehicle.getPedInSeat(-1) === localplayer.handle && localplayer.mileageClass.vehicle.handle === vehicle.handle))
            localplayer.mileageClass = null
    } else if (vehicle && vehicle.getPedInSeat(-1) === localplayer.handle) {
        localplayer.mileageClass = new MileageClass(vehicle);
    }
});

gm.events.add(global.renderName ["200ms"], () => {
    if (localplayer.mileageClass && localplayer.vehicle)
        localplayer.mileageClass.calc();
});

gm.events.add(global.renderName ["10s"], () => {
    if (localplayer.mileageClass && localplayer.vehicle)
        localplayer.mileageClass.update();
});

gm.events.add("vehicles.mileage.onUpdate", vehicle => {
    if (localplayer.mileageClass && localplayer.vehicle)
        localplayer.mileageClass.reset(vehicle);
});

class MileageClass {
    constructor(vehicle) {
        this.vehicle = vehicle;
        this.centimeters = 0;
    }
    calc() {
        const speed = 3.6 * this.vehicle.getSpeed() * 1000 / 3600 * 100;
        this.centimeters += parseInt(speed);
    }
    update() {
        if (this.centimeters > 0 && mp.vehicles.exists(this.vehicle) && this.vehicle.remoteId) {
            mp.events.callRemote('server.vehicle.updateMileage', this.vehicle.remoteId, this.centimeters);
            this.centimeters = 0;
        }
    }
    reset(vehicle) {
        if (mp.vehicles.exists(vehicle) && vehicle.handle === this.vehicle.handle)
            this.centimeters = 0;
    }
}