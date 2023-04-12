const player = global.localplayer;
let _lambo = null
let _isShowCar = false
let _wheelPos = new mp.Vector3(1109.76, 227.89, -49.64);

let Keys = {
	"ESC": 322, "BACKSPACE": 177, "E": 38, "ENTER": 18,	"LEFT": 174, "RIGHT": 175, "TOP": 27, "DOWN": 173
}
let _isRolling = false;

let coordsWheel = new mp.Vector3(1111.052, 229.8579, -49.133);

let model = mp.game.joaat('vw_prop_vw_luckywheel_02a')
const _wheel = mp.objects.new(model,coordsWheel)
mp.game.streaming.setModelAsNoLongerNeeded(model);
_wheel.setHeading(0);

function getDistAnim() {
    return mp.game.joaat("MP_M_Freemode_01") === player.model ? 'anim_casino_a@amb@casino@games@lucky7wheel@male' :'anim_casino_a@amb@casino@games@lucky7wheel@female'
}

async function doRoll(){
    if(!_isRolling){
        _isRolling = true;
        let lib = getDistAnim();
        let anim = 'enter_right_to_baseidle';
        loadtAnimDict(lib).then(async function(){
            let pos = mp.game.ped.getAnimInitialOffsetPosition(lib, anim, 1111.052, 229.8492, -50.6409, 0, 0, 0, 0, 2)
            let rotation =  mp.game.ped.getAnimInitialOffsetRotation(lib, anim, player.position, 0, 0, 0, 0, 2)
            player.taskGoStraightToCoord(pos.x,  pos.y,  pos.z,  1.0,  5000,  rotation,  0.0)
            let _isMoved = false
            while (!_isMoved){
                let coords = player.position;
                if(coords.x >= (pos.x - 0.01) && coords.x <= (pos.x + 0.01) && coords.y >= (pos.y - 0.01) && coords.y <= (pos.y + 0.01) ){
                    _isMoved = true
                }
                await global.wait(1);
            }
            player.taskPlayAnim(lib, anim, 8.0, -8.0, -1, 0, 0, false, false, false)
            while(player.isPlayingAnim(lib, anim, 3)){
                mp.game.controls.disableAllControlActions(0)
                await global.wait(1);
            }
            player.taskPlayAnim(lib, 'enter_to_armraisedidle', 8.0, -8.0, -1, 0, 0, false, false, false)
            while(player.isPlayingAnim(lib, 'enter_to_armraisedidle', 3)){
                mp.game.controls.disableAllControlActions(0)
                await global.wait(1);
            }
            mp.events.callRemote("CASINO_LUCKYWHEEL:getLucky")
            player.taskPlayAnim(lib, 'armraisedidle_to_spinningidle_high', 8.0, -8.0, -1, 0, 0, false, false, false)
        })
    }
}


gm.events.add({
    // Menu Controls
    'render': ()=>{
		try 
		{
            if (!global.loggedin) return;
			let coords = player.position;
			if(mp.Vector3.Distance(coords, _wheelPos) < 1.5 && !_isRolling)  {
				alert(translateText("Нажмите E, чтобы испытать свою удачу с вращением 1 раз 100,000$"))
				if(mp.game.controls.isControlJustReleased(0, Keys['E'])){
					doRoll()
				}
			}
		}
		catch (e) 
		{
			if(new Date().getTime() - global.trycatchtime["luckyWheel/index"] < 60000) return;
			global.trycatchtime["luckyWheel/index"] = new Date().getTime();
			mp.events.callRemote("client_trycatch", "luckyWheel/index", "render", e.toString());
		}
    },
    "CASINO_LUCKYWHEEL:rollFinished": function(){
        _isRolling = false
    },
    "CASINO_LUCKYWHEEL:doRoll": async function(_priceIndex){
        try{
            _isRolling = true;

            _wheel.setHeading(-30.0);
            _wheel.setRotation(0.0, 0.0, 0.0, 1, true)
            let speedIntCnt = 1;
            let rollspeed = 1.0;
            let _winAngle = (_priceIndex - 1) * 18
            let _rollAngle = _winAngle + (360 * 8)
            let _midLength = (_rollAngle / 2)
            let intCnt = 0
            while(speedIntCnt > 0){
                let retval = _wheel.getRotation(1)
                if(_rollAngle > _midLength ){

                    speedIntCnt = speedIntCnt + 1
                }
                else{
                    speedIntCnt = speedIntCnt - 1
                    if(speedIntCnt < 0) {
                        speedIntCnt = 0
                        
                    }
                }
                intCnt = intCnt + 1
                rollspeed = speedIntCnt / 10
                let _y = retval.y - rollspeed
                _rollAngle -= rollspeed;
                _wheel.setRotation( 0.0, _y, 0.0, 1, true)
                await global.wait(0);
                playSound(_priceIndex)
            }
            let lib = getDistAnim();
            let animSpining = getAnimSpining(_priceIndex);
            player.taskPlayAnim(lib, animSpining, 8.0, -8.0, -1, 0, 0, false, false, false)
            while(player.isPlayingAnim(lib, animSpining, 3)){
                mp.game.controls.disableAllControlActions(0)
                await global.wait(1);
            }
            let animWin = getAnimWin(_priceIndex);
            player.taskPlayAnim(lib, animWin, 8.0, -8.0, -1, 0, 0, false, false, false)
            while(player.isPlayingAnim(lib, animWin, 3)){
                mp.game.controls.disableAllControlActions(0)
                await global.wait(0);
            }

        }catch(e){
            chat(e)
        }
    }
})

function getAnimSpining(_priceIndex) {
    // 'SpinningIDLE_Medium','SpinningIDLE_Low'
    return 'SpinningIDLE_High';
}

function getAnimWin(_priceIndex) {
    // 'Win_Big','Win'
    return 'Win_Huge';
}

// todo в утилы
function loadtAnimDict(dict) {
    return new Promise((resolve, reject) => {
        if(!mp.game.streaming.doesAnimDictExist(dict))return resolve('error does anim')
        mp.game.streaming.requestAnimDict(dict);
        const timer = setInterval(() => {
            if(mp.game.streaming.hasAnimDictLoaded(dict)) {
                clearInterval(timer);
                resolve();
            }
        }, 100);
    });
}

function getSound(id) {
    let sound;
    switch (id)
    {
        case 1:
        case 17:
        case 5:
        case 9:
        case 13:
            sound = "Win_RP";
            break;
        
        case 2:
        case 6:
        case 14:
        case 4:
            sound = "Win_Cash";
            break;
        
        case 19:
            sound = "Win_Cash";
            break;
        
        case 3:
        case 7:
        case 10:
            sound = "Win_Chips";
            break;
        
        case 15:
            sound = "Win_Chips";
            break;
        
        case 11:
            sound = "Win_Mystery";
            break;
        
        case 18:
            sound = "Win_Car";
            break;
        
        default:
            sound = "Win_Clothes";
            break;
    }
    return sound;
}

const playSound =  function (idWin) {
    let soundId = mp.game.invoke('0x430386FE9BF80B45')//getSoundId
    let sound = getSound(idWin);
    mp.game.audio.stopSound(soundId);
    mp.game.audio.releaseSoundId(soundId)
    mp.game.audio.playSoundFromCoord(1, sound, coordsWheel.x, coordsWheel.y, coordsWheel.z, "dlc_vw_casino_lucky_wheel_sounds", true, 0, false);
}

const animSpiningWheel = (id,max)=>{
    let lib = getDistAnim();
    _wheel.playAnim(getAnimWheel(id,max),lib,1000, false, true, false, 0, 2)
} 
