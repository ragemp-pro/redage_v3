const activeResolution = mp.game.graphics.getScreenActiveResolution(0, 0);

let MOVE_SENSITIVTY = 50;
let ROT_SENSITIVITY = 800;

let selObj = null;
let oldPos;
let oldRot;
let mode = 'Move';
let curBtn = "";
let LastCursorPos = [0, 0];

let xbox;
let ybox;
let zbox;
let switchbox;
let groundbox;
let cancelbox;
let savebox;

gm.events.add('objecteditor:start', (objid) => {
    mp.gui.cursor.show(true, true);
    selObj = mp.objects.at(objid);
    selObj.setCollision(false, false);
    oldPos = selObj.position;
    oldRot = selObj.rotation;
});

let Buttons = {
	x: [0.0, 0.0, 0.0, 0.0],
	y: [0.0, 0.0, 0.0, 0.0],
	z: [0.0, 0.0, 0.0, 0.0],
};

let IsDowned = false;


/*
if(xbox != undefined && mouseRel.x >= Buttons["x"][0]-0.01 && mouseRel.x <= Buttons["x"][0]+0.009 && mouseRel.y >= Buttons["x"][1]-0.015 && mouseRel.y <= Buttons["x"][1]+0.009) {
    curBtn = 'x';
} else if(ybox != undefined && mouseRel.x >= Buttons["y"][0]-0.01 && mouseRel.x <= Buttons["y"][0]+0.009 && mouseRel.y >= Buttons["y"][1]-0.015 && mouseRel.y <= Buttons["y"][1]+0.009) {
    curBtn = 'y';
} else if(zbox != undefined && mouseRel.x >= Buttons["z"][0]-0.01 && mouseRel.x <= Buttons["z"][0]+0.009 && mouseRel.y >= zbox.y-0.015 && mouseRel.y <= zbox.y+0.009)*/


gm.events.add("render", () => {
    if (selObj && mp.objects.exists (selObj) && selObj.handle != 0) {
        if (!selObj.isOnScreen())
            return;

        const
            cursorPos = mp.gui.cursor.position,
            resX = activeResolution.x,
            resY = activeResolution.y;

        const xStart = selObj.getOffsetFromInWorldCoords(-0.85, 0.0, 0.0);
        const xEnd = selObj.getOffsetFromInWorldCoords(0.85, 0.0, 0.0);
        Buttons["x"] = global.DrawAxis(mode == 'Move' ? "X" : "RX", xStart, xEnd, [153, 153, 204, (curBtn == "x" ? 255 : 150)]);

        const yStart = selObj.getOffsetFromInWorldCoords(0.0, -0.85, 0.0);
        const yEnd = selObj.getOffsetFromInWorldCoords(0.0, 0.85, 0.0);
        Buttons["y"] = global.DrawAxis(mode == 'Move' ? "Y" : "RY", yStart, yEnd, [190, 143, 143, (curBtn == "y" ? 255 : 150)]);

        const zStart = selObj.getOffsetFromInWorldCoords(0.0, 0.0, -0.85);
        const zEnd = selObj.getOffsetFromInWorldCoords(0.0, 0.0, 0.85);
        Buttons["z"] = global.DrawAxis(mode == 'Move' ? "Z" : "RZ", zStart, zEnd, [140, 180, 139, (curBtn == "z" ? 255 : 150)]);

        /*mp.game.controls.enableControlAction(0, 237, true);

        if (mp.game.controls.isControlPressed(0, 237)) {
            if (!IsDowned) {

                IsDowned = true;

                if (curBtn == "") {
                    const x = cursorPos[0] / resX;
                    const y = cursorPos[1] / resY;
                    for (let key in Buttons) {
                        const position = Buttons [key];
                        const dist = mp.game.system.vdist(x, y, 0.0, position[0], position[1], 0.0);
                        mp.gui.chat.push("dist - " + dist);
                        if (0.0175 >= dist)
                            curBtn = key;
                    }
                }
            }
        } else {
            IsDowned = false;
            curBtn = "";
        }*/

        if (curBtn !== "") {
            const cursorDirX = (cursorPos[0] - LastCursorPos[0]) / resX;
            const cursorDirY = (cursorPos[1] - LastCursorPos[1]) / resY;

            const screen = mp.game.graphics.world3dToScreen2d(selObj.position.x, selObj.position.y, selObj.position.z);

            const
                mainScreenX = screen.x,
                mainScreenY = screen.y;

            let magnitude = null

            if(curBtn == 'x') {
                if(mode == 'Move') {
                    magnitude = global.GetMagnitudeOffset(selObj.position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 1.0);
                    selObj.position = new mp.Vector3(selObj.position.x+magnitude*MOVE_SENSITIVTY, selObj.position.y, selObj.position.z);
                } else {
                    magnitude = global.GetMagnitudeOffset(selObj.position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0, 1);
                    selObj.rotation = new mp.Vector3(selObj.rotation.x-magnitude*ROT_SENSITIVITY, selObj.rotation.y, selObj.rotation.z);
                }
            } else if(curBtn == 'y') {
                if(mode == 'Move') {
                    magnitude = global.GetMagnitudeOffset(selObj.position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0, 1);
                    selObj.position = new mp.Vector3(selObj.position.x, selObj.position.y+magnitude*MOVE_SENSITIVTY, selObj.position.z);
                } else {
                    magnitude = global.GetMagnitudeOffset(selObj.position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 1);
                    selObj.rotation = new mp.Vector3(selObj.rotation.x, selObj.rotation.y+magnitude*ROT_SENSITIVITY, selObj.rotation.z);
                }
            } else if(curBtn == 'z') {
                magnitude = global.GetMagnitudeOffset(selObj.position, cursorDirX, cursorDirY, mainScreenX, mainScreenY, 0, 0, 1);
                if(mode == 'Move') {
                    selObj.position = new mp.Vector3(selObj.position.x, selObj.position.y, selObj.position.z+magnitude*MOVE_SENSITIVTY);
                } else {
                    selObj.rotation = new mp.Vector3(selObj.rotation.x, selObj.rotation.y, selObj.rotation.z+cursorDirX*ROT_SENSITIVITY*0.2); //Here direction can be determined by just x axis of mouse, hence the *0.2
                }
            }
        }

        LastCursorPos = cursorPos;
    }
});

gm.events.add('click', (x, y, upOrDown, leftOrRight, relativeX, relativeY, worldPosition, hitEntity) => {
    if(!selObj) return;
    
    let mouseRel = {x: x/activeResolution.x, y: y/activeResolution.y};

    if (upOrDown == 'up') {
        curBtn = '';
    } else if (upOrDown == 'down') {
        if(xbox != undefined && mouseRel.x >= Buttons["x"][0]-0.01 && mouseRel.x <= Buttons["x"][0]+0.009 && mouseRel.y >= Buttons["x"][1]-0.015 && mouseRel.y <= Buttons["x"][1]+0.009) {
            curBtn = 'x';
        } else if(ybox != undefined && mouseRel.x >= Buttons["y"][0]-0.01 && mouseRel.x <= Buttons["y"][0]+0.009 && mouseRel.y >= Buttons["y"][1]-0.015 && mouseRel.y <= Buttons["y"][1]+0.009) {
            curBtn = 'y';
        } else if(zbox != undefined && mouseRel.x >= Buttons["z"][0]-0.01 && mouseRel.x <= Buttons["z"][0]+0.009 && mouseRel.y >= zbox.y-0.015 && mouseRel.y <= zbox.y+0.009) {
            curBtn = 'z';
        }
    }
});

function switchMode() {
    mode = (mode == 'Move' ? 'Rotation' : 'Move');
}

function groundObject() {
    selObj.placeOnGroundProperly();
    let pos = selObj.getCoords(true);
    let rot = selObj.getRotation(2);
    selObj.position = new mp.Vector3(pos.x, pos.y, pos.z);
    selObj.rotation = new mp.Vector3(rot.x, rot.y, rot.z); //FIX BUG WHERE POSITION PROPERTY != GAME POSITION
}

function cancel() {
    selObj.position = oldPos;
    selObj.rotation = oldRot;
    selObj.setCollision(true, true);
    selObj = null;
    mp.gui.cursor.show(false, false);
}

function saveChanges() {
    let pos = selObj.getCoords(true);
    let rot = selObj.getRotation(2);
    mp.events.call('objecteditor:finish', selObj.id, JSON.stringify(pos), JSON.stringify(rot));
    selObj.setCollision(true, true);
    selObj = null;
    mp.gui.cursor.show(false, false);
}


gm.events.add('objecteditor', (model) => {
    let obj = mp.objects.new(mp.game.joaat(model), new mp.Vector3(global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z));
    mp.events.call('objecteditor:start', obj.id);
})