<script>
    import { translateText } from 'lang'
    import '../sass/games.sass';
    import { executeClient } from 'api/rage';
    
    //roles
    import unknown from '../images/roles/unknown.png';
    import mafia from '../images/roles/mafia.png';
    import komisar from '../images/roles/komisar.png';
    import doctor from '../images/roles/doctor.png';
    import courtesan from '../images/roles/courtesan.png';
    import civilian from '../images/roles/civilian_man.png';

    //action
    import act_cure from '../images/action/cure.png';
    import act_kill from '../images/action/kill.png';
    import act_killed from '../images/action/killed.png';
    import act_night from '../images/action/night.png';
    import act_unknown from '../images/action/unknown.png';

    export let viewData;

    let
        timesday = viewData[2], // day/ night
        voteStatus = viewData[1],
        header_title = viewData[3],
        header_text = viewData[4],

        helpID = 0,
        helpArr = [
            {title: translateText('games', 'Ожидайте когда у вас загорится зеленый микрофон,'), desc: translateText('games', 'после этого удерживайте клавишу N для разговора.')},
            {title: translateText('games', 'Не раскрывайте свою роль.'), desc: translateText('games', 'Мафия может воспользоваться этим и убить вас.')},
            {title: translateText('games', 'Следите за интонацией игроков.'), desc: translateText('games', 'Кто-то из них может раскрыть себя.')},
            {title: translateText('games', 'Будучи мафиози, отводите от себя подозрения.'), desc: translateText('games', 'Если мирные не подозревают вас - вы на верном пути.')},
            {title: translateText('games', 'Внимательно слушайте диктора.'), desc: translateText('games', 'Он подробно объясняет очередность ходов и процесс игры.')},
            {title: translateText('games', 'Если вы Комиссар, то будьте осторожны раскрывая себя.'), desc: translateText('games', 'Мафиози воспользуются этим и постараются убрать вас.')},
            {title: translateText('games', 'Доктор может вылечить любого игрока с очередностью в два хода.'), desc: translateText('games', 'Себя доктор может лечить лишь 1 раз.')},
            {title: translateText('games', 'Если в игре присутствует куртизанка, то она принимает мирную роль.'), desc: translateText('games', 'Куртизанка может остановить ход любой активной роли.')},
            {title: translateText('games', 'Во время общей минуты будьте внимательны.'), desc: translateText('games', 'Мафиози могут совместно указывать на мирного игрока.')},
            {title: translateText('games', 'Мафия - это лишь психологическая игра.'), desc: translateText('games', 'Соблюдайте позитивную обстановку.')},
            {title: translateText('games', 'Имейте в виду, что мафия может прикрываться другой ролью.'), desc: translateText('games', 'Вовремя раскройте свою роль, если заметите это.')},
            {title: translateText('games', 'Учитывайте, что если в тюрьму вы отправили мирного игрока,'), desc: translateText('games', 'то шансы на победу мафии увеличиваются.')},
            {title: translateText('games', 'При нахождении любых недоработок'), desc: translateText('games', 'сообщите об этом в Discord-канал bug-report.')},
            {title: translateText('games', 'Играя за мафию имейте в виду,'), desc: translateText('games', 'что обе мафии должны принять общее решение об убийстве.')}
        ],
        

        playersList = JSON.parse(viewData[0]),

        standby_screen_status = viewData[5],
        standby_screen_seconds = viewData[7],
        standby_screen_interval = undefined,

        main_hint_id = 0,
        actionIdIconShow = -1,
        playerRole = viewData[6]
    ;
    
    /*
        Пример заполнения:

        playersList = [
            {id: 1, name: 'Lloyd_Snipes', role: {name: "Мирный житель", img: civilian_girl}, mute: true, life: true},
            {id: 2, name: 'Lloyd_Snipes', role: {name: "Доктор", img: doctor, action: {title: "Вылечить", act: "cure", img: act_cure}}, mute: true, life: true},
            {id: 3, name: 'Lloyd_Snipes', role: {name: "Мафия", img: mafia, action: {title: "Убить игрока", act: "kill", img: act_kill}}, mute: true, life: true},
            {id: 4, name: 'Lloyd_Snipes', role: {name: "Коммисар", img: komisar, action: {title: "Хм кто же он?", act: "unknown", img: act_unknown}}, mute: false, life: true},
            {id: 5, name: 'Lloyd_Snipes', role: {name: "Дон Мафия", img: donmafia, action: {title: "Убить игрока", act: "kill", img: act_kill}}, mute: true, life: true},
            {id: 6, name: 'Lloyd_Snipes', role: {name: "Куртизанка", img: courtesan, action: {title: "Провести ночь с", act: "night", img: act_night}}, mute: true, life: true},
            {id: 7, name: 'Lloyd_Snipes', role: {name: "Мирный житель", img: civilian_man}, mute: true, life: true},
            {id: 8, name: 'Lloyd_Snipes', role: {name: "Мирный житель", img: civilian_man}, mute: true, life: true},
            {id: 9, name: 'Lloyd_Snipes', role: {name: "Мирный житель", img: mafia}, mute: true, life: true},
            {id: 10, name: 'Lloyd_Snipes', role: {name: "Мирный житель", img: unknown}, mute: true, life: true},
        ];
    */

    const roles_info = [
        {
            name: translateText('games', 'Мафия'),
            img: mafia,
            action: act_kill,
            actionClass: "kill",
            actionText: translateText('games', 'Убить игрока')
        },
        {
            name: translateText('games', 'Комиссар'),
            img: komisar,
            action: act_unknown,
            actionClass: "unknown",
            actionText: translateText('games', 'Хм, кто же он?')
        },
        {
            name: translateText('games', 'Доктор'),
            img: doctor,
            action: act_cure,
            actionClass: "cure",
            actionText: translateText('games', 'Вылечить')
        },
        {
            name: translateText('games', 'Куртизанка'),
            img: courtesan,
            action: act_night,
            actionClass: "night",
            actionText: translateText('games', 'Провести ночь с')
        },
        {
            name: translateText('games', 'Мирный житель'),
            img: civilian
        },
        {
            name: translateText('games', 'Неизвестно'),
            img: unknown
        }
    ];

    //

    const main_hint_texts = [
        `<div style="text-align: justify; text-indent: 25px;"><center>${translateText('games', 'Приветствуем тебя на игре')} &laquo;<strong>${translateText('games', 'Мафия')}</strong>&raquo;!</center></div><br>
        <div style="text-align: justify; text-indent: 25px;"><center>${translateText('games', 'В мафию играют от')} <strong>6</strong> ${translateText('games', 'до')} <strong>15</strong> ${translateText('games', 'человек. Сервер случайным образом распределяет роли.')}</center></div>
        <div style="text-align: justify; text-indent: 25px;"><center> ${translateText('games', 'Роль может быть')} <strong>${translateText('games', 'активная')}</strong>: ${translateText('games', '(мафия, комиссар, доктор, куртизанка) или')} <strong>${translateText('games', 'пассивная')}</strong> (${translateText('games', 'мирный житель')}).</center></div>
        
        <br>
        <div style="text-align: justify; text-indent: 25px;">${translateText('games', 'В игре есть два времени суток')}:</div>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Ночь')}</strong> &ndash; &nbsp;${translateText('games', 'период, когда ходят игроки с активными ролями')};</div>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'День')}</strong> &ndash; ${translateText('games', 'период, когда все игроки обсуждают итоги ночи и голосуют кого хотят посадить в тюрьму')}.</div>
        <br>
        <div style="text-align: justify; text-indent: 25px;">${translateText('games', 'Коротко о каждой роли')}:</div>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Мафия')}</strong> ${translateText('games', 'обсуждает ночью кого хотят убить, после обсуждения выбирают жертву. Если будут выбраны разные игроки, то будет убит тот игрок, за которого больше всего проголосовали. Задача мафии -; убить всех жителей и выжить')};</div>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Комиссар')}</strong> ${translateText('games', 'ночью совершает проверку игрока и узнает является ли этот игрок мафией')};</div>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Доктор')}</strong> ${translateText('games', 'ночью выбирает игрока, к которому придет на помощь. Если доктор попадёт на того, кого хотела убить мафия, то этот игрок не умрёт')};</div>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Куртизанка')}</strong> ${translateText('games', 'ночью выбирает игрока, с которым будет развлекаться до самого утра. Тот, кого выбрала куртизанка, не сможет совершать никаких действий. Если выбранный игрок был с активной ролью, то действие данного игрока отменяется.')}</div>`,
        
        `
        <div style="text-align: justify;text-indent: 25px;"><center>${translateText('games', 'Подробнее об активных ролях')}:</center></div><br>
         <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Мафия')}</strong> ${translateText('games', 'путем голования выбирает цель, которую убьет. Чтобы убить определенного игрока, мафия должна единогласно проголосовать за одного и того же игрока. Если же будет выбрано несколько игроков, то будет убит тот игрок, за которого больше всего проголосовали.')}</div><br>
         <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Комиссару')}</strong> ${translateText('games', 'необходимо вычислить всех мафиози и во время обсуждений донести это до мирных жителей. Будьте осторожны, если Вы не сможете убедить игроков, то мафия может убить Вас во время следующей ночи.')} </div> <br>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Доктор')}</strong> ${translateText('games', 'должен выбрать игрока, к которому поедет на вызов. В случае, если доктор приедет к тому, кого пыталась убить мафия, то этот игрок выживает. Обращаем Ваше внимание, что доктор может осуществлять лечение одного и того же игрока лишь раз в два хода. Вылечить самого себя доктор может лишь раз за всю игру.')}</div><br>
        <div style="text-align: justify; text-indent: 45px;"><strong>&bull; ${translateText('games', 'Куртизанка')}</strong> ${translateText('games', 'может выбрать любого из игроков, с кем проведет ночь. Если куртизанка выбирает одну из активных ролей, то его действие не будет засчитано по окончанию ночи, будь то выезд врача или убийство.')}</div><br>`,
          ` <div style="text-align: justify;text-indent: 25px;"><center>${translateText('games', 'Ночь заканчивается, когда все игроки с активными ролями сделают свой ход.')}</center></div><br>
        <div style="text-align: justify;text-indent: 25px;"><center>${translateText('games', 'Днем происходит обсуждение. Все жители пытаются вычислить кто в игре мафия. Каждому игроку открывается доступ к микрофону и чату на')} <strong>30</strong> ${translateText('games', 'секунд. После обсуждения, игроки путём голосования выбирают кого посадить в тюрьму. Арестованный игрок выбывает из игры.')}</center></div><br>
        <div style="text-align: justify; text-indent: 25px;"><center>${translateText('games', 'Всем желаем приятной игры! Скоро будут распределены роли')}.</center></div>`
    ];

    const switchMainHint = (state) => {
        if (state == 1) {
            if ((main_hint_id - 1) < 0) {
                main_hint_id = (main_hint_texts.length - 1);
            } else {
                main_hint_id -= 1;
            }
        }
        else if (state == 2) {
            if ((main_hint_id + 1) >= main_hint_texts.length) {
                main_hint_id = 0;
            } else {
                main_hint_id += 1;
            }
        }

        updateHintButtonStatus();
    };

    //

    const updateHintButtonStatus = () => {
        let sliders_divs = document.querySelectorAll(".popup-mafia__slider");
        if (sliders_divs) {
            sliders_divs.forEach(item => {
                item.classList.remove("active");
            });

            document.getElementById(`main_hint_slider_${main_hint_id}`).classList.add("active");
        }
    };

    //

    const updateGameWindow = (playersListData, voteStatusData, timesdayData, actionInfoData, actionTimeInfoData, standbyScreenStatusData) => {
        playersList = JSON.parse(playersListData);
        voteStatus = voteStatusData;
        timesday = timesdayData;
        header_title = actionInfoData;
        header_text = actionTimeInfoData;
        standby_screen_status = standbyScreenStatusData;
    };
    
    window.events.addEvent("cef.mafiaGame.updateGameWindow", updateGameWindow);
    
    //

    const updateVoteStatus = (status) => {
        voteStatus = status;
    };
    
    window.events.addEvent("cef.mafiaGame.updateVoteStatus", updateVoteStatus);

    //

    const updateStandbyScreenStatus = (status, seconds = 0) => {
        standby_screen_status = status;
        standby_screen_seconds = seconds;

        main_hint_id = 0;

        updateHintButtonStatus();
    };

    window.events.addEvent("cef.mafiaGame.updateStandbyScreenStatus", updateStandbyScreenStatus);

    //

    import { onMount } from 'svelte';
    onMount(() => {
        if (standby_screen_interval !== undefined) {
            clearInterval(standby_screen_interval);
            standby_screen_interval = undefined;
        }

        standby_screen_interval = setInterval(() => {
            if (standby_screen_seconds !== undefined && standby_screen_seconds > 0) {
                standby_screen_seconds -= 1;

                if (standby_screen_seconds <= 0) {
                    clearInterval(standby_screen_interval);
                    standby_screen_interval = undefined;
                    executeClient ('updateClientMafiaStandbyStatus', true);
                }
            }
        }, 1000);
    });

    //

    import { onDestroy } from 'svelte';
    onDestroy(() => {
        window.events.addEvent("cef.mafiaGame.updateGameWindow", updateGameWindow);
        window.events.addEvent("cef.mafiaGame.updateVoteStatus", updateVoteStatus);
        window.events.addEvent("cef.mafiaGame.updateStandbyScreenStatus", updateStandbyScreenStatus);
    });
</script>

<div id="games">
    <div class="games-mafia">
        <div class="games-mafia-header">
            <div class={"games-mafia-icon " + (timesday ? "day" : "night")}></div>
            <div class="games-mafia-header-title">{header_title}</div>
            <div class="games-mafia-header-text">{header_text}</div>
        </div>

        <ul class="games-mafia-list {playersList.length > 10 ? "pad" : ""}">
            {#each playersList as p, index}
                <li>
                    {#if timesday}
                        <div class={"games-mafia-icon " + ((p.mute || !p.life) ? "mute" : "unmute")}/>
                    {/if}
                    <div class="games-mafia-list-title">Игрок {p.id}</div>
                    <div class="games-mafia-list-subtitle">{p.name}</div>
                    <div class="games-mafia-list-role" on:mouseenter={() => actionIdIconShow = p.id} on:mouseleave={() => actionIdIconShow = -1} on:click={
                            () => {
                                if (voteStatus && p.life) {
                                    executeClient ('voteMafiaGame_client', p.id);
                                }
                            }
                        }>
                        {#if !p.life}
                            <div class={"games-mafia-list-action killed"}>
                                <img src={act_killed} alt="action"/>
                                <div class="games-mafia-list-action-title">{translateText('games', 'Игрок мертв')}</div>
                            </div>
                        {:else if actionIdIconShow == p.id && voteStatus && !timesday && playerRole <= 3}
                            <div class={"games-mafia-list-action " + (roles_info[playerRole].actionClass)}>
                                <img src={roles_info[playerRole].action} alt="action"/>
                                <div class="games-mafia-list-action-title">{roles_info[playerRole].actionText}</div>
                            </div>
                        {/if}
                        <img src={roles_info[p.role].img} alt="role" class="games-mafia-list-role-img"/>
                    </div>
                </li>
            {/each}
        </ul>

        <div class="games-mafia-help">
            <div class="games-mafia-icon stars"/>
                <div class="games-mafia-flex-column">
                    <div class="games-mafia-help-head">{translateText('games', 'Подсказка')} №{helpID + 1}</div>
                    <div class="games-mafia-help-title">{helpArr[helpID].title}</div>
                    <div class="games-mafia-help-subtitle">{helpArr[helpID].desc}</div>
                </div>
            </div>
        </div>
        
        {#if standby_screen_status}
            <div class="popup-mafia">
                <div class="popup-mafia__img-time">
                    <div class="popup-mafia__time">{((Math.floor(standby_screen_seconds / 60) < 10 ? '0' : '') + Math.floor(standby_screen_seconds / 60) + ':' + (standby_screen_seconds % 60 < 10 ? '0' : '') + standby_screen_seconds % 60)}</div>
                </div>
                <div class="popup-mafia-title">{translateText('games', 'Подсказка')} №{main_hint_id + 1}</div>
                <div class="popup-mafia-subtitle" contenteditable="false" bind:innerHTML={main_hint_texts[main_hint_id]} />
                <div class="box-flex">
                    <div class="popup-mafia__slider active" id="main_hint_slider_0"></div>
                    <div class="popup-mafia__slider" id="main_hint_slider_1"></div>
                    <div class="popup-mafia__slider" id="main_hint_slider_2"></div>
                </div>
                <div class="popup-mafia__buttons">
                    <div class="popup-mafia__button" on:click={() => switchMainHint(1)}>&larr;</div>
                    <div class="popup-mafia__button" on:click={() => switchMainHint(2)}>&rarr;</div>
                </div>
            </div>
        {/if}
</div>