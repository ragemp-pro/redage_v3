//https://github.com/WillyPot/fivem_golf/blob/d7c7d030c0b9ed947beb3b158f83da1e66773df1/client/main.lua
let golf = {};

let golfhole = 0;
let golfstrokes = 0;
let totalgolfstrokes = 0;
let golfplaying = false;

// ballstate, 1 = in hole, 0 out of hole.
let ballstate = 1;
let balllocation = 0;

// golfstate, 2 on ball ready to swing, 1 free roam
let golfstate = 1;

// 0 for putter, 1 iron, 2 wedge, 3 driver.
let golfclub = 1;

// am i in idle loop mode
let inLoop = false;
let inTask = false;
let power = 0.1;
// golfball object, only used while hitting.
let mygolfball = 0;

let doingdrop = false;
let clubname = "None";

let holes = [
    { par: 5, x: -1371.3370361328, y: 173.09497070313, z: 57.013027191162, x2: -1114.2274169922, y2: 220.8424987793, z2: 63.8947830200},
    { par: 5, x: -1371.3370361328, y: 173.09497070313, z: 57.013027191162, x2: -1114.2274169922, y2: 220.8424987793, z2: 63.8947830200},
    { par: 4, x: -1107.1888427734, y: 156.581298828, z: 62.03958129882, x2: -1322.0944824219, y2: 158.8779296875, z2: 56.80027008056},
    { par: 3, x: -1312.1020507813, y: 125.8329391479, z: 56.4341888427, x2: -1237.347412109, y2: 112.9838562011, z2: 56.20140075683},
    { par: 4, x: -1216.913208007, y: 106.9870910644, z: 57.03926086425, x2: -1096.6276855469, y2: 7.780227184295, z2: 49.73574447631},
    { par: 4, x: -1097.859619140, y: 66.41466522216, z: 52.92545700073, x2: -957.4982910156, y2: -90.37551879882, z2: 39.2753639221},
    { par: 3, x: -987.7417602539, y: -105.0764007568, z: 39.585887908936, x2: -1103.506958007, y2: -115.2364349365, z2: 40.55868911743},
    { par: 4, x: -1117.0194091797, y: -103.8586044311, z: 40.8405838012, x2: -1290.536499023, y2: 2.7952194213867, z2: 49.34057998657},
    { par: 5, x: -1272.251831054, y: 38.04283142089, z: 48.72544860839, x2: -1034.80187988, y2: -83.16706085205, z2: 43.0353431701},
    { par: 4, x: -1138.319580078, y: -0.1342505216598, z: 47.98218917846, x2: -1294.685913085, y2: 83.5762557983, z2: 53.92817306518}
];

let sounds = [
    "GOLF_SWING_GRASS_LIGHT_MASTER",
    "GOLF_SWING_GRASS_LIGHT_MASTER",
    "GOLF_SWING_GRASS_PERFECT_MASTER",
    "GOLF_SWING_GRASS_MASTER",
    "GOLF_SWING_TEE_LIGHT_MASTER",
    "GOLF_SWING_TEE_PERFECT_MASTER",
    "GOLF_SWING_TEE_MASTER",
    "GOLF_SWING_TEE_IRON_LIGHT_MASTER",
    "GOLF_SWING_TEE_IRON_PERFECT_MASTER",
    "GOLF_SWING_TEE_IRON_MASTER",
    "GOLF_SWING_FAIRWAY_IRON_LIGHT_MASTER",
    "GOLF_SWING_FAIRWAY_IRON_PERFECT_MASTER",
    "GOLF_SWING_FAIRWAY_IRON_MASTER",
    "GOLF_SWING_ROUGH_IRON_LIGHT_MASTER",
    "GOLF_SWING_ROUGH_IRON_PERFECT_MASTER",
    "GOLF_SWING_ROUGH_IRON_MASTER",
    "GOLF_SWING_SAND_IRON_LIGHT_MASTER",
    "GOLF_SWING_SAND_IRON_PERFECT_MASTER",
    "GOLF_SWING_SAND_IRON_MASTER",
    "GOLF_SWING_CHIP_LIGHT_MASTER",
    "GOLF_SWING_CHIP_PERFECT_MASTER",
    "GOLF_SWING_CHIP_MASTER",
    "GOLF_SWING_CHIP_GRASS_LIGHT_MASTER",
    "GOLF_SWING_CHIP_GRASS_MASTER",
    "GOLF_SWING_CHIP_SAND_LIGHT_MASTER",
    "GOLF_SWING_CHIP_SAND_PERFECT_MASTER",
    "GOLF_SWING_CHIP_SAND_MASTER",
    "GOLF_SWING_PUTT_MASTER",
    "GOLF_FORWARD_SWING_HARD_MASTER",
    "GOLF_BACK_SWING_HARD_MASTER"
];


golf.drawGolfHud = function() {
    try {
        if (golfplaying) {
            mp.game.graphics.drawRect(0.5,0.93,0.2,0.04,0,0,0,140);
            if (golfhole != 0 ) {
                //Math.ceil()
                const distance = mp.game.system.vdist(mygolfball.position.x, mygolfball.position.y, mygolfball.position.z, holes[golfhole].x2,holes[golfhole].y2,holes[golfhole].z2);
                
                mp.game.graphics.drawText("~s~" + golfstrokes + "~r~ - ~s~" + totalgolfstrokes + "~r~ - ~s~" + clubname + "~r~ - ~s~" + distance + " m", [0.5, 0.9], {
                    scale: [0.5, 0.93],
                    outline: true,
                    color: [255, 255, 255, 185],
                    font: 0
                });
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "drawGolfHud", e.toString());
    }
};

setInterval(() => {
    try {
        if (golfplaying) {
            let playerLoc = global.localplayer.position;
            const distance = mp.game.system.vdist(playerLoc.x, playerLoc.y, playerLoc.z, -1332.7823486328,128.18229675293,57.032329559326);
            if (ballstate == 1) {
                golfhole = golfhole + 1;
                if (golfhole == 10)
                    golf.endgame();
                else
                    golf.startHole();
            }
            else {
                if (golfhole == 2 && !inTask && !doingdrop)
                    golf.idleShot();
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "setInterval", e.toString());
    }
}, 1000);

golf.startGolf = function() {
    user.stopAllScreenEffect();
    inTask = false;
    user.setCurrentWeapon('weapon_unarmed');
    golfplaying = true;
};

golf.rotateShot = function(moveType) {
    try {
        let curHeading = mygolfball.getRotation(0).z;
        if (curHeading >= 360.0)
            curHeading = 0;
        if (moveType)
            mygolfball.setHeading(curHeading-0.7);
        else
            mygolfball.setHeading(curHeading+0.7);

    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "rotateShot", e.toString());
    }
};

golf.createBall = function(x, y, z) {
    try {
        golf.destroyBall();

        mygolfball = mp.objects.new('prop_golf_ball', new mp.Vector3(x, y, z));
        mygolfball.freezePosition(true);
        mygolfball.setRecordsCollisions(true);
        mygolfball.setDynamic(true);
        mygolfball.setHasGravity(true);
        mygolfball.setHeading(global.localplayer.getRotation(0).z);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "createBall", e.toString());
    }
};

golf.destroyBall = function() {
    try {
        mygolfball.destroy();
    }
    catch (e) {}
    mygolfball = null;
};

golf.endgame = function() {
    golf.destroyBall();

    golfhole = 0;
    golfstrokes = 0;
    golfplaying = false;
    ballstate = 1;
    balllocation = 0;
    golfstate = 1;
    golfclub = 1;
    inLoop = false;
    inTask = false;
};

golf.MoveToBall = function() {
    try {
        if (golfstate == 1) {
            let ballLoc = mygolfball.position;
            let playerLoc = global.localplayer.position;
            const distance = mp.game.system.vdist(playerLoc.x, playerLoc.y, playerLoc.z, ballLoc.x, ballLoc.y, ballLoc.z);
            if (distance < 50.0) {
                if (mp.game.controls.isControlJustReleased(1, 38)) {
                    doingdrop = true;
                }
                if (distance < 5.0 && !doingdrop) {
                    golfstate = 2;
                    ballstate = 0;
                }
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "MoveToBall", e.toString());
    }
};

golf.endShot = function() {
    try {
        //TriggerEvent("attachItem","golfbag01")
        inTask = false;
        golfstrokes = golfstrokes + 1;

        const distance = mp.game.system.vdist(mygolfball.position.x, mygolfball.position.y, mygolfball.position.z, holes[golfhole].x2,holes[golfhole].y2,holes[golfhole].z2);

        if (distance < 1.5) {
            totalgolfstrokes = golfstrokes + totalgolfstrokes;
            golfstrokes = 0;
            ballstate = 1;
        }
        if (golfstrokes > 12) {
            totalgolfstrokes = golfstrokes + totalgolfstrokes;
            golfstrokes = 0;
            ballstate = 1;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "endShot", e.toString());
    }
};

golf.dropShot = function() {
    try {
        if (doingdrop) {
            const position = global.localplayer;
            const distance = mp.game.system.vdist(mygolfball.position.x, mygolfball.position.y, mygolfball.position.z, position.z, position.z, position.z);
            const distanceHole = mp.game.system.vdist(position.x, position.y, position.z, holes[golfhole].x2,holes[golfhole].y2,holes[golfhole].z2);
            if (distance < 50.0 && distanceHole > 50.0) {
                if (mp.game.controls.isControlJustReleased(1, 38)) {
                    doingdrop = false;
                    golfstrokes = golfstrokes + 1;
                    let plPos = global.localplayer.position;
                    golf.createBall(plPos.x, plPos.y, plPos.z - 1);
                }
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "dropShot", e.toString());
    }
};

golf.attachClub = function() {

};

golf.idleShot = async function() {
    try {
        power = 0.1;
        let distance = methods.distanceToPos(mygolfball.position, new mp.Vector3(holes[golfhole].x2,holes[golfhole].y2,holes[golfhole].z2));

        if (distance >= 200.0)
            golfclub = 3; // wood 200m-250m)
        else if (distance >= 150.0 && distance < 200.0)
            golfclub = 1; // iron 1 140m-180m
        else if (distance >= 120.0 && distance < 250.0)
            golfclub = 4; // iron 3 -- 120m-150m
        else if (distance >= 90.0 && distance < 120.0)
            golfclub = 5; // -- iron 5 -- 70m-120m
        else if (distance >= 50.0 && distance < 90.0)
            golfclub = 6; // iron 7 -- 50m-100m
        else if (distance >= 20.0 && distance < 50.0)
            golfclub = 2; //  wedge 50m-80m
        else
            golfclub = 0; // else putter


        while (golfstate == 2) {
            if (mp.game.controls.isControlJustReleased(1, 38)) {
                let addition = 0.5;
                if (power > 25)
                    addition = addition + 0.1;
                if (power > 50)
                    addition = addition + 0.2;
                if (power > 75)
                    addition = addition + 0.3;
                power = power + addition;
                if (power > 100.0)
                    power = 1.0
            }

            let box = (power * 2) / 1000;
            if (power > 90)
                mp.game.graphics.drawRect(0.5,0.93,box,0.02,255,22,22,210);
            else
                mp.game.graphics.drawRect(0.5,0.93,box,0.02,22,235,22,210);

            let offsetball = mygolfball.getOffsetFromInWorldCoords((power) - (power*2), 0.0, 0.0);
            let ballPos = offsetball.position;

            mp.game.graphics.drawLine(ballPos.x, ballPos.y, ballPos.z, holes[golfhole].x2,holes[golfhole].y2,holes[golfhole].z2, 222, 111, 111, 0.2);

            mp.game.graphics.drawMarker(
                27,
                holes[golfhole].x2,holes[golfhole].y2,holes[golfhole].z2,
                0, 0, 0,
                0, 0, 0,
                0.5, 0.5, 10.3,
                212, 189, 0, 105,
                false, false, 2,
                false, "", "",false
            );

            if (mp.game.controls.isControlJustReleased(1, 246)) {
                let newclub = golfclub+1;
                if (newclub > 6)
                    newclub = 0;
                golfclub = newclub;
                golf.attachClub();
            }

            if (mp.game.controls.isControlJustReleased(1, 34)) {
                golf.rotateShot(true);
            }
            if (mp.game.controls.isControlJustReleased(1, 9)) {
                golf.rotateShot(false);
            }
            if (mp.game.controls.isControlJustReleased(1, 38)) {
                golfstate = 1;
                inLoop = false
            }
            else if (!inLoop) {
                golf.loopStart();
            }

            /*
            if golfclub == 0 then
                AttachEntityToEntity(GetPlayerPed(-1), mygolfball, 20, 0.14, -0.62, 0.99, 0.0, 0.0, 0.0, false, false, false, false, 1, true)
            elseif golfclub == 3 then
                AttachEntityToEntity(GetPlayerPed(-1), mygolfball, 20, 0.3, -0.92, 0.99, 0.0, 0.0, 0.0, false, false, false, false, 1, true)
            elseif golfclub == 2 then
                AttachEntityToEntity(GetPlayerPed(-1), mygolfball, 20, 0.38, -0.79, 0.94, 0.0, 0.0, 0.0, false, false, false, false, 1, true)
            else
                AttachEntityToEntity(GetPlayerPed(-1), mygolfball, 20, 0.4, -0.83, 0.94, 0.0, 0.0, 0.0, false, false, false, false, 1, true)
            end
            * */
        }

        //PlaySoundFromEntity(-1, "GOLF_SWING_FAIRWAY_IRON_LIGHT_MASTER", GetPlayerPed(-1), 0, 0, 0)

        //golf.playGolfAnim(playAnim)
        golf.swing();

        await global.wait(3380);
        golf.endShot()

    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "idleShot", e.toString());
    }
};

golf.loopStart = function() {
    inLoop = true;
};

golf.idleLoop = async function() {

};

golf.swing = async function() {
    try {
        let enabledroll = false;
        let dir = mygolfball.getRotation(0).z;
        let mafs = golf.quickmafs();
        mygolfball.freezePosition(false);
        let rollpower = power / 3;
        let airpower = 0;

        if (golfclub == 0) {
            power = power / 3;
            let check = 5.0;

            while (check < power) {
                mygolfball.setVelocity(mafs.x*check,mafs.y*check,-0.1);
                await global.wait(20);
                check = check + 0.3;
            }

            while (power > 0) {
                mygolfball.setVelocity(mafs.x*power,mafs.y*power,-0.1);
                await global.wait(20);
                power = power - 0.3;
            }
        }
        else if (golfclub == 1) // iron 1 140m-180m
        {
            power = power * 1.85;
            airpower = power / 2.6;
            enabledroll = true;
            rollpower = rollpower / 4;
        }
        else if (golfclub == 3) // wood 200m-250m
        {
            power = power * 2.0;
            airpower = power / 2.6;
            enabledroll = true;
            rollpower = rollpower / 2;
        }
        else if (golfclub == 2) // wedge -- 50m-80m
        {
            power = power * 1.5;
            airpower = power / 2.1;
            enabledroll = true;
            rollpower = rollpower / 4.5;
        }
        else if (golfclub == 4) // iron 3 -- 110m-150m
        {
            power = power * 1.8;
            airpower = power / 2.55;
            enabledroll = true;
            rollpower = rollpower / 5;
        }
        else if (golfclub == 5) // iron 5 -- 70m-120m
        {
            power = power * 1.75;
            airpower = power / 2.5;
            enabledroll = true;
            rollpower = rollpower / 5.5;
        }
        else if (golfclub == 6) // iron 7 -- 50m-100m
        {
            power = power * 1.7;
            airpower = power / 2.45;
            enabledroll = true;
            rollpower = rollpower / 6.0;
        }

        await global.wait(2000);
        mygolfball.setVelocity(0.0,0.0,0.0);

        //if (golfclub != 0) ballCamOff()
        let ballPos = mygolfball.position;
        golf.createBall(ballPos.x, ballPos.y, ballPos.z);
        mygolfball.freezePosition(true);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "swing", e.toString());
    }
};

golf.quickmafs = function(dir) {
    try {
        let x = 0.0;
        let y = 0.0;

        if (dir >= 0.0 && dir <= 90.0)
        {
            let factor = (dir/9.2) / 10;
            x = -1.0 + factor;
            y = 0.0 - factor;
        }

        if (dir > 90.0 && dir <= 180.0) {
            let dirp = dir - 90.0;
            let factor = (dirp/9.2) / 10;
            x = factor;
            y = -1.0 + factor;
        }

        if ( dir > 180.0 && dir <= 270.0) {
            let dirp = dir - 180.0;
            let factor = (dirp/9.2) / 10;
            x = 1.0 - factor;
            y = factor;
        }

        if (dir > 270.0 && dir <= 360.0) {
            let dirp = dir - 270.0;
            let factor = (dirp/9.2) / 10;
            x = 0.0 - factor;
            y = 1.0 - factor
        }
        return {x:x,y:y};
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/golf", "quickmafs", e.toString());
    }
};

new class extends debugRender {
    constructor() {
        super("r_world_golf");
    }

    render () {
        if (golfplaying) {
            golf.drawGolfHud();
            golf.MoveToBall();
        }
    }
};

