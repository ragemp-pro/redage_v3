<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    import { charUUID } from 'store/chars';

    import moment from 'moment';
    let isLog = false;
    executeClientAsyncToGroup("isLog").then((result) => {
        isLog = result;
    });
    let logType = 0;

    let searchText = "";
    const onGetLogs = (index) => {
        if (index !== 0 && !isLog)
            return;

        if (logType === index)
            return;

        logType = index;

        if (!searchText)
            searchText = "";

        if (index === 1) //Общий
            getLog (-1, false, searchText, 0)
        else if (index === 0) //Личный
            getLog ($charUUID, false, searchText, 0)
        else if (index === 2) //Склад
            getLog (-1, true, searchText, 0)
    }

    let logs = []
    const getLogs = () => {
        executeClientAsyncToGroup("getLogs").then((result) => {
            if (result && typeof result === "string")
                logs = JSON.parse(result);
        });
    }

    getLogs();

    import { addListernEvent } from "api/functions";
    import {setView, getLog} from "../../data";
    addListernEvent ("table.logs", getLogs)

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13 && searchText && searchText.length >= 1) {
            onGetLogs ()
            searchText = ""
        }
    }

</script>

<svelte:window on:keyup={HandleKeyDown} />

<div class="fractions__stats_subtitle gray hover mt-20 large" on:click={() => setView ("Main")}>&#x2190; {translateText('player1', 'Вернуться к общей информации')}</div>
<div class="fractions__content backgr">
    <div class="fractions__main_head box-between">
        <div class="box-flex">
            <span class="fractionsicon-stats"></span>
            <div class="fractions__main_title">{translateText('player1', 'Лог действий')}</div>
        </div>
        <div class="fractions__input">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={searchText} type="text" placeholder={translateText('player1', 'Поиск..')}>
        </div>
    </div>
    <div class="box-flex">
        <div class="fractions__selector" class:active={logType === 0}  on:click={() => onGetLogs (0)}>
            {translateText('player1', 'Личный')}
        </div>
        <div class="fractions__selector" class:active={logType === 1} on:click={() => onGetLogs (1)}>
            {translateText('player1', 'Общий')}
        </div>
        <div class="fractions__selector" class:active={logType === 2}  on:click={() => onGetLogs (2)}>
            {translateText('player1', 'Склад')}
        </div>
    </div>
    <div class="box-between pr-22">
        {#if logs && logs[0] && logs[0].uuid}
            <div class="fractions_stats_title name">{translateText('player1', 'Имя')}</div>
            <div class="fractions_stats_title rank">{translateText('player1', 'Ранг')}</div>
        {/if}
        <div class="fractions_stats_title descr">{translateText('player1', 'Действие')}</div>
        <div class="fractions_stats_title date">{translateText('player1', 'Дата')}</div>
    </div>
    <div class="fractions__main_scroll big prscroll-22">
        {#if logs && logs.length}
            {#each logs as log}
                <div class="fractions__scroll_element">
                    {#if log.uuid}
                        <div class="fractions_stats_title whitecolor name">{log.name}</div>
                        <div class="fractions_stats_title whitecolor rank">{log.rankName}</div>
                    {/if}
                    <div class="fractions_stats_title taleft descr">{log.text}</div>
                    <div class="box-flex date">
                        <div class="fractions_stats_title whitecolor mr-5 w-max">{moment(log.time).format('DD.MM.YYYY')}</div>
                        <div class="fractions_stats_title w-max">{moment(log.time).format('HH:mm')}</div>
                    </div>
                </div>
            {/each}
        {:else}
            <div class="fractions__stats_subtitle box-center gray wh-100">
                {translateText('player1', 'Никаких действий не замечено!')}
            </div>
        {/if}
    </div>
</div>