const luckywheel = {
    object: {
        name: 'vw_prop_vw_luckywheel_02a',
        pos: new mp.Vector3(1111.052, 229.8579, -49.133),
        model: undefined,
        isSpinning: false,
        lastClickTime: 0,
        animations: [
            'Enter_to_ArmRaisedIDLE',
            'ArmRaisedIDLE_to_SpinningIDLE_High',
            'SpinningIDLE_High',
            'Win_Big'
        ],
        getDictionary() {
            return mp.players.local.getModel() == 1885233650 ? 'ANIM_CASINO_A@AMB@CASINO@GAMES@LUCKY7WHEEL@MALE' : 'ANIM_CASINO_A@AMB@CASINO@GAMES@LUCKY7WHEEL@FEMALE';
        },
        async spin(pos, isOwner) {
            if (this.isSpinning) return;
            this.isSpinning = true;
            let spins = 320,
                maxSpeed = 2.25;
            const speed = maxSpeed / (spins * 2 + (pos + this.model.getRotation(1).y / 18) * 16 + 1);
            mp.game.audio.playSoundFromCoord(1, 'Spin_Start', this.pos.x, this.pos.y, this.pos.z, 'dlc_vw_casino_lucky_wheel_sounds', true, 0, false);
            while (true) {
                if (spins <= 0) {
                    maxSpeed -= speed;
                    this.model.setRotation(0, this.model.getRotation(1).y - maxSpeed, 0, 2, true);
                    if (maxSpeed <= 0) {
                        this.model.setRotation(0, Math.round(this.model.getRotation(1).y), 0, 2, true);
                        mp.game.audio.playSoundFromCoord(1, 'Win', this.pos.x, this.pos.y, this.pos.z, "dlc_vw_casino_lucky_wheel_sounds", true, 0, false);
                        this.isSpinning = false;
                        if (isOwner) {
                            mp.events.callRemote('luckywheel.finishspin');
                            mp.players.local.taskPlayAnim(this.getDictionary(), this.animations[3], 4, -1000, -1, 1048576, 0, false, true, false);
                            while (true) {
                                if (mp.players.local.isPlayingAnim(this.getDictionary(), this.animations[3], 3) && mp.players.local.getAnimCurrentTime(this.getDictionary(), this.animations[3]) > 0.75) {
                                    mp.players.local.clearTasks();
                                    break;
                                }
                                await mp.game.waitAsync(0);
                            }
                        }
                        break;
                    }
                } else {
                    spins--;
                    this.model.setRotation(0, this.model.getRotation(1).y - maxSpeed, 0, 2, true);
                }
                await mp.game.waitAsync(5);
            }
        }
    },
    interaction: {
        pos: new mp.Vector3(1110.8710, 228.8737, -49.6358),
        radius: 3,
        sendNotify(text) {
            mp.game.ui.setTextComponentFormat('STRING');
            mp.game.ui.addTextComponentSubstringWebsite(text);
            mp.game.ui.displayHelpTextFromStringLabel(0, true, true, 1000);
        },
        clearNotify() {
            mp.game.ui.clearHelp(true);
        },
        isNear: false,
        button: 0x45 // 'E'
    },
    onClick() {
        if (luckywheel.object.isSpinning) {
            mp.events.call('notify', 2, 9, 'Колесо удачи уже крутится!', 3000);
            return;
        }

        if (new Date().getTime() - this.lastClickTime < 1000) return;
        mp.events.callRemote('luckywheel.cometoluckywheel');
        this.lastClickTime = new Date().getTime();
    },
    async comeToLuckyWheel(pos) {
        const dict = this.object.getDictionary();
        global.requestAnimDict(dict);
        while (!mp.game.streaming.hasAnimDictLoaded(dict)) {
            await mp.game.waitAsync(0);
        }
        if (mp.players.local.getScriptTaskStatus(2106541073) != 1 && mp.players.local.getScriptTaskStatus(2106541073) != 0) {
            const offset = mp.game.ped.getAnimInitialOffsetPosition(dict, this.object.animations[0], 1111.052, 229.8492, -50.6409, 0, 0, 0, 0, 2);
            mp.players.local.taskGoStraightToCoord(offset.x, offset.y, offset.z, 1, 8000, 317, 0.001);
            while (!(mp.players.local.getScriptTaskStatus(2106541073) == 7 || mp.players.local.isAtCoord(offset.x, offset.y, offset.z, 0.1, 0.0, 0.0, false, true, 0))) {
                await mp.game.waitAsync(0);
            }
            mp.players.local.taskPlayAnim(dict, this.object.animations[0], 4, -1000, -1, 1048576, 0, false, true, false);
            let isGoing;
            while (true) {
                if (mp.players.local.isPlayingAnim(dict, this.object.animations[0], 3) && mp.players.local.getAnimCurrentTime(dict, this.object.animations[0]) > 0.97) {
                    mp.players.local.taskPlayAnim(dict, this.object.animations[1], 4, -1000, -1, 1048576, 0, false, true, false);
                }
                if (mp.players.local.isPlayingAnim(dict, this.object.animations[1], 3)) {
                    if (!isGoing && mp.players.local.getAnimCurrentTime(dict, this.object.animations[1]) > 0.04) {
                        isGoing = true;
                        this.object.spin(pos, true);
                        mp.events.callRemote('luckywheel.spin');
                    }
                    if (mp.players.local.getAnimCurrentTime(dict, this.object.animations[1]) > 0.8) {
                        mp.players.local.taskPlayAnim(dict, this.object.animations[2], 8.0, 1.0, -1, 1, 1.0, false, false, false);
                        break;
                    }
                }
                await mp.game.waitAsync(0);
            }
        }
    },
    init() {
        this.object.model = mp.objects.new(mp.game.joaat(this.object.name), this.object.pos);
    }
};

luckywheel.init();

mp.events.add({
    'render': () => {

        const data = luckywheel.interaction;
        if (mp.game.gameplay.getDistanceBetweenCoords(mp.players.local.position.x, mp.players.local.position.y, mp.players.local.position.z, data.pos.x, data.pos.y, data.pos.z, true) < data.radius && !(
                !loggedin || chatActive || global.menuOpened || cuffed || localplayer.getVariable('InDeath')
            )) {
            if (!data.isNear) {
                data.isNear = true;
                mp.keys.bind(luckywheel.interaction.button, false, luckywheel.onClick);

                // Будем показывать что можно взаимодействовать пока рядом
                mp.events.call('hud.oEnter');
            }
        } else if (data.isNear) {
            data.isNear = false;
            mp.keys.unbind(luckywheel.interaction.button, false, luckywheel.onClick);

            // Скроем подсказку взаимодействия
            mp.events.call('hud.cEnter');
        }
    },
    'luckywheel.cometoluckywheel': (pos) => {
        luckywheel.comeToLuckyWheel(pos);
    },
    'luckywheel.spin': (pos) => {
        luckywheel.object.spin(pos);
    }
});

require('./events.js');