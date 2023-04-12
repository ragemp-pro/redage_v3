/*_3990614 = [
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_HELI_CRASH")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_STINGER")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_RAILGUN")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_AIRSTRIKE_ROCKET")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_PASSENGER_ROCKET")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_VEHICLE_ROCKET")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("WEAPON_AIR_DEFENCE_GUN")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("VEHICLE_WEAPON_TANK")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("VEHICLE_WEAPON_PLAYER_LASER")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("VEHICLE_WEAPON_PLAYER_LAZER")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("VEHICLE_WEAPON_PLAYER_BULLET")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("VEHICLE_WEAPON_PLAYER_BUZZARD")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("VEHICLE_WEAPON_PLAYER_HUNTER")
    },
    {
        'type': "forbid",
        'hash': mp.game.joaat("weapon_cougar")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_rpg")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_grenadelauncher")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_grenadelauncher_smoke")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_minigun")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_firework")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_hominglauncher")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_compactlauncher")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_rayminigun")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_raypistol")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_raycarbine")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_pipebomb")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_stickybomb")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_proxmine")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_flaregun")
    },
    {
        'type': "check",
        'hash': mp.game.joaat("weapon_grenade")
    }
];*/

const vehicleWeapons = [
    "vehicle_weapon_player_lazer",
    "VEHICLE_WEAPON_PLAYER_BULLET",
    "VEHICLE_WEAPON_PLAYER_HUNTER",
    "vehicle_weapon_akula_barrage",
    "vehicle_weapon_akula_minigun",
    "vehicle_weapon_akula_missile",
    "vehicle_weapon_akula_turret_dual",
    "vehicle_weapon_akula_turret_single",
    "vehicle_weapon_apc_cannon",
    "vehicle_weapon_apc_mg",
    "vehicle_weapon_apc_missile",
    "vehicle_weapon_ardent_mg",
    "vehicle_weapon_avenger_cannon",
    "vehicle_weapon_barrage_rear_gl",
    "vehicle_weapon_barrage_rear_mg",
    "vehicle_weapon_barrage_rear_minigun",
    "vehicle_weapon_barrage_top_mg",
    "vehicle_weapon_barrage_top_minigun",
    "vehicle_weapon_bombushka_cannon",
    "vehicle_weapon_bombushka_dualmg",
    "vehicle_weapon_cannon_blazer",
    "vehicle_weapon_caracara_mg",
    "vehicle_weapon_caracara_minigun",
    "vehicle_weapon_cherno_missile",
    "vehicle_weapon_comet_mg",
    "vehicle_weapon_deluxo_mg",
    "vehicle_weapon_deluxo_missile",
    "vehicle_weapon_dogfighter_mg",
    "vehicle_weapon_dogfighter_missile",
    "vehicle_weapon_dune_grenadelauncher",
    "vehicle_weapon_dune_mg",
    "vehicle_weapon_dune_minigun",
    "vehicle_weapon_enemy_laser",
    "vehicle_weapon_hacker_missile",
    "vehicle_weapon_hacker_missile_homing",
    "vehicle_weapon_halftrack_dualmg",
    "vehicle_weapon_halftrack_quadmg",
    "vehicle_weapon_havok_minigun",
    "vehicle_weapon_hunter_barrage",
    "vehicle_weapon_hunter_cannon",
    "vehicle_weapon_hunter_mg",
    "vehicle_weapon_hunter_missile",
    "vehicle_weapon_insurgent_minigun",
    "vehicle_weapon_khanjali_cannon",
    "vehicle_weapon_khanjali_cannon_heavy",
    "vehicle_weapon_khanjali_gl",
    "vehicle_weapon_khanjali_mg",
    "vehicle_weapon_menacer_mg",
    "vehicle_weapon_microlight_mg",
    "vehicle_weapon_mobileops_cannon",
    "vehicle_weapon_mogul_dualnose",
    "vehicle_weapon_mogul_dualturret",
    "vehicle_weapon_mogul_nose",
    "vehicle_weapon_mogul_turret",
    "vehicle_weapon_mule4_mg",
    "vehicle_weapon_mule4_missile",
    "vehicle_weapon_mule4_turret_gl",
    "vehicle_weapon_nightshark_mg",
    "vehicle_weapon_nose_turret_valkyrie",
    "vehicle_weapon_oppressor_mg",
    "vehicle_weapon_oppressor_missile",
    "vehicle_weapon_oppressor2_cannon",
    "vehicle_weapon_oppressor2_mg",
    "vehicle_weapon_oppressor2_missile",
    "vehicle_weapon_plane_rocket",
    "vehicle_weapon_player_buzzard",
    "vehicle_weapon_player_lazer",
    "vehicle_weapon_player_savage",
    "vehicle_weapon_pounder2_barrage",
    "vehicle_weapon_pounder2_gl",
    "vehicle_weapon_pounder2_mini",
    "vehicle_weapon_pounder2_missile",
    "vehicle_weapon_revolter_mg",
    "vehicle_weapon_rogue_cannon",
    "vehicle_weapon_rogue_mg",
    "vehicle_weapon_rogue_missile",
    "vehicle_weapon_ruiner_bullet",
    "vehicle_weapon_ruiner_rocket",
    "vehicle_weapon_savestra_mg",
    "vehicle_weapon_scramjet_mg",
    "vehicle_weapon_scramjet_missile",
    "vehicle_weapon_seabreeze_mg",
    "vehicle_weapon_searchlight",
    "vehicle_weapon_space_rocket",
    "vehicle_weapon_speedo4_mg",
    "vehicle_weapon_speedo4_turret_mg",
    "vehicle_weapon_speedo4_turret_mini",
    "vehicle_weapon_strikeforce_barrage",
    "vehicle_weapon_strikeforce_cannon",
    "vehicle_weapon_strikeforce_missile",
    "vehicle_weapon_subcar_mg",
    "vehicle_weapon_subcar_missile",
    "vehicle_weapon_subcar_torpedo",
    "vehicle_weapon_tampa_dualminigun",
    "vehicle_weapon_tampa_fixedminigun",
    "vehicle_weapon_tampa_missile",
    "vehicle_weapon_tampa_mortar",
    "vehicle_weapon_tank",
    "vehicle_weapon_technical_minigun",
    "vehicle_weapon_thruster_mg",
    "vehicle_weapon_thruster_missile",
    "vehicle_weapon_trailer_dualaa",
    "vehicle_weapon_trailer_missile",
    "vehicle_weapon_trailer_quadmg",
    "vehicle_weapon_tula_dualmg",
    "vehicle_weapon_tula_mg",
    "vehicle_weapon_tula_minigun",
    "vehicle_weapon_tula_nosemg",
    "vehicle_weapon_turret_boxville",
    "vehicle_weapon_turret_insurgent",
    "vehicle_weapon_turret_limo",
    "vehicle_weapon_turret_technical",
    "vehicle_weapon_turret_valkyrie",
    "vehicle_weapon_vigilante_mg",
    "vehicle_weapon_vigilante_missile",
    "vehicle_weapon_viseris_mg",
    "vehicle_weapon_volatol_dualmg",
    3405172033
];

vehicleWeapons.forEach((weapon) => {
    mp.game.weapon.addWeaponModelBlacklist(("number" == typeof weapon ? weapon : mp.game.joaat(weapon)) >> 0)
})

const meleeWeapons = [
    "weapon_knife",
];

meleeWeapons.forEach((weapon) => {
    mp.game.weapon.addWeaponModelBlacklist(("number" == typeof weapon ? weapon : mp.game.joaat(weapon)) >> 0)
})