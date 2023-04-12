let sendShootTimeout = null,
    sendShootCount = 0,
    lastSpawnTime = -1;
const WEAPON_UNARMED = mp.game.joaat("weapon_unarmed"),
    WEAPON_STUNGAN = mp.game.joaat("weapon_stungun"),
    WEAPON_FIREEXTINGUISHER = mp.game.joaat("weapon_fireextinguisher"),
    WEAPON_GRENADELAUNCHER_SMOKE = mp.game.joaat(
        "weapon_grenadelauncher_smoke"
    ),
    WEAPON_SNOWBALL = mp.game.joaat("weapon_snowball"),
    WEAPON_SHOOTGUN = [
        mp.game.joaat("weapon_bullpupshotgun"),
        mp.game.joaat("weapon_dbshotgun"),
        mp.game.joaat("weapon_pumpshotgun"),
        mp.game.joaat("weapon_pumpshotgun_mk2"),
        mp.game.joaat("weapon_assaultshotgun"),
        mp.game.joaat("weapon_heavyshotgun"),
        mp.game.joaat("weapon_musket"),
    ],
    WEAPON_MELEE = [
        mp.game.joaat("weapon_dagger"),
        mp.game.joaat("weapon_bat"),
        mp.game.joaat("weapon_bottle"),
        mp.game.joaat("weapon_crowbar"),
        mp.game.joaat("weapon_flashlight"),
        mp.game.joaat("weapon_golfclub"),
        mp.game.joaat("weapon_hammer"),
        mp.game.joaat("weapon_hatchet"),
        mp.game.joaat("weapon_knuckle"),
        mp.game.joaat("weapon_knife"),
        mp.game.joaat("weapon_machete"),
        mp.game.joaat("weapon_switchblade"),
        mp.game.joaat("weapon_nightstick"),
        mp.game.joaat("weapon_wrench"),
        mp.game.joaat("weapon_battleaxe"),
        mp.game.joaat("weapon_poolcue"),
        mp.game.joaat("weapon_stone_hatchet"),
        WEAPON_FIREEXTINGUISHER,
    ],
    WEAPON_PROJECTILE = [WEAPON_GRENADELAUNCHER_SMOKE, WEAPON_SNOWBALL],
    WEAPON_CANCEL_REMOVE_LIST = [WEAPON_SNOWBALL],
    WEAPON_BLOCK_LIST = [];

mp._events.add("incomingDamage", (sourceEntity, sourcePlayer, targetEntity, weapon, boneIndex) => {
    if (targetEntity === global.localplayer) {
        if (lastSpawnTime + 1500 > new Date().getTime()) return !0;
        if (WEAPON_UNARMED === weapon)
            return void mp.game.weapon.setCurrentDamageEventAmount(
                sourcePlayer instanceof mp.Ped && sourcePlayer.isDynamic && sourcePlayer.serverPed
                    ? sourcePlayer.serverPed.weaponUnarmedDamage
                    : 0 === playerInGreenZone
                        ? 10
                        : 0
            );
        if (-1 !== WEAPON_MELEE.indexOf(weapon))
            return void mp.game.weapon.setCurrentDamageEventAmount(
                weapon === WEAPON_FIREEXTINGUISHER ? 0 : 0 === playerInGreenZone ? 15 : 0
            );
        if (
            (mp.game.weapon.setCurrentDamageEventAmount(0),
                mp.game.weapon.setCurrentDamageEventCritical(!1),
            sourcePlayer instanceof mp.Ped && sourcePlayer.isDynamic && sourcePlayer.serverPed)
        ) {
            if (
                -1 !== WEAPON_SHOOTGUN.indexOf(weapon) &&
                !global.actionAntiFlood("shoot_sg", 50)
            )
                return !0;
            const { x: a, y: c, z: f } = global.localplayer.position,
                { x: g, y: h, z: i } = sourcePlayer.getCoords(!0);
            mp.events.callRemoteUnreliable(
                "server_ped_damage_in",
                sourcePlayer.serverPed.id,
                "" + weapon,
                boneIndex,
                mp.dist(g, h, i, a, c, f)
            );
        }
        if (weapon === WEAPON_STUNGAN) {
            if (!global.actionAntiFlood("incomingDamage_stun", 1500)) return !0;
            if (2 > global.localplayer.getHealth())
                return global.localplayer.setToRagdoll(4e3, 4e3, 0, !1, !1, !1), !0;
            global.mainBrowser.execute(
                "for (let i = 0; i < 15; i++) UI_effect_drugs();"
            );
        }
    } else
        targetEntity instanceof mp.Ped &&
        targetEntity.isDynamic &&
        (mp.game.weapon.setCurrentDamageEventAmount(0),
            mp.game.weapon.setCurrentDamageEventCritical(!1));
})
mp._events.add("outgoingDamage", (sourceEntity, targetEntity, sourcePlayer, weapon, boneIndex, damage) => {
    if (sourceEntity !== global.localplayer) {
        if (
            targetEntity === global.localplayer &&
            sourceEntity instanceof mp.Ped &&
            sourceEntity.isDynamic &&
            sourceEntity.serverPed
        )
            if (weapon === WEAPON_UNARMED)
                mp.game.weapon.setCurrentDamageEventAmount(
                    sourceEntity.serverPed.weaponUnarmedDamage
                );
            else if (-1 !== WEAPON_MELEE.indexOf(weapon))
                mp.game.weapon.setCurrentDamageEventAmount(15);
            else {
                if (
                    -1 !== WEAPON_SHOOTGUN.indexOf(weapon) &&
                    !global.actionAntiFlood("shoot_sg", 50)
                )
                    return !0;
                const { x: b, y: c, z: f } = global.localplayer.position,
                    { x: g, y: h, z: i } = sourceEntity.getCoords(!0);
                mp.events.callRemoteUnreliable(
                    "server_ped_damage_in",
                    sourceEntity.serverPed.id,
                    "" + weapon,
                    boneIndex,
                    mp.dist(g, h, i, b, c, f)
                );
            }
        return;
    }
    if ("player" === targetEntity.type) {
        if (weapon === WEAPON_UNARMED)
            return void mp.game.weapon.setCurrentDamageEventAmount(10);
        if (-1 !== WEAPON_MELEE.indexOf(weapon))
            return void mp.game.weapon.setCurrentDamageEventAmount(15);
        if (
            (mp.game.weapon.setCurrentDamageEventAmount(0),
                mp.game.weapon.setCurrentDamageEventCritical(!1),
            weapon === currentWeapon)
        ) {
            if (
                -1 !== WEAPON_SHOOTGUN.indexOf(weapon) &&
                !global.actionAntiFlood("shoot_sg", 50)
            )
                return !0;
            const a = targetEntity.getVariable("rsd");
            if (null == a) return !0;
            const c =
                20 === boneIndex
                    ? 0
                    : 19 === boneIndex ||
                    10 === boneIndex ||
                    9 === boneIndex ||
                    8 === boneIndex ||
                    7 === boneIndex ||
                    0 === boneIndex ||
                    15 === boneIndex ||
                    11 === boneIndex
                        ? 1
                        : 2;
            100 < playerInStreamCount
                ? (sendShootCount++,
                    3 <= sendShootCount
                        ? (mp.events.callRemoteUnreliable(
                            "spw",
                            targetEntity,
                            c,
                            a,
                            sendShootCount
                        ),
                        null !== sendShootTimeout && clearTimeout(sendShootTimeout),
                            (sendShootTimeout = null),
                            (sendShootCount = 0))
                        : (null !== sendShootTimeout && clearTimeout(sendShootTimeout),
                            (sendShootTimeout = setTimeout(() => {
                                mp.events.callRemoteUnreliable(
                                    "spw",
                                    targetEntity,
                                    c,
                                    a,
                                    sendShootCount
                                ),
                                    (sendShootTimeout = null),
                                    (sendShootCount = 0);
                            }, 199))))
                : (mp.events.callRemoteUnreliable("spw", targetEntity, c, a),
                null !== sendShootTimeout && clearTimeout(sendShootTimeout),
                    (sendShootTimeout = null),
                    (sendShootCount = 0));
        }
        return;
    }
    if (targetEntity instanceof mp.Ped && targetEntity.isDynamic && targetEntity.serverPed) {
        if (
            -1 !== WEAPON_SHOOTGUN.indexOf(weapon) &&
            !global.actionAntiFlood("shoot_sg", 50)
        )
            return !0;
        mp.game.weapon.setCurrentDamageEventAmount(0),
            mp.game.weapon.setCurrentDamageEventCritical(!1);
        const { x: a, y: c, z: f } = global.localplayer.position,
            { x: g, y: h, z: i } = targetEntity.getCoords(!0);
        return void mp.events.callRemoteUnreliable(
            "server_ped_damage_out",
            targetEntity.serverPed.id,
            boneIndex,
            mp.dist(g, h, i, a, c, f)
        );
    }
    if (!(150 <= damage && "vehicle" === targetEntity.type && weapon === WEAPON_UNARMED)) return;
    mp.game.weapon.setCurrentDamageEventAmount(0);
    const g = targetEntity.getCoords(!0),
        h = Date.now(),
        i = setInterval(() => {
            if (Date.now() > h + 5e3 || 0 >= targetEntity.handle || !mp.vehicles.exists(targetEntity))
                return void clearInterval(i);
            const a = mp.dist(
                g.x,
                g.y,
                g.z,
                targetEntity.position.x,
                targetEntity.position.y,
                targetEntity.position.z
            );
            2.5 <= a &&
            (clearInterval(i),
                targetEntity.setCoords(g.x, g.y, g.z, !1, !1, !1, !1),
                mp.events.callRemote("s_ac_veh_damage"));
        }, 0);
})

const resetDamageBlock = () => {
    global.localplayer.setProofs(
        !1,
        !1,
        !1,
        !(global.localplayer.isInAir() || global.localplayer.isFalling()),
        !1,
        !1,
        !1,
        !1
    ),
        mp.game.player.resetStamina(),
        global.localplayer.setConfigFlag(429, !0),
        global.localplayer.setConfigFlag(241, !0),
        mp.game.player.setWeaponDamageModifier(-999999);
};
setInterval(resetDamageBlock, 2500),
    gm.events.add("explosion", () => !0),
    gm.events.add("projectile", (a, b) => -1 === WEAPON_PROJECTILE.indexOf(b));