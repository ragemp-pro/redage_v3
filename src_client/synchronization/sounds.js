
let soundApiBrowser = mp.browsers.new("https://cdn.redage.net/soundapi/index.html");

let isBrowserLoadingFailed = false;
mp.events.add('browserLoadingFailed', (browser) => {
    if (browser == soundApiBrowser && !isBrowserLoadingFailed) {
        isBrowserLoadingFailed = true;
        soundApiBrowser.destroy();
        soundApiBrowser = mp.browsers.new("https://redage.akamaized.net/soundapi/index.html");
    }
});

const localExecuteJson = (name, json) => {
    soundApiBrowser.execute (`${name}('${JSON.stringify (json)}')`);
}

const localExecute = (execute) => {
    soundApiBrowser.execute (execute);
}

const entitysSoundData = {};

global.VolumeInterface = 100;
global.VolumeQuest = 100;
global.VolumeAmbient = 100;
global.VolumePhoneRadio = 100;


gm.events.add("sounds.playInterface", (sound, volume) => {
    soundApi.playInterface (sound, volume);
});

//
gm.events.add("sounds.playAmbient", (sound, data) => {  
    data.volume = soundApi.getVolume (data.volume, global.VolumeAmbient)
    soundApi.playAmbient (sound, data);
});

gm.events.add("sounds.playPosAmbient", (id, sound, position, data) => {
    const dimension = data && null != data.dimension ? data.dimension : 0;
    if (position && null != sound && dimension === global.localplayer.dimension) {        
        data.volume = soundApi.getVolume (data.volume, global.VolumeAmbient)
        new soundApi.Sounds3DPos (id, sound, position, data);
    }
});

gm.events.add("sounds.playEntityAmbient", (sound, entity, data) => {

    if (entity && entity.doesExist() &&
        null != sound &&
        entity.dimension === global.localplayer.dimension) {
        if (entity.type === "player" && entity.remoteId === global.localplayer.remoteId)
            return;
        data.volume = soundApi.getVolume (data.volume, global.VolumeAmbient)
        new soundApi.Sounds3DEntity (sound, entity, data);
    }
});

//
gm.events.add("sounds.play2DRadio", (url, volume) => {
    soundApi.play2DRadio (url, volume);
});

gm.events.add("sounds.volume2DRadio", (volume) => {
    soundApi.volume2DRadio (volume);
});

gm.events.add("sounds.stop2DRadio", () => {
    soundApi.stop2DRadio ();

});

//
gm.events.add("sounds.stop", (id) => {
    soundApi.stopSound (id);
});

gm.events.add("sounds.trigger", (name, id) => {
    if ("onEnd" !== name)
        return;

    const entitySoundData = entitysSoundData[id];
    if (entitySoundData)
        entitySoundData.destroy();
});

soundApi.getVolume = (value, volume) => {
    if (volume <= 0) return 0;
    return value / 100 * volume;
}


//

soundApi.play2DRadio = (url, volume) => {
    localExecuteJson("play2DRadio", {
        url: url,
        volume: volume
    });
}

soundApi.volume2DRadio = (volume) => localExecute(`window.volume2DRadio(${volume})`);

soundApi.stop2DRadio = () => localExecute("window.stop2DRadio()");

//

soundApi.stopSound = (id) => localExecute(`window.stopSound('${id}')`)//localExecuteJson("stopSound", id);

soundApi.playInterface = (sound, volume) => {
    new Sounds2D(sound, {        
        volume: soundApi.getVolume (volume, global.VolumeInterface)
    });
}

soundApi.playAmbient = (sound, data) => {
    new Sounds2D(sound, data);
}

//Class`s

class Sounds3DPos {
    constructor(id, sound, position, data = {}) {
        this.id = id;
        this.sound = sound;
        this.position = position;
        this.volume = data && null != data.volume ? data.volume : 1;
        this.looped = !(!data || null == data.looped) && data.looped;
        this.maxDistance = data && null != data.maxDistance ? data.maxDistance : 10;
        this.dimension = data && null != data.dimension ? data.dimension : 0;
        this.rolloffFactor = data && null != data.rolloffFactor ? data.rolloffFactor : 1;
        this.refDistance = data && null != data.refDistance ? data.refDistance : 1;
        this.startOffsetPercent = data.startOffsetPercent;
        this.pannerAttr = data.pannerAttr;
        this.fade = data.fade;
        this.syncAudio = data.syncAudio;
        this.startedTime = Date.now();
        const executeData = {};
        executeData.id = id;
        executeData.sound = sound;
        executeData.volume = this.volume;
        executeData.loop = this.looped;
        executeData.rolloffFactor = this.rolloffFactor;
        executeData.refDistance = this.refDistance;
        executeData.startOffsetPercent = this.startOffsetPercent;
        executeData.pannerAttr = this.pannerAttr;
        executeData.fade = this.fade;
        executeData.syncAudio = this.syncAudio;
        executeData.maxDistance = this.maxDistance;
        executeData.x = position.x;
        executeData.y = position.y;
        executeData.z = position.z;
        localExecuteJson("play3DSound", executeData);
        entitysSoundData[this.id] = this;
    }
    destroy() {
        soundApi.stopSound(this.id);
        delete entitysSoundData[this.id];
    }
}

soundApi.Sounds3DPos = Sounds3DPos;

class Sounds3DEntity {
    constructor(sound, entity, data = {}) {
        if (!mp.players.exists(entity) && !mp.vehicles.exists(entity) && !mp.objects.exists(entity) && !mp.peds.exists(entity))
            return;

        this.id = data && null != data.id ? data.id : new Date().getTime();
        this.sound = sound;
        this.entity = entity;
        this.volume = data && null != data.volume ? data.volume : 1;
        this.looped = !(!data || null == data.looped) && data.looped;
        this.maxDistance = data && null != data.maxDistance ? data.maxDistance : 10;
        this.rolloffFactor = data && null != data.rolloffFactor ? data.rolloffFactor : 1;
        this.refDistance = data && null != data.refDistance ? data.refDistance : 1;
        this.startOffsetPercent = data.startOffsetPercent;
        this.pannerAttr = data.pannerAttr;
        this.fade = data.fade;
        this.startedTime = Date.now();

        const 
            position = entity.position,
            executeData = {};

        executeData.id = this.id;
        executeData.sound = sound;
        executeData.volume = this.volume;
        executeData.loop = this.looped;
        executeData.rolloffFactor = this.rolloffFactor;
        executeData.refDistance = this.refDistance;
        executeData.startOffsetPercent = this.startOffsetPercent;
        executeData.pannerAttr = this.pannerAttr;
        executeData.fade = this.fade;
        executeData.maxDistance = this.maxDistance;
        executeData.x = position.x;
        executeData.y = position.y;
        executeData.z = position.z;
        localExecuteJson("play3DSound", executeData);
        entitysSoundData[this.id] = this;
    }
    destroy() {
        soundApi.stopSound(this.id);
        delete entitysSoundData[this.id];
    }
}
soundApi.Sounds3DEntity = Sounds3DEntity;

class Sounds2D {
    constructor(sound, data) {
        this.sound = sound;
        this.id = data && null != data.id ? data.id : new Date().getTime();
        this.volume = data && null != data.volume ? data.volume : 1;
        this.loop = !(!data || null == data.loop) && data.loop;

        const executeData = {};
        executeData.sound = sound;
        executeData.id = this.id;
        executeData.volume = this.volume;
        executeData.loop = this.loop;
        localExecuteJson("playSound", executeData);
    }
}
soundApi.Sounds2D = Sounds2D;

//

gm.events.add(global.renderName ["soundRot"], () => {

    if (!global.loggedin)
        return;

    const cameraPos = global.cameraManager.gameplayCam();
    const playerPos = mp.players.local.position;
    const cameraCoord = cameraPos.getCoord();
    const cameraRot = cameraPos.getRot(2);
    const pPos = global.midVector(playerPos, cameraCoord);
    const {
            forward: ofPos,
            up: upPos
        } = global.principalAxesToOrientation(cameraRot.x, cameraRot.y, cameraRot.z);
    const px = global.roundNumber(pPos.x);
    const py = global.roundNumber(pPos.y);
    const pz = global.roundNumber(pPos.z);

    const ofx = global.roundNumber(ofPos.x);
    const ofy = global.roundNumber(ofPos.y);
    const ofz = global.roundNumber(ofPos.z);

    /*const heading = (cameraRot.z - 90) * Math.PI / 180;

    const ofx = Math.cos(heading);
    const ofy = 0;
    const ofz = Math.sin(heading);*/

    const oux = 0//global.clamp(global.roundNumber(upPos.x), -1, 1);
    const ouy = 1//global.clamp(global.roundNumber(upPos.y), -1, 1);
    const ouz = 0//global.clamp(global.roundNumber(upPos.z), -1, 1);

    localExecuteJson("updatePlayerSoundPos", {
        px: px,
        py: py,
        pz: pz,
        ofx: ofx,
        ofy: ofy,
        ofz: ofz,
        oux: oux,
        ouy: ouy,
        ouz: ouz,
    });
})

gm.events.add(global.renderName ["sound"], () => {
    const playerPos = global.localplayer.position;
    Object.values(entitysSoundData).forEach(soundData => {
        if (!soundData.looped && soundData.startedTime > 0 && Date.now() - soundData.startedTime > 1200000)
            return soundData.destroy();

        if (soundData.maxDistance > 0 && !soundData.isMutedFarAway && soundData.position && global.vdist2(playerPos, soundData.position) > 2 * soundData.maxDistance) {
            soundData.isMutedFarAway = true;

            localExecuteJson("setMuted", {
                id: soundData.id,
                mute: true
            })
        } else if (soundData.isMutedFarAway) {
            soundData.isMutedFarAway = false;

            localExecuteJson("setMuted", {
                id: soundData.id,
                mute: false
            })
        }

        const entity = soundData.entity;
        if (entity) {
            if (mp.players.exists(entity) || mp.vehicles.exists(entity) || mp.objects.exists(entity) || mp.peds.exists(entity)) {
                const pos = new mp.Vector3(global.roundNumber(entity.position.x), global.roundNumber(entity.position.y), global.roundNumber(entity.position.z));
                localExecuteJson("updateSoundPos", {
                    id: soundData.id,
                    x: pos.x,
                    y: pos.y,
                    z: pos.z,
                    hasLos: mp.players.local.hasClearLosTo(entity.handle, 17)
                });
            } else
                soundData.destroy();
        }
    })
});

global.principalAxesToOrientation = (y = 0, p = 0, r = 0) => {
    const { yaw = 0, pitch = 0, roll = 0 } = typeof y === 'object' ? y : { yaw: y, pitch: p, roll: r };
    // vector determining which way the listener is facing
    const forward = { x: 0, y: 0, z: 0 };
    // vector determining the rotation of the listener's head
    // where no rotation means the head is pointing up
    const up = { x: 0, y: 0, z: 0 };

    // Yaw (a.k.a. heading) is the rotation around the Y axis
    // convert to radians first
    const yawRad = yaw * (Math.PI / 180);
    // at 0 degrees, the X component should be 0
    //so we calculate it using sin(), which starts at 0
    forward.x = Math.sin(yawRad);
    // at 0 degrees, the Z component should be -1,
    // because the negative Z axis points *away from* the listener
    // so we calculate it using cos(), which starts at 1
    // with a phase shift of 90 degrees (or PI radians)
    forward.z = Math.cos(yawRad + Math.PI);

    // Pitch is the rotation around the X axis
    // we can use it to calculate both vectors' Y components
    const pitchRad = pitch * (Math.PI / 180);
    // Forward Y component should start at 0
    forward.y = Math.sin(pitchRad);
    // Up Y component should start at 1 (top of the head pointing up)
    up.y = Math.cos(pitchRad);

    // Roll is the rotation around the Z axis
    const rollRad = roll * (Math.PI / 180);
    // both X and Y components should start at 0
    // (top of the head pointing directly upwards)
    up.x = Math.sin(rollRad);
    up.z = Math.sin(rollRad);

    return { forward, up };
}