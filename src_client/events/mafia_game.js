const mafiaGameInfo = {
    standbyStatus: false,
    currentSoundId: "",
    wasVoiceVolume: 100,
    joinLobbyIndex: -1
};

gm.events.add('mafia_startGameStatusUpdate', () => {
    global.menuOpened = true;
    global.startedMafiaGame = true;
});

gm.events.add('updateClientMafiaStandbyStatus', (status) => {
    mafiaGameInfo.standbyStatus = status;

    if (status == true && global.mafiaMenuActive) {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud();`);
        global.mafiaMenuActive = false;
        global.menuOpened = true;
    }
});

gm.events.add('mafia_updateMicroStatus', (status) => {
    global.mafiaGameProcess = status;

    if (status == 1) {
        global.MaxVoiceVolume = 0;
    } else {
        global.MaxVoiceVolume = mafiaGameInfo.wasVoiceVolume;
    }
});

gm.events.add('showMafiaGameMenu', (data, mute_status, vote_status, timesday, action_info, action_time_info) => {
    try
    {
        let currentMafiaMenuData = [];

        data = JSON.parse(data);
        
        for (let i in data) {
            currentMafiaMenuData.push(
                {
                    id: data[i].PlayerId,
                    name: (global.localplayer.getVariable("mafiaGameRole") == 1 && data[i].PlayerRole == 1 || !data[i].PlayerLife || data[i].PlayerId == global.localplayer.remoteId) ? data[i].PlayerName : translateText("Неизвестно"),
                    role: (global.localplayer.getVariable("mafiaGameRole") == 1 && data[i].PlayerRole == 1 || data[i].PlayerId == global.localplayer.remoteId) ? (data[i].PlayerRole - 1) : 4,
                    mute: data[i].PlayerMute,
                    life: data[i].PlayerLife
                }
            );
        }
        
        if (!global.mafiaMenuActive) {
            mp.gui.emmit(`window.router.setView("GamesOtherMafia", ['${JSON.stringify(currentMafiaMenuData)}', ${vote_status}, ${timesday}, '${action_info}', '${action_time_info}', false, ${global.localplayer.getVariable("mafiaGameRole") - 1}])`);
            gm.discord(translateText("Играет в Мафию"));

            global.menuOpen();
            global.mafiaMenuActive = true;
        } else {
            mp.gui.emmit(`window.events.callEvent("cef.mafiaGame.updateGameWindow", '${JSON.stringify(currentMafiaMenuData)}', ${vote_status}, ${timesday}, '${action_info}', '${action_time_info}', false)`);
        }

        mafiaGameInfo.standbyStatus = false;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/mafia_game", "showMafiaGameMenu", e.toString());
    }
});

gm.events.add('mafia_closeGameMenu', () => {
    try
    {
        if (global.mafiaMenuActive) {
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            global.mafiaMenuActive = false;
            global.menuOpened = true;
        }

        mafiaGameInfo.standbyStatus = true;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/mafia_game", "mafiaLobbyFunctions_client", e.toString());
    }
});

gm.events.add('updateVoteStatusMafiaGame', (status) => {
    mp.gui.emmit(`window.events.callEvent("cef.mafiaGame.updateVoteStatus", ${status})`);
});

gm.events.add('voteMafiaGame_client', (value) => {
    mp.events.callRemote('voteMafiaGame', value);
});

gm.events.add('openMafiaHelpStartPage', (seconds) => {
    try
    {
        if (!global.mafiaMenuActive) {
            mp.gui.emmit(`window.router.setView("GamesOtherMafia", ['[]', false, false, '', '', true, 0, ${seconds}])`);

            setTimeout(() => {
                mp.gui.emmit(`window.events.callEvent("cef.mafiaGame.updateStandbyScreenStatus", true, ${seconds})`);
            }, 100);

            global.menuOpen();
            global.mafiaMenuActive = true;

            mafiaGameInfo.standbyStatus = false;

            mp.events.call("sounds.playInterface", "mafia/start", 0.03);
            mafiaGameInfo.currentSoundId = "mafia/start";
        } else {
            mp.gui.emmit(`window.events.callEvent("cef.mafiaGame.updateStandbyScreenStatus", true, ${seconds})`);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/mafia_game", "openMafiaHelpStartPage", e.toString());
    }
});

gm.events.add('mafia_startSoundSpeech', (speech) => {
    mp.events.call("sounds.stop", mafiaGameInfo.currentSoundId);
    mp.events.call("sounds.playInterface", `mafia/${speech}`, 0.07);
    mafiaGameInfo.currentSoundId = `mafia/${speech}`;
});

gm.events.add('mafia_clearGameInfo', () => {
    try
    {
        global.menuClose();
        global.mafiaGameProcess = 0;
        global.startedMafiaGame = false;
        global.MaxVoiceVolume = mafiaGameInfo.wasVoiceVolume;
        mafiaGameInfo.standbyStatus = false;

        if (global.mafiaMenuActive) {
            mp.gui.emmit(`window.router.setHud();`);
            global.mafiaMenuActive = false;
        }

        if (global.lobbyMenuActive) {
            mp.gui.emmit(`window.router.setHud();`);
            global.lobbyMenuActive = false;
        }

        if (mafiaGameInfo.currentSoundId != "mafia/peaceful_win" && mafiaGameInfo.currentSoundId != "mafia/mafia_win") {
            mp.events.call("sounds.stop", mafiaGameInfo.currentSoundId);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/mafia_game", "mafiaLobbyFunctions_client", e.toString());
    }
});

//

gm.events.add('mafia_createLobby_client', (lobby_name, lobby_price, lobby_password, lobby_map) => {
    mp.events.callRemote('mafia_createLobby_server', lobby_name, lobby_price, lobby_password, lobby_map);
});

gm.events.add('mafia_joinLobby_client', (lobby_index) => {
    mp.events.callRemote('mafia_joinLobby_server', lobby_index, 0);
});

gm.events.add('mafia_joinPrivateLobby_client', (state, value) => {
    try
    {
        if (state == 1) {
            mafiaGameInfo.joinLobbyIndex = value;
    
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            global.lobbyMenuActive = false;
            
            global.input.set(translateText("Пароль для входа в лобби"), translateText("Введите пароль"), 4, "mafia_join_private_lobby");
        }
        else if (state == 2) {
            mp.events.callRemote('mafia_joinLobby_server', mafiaGameInfo.joinLobbyIndex, value);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/mafia_game", "mafia_joinPrivateLobby_client", e.toString());
    }
});

//

gm.events.add("render", () => {
    if (mafiaGameInfo.standbyStatus != 0) {
        mp.game.graphics.drawText(translateText("Вы ожидаете свою очередь..."), [0.5, 0.9], {
            font: 4,
            color: [255, 255, 255, 255],
            scale: [0.75, 0.75],
            centre: true,
        });
    }

    if (global.startedMafiaGame === true) {
        mp.game.controls.disableControlAction(0, 85, true);
        mp.game.controls.disableControlAction(0, 8, true);
        mp.game.controls.disableControlAction(0, 9, true);
        mp.game.controls.disableControlAction(0, 30, true);
        mp.game.controls.disableControlAction(0, 31, true);
        mp.game.controls.disableControlAction(0, 32, true);
        mp.game.controls.disableControlAction(0, 33, true);
        mp.game.controls.disableControlAction(0, 34, true);
        mp.game.controls.disableControlAction(0, 35, true);
        mp.game.controls.disableControlAction(0, 36, true);
        mp.game.controls.disableControlAction(0, 63, true);
        mp.game.controls.disableControlAction(0, 64, true);
        mp.game.controls.disableControlAction(0, 71, true);
        mp.game.controls.disableControlAction(0, 72, true);
        mp.game.controls.disableControlAction(0, 77, true);
        mp.game.controls.disableControlAction(0, 78, true);
        mp.game.controls.disableControlAction(0, 78, true);
        mp.game.controls.disableControlAction(0, 87, true);
        mp.game.controls.disableControlAction(0, 88, true);
        mp.game.controls.disableControlAction(0, 89, true);
        mp.game.controls.disableControlAction(0, 90, true);
        mp.game.controls.disableControlAction(0, 129, true);
        mp.game.controls.disableControlAction(0, 130, true);
        mp.game.controls.disableControlAction(0, 133, true);
        mp.game.controls.disableControlAction(0, 134, true);
        mp.game.controls.disableControlAction(0, 136, true);
        mp.game.controls.disableControlAction(0, 139, true);
        mp.game.controls.disableControlAction(0, 146, true);
        mp.game.controls.disableControlAction(0, 147, true);
        mp.game.controls.disableControlAction(0, 148, true);
        mp.game.controls.disableControlAction(0, 149, true);
        mp.game.controls.disableControlAction(0, 150, true);
        mp.game.controls.disableControlAction(0, 151, true);
        mp.game.controls.disableControlAction(0, 232, true);
        mp.game.controls.disableControlAction(0, 266, true);
        mp.game.controls.disableControlAction(0, 267, true);
        mp.game.controls.disableControlAction(0, 268, true);
        mp.game.controls.disableControlAction(0, 269, true);
        mp.game.controls.disableControlAction(0, 278, true);
        mp.game.controls.disableControlAction(0, 279, true);
        mp.game.controls.disableControlAction(0, 338, true);
        mp.game.controls.disableControlAction(0, 339, true);
        mp.game.controls.disableControlAction(0, 44, true);
        mp.game.controls.disableControlAction(0, 20, true);
        mp.game.controls.disableControlAction(0, 22, true);
        mp.game.controls.disableControlAction(0, 47, true);
    }
});