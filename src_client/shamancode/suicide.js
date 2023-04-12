const localPlayer = mp.players.local;
const animDict = "MP_SUICIDE";
const fireActionHash = mp.game.joaat("Fire");

let animCheckerHandle = undefined;
let shotFired = false;

function destroyAnimChecker() {
    animCheckerHandle.destroy();
    animCheckerHandle = undefined;
}

const pistolHashes = [
    mp.joaat("WEAPON_PISTOL"), mp.joaat("WEAPON_COMBATPISTOL"), mp.joaat("WEAPON_APPISTOL"), mp.joaat("WEAPON_PISTOL50"), mp.joaat("WEAPON_REVOLVER"),
    mp.joaat("WEAPON_SNSPISTOL"), mp.joaat("WEAPON_HEAVYPISTOL"), mp.joaat("WEAPON_DOUBLEACTION"), mp.joaat("WEAPON_REVOLVER_MK2"), mp.joaat("WEAPON_SNSPISTOL_MK2"),
    mp.joaat("WEAPON_PISTOL_MK2"), mp.joaat("WEAPON_VINTAGEPISTOL"), mp.joaat("WEAPON_MARKSMANPISTOL")
];

const commandCooldown = 30000; // milliseconds a player needs to wait to use the command again

mp.events.add({
    "Suicide_Shoot": (shooter) => {
        mp.players.forEachInRange(shooter.position, mp.config["stream-distance"], (player) => {
            player.invoke("0x96A05E4FB321B1BA", shooter, 0.0, 0.0, 0.0, false); // SET_PED_SHOOTS_AT_COORD
        });
    },

    "Suicide_Kill": (player) => {
        player.health = 0;
    }
});

gm.events.add('client_suicide', (player) => {
 try{
    const now = Date.now();
        if (now - player.lastSuicide < commandCooldown) {
            //player.outputChatBox(`Wait ${Math.round((commandCooldown - (now - player.lastSuicide)) / 1000)} seconds to use the suicide command again.`);
            return;
        }
        let usePistol = pistolHashes.includes(player.weapon); // "&& player.weaponAmmo > 0" once existed here but didn't work...
        let animName = usePistol ? "PISTOL" : "PILL";
        let animTime = usePistol ? 0.365 : 0.536;
        player.lastSuicide = Date.now();
        mp.players.callInRange(player.position, mp.config["stream-distance"], "Suicide_ApplyAnimation", [player, animName, animTime]);
        }
 catch(e){
        mp.events.callRemote("client_trycatch", "shamancode/suicide", "client_suicide", e.toString());
    }
});

mp.events.add("Suicide_ApplyAnimation", (player, animName, animTime) => {
    if (player.handle) {
        mp.game.streaming.requestAnimDict(animDict);
        while (!mp.game.streaming.hasAnimDictLoaded(animDict)) mp.game.wait(0);
        player.taskPlayAnim(animDict, animName, 8.0, 0.0, -1, 0, 0.0, false, false, false);

        if (player.remoteId === localPlayer.remoteId) {
            shotFired = false;
            if (animCheckerHandle) destroyAnimChecker();

            animCheckerHandle = new mp.Event("render", () => {
                if (localPlayer.isPlayingAnim(animDict, animName, 3)) {
                    if (animName === "PISTOL" && !shotFired && localPlayer.hasAnimEventFired(fireActionHash)) {
                        shotFired = true;
                        mp.events.callRemote("Suicide_Shoot");
                    }

                    if (localPlayer.getAnimCurrentTime(animDict, animName) >= animTime) {
                        destroyAnimChecker();
                        mp.events.callRemote("Suicide_Kill");
                    }
                } else {
                    destroyAnimChecker();
                }
            });
        }
    }
});
