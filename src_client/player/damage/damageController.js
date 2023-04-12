export default new class {
    constructor() {
        this.parts = {};

        /*gm.events.add("weapons.client.giveDamage", (_0x49e961, _0x4808bf, _0xe52a03, _0x44fec3, _0x1482dc, _0x22cc74, _0x4e00b5, _0x210d42, _0x383d53, _0x375d13) => {
            if (_0x3c4cad.greenZoneController.inGreenZone || _0x54e26f.noDamageEnabled || mp.players.local.godmode || mp.players.local.localGodMode || mp.players.local.getVariable("isSpectating"))
                return;
            _0x49e961 > 0 && mp.players.local.applyDamageTo(_0x49e961 + 0x1, true);
            const _0x5a8399 = parseInt(_0x383d53, 0x24);
            mp.game.gameplay.shootSingleBulletBetweenCoords(_0x4808bf, _0xe52a03, _0x44fec3, _0x1482dc, _0x22cc74, _0x4e00b5, 0, truex1, _0x5a8399, -1, true, truex1, 100);
        });

        gm.events.add("weapons.client.showHitmarker", (_0x206f3c, _0x3cad28, _0x4bbe86, _0x4a29cc, _0x5df976) => {
            this.setHitmarker(_0x5df976);
        });

        gm.events.add("weapons.client.playHeadshot", () => {
            this.playHeadshot();
        });*/
    }

    onSuccessShoot(targetPosition, targetEntity) {
        const gameplayCam = global.cameraManager.gameplayCam(),
            getDirection = gameplayCam.getDirection(),
            getCoord = gameplayCam.getCoord();

        let worldCoord = new mp.Vector3(getCoord.x + 200 * getDirection.x, getCoord.y + 200 * getDirection.y, getCoord.z + 200 * getDirection.z);

        const getBoneCoords = mp.players.local.getBoneCoords(26610, 0, 0, 0),
            result = mp.raycasting.testPointToPoint(getCoord, worldCoord, mp.players.local, 31);

        let entity = null;
        if (result) {
            worldCoord = new mp.Vector3(result.position.x, result.position.y, result.position.z);

            if (result.entity && "number" != typeof result.entity && "player" === result.entity.type) {
                const resultPlayer = mp.raycasting.testPointToPoint(getBoneCoords, worldCoord, mp.players.local, 31);
                if (resultPlayer) {
                    if ("number" == typeof resultPlayer.entity || "player" !== resultPlayer.entity.type) {
                        entity = null;
                        worldCoord = resultPlayer.position;
                    } else
                        entity = result.entity
                } else
                    entity = result.entity
            }
            if (result.entity && "number" != typeof result.entity && "vehicle" === result.entity.type) {
                worldCoord = targetPosition
                entity = targetEntity
            }
        }
        if (worldCoord && entity && "player" === entity.type) {
            if (!mp.players.local.isShooting() && "0.2 beta" == mp.version.core)
                return;

            this.getHitBone(worldCoord, entity);
        }
    }


    getBoneDamageMultiplier(boneId) {
        const parts = this.parts[boneId];
        return parts ? parts.damageMultiplier : 1;
    }

    addBone(boneId, damageMultiplier) {
        const data = {};
        data.damageMultiplier = damageMultiplier;
        this.parts[boneId] = data;
    }
}