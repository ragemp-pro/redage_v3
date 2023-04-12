const weaponCategory = {
    none: 0,
    melee: 1,
    pistol: 2,
    shotgun: 3,
    smg: 4,
    assaultRifle: 5,
    sniperRifle: 6,
    extra: 7
}

export default {
    [mp.game.joaat('weapon_dagger')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_bat')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_bottle')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_crowbar')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_unarmed')]: {
        damage: 9,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_flashlight')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_golfclub')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_hammer')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_hatchet')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_knuckle')]: {
        damage: 27,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_knife')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_machete')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_switchblade')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_nightstick')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_wrench')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_battleaxe')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_poolcue')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_stone_hatchet')]: {
        damage: 33,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        category: weaponCategory.melee
    },
    [mp.game.joaat('weapon_pistol')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_pistol_mk2')]: {
        damage: 11,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_combatpistol')]: {
        damage: 11,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_appistol')]: {
        damage: 7,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_stungun')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        isStungun: true,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_flaregun')]: {
        damage: 5,
        distance: 129,
        shotCount: 1,
        isProjectile: true,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_pistol50')]: {
        damage: 15,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_snspistol')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_snspistol_mk2')]: {
        damage: 11,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_heavypistol')]: {
        damage: 12,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_vintagepistol')]: {
        damage: 12,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_revolver')]: {
        damage: 50,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_revolver_mk2')]: {
        damage: 57,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_doubleaction')]: {
        damage: 29,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_marksmanpistol')]: {
        damage: 60,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_ceramicpistol')]: {
        damage: 15,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_navyrevolver')]: {
        damage: 55,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_gadgetpistol')]: {
        damage: 65,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_glockp80')]: {
        damage: 15,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.pistol
    },
    [mp.game.joaat('weapon_pumpshotgun')]: {
        damage: 22,
        distance: 30,
        shotCount: 8,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_pumpshotgun_mk2')]: {
        damage: 26,
        distance: 40,
        shotCount: 8,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_sawnoffshotgun')]: {
        damage: 25,
        distance: 30,
        shotCount: 8,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_assaultshotgun')]: {
        damage: 13,
        distance: 30,
        shotCount: 6,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_bullpupshotgun')]: {
        damage: 28,
        distance: 30,
        shotCount: 8,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_dbshotgun')]: {
        damage: 50,
        distance: 20,
        shotCount: 12,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_heavyshotgun')]: {
        damage: 33,
        distance: 75,
        shotCount: 1,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_autoshotgun')]: {
        damage: 20,
        distance: 30,
        shotCount: 6,
        category: weaponCategory.shotgun
    },
    [mp.game.joaat('weapon_musket')]: {
        damage: 60,
        distance: 100,
        shotCount: 1,
        category: weaponCategory.shotgun
    },

    [mp.game.joaat('weapon_microsmg')]: {
        damage: 6,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_smg')]: {
        damage: 6,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_smg_mk2')]: {
        damage: 8,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_combatpdw')]: {
        damage: 7,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_machinepistol')]: {
        damage: 7,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_minismg')]: {
        damage: 7,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_assaultsmg')]: {
        damage: 8,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_assaultrifle')]: {
        damage: 8,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_assaultrifle_mk2')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_carbinerifle')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_carbinerifle_mk2')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_advancedrifle')]: {
        damage: 8,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_specialcarbine')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_specialcarbine_mk2')]: {
        damage: 10,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_bullpuprifle')]: {
        damage: 8,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_bullpuprifle_mk2')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_compactrifle')]: {
        damage: 8,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_militaryrifle')]: {
        damage: 10,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_mg')]: {
        damage: 12,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_combatmg')]: {
        damage: 13,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_combatmg_mk2')]: {
        damage: 14,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_gusenberg')]: {
        damage: 9,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.smg
    },
    [mp.game.joaat('weapon_sniperrifle')]: {
        damage: 50,
        distance: 300,
        shotCount: 1,
        isSniper: true,
        category: weaponCategory.sniperRifle
    },
    [mp.game.joaat('weapon_heavysniper')]: {
        damage: 70,
        distance: 300,
        shotCount: 1,
        isSniper: true,
        category: weaponCategory.sniperRifle
    },
    [mp.game.joaat('weapon_heavysniper_mk2')]: {
        damage: 105,
        distance: 300,
        shotCount: 1,
        isSniper: true,
        category: weaponCategory.sniperRifle
    },
    [mp.game.joaat('weapon_marksmanrifle')]: {
        damage: 25,
        distance: 300,
        shotCount: 1,
        category: weaponCategory.sniperRifle
    },
    [mp.game.joaat('weapon_marksmanrifle_mk2')]: {
        damage: 33,
        distance: 300,
        shotCount: 1,
        category: weaponCategory.sniperRifle
    },
    [mp.game.joaat('weapon_rpg')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_grenadelauncher')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_grenadelauncher_smoke')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        isProjectile: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_minigun')]: {
        damage: 20,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_firework')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_railgun')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_hominglauncher')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_compactlauncher')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_rayminigun')]: {
        damage: 20,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_fireextinguisher')]: {
        damage: 0,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_snowball')]: {
        damage: 1,
        distance: 25,
        shotCount: 1,
        isProjectile: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_ball')]: {
        damage: 1,
        distance: 25,
        shotCount: 1,
        isProjectile: true,
        category: weaponCategory.extra
    },
    //

    [mp.game.joaat('a_c_chop')]: {
        damage: 20,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('a_c_husky')]: {
        damage: 20,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('a_c_poodle')]: {
        damage: 20,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('a_c_pug')]: {
        damage: 10,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('a_c_rottweiler')]: {
        damage: 20,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('a_c_shepherd')]: {
        damage: 20,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('a_c_westy')]: {
        damage: 10,
        distance: 5,
        shotCount: 1,
        isMelee: true,
        isPet: true,
        category: weaponCategory.extra
    },
    [mp.game.joaat('weapon_tacticalrifle')]: {
        damage: 10,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_heavyrifle')]: {
        damage: 10,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.assaultRifle
    },
    [mp.game.joaat('weapon_precisionrifle')]: {
        damage: 43,
        distance: 129,
        shotCount: 1,
        category: weaponCategory.sniperRifle
    },
    [mp.game.joaat('weapon_combatshotgun')]: {
        damage: 17,
        distance: 35,
        shotCount: 6,
        category: weaponCategory.shotgun
    },
};