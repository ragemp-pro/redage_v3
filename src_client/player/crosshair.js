const crosshairParams = {	
	width: [
        0.002,
        0.0025,
        0.003,
        0.0035,
        0.004,
        0.0045,
        0.005,
        0.0055,
        0.006,
        0.0065,
        0.007,
        0.0075,
        0.008,
        0.0085,
        0.009,
        0.0095,
        0.010, 
        0.0105,
        0.011,
        0.0115,
        0.012,
        0.0125,
        0.013,
        0.0135,
        0.014,
        0.0145,
        0.015,
        0.0155, 
        0.016,
        0.0165,
        0.017,
        0.0175,
        0.018,
        0.0185,
        0.019,
        0.0195,
        0.02
    ],
	gap: [
        0.0,
        0.0005,
        0.001,
        0.0015,
        0.002,
        0.0025,
        0.003,
        0.0035,
        0.004,
        0.0045,
        0.005,
        0.0055,
        0.006,
        0.0065,
        0.007,
        0.0075,
        0.008,
        0.0085,
        0.009,
        0.0095,
        0.01
    ],
    thickness: [
        0.002,
        0.004,
        0.006,
        0.008,
        0.01,
        0.012,
        0.014,
        0.016,
        0.018,
        0.02
    ],
	opacity: [
        25,
        50,
        75,
        100,
        125,
        150,
        175,
        200,
        225,
        255
    ],
}


global.crosshairParameters = {
    toggled: false,
    width: 2,
    gap: 2,
    dot: true,
    thickness: 0,
    color: [ 255, 255, 255 ],
    opacity: 9,
	checkp: true,
}

gm.events.add("render", () => {
    if (!global.loggedin) return;
    else if (global.menuCheck ()) return;
    if (global.crosshairParameters.toggled && !global.IsWeaponSniper())
    {
        mp.game.ui.hideHudComponentThisFrame(14);
        if(mp.game.player.isFreeAiming() && global.weaponData.weapon != -1569615261)
        {
            const ratio = mp.game.graphics.getScreenAspectRatio(true);
            const graphics = mp.game.graphics;
            const thickness = crosshairParams.thickness[global.crosshairParameters.thickness];
            const width		= crosshairParams.width[global.crosshairParameters.width];
            const gap		= crosshairParams.gap[global.crosshairParameters.gap];

            const colorR = global.crosshairParameters.color[0];
            const colorG = global.crosshairParameters.color[1];
            const colorB = global.crosshairParameters.color[2];
            const colorOpacity	= crosshairParams.opacity[global.crosshairParameters.opacity];

            graphics.drawRect(0.5 - gap - width / 2, 0.5, width, thickness, colorR, colorG, colorB, colorOpacity);
            graphics.drawRect(0.5 + gap + width / 2, 0.5, width, thickness, colorR, colorG, colorB, colorOpacity);
            graphics.drawRect(0.5, 0.5 - (gap*ratio) - (width*ratio) / 2, thickness / ratio, width * ratio, colorR, colorG, colorB, colorOpacity);
            graphics.drawRect(0.5, 0.5 + (gap*ratio) + (width*ratio) / 2, thickness / ratio, width * ratio, colorR, colorG, colorB, colorOpacity);

            if (global.crosshairParameters.dot) graphics.drawRect(0.5, 0.5, (thickness/2), (thickness/2) * ratio, colorR, colorG, colorB, colorOpacity);

            if(global.crosshairParameters.checkp)
            {
                const aimingat = mp.game.player.getEntityIsFreeAimingAt();
                if(aimingat !== undefined && aimingat != null && aimingat.type == "player") graphics.drawRect(0.5, 0.517 + (gap*ratio) + (width*ratio) / 2, width * 2, thickness, colorR, colorG, colorB, colorOpacity);
            }
        }
    }
});