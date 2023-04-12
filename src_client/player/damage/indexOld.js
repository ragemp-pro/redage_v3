import './hit';
import './pet';
import './vehicle';
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


const isPetWeapon = [
    -100946242,
    -440934790,
    861723357,
    -495648874,
    148160082,
    94548753,
    1161062353,
    -188319074,
    -1263987253,
    955837630,
    -96609051,
    743550225,
    -1148198339,
    1205296881,
    -1501041657,
]

mp._events.add('outgoingDamage', (sourceEntity, targetEntity, sourcePlayer, weapon, boneIndex, damage) => {
    if (sourceEntity === global.localplayer && (global.cuffed || global.isDeath === true || global.isDemorgan || global.dmgdisabled || (weapon !== WEAPON_STUNGAN && global.isInSafeZone)))
        return true;

    const weaponHash = sourceEntity !== global.localplayer && isPetWeapon.includes(weapon >> 0) ?
        sourceEntity.model >> 0 :
        weapon >> 0;

    const rageBoneId = boneIdGtaToRage[boneIndex],
        damageData = damagesData[weaponHash];

    if (!damageData)
        return true;

    const boneDamageMultiplier = damageController.getBoneDamageMultiplier(rageBoneId);

    let customDamage = global.clamp(damageData.damage, damageData.damage / 2, damageData.damage) * boneDamageMultiplier / damageData.shotCount;
    customDamage = Math.round(customDamage);


    if (targetEntity.type === "player") {
        if (global.localplayer['PlayerAirsoftTeam'] && global.localplayer['PlayerAirsoftTeam'] >= 0 && global.localplayer['PlayerAirsoftTeam'] == targetEntity['PlayerAirsoftTeam'])
            return true;
        else if (targetEntity['AGM'])
            return true;
        else if (targetEntity['InDeath'])
            return true;
        else if (targetEntity['SZ'])
            return true;
        else if (targetEntity['DMGDisable'])
            return true;

        if (isZoneAirDrop) {
            const playerOrganization = targetEntity['organization'];
            if (global.organizationId != 0 && playerOrganization != 0 && global.organizationId === playerOrganization)
                return true;
        }

        //if (damageData.shotCount > 1 && !global.antiFlood ("shotPlayer_" + targetEntity.remoteId, 50))
        //    return true;

        const healthAndArmour = targetEntity.getHealth() + targetEntity.getArmour();
        const isDead = !!(healthAndArmour - customDamage <= 0);

        if (global.hitPoint)
            mp.events.call("client.addHit", Math.round(customDamage), targetEntity.remoteId, rageBoneId, !!headBone.includes(boneIndex), isDead);

        if (damageData.isPet) {
            mp.events.callRemote("server.damage.petToPlayer", targetEntity, sourceEntity, customDamage);

        } else {
            mp.game.weapon.setCurrentDamageEventAmount(Math.round(customDamage + (isDead ? 5 : 0))); //Фикс 0 hp
            mp.game.weapon.setCurrentDamageEventCritical(false);
        }

    }

    if (targetEntity.type === "vehicle") {
        //mp.gui.chat.push("outgoingDamage 3 - " + damage);
        //mp.game.weapon.setCurrentDamageEventAmount(0);
        //mp.game.weapon.setCurrentDamageEventCritical(false);
        //mp.game.weapon.cancelCurrentDamageEvent();
        //return true;


        /*const getEngineHealth = targetEntity.getEngineHealth(),
            getBodyHealth = targetEntity.getBodyHealth();

        mp.gui.chat.push("getEngineHealth - " + getEngineHealth);
        mp.gui.chat.push("getBodyHealth - " + getBodyHealth);

        mp.gui.chat.push("outgoingDamage 3 - " + damage);*/
        //targetEntity.setEngineHealth(getEngineHealth - damage);

        //targetEntity.setEngineHealth(damage);

        //mp.gui.chat.push("outgoingDamage 3 - " + damage);
        //r < 0 && e.setEngineHealth(0),
        //a < 0 && e.setBodyHealth(0);
        //targetEntity.setEngineHealth(damage);
    }
    if (sourceEntity !== global.localplayer && sourceEntity && sourceEntity.type === "ped") {
        if (targetEntity && targetEntity.type === "ped" && targetEntity.isPet) {
            if (global.isGreenZone (targetEntity.position))
                return;

            mp.events.callRemoteUnreliable("server.damage.petToPet", targetEntity, sourceEntity, customDamage);
            return true;
        }

    }
    if (targetEntity && targetEntity.type === "ped" && targetEntity.isPet) {
        if (global.isGreenZone (targetEntity.position))
            return;

        mp.events.callRemoteUnreliable("server.damage.playerToPet", targetEntity, customDamage);
        return true;
    }
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

const WEAPON_STUNGAN = mp.game.joaat("weapon_stungun");

mp._events.add('incomingDamage', (sourceEntity, sourcePlayer, targetEntity, weapon, boneIndex, damage) => {

    if (targetEntity === mp.players.local) {
        if (global.isDeath === true || global.admingm || global.dmgdisabled || (weapon !== WEAPON_STUNGAN && global.isInSafeZone))
            return true;

        if (mp.players.exists(sourcePlayer))
            mp.events.call('client.pet.attack', sourcePlayer.remoteId);

        if (weapon === WEAPON_STUNGAN) {
            if (!global.antiFlood("incomingDamage_stun", 1500)) return true;
            if (2 > global.localplayer.getHealth()) {
                global.localplayer.setToRagdoll(4000, 4000, 0, false, false, false);
                return true;
            }
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
    weaponHash = weaponHash >> 0;

    const damageData = damagesData[weaponHash];

    if (damageData && damageData.isProjectile)
        return false;

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
    clearArmour ();
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

const clearStealth = () => {

    const isStealthMovement = global.localplayer.getStealthMovement();

    if(isStealthMovement)
        global.localplayer.setStealthMovement(false, "DEFAULT_ACTION");

    if(global.localplayer.isPerformingStealthKill() || isStealthMovement) {
        mp.game.controls.disableControlAction(2, global.Inputs.MELEE_ATTACK_HEAVY, true);
        mp.game.controls.disableControlAction(2, global.Inputs.MELEE_ATTACK_ALTERNATE, true);
    }

}

gm.events.add("render", () => {
    if (!global.loggedin)
        return;

    if (global.weaponData.isMelee)
        clearStealth ();
});

gm.events.add(global.renderName ["render"], () => {
    if (!global.loggedin)
        return;
    //

    //mp.game.controls.disableControlAction(2, global.Inputs.DUCK, true);
    //mp.game.controls.disableControlAction(2, global.Inputs.SCRIPT_RLEFT, true);

    const weaponHash = global.weaponData.weapon >> 0;

    const damageData = damagesData[weaponHash];
    if (damageData) {
        if (!damageData.isMelee) {
            meleeControls.forEach((contolId) => {
                mp.game.controls.disableControlAction(2, contolId, true);
            });

            if (global.localplayer.isUsingActionMode())
                global.localplayer.setUsingActionMode(false, -1, "DEFAULT_ACTION")
        } else {
            mp.game.controls.disableControlAction(2, global.Inputs.MELEE_ATTACK_LIGHT, true);

            clearStealth();
        }

        //

        if (global.localplayer.vehicle) {
            mp.game.controls.disableControlAction(2, global.Inputs.VEH_MELEE_HOLD, true);

            if (global.localplayer.vehicle.getPedInSeat(-1) === global.localplayer.handle && !damageData.isMelee) {
                vehAttackControls.forEach((contolId) => {
                    mp.game.controls.disableControlAction(2, contolId, true);
                });
            }
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

gm.events.add("render", () => {
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
});

//

let isZoneAirDrop = false;
gm.events.add('client.AirDrop.isZone', (toggled) => {
    isZoneAirDrop = toggled;
})

//

const ConsoleLog = (type, message) => {

    switch (type) {
        case "info":
            mp.console.logInfo(message);
            break;
        case "error":
            mp.console.logError(message);
            break;
        case "fatal":
            mp.console.logFatal(message);
            break;
        case "warning":
            mp.console.logWarning(message);
            break;
    }
}

gm.events.add("ready", () => {
    setTimeout(() => {
        try {
            mp.game.weapon.setEnableLocalOutgoingDamage(true);
        }
        catch (e) {}
    }, 1000);
})