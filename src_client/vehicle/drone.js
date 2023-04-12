global.IsDrone = false;

let drone = {};

let isInDrone = false;

//CONTROL
let contRcRatePR = 1.5;
let contRcRateY = 1.5;
let contExpoPR = 0.8;
let contExpoY = 0.8;
let contRateP = 0.5;
let contRateR = 0.5;
let contRateY = 0.3;
let PlayeInitPos = null;



gm.events.add('client.dron.init', (posX, posY, posZ) => {
	PlayeInitPos = new mp.Vector3(posX, posY, posZ);
});

gm.events.add("playerEnterVehicle", (entity, seat) => {
	try
	{
        if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0 && entity.model === mp.game.joaat("rcbandito") && entity.getVariable("isDrone")) {
            isInDrone = true;
            global.IsDrone = true;
        }
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/circle", "playerEnterVehicle", e.toString());
	}
});
gm.events.add("playerLeaveVehicle", () => {
    drone.exit ()
})

drone.exit = function() {
    if (!global.loggedin) return;
    if (isInDrone) {
        isInDrone = false;
        vision_state = 0;
        global.IsDrone = false;
        PlayeInitPos = null;
        exitTime = 0;
        exitCount = 10;
        global.stopAllScreenEffect();
    }
};

drone.isDrone = function() {
    return isInDrone;
};

drone.calculateDegSec = function(input, rcRate, gRate, expo) {
    let RPY_useRates = 1.0 - Math.abs(input) * gRate;
    let input2 = ((input*input*input)*expo + input*(1 - expo));
    return 200.0 / RPY_useRates * input2 * rcRate;
};

drone.calculateDegSecPitch = function(inputP) {
    return drone.calculateDegSec(inputP, contRcRatePR, contRateP, contExpoPR);
};

drone.calculateDegSecRoll = function(inputR) {
    return drone.calculateDegSec(inputR, contRcRatePR, contRateR, contExpoPR);
};

drone.calculateDegSecYaw = function(inputY) {
    return drone.calculateDegSec(inputY, contRcRateY, contRateY, contExpoY);
};

drone.vectorMag = function(vector) {
    return Math.sqrt(vector.x * vector.x + vector.y * vector.y);
};

drone.vectorNorm = function(vector) {
    return Math.sqrt(vector.x * vector.x + vector.y * vector.y);
};

let maxSpeed = 13;
let speedSlowly = 0.1;
let currentSpeed = 0;
let currentSpeedZ = 0;
let speedOffset = 0.4;
let speedLeftRight = 0.9;
let vision_state = 0;

drone.keyPressToggleVision = function () {
    try {
        if (isInDrone) {
            if (vision_state === 0) {
                mp.game.graphics.setNightvision(true);
                vision_state = 1;
            }
            else if (vision_state === 1) {
                mp.game.graphics.setNightvision(false);
                mp.game.graphics.setSeethrough(true);
                vision_state = 2;
            }
            else {
                mp.game.graphics.setNightvision(false);
                mp.game.graphics.setSeethrough(false);
                vision_state = 0;
            }
            mp.game.audio.playSoundFrontend(-1, "SELECT", "HUD_FRONTEND_DEFAULT_SOUNDSET", false);
        }
    }
    catch (e) {

    }
};

//ESC
mp.keys.bind(0x1B, true, function() {
    if (isInDrone) {
        global.localplayer.clearTasks();
        global.localplayer.clearTasksImmediately();
    }
});

const maxDist = 150;

let exitTime = 0;
let exitCount = 10;
gm.events.add("render", () => {
    if (!isInDrone)
        return;
    let v = global.localplayer.vehicle;
    if (v) {
        if (PlayeInitPos && new Date().getTime() >= exitTime) {
            const playerLoc = global.localplayer.position;
            const distance = mp.game.system.vdist(PlayeInitPos.x, PlayeInitPos.y, PlayeInitPos.z, playerLoc.x, playerLoc.y, playerLoc.z);
            if (distance > maxDist) {
                exitTime = new Date().getTime() + 1000;
                mp.gui.chat.push(translateText("Вы слишком далеко от места запуска коптера. Вернитесь ближе, иначе связь оборвётся через {0} секунд.", exitCount));
                exitCount--;
                if (exitCount < 0) {
                    drone.exit ();
                    global.localplayer.clearTasks();
                    global.localplayer.clearTasksImmediately();
                }
            } else {
                exitTime = 0;
                exitCount = 10;
            }
        }


        drone.disableControls();


        let isPressS = mp.game.controls.isDisabledControlPressed(0, 32); //S
        let isPressW = mp.game.controls.isDisabledControlPressed(0, 33); //W
        let isPressA = mp.game.controls.isDisabledControlPressed(0, 34); //A
        let isPressD = mp.game.controls.isDisabledControlPressed(0, 35); //D

        let inputPitch    = isPressS ? speedOffset : 0; //S
        let inputRoll     = isPressW ? speedOffset * -1 : 0; //W
        let inputYaw      = isPressA ? speedLeftRight : 0; //A
        let inputThrottle = isPressD ? speedLeftRight * -1 : 0; //D

        let hasColl = v.hasCollidedWithAnything();

        let offsetStop = 0;
        let offsetRotRoll = 0;
        let offsetRotYaw = 0;

        if (isPressS)
            offsetRotRoll = -0.6;
        if (isPressW)
            offsetRotRoll = 0.6;

        if (isPressA && (currentSpeed > 5 || currentSpeed < -5))
            offsetRotYaw = -2;
        if (isPressD && (currentSpeed > 5 || currentSpeed < -5))
            offsetRotYaw = 2;


        let zOffset = 0;
        let zOffsetStop = 0;
        if (mp.game.controls.isDisabledControlPressed(0, 44) && !hasColl) //Q
            zOffset = 0.004;
        if (mp.game.controls.isDisabledControlPressed(0, 20) && !hasColl) //Z
            zOffset = -0.004;

        if (!mp.game.controls.isDisabledControlPressed(0, 20) && !mp.game.controls.isDisabledControlPressed(0, 44) && !hasColl) {
            if (currentSpeedZ < -0.1)
                zOffsetStop = 0.004;
            else if (currentSpeedZ > 0.1)
                zOffsetStop = -0.004;
            else if (currentSpeedZ < 0)
                zOffsetStop = 0.001;
            else if (currentSpeedZ > 0)
                zOffsetStop = -0.001;
        }

        if (!isPressA && !isPressD) {
            if (v.getRotation(0).y < -1)
                offsetRotYaw = 2;
            else if (v.getRotation(0).y > 1)
                offsetRotYaw = -2;
            else if (v.getRotation(0).y < 0)
                offsetRotYaw = 0.001;
            else if (v.getRotation(0).y > 0)
                offsetRotYaw = -0.001;
        }

        if (!isPressS && !isPressW && currentSpeed !== 0) {
            if (currentSpeed < -1)
                offsetStop = 0.05;
            else if (currentSpeed > 1)
                offsetStop = -0.05;
            else if (currentSpeed < 0)
                offsetStop = 0.001;
            else if (currentSpeed > 0)
                offsetStop = -0.001;

            if (v.getRotation(0).x < -1)
                offsetRotRoll = 0.6;
            else if (v.getRotation(0).x > 1)
                offsetRotRoll = -0.6;
            else if (v.getRotation(0).x < 0)
                offsetRotRoll = 0.001;
            else if (v.getRotation(0).x > 0)
                offsetRotRoll = -0.001;
        }

        let yoff = inputPitch + inputRoll;
        let xoff = inputYaw + inputThrottle;

        currentSpeed += yoff + offsetStop;

        /*mp.game.graphics.drawText(`YO:${yoff}\nXO:${xoff}\nZO:${zOffset}\nRL:${offsetRotRoll}\nSPEED:${currentSpeed.toFixed(2)}\nSTOP:${offsetStop}`, [0.5, 0.005], {
            font: 7,
            color: [255, 255, 255, 255],
            scale: [0.4, 0.4],
            outline: true
        });*/

        if (v.isInWater())
            zOffset = 0.01;

        let speedOffsetZ = 0;
        if (currentSpeed > 1) {
            speedOffsetZ = zOffset + (v.getRotation(0).x / -200);
        }
        if (currentSpeed < -1) {
            speedOffsetZ = zOffset + (v.getRotation(0).x / 400);
        }

        currentSpeedZ += zOffset + zOffsetStop;
        if (currentSpeedZ > maxSpeed / 100)
            currentSpeedZ = maxSpeed / 100;
        if (currentSpeedZ < maxSpeed / -100)
            currentSpeedZ = maxSpeed / -100;

        if (maxSpeed < currentSpeed)
            currentSpeed = maxSpeed;
        if ((maxSpeed * -1 / 2) > currentSpeed)
            currentSpeed = (maxSpeed * -1 / 2);

        if (hasColl && currentSpeed > 5)
            currentSpeed = 5;
        /*if (hasColl && currentSpeedZ > 0.05)
            currentSpeedZ = 0.05;*/

        let newPos = v.getOffsetFromInWorldCoords(0, currentSpeed / 50, currentSpeedZ + speedOffsetZ);
        let heading = v.getRotation(0).z;

        //let vel = v.getVelocity();
        //v.setVelocity(vel.x, vel.y, 0);
        v.setVelocity(0, currentSpeed / 30, currentSpeedZ + (v.getRotation(0).x / - 100));

        let finalX = offsetRotRoll + v.getRotation(0).x;
        let finalY = offsetRotYaw + v.getRotation(0).y;

        if (finalX > 25)
            finalX = 25;
        if (finalX < -25)
            finalX = -25;

        if (finalY > 50)
            finalY = 50;
        if (finalY < -50)
            finalY = -50;

        v.setRotation(finalX, finalY, heading + xoff, 0, false);
        v.setCoordsNoOffset(newPos.x, newPos.y, newPos.z, true, true, true);
    }
});

drone.disableControls = function() {
    mp.game.controls.disableControlAction(0, 85, true);//Q
    //mp.game.controls.disableControlAction(0, 75,true);//F

    mp.game.controls.disableControlAction(0, 8, true);
    mp.game.controls.disableControlAction(0, 9, true);
    mp.game.controls.disableControlAction(0, 30, true);
    mp.game.controls.disableControlAction(0, 31, true);
    mp.game.controls.disableControlAction(0, 32, true);
    mp.game.controls.disableControlAction(0, 33, true);
    mp.game.controls.disableControlAction(0, 34, true);
    mp.game.controls.disableControlAction(0, 35, true);
    mp.game.controls.disableControlAction(0, 36, true);
    mp.game.controls.disableControlAction(0, 63, true);
    mp.game.controls.disableControlAction(0, 64, true);
    mp.game.controls.disableControlAction(0, 71, true);
    mp.game.controls.disableControlAction(0, 72, true);
    mp.game.controls.disableControlAction(0, 77, true);
    mp.game.controls.disableControlAction(0, 78, true);
    mp.game.controls.disableControlAction(0, 78, true);
    mp.game.controls.disableControlAction(0, 87, true);
    mp.game.controls.disableControlAction(0, 88, true);
    mp.game.controls.disableControlAction(0, 89, true);
    mp.game.controls.disableControlAction(0, 90, true);
    mp.game.controls.disableControlAction(0, 129, true);
    mp.game.controls.disableControlAction(0, 130, true);
    mp.game.controls.disableControlAction(0, 133, true);
    mp.game.controls.disableControlAction(0, 134, true);
    mp.game.controls.disableControlAction(0, 136, true);
    mp.game.controls.disableControlAction(0, 139, true);
    mp.game.controls.disableControlAction(0, 146, true);
    mp.game.controls.disableControlAction(0, 147, true);
    mp.game.controls.disableControlAction(0, 148, true);
    mp.game.controls.disableControlAction(0, 149, true);
    mp.game.controls.disableControlAction(0, 150, true);
    mp.game.controls.disableControlAction(0, 151, true);
    mp.game.controls.disableControlAction(0, 232, true);
    mp.game.controls.disableControlAction(0, 266, true);
    mp.game.controls.disableControlAction(0, 267, true);
    mp.game.controls.disableControlAction(0, 268, true);
    mp.game.controls.disableControlAction(0, 269, true);
    mp.game.controls.disableControlAction(0, 278, true);
    mp.game.controls.disableControlAction(0, 279, true);
    mp.game.controls.disableControlAction(0, 338, true);
    mp.game.controls.disableControlAction(0, 339, true);
    mp.game.controls.disableControlAction(0, 44, true);
    mp.game.controls.disableControlAction(0, 20, true);
    mp.game.controls.disableControlAction(0, 47, true);
};

global.stopAllScreenEffect = function() {
    Natives.ANIMPOSTFX_STOP_ALL ();

    mp.game.graphics.setNightvision(false);
    mp.game.graphics.setSeethrough(false);
    mp.game.graphics.setTimecycleModifierStrength(0);

    mp.game.graphics.setNoiseoveride(false);
    mp.game.graphics.setNoisinessoveride(0);

    mp.game.graphics.transitionFromBlurred(0);
};

mp.events.addDataHandler("isDrone", (entity, value, oldValue) => {
	SetDrone (entity, value);
});

gm.events.add("vehicleStreamIn", (entity) => {
    SetDrone(entity, null);
});

const SetDrone = (entity, data) => {
    if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.handle !== 0 && entity.model === mp.game.joaat("rcbandito")) {
        if (!data) data = entity.getVariable("isDrone");
        if (data) {
            entity.setAlpha(0);
            global.playSound(entity, "Flight_Loop", "DLC_Arena_Drone_Sounds");
            entity.setCanBeDamaged(false);
            entity.setInvincible(true);
        }
    }
}

global.playSound = function(vehicle, name, ref = "") {
    try {
        if (vehicle !== undefined && mp.vehicles.exists(vehicle)) {

            let sId = Natives.GET_SOUND_ID ();
            //mp.game.invoke(methods.PLAY_SOUND_FROM_ENTITY, sId, name, veh.handle, ref, 0, 0);
            mp.game.audio.playSoundFromEntity(sId, name, vehicle.handle, ref, true, 0);
        }
    }
    catch (e) {
        methods.debug(e);
    }
};
/*
global.stopSound = function(vehId, prefix) {
    try {
        let veh = mp.vehicles.atRemoteId(vehId);
        if (veh !== undefined && mp.vehicles.exists(veh)) {
            if (vehicles.has(veh.remoteId, prefix + 'currentSound')) {
                let sId = vehicles.get(veh.remoteId, prefix + 'currentSound');
                mp.game.audio.stopSound(sId);
                mp.game.audio.releaseSoundId(sId);
                vehicles.reset(veh.remoteId, prefix + 'currentSound');
            }
        }
    }
    catch (e) {
        methods.debug(e);
    }
};*/