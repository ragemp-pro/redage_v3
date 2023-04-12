const airsoft = {
    joinLobbyIndex: -1,

    local_seconds: 600,
    local_interval: undefined,

    zone_marker: undefined,
    zone_shape: undefined,
    zone_leave_interval: undefined,
    zone_return_seconds: 15,
    zone_return_show_message: undefined
};

gm.events.add('playerEnterColshape', (shape) => {
    try
    {
        if (shape === airsoft.zone_shape) {
            if (airsoft.zone_leave_interval !== undefined) {
                clearInterval(airsoft.zone_leave_interval);
                airsoft.zone_leave_interval = undefined;
            }

            airsoft.zone_return_show_message = undefined;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "playerEnterColshape", e.toString());
    }
});

gm.events.add('playerExitColshape', (shape) => {
    try
    {
        if (shape === airsoft.zone_shape && global.inAirsoftLobby !== undefined && global.inAirsoftLobby >= 0) {
            if (airsoft.zone_leave_interval !== undefined) {
                clearInterval(airsoft.zone_leave_interval);
                airsoft.zone_leave_interval = undefined;
            }
            
            airsoft.zone_return_seconds = 15;

            airsoft.zone_leave_interval = setInterval(() => {
                airsoft.zone_return_seconds--;

                if (airsoft.zone_return_seconds >= 1) {
                    airsoft.zone_return_show_message = translateText("~w~У вас есть ~r~{0} ~w~секунд,\\nчтобы вернуться в зону.", airsoft.zone_return_seconds);
                } else {
                    airsoft.zone_return_show_message = undefined;
                    mp.events.callRemote('airsoft_respawnPlayer');
                    
                    clearInterval(airsoft.zone_leave_interval);
                    airsoft.zone_leave_interval = undefined;
                }
            }, 1000);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "playerExitColshape", e.toString());
    }
});

gm.events.add('airsoft_lobbyMenuHandler', (state, data, gameType = 'airsoft', startSeconds = 120) => {
    try
    {
        if (state == 1 && !global.lobbyMenuActive) { // Подгрузка списка лобби с сервера
            let current_lobby_list = [];
            
            data = JSON.parse(data);

            data.forEach((item) => {
                current_lobby_list.push(
                    {
                        lobby_number: item.LobbyIndex,
                        join_password: item.LobbyPassword,
                        title: item.LobbyName,
                        price: item.LobbyPrice,
                        mode: item.LobbyMode,
                        close: ((item.LobbyPassword == 0) ? false : true),
                        class: gameType
                    }
                );
            })

            mp.gui.emmit(`window.router.setView("GamesOtherLobby", ['${JSON.stringify(current_lobby_list)}', '${gameType}', false, '[0, 0, 0]', ${startSeconds}])`);
            gm.discord(translateText("В лобби на Арене"));

            global.menuOpen();
            global.lobbyMenuActive = true;

            if (gameType == 'airsoft') mp.events.callRemote('UpdateServerPlayersInLobbyMenuList', 1);
            else if (gameType == 'mafia') mp.events.callRemote('MafiaUpdateServerPlayersInLobbyMenuList', 1);
            else if (gameType == 'tanks') mp.events.callRemote('TanksUpdateServerPlayersInLobbyMenuList', 1);
        }
        else if (state == 2) { // Закрытие интерфейса
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            global.lobbyMenuActive = false;

            mp.events.callRemote('UpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('MafiaUpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('TanksUpdateServerPlayersInLobbyMenuList', 2);
        }
        else if (state == 3) { // Присоединение к лобби
            if (!global.lobbyMenuActive) {
                mp.gui.emmit(`window.router.setView("GamesOtherLobby", ['[]', '${gameType}', true, '${data}', ${startSeconds}])`);

                global.menuOpen();
                global.lobbyMenuActive = true;
            } else {
                mp.gui.emmit(`window.events.callEvent("cef.lobby.updatePopupInfo", true, '${data}', ${startSeconds})`);
            }

            mp.events.callRemote('UpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('MafiaUpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('TanksUpdateServerPlayersInLobbyMenuList', 2);
        }
        else if (state == 4) { // Выход из лобби
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            global.lobbyMenuActive = false;

            mp.events.callRemote('UpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('MafiaUpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('TanksUpdateServerPlayersInLobbyMenuList', 2);
        }
        else if (state == 5) { // Обновление числа игроков в лобби
            if (global.lobbyMenuActive) {
                mp.gui.emmit(`window.events.callEvent("cef.lobby.updatePopupInfo", true, '${data}')`);
            }

            mp.events.callRemote('UpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('MafiaUpdateServerPlayersInLobbyMenuList', 2);
            mp.events.callRemote('TanksUpdateServerPlayersInLobbyMenuList', 2);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "airsoft_lobbyMenuHandler", e.toString());
    }
});

gm.events.add('airsoft_createLobby_client', (lobby_name, lobby_price, lobby_password, lobby_mode, lobby_weapon, lobby_map) => {
    mp.events.callRemote('airsoft_createLobby_server', lobby_name, lobby_price, lobby_password, lobby_mode, lobby_weapon, lobby_map);
});

gm.events.add('airsoft_joinLobby_client', (lobby_index) => {
    mp.events.callRemote('airsoft_joinLobby_server', lobby_index, 0);
});

gm.events.add('airsoft_joinPrivateLobby_client', (state, value) => {
    try
    {
        if (state == 1) {
            airsoft.joinLobbyIndex = value;
    
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            global.lobbyMenuActive = false;
            
            global.input.set(translateText("Пароль для входа в лобби"), translateText("Введите пароль"), 4, "join_private_lobby");
        }
        else if (state == 2) {
            mp.events.callRemote('airsoft_joinLobby_server', airsoft.joinLobbyIndex, value);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "airsoft_joinPrivateLobby_client", e.toString());
    }
});

gm.events.add('airsoft_updateLobbyList_client', (data, gameType = 'airsoft') => {
    try
    {
        let current_lobby_list = [];
            
        data = JSON.parse(data);

        for (let i in data) {
            current_lobby_list.push(
                {
                    lobby_number: data[i].LobbyIndex,
                    join_password: data[i].LobbyPassword,
                    title: data[i].LobbyName,
                    price: data[i].LobbyPrice,
                    mode: data[i].LobbyMode,
                    close: ((data[i].LobbyPassword == 0) ? false : true),
                    class: gameType
                }
            );
        }

        mp.gui.emmit(`window.events.callEvent("cef.lobby.updateLobbyList", '${JSON.stringify(current_lobby_list)}')`);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "airsoft_updateLobbyList_client", e.toString());
    }
});

let start_interval = null;

gm.events.add('airsoft_updateStats_client', async (state, value1, value2) => {
    try
    {
        if (state == 0) {
            if (airsoft.local_interval !== undefined) {
                clearInterval(airsoft.local_interval);
                airsoft.local_interval = undefined;
            }

            mp.gui.emmit(`window.airsoftFunctions(0);`);
            mp.events.call("hud.kill.clear");
        }
        else if (state == 1) { // 1x1, 2x2, 3x3, 5x5
            let data = [
                {
                    name: translateText("СЧЕТ"),
                    score: `${value1} : ${value2}`
                }
            ];

            mp.gui.emmit(`window.airsoftFunctions(1, '${JSON.stringify(data)}');`);
        }
        else if (state == 2) { // GunGame
            let players = mp.players.toArray().filter(p => p.hasVariable("killsWeapon"));
            players.sort((a, b) => b.getVariable("killsWeapon") - a.getVariable("killsWeapon"));
            players = players.slice(0, 3);
            gm.discord(translateText("Играет в GunGame"));

            let data = [
                {
                    name: players[0] ? players[0].name : translateText("Нет"),
                    score: players[0] ? players[0].getVariable("killsWeapon") : 0
                },
                {
                    name: players[1] ? players[1].name : translateText("Нет"),
                    score: players[1] ? players[1].getVariable("killsWeapon") : 0
                },
                {
                    name: players[2] ? players[2].name : translateText("Нет"),
                    score: players[2] ? players[2].getVariable("killsWeapon") : 0
                },
                {
                    name: translateText("Уровень"),
                    score: ((global.localplayer.getVariable("weaponLevel") || 0) + 1)
                }
            ];

            mp.gui.emmit(`window.airsoftFunctions(2, '${JSON.stringify(data)}');`);
        }
        else if (state == 3) { // Timer
            airsoft.local_seconds = value1;

            mp.gui.emmit(`window.airsoftFunctions(3, ${airsoft.local_seconds});`);

            mp.events.call('freeze', true);

            let start_seconds = 5;
            
            mp.gui.emmit(`window.updateGameTime (${start_seconds}, '${translateText("Матч")}', '${translateText("Цель: убить как можно больше противников")}');`);
            
            if (start_interval)
                clearInterval(start_interval);

            start_interval = setInterval(() => {
                start_seconds -= 1;
                mp.gui.emmit(`window.updateGameTime (${start_seconds}, '${translateText("Матч")}', '${translateText("Цель: убить как можно больше противников")}');`);

                if (start_seconds <= 0) {
                    clearInterval(start_interval);
                    start_interval = null

                    start_seconds = 0;

                    mp.events.call('freeze', false);
                }
            }, 1000);
            
            await global.wait(5000);

            airsoft.local_interval = setInterval(() => {
                airsoft.local_seconds -= 1;

                if (airsoft.local_seconds <= 0) {
                    clearInterval(airsoft.local_interval);
                    airsoft.local_interval = undefined;
                }

                mp.gui.emmit(`window.airsoftFunctions(3, ${airsoft.local_seconds});`);
            }, 1000);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "airsoft_updateStats_client", e.toString());
    }
});

gm.events.add('airsoft_updateAreaLimit', (state, coords, radius) => {
    try
    {
        if (state == 0) {
            if (airsoft.zone_marker !== undefined) {
                airsoft.zone_marker.destroy();
                airsoft.zone_marker = undefined;
            }

            if (airsoft.zone_shape !== undefined) {
                airsoft.zone_shape.destroy();
                airsoft.zone_shape = undefined;
            }

            if (airsoft.zone_leave_interval !== undefined) {
                clearInterval(airsoft.zone_leave_interval);
                airsoft.zone_leave_interval = undefined;
            }
            
            airsoft.zone_return_show_message = undefined;
        }
        else if (state == 1) {
            airsoft.zone_marker = mp.markers.new(28, new mp.Vector3(coords.x, coords.y, coords.z - 1.25), radius, {
                visible: true,
                color: [255, 0, 0, 90],
                dimension: -1
            });

            airsoft.zone_shape = mp.colshapes.newSphere(coords.x, coords.y, coords.z, radius, -1);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "airsoft_updateAreaLimit", e.toString());
    }
});

gm.events.add('airsoft_updateAirsoftLobbyValue', (value) => {
    try
    {
        global.inAirsoftLobby = value;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/airsoft", "airsoft_updateAirsoftLobbyValue", e.toString());
    }
});

gm.events.add("render", () => {
    if (!global.loggedin) return;
    if (airsoft.zone_return_show_message !== undefined) {
        mp.game.graphics.drawText(airsoft.zone_return_show_message, [0.5, 0.5], { 
            font: 0,
            color: [255, 255, 255, 185],
            scale: [1.0, 1.0],
            outline: true
        });
    }
});