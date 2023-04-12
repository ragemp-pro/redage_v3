global.requestNamedPtfxAsset = (fxLib) => new Promise(async (resolve, reject) => {
	try {
        if (mp.game.streaming.hasNamedPtfxAssetLoaded(fxLib))
            return resolve(true);

        mp.game.streaming.requestNamedPtfxAsset(fxLib);
        let d = 0;
        while (!mp.game.streaming.hasNamedPtfxAssetLoaded(fxLib)) {
            if (d > 500) return resolve(translateText("Ошибка requestNamedPtfxAsset. asset: ") + fxLib);
            d++;
            await global.wait (10);
        }
        return resolve(true);
    } 
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "synchronization/animation", "requestNamedPtfxAsset", e.toString());
        resolve();
    }
})

class ParticleFx {

    constructor(fxLib, fxName, data = {}) {
        try {
            this.fxLib = fxLib;
            this.fxName = fxName;
            this.scale = data && null != data.scale ? data.scale : 1;
            this.rgb = data && null != data.r ? [data.r, data.g, data.b] : [0, 0, 0];
            this.xOffset = data && null != data.xOffset ? data.xOffset : 0;
            this.yOffset = data && null != data.yOffset ? data.yOffset : 0;
            this.zOffset = data && null != data.zOffset ? data.zOffset : 0;
            this.xRot = data && null != data.xRot ? data.xRot : 0;
            this.yRot = data && null != data.yRot ? data.yRot : 0;
            this.zRot = data && null != data.zRot ? data.zRot : 0;
            this.fxCall;
            this.fxHandle;
            this.load();
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "constructor", e.toString());
        }
    }

    async load() {
        try {
            return new Promise(async (resolve, reject) => {
                return await global.requestNamedPtfxAsset (this.fxLib), resolve();
            });
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "load", e.toString());
        }
    }
    async timeoutRemove(time) {
        try {
            await global.wait(time);

            if (mp.game.graphics.doesParticleFxLoopedExist(this.fxHandle))
                mp.game.graphics.stopParticleFxLooped(this.fxHandle, true);

            if (this.entity) 
                Natives.REMOVE_PARTICLE_FX_FROM_ENTITY (this.entity);

            mp.game.graphics.removeParticleFx(this.fxHandle, true);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "timeoutRemove", e.toString());
        }
    }

    objRemove() {
        try {
            mp.game.graphics.stopParticleFxLooped(this.fxHandle, false);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "objRemove", e.toString());
        }
    }

    remove(player, fxHandle) {
        try {
            if (mp.game.graphics.doesParticleFxLoopedExist(fxHandle))
                mp.game.graphics.stopParticleFxLooped(fxHandle, true);

            if (this.entity) 
                Natives.REMOVE_PARTICLE_FX_FROM_ENTITY (this.entity);

            mp.game.graphics.removeParticleFx(fxHandle, true);
            delete player.fxHandle;
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "remove", e.toString());
        }
    }

    async playOnCoords(PosX, PosY, PosZ, dellTime = 1000) {
        try {
            await this.load();
            this.fxCall = mp.game.graphics.setPtfxAssetNextCall(this.fxLib);

            mp.game.graphics.setParticleFxNonLoopedColour(this.rgb[0], this.rgb[1], this.rgb[2]);

            this.fxHandle = mp.game.graphics.startParticleFxLoopedAtCoord(this.fxName, PosX, PosY, PosZ, 0, 0, 0, this.scale, false, false, false, false);

            if (-1 != dellTime)
                this.timeoutRemove(dellTime);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playOnCoords", e.toString());
        }
    }
    async playOnCoordsOnce(PosX, PosY, PosZ, dellTime = 1000) {
        try {
            await this.load();
            this.fxCall = mp.game.graphics.setPtfxAssetNextCall(this.fxLib);

            mp.game.graphics.setParticleFxNonLoopedColour(this.rgb[0], this.rgb[1], this.rgb[2]);

            this.fxHandle = mp.game.graphics.startParticleFxNonLoopedAtCoord(this.fxName, PosX, PosY, PosZ, 0, 0, 0, this.scale, false, false, false, false);
            
            if (-1 != dellTime)
                this.timeoutRemove(dellTime);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playOnCoordsOnce", e.toString());
        }
    }
    async playOnEntity(entity, dellTime = 1000) {
        try {
            await this.load();
            this.entity = entity;

            if (entity.fxHandle) 
                this.remove(entity, entity.fxHandle);

            this.fxCall = mp.game.graphics.setPtfxAssetNextCall(this.fxLib);
            
            mp.game.graphics.setParticleFxNonLoopedColour(this.rgb[0], this.rgb[1], this.rgb[2]);

            this.fxHandle = mp.game.graphics.startParticleFxLoopedOnEntity(this.fxName, entity.handle, 0, 0, 0, 0, 0, 0, this.scale, false, false, false);

            entity.fxHandle = this.fxHandle;

            this.timeoutRemove(dellTime);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playOnEntity", e.toString());
        }
    }
    async playOnEntityOnce(entity) {
        try {
            await this.load();
            this.entity = entity;

            if (entity.fxHandle) 
                this.remove(entity, entity.fxHandle);

            this.fxCall = mp.game.graphics.setPtfxAssetNextCall(this.fxLib);
        
            mp.game.graphics.setParticleFxNonLoopedColour(this.rgb[0], this.rgb[1], this.rgb[2]);

            this.fxHandle = mp.game.graphics.startParticleFxNonLoopedOnEntity(this.fxName, entity.handle, 0, 0, 0, 0, 0, 0, this.scale, false, false, false);
            
            entity.fxHandle = this.fxHandle;

            this.timeoutRemove(1000);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playOnEntityOnce", e.toString());
        }
    }
    async playOnEntityBone(entity, boneName, dellTime = 1000) {
        try {
            
            await this.load();
            this.entity = entity;

            if (entity.fxHandle) 
                this.remove(entity, entity.fxHandle);

            this.fxCall = mp.game.graphics.setPtfxAssetNextCall(this.fxLib);
        
            mp.game.graphics.setParticleFxNonLoopedColour(this.rgb[0], this.rgb[1], this.rgb[2]);

            this.fxHandle = mp.game.graphics.startParticleFxLoopedOnEntityBone(
                this.fxName, 
                entity.handle,
                this.xOffset,
                this.yOffset,
                this.zOffset,
                this.xRot,
                this.yRot,
                this.zRot,
                typeof boneName === "string" ? entity.getBoneIndexByName(boneName) : entity.getBoneIndex(boneName),
                this.scale,
                false,
                false,
                false
            );

            entity.fxHandle = this.fxHandle;

            this.timeoutRemove(dellTime);
        } catch (e) {
            mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playOnEntityBone", e.toString());
        }
    }
}

//************************************************ */
global.playFXonPos = async (PosX, PosY, PosZ, fxLib, fxName, dellTime = 1000, data = {}) => {
    try {
        new ParticleFx(fxLib, fxName, data).playOnCoords(PosX, PosY, PosZ, dellTime);
    } catch (e) {
        mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playFXonPos", e.toString());
    }
};

gm.events.add("client.playFXonPos", (PosX, PosY, PosZ, fxLib, fxName, dellTime = 1000, data = {}) => {
    global.playFXonPos(PosX, PosY, PosZ, fxLib, fxName, dellTime, data);
});

//************************************************ */
global.playFXonPosOnce = async (PosX, PosY, PosZ, fxLib, fxName, dellTime = 1000, data = {}) => {
    try {
        new ParticleFx(fxLib, fxName, data).playOnCoordsOnce(PosX, PosY, PosZ, dellTime);
    } catch (e) {
		mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playFXonPosOnce", e.toString());
    }
};

gm.events.add("client.playFXonPosOnce", (PosX, PosY, PosZ, fxLib, fxName, dellTime = 1000, data = {}) => {
    global.playFXonPosOnce(PosX, PosY, PosZ, fxLib, fxName, dellTime, data);
});

//************************************************ */
global.playFXonEntity = async (entity, fxLib, fxName, dellTime = 1000, data = {}) => {
    try {
        if (!entity) return;
        new ParticleFx(fxLib, fxName, data).playOnEntity(entity, dellTime);
    } catch (e) {
		mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playFXonEntity", e.toString());
    }
};

gm.events.add("client.playFXonEntity", (entity, fxLib, fxName, dellTime = 1000, data = {}) => {    
    if (!mp.players.exists(entity) && !mp.vehicles.exists(entity) && !mp.objects.exists(entity))
        return;
    global.playFXonEntity(entity, fxLib, fxName, dellTime, data);
});

//************************************************ */
global.playFXonEntityOnce = async (entity, fxLib, fxName, data = {}) => {
    try {
        if (!entity) return;
        new ParticleFx(fxLib, fxName, data).playOnEntityOnce(entity);
    } catch (e) {
		mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playFXonEntityOnce", e.toString());
    }
};

gm.events.add("client.playFXonEntityOnce", (entity, fxLib, fxName, data = {}) => {    
    if (!mp.players.exists(entity) && !mp.vehicles.exists(entity) && !mp.objects.exists(entity))
        return;
    global.playFXonEntityOnce(entity, fxLib, fxName, data);
});

//************************************************ */
global.playFXonEntityBone = async (entity, boneName, fxLib, fxName, dellTime = 1000, data = {}) => {
    try {
        if (!entity) return;
        new ParticleFx(fxLib, fxName, data).playOnEntityBone(entity, boneName, dellTime);
    } catch (e) {
		mp.events.callRemote("client_trycatch", "synchronization/particleFx", "playFXonEntityBone", e.toString());
    }
};

gm.events.add("client.playFXonEntityBone", (entity, boneName, fxLib, fxName, dellTime = 1000, data = {}) => {
    if (!mp.players.exists(entity) && !mp.vehicles.exists(entity) && !mp.objects.exists(entity))
        return;
    global.playFXonEntityBone(entity, boneName, fxLib, fxName, dellTime, data);
});

global.particleFx = ParticleFx;

