/*const colorHudIds = [
    9,
    18,
    143,
    144,
    145,
]

gm.events.add('playerReady', () => {
    colorHudIds.forEach((id) => {
        Natives.ReplaceHudColourWithRgba (id, 43, 182, 168, 255);
    })
});*/

let isMerger = false;

rpc.register("rpc.isMerger", () => isMerger);

gm.events.add('client.init', function (serverId, serverName, donatMultiplier, donateDoubleConvert, merger) {

    isMerger = merger;

    mp.events.call("setTraffic", 0);
    mp.events.call("cleartraffic");


    mp.game.gxt.set('PM_PAUSE_HDR', `REDAGE.NET | ${serverName}`);
    mp.gui.emmit(`window.setServerName ('${serverName}')`);
    mp.gui.emmit(`window.serverStore.serverId (${parseInt(serverId)})`);
    mp.gui.emmit(`window.serverStore.serverDonatMultiplier (${donatMultiplier})`);
    mp.gui.emmit(`window.serverStore.serverDonateDoubleConvert (${donateDoubleConvert})`);
    global.menuOpen();
});

gm.events.add('client.closeAll', () => {
    global.FadeScreen (false, 500);
	mp.gui.emmit(`window.router.close()`);
});

let cameraIndex = 0;

global.setStartCam = async () => {
    try
    {
        cameraIndex = 0;
        const camera = global.cameraManager.createCamera ("authentication", new mp.Vector3(723.5045, 851.446, 382.3506), new mp.Vector3(0, 0, 358.6186), 70);
        cameraIndex = 1
        global.localplayer.position = new mp.Vector3(723.5045, 851.446, 382.3506);
        cameraIndex = 2;
        global.localplayer.freezePosition(true);
        cameraIndex = 3;
        global.localplayer.setVisible(false, false);
        cameraIndex = 4;
        global.cameraManager.setActiveCamera(camera, true);
        cameraIndex = 5;
    }
    catch (e)
    {
        mp.events.callRemote("client_trycatch", "player/auth", "setStartCam - " + cameraIndex, e.toString());
        global.FadeScreen (false, 2500);
    }
}

setTimeout(() => {
    global.setStartCam ();
    gm.discord(translateText("Восхищается окном логина"));
}, 150)

gm.events.add('client.auth', async (login) => {
    mp.gui.emmit(`window.listernEvent ('queueText', false);`);
    mp.gui.emmit(`window.accountStore.accountLogin('${login}')`);
});



var lastButAuth = 0;
var lastButSlots = 0;

// events from cef
gm.events.add('client:OnSignInv2', function (username, password) {
    try
    {
        if (new Date().getTime() - lastButAuth < 500) {
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();
        mp.events.callRemote('signin', username, password)
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "client:OnSignInv2", e.toString());
    }
});

gm.events.add('restorepass', function (state, authData) {
    try
    {
        if (new Date().getTime() - lastButAuth < 1000) {
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();

        var nameorcode = authData;
        mp.events.callRemote('restorepass', state, nameorcode);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "restorepass", e.toString());
    }
});

gm.events.add('restorepassstep', function (state) {
	mp.gui.emmit(`window.events.callEvent("cef.authentication.restoreStep", ${state})`);
});

gm.events.add('client:OnSignUpv2', function (username, email, promo, pass1, pass2) {
    try
    {
        if (new Date().getTime() - lastButAuth < 500) {
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();

        if (global.isInvalidLogin(username)) {
            mp.events.call('notify', 1, 9, translateText("Логин не соответствует формату или слишком длинный!"), 3000);
            return;
        }

        if (global.isInvalidEmail(email)) {
            mp.events.call('notify', 1, 9, translateText("Электронная почта не соответствует формату!"), 3000);
            return;
        }

        if(pass1 != pass2) {
            mp.events.call('notify', 1, 9, translateText("Пароли не совпадают!"), 3000);
            return;
        }
        
        if(pass1.length < 3) {
            mp.events.call('notify', 1, 9, translateText("Слишком короткий пароль!"), 3000);
            return;
        }

        mp.events.callRemote('signup', username, pass1, email, promo);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "client:OnSignUpv2", e.toString());
    }
});

gm.events.add('client.registration.error', function (message) {
    mp.events.call('notify', 1, 9, message, 3000);
    mp.gui.emmit(`window.listernEvent ('isSendEmailMessage', false);`);
});

gm.events.add('client.registration.sendEmail', function () {
    mp.gui.emmit(`window.listernEvent ('isSendEmailMessage', true);`);
});

gm.events.add('client:OnSelectCharacterv2', function (uuid, spawnid) {
    try
    {
        if (new Date().getTime() - lastButSlots < 500) {
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButSlots = new Date().getTime();
        mp.gui.emmit(`window.router.close()`);
        global.FadeScreen (true, 0);
        mp.events.call('client.charcreate.close');
        global.localplayer.freezePosition(false);
        global.setPlayerToGround ();
        mp.events.callRemote('selectchar', uuid, spawnid);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "client:OnSelectCharacterv2", e.toString());
    }
});

gm.events.add('client:OnCreateCharacterv2', function (slot, name, lastname) {
    try
    {
        if (global.checkName(name) || !global.checkName2(name) || name.length > 25 || name.length <= 2) {
            mp.events.call('notify', 1, 9, translateText("Правильный формат имени: 3-25 символов и первая буква имени заглавная"), 3000);
            return;
        }

        if (global.checkName(lastname) || !global.checkName2(lastname) || lastname.length > 25 || lastname.length <= 2) {
            mp.events.call('notify', 1, 9, translateText("Правильный формат фамилии: 3-25 символов и первая буква фамилии заглавная"), 3000);
            return;
        }

        if (new Date().getTime() - lastButSlots < 500) {
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButSlots = new Date().getTime();

        mp.events.callRemote('newchar', slot, name, lastname);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "client:OnCreateCharacterv2", e.toString());
    }
});

gm.events.add('buyNewSlot', function (slotId) {
    try
    {
        if (new Date().getTime() - lastButSlots < 500) {
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButSlots = new Date().getTime();
        mp.events.callRemote('server.buySlots', Number (slotId));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "buyNewSlot", e.toString());
    }
});

//***********************************Удаление */

gm.events.add('client.char.delete', function (slot) {
    mp.events.callRemote('server.character.delete', slot);
});

gm.events.add('client.character.canceldelete', function (slot) {
    mp.gui.emmit(`window.accountStore.deleteCharacter(${slot}, "-")`);
});

gm.events.add('client.character.delete', function (slot, data) {
    mp.gui.emmit(`window.accountStore.deleteCharacter(${slot}, ${data})`);
});

gm.events.add('client.character.deleteSuccess', function (slot) {
    mp.gui.emmit(`window.accountStore.deleteSuccessCharacter(${slot})`);
});

gm.events.add('client.character.accountIsSession', function (toggled) {
    mp.gui.emmit(`window.accountStore.accountIsSession(${toggled})`);
});
//****************************************** */


gm.events.add('queue.text', (withkick, data) => {
    mp.gui.emmit(`window.listernEvent ('queueText', '${data}');`);

	if(withkick == true)
        mp.events.callRemote('kickclient');
});

gm.events.add('unlockSlot', function (slotId) {
    mp.gui.emmit(`window.accountStore.unlockSlots(${slotId})`);
});

let useModel = -1;

gm.events.add(global.renderName ["2.5ms"], () => {
    if (global.loggedin && useModel !== global.localplayer.model) {
        useModel = global.localplayer.model;
        global.localplayer.setConfigFlag (429, true);
    }
});

gm.events.add('ready', async function (isSpawn = true) {
    try
    {
        global.loggedin = true;
        global.menuClose();
        mp.events.call('showHUD', true);
        mp.gui.emmit(`window.serverStore.serverPlayerId (${global.localplayer.remoteId})`);
        global.localplayer.setInvincible(false);
        global.localplayer.setVisible(true, false);
        global.SetWalkStyle (global.localplayer, null);
        global.SetFacialClipset (global.localplayer, null);
        global.setPlayerToGround ();
        await global.wait(500);
        global.FadeScreen (false, 2500);
        mp.gui.emmit(`window.router.setHud()`);
        global.setPlayerToGround ();
        await global.wait(500);
        //global.localplayer.freezePosition(false);
        if (!global.isNewChar && isSpawn) {
            mp.events.callRemote('IsCompensation');
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "ready", e.toString());
    }
});

gm.events.add("initAwards", async (json) => {

    if (json) {
        json = JSON.parse(json);
        const award = {
            desc: json[0],
            type: json[1],
            itemId: json[2],
            data: json[3],
            image: json[4],
            time: json[5],
        }
        mp.gui.emmit(`window.listernEvent ('hud.award', true, '${JSON.stringify(award)}');`);

        await global.wait(1000 * 15);

        mp.gui.emmit(`window.listernEvent ('hud.award', false);`);

        await global.wait(500);
    }

    await global.wait(1000 * 30);

    mp.gui.emmit(`window.listernEvent ('hud.bp.info', true);`);

    await global.wait(1000 * 15);

    mp.gui.emmit(`window.listernEvent ('hud.bp.info', false);`);
    await global.wait(500);
    mp.events.call('initWarZone', null, true);
});

// events from cef
gm.events.add('client.merger.auntification', function (password, serverId) {
    try
    {
        if (new Date().getTime() - lastButAuth < 3000) {
            mp.gui.emmit(`window.events.callEvent("cef.merger.progress", -2)`);
            mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
            return;
        }
        lastButAuth = new Date().getTime();
        mp.events.callRemote('server.merger.auntification', password, serverId)
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/auth", "client.merger.auntification", e.toString());
    }
});

gm.events.add('client.merger.progress', function (value) {
    if (value === 999) {
        global.FadeScreen (true, 0);
        mp.events.call('client.charcreate.close', true);
        mp.gui.emmit(`events.callEvent("cef.authentication.setView", "Start")`);
    } else mp.gui.emmit(`window.events.callEvent("cef.merger.progress", ${value})`);
});

//

gm.events.add('client.session.update', function () {
    mp.events.callRemote('server.session.update');
});

//

gm.events.add('client.email.confirm', function (email) {
    if (global.isInvalidEmail(email)) {
        mp.events.call('notify', 1, 9, translateText("Электронная почта не соответствует формату!"), 3000);
        return;
    }

    mp.events.callRemote('server.email.confirm', email);
});