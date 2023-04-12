import { faceAnimList , animationList, timeCycle, clampMin } from './data'

let isPhoneThisFrame = false,
    isMode = false,
    isDofEnabled = false,
    dofEnabled = false,
    playerHealth = 100,
    xOffset = 0.0,
    yOffset = 1.0,
    roll = 0.0,
    headY = 0.0,
    headRoll = 0.0,
    headHeight = 0.0;

let selectTimeCycle = 0;
let isFreeMode = false;

let selectAnim = 0,
    isTaskAnim = false,
    useDict = "",
    useAnim = "";

let selectFaceAnim = 0,
    countPhone = 0;

const updateInterfaceState = () => {

    let popupData = {
        timeCycle: translateText("Фильтр ({0})", selectTimeCycle + 1),
        animName: animationList[selectAnim].name,
        emotion: translateText("Эмоция ({0})", faceAnimList[selectFaceAnim].name),
        isPhoneThisFrame: isPhoneThisFrame,
        isFreeMode: isFreeMode,
        isDofEnabled: isDofEnabled
    }

    mp.gui.emmit(`window.events.callEvent("camera.update", '${JSON.stringify(popupData)}')`);
}

global.phoneCameraOpen = false;

gm.events.add('camera.open', (_isDofEnabled = true) => {
    isPhoneThisFrame = false;
    isFreeMode = false;
    isMode = false;
    isDofEnabled = _isDofEnabled;
    dofEnabled = false;
    playerHealth = global.localplayer.getHealth();
    xOffset = 0.0;
    yOffset = 1.0;
    roll = 0.0;
    headY = 0.0;
    headRoll = 0.0;
    headHeight = 0.0;
    selectTimeCycle = 0;

    global.phoneCameraOpen = true;
    global.menuClose (true);
    global.menuOpened = true;

    updateInterfaceState();
    CreateMobile();

    mp.events.add("render", onRender);

});

const onDestroy = (_isScreen) => {
    DestroyMobile ();
    mp.gui.emmit(`window.events.callEvent("camera.close", ${_isScreen})`);
    mp.events.callRemote("server.phone.anim", 0);

    mp.events.remove("render", onRender);

    if (global.isPhoneOpen)
        global.menuOpen (true);
    else
        global.menuOpen ();

    global.phoneCameraOpen = false;
}
gm.events.add("camera.close", onDestroy);

let screenshotsBrowser = null;
let nameImg = null;

const onScreen = async () => {
    mp.gui.emmit(`window.router.opacity(0)`);
    mp.game.ui.displayRadar(false);

    await mp.game.waitAsync(25);

    nameImg = Date.now() + ".jpg";
    mp.gui.takeScreenshot(nameImg, 0, 100, 0);

    screenshotsBrowser = mp.browsers.new(`screenshots://${nameImg}`);

    onDestroy (true);

    mp.game.audio.playSoundFrontend(-1, "Camera_Shoot", "Phone_SoundSet_Michael", true);
    return;
}
gm.events.add("camera.screen", onScreen);

gm.events.add('browserDomReady', (browser) => {
    if (screenshotsBrowser === browser) {
        mp.game.ui.displayRadar(true);
        mp.gui.emmit(`window.router.opacity(1)`);
        mp.gui.emmit(`window.screenshot_getbase64 ('http://screenshots/${nameImg}')`);
        nameImg = null;
        screenshotsBrowser.destroy();
        screenshotsBrowser = null;
    }
});

const onFreeMode = () => {
    if (isPhoneThisFrame)
        return;

    isFreeMode = !isFreeMode;
    updateInterfaceState();

    if (isFreeMode)
        DestroyMobile ();
    else
        CreateMobile();
};
gm.events.add("camera.freemode", onFreeMode);

const CreateMobile = () => {
    mp.game.ui.displayRadar(false);

    mp.game.mobile.createMobilePhone(0);
    mp.game.mobile.setMobilePhoneScale(0);

    global.localplayer.setConfigFlag(242, true);
    global.localplayer.setConfigFlag(243, true);
    global.localplayer.setConfigFlag(244, false);
    //
    mp.game.mobile.cellCamActivate(true, true);
    nativeInvoke ("_CELL_CAM_DISABLE_THIS_FRAME", isPhoneThisFrame);
    nativeInvoke ("0xA2CCBE62CD4C91A4", nativeInvoke ("_SET_MOBILE_PHONE_UNK", dofEnabled));
    mp.events.callRemote("server.phone.anim", 1);
}

const DestroyMobile = () => {
    mp.game.ui.displayRadar(true);
    nativeInvoke ("DESTROY_MOBILE_PHONE");

    mp.game.mobile.cellCamActivate(false, false);

    global.localplayer.setConfigFlag(242, false);
    global.localplayer.setConfigFlag(243, false);
    global.localplayer.setConfigFlag(244, true);
    nativeInvoke ("CLEAR_TIMECYCLE_MODIFIER");
    global.localplayer.clearFacialIdleAnimOverride();

    if (global.isPhoneOpen) {
        mp.game.mobile.createMobilePhone(0);
        mp.game.mobile.setMobilePhoneScale(0);
    }
}

DestroyMobile ();

const changeView = async () => {
    //if (mp.keys.isUp(40)/* && global.actionAntiFlood("__smartphoneScaleVisibleChange", 500)*/) {
    if (!isMode && mp.game.controls.isControlJustPressed(3, 172)) {
        isPhoneThisFrame = !isPhoneThisFrame;
        isMode = true;
        mp.game.cam.doScreenFadeOut(500);
        await mp.game.waitAsync(500);

        //Natives.CellFrontCamActivate (isPhoneThisFrame)
        nativeInvoke ("_CELL_CAM_DISABLE_THIS_FRAME", isPhoneThisFrame);
        await mp.game.waitAsync(350);
        updateInterfaceState();

        mp.game.cam.doScreenFadeIn(500);
        await mp.game.waitAsync(550);
        isMode = false;
    }
    //}
    return isPhoneThisFrame;
}

const initPhone = () => {
    const
        mouseX = mp.game.controls.getDisabledControlNormal(0, 1) / 20,
        mouseY = -mp.game.controls.getDisabledControlNormal(0, 2) / 20;

    if (mp.game.controls.isDisabledControlPressed(0, 69)) {
        mp.game.controls.disableControlAction(0, 1, true);
        mp.game.controls.disableControlAction(0, 2, true);

        xOffset = clampMin(xOffset + mouseX, 0.0, 1.0);
        yOffset = clampMin(yOffset + mouseY, 0.0, 2.0);
        roll = clampMin(roll + mp.game.controls.getDisabledControlNormal(0, 30) / 12, -1.0, 1.0);

    } else if (mp.game.controls.isDisabledControlPressed(0, 68)) {
        mp.game.controls.disableControlAction(0, 1, true);
        mp.game.controls.disableControlAction(0, 2, true);

        headY = clampMin(headY + mouseX, -1.0, 1.0);
        headRoll = clampMin(headRoll + mp.game.controls.getDisabledControlNormal(0, 30) / 12, -1.0, 1.0);
        headHeight = clampMin(headHeight + mouseY, -1.0, 1.0);
    }

    nativeInvoke("0x1B0B4AEED5B9B41C", xOffset);//CellCamSetHorizontalOffset
    nativeInvoke("0x3117D84EFA60F77B", yOffset);//CellCamSetVerticalOffset
    nativeInvoke("0x15E69E2802C24B8D", roll);//CellCamSetRoll

    nativeInvoke("0xD6ADE981781FCA09", headY);//CellCamSetHeadY
    nativeInvoke("0xF1E22DC13F5EEBAD", headRoll);//CellCamSetHeadRoll
    nativeInvoke("0x466DA42C89865553", headHeight);//CellCamSetHeadHeight

    if (isDofEnabled && mp.game.controls.isControlJustPressed(3, 0)) {
        dofEnabled = !dofEnabled;
        nativeInvoke ("0xA2CCBE62CD4C91A4", nativeInvoke ("_SET_MOBILE_PHONE_UNK", dofEnabled));
    }
    return 0 == ++countPhone % 15;
}

const onRender = async () => {
    mp.game.controls.disableControlAction(0, 44, true);
    mp.game.controls.disableControlAction(0, 156, true);
    mp.game.controls.disableControlAction(0, 199, true);
    mp.game.controls.disableControlAction(0, 200, true);
    mp.game.controls.disableControlAction(2, 156, true);
    mp.game.controls.disableControlAction(2, 199, true);
    mp.game.controls.disableControlAction(2, 200, true);

    const health = global.localplayer.getHealth();

    if (
        (health < playerHealth && 5 < playerHealth - health) ||
        null != global.localplayer.vehicle ||
        global.isDeath ||
        global.localplayer.isFalling() ||
        global.localplayer.isJumping() ||
        global.localplayer.isSwimming() ||
        //0 !== global.getPlayerCurrentWeaponData().weapon ||
        global.cuffed
    )
        return void onDestroy();

    playerHealth = health;
    if (!isFreeMode) {

        if (mp.game.controls.isControlJustPressed(0, 87) || mp.game.controls.isControlJustPressed(0, 88)) {
            const number = mp.game.controls.isControlJustPressed(0, 87) ? 1 : -1;
            selectTimeCycle += number;

            if (selectTimeCycle >= timeCycle.length)
                selectTimeCycle = 0;
            else if (0 > selectTimeCycle)
                selectTimeCycle = timeCycle.length - 1;

            mp.game.graphics.setTimecycleModifierStrength(1);
            mp.game.graphics.setTimecycleModifier(timeCycle[selectTimeCycle]);
            updateInterfaceState();
        }

        if (isDofEnabled) {
            if (await changeView()) {
                if (initPhone()) {
                    const {x: posX, y: posY, z: posZ} = global.localplayer.position,
                        cameraCoord = global.cameraManager.gameplayCam().getCoord();

                    mp.players.forEachInStreamRange((player) => {
                        if (player === global.localplayer || (5 > mp.game.system.vdist(posX, posY, posZ, player.position.x, player.position.y, player.position.z))) {
                            nativeInvoke("TASK_LOOK_AT_COORD", player.handle, cameraCoord.x, cameraCoord.y, cameraCoord.z, 2500, 2048, 3);
                        }
                    });
                }
            }

            if (mp.game.controls.isControlJustPressed(0, 89) || mp.game.controls.isControlJustPressed(0, 90)) {
                const number = mp.game.controls.isControlJustPressed(0, 89) ? -1 : 1;
                selectAnim += number;

                if (selectAnim >= animationList.length)
                    selectAnim = 0;
                else if (0 > selectAnim)
                    selectAnim = animationList.length - 1;

                updateInterfaceState();
            }
            if (mp.game.controls.isControlJustPressed(0, 51) || mp.game.controls.isControlJustPressed(0, 52)) {
                const number = mp.game.controls.isControlJustPressed(0, 51) ? -1 : 1;

                selectFaceAnim += number;
                if (selectFaceAnim >= faceAnimList.length)
                    selectFaceAnim = 0;
                else if (0 > selectFaceAnim)
                    selectFaceAnim = faceAnimList.length - 1;

                if (null == faceAnimList[selectFaceAnim].animName)
                    global.localplayer.clearFacialIdleAnimOverride()
                else
                    nativeInvoke("SET_FACIAL_IDLE_ANIM_OVERRIDE", global.localplayer.handle, faceAnimList[selectFaceAnim].animName, 0);

                updateInterfaceState();
            }

            const isBtnMoveUp = mp.game.controls.isControlPressed(0, 61);

            if (isBtnMoveUp && !isTaskAnim) {
                const anim = animationList[selectAnim];
                if (anim) {
                    isTaskAnim = true;

                    await global.requestAnimDict(anim.dict);

                    if (anim.anim) {

                        global.localplayer.taskPlayAnim(anim.dict, anim.anim, 4, 4, -1, 128, -1, false, false, false);

                        useDict = anim.dict;
                        useAnim = anim.anim;
                    } else {
                        global.localplayer.taskPlayAnim(anim.dict, "enter", 4, 4, -1, 128, -1, false, false, false);

                        await mp.game.waitAsync(1000 * mp.game.entity.getEntityAnimDuration(anim.dict, "enter"));

                        global.localplayer.taskPlayAnim(anim.dict, "idle_a", 8, 4, -1, 129, -1, false, false, false);

                        useDict = anim.dict;
                        useAnim = "";
                    }
                }
            } else if (isTaskAnim && !isBtnMoveUp) {
                isTaskAnim = false;

                if (useDict && !useAnim) {
                    global.localplayer.taskPlayAnim(useDict, "exit", 4, 4, -1, 128, -1, false, false, false);
                    await mp.game.waitAsync(1000 * mp.game.entity.getEntityAnimDuration(useDict, "exit"));
                    global.localplayer.taskPlayAnim("", "", 4, 4, -1, 128, -1, false, false, false);
                } else {
                    global.localplayer.stopAnimTask(useDict, useAnim, 3);
                    global.localplayer.taskPlayAnim("", "", 4, 4, -1, 128, -1, false, false, false);
                }
            }
        }
    }
}