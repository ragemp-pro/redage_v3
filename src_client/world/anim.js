let lastUpdateCarryng = null,
AnimData = {
    1: {
        player: {
            dict: "anim@heists@box_carry@",
            name: "idle",
            flag: 50
        },
        target: {
            dict: "amb@code_human_in_car_idles@generic@ps@base",
            name: "base",
            flag: 33,
            bone: 9816,
            positionOffset: new mp.Vector3(0.015, 0.38, 0.11),
            rotationOffset: new mp.Vector3(0, 0, 0x5a)
        }
    },
    2: {
        player: {
            dict: "anim@arena@celeb@flat@paired@no_props@",
            name: "piggyback_c_player_a",
            flag: 49
        },
        target: {
            dict: "anim@arena@celeb@flat@paired@no_props@",
            name: "piggyback_c_player_b",
            flag: 33,
            bone: 0,
            positionOffset: new mp.Vector3(0, -0.07, 0.45),
            rotationOffset: new mp.Vector3(0, 0, 0)
        }
    },
    3: {
        player: {
            dict: "missfinale_c2mcs_1",
            name: "fin_c2_mcs_1_camman",
            flag: 49
        },
        target: {
            dict: 'nm',
            name: "firemans_carry",
            flag: 33,
            bone: 0,
            positionOffset: new mp.Vector3(0.27, 0.15, 0.63),
            rotationOffset: new mp.Vector3(0, 0, 0)
        }
    },
    4: {
        player: {
            dict: "anim@gangops@hostage@",
            name: "perp_idle",
            flag: 49
        },
        target: {
            dict: "anim@gangops@hostage@",
            name: "victim_idle",
            flag: 49,
            bone: 0,
            positionOffset: new mp.Vector3(-0.24, 0.11, 0),
            rotationOffset: new mp.Vector3(0, 0, 0)
        }
    }
};

gm.events.add("clearCarryng", () => {
    if (!global.localplayer.carryngData)
        return;
    mp.events.callRemote("carryng.stop");
    ClearData(global.localplayer);
});

const ClearData = player => {
    try
    {
        const {
            player: _player,
            target: _target
        } = player.carryngData;
        if (_player === global.localplayer || _target === global.localplayer) {            
            mp.gui.emmit(`window.UpdateButtonText('', '');`);
            //mp.api.usingSystemAnim(false);
            if (_target === global.localplayer) {
                if (0 == _player.position.x && 0 == _player.position.y && 0 == _player.position.z) 
                    global.localplayer.setCoordsNoOffset(global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z + 0.35, true, false, false);
                else 
                    global.localplayer.setCoordsNoOffset(_player.position.x, _player.position.y, _player.position.z, true, false, false)
            }
        }

        _player.detach(true, false);

        _player.clearTasksImmediately();

        _target.detach(true, false);

        _target.clearTasksImmediately();

        delete _player.carryngData;
        delete _target.carryngData;        
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "ClearData", e.toString());
    }
}

const UseAnim = async player => {
    try
    {        
        if (!player.carryngData)
            return;    
            
        const {
            player: _player,
            target: _target,
            type: _type
        } = player.carryngData;    
        
        const {
            player: _playerAnimData,
            target: _targetAnimData
        } = AnimData[_type];

        if (_playerAnimData.dict !== _targetAnimData.dict) 
            await Promise.all([
                global.requestAnimDict(_playerAnimData.dict),
                global.requestAnimDict(_targetAnimData.dict)
            ]);
        else 
            await global.requestAnimDict(_playerAnimData.dict);


        if (!_player.isPlayingAnim(_playerAnimData.dict, _playerAnimData.name, 3)) {
            _player.taskPlayAnim(_playerAnimData.dict, _playerAnimData.name, 8, -8, -1, _playerAnimData.flag, 0, false, false, false);
        }
        
        if (!_target.isPlayingAnim(_targetAnimData.dict, _targetAnimData.name, 3)) {
            _target.taskPlayAnim(_targetAnimData.dict, _targetAnimData.name, 8, -8, -1, _targetAnimData.flag, 0, false, false, false);
        }

        if (!_target.isAttachedTo(_player.handle))  {
            if (0 == _player.position.x && 0 == _player.position.y && 0 == _player.position.z)
                return;

            _target.detach(true, false);
            _target.attachTo(_player.handle, _targetAnimData.bone, _targetAnimData.positionOffset.x, _targetAnimData.positionOffset.y, _targetAnimData.positionOffset.z, _targetAnimData.rotationOffset.x, _targetAnimData.rotationOffset.y, _targetAnimData.rotationOffset.z, false, false, false, false, 0, true);
        }
    
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "UseAnim", e.toString());
    }
};

gm.events.add("syncCarryng", (player, data) => {
    try
    {    
        let 
            _carryngData = player.carryngData;
    
        if (_carryngData && !data)
            return void ClearData(player);            
             
        if (_carryngData || !data)
            return;
              
        const {
            id: _id,
            type: _type
        } = data,
        _target = mp.players.atRemoteId(_id);
        _carryngData = {
            player: player,
            target: _target,
            type: _type
        };
    
        _target.carryngData = _carryngData;
        player.carryngData = _carryngData;
    
        if (player === global.localplayer || _target === global.localplayer) {
            mp.gui.emmit(`window.UpdateButtonText('hud__icon-Anim', '${translateText('Чтобы сбросить анимацию, нажмите "Пробел" дважды.')}');`);
            //mp.api.usingSystemAnim(true);
            //if (4 != _type)
            //   mp.inventory.pocketItem();
        }

        lastUpdateCarryng = Date.now() + 2500; 
        UseAnim (player);   
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "syncCarryng", e.toString());
    }
});

setInterval(() => {
    try
    {
        const _carryngData = global.localplayer.carryngData;
        if (!_carryngData)
            return;
        const {
            player: _player,
            target: _target
        } = _carryngData;

        const selectPlayer = _player === global.localplayer ? _target : _player;

        if (!mp.players.exists(selectPlayer))
            return mp.events.callRemote("carryng.stop");
        if (global.vdist2(global.localplayer.position, selectPlayer.position) > 10
            || selectPlayer.dimension != global.localplayer.dimension
            || global.cuffed
            //|| global.isDeath == true
            || global.isDemorgan == true
            || global.attachedtotrunk) {
                mp.events.callRemote("carryng.stop");
                ClearData(global.localplayer);
        }
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "time.everySec", e.toString());
    }
}, 1000);

gm.events.add("playerStreamIn", (entity) => {
    const _carryngData = entity.carryngData;

    if (!_carryngData)
        return;

    const {
        player: _player,
        target: _target
    } = _carryngData;

    if (_player === global.localplayer || _target === global.localplayer) {
        mp.events.callRemote("carryng.stop");
        ClearData(entity);
    };
});

gm.events.add("playerQuit", player => {
    try {
        if (player && player.carryngData)
            ClearData(player);
    } 
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "playerQuit", e.toString());
    }
});

gm.events.add("playerDeath", (player, reason, killer) => {
    try {
        if (player !== global.localplayer) return;
        if (global.localplayer && global.localplayer.carryngData) {
            mp.events.callRemote("carryng.stop");
            ClearData(global.localplayer);
        }  
    } 
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "playerQuit", e.toString());
    }
});

gm.events.add('client.animation.stop', () => { // F8
    try {
        if (global.localplayer.carryngData && !global.isDeath) {
            mp.events.callRemote("carryng.stop");
            ClearData(global.localplayer);
        }   
    } 
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "VK_SPACE", e.toString());
    }
});

const controls = mp.game.controls;

gm.events.add("render", () => {
    try
    {
        const _time = Date.now(),
            _carryngData = global.localplayer.carryngData;

        if (_carryngData) {
            controls.disableControlAction(0, 0x15, true);
            controls.disableControlAction(0, 0x18, true);
            controls.disableControlAction(0, 0x19, true);
            controls.disableControlAction(0, 0x45, true);
            controls.disableControlAction(0, 0x4b, true);
            controls.disableControlAction(0, 0x5c, true);
            controls.disableControlAction(0, 0x72, true);
            controls.disableControlAction(0, 0x8c, true);
            controls.disableControlAction(0, 0x8d, true);
            controls.disableControlAction(0, 0x8e, true);
            controls.disableControlAction(0, 0x101, true);
            controls.disableControlAction(0, 0x107, true);
            controls.disableControlAction(0, 0x108, true);
            //mp.api.displayHelpString(translateText("Нажмите ~INPUT_JUMP~ чтобы отменить ношение на руках"), false, false, -1);
            if (_carryngData.player === global.localplayer &&
             (global.localplayer.isRagdoll() || global.localplayer.isInWater())) {
                ClearData(global.localplayer);
                mp.events.callRemote("carryng.stop");
            }
        }
        
        if (!lastUpdateCarryng || _time >= lastUpdateCarryng) {
            lastUpdateCarryng = _time + 2500;
            mp.players.forEachInStreamRange(player => {
                UseAnim(player);
            });
            UseAnim(global.localplayer)
        }
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "render", e.toString());
    }
});

global.vdist2 = (_Pos1, _Pos2, toggle = true) => {
    try {
        if (!_Pos1 || !_Pos2)
            return -1;
        const _rY = _Pos1.y - _Pos2.y,
            _rX = _Pos1.x - _Pos2.x;
        if (toggle) {
            const _rZ = _Pos1.z - _Pos2.z;
            return Math.sqrt(_rY * _rY + _rX * _rX + _rZ * _rZ);
        }
        return Math.sqrt(_rY * _rY + _rX * _rX);
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "world/anim", "vdist2", e.toString());
    }
}

gm.events.add("ta1", (player, target) => {
    player.attachTo(target.handle, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, 0, false);
});

gm.events.add("ta3", () => {    
    player.detach(true, true);
});

gm.events.add("ta2", () => {    
    mp.players.local.detach(true, true);
});

gm.events.add("Client_CarryPlayer", function (player, target, id) {
    try{
        if (player && mp.players.exists(player) && target && mp.players.exists(target)) {
            if (target && mp.players.exists(target))
                target.detach(true, false);
            if (id == 1)
                target.attachTo(player.handle, 0x0, 0.2, 0.25, 0.3, 0x0, 0x0, 0x5a, false, false, false, false, 0x0, false);
            else if (id == 2)
                target.attachTo(player.handle, 0x0, 0.2, 0.1, 0.6, 0x0, 0x0, 0x0, false, false, false, false, 0x0, false);
            else if (id == 3)
                target.attachTo(player.handle, 0x0, 0x0, -0.07, 0.45, 0x0, 0x0, 0x0, false, false, false, false, 0x0, false);
        }
    }
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "world/anim", "Client_CarryPlayer", e.toString());
	}
});