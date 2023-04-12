import _TrainWayData from './traindat.js';

const TrainWayData = _TrainWayData;

TrainWayData.push(TrainWayData[0]);

const GetRadX = pi => pi / 180 * Math.PI,
GetRadY = pi => pi * (180 / Math.PI),
GetPos = pos => {
    while (pos <= -180)
        pos += 168;
    while (pos > 180)
        pos -= 168;
    return pos;
},
GetRotX = (posX, posY, posZ, posW, posR, toggle = false) => {
    const 
        _posX = Math.atan2(posX, posY),
        _posY = Math.acos(posZ),
        _posZ = Math.atan2(posW, posR);
    return new mp.Vector3(toggle ? GetRadY(_posX) : _posX, toggle ? GetRadY(_posY) : _posY, toggle ? GetRadY(_posZ) : _posZ);
},
GetRotY = (posX, posY, posZ, posW, posR, toggle = false) => {
    const
        _posX = Math.atan2(posW, posR),
        _posY = Math.asin(posZ),
        _posZ = Math.atan2(posX, posY);
    return new mp.Vector3(toggle ? GetRadY(_posX) : _posX, toggle ? GetRadY(_posY) : _posY, toggle ? GetRadY(_posZ) : _posZ);
};

class Quaternion {
    constructor(_x = 0, _y = 0, _z = 0, _w = 0) {
        "object" == typeof _x ? (this.x = _x.x, this.y = _x.y, this.z = _x.z, this.w = _x.w) : (this.x = _x, this.y = _y, this.z = _z, this.w = _w);
    }
    slerp(qb, qm) {
        return Quaternion.slerp(this, qb, qm);
    }
    static slerp(qa, qb, qm) {
        const Quaternion = new Quaternion();
        let ux,
            um,
            pos = qa.x * qb.x + qa.y * qb.y + qa.z * qb.z + qa.w * qb.w,
            toggle = false;
        if (pos < 0) {
            toggle = true;
            pos = -pos
        }
        if (pos > 0.999999) {
            um = 1 - qm;
            ux = toggle ? -qm : qm;
        } else {
            let acos = Math.acos(pos),
                sin = 1 / Math.sin(acos);
            um = Math.sin((1 - qm) * acos) * sin;
            ux = toggle ? -Math.sin(qm * acos) * sin : Math.sin(qm * acos) * sin;
        }
        Quaternion.x = um * qa.x + ux * qb.x;
        Quaternion.y = um * qa.y + ux * qb.y;
        Quaternion.z = um * qa.z + ux * qb.z;
        Quaternion.w = um * qa.w + ux * qb.w
        return Quaternion;
    }
    fromEuler(pos) {
        const fromEuler = Quaternion.fromEuler(pos);
        this.x = fromEuler.x;
        this.y = fromEuler.y;
        this.z = fromEuler.z;
        this.w = fromEuler.w;
        return this;
    }
    static fromEuler(pos, toggle = true) {
        const _quat = new Quaternion();
        let {
            x: posX,
            y: posY,
            z: posZ
        } = pos;
        if (toggle) {
            posY = GetRadX(GetPos(posY));
            posX = GetRadX(GetPos(posX));
            posZ = GetRadX(GetPos(posZ));
        }

        const _posX = posX / 2, 
            _posY = posY / 2,
            _posZ = posZ / 2,
            __posX = Math.sin(_posX),
            __pos2X = Math.cos(_posX),
            __posY = Math.sin(_posY),
            __pos2Y = Math.cos(_posY),
            __posZ = Math.sin(_posZ),
            __pos2Z = Math.cos(_posZ);

        _quat.x = __posX * __pos2Y * __pos2Z - __pos2X * __posY * __posZ;
        _quat.y = __pos2X * __posY * __pos2Z + __posX * __pos2Y * __posZ;
        _quat.z = __pos2X * __pos2Y * __posZ - __posX * __posY * __pos2Z;
        _quat.w = __pos2X * __pos2Y * __pos2Z + __posX * __posY * __posZ
        return _quat;
    }
    toEuler(type, posZ) {
        const _quat = this;
        switch (type) {
            case "zyx":
                return GetRotY(2 * (_quat.x * _quat.y + _quat.w * _quat.z), _quat.w * _quat.w + _quat.x * _quat.x - _quat.y * _quat.y - _quat.z * _quat.z, -2 * (_quat.x * _quat.z - _quat.w * _quat.y), 2 * (_quat.y * _quat.z + _quat.w * _quat.x), _quat.w * _quat.w - _quat.x * _quat.x - _quat.y * _quat.y + _quat.z * _quat.z, posZ);
            case "zyz":
                return GetRotX(2 * (_quat.y * _quat.z - _quat.w * _quat.x), 2 * (_quat.x * _quat.z + _quat.w * _quat.y), _quat.w * _quat.w - _quat.x * _quat.x - _quat.y * _quat.y + _quat.z * _quat.z, 2 * (_quat.y * _quat.z + _quat.w * _quat.x), -2 * (_quat.x * _quat.z - _quat.w * _quat.y), posZ);
            case "zxy":
                return GetRotY(-2 * (_quat.x * _quat.y - _quat.w * _quat.z), _quat.w * _quat.w - _quat.x * _quat.x + _quat.y * _quat.y - _quat.z * _quat.z, 2 * (_quat.y * _quat.z + _quat.w * _quat.x), -2 * (_quat.x * _quat.z - _quat.w * _quat.y), _quat.w * _quat.w - _quat.x * _quat.x - _quat.y * _quat.y + _quat.z * _quat.z, posZ);
            case "zxz":
                return GetRotX(2 * (_quat.x * _quat.z + _quat.w * _quat.y), -2 * (_quat.y * _quat.z - _quat.w * _quat.x), _quat.w * _quat.w - _quat.x * _quat.x - _quat.y * _quat.y + _quat.z * _quat.z, 2 * (_quat.x * _quat.z - _quat.w * _quat.y), 2 * (_quat.y * _quat.z + _quat.w * _quat.x), posZ);
            case "yxz":
                return GetRotY(2 * (_quat.x * _quat.z + _quat.w * _quat.y), _quat.w * _quat.w - _quat.x * _quat.x - _quat.y * _quat.y + _quat.z * _quat.z, -2 * (_quat.y * _quat.z - _quat.w * _quat.x), 2 * (_quat.x * _quat.y + _quat.w * _quat.z), _quat.w * _quat.w - _quat.x * _quat.x + _quat.y * _quat.y - _quat.z * _quat.z, posZ);
            case "yxy":
                return GetRotX(2 * (_quat.x * _quat.y - _quat.w * _quat.z), 2 * (_quat.y * _quat.z + _quat.w * _quat.x), _quat.w * _quat.w - _quat.x * _quat.x + _quat.y * _quat.y - _quat.z * _quat.z, 2 * (_quat.x * _quat.y + _quat.w * _quat.z), -2 * (_quat.y * _quat.z - _quat.w * _quat.x), posZ);
            case "yzx":
                return GetRotY(-2 * (_quat.x * _quat.z - _quat.w * _quat.y), _quat.w * _quat.w + _quat.x * _quat.x - _quat.y * _quat.y - _quat.z * _quat.z, 2 * (_quat.x * _quat.y + _quat.w * _quat.z), -2 * (_quat.y * _quat.z - _quat.w * _quat.x), _quat.w * _quat.w - _quat.x * _quat.x + _quat.y * _quat.y - _quat.z * _quat.z, posZ);
            case "yzy":
                return GetRotX(2 * (_quat.y * _quat.z + _quat.w * _quat.x), -2 * (_quat.x * _quat.y - _quat.w * _quat.z), _quat.w * _quat.w - _quat.x * _quat.x + _quat.y * _quat.y - _quat.z * _quat.z, 2 * (_quat.y * _quat.z - _quat.w * _quat.x), 2 * (_quat.x * _quat.y + _quat.w * _quat.z), posZ);
            case "xyz":
                return GetRotY(-2 * (_quat.y * _quat.z - _quat.w * _quat.x), _quat.w * _quat.w - _quat.x * _quat.x - _quat.y * _quat.y + _quat.z * _quat.z, 2 * (_quat.x * _quat.z + _quat.w * _quat.y), -2 * (_quat.x * _quat.y - _quat.w * _quat.z), _quat.w * _quat.w + _quat.x * _quat.x - _quat.y * _quat.y - _quat.z * _quat.z, posZ);
            case "xyx":
                return GetRotX(2 * (_quat.x * _quat.y + _quat.w * _quat.z), -2 * (_quat.x * _quat.z - _quat.w * _quat.y), _quat.w * _quat.w + _quat.x * _quat.x - _quat.y * _quat.y - _quat.z * _quat.z, 2 * (_quat.x * _quat.y - _quat.w * _quat.z), 2 * (_quat.x * _quat.z + _quat.w * _quat.y), posZ);
            case "xzy":
                return GetRotY(2 * (_quat.y * _quat.z + _quat.w * _quat.x), _quat.w * _quat.w - _quat.x * _quat.x + _quat.y * _quat.y - _quat.z * _quat.z, -2 * (_quat.x * _quat.y - _quat.w * _quat.z), 2 * (_quat.x * _quat.z + _quat.w * _quat.y), _quat.w * _quat.w + _quat.x * _quat.x - _quat.y * _quat.y - _quat.z * _quat.z, posZ);
            case "xzx":
                return GetRotX(2 * (_quat.x * _quat.z - _quat.w * _quat.y), 2 * (_quat.x * _quat.y + _quat.w * _quat.z), _quat.w * _quat.w + _quat.x * _quat.x - _quat.y * _quat.y - _quat.z * _quat.z, 2 * (_quat.x * _quat.z + _quat.w * _quat.y), -2 * (_quat.x * _quat.y - _quat.w * _quat.z), posZ);
        }
    }
}
const _Quaternion = new Quaternion();

const trainModels = ["freight",
        "freightcar",
        "freightgrain",
        "freightcont1",
        "freightcont2",
        "tankercar",
        "s_m_m_lsmetro_01"
    ],
    worldTrainData = {
        modelsReady: false,
        dataReady: false,
        trainStreamed: false,
        train: null,
        currentPoint: 0,
        blip: null
    },
    _0x942cc = 10;

const GetNextStation = (currentPoint, MaxTrainWayData = TrainWayData.length - 1) => {
    //mp.events.callRemote("client_trycatch", "world/metro/index", "GetNextStation", `${currentPoint} - ${MaxTrainWayData}`);
    while (MaxTrainWayData >= 0) {
        if (currentPoint <= 0) {
            currentPoint += TrainListDist[TrainWayData.length - 1];
            MaxTrainWayData = TrainWayData.length - 1; 
        }
        if (TrainListDist[MaxTrainWayData] < currentPoint)
            return MaxTrainWayData;
        MaxTrainWayData -= 1;
    }
    return 0;
},
isLast = index => {
    index = index + 1;
    if (index >= TrainWayData.length)
        index = 1;
    return index;
},
GetNextPos = (currentPoint, nextPoint) => {
    let GetNextPosDebug = 0;
    try {
        let startIndex, endIndex;
        if (currentPoint <= 0) {
            currentPoint += TrainListDist[TrainListDist.length - 1];
        }
        startIndex = nextPoint;
        endIndex = isLast(nextPoint);
        const _abs = Math.abs(TrainListDist[endIndex] - TrainListDist[startIndex]);
        const _difference = currentPoint - TrainListDist[startIndex];
        const _min = Math.min(1, _difference / _abs);
        GetNextPosDebug = `endIndex - ${endIndex}(${JSON.stringify (TrainWayData[endIndex])}) - startIndex - ${startIndex}(${JSON.stringify (TrainWayData[startIndex])})`;
        const PosDiff = new mp.Vector3(TrainWayData[endIndex].x - TrainWayData[startIndex].x, TrainWayData[endIndex].y - TrainWayData[startIndex].y, TrainWayData[endIndex].z - TrainWayData[startIndex].z);
        
        const PosEnd = new mp.Vector3(PosDiff.x * _min, PosDiff.y * _min, PosDiff.z * _min);
            
        return new mp.Vector3(TrainWayData[startIndex].x + PosEnd.x, TrainWayData[startIndex].y + PosEnd.y, TrainWayData[startIndex].z + PosEnd.z);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/metro/index", GetNextPosDebug, e.toString());
        return new mp.Vector3();
    }
},
GetSlerp = (indexStation, currentPoint) => {
    let _index = 0, _quaternion1 = new Quaternion(), _quaternion2 = new Quaternion();
    const penultimateStation = (_indexStation => {
            _indexStation -= 1;
            if (_indexStation < 0)
                _indexStation = TrainWayData.length - 2;
            return _indexStation;
    })(indexStation), indexnextStation1 = isLast(indexStation), indexnextStation2 = isLast(indexnextStation1);
    if (currentPoint < 0)
        currentPoint += TrainListDist[TrainListDist.length - 1];
    const data = (currentPoint - TrainListDist[indexStation]) / (TrainListDist[indexnextStation1] - TrainListDist[indexStation]);
    if (data < 0.5) {
        _index = data + 0.5;
        _quaternion1 = _Quaternion.fromEuler(GetDistToPos(penultimateStation, indexStation), false);
        _quaternion2 = _Quaternion.fromEuler(GetDistToPos(indexStation, indexnextStation1), false)
    } else {
        _index = data - 0.5;
        _quaternion1 = _Quaternion.fromEuler(GetDistToPos(indexStation, indexnextStation1), false);
        _quaternion2 = _Quaternion.fromEuler(GetDistToPos(indexnextStation1, indexnextStation2), false);
    }
    return _Quaternion.slerp(_quaternion1, _quaternion2, _index);
},
GetDistToPos = (i1, i2) => {
    const 
        _pos = GetDist(TrainWayData[i2], TrainWayData[i1]),
        atan1 = Math.atan2(_pos.x, _pos.y),
        atan2 = Math.atan2(_pos.z, Math.sqrt(_pos.x * _pos.x + _pos.y * _pos.y));
    return new mp.Vector3(atan2, 0, -atan1);
},
GetDist = (i1, i2) => {
    const _pos = new mp.Vector3(i1.x - i2.x, i1.y - i2.y, i1.z - i2.z),
            _dist = 1 / (Math.sqrt(_pos.x * _pos.x + _pos.y * _pos.y + _pos.z * _pos.z) || 1);
    _pos.x *= _dist;
    _pos.y *= _dist;
    _pos.z *= _dist;
    return _pos;
};


let TrainListDist = [];
            

let lastPoint = 0;
const InitTrain = async () => {
    let dist = 0;
    for (let i = 0; i < TrainWayData.length; i++) {
        TrainListDist[i] = dist;
        const rPos = TrainWayData[i],
            nPos = TrainWayData[i + 1];
        if (i < TrainWayData.length - 1)
            dist += global.vdist2(rPos, nPos);
    }
    lastPoint = TrainListDist[TrainListDist.length - 1];
    await Promise.all(trainModels.map(model => global.loadModel(model)));
    worldTrainData.modelsReady = true;
    OnStartTrain(Date.now().toString());
};

setTimeout(() => {
    InitTrain ();
}, 1000)
const OnStartTrain = (trainData = Date.now().toString()) => {
	try
	{
        if ("string" != typeof trainData || !trainData.length || !worldTrainData.modelsReady)
            return;
        const _0x2aaa1f = Date.now();
        worldTrainData.lastUpdate = _0x2aaa1f;
        const _0x37203e = _0x2aaa1f - trainData / 1000;
        worldTrainData.currentPoint = _0x942cc * _0x37203e;
        worldTrainData.currentPoint = worldTrainData.currentPoint % lastPoint;
        if (worldTrainData.currentPoint < 0)
            worldTrainData.currentPoint += lastPoint;
        worldTrainData.dataReady = true;
     
        worldTrainData.blip = mp.blips.new(285, new mp.Vector3(), { alpha: 255, color: 75, name: translateText("Поезд") });
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/metro/index", "OnStartTrain", e.toString());
    }
};
gm.events.add("worldDataReady", () => {
    OnStartTrain();
});
gm.events.add("worldDataChanged", (time) => {
    OnStartTrain(time);
});

let renderTrainDebug = 0;
gm.events.add("render", () => {
	try
	{
        renderTrainDebug = 1;
        if (worldTrainData.nextFrameUnfreeze) {
            global.localplayer.freezePosition(false);
            worldTrainData.nextFrameUnfreeze = false;
        }
        renderTrainDebug = 2;
        if (worldTrainData.dataReady && worldTrainData.modelsReady) {
            const currentTime = Date.now(),
                _time = (currentTime - worldTrainData.lastUpdate) / 1000;
            renderTrainDebug = 3;
            worldTrainData.lastUpdate = currentTime;
            worldTrainData.currentPoint += _0x942cc * _time;
            worldTrainData.currentPoint = worldTrainData.currentPoint % lastPoint;
            renderTrainDebug = 4;
            if (worldTrainData.currentPoint < 0)
                worldTrainData.currentPoint += lastPoint;

            renderTrainDebug = 5;
            let _nextStation = null, _nextPos = null;
            if (!worldTrainData.lastUpdateStream || currentTime >= worldTrainData.lastUpdateStream + 1000) {
                renderTrainDebug = 6;
                worldTrainData.lastUpdateStream = currentTime;
                renderTrainDebug = 66;
                _nextStation = GetNextStation(worldTrainData.currentPoint);
                renderTrainDebug = 68;
                _nextPos = GetNextPos(worldTrainData.currentPoint, _nextStation);
                if (worldTrainData.blip)
                    worldTrainData.blip.setCoords(_nextPos);
                renderTrainDebug = 69;
                const _dist = global.vdist2(global.localplayer.position, _nextPos);
                //if (_dist > 500) global.localplayer.position = _nextPos;
                renderTrainDebug = 7;
                if (!worldTrainData.trainStreamed && _dist <= 500) {
                    worldTrainData.trainStreamed = true;
                    renderTrainDebug = 8;
                    (_pos => {
                        renderTrainDebug = 9;
                        Natives.DELETE_ALL_TRAINS();
                        const train  = mp.game.vehicle.createMissionTrain(14, _pos.x, _pos.y, _pos.z, true);
                        worldTrainData.train = train;
                        Natives.SET_MISSION_TRAIN_COORDS(train , _pos.x, _pos.y, _pos.z);
                        Natives.SET_TRAIN_SPEED(train , _0x942cc + 1e-9);
                        Natives.SET_TRAIN_CRUISE_SPEED(train , _0x942cc + 1e-9);
                        Natives.SWITCH_TRAIN_TRACK(0, true);
                        Natives.SWITCH_TRAIN_TRACK(3, true);
                        Natives.SET_ENTITY_AS_MISSION_ENTITY(train , true, true);
                        Natives.SET_ENTITY_INVINCIBLE(train , true);
                        renderTrainDebug = 10;
                        if ("number" == typeof mp.storage.data.currentTrainHash) {
                            Natives.DOES_ENTITY_EXIST(mp.storage.data.currentTrainHash);
                            mp.game.vehicle.deleteMissionTrain(mp.storage.data.currentTrainHash);
                            renderTrainDebug = 11;
                        }
                        mp.storage.data.currentTrainHash = train;
                        renderTrainDebug = 12;
                    })(_nextPos)
                } else if (worldTrainData.trainStreamed && _dist > 500) {
                    renderTrainDebug = 13;
                    worldTrainData.trainStreamed = false
                    if ("number" == typeof worldTrainData.train) {
                        renderTrainDebug = 14;
                        Natives.DOES_ENTITY_EXIST(worldTrainData.train);
                        mp.game.vehicle.deleteMissionTrain(worldTrainData.train);
                        worldTrainData.train = null;
                        mp.storage.data.currentTrainHash = null
                        renderTrainDebug = 15;
                    }
                }
            }
            renderTrainDebug = 16;
            if (worldTrainData.trainStreamed && worldTrainData.train && (!worldTrainData.lastUpdatePos || currentTime >= worldTrainData.lastUpdatePos + 1000)) {
                worldTrainData.lastUpdatePos = currentTime;
                renderTrainDebug = 17;
                if ("number" != typeof _nextStation) {
                    _nextStation = GetNextStation(worldTrainData.currentPoint);
                    _nextPos = GetNextPos(worldTrainData.currentPoint, _nextStation);
                }
                if (worldTrainData.blip)
                    worldTrainData.blip.setCoords(_nextPos);
                renderTrainDebug = 18;
                const _getSlerp = GetSlerp(_nextStation, worldTrainData.currentPoint);
                global.localplayer.getConfigFlag(138, true);
                global.localplayer.freezePosition(true);
                worldTrainData.nextFrameUnfreeze = true;
                renderTrainDebug = 19;
                Natives.SET_MISSION_TRAIN_COORDS(worldTrainData.train, _nextPos.x, _nextPos.y, _nextPos.z);
                Natives.SET_ENTITY_QUATERNION(worldTrainData.train, _getSlerp.x, _getSlerp.y, _getSlerp.z, _getSlerp.w);
                Natives.SET_TRAIN_SPEED(worldTrainData.train, _0x942cc + 1e-9);
                Natives.SET_TRAIN_CRUISE_SPEED(worldTrainData.train, _0x942cc + 1e-9);
                renderTrainDebug = 20;
            }
        }
        renderTrainDebug = 21;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/metro/index", "render - " + renderTrainDebug, e.toString());
    }
});