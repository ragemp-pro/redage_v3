let smartphoneScenarioList = [];
class PhoneScenarioBase extends global.CustomScenario {
    constructor() {
        super("cphone_base");
    }
    async onStart(player) {
        if (player === global.localplayer)
            return;

        global.requestAnimDict("cellphone@str").then(async () => {
            if (mp.players.exists(player) && 0 !== player.handle) {
                mp.attachments.addFor (player, mp.game.joaat("phonecall"));
                player.taskPlayAnim("cellphone@str", "cellphone_text_press_a", 8, 0, -1, 49, 0, false, false, false)
            }
        });
    }
    onStartForNew(player) {
        this.onStart(player);
    }
    onEnd(player) {
        if (player === global.localplayer)
            return;

        if (mp.players.exists(player) && 0 !== player.handle) {
            mp.attachments.removeFor(player, mp.game.joaat("phonecall"));
            player.stopAnimTask("cellphone@str", "cellphone_text_press_a", 3)
            //player.vehicle ? player.stopAnimTask("cellphone@str", "cellphone_text_press_a", 3) : player.clearTasksImmediately()
        }
    }
}
smartphoneScenarioList.push(new PhoneScenarioBase());
class PhoneScenarioCall extends global.CustomScenario {
    constructor() {
        super("cphone_call");
    }
    async onStart(player) {
        if (player === global.localplayer) {
            player.taskUseMobilePhone(1);
            return;
        }
        global.requestAnimDict("anim@cellphone@in_car@ds").then(async () => {
            if (mp.players.exists(player) && 0 !== player.handle) {
                mp.attachments.addFor (player, mp.game.joaat("phonecall"));
                player.taskPlayAnim("anim@cellphone@in_car@ds", "cellphone_call_listen_base", 8, 0, -1, 49, 0, false, false, false)
            }
        });
    }
    onStartForNew(player) {
        this.onStart(player);
    }
    onEnd(player) {
        if (player === global.localplayer) {
            player.taskUseMobilePhone(0);
            return;
        }
        if (mp.players.exists(player) && 0 !== player.handle) {
            mp.attachments.removeFor(player, mp.game.joaat("phonecall"));
            player.stopAnimTask("anim@cellphone@in_car@ds", "cellphone_call_listen_base", 3)
            //player.vehicle ? player.stopAnimTask("cellphone@str", "cellphone_text_press_a", 3) : player.clearTasksImmediately()
        }
    }
}
smartphoneScenarioList.push(new PhoneScenarioCall());
const isPlayerHasAnyPhoneScenario = (player) => {
    const name = player.cSen;
    return "cphone_base" === name || "cphone_call" === name;
};

gm.events.add("client.phone.anim", async (playerId, type) => {
    const player = mp.players.atRemoteId(playerId);
    if (mp.players.exists(player) && 0 !== player.handle && player !== global.localplayer)
        if (0 === type) {
            for (const scenario of smartphoneScenarioList)
                if (scenario.isActive(player)) {
                    scenario.onStart(player);
                    break;
                }
        } else if (1 === type) {
            global.requestAnimDict("cellphone@self").then(async () => {
                if (mp.players.exists(player) && 0 !== player.handle) {
                    player.taskPlayAnim("cellphone@self", "selfie", 4, 4, -1, 49, 0, false, false, false);
                }
            });
        }
})