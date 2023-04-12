<script>
    import { translateText } from 'lang'
    import '../sass/games.sass';
    import { format } from 'api/formatter';
    import { executeClient } from 'api/rage';

    import map_0 from '../images/maps/map_0.png';
    import map_1 from '../images/maps/map_1.png';
    import map_2 from '../images/maps/map_2.png';
    import map_3 from '../images/maps/map_3.png';
    import map_4 from '../images/maps/map_4.png';
    import map_5 from '../images/maps/map_5.png';
    import map_6 from '../images/maps/map_6.png';

    import mafia1 from '../images/maps/mafia1.png';
    import mafia2 from '../images/maps/mafia2.png';
    
    import tanks1 from '../images/maps/tanks1.png';

    import ProgressBar from 'progressbar.js';
    
    export let viewData;

    let ProgressBarId;
    
    let
        popup = viewData[2],
        counterSeconds = viewData[4],
        counterInterval = undefined,
        create = false,
        activeClass = viewData[1],
        lists = {
            "mafia": {title: 'Мафия', class: 'mafia', desc: translateText('games', 'Примерь на себя одну из ролей разбойного города и прими участие в легендарной игре!')},
            "tanks": {title: 'Танки', class: 'tanks', desc: translateText('games', 'Останься последним выжившим в хардкорном сражении на настоящих танках!')},
            "airsoft": {title: 'Стрельба', class: 'airsoft', desc: translateText('games', 'Сражайтесь в режимах Team PVP и GunGame.')},
        },
        lobbyList = JSON.parse(viewData[0]),
        lobbyPlayersInfo = JSON.parse(viewData[3]),

        lobby_name = "",
        lobby_price = 1,
        lobby_password = null,
        lobby_mode = 0,
        lobby_weapon = 0,
        lobby_map = 0
    ;

    const lobby_mode_name = [
        translateText('games', '1x1 (до первого убийства)'),
        translateText('games', '1x1 (на время)'),
        translateText('games', 'Командный 2x2'),
        translateText('games', 'Командный 3x3'),
        translateText('games', 'Командный 5x5'),
        'GunGame'
    ];

    const lobby_weapon_name = [
        'Revolver',
        'Heavy Revolver Mk II',
        'Navy Revolver',
        'Double Action Revolver',
        'Marksman Pistol',
        'Pistol .50',

        'Ray Pistol',
        'Rail Gun',
        'RPG',
    
        'Assault SMG',
        'Micro SMG',
        'Mini SMG',
        'SMG',
        'Combat PDW',
        
        'Heavy Shotgun',
        'Musket',
        'Assault Shotgun',
        
        'Assault Rifle',
        'Assault Rifle Mk II',
        'Carbine Rifle',
        'Carbine Rifle Mk II',
        'Special Carbine',
        'Compact Rifle',
        
        'Combat MG',
        'Combat MG Mk II',
        'MG',
        'Gusenberg Sweeper',

        'Marksman Rifle',
        'Sniper Rifle',
        'Heavy Sniper',
        'Heavy Sniper Mk II'
    ];

    const lobby_map_name = {
        "airsoft": [
            'Дамба',
            'Казино',
            `Высотка Vinewood`,
            'Обсерватория',
            'Крыша',
            'Лесник*',
            'Dust*'
        ],
        "mafia": [
            translateText('games', 'Казино'),
            translateText('games', 'Костёр')
        ],
        "tanks": [
            'Порт'
        ]
    };

    const lobby_map_img_src = {
        "airsoft": [
            map_0,
            map_1,
            map_2,
            map_3,
            map_4,
            map_5,
            map_6
        ],
        "mafia": [
            mafia1,
            mafia2
        ],
        "tanks": [
            tanks1
        ],
    };

    const createLobby = () => {
        if (lobby_name == undefined || lobby_name.length < 0 || lobby_name.length > 16) {
            window.notificationAdd(1, 9, translateText('games', 'Неверно указано название лобби.'), 3000);
            return;
        }
        
        if (lobby_price == undefined || lobby_price < 1 || lobby_price > 10000) {
            window.notificationAdd(1, 9, translateText('games', 'Цена за вхождение в лобби может быть минимум 1$ и максимум 10.000$'), 3000);
            return;
        }

        if (lobby_map == 4 && lobby_mode != 0) {
            window.notificationAdd(1, 9, translateText('games', 'Эта карта доступна только для режима 1x1 (до первого убийства)'), 3000);
            return;
        }
        
        if (activeClass == 'airsoft') {
            executeClient ('airsoft_createLobby_client', lobby_name, Math.round (lobby_price), lobby_password, lobby_mode, lobby_weapon, lobby_map);
        }
        else if (activeClass == 'mafia') {
            executeClient ('mafia_createLobby_client', lobby_name, Math.round (lobby_price), lobby_password, lobby_map);
        }
        else if (activeClass == 'tanks') {
            executeClient ('tanks_createLobby_client', lobby_name, Math.round (lobby_price), lobby_password, lobby_map);
        }
    }

    const joinLobby = (index, password) => {
        if (password == 0) {
            if (activeClass == 'airsoft') {
                executeClient ('airsoft_joinLobby_client', index);
            }
            else if (activeClass == 'mafia') {
                executeClient ('mafia_joinLobby_client', index);
            }
            else if (activeClass == 'tanks') {
                executeClient ('tanks_joinLobby_client', index);
            }
        } else {
            if (activeClass == 'airsoft') {
                executeClient ('airsoft_joinPrivateLobby_client', 1, index);
            }
            else if (activeClass == 'mafia') {
                executeClient ('mafia_joinPrivateLobby_client', 1, index);
            }
            else if (activeClass == 'tanks') {
                executeClient ('tanks_joinPrivateLobby_client', 1, index);
            }
        }
    }

    const handleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        CloseLobby ();
    }

    const CloseLobby = () => {
        if (!popup) {
            executeClient ('airsoft_lobbyMenuHandler', 2);
        } else {
            executeClient ('airsoft_lobbyMenuHandler', 4);
        }
    }
    
    const updatePopupInfo = (status, data, cSeconds = -1) => {
        popup = status;
        lobbyPlayersInfo = JSON.parse(data);
        
        if (cSeconds > -1)
            counterSeconds = cSeconds

        if (ProgressBarId)
            ProgressBarId.animate(lobbyPlayersInfo[0] / lobbyPlayersInfo[2]);
    };

    window.events.addEvent("cef.lobby.updatePopupInfo", updatePopupInfo);

    const updateLobbyList = (data) => {
        lobbyList = JSON.parse(data);
    };

    window.events.addEvent("cef.lobby.updateLobbyList", updateLobbyList);

    import { onMount } from 'svelte';
    onMount(() => {
        if (counterInterval !== undefined) {
            clearInterval(counterInterval);
            counterInterval = undefined;
        }

        counterInterval = setInterval(() => {
            if (counterSeconds !== undefined && counterSeconds > 0) {
                if (activeClass == "airsoft" && lobbyPlayersInfo[1] == lobbyPlayersInfo[2] || activeClass == "airsoft" && lobbyPlayersInfo[0] < lobbyPlayersInfo[1]) {
                    return;
                }

                counterSeconds -= 1;

                if (counterSeconds <= 0) {
                    clearInterval(counterInterval);
                    counterInterval = undefined;
                }
            }
        }, 1000);
    });

    import { onDestroy } from 'svelte';
    onDestroy(() => {
        window.events.addEvent("cef.lobby.updatePopupInfo", updatePopupInfo);
        window.events.addEvent("cef.lobby.updateLobbyList", updateLobbyList);
    });

	const CreateProgressBar = () => {
        ProgressBarId = new ProgressBar.Circle("#ProgressBarKey", {
            color: '#FF4040',
            strokeWidth: 6,
            trailWidth: 6,
            easing: 'easeInOut',
            duration: 250,
            trailColor: 'rgba(55,63,70,1.0)', //цвет фонового кружка
            text: {
                autoStyleContainer: false
            },
            from: { color: '#FFFFFF', width: 6 },
            to: { color: '#FFFFFF', width: 6 },
            // Set default step function for all animate calls
            step: function(state, circle) {
                circle.path.setAttribute('stroke-width', state.width);
            }
        });
        ProgressBarId.animate(lobbyPlayersInfo[0] / lobbyPlayersInfo[2]);
    };
</script>

<svelte:window on:keyup={handleKeyDown} />

<div id="games">
    {#if !popup}
        <div class="games-lobby">
            <div class="games-lobby-left">
                <div class="games-lobby-header">
                    <div class="games-lobby-icon read"/>
                    <div class="games-lobby-left-title">{translateText('games', 'Создание лобби')}</div>
                </div>

                {#if activeClass == "airsoft"}
                    <div class="games-lobby-left-desc">
                        {translateText('games', 'Чтобы создать лобби для игры с друзьями или случайными людьми - нужно нажать на кнопку `Создать свою игру`, после чего установить название лобби, цену входа (из неё будет сложен призовой фонд игры) и пароль (можно и без пароля), затем выбрать карту, игровой режим и оружие, на котором будет проводиться игра. Отдельного внимания заслуживает режим GunGame, который находится в самом низу списка режимов. В нём вам предстоит двигаться вверх по лестнице убийств, каждый раз получая получая всё более мощное оружие. Советуем попробовать!')}
                    </div>
                {:else}
                    <div class="games-lobby-left-desc">
                        {translateText('games', 'Чтобы создать лобби для игры с друзьями или случайными людьми - нужно нажать на кнопку `Создать свою игру`, после чего установить название лобби, цену входа (из неё будет сложен призовой фонд игры) и пароль (можно и без пароля), затем выбрать карту, на которой будет проводиться игра.')}
                    </div>
                {/if}

                <div class="games-lobby-header s1">
                    <div class="games-lobby-icon games"/> 
                    <div class="games-lobby-left-title">{translateText('games', 'Выбор ивента')}</div>
                </div>
                
                <ul class="games-lobby-left-list">
                    <div class="games-lobby-left-flex">
                        <div class="games-lobby-left-list-leftIcon"><div class={'games-lobby-left-list-icon ' + activeClass}/></div>
                        <div class="games-lobby-left-list-content">
                            <div class="games-lobby-left-list-content-flex">
                                <div class="games-lobby-left-list-content-text">{lists[activeClass].title}</div>
                                <div class="games-lobby-btn">{translateText('games', 'Вы уже тут!')}</div>
                            </div>
                            <div class="games-lobby-left-list-content-desc">{lists[activeClass].desc}</div>
                        </div>
                    </div>
                </ul>
            </div>

            <div class="games-lobby-center">
                {#if create}
                <div class="games-lobby-header s3">
                    <div class="games-lobby-header-title">
                        <div class="games-lobby-icon pencil"/> {translateText('games', 'Создание лобби')}
                    </div>
                </div>

                <div class="games-lobby-header s4">
                    <div class="games-lobby-left-title">{translateText('games', 'Основные параметры')}</div>
                </div>

                <ul class="games-lobby-sett-start">
                    <li class="games-lobby-column-style left">
                        <div class="games-lobby-sett-start-header"><div class="games-lobby-icon award"/>{translateText('games', 'Название')}</div>
                        <input type="text" on:input={(event) => lobby_name = event.target.value} class="games-lobby-settings-maps-input" placeholder={translateText('games', 'Максимум 10')} maxlength="10"/>
                    </li>
                    <li class="games-lobby-column-style">
                        <div class="games-lobby-sett-start-header"><div class="games-lobby-icon dollar"/>{translateText('games', 'Цена входа')}</div>
                        <input type="text" on:input={(event) => {
                            event.target.value = Math.round(event.target.value.replace(/\D+/g, ""));

                            if (event.target.value < 0) {
                                event.target.value = 0;
                            }
                            else if (event.target.value > 10000) {
                                event.target.value = 10000;
                            }

                            lobby_price = event.target.value;
                        }} class="games-lobby-settings-maps-input" placeholder="10000"/>
                    </li>
                    <li class="games-lobby-column-style right">
                        <div class="games-lobby-sett-start-header"><div class="games-lobby-icon closed"/>{translateText('games', 'Пароль лобби')}</div>
                        <input type="password" bind:value={lobby_password} class="games-lobby-settings-maps-input" maxlength="10" placeholder={translateText('games', 'Пароль')}/>
                    </li>
                </ul>

                <div class="games-lobby-header s4">
                    <div class="games-lobby-left-title">{translateText('games', 'Дополнительные параметры')}</div>
                </div>

                <ul class="games-lobby-sett-start s1">
                    <li>
                        <div class="games-lobby-sett-start-header"><div class="games-lobby-icon maps"/>{translateText('games', 'Выбор карты')}</div>
                        <div class="radioList">
                            {#each lobby_map_name[activeClass] as name, index}
                                <div class="form_radio">
                                    {#if index == 0}
                                        <input id="lobby_map_radio-{index}" type="radio" name="radio" value="{index + 1}" checked on:click={() => lobby_map = index}>
                                    {:else}
                                        <input id="lobby_map_radio-{index}" type="radio" name="radio" value="{index + 1}" on:click={() => lobby_map = index}>
                                    {/if}
                                    <label for="lobby_map_radio-{index}">{lobby_map_name[activeClass][index]}</label>
                                </div>
                            {/each}
                        </div>
                    </li>
                    {#if activeClass == "airsoft"}
                        <li>
                            <div class="games-lobby-sett-start-header"><div class="games-lobby-icon helmet"/>{translateText('games', 'Режим Боя')}</div>
                            <div class="radioList">
                                {#each lobby_mode_name as name, index}
                                    <div class="form_radio">
                                        {#if index == 0}
                                            <input id="lobby_name_radio-{index}" type="radio" name="radio-battle" value="{index + 1}" checked on:click={() => lobby_mode = index}>
                                        {:else}
                                            <input id="lobby_name_radio-{index}" type="radio" name="radio-battle" value="{index + 1}" on:click={() => lobby_mode = index}>
                                        {/if}
                                        <label for="lobby_name_radio-{index}">{lobby_mode_name[index]}</label>
                                    </div>
                                {/each}
                            </div>
                        </li>
                        {#if lobby_mode != 5}
                        <li>
                            <div class="games-lobby-sett-start-header"><div class="games-lobby-icon aim"/>{translateText('games', 'Оружие')}</div>
                            <div class="radioList">
                                {#each lobby_weapon_name as name, index}
                                    <div class="form_radio">
                                        {#if index == 0}
                                            <input id="lobby_weapon_radio-{index}" type="radio" name="radio-aim" value="{index + 1}" checked on:click={() => lobby_weapon = index}>
                                        {:else}
                                            <input id="lobby_weapon_radio-{index}" type="radio" name="radio-aim" value="{index + 1}" on:click={() => lobby_weapon = index}>
                                        {/if}
                                        <label for="lobby_weapon_radio-{index}">{lobby_weapon_name[index]}</label>
                                    </div>
                                {/each}
                            </div>
                        </li>
                    {/if}
                {/if}
                </ul>
                {:else}
                <div class="games-lobby-header">
                    <div class="games-lobby-header-title">
                        {translateText('games', 'Список лобби')}
                    </div>
                    <div class="games-lobby-btn" on:click={() => create = true}><div class="games-lobby-icon pencil"/>{translateText('games', 'Создать свою игру')}</div>
                </div>

                <ul class="games-lobby-table">
                    {#each lobbyList as l, index}
                        {#if l.class == activeClass}
                            <li on:click={() => joinLobby(l.lobby_number, l.join_password)}>
                                <div class="games-lobby-table-row"><div class="games-lobby-icon award"/>{l.title}</div>
                                <div class="games-lobby-table-row"><div class="games-lobby-icon dollar"/>{l.price}</div>
                                {#if activeClass == "airsoft"}
                                    <div class="games-lobby-table-row"><div class="games-lobby-icon helmet"/>{lobby_mode_name[l.mode]}</div>
                                {/if}
                                <div class="games-lobby-table-row"><div class={"games-lobby-icon " + (l.close ? "closed":"open")}/></div>
                            </li>
                        {/if}
                    {/each}
                </ul>
                {/if}
            </div>
            <div class="games-lobby-right">
                {#if create}
                <div class="games-lobby-header s2">
                    <div class="games-lobby-icon settings"/>
                    <div class="games-lobby-left-title">{translateText('games', 'Параметры лобби')}</div>
                </div>

                <div class="games-lobby-flex">
                    <ul class="games-lobby-settings">
                            <li>
                                <div class="games-lobby-settings-header">
                                    <div class="games-lobby-icon award"/>{translateText('games', 'Название')}
                                </div>
                                <div class="games-lobby-settings-header-desc">
                                    {lobby_name}
                                </div>
                            </li>
                            <li>
                                <div class="games-lobby-settings-header">
                                    <div class="games-lobby-icon dollar"/>{translateText('games', 'Цена входа')}
                                </div>
                                <div class="games-lobby-settings-header-desc">
                                    $ {lobby_price}
                                </div>
                            </li>
                            {#if activeClass == "airsoft"}
                                <li>
                                    <div class="games-lobby-settings-header">
                                        <div class="games-lobby-icon aim"/>{translateText('games', 'Оружие')}
                                    </div>
                                    <div class="games-lobby-settings-header-desc">
                                        {lobby_weapon_name[lobby_weapon]}
                                    </div>
                                </li>
                                <li>
                                    <div class="games-lobby-settings-header">
                                        <div class="games-lobby-icon helmet"/>{translateText('games', 'Режим боя')}
                                    </div>
                                    <div class="games-lobby-settings-header-desc">
                                        {lobby_mode_name[lobby_mode]}
                                    </div>
                                </li>
                            {/if}
                    </ul>

                    <div class="games-lobby-settings-maps">
                        <div class="games-lobby-settings-maps-image"><img src={lobby_map_img_src[activeClass][lobby_map]} alt="maps"/></div>
                        <div class="games-lobby-settings-maps-title">{translateText('games', 'Карта')}: {lobby_map_name[activeClass][lobby_map]}</div>
                        <div class="games-lobby-btn" on:click={createLobby}>{translateText('games', 'Создать')}</div>
                    </div>
                </div>
                {/if}
            </div>
        </div>
        
        <div class={"games-help-info " + (popup ? 'active':'')} on:click={CloseLobby}>
            <div class="games-help-info-subtitle">
                <div class="games-help-info-icon"/>
                <div class="games-help-info-text">ESС</div>
            </div>
            <p>{translateText('games', 'Нажмите чтобы закрыть выбор игр')}</p>
        </div>
    {:else}
        <div class="games-lobby-popup">
            <div id="ProgressBarKey" class="games-lobby-popup-content" use:CreateProgressBar>            
                <div class="games-lobby-popup-content-centerCircle">
                    <div class="games-lobby-popup-content-title">{lobbyPlayersInfo[0]}/{lobbyPlayersInfo[0] < lobbyPlayersInfo[1] ? lobbyPlayersInfo[1] : lobbyPlayersInfo[2]}</div>
                    <div class="games-lobby-popup-content-desc">{((Math.floor(counterSeconds / 60) < 10 ? '0' : '') + Math.floor(counterSeconds / 60) + ':' + (counterSeconds % 60 < 10 ? '0' : '') + counterSeconds % 60)}</div>
                </div>
            </div>
            <div class="games-lobby-popup-h">
                {translateText('games', 'Минимальное кол-во игроков')}: {lobbyPlayersInfo[1]}<br>
                {translateText('games', 'Максимальное кол-во игроков')}: {lobbyPlayersInfo[2]}
            </div>
        </div>
    {/if}
</div>