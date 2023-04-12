global.VolumeInterface = 100;
global.VolumeQuest = 100;
global.VolumeAmbient = 100;
global.VolumePhoneRadio = 100;

let soundPoolData = {};

let pos = null,
    rot = null;


const updatePlayerPos = () => {
    const playerPos = global.localplayer.position;
    const cameraPos = global.cameraManager.gameplayCam().getDirection();
    if (playerPos !== pos || cameraPos !== rot) {
        pos = playerPos;
        rot = cameraPos;
        mp.gui.emmit(`window.sounds.updatePlayerSoundPos(${playerPos.x},
                                                        ${playerPos.y},
                                                        ${playerPos.z},
                                                        ${cameraPos.x},
                                                        ${cameraPos.y},
                                                        ${cameraPos.z});`);
    }
}

const updateSoundsPos = () => {
    try {
        Object.values (soundPoolData).forEach((data) => {
            const entity = data.entity;
            if (entity) {
                if (entity.doesExist()) {
                    const entityPos = entity.position;
                    mp.gui.emmit(`window.sounds.updateSoundPos('${data.id}',
                                                                ${entityPos.x},
                                                                ${entityPos.y},
                                                                ${entityPos.z});`);
                } else {
                    data.destroy();
                }
            }
        });
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "synchronization/sounds", "updateSoundsPos", e.toString());
    }
}

setInterval(() => {
    if (!global.loggedin || true)
        return;
    updatePlayerPos ();
    updateSoundsPos ();
}, 2000)

gm.events.add('client.sounds.play', function (_sound, _volume, _loop) {
    global.PlaySound (_sound, _volume, _loop);
});

gm.events.add('client.sounds.stop', function (_sound) {
    global.StopSound (_sound);
});

global.StopSound = (_sound) => {
    mp.gui.emmit(`window.sounds.stopSound('${_sound}');`);
}

global.PlaySound = (_sound, _volume = 0.1, _loop = false) => {
    mp.gui.emmit(`window.sounds.playSound('${_sound}',
        ${_volume},
        ${_loop});`);
}

global.Play2DSound = (_url, _volume = 0.1) => {
    mp.gui.emmit(`window.sounds.play2DRadio('${_url}',
        ${_volume});`);
}

global.change2DSoundVolume = (_volume = 0.1) => {
    mp.gui.emmit(`window.sounds.change2DRadioVolume(${_volume});`);
}

gm.events.add('client.sounds.play2d', function (_url, _volume) {
    global.Play2DSound (_url, _volume);
});

gm.events.add('client.sounds.change2dVolume', function (_volume) {
    global.change2DSoundVolume (_volume);
});

global.Stop2DRadio = () => {
    mp.gui.emmit(`window.sounds.stop2DRadio();`);
}

gm.events.add('client.sounds.stop2d', function () {
    global.Stop2DRadio ();
});


class Play3DSoundPos {
    constructor(id, sound, position, data = {}) {
        try {
            this.id = id;
            this.sound = sound;
            this.position = position;
            this.volume = data && null != data.volume ? data.volume : 100;
            this.looped = !(!data || null == data.looped) && data.looped;
            this.maxDistance = data && null != data.maxDistance ? data.maxDistance : 10;
            this.dimension = data && null != data.dimension ? data.dimension : 0;
            this.rolloffFactor = data && null != data.rolloffFactor ? data.rolloffFactor : 1;
            this.refDistance = data && null != data.refDistance ? data.refDistance : 1;
            this.startOffsetPercent = data.startOffsetPercent;
            this.pannerAttr = data.pannerAttr;
            this.fade = data.fade;

            const soundData = {};
            soundData.id = id;
            soundData.sound = sound;
            soundData.volume = this.volume;
            soundData.loop = this.looped;
            soundData.rolloffFactor = this.rolloffFactor;
            soundData.refDistance = this.refDistance;
            soundData.startOffsetPercent = this.startOffsetPercent;
            soundData.pannerAttr = this.pannerAttr;
            soundData.fade = this.fade;
            soundData.maxDistance = this.maxDistance;
            soundData.x = position.x;
            soundData.y = position.y;
            soundData.z = position.z;
            mp.gui.emmit(`window.sounds.play3DSound('${JSON.stringify(soundData)}');`);
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sounds", "Play3DSoundPos", e.toString());
        }
    }
}


class Play3DSoundEntity {
    constructor(id, sound, entity, data = {}) {
        try {
            if (!entity || !entity.doesExist()) return;

            this.id = id;
            this.sound = sound;
            this.entity = entity;
            this.volume = data && null != data.volume ? data.volume : 100;
            this.looped = !(!data || null == data.looped) && data.looped;
            this.maxDistance = data && null != data.maxDistance ? data.maxDistance : 10;
            this.rolloffFactor = data && null != data.rolloffFactor ? data.rolloffFactor : 1;
            this.refDistance = data && null != data.refDistance ? data.refDistance : 1;
            this.startOffsetPercent = data.startOffsetPercent;
            this.pannerAttr = data.pannerAttr;
            this.fade = data.fade;

            const 
                pos = entity.position,
                soundData = {};
            
            soundData.id = id;
            soundData.sound = sound;
            soundData.volume = this.volume;
            soundData.loop = this.looped;
            soundData.rolloffFactor = this.rolloffFactor;
            soundData.refDistance = this.refDistance;
            soundData.startOffsetPercent = this.startOffsetPercent;
            soundData.pannerAttr = this.pannerAttr;
            soundData.fade = this.fade;
            soundData.maxDistance = this.maxDistance;
            soundData.x = pos.x;
            soundData.y = pos.y;
            soundData.z = pos.z;

            mp.gui.emmit(`window.sounds.play3DSound('${JSON.stringify(soundData)}');`);
            soundPoolData[id] = this;
        }
        catch (e) 
        {
            mp.events.callRemote("client_trycatch", "synchronization/sounds", "Play3DSoundEntity", e.toString());
        }
    }
    destroy() {
        delete soundPoolData[this.id];
    }
}

gm.events.add("api.play3DSoundPos", (id, sound, position, data) => {
    try {
        const dimension = data && null != data.dimension ? data.dimension : 0;
        if (position && null != sound && dimension === global.localplayer.dimension)
            new Play3DSoundPos(id, sound, position, data);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/sounds", "play3DSoundPos", e.toString());
	}
});

gm.events.add("api.play3DSoundEntity", (id, sound, entity, data) => {
    try {
        if (entity && entity.doesExist() &&
            null != sound &&
            entity.dimension === global.localplayer.dimension) {
            if (entity.dimension === global.localplayer.dimension && entity.type === "player" && entity.remoteId === global.localplayer.remoteId)
                return;
            new Play3DSoundEntity(id, sound, entity, data);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "synchronization/sounds", "play3DSoundEntity", e.toString());
    }
})

gm.events.add("sounds.dell", (id) => {
    if (soundPoolData[id])
        soundPoolData[id].destroy();
});