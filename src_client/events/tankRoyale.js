let tanksJoinLobbyIndex = -1;

gm.events.add('tanks_createLobby_client', (lobby_name, lobby_price, lobby_password, lobby_map) => {
    mp.events.callRemote('tanks_createLobby_server', lobby_name, lobby_price, lobby_password, lobby_map);
});

gm.events.add('tanks_joinLobby_client', (lobby_index) => {
    mp.events.callRemote('tanks_joinLobby_server', lobby_index, 0);
});

gm.events.add('tanks_joinPrivateLobby_client', (state, value) => {
    try
    {
        if (state == 1) {
            tanksJoinLobbyIndex = value;
    
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            global.lobbyMenuActive = false;
            
            global.input.set(translateText("Пароль для входа в лобби"), translateText("Введите пароль"), 4, "tanks_join_private_lobby");
        }
        else if (state == 2) {
            mp.events.callRemote('tanks_joinLobby_server', tanksJoinLobbyIndex, value);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/tanksRoyale", "tanks_joinPrivateLobby_client", e.toString());
    }
});