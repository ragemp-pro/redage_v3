<script>
    import { translateText } from 'lang'
    import { setView, getLog } from "../../../data";
    import {executeClientAsyncToGroup} from "api/rage";
    import moment from 'moment';

    import { charUUID } from 'store/chars';

    let isLog = false;
    executeClientAsyncToGroup("isLog").then((result) => {
        isLog = result;
    });

    let logType = 0;

    const onGetLogs = (index) => {
        if (index !== 0 && !isLog)
            return;

        if (logType === index)
            return;

        logType = index;

        if (index === 1) //Общий
            getLog (-1, false, "", 0)
        else if (index === 0) //Личный
            getLog ($charUUID, false, "", 0)
        else if (index === 2) //Склад
            getLog (-1, true, "", 0)
    }

    const onOpenLogs = () => {
        if (!isLog)
            return;

        setView ("Logs")
    }

    let logs = []
    const getLogs = () => {
        executeClientAsyncToGroup("getLogs").then((result) => {
            if (result && typeof result === "string")
                logs = JSON.parse(result);
        });
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.logs", getLogs)
</script>
<div class="fractions__main_box big">
    <div class="fractions__main_head box-between">
        <div class="box-flex">
            <span class="fractionsicon-stats"></span>
            <div class="fractions__main_title">{translateText('player1', 'Лог действий')}</div>
        </div>
        <div class="fractions__stats_subtitle gray hover" on:click={onOpenLogs}>{translateText('player1', 'Подробнее')} ></div>
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
    <div class="fractions__main_scroll">
        {#if logs && logs.length}
            {#each logs as log}
                <div class="fractions__scroll_element">
                    {#if log.uuid}
                        <div class="box-column">
                            <div class="fractions_stats_title whitecolor logs">{log.name}</div>
                            <div class="fractions_stats_title">{log.rankName}</div>
                        </div>
                    {/if}
                    <div class="fractions_stats_title logscenter">{log.text}</div>
                    <div class="box-column">
                        <div class="fractions_stats_title whitecolor">{moment(log.time).format('DD.MM.YYYY')}</div>
                        <div class="fractions_stats_title">{moment(log.time).format('HH:mm')}</div>
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