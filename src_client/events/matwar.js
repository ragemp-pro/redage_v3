const 
    covercrate_model = ["ex_prop_adv_case_sm", "ex_prop_adv_case_sm_02", "ex_prop_adv_case_sm_03", "ex_prop_adv_case_sm_flash", "ex_prop_adv_case_sm_flash"],
    parachute_model = "p_cargo_chute_s";
let globalFightData = {};
let IntervalId = null;

gm.events.add("client.matwar.fight.create", async (id, propId, position, time) => {
    try
    {
        mp.events.call("client.matwar.fight.dell", id);
        await global.loadModel(covercrate_model [propId]);
        await global.loadModel(parachute_model);

        const fightData = {};
        const posZ = -0.19;
        fightData.object = mp.game.object.createObject(mp.game.joaat(covercrate_model [propId]), position.x, position.y, position.z + (27.5 * time), true, true, true);
        Natives.SET_ENTITY_LOD_DIST (fightData.object, 1000);
        Natives.ACTIVATE_PHYSICS (fightData.object);
        Natives.SET_DAMPING (fightData.object, 2, 3063 - 3062.1);
        Natives.SET_ENTITY_VELOCITY (fightData.object, 0, 0, posZ);
        Natives.SET_ENTITY_INVINCIBLE (fightData.object, true);
        Natives.SET_ENTITY_PROOFS (fightData.object, true, true, true, true, true, true, true, true, true);

        fightData.parachute = mp.game.object.createObject(mp.game.joaat(parachute_model), position.x, position.y, position.z + (27.5 * time), true, true, true);
        Natives.SET_ENTITY_LOD_DIST (fightData.parachute, 1000);
        Natives.SET_ENTITY_VELOCITY (fightData.parachute, 0, 0, posZ);

        fightData.sound = Natives.GET_SOUND_ID ();
        mp.game.audio.playSoundFromEntity(fightData.sound, "Crate_Beeps", fightData.object, "MP_CRATE_DROP_SOUNDS", true, 0);
        Natives.ATTACH_ENTITY_TO_ENTITY (fightData.parachute, fightData.object, 0, 0, 0, 3860.35 - 3860, 0, 0, 0, false, false, false, false, 2, true);
        Natives.FREEZE_ENTITY_POSITION (fightData.object, false);
        
        globalFightData[id] = fightData;

        setTimeout(() => {
            const _fightData = globalFightData[id];
            if (_fightData) {
                if (_fightData.parachute && Natives.DOES_ENTITY_EXIST (_fightData.parachute)) {
                    mp.game.object.deleteObject(_fightData.parachute);
                    _fightData.parachute = null;
                }
                const shape = mp.colshapes.newSphere(
                    position.x, position.y, position.z,
                    2.5,
                    0
                );
        
                shape.MatWarFightId = id;
                globalFightData[id].share = shape;

                position.z -= 0.3;
                mp.events.call("client.particleEffect", "core", "exp_grd_flare", position, 30000);
            }
        }, 1000 * (8.33 * time));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/matwar", "client.matwar.fight.create", e.toString());
    }
});

gm.events.add("client.matwar.fight.dell", (id) => {
    try
    {
        const fightData = globalFightData[id];
        if (fightData) {
            if (fightData.object && Natives.DOES_ENTITY_EXIST (fightData.object)) 
                mp.game.object.deleteObject(fightData.object);
            if (fightData.parachute && Natives.DOES_ENTITY_EXIST (fightData.parachute)) 
                mp.game.object.deleteObject(fightData.parachute);
            if (fightData.sound) {
                mp.game.audio.stopSound(fightData.sound);
                mp.game.audio.releaseSoundId(fightData.sound);
            }
            if (fightData.share) {
                fightData.share.destroy();
            }
            delete globalFightData[id];
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/matwar", "client.matwar.fight.dell", e.toString());
    }
});

global.selectMatwarFightId = -1;
gm.events.add('playerEnterColshape', (shape) => {
	try
	{
		if (shape && shape.MatWarFightId !== undefined && global.selectMatwarFightId === -1) {
			mp.events.call('hud.oEnter', 1);
			global.selectMatwarFightId = shape.MatWarFightId;
            global.canHackMatWarDrop = false;
            mp.events.callRemote("CheckMatWarDropLockStatus", global.selectMatwarFightId);
            gm.discord(translateText("Играет MatWar"));
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/matwar", "playerEnterColshape", e.toString());
    }
});

gm.events.add('playerExitColshape', (shape) => {
	try
	{
		if (shape && shape.MatWarFightId !== undefined && global.selectMatwarFightId !== -1 && shape.MatWarFightId === global.selectMatwarFightId) {
			mp.events.call('hud.cEnter');
			global.selectMatwarFightId = -1;
            global.canHackMatWarDrop = false;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/matwar", "playerExitColshape", e.toString());
    }
});

// Взлом:

const matWarDropLock = {
    interval: undefined,
    health: 100
};

gm.events.add("client.updateMatWarHackStatus", (status, health) => {
    try
    {
        global.canHackMatWarDrop = status;
        matWarDropLock.health = health;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/matwar", "client.updateMatWarHackStatus", e.toString());
    }
});

gm.events.add('matWarDrop_hackStatus', (state) => {
    if (state == 1) {
        global.localplayer.freezePosition(true);

        if (matWarDropLock.interval !== undefined) {
            clearInterval(matWarDropLock.interval);
            matWarDropLock.interval = undefined;
        }

        matWarDropLock.interval = setInterval(() => {
            matWarDropLock.health -= 1;
            
            if (matWarDropLock.health <= 0) {
                matWarDropLock.health = 0;

                if (matWarDropLock.interval !== undefined) {
                    clearInterval(matWarDropLock.interval);
                    matWarDropLock.interval = undefined;
                }

                global.canHackMatWarDrop = false;
                global.localplayer.freezePosition(false);
                mp.events.callRemote('MatWarDropChangeLockStatus', global.selectMatwarFightId, matWarDropLock.health);
            }
        }, 1000);
    }
    else if (state == 2) {
        if (matWarDropLock.interval !== undefined) {
            clearInterval(matWarDropLock.interval);
            matWarDropLock.interval = undefined;
        }

        global.localplayer.freezePosition(false);
        mp.events.callRemote('MatWarDropChangeLockStatus', global.selectMatwarFightId, matWarDropLock.health);
    }
});

gm.events.add("render", () =>  {
    if (global.selectMatwarFightId !== -1 && global.canHackMatWarDrop === true && mp.game.controls.isControlJustPressed(0, 38)) {
        mp.events.callRemote('server.matWar.fight.player.start.hack', global.selectMatwarFightId);
    }

    if (global.canHackMatWarDrop === true && mp.game.controls.isControlJustReleased(0, 38)) {
        mp.events.call('matWarDrop_hackStatus', 2);
    }
});