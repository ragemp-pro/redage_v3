
gm.events.add('luckyWheel', (entity) => {
    let wheelPos = new mp.Vector3(1110.2651, 228.62857, -50.7558);
    
    mp.game.invoke("0x960C9FF8F616E41C", "Press ~INPUT_PICKUP~ to start shopping", true);
    entity.taskGoStraightToCoord(wheelPos.x, wheelPos.y, wheelPos.z, 1.0,  -1,  312.2,  0.0);

    setTimeout(() => {
        entity.setRotation(0.0, 0.0, 2.464141, 1, true);
        entity.taskPlayAnim( "anim_casino_a@amb@casino@games@lucky7wheel@male", "enter_right_to_baseidle", 8.0, -8.0, -1, 0, 0, false, false, false);
    }, 1000);

    setTimeout(() => {

        entity.taskPlayAnim( "anim_casino_a@amb@casino@games@lucky7wheel@male", "enter_to_armraisedidle", 8.0, -8.0, -1, 0, 0, false, false, false);
        if(entity == mp.players.local){

            setTimeout(() => {
                mp.events.callRemote('startRoll');
                entity.freezePosition(true);
                rouletteCamera = mp.cameras.new('default', new mp.Vector3(1111.015, 227.7846, -50.755825 +2.5), new mp.Vector3(0,0,0), 40);
                rouletteCamera.setRot(0.0, 0, 0, 2);
                rouletteCamera.setActive(true);
                //localplayer.freezePosition(true);
                mp.game.cam.renderScriptCams(true, true, 1500, true, false);
            }, 1000);
        }
    }, 2000);

    setTimeout(() => {

        entity.taskPlayAnim( "anim_casino_a@amb@casino@games@lucky7wheel@male", "armraisedidle_to_spinningidle_high", 8.0, -8.0, -1, 0, 0, false, false, false);
    }, 3000);
});
var count = 0;
gm.events.add('delWheelCam', () => {
    rouletteCamera.destroy(true);
    rouletteCamera = null;
    mp.game.cam.renderScriptCams(false, true, 1000, true, false);
    localplayer.freezePosition(false);
});