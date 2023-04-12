const player = mp.players.local;
let antiCheatDisableCounter = 0;

const math_pow = Math.pow,
    math_cos = Math.cos,
    math_sin = Math.sin,
    math_abs = Math.abs;

(() => {
    let lastPosition = new mp.Vector3(0, 0, 0),
        lastDimension = 0,
        lastTime = 0,
        spawnTime = 0,
        noclipCount = 0,
        vehiclesControll = [],
        gravityCount = 0;

    setInterval(() => {
        var math_abs = Math.abs;

        if (0 < global.adminLevel || !global.isAuth) return;

        const time = new Date().getTime(),
            position = player.position,
            dimension = player.dimension,
            vehicle = player.vehicle;

        if (dimension != lastDimension) {
            lastTime = time;
            lastPosition = position;
            lastDimension = dimension;
            return;
        }

        const dist = Math.sqrt(math_pow(position.x - lastPosition.x, 2) + math_pow(position.y - lastPosition.y, 2));

        if (0 === dimension && 650 <= dist && 10000 < time - lastTime && 2500 < time - spawnTime && global.antiFlood("s_ac_teleport", 25000))
            return void callRemote("s_ac_teleport", `${mp.api.location.getZoneName(lastPosition.x, lastPosition.y, lastPosition.z)} - ${mp.api.location.getZoneName(position.x, position.y, position.z)} - ${Math.round(dist)}m`);
        if (
            0 === dimension &&
            (25 < dist || 1 > math_abs(lastPosition.z - position.z)) &&
            !mp.raycasting.testPointToPoint(position, new mp.Vector3(position.x, position.y, position.z - 10), player.handle, 17) &&
            10 < math_abs(position.z - mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z + 3, 0, false)) &&
            !mp.raycasting.testPointToPoint(new mp.Vector3(position.x + 1, position.y, position.z), new mp.Vector3(position.x + 1, position.y, position.z - 10), player.handle, 17) &&
            !mp.raycasting.testPointToPoint(new mp.Vector3(position.x - 1, position.y, position.z), new mp.Vector3(position.x - 1, position.y, position.z - 10), player.handle, 17) &&
            !mp.raycasting.testPointToPoint(new mp.Vector3(position.x, position.y + 1, position.z), new mp.Vector3(position.x, position.y + 1, position.z - 10), player.handle, 17) &&
            !mp.raycasting.testPointToPoint(new mp.Vector3(position.x, position.y - 1, position.z), new mp.Vector3(position.x, position.y - 1, position.z - 10), player.handle, 17)
        ) {
            if (
                !player.isSwimming() &&
                !player.isSwimmingUnderWater() &&
                !player.isClimbing() &&
                !(null != vehicle && [14, 15, 16, 17, 18, 19, 20].includes(vehicle.getClass())) &&
                !(() => {
                    let toggled = false;
                    mp.vehicles.forEachInStreamRange((vehicle) => {
                        if (!toggled && 14 === vehicle.getClass() && 15 > global.vdist2(position, vehicle.position)) {
                            toggled = true;
                        }
                    })
                    return toggled;
                })() &&
                !([0, 1, 2].includes(player.getParachuteState()))
            )
                noclipCount += 1;

            if (15 < noclipCount && global.antiFlood("s_ac_noclip", 10000))
                return callRemote("s_ac_noclip");
        } else
            noclipCount = 0;
        if (0 === dimension) {
            //const a = player.getModel();
            //if (a !== mp.game.joaat("mp_m_freemode_01") && a !== mp.game.joaat("mp_f_freemode_01") && global.antiFlood("s_ac_modelchange", 6e5)) return h("s_ac_modelchange");
            if (vehicle) vehiclesControll = [];
            else {
                const vehicles = mp.vehicles.streamed.filter((vehicle) => vehicle.controller === player);
                vehiclesControll = vehiclesControll.filter((vehicle) => mp.vehicles.exists(vehicle) && 0 !== vehicle.handle && vehicle.controller === player);
                for (const vehicle of vehicles)
                    if (-1 === vehiclesControll.indexOf(vehicle)) {
                        vehicle.__acLastCoords = vehicle.getCoords(true);
                        vehicle.__acLastSpeed = vehicle.getSpeed();
                        vehicle.__acLastDist = 0;
                        vehiclesControll.push(vehicle);
                    } else {
                        const vehiclePosition = vehicle.getCoords(true),
                            vehicleSpeed = vehicle.getSpeed(),
                            vehicleDist = global.vdist2(vehiclePosition, vehicle.__acLastCoords),
                            diffSpeed = math_abs(vehicleSpeed - vehicle.__acLastSpeed);
                        if (110 < diffSpeed) {
                            if (global.antiFlood("s_ac_veh_gravity", 600000))
                                callRemote("s_ac_veh_gravity", vehicle);
                        } else {
                            if (1 > vehicleSpeed && 1 < vehicleDist) {
                                if (10 < ++gravityCount && global.antiFlood("s_ac_veh_gravity", 600000))
                                    callRemote("s_ac_veh_gravity", vehicle);
                            } else {
                                gravityCount = 0
                            }

                            vehicle.__acLastCoords = vehiclePosition;
                            vehicle.__acLastSpeed = vehicleSpeed;
                            vehicle.__acLastDist = vehicleDist;
                        }
                    }
            }
        }
        lastPosition = position;
        lastDimension = dimension;
    }, 250);

    gm.events.add("playerSpawn", (player) => {
        if (player == mp.players.local)
            spawnTime = new Date().getTime();
    });

    const callRemote = (a, ...b) => {
        if (0 === antiCheatDisableCounter)
            mp.events.callRemoteUnreliable(a, ...b);
    };
})();

//
/*
(() => {
    let vehicleAntiCheatTrigger = 0,
        vehicleAntiCheatTaskCounter = 0,
        vehicleAntiCheatTaskLast = 0,
        vehicleAntiCheatEngineTrigger = 0,
        vehicleAntiCheatEngineTaskCounter = 0,
        vehicleAntiCheatEngineTaskLast = 0,
        vehicleAntiCheatEngine2TaskCounter = 0,
        vehicleAntiCheatEngine2TaskLast = 0;

    setInterval(() => {
        const vehicle = player.vehicle;

        if (!(null == vehicle ||
            65535 === vehicle.remoteId ||
            -1 !== lastVehicleSeatingSeat ||
            lastVehicleSeating !== vehicle ||
            13 === vehicle.getClass())) {

            if (vehicle.__сEngine) {
                if (!vehicle.getIsEngineRunning()) {
                    if (2 <= ++vehicleAntiCheatEngineTrigger && 5 <= ++vehicleAntiCheatEngineTaskCounter) {
                        if (vehicleAntiCheatEngineTaskLast + 2500 > new Date().getTime()) {
                            mp.events.callRemote("s_ac_veh_door")
                        } else {
                            vehicleAntiCheatEngineTaskCounter = 0
                        }
                        player.clearTasksImmediately();
                        vehicleAntiCheatEngineTrigger = 0;
                        vehicleAntiCheatEngineTaskLast = new Date().getTime();
                    }
                }
            } else {
                if (100 < 3.6 * vehicle.getSpeed()) {
                    if (80 < ++vehicleAntiCheatEngine2TaskCounter) {
                        if (vehicleAntiCheatEngine2TaskLast + 1000 > new Date().getTime() &&
                            global.actionAntiFlood("s_ac_veh_engine", 30000)) {
                            mp.events.callRemote("s_ac_veh_engine");
                            vehicleAntiCheatEngine2TaskCounter = 0;
                        }
                    } else {
                        vehicleAntiCheatEngine2TaskLast = new Date().getTime()
                    }
                }
            }

        }



        return null == vehicle ||
        65535 === vehicle.remoteId ||
        -1 !== lastVehicleSeatingSeat ||
        lastVehicleSeating !== vehicle ||
        13 === vehicle.getClass()
            ? void 0
            : engine
                ? engineState
                    ? void ()
                    : vehicle.getIsEngineRunning()
                        ? (vehicle.setEngineOn(!1, !0, !0),
                            vehicle.setLights(1),
                            void (
                                2 <= ++vehicleAntiCheatEngineTrigger &&
                                (5 <= ++vehicleAntiCheatEngineTaskCounter &&
                                (vehicleAntiCheatEngineTaskLast + 2500 > new Date().getTime()
                                    ? mp.events.callRemote("s_ac_veh_door")
                                    : (vehicleAntiCheatEngineTaskCounter = 0)),
                                    player.clearTasksImmediately(),
                                    (vehicleAntiCheatEngineTrigger = 0),
                                    (vehicleAntiCheatEngineTaskLast = new Date().getTime()))
                            ))
                        : void 0
                : (8 < vehicleAntiCheatTrigger &&
                0 === vehicle.getPedInSeat(-1) &&
                global.actionAntiFlood("s_ac_veh_door_f", 3e4) &&
                mp.events.callRemote("s_ac_veh_door_f"),
                    void (
                        10 <= ++vehicleAntiCheatTrigger &&
                        (5 <= ++vehicleAntiCheatTaskCounter &&
                        (vehicleAntiCheatTaskLast + 2500 > new Date().getTime()
                            ? mp.events.callRemote("s_ac_veh_door")
                            : (vehicleAntiCheatTaskCounter = 0)),
                            player.clearTasksImmediately(),
                            (vehicleAntiCheatTrigger = 0),
                            (vehicleAntiCheatTaskLast = new Date().getTime()))
                    ));
    }, 150);

    let lastVehicleSeating = null,
        lastVehicleSeatingSeat = -2;

    gm.events.add("playerEnterVehicle", (e, a) => {
        (lastVehicleSeating = e), (lastVehicleSeatingSeat = a);
    });

    gm.events.add("playerLeaveVehicle", (e) => {
        lastVehicleSeating === e &&
        ((lastVehicleSeatingSeat = -2), (lastVehicleSeating = null));
    });
})();*/