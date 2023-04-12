const objectType =  {
    Target: 1,
    StartCrash: 2,
    Explode: 3,
    Heli: 4
}

const zoneRadius = 250;

const heliHash = mp.game.joaat("maverick");

mp.events.addDataHandler("HeliCrash", (entity, value) => {

    entity.heliCrash = value;
    entity.notifyStreaming = true;

    createHeli(entity);
});

gm.events.add("objectStreamIn", (entity) => {
    if (entity.heliCrash)
        createHeli(entity, true);
});

gm.events.add("objectStreamOut", (entity) => {
    if (entity.heliCrash) {
        //destroyFx ();
        destroyHeli ();
    }
});

const randomPositionInCircle = (x, y, radius) => {
    let angle = Math.random() * Math.PI * 2;
    return new mp.Vector3(x + Math.cos(angle) * radius, y + Math.sin(angle) * radius, 0);
}

const heliData = {
    mainObj: null,
    vehicle: null,
    driver: null,
    hidden: null,
    fx: null,
    isStart: null
}

const createHeli = async (entity, isStream = false) => {
    const type = entity.heliCrash;
    const position = entity.position;

    if (objectType.Explode === type) {
        mp.events.call("sounds.playPosAmbient", "heliCrash", "cloud/sound/heliCrash.mp3", position, {
            maxDistance: 1500,
            fade: 1000,
            volume: 1
        });
    }

    if ([objectType.Target, objectType.StartCrash, objectType.Explode].includes(type)) {
        destroyFx ();

        if (!entity.handle)
            return;

        const posZ = Math.round(position.z + 50);

        if (heliData.vehicle === null) {

            let cord = randomPositionInCircle(position.x, position.y, zoneRadius)
            const vehicle = mp.vehicles.newLegacy(heliHash, new mp.Vector3(cord.x, cord.y, posZ),
            {
                numberPlate: "helicrash",
                color: [[28, 29, 33], [28, 29, 33]],
                dimension: 0
            }, vehicle => {

                vehicle.freezePosition(true);
                vehicle.setInvincible(true);
                vehicle.setGravity(false);
                vehicle.setEngineOn(true, true, true);

                updateType (type, position);
            });
            //heliData.vehicle.streamRange = 9999;
            heliData.vehicle = vehicle;
        }

        if (heliData.driver === null) {
            const driver = mp.peds.newLegacy(mp.game.joaat('s_m_y_fireman_01'), heliData.vehicle.position, 0, ped => {
                ped.setIntoVehicle(heliData.vehicle.handle, -1);
                updateType (type, position);
            }, mp.players.local.dimension);
            heliData.driver = driver;
        }

        if (heliData.hidden === null) {
            const hidden = mp.peds.newLegacy(mp.game.joaat('s_m_y_fireman_01'), new mp.Vector3(position.x, position.y, position.z - 20), 0, ped => {
                ped.freezePosition(false);
                ped.setVisible(false, false);
                ped.setAlpha(0);
                updateType (type, position);
            }, mp.players.local.dimension);
            heliData.hidden = hidden;
        }

        updateType (type, position);
    }
    else if (objectType.Heli === type) {
        if (isStream)
            return;

        destroyHeli ();

        heliData.mainObj = entity;
        heliData.fx = new global.particleFx("core", "ent_amb_smoke_chicken", {
            scale: 3.5
        });
        heliData.fx.playOnCoords(position.x, position.y, position.z, -1);
    }
}

gm.events.add(global.renderName ["1s"], () => {
   if (heliData.fx !== null && !mp.objects.exists(heliData.mainObj)) {
       destroyFx ();
   }
});


const updateType = async (type, position) => {
    if (type === heliData.isStart)
        return;

    if (heliData.vehicle.handle === 0)
        return;

    heliData.vehicle.setGravity(true);

    const posZ = Math.round(position.z + 50);

    if (objectType.Target === type) {
        if (heliData.driver.handle === 0)
            return;

        if (heliData.hidden.handle === 0)
            return;

        heliData.isStart = type;

        heliData.driver.taskHeliMission(heliData.vehicle.handle, 0, heliData.hidden.handle, position.x, position.y, posZ, 23, 15, 5, 0xBF800000, 35, 35, 5.0, 0);

    } else if (objectType.StartCrash === type) {
        if (heliData.driver.handle === 0)
            return;
        heliData.isStart = type;
        heliData.vehicle.freezePosition(false);

        const posVehZ = heliData.vehicle.position.z;
        heliData.driver.taskHeliMission(heliData.vehicle.handle, 0, 0, position.x, position.y, posVehZ, 9, 25.0, 5, 35, 10, 60, 5.0, 1024);
        heliData.vehicle.setInvincible(false)
    }
    else if (objectType.Explode === type) {
        heliData.isStart = type;
        const posVehZ = heliData.vehicle.position.z;
        heliData.vehicle.setCoordsNoOffset(position.x, position.y, posVehZ, false, false, false);
        heliData.vehicle.freezePosition(true);
        heliData.vehicle.explode(true, true);
    }

    await global.wait(50);
    heliData.vehicle.freezePosition(false);
}

const destroyHeli = () => {
    if (heliData.vehicle !== null)
        heliData.vehicle.destroy();

    if (heliData.driver !== null)
        heliData.driver.destroy();

    if (heliData.hidden !== null)
        heliData.hidden.destroy();

    heliData.vehicle = null;
    heliData.driver = null;
    heliData.hidden = null;
    heliData.isStart = null;
}

const destroyFx = () => {
    if (heliData.fx !== null)
        heliData.fx.objRemove();

    heliData.fx = null;
    heliData.mainObj = null;
}



new class extends keyClamp {
    constructor() {
        super("heliCrash", global.Keys.VK_E);
        this.animDictionary = "mp_weapons_deal_sting";
        this.animName = "crackhead_bag_loop";
        this.animFlag = gm.animationFlags.StopOnLastFrame;
    }

    confirm () {
        //gm.playAnimation (localplayer, this.animDictionary, this.animName, 4, this.animFlag);
        this.intervalId = setInterval(() => {
            this.setValue(-1);
        }, 8)
    }

    cancel () {
        //gm.stopAnimation (localplayer, this.animDictionary, this.animName);
    }
}