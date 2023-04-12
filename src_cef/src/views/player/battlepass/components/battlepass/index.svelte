<script>
    import { translateText } from 'lang'
    import { onMount } from 'svelte';

    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { addListernEvent } from 'api/functions'
    import { GetTime } from 'api/moment'

    import Awards from './awards.svelte'
    import Tasks from './tasks.svelte'


    import { isPopupInfoOpened, isPopupLvlOpened } from '../../stores.js'
    onMount(async () => {
        let bg = document.querySelector('.battlepass__person');
        window.addEventListener('mousemove', function(e) {
            let x = e.clientX / window.innerWidth;
            let y = e.clientY / window.innerHeight;  
            bg.style.transform = 'translate(-' + x * 50 + 'px, -' + y * 50 + 'px)';
        });
    });

    let awardsCount = 0;
    let maxPage = 0;
    executeClientAsyncToGroup("getAwardsCount").then((result) => {
        awardsCount = result;
        maxPage = Math.floor ((awardsCount - 1) / maxCountAwards);
    });

    //

    const maxCountAwards = 6;
    let selectPage = 0;
    let currentLvl = 0;

    const getLvl = (isInit = false) => {
        executeClientAsyncToGroup("getLvl").then((result) => {
            currentLvl = result;

            if (isInit) {
                selectPage = Math.floor((currentLvl - 1) / maxCountAwards);
                if (selectPage < 0)
                    selectPage = 0;
            }
        });
    }

    getLvl (true);

    //

    const maxExp = 50;
    let currentExp = 0;
    const getExp = () => {
        executeClientAsyncToGroup("getExp").then((result) => {
            currentExp = result;
        });
    }

    getExp ();

    //

    addListernEvent ("battlePassUpdateLvlAndExp", () => {
        getLvl ();
        getExp ();
    })

    //

    const onTakeEverything = () => {
        if (!window.loaderData.delay ("battlePass.onTakeEverything", 2))
            return;

        executeClientToGroup ("takeAll")
    }

    const onPage = (number) => {

        number += selectPage;

        if (number >= 0 && number <= maxPage)
            selectPage = number;
    }

    //
    const maxMinutesAddExp = 60 * 3;

    let minutesAddExp = 0;

    import { serverDateTime } from 'store/server'

    let minutes = -1;

    serverDateTime.subscribe((dateTime) => {
        const moment = GetTime (dateTime);

        if (moment.minutes() !== minutes) {
            if (minutes !== -1)
                minutesAddExp++;

            minutes = moment.minutes();
        }
    });

    executeClientAsyncToGroup("getMinutesAddExp").then((result) => {
        minutesAddExp = result;
    });

    const getTimeFromMins = (mins) => {
        let hours = Math.trunc(mins / 60);
        let minutes = mins % 60;
        return hours + translateText('player', ' ч. ') + minutes + translateText('player', ' м.');
    }



    //

    let isAllAwardsTaked = false;

    const getIsAllAwardsTaked = () => {

        executeClientAsyncToGroup("isAllAwardsTaked").then((result) => {
            isAllAwardsTaked = result;
        });
    }

    getIsAllAwardsTaked ();

    addListernEvent ("isAllAwardsTaked", () => {
        getIsAllAwardsTaked ();
    })

</script>
<div class="box-between w-1">
    <div class="battlepass__level">
        <div class="box-between">
            <div class="battlepass__subtitle">4 {translateText('player', 'сезон')}</div>
            <div class="battlepass__subtitle">{currentExp} / {maxExp} <span class="red">XP</span></div>
        </div>
        <div class="battlepass__level_progress">
            <div class="current__progress" style="width: {currentExp / maxExp * 100}%"></div>
        </div>
        <div class="box-between">
            <div class="gray">{maxExp - currentExp} XP {translateText('player', 'до следующего уровня')}</div>
            <div>
                <span class="red">{translateText('player', 'До получения')} 2 XP: </span>{getTimeFromMins (maxMinutesAddExp - minutesAddExp)}
            </div>
        </div>
    </div>
    <div class="box-between w-1">
        <div class="battlepass__button black" on:click={() => isPopupLvlOpened.set(true)}>{translateText('player', 'КУПИТЬ УРОВНИ')}</div>
        {#if isAllAwardsTaked}
        <div class="battlepass__button" on:click={onTakeEverything}>{translateText('player', 'ЗАБРАТЬ НАГРАДЫ')}</div>
        {/if}
    </div>
</div>

<div class="battlepass__main">
    <Tasks />
    <Awards {selectPage} {currentLvl} />
</div>

<div class="box-between w-1">
    <div class="box-center box-flex" on:click={() => isPopupInfoOpened.set(true)}>
        <div class="battlepass__image_info"></div>
        <div class="battlepass__info_button">{translateText('player', 'Информация о мероприятии')}</div>
    </div>
    <div class="box-flex">
        <div class="battlepass__page_button left" on:click={() => onPage (-1)}></div>
        <div class="battlepass__info_text">{translateText('player', 'Страница')} {selectPage + 1} {translateText('player', 'из')} {maxPage + 1}</div>
        <div class="battlepass__page_button right" on:click={() => onPage (1)}></div>
    </div>
</div>