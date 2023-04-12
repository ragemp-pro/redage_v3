
let screenPed;

const fix = () => {

    const position = screenPed.position;
    const heading = screenPed.getHeading();
  
    const leftTop = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, -0.3, 0.1, 0.3);
    const rightTop = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, 0.3, 0.1, 0.3);
    const bottom = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, 0.0, 0.1, -0.5);
  
    mp.game.graphics.drawPoly( 
      leftTop.x,
      leftTop.y,
      leftTop.z,
  
      rightTop.x,
      rightTop.y,
      rightTop.z,
  
      bottom.x,
      bottom.y,
      bottom.z,
  
      0, 255, 0, 255
    );
}
function DrawGreenWall()
{
    var player = screenPed ? screenPed : screenVeh ? screenVeh : screenObj;
    var position = player.position;
    
    var r = 0;
    var g = 255;
    var b = 0;
    var a = 255;

    var size = 45;
    var halfSize = size / 1.25;
    //fix ();

    for (var heading = 0; heading <= 271; heading += 90)
    {
        //DrawLight(position, heading, new mp.Vector3(halfSize, halfSize, halfSize));
        
        var rightBottom = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, size, size, -size);
        var rightTop = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, size, size, size);

        var leftBottom = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, -size, size, -size);
        var leftTop = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, -size, size, size);

        mp.game.graphics.drawPoly(rightBottom.x, rightBottom.y, rightBottom.z, rightTop.x, rightTop.y, rightTop.z, leftTop.x, leftTop.y, leftTop.z, r, g, b, a);
        mp.game.graphics.drawPoly(leftTop.x, leftTop.y, leftTop.z, leftBottom.x, leftBottom.y, leftBottom.z, rightBottom.x, rightBottom.y, rightBottom.z, r, g, b, a);
    }
    
    //DrawLight(position, 0, new mp.Vector3(0, 0, halfSize));
    //DrawLight(position, 0, new mp.Vector3(0, 0, -halfSize));
}

function DrawLight(position, heading, offset)
{
    var lightPos = mp.game.object.getObjectOffsetFromCoords(position.x, position.y, position.z, heading, offset.x, offset.y, offset.z);
    mp.game.graphics.drawLightWithRange(lightPos.x, lightPos.y, lightPos.z, 255, 255, 255, 10, 1);
}

let isDrawGreenWall = false;

gm.events.add("render", () => {
    if (!global.loggedin) return;
    else if (!isDrawGreenWall) return;
    else if (!screenPed && !screenVeh && !screenObj) return;
    DrawGreenWall();
    global.localplayer['AFK_STATUS'] = false;
});

let lastScreen = -1;
let screenVeh;
gm.events.add('screen', async (type, gender) => {
    global.localplayer.position = new mp.Vector3(0.0, 0.0, 0.0);
    global.localplayer.setHeading(0.0);
    global.localplayer.freezePosition(true);
    await global.requestAnimDict("amb@code_human_wander_idles_fat@male@static");
    await global.wait(100);
    //mp.events.call('setTimeCmd', 12, 0, 0);
    //mp.events.call('SetWeather', 0);
    switch (type) {
        case "char":
            screenPed = mp.peds.new(gender ? mp.game.joaat('mp_m_freemode_01') : mp.game.joaat('mp_f_freemode_01'), new mp.Vector3(0.0, 0.0, 150.0), 0, 0);
            screenPed.taskPlayAnim("amb@code_human_wander_idles_fat@male@static", "static", 0.0, 1, -1, 39, 0.0, false, false, false);
            await global.wait(100);
            screenPed.freezePosition(true);
            screenPed.setPropIndex(Number (0), Number (0), Number (0), true);
            global.createCamera ("char", screenPed);
            global.updateCameraToBone ("top", screenPed);
            break;
        case "veh":
            await createVeh ("issi3", true);
            break;
        case "obj":
            screenObj = mp.objects.new(mp.game.joaat ("redagelogo"), new mp.Vector3 (0.0, 0.0, 150.0), {
                rotation: new mp.Vector3 (0, 0, 180),
                dimension: -1
            });
            await global.wait(50);
            global.createCamera ("autoshop", screenObj);
            break;
    }
    isDrawGreenWall = true;
});
gm.events.add('startVariationCam', async (variation) => {
    mp.gui.emmit(`window.router.close()`);
    global.menuOpen();
    const PropArray = [0, 1, 2, 6, 7];
    for(let i = 0; i < 8; i++) {
        if (PropArray.includes (i))
            screenPed.clearProp(i);
    }

    for(let i = 0; i < 12; i++) {
        screenPed.setComponentVariation(i, -1, 0, 0)
    }

    switch (variation) {
        case 11:
        case 8:
        case 9:
            global.updateCameraToBone ("top", screenPed);
            break;
        case 3:
        case 5:
        case 7:
            global.updateCameraToBone ("top", screenPed);
            break;
        case 6:
            global.updateCameraToBone ("shoes", screenPed);
            break;
        case 4:
            global.updateCameraToBone ("legs", screenPed);
            break;
        case 2:
        case 1:
            global.updateCameraToBone ("hat", screenPed);
            break;
    }
})

gm.events.add('startVariation', async (id, variation, d, t) => {
    lastScreen = new Date().getTime() + 5000;

    if (id === -1)
        lastScreen = new Date().getTime() + 15000;

    screenPed.setComponentVariation(Number (variation), Number (d), Number (t), 0);
    await global.wait(100);
    if (id !== -1) 
        mp.gui.takeScreenshot(`${Number (id)}_${Number (t)}.png`, 1, 100, 0);
})

gm.events.add('startPropCam', async (prop) => {
    mp.gui.emmit(`window.router.close()`);
    global.menuOpen();
    switch (prop) {
        case 0:
        case 1:
        case 2:
            global.updateCameraToBone ("hat", screenPed);
            break;
        case 6:
        case 7:
            global.updateCameraToBone ("top", screenPed);
            break;
    }

    for(let i = 0; i < 12; i++) {
        screenPed.setComponentVariation(i, -1, 0, 0);
    }
    screenPed.setComponentVariation(5, 88, 0, 0);
    const PropArray = [0, 1, 2, 6, 7];
    for(let i = 0; i < 8; i++) {
        if (PropArray.includes (i))
            screenPed.clearProp(i);
    }
})
gm.events.add('startProp', async (id, variation, d, t) => {
    lastScreen = new Date().getTime() + 5000;

    if (id === -1)
        lastScreen = new Date().getTime() + 15000;

    screenPed.setPropIndex(Number (variation), Number (d), Number (t), true)
    await global.wait(100);
    if (id !== -1) 
        mp.gui.takeScreenshot(`${Number (id)}_${Number (t)}.png`, 1, 100, 0);
})

const createVeh = async (model, toggled = false) => {
    if (screenVeh && mp.vehicles.exists(screenVeh))
        screenVeh.destroy();

    screenVeh = mp.vehicles.new(mp.game.joaat (model), new mp.Vector3(0.0, 0.0, 150.0), {
        heading: 0.0,
        numberPlate: 'RedAge',
        alpha: 255,
        color: [[0, 0, 0], [0, 0, 0]],
        locked: false,
        engine: false,
        dimension: -1
    });
    screenVeh.freezePosition(true);
    await global.IsLoadEntity (screenVeh);
    screenVeh.freezePosition(true);
    await global.IsLoadEntity (screenVeh);
    screenVeh.freezePosition(true);
    await global.wait(50);
    screenVeh.freezePosition(true);
    screenVeh.setDirtLevel(0.0);
		
    const color = [255, 255, 255];
    
    screenVeh.setCustomPrimaryColour(color[0], color[1], color[2]);
    screenVeh.setCustomSecondaryColour(color[0], color[1], color[2]);

    if (toggled)
        global.createCamera ("autoshop", screenVeh);
    return true;
}

gm.events.add('startVeh', async (model, engine = false) => {
    mp.gui.emmit(`window.router.close()`);
    global.menuOpen();
    lastScreen = new Date().getTime() + 5000;

    await createVeh (model);

    //UpdateVehicleEngine (screenVeh, engine)
    await global.wait(150);

    mp.gui.takeScreenshot(`${model}.png`, 1, 100, 0);
})


const UpdateVehicleEngine = async (vehicle, toggle) => {
	try {
		vehicle.setEngineOn(toggle, true, true);
		vehicle.setUndriveable(!toggle);
		vehicle.setLights(!toggle ? 1 : 2);
	} catch (e) {
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "UpdateVehicleEngine", e.toString());
	}
}

gm.events.add("render", () => {
    if (lastScreen != -1 && new Date().getTime() > lastScreen) {
        lastScreen = -1;
        mp.gui.emmit(`window.router.setHud();`);
        global.menuClose ();
    }
});

gm.events.add('startTestV', async (id) => {
    screenPed.setComponentVariation(id, -1, 0, 0)
})

let screenObj;
gm.events.add('startObj', async (id, model) => {
    mp.gui.emmit(`window.router.close()`);
    global.menuOpen();
    lastScreen = new Date().getTime() + 5000;

    if (screenObj && mp.objects.exists(screenObj))
        screenObj.destroy();

    screenObj = mp.objects.new(model, new mp.Vector3 (0.0, 0.0, 150.0), {
        rotation: new mp.Vector3 (0, 0, 0),
        dimension: -1,
    });

    await global.wait(100);

    mp.gui.takeScreenshot(`${id}.png`, 1, 100, 0);
})