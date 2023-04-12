const config = {
    AllowVehicleClass: [0, 1, 2, 3, 4, 5, 6, 9, 18],
    MaxVehicleCubedSize: 25,
    AllowVehicleSteering: true
}

let markersShown = false;
let visibleMarkers = [null, null, null];

let PlayerPushingStarted = false;
let PlayerPushing = false;
let PlayerPushingVehicle = null;
let PlayerPushingIsFront = null;

let eventsRefresh = new Date().getTime();
global.binderFunctions.pushVehicleToggle = () => {
    try
    {
        if (global.localplayer.vehicle || global.cuffed || global.isDeath === true || global.isDemorgan === true) {
            return;
        }

        mp.gui.chat.push("pushVehicleToggle 1");
        if (!PlayerPushingStarted && global.entity !== null && mp.vehicles.exists(global.entity) && global.entity.handle !== 0) {    
            const Vehicle = global.entity;    

            if (!Vehicle.isOnAllWheels())
                return false;
                mp.gui.chat.push("pushVehicleToggle 2");

            const vehClass = Vehicle.getClass();
    
            if (config.AllowVehicleClass !== null && config.AllowVehicleClass !== false && config.AllowVehicleClass.indexOf(vehClass) === -1)
                return false;
            else if (new Date().getTime() - eventsRefresh < 1000) {
                mp.events.call('notify', 4, 9, translateText("Слишком быстро."), 1500);
                return;
            }
            mp.gui.chat.push("pushVehicleToggle 3");
        
            eventsRefresh = new Date().getTime();
            const vehicleSize = vehicleLayout(Vehicle);
    
            const distanceFront = Math.round(mp.game.system.vdist2(vehicleSize.front.x, vehicleSize.front.y, Vehicle.position.z,
                global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z) / 3);
    
            const distanceBack = Math.round(mp.game.system.vdist2(vehicleSize.back.x, vehicleSize.back.y, Vehicle.position.z,
                global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z) / 3);
    
                mp.gui.chat.push("pushVehicleToggle 4");
            Vehicle.IsInFront = (distanceFront < distanceBack);
            Vehicle.distanceFront = distanceFront;
            Vehicle.distanceBack = distanceBack;
            Vehicle.vehicleSize = vehicleSize;
    
            mp.gui.chat.push("pushVehicleToggle 5");
            if ((distanceFront <= 1.5 || distanceBack <= 1.5) && ((vehicleSize.size.lengthY * vehicleSize.size.lengthX) <= config.MaxVehicleCubedSize)) {
                mp.gui.chat.push("pushVehicleToggle 6");
                mp.events.callRemote("StartPushVehicle", Vehicle);
            }
        } else if (PlayerPushingStarted) {
            global.localplayer.detach(true, false)
            global.localplayer.stopAnimTask('missfinale_c2ig_11', 'pushcar_offcliff_m', 2);
            
            setTimeout(function () {
                global.localplayer.freezePosition(false);
            }, 100);
            
            PlayerPushing = false;
            PlayerPushingStarted = false;

            mp.events.callRemote("StopPushVehicle");
        }
        
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/push_car", "global.binderFunctions.pushVehicleToggle", e.toString());
    }
}

gm.events.add('StartPushVehicle_client', (Vehicle) => {
    try
    {
        mp.gui.chat.push("StartPushVehicle_client 1");
        const vehicleSize = vehicleLayout(Vehicle);

        const distanceFront = Math.round(mp.game.system.vdist2(vehicleSize.front.x, vehicleSize.front.y, Vehicle.position.z,
            global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z) / 3);

        const distanceBack = Math.round(mp.game.system.vdist2(vehicleSize.back.x, vehicleSize.back.y, Vehicle.position.z,
            global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z) / 3);

        Vehicle.IsInFront = (distanceFront < distanceBack);
        Vehicle.vehicleSize = vehicleSize;
            
        mp.gui.chat.push("StartPushVehicle_client 2");
        if (!PlayerPushing && !global.localplayer.isInAnyVehicle(false)) {
            PlayerPushing = true;
            mp.gui.chat.push("StartPushVehicle_client 3");
            global.localplayer.freezePosition(true);
            
            if (Vehicle.IsInFront) {
                global.localplayer.attachTo(Vehicle.handle, 6286, 0.0, Vehicle.vehicleSize.size.max.y + 0.35, Vehicle.vehicleSize.size.z + 0.95, 0.0, 0.0, 180.0, false, false, false, false, 0, true)
            } else {
                global.localplayer.attachTo(Vehicle.handle, 6286, 0.0, Vehicle.vehicleSize.size.min.y - 0.6, Vehicle.vehicleSize.size.z + 0.95, 0.0, 0.0, 0.0, false, false, false, false, 0, true)
            }

            PlayerPushingIsFront = Vehicle.IsInFront;
            PlayerPushingVehicle = Vehicle;

            mp.game.streaming.requestAnimDict('missfinale_c2ig_11');
            global.localplayer.taskPlayAnim('missfinale_c2ig_11', 'pushcar_offcliff_m', 2.0, -8.0, -1, 35, 0, false, false, false);
            global.localplayer.freezePosition(false);
            gm.discord(translateText("Толкает машину"));

            PlayerPushing = true;
            PlayerPushingStarted = false;

            setTimeout(() => {
                PlayerPushingStarted = true;
            }, 400);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/push_car", "StartPushVehicle_client", e.toString());
    }
});

function placeMarker(x, y, z, color) {
    return mp.markers.new(0, [x, y, z], 1,
        {
            direction: [x, y, z],
            rotation: new mp.Vector3(0, 0, 0),
            color: [color, 0, color, 255],
            visible: true,
            dimension: 0
        });
}

function rotateRect(angle, ox, oy, x, y, w, h) {
    const xAx = Math.cos(angle);
    const xAy = Math.sin(angle);
    x -= ox;
    y -= oy;
    return [[
        x * xAx - y * xAy + ox,
        x * xAy + y * xAx + oy,
    ], [
        (x + w) * xAx - y * xAy + ox,
        (x + w) * xAy + y * xAx + oy,
    ], [
        (x + w) * xAx - (y + h) * xAy + ox,
        (x + w) * xAy + (y + h) * xAx + oy,
    ], [
        x * xAx - (y + h) * xAy + ox,
        x * xAy + (y + h) * xAx + oy,
    ]
    ];
}

function vehicleLayout(Vehicle) {
    const sizeofVehicle = mp.game.gameplay.getModelDimensions(Vehicle.model);
    const vehicleRotation = Vehicle.getRotation(2);
    const Xwidth = (0 - sizeofVehicle.min.x) + (sizeofVehicle.max.x);
    const Ywidth = (0 - sizeofVehicle.min.y) + (sizeofVehicle.max.y);
    const degree = (vehicleRotation.z + 180) * Math.PI / 180;

    const newDegrees = rotateRect(degree, Vehicle.position.x, Vehicle.position.y, Vehicle.position.x - sizeofVehicle.max.x, Vehicle.position.y - sizeofVehicle.max.y, Xwidth, Ywidth);

    const frontX = newDegrees[0][0] + ((newDegrees[1][0] - newDegrees[0][0]) / 2);
    const frontY = newDegrees[0][1] + ((newDegrees[1][1] - newDegrees[0][1]) / 2);

    const bottomX = newDegrees[2][0] + ((newDegrees[3][0] - newDegrees[2][0]) / 2);
    const bottomY = newDegrees[2][1] + ((newDegrees[3][1] - newDegrees[2][1]) / 2);

    return {
        front: {x: frontX, y: frontY},
        back: {x: bottomX, y: bottomY},
        center: {x: Vehicle.position.x, y: Vehicle.position.y},
        size: {
            lengthX: Xwidth,
            lengthY: Ywidth,
            min: {x: sizeofVehicle.min.x, y: sizeofVehicle.min.y},
            max: {x: sizeofVehicle.max.x, y: sizeofVehicle.max.y},
            z: sizeofVehicle.min.z
        }
    };
}

gm.events.add("render", () => {
    if (PlayerPushingStarted) {
        let distanceToVeh = 0;
        let localPos = global.localplayer.position;

        if (mp.vehicles.exists(PlayerPushingVehicle) && PlayerPushingVehicle.handle !== 0)
            distanceToVeh = mp.game.system.vdist(localPos.x, localPos.y, localPos.z, PlayerPushingVehicle.position.x, PlayerPushingVehicle.position.y, PlayerPushingVehicle.position.z)
        else
            distanceToVeh = 10;

        if (distanceToVeh < 5 && global.localplayer.isPlayingAnim('missfinale_c2ig_11', 'pushcar_offcliff_m', 3)) {
            const LeftArrow = mp.keys.isDown(65);
            if (LeftArrow && config.AllowVehicleSteering) {
                global.localplayer.taskVehicleTempAction(PlayerPushingVehicle.handle, 11, 500)
            }

            const RightArrow = mp.keys.isDown(68);
            if (RightArrow && config.AllowVehicleSteering) {
                global.localplayer.taskVehicleTempAction(PlayerPushingVehicle.handle, 10, 500)
            }

            if (PlayerPushingVehicle.hasCollidedWithAnything()) {
                PlayerPushingVehicle.setOnGroundProperly()
            }

            if (!PlayerPushingIsFront) {
                PlayerPushingVehicle.setForwardSpeed(1);
            } else {
                PlayerPushingVehicle.setForwardSpeed(-1);
            }

            mp.game.graphics.drawText(translateText("Чтобы менять направление движения, используйте A и D."), [0.5, 0.9], {
                font: 4,
                color: [255, 255, 255, 255],
                scale: [0.4, 0.4],
                centre: true,
            });

            mp.game.graphics.drawText(`\nЧтобы перестать толкать транспорт, нажмите ${global.Keys[global.userBinder[53].keyCode]}.`, [0.5, 0.9], {
                font: 4,
                color: [255, 255, 255, 255],
                scale: [0.4, 0.4],
                centre: true,
            });
        }
        else if (distanceToVeh > 5 || !global.localplayer.isPlayingAnim('missfinale_c2ig_11', 'pushcar_offcliff_m', 3) || !mp.vehicles.exists(PlayerPushingVehicle)) {
            if (!global.anti_flood || global.anti_flood && new Date().getTime() - global.anti_flood >= 5000) {
                global.localplayer.detach(true, false)
                global.localplayer.stopAnimTask('missfinale_c2ig_11', 'pushcar_offcliff_m', 2);

                setTimeout(function () {
                    global.localplayer.freezePosition(false);
                }, 100);

                PlayerPushing = false;
                PlayerPushingStarted = false;

                mp.events.callRemote("StopPushVehicle");
            }

            global.anti_flood = new Date().getTime();
        }
    }
});