import './hit';
import './pet';
//import './blackList';

import customDamagesData from './customDamagesData'


const damagesData = {}
for (const weaponHash in customDamagesData)
    damagesData[weaponHash >> 0] = customDamagesData[weaponHash];

import damageController from './damageController'

damageController.addBone(0,			    0.90); // 0 - таз
damageController.addBone(11816,          0.90); // 1 - нога
damageController.addBone(58271,          0.85); // 2 - голень
damageController.addBone(63931,          0.60); // 3 - ступня
damageController.addBone(14201,          0.90); // 4 - нога
damageController.addBone(2108,           0.85); // 5 - голень
damageController.addBone(65245,          0.60); // 6 - ступня
damageController.addBone(57717,	        0.95); // 7 - живот-таз
damageController.addBone(64157,		    1); // 8 - живот
damageController.addBone(60734,			1); // 9 - грудь
damageController.addBone(4115, 		    1.15); // 10 - грудь-горло
damageController.addBone(46078,		    1.10); // 11 - правое плечо
damageController.addBone(50201,		    0.95); // 12 - правое предплечье
damageController.addBone(24589,		    0.85); // 13 - правая рука
damageController.addBone(51826,		    0.60); // 14 - правая ладонь
damageController.addBone(36864,			1.10); // 15 - левое плечо
damageController.addBone(52301,			0.95); // 16 - левое предплечье
damageController.addBone(20781,			0.85); // 17 - левая рука
damageController.addBone(35502,			0.60); // 18 - левая ладонь
damageController.addBone(24806,			1.20); // 19 - горло
damageController.addBone(34414,			1.25); // 20 - голова

import boneIdGtaToRage from './boneIdGtaToRage'

gm.events.add('outgoingDamage', (sourceEntity, targetEntity, sourcePlayer, weapon, boneIndex, damage) => {

    if (global.cuffed || global.isDeath === true || global.isDemorgan || global.dmgdisabled || global.isInSafeZone)
        return true;

    if (targetEntity && targetEntity.type === "player" && mp.players.exists(targetEntity)) {
        if (global.localplayer['PlayerAirsoftTeam'] && global.localplayer['PlayerAirsoftTeam'] >= 0 && global.localplayer['PlayerAirsoftTeam'] == targetEntity['PlayerAirsoftTeam'])
            return true;
        else if (targetEntity['AGM'])
            return true;
        else if (targetEntity['InDeath'])
            return true;

        if (isZoneAirDrop) {
            const playerOrganization = targetEntity['organization'];
            if (global.organizationId != 0 && playerOrganization != 0 && global.organizationId === playerOrganization)
                return true;
        }

        ///Hit
        if (global.hitPoint) {

            const weaponHash = weapon >> 0;

            let dist = global.vdist2(global.localplayer.position, targetEntity.position, true);
            let customDamage = damage;

            const rageBoneId = boneIdGtaToRage[boneIndex],
                damageData = damagesData[weaponHash];

            if (damageData/* && damageData.distance >= dist*/) {
                const boneDamageMultiplier = damageController.getBoneDamageMultiplier(rageBoneId),
                    shotCount = damageData.shotCount;

                //customDamage = global.clamp(damageData.damage / (damageData.isSniper ? 1 : (dist / 40)), damageData.damage / 2, damageData.damage) * boneDamageMultiplier / shotCount;
                customDamage = global.clamp(damageData.damage, damageData.damage / 2, damageData.damage) * boneDamageMultiplier / shotCount;
            }

            const getHealth = targetEntity.getHealth(),
                getArmour = targetEntity.getArmour(),
                healthAndArmour = getHealth + getArmour;

            mp.events.call("client.addHit", Math.round(customDamage), targetEntity.remoteId, rageBoneId, !!headBone.includes(boneIndex), !!(healthAndArmour - customDamage <= 0));
        }
    }

    if (sourcePlayer && "player" === sourcePlayer.type)
        mp.game.weapon.cancelCurrentDamageEvent();
});

const headBone = [
    20
]

const clearArmour = () => {
    if (global.localplayer.getArmour() <= 0 && global.isArmor === true) {
        mp.events.callRemote('deletearmor');
        global.isArmor = false;
    }
}

mp._events.add('incomingDamage', (sourceEntity, sourcePlayer, targetEntity, weapon, boneIndex, damage) => {
    if (targetEntity === mp.players.local) {
        if (global.isDeath === true || global.admingm || global.isInSafeZone)
            return true;

        const weaponHash = weapon >> 0;
        if (weaponHash == 911657153)//Stungun
            return true;

        if (mp.players.exists(sourcePlayer)) {
            if (global.localplayer['PlayerAirsoftTeam'] && global.localplayer['PlayerAirsoftTeam'] >= 0 && global.localplayer['PlayerAirsoftTeam'] == sourcePlayer['PlayerAirsoftTeam'])
                return true;

            if (isZoneAirDrop) {
                const playerOrganization = sourcePlayer['organization'];
                if (global.organizationId != 0 && playerOrganization != 0 && global.organizationId === playerOrganization)
                    return true;
            }

            let dist = global.vdist2(sourcePlayer.position, mp.players.local.position, true);
            let customDamage = damage;

            const rageBoneId = boneIdGtaToRage[boneIndex],
                damageData = damagesData[weaponHash];

            if (damageData/* && damageData.distance >= dist*/) {
                const boneDamageMultiplier = damageController.getBoneDamageMultiplier(rageBoneId),
                    shotCount = damageData.shotCount;

                //customDamage = global.clamp(damageData.damage / (damageData.isSniper ? 1 : (dist / 40)), damageData.damage / 2, damageData.damage) * boneDamageMultiplier / shotCount;
                customDamage = global.clamp(damageData.damage, damageData.damage / 2, damageData.damage) * boneDamageMultiplier / shotCount;
            }

            const getHealth = mp.players.local.getHealth(),
                getArmour = mp.players.local.getArmour(),
                healthAndArmour = getHealth + getArmour;

            if (!damageData || !damageData.isMelee) {
                if (healthAndArmour - customDamage <= 0) {

                    clearArmour ()

                    //mp.events.callRemoteUnreliable("server.addHit", sourcePlayer, Math.round(customDamage), rageBoneId, !!headBone.includes(boneIndex), true);
                    return false;
                }

                mp.players.local.applyDamageTo(Math.round(customDamage) + 1, true);

                clearArmour ()

                //mp.events.callRemoteUnreliable("server.addHit", sourcePlayer, Math.round(customDamage), rageBoneId, !!headBone.includes(boneIndex), false);
                return true;
            }

            mp.game.weapon.setCurrentDamageEventAmount(Math.round(customDamage));

            clearArmour ()

            //mp.events.callRemoteUnreliable("server.addHit", sourcePlayer, Math.round(customDamage), rageBoneId, !!headBone.includes(boneIndex), !!(healthAndArmour - customDamage <= 0));
        }
    }
});
/*
gm.events.add("projectile", (sourcePlayer, weaponHash, ammoType, position, velocity) =>  mp.players.local === sourcePlayer && (weaponHash !== mp.game.joaat("weapon_snowball") && global.weaponData.weapon >> 0 == weaponHash >> 0));//Танк рпг
gm.events.add("explosion", () => true);//c4
*/
gm.events.add("explosion", function (sourcePlayer, type, pos) {
    return true;
});
gm.events.add("projectile", function (sourcePlayer, weaponHash, ammoType, position, direction) {
    //mp.gui.chat.push(`${sourcePlayer}, ${weaponHash}, ${ammoType}, ${position}, ${direction}`);
    return true;
});

global.weaponGroup = {
    Unarmed: 2685387236,
    Melee: 3566412244,
    Pistol: 416676503,
    SMG: 3337201093,
    AssaultRifle: 970310034,
    DigiScanner: 3539449195,
    FireExtinguisher: 4257178988,
    MG: 1159398588,
    NightVision: 3493187224,
    Parachute: 431593103,
    Shotgun: 860033945,
    Sniper: 3082541095,
    Stungun: 690389602,
    Heavy: 2725924767,
    Thrown: 1548507267,
    PetrolCan: 1595662460,
}

global.getCurrentWeapon = function() {
    return mp.game.invoke(getNative("GET_SELECTED_PED_WEAPON"), global.localplayer.handle);
}

global.weaponData = {
    weapon: mp.game.joaat('weapon_unarmed'),
    group: global.weaponGroup.Unarmed,
    ammo: 0,
    isMelee: false
}

const revolverData = [
    mp.game.joaat('weapon_revolver') >> 0,//-1045183535,
    mp.game.joaat('weapon_revolver_mk2') >> 0,//-879347409,
    mp.game.joaat('weapon_doubleaction') >> 0,//-1746263880,
    mp.game.joaat('weapon_navyrevolver') >> 0,//-1853920116
];

gm.events.add(global.renderName ["250ms"], () => {
    const weaponHash = global.getCurrentWeapon();
    if (weaponHash != global.weaponData.weapon) {
        switch (global.weaponData.weapon) {
            case mp.game.joaat('weapon_ball') >> 0://600439132:
            case mp.game.joaat('weapon_snowball') >> 0://126349499:
                global.weaponData.weapon = weaponHash;
                global.weaponData.group = mp.game.weapon.getWeapontypeGroup(global.weaponData.weapon);
                global.weaponData.isMelee = [global.weaponGroup.Unarmed, global.weaponGroup.Melee].includes(global.weaponData.group);
                mp.events.callRemote('server.weapon.remove');
                return;
        }

        mp.game.invoke(getNative("GIVE_WEAPON_TO_PED"), global.localplayer.handle, global.weaponData.weapon, global.weaponData.ammo, false, true);
        if(!global.localplayer.isClimbing)
        {
            global.weaponData.ammo = 0;
            mp.game.invoke(getNative("SET_PED_AMMO"), global.localplayer.handle, global.weaponData.weapon, 0);
            global.localplayer.taskReloadWeapon(false);
            global.localplayer.taskSwapWeapon(false);
            mp.gui.emmit(`window.hudStore.ammo (0)`);
        }
    }
    mp.game.player.setVehicleDefenseModifier(0.005);
});

/*
setInterval(() => {
    if (!global.localplayer.vehicle) {
        mp.game.player.setWeaponDefenseModifier(-9999999);
        mp.game.ped.setAiWeaponDamageModifier(0);
    } else {
        mp.game.player.setWeaponDefenseModifier(1);
        mp.game.ped.setAiWeaponDamageModifier(dmgmodifadm);
    }
    mp.game.ped.setAiMeleeWeaponDamageModifier(0);
    mp.game.player.setMeleeWeaponDefenseModifier(0);
    mp.game.player.setWeaponDamageModifier(0); // Less Damage from Players
    global.localplayer.setSuffersCriticalHits(false); // **No Headshotkill**
    mp.game.player.setVehicleDefenseModifier(0.005);
}, 1000);*/







const meleeControls = [
    global.Inputs.MELEE_ATTACK1,
    global.Inputs.MELEE_ATTACK_LIGHT,
    global.Inputs.MELEE_ATTACK_ALTERNATE
]

const vehAttackControls = [
    global.Inputs.VEH_ATTACK,
    global.Inputs.VEH_ATTACK2,
    global.Inputs.VEH_PASSENGER_ATTACK,
]

Natives._REMOVE_STEALTH_KILL = (hash, p1) => mp.game.invoke('0xA6A12939F16D85BE', hash, p1);

gm.events.add(global.renderName ["render"], () => {
    if (!global.loggedin)
        return;
    //

    //mp.game.controls.disableControlAction(2, global.Inputs.DUCK, true);
    //mp.game.controls.disableControlAction(2, global.Inputs.SCRIPT_RLEFT, true);


    if (!global.weaponData.isMelee) {
        meleeControls.forEach((contolId) => {
            mp.game.controls.disableControlAction(2, contolId, true);
        });
    } else {
        mp.game.controls.disableControlAction(2, global.Inputs.MELEE_ATTACK_LIGHT, true);

        if (global.localplayer.isPerformingStealthKill() || Natives.GET_PED_STEALTH_MOVEMENT(global.localplayer.handle)) {
            mp.game.controls.disableControlAction(2, global.Inputs.MELEE_ATTACK_HEAVY, true);
            mp.game.controls.disableControlAction(2, global.Inputs.MELEE_ATTACK_ALTERNATE, true);
        }
    }

    //

    if (global.localplayer.vehicle) {
        mp.game.controls.disableControlAction(2, global.Inputs.VEH_MELEE_HOLD, true);
        const weaponHash = global.getCurrentWeapon();
        const weaponGroup = mp.game.weapon.getWeapontypeGroup(weaponHash);
        if (/*revolverData.includes (global.weaponData.weapon) && */global.localplayer.vehicle.getPedInSeat(-1) === global.localplayer.handle && [global.weaponGroup.Pistol, global.weaponGroup.SMG, global.weaponGroup.AssaultRifle, global.weaponGroup.MG, global.weaponGroup.Shotgun, global.weaponGroup.Sniper, global.weaponGroup.Stungun, global.weaponGroup.Heavy].includes(weaponGroup)) {
            vehAttackControls.forEach((contolId) => {
                mp.game.controls.disableControlAction(2, contolId, true);
            });
        }
    }

})

//Контрол стандартного интерфейса

const weaponWheelControls = [
    global.Inputs.WEAPON_WHEEL_UD,
    global.Inputs.WEAPON_WHEEL_LR,
    global.Inputs.WEAPON_WHEEL_NEXT,
    global.Inputs.WEAPON_WHEEL_PREV,
    global.Inputs.SELECT_NEXT_WEAPON,
    global.Inputs.SELECT_PREV_WEAPON,
];

const vehSelectWeaponControls = [
    global.Inputs.VEH_SELECT_NEXT_WEAPON,
    global.Inputs.VEH_SELECT_PREV_WEAPON,
];

const selectWeaponControls = [
    global.Inputs.SELECT_WEAPON_UNARMED,
    global.Inputs.SELECT_WEAPON_MELEE,
    global.Inputs.SELECT_WEAPON_HANDGUN,
    global.Inputs.SELECT_WEAPON_SHOTGUN,
    global.Inputs.SELECT_WEAPON_SMG,
    global.Inputs.SELECT_WEAPON_AUTO_RIFLE,
    global.Inputs.SELECT_WEAPON_SNIPER,
    global.Inputs.SELECT_WEAPON_HEAVY,
    global.Inputs.SELECT_WEAPON_SPECIAL,
];

const scrollWeaponWheelControls = [
    global.Inputs.PREV_WEAPON,
    global.Inputs.NEXT_WEAPON,
];

//

new class extends debugRender {
    constructor() {
        super("r_player_damage_index2");
    }

    render () {
    if (!global.loggedin)
        return;

    weaponWheelControls.forEach((contolId) => {
        mp.game.controls.disableControlAction(2, contolId, true);
    });

    mp.game.controls.disableControlAction(2, global.Inputs.SELECT_WEAPON, true);

    vehSelectWeaponControls.forEach((contolId) => {
        mp.game.controls.disableControlAction(2, contolId, true);
    });

    selectWeaponControls.forEach((contolId) => {
        mp.game.controls.disableControlAction(2, contolId, true);
    });

    scrollWeaponWheelControls.forEach((contolId) => {
        mp.game.controls.disableControlAction(2, contolId, true);
    });

}};

//

let isZoneAirDrop = false;
gm.events.add('client.AirDrop.isZone', (toggled) => {
    isZoneAirDrop = toggled;
})

//