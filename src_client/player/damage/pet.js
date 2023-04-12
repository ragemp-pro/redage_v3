/*mp._events.add('outgoingDamage', (sourceEntity, targetEntity, sourcePlayer, weapon, boneIndex, damage) => {
    if (damage < 0)
        return;
    else if (sourceEntity && sourceEntity.type === "ped") {
        if (targetEntity && targetEntity.type === "player")
            mp.events.callRemote("server.damage.petToPlayer", targetEntity, sourceEntity, boneIndex);
        else if (targetEntity && targetEntity.type === "ped" && targetEntity.isPet) {
            mp.events.callRemote("server.damage.petToPet", targetEntity, sourceEntity, boneIndex);
            return true;
        }
    } else if (targetEntity && targetEntity.type === "ped" && targetEntity.isPet) {
        mp.events.callRemote("server.damage.playerToPet", targetEntity, boneIndex, 1.0);
        return true;
    }
});


import boneIdGtaToRage from './boneIdGtaToRage'

const getPointToBoneOffset = (targetX, targetY, targetZ, boneX, boneY, boneZ) => {
    const x = targetX - boneX < 0 ? -(targetX - boneX) : targetX - boneX;
    const y = targetY - boneY < 0 ? -(targetY - boneY) : targetY - boneY;
    const z = targetZ - boneZ < 0 ? -(targetZ - boneZ) : targetZ - boneZ;
    return x + y + z;
}

const onSuccessShoot = (targetPosition, targetEntity, flags = 31) => {
    const gameplayCam = global.cameraManager.gameplayCam(),
        getDirection = gameplayCam.getDirection(),
        getCoord = gameplayCam.getCoord(),
        worldCoord = new mp.Vector3(getCoord.x + 200 * getDirection.x,
            getCoord.y + 200 * getDirection.y,
            getCoord.z + 200 * getDirection.z),
        getBoneCoords = global.localplayer.getBoneCoords(26610, 0, 0, 0),
        result = mp.raycasting.testPointToPoint(getCoord, worldCoord, global.localplayer, flags);

    let resultPosition = worldCoord,
        resultEntity = null;

    if (result) {
        resultPosition = new mp.Vector3(result.position.x, result.position.y, result.position.z);
        if (flags === (4 | 8)) {
            if (result.entity && "number" == typeof result.entity) {
                const entity = mp.peds.atHandle(result.entity);
                if (entity && entity.handle !== 0) {
                    resultPosition = resultPosition;
                    resultEntity = entity;
                }
            }
        } else {
            if (result.entity && "number" != typeof result.entity && "player" === result.entity.type) {
                const newResult = mp.raycasting.testPointToPoint(getBoneCoords, resultPosition, global.localplayer, flags);
                if (newResult) {
                    if (newResult.entity && "number" == typeof newResult.entity || "player" !== newResult.entity.type) {
                        resultEntity = null;
                        resultPosition = newResult.position;
                    } else
                        resultEntity = result.entity
                }
                else
                    resultEntity = result.entity
            }

            if (result.entity && "number" != typeof result.entity && "vehicle" === result.entity.type) {
                resultPosition = targetPosition;
                resultEntity = targetEntity;
            }
        }
    }
    return {
        resultPosition: resultPosition,
        resultEntity: resultEntity,
    };
}

gm.events.add('playerWeaponShot', (targetPosition, targetEntity) => {
    const result = onSuccessShoot (targetPosition, targetEntity, 4 | 8);

    targetPosition = result.resultPosition;
    targetEntity = result.resultEntity;

    if (targetEntity && targetEntity.type === "ped") {
        let boneIndex = -1;
        let lastBoneOffset = 9999;

        for (let key in boneIdGtaToRage) {
            if (boneIdGtaToRage [key]) {
                const boneCoords = targetEntity.getBoneCoords(boneIdGtaToRage [key], 0, 0, 0);
                const boneOffset = getPointToBoneOffset(targetPosition.x, targetPosition.y, targetPosition.z, boneCoords.x, boneCoords.y, boneCoords.z);
                if (boneOffset < lastBoneOffset) {
                    boneIndex = key;
                    lastBoneOffset = boneOffset;
                }
            }
        }
        if (boneIndex != -1) {
            const position = global.localplayer.position;
            mp.events.callRemote("server.damage.playerToPet", targetEntity, boneIndex, getDist (position, targetPosition));
        }
        return;
    }
});*/