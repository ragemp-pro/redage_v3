<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    export let selectMember;
    export let member;

    //Логи

    import moment from 'moment';
    let isLog = false;
    executeClientAsyncToGroup("isLog").then((result) => {
        isLog = result;
        onGetLogs (0);
    });
    let logType = -1;

    let searchText = "";
    const onGetLogs = (index) => {
        if (!isLog)
            return;

        if (logType === index)
            return;

        logType = index;

        if (!searchText)
            searchText = "";

        if (index === 0) //Общий
            getLog (selectMember.uuid, false, searchText, 0)
        else if (index === 1) //Склад
            getLog (selectMember.uuid, true, searchText, 0)
    }

    let logs = []
    const getLogs = () => {
        executeClientAsyncToGroup("getLogs").then((result) => {
            if (result && typeof result === "string")
                logs = JSON.parse(result);
        });
    }

    import { addListernEvent } from "api/functions";
    import {getLog, setPopup, onUpdateRank} from "../../data";
    addListernEvent ("table.logs", getLogs)

    import Access from '../access/index.svelte'

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13 && searchText && searchText.length >= 1) {
            onGetLogs ()
            searchText = ""
        }
    }

    //

    const onUpdateDepartmentCallback = (rank) => {
        executeClientToGroup('invitePlayerDepartment', rank, selectMember.uuid);

    }
    const onUpdateDepartment = () => {
        executeClientAsyncToGroup("getDepartments").then((result) => {
            if (result && typeof result === "string") {
                result = JSON.parse(result);

                if (!result || !result.length)
                    return;

                let departments = [];

                if (selectMember.departmentId > 0 && (settings.isLeader || result.findIndex(r => Number(i.id) === Number(member.departmentId) &&  Number(r.id) === Number(selectMember.departmentId)) !== -1))
                    departments.push({
                        id: 0,
                        name: "Без отряда"
                    })

                result.forEach(item => {
                    if (settings.isLeader || Number(item.id) === Number(member.departmentId)) {
                        departments.push(item)
                    }
                });

                setPopup("List", {
                    headerTitle: "Выдать очки",
                    headerIcon: "fractionsicon-members",
                    listTitle: 'Выберите отряд',
                    list: departments,
                    button: 'Выбрать',
                    callback: onUpdateDepartmentCallback
                })
            }
        });
    }

    const onReprimandCallback = (title, text) => {
        executeClientToGroup("reprimand", selectMember.uuid, title, text);
    }

    const onReprimand = () => {
        setPopup ("Input", {
            headerTitle: "Выговор",
            headerIcon: "fractionsicon-news",
            inputs: [
                {
                    title: "Заголовок выговора",
                    placeholder: "Введите заголовок",
                    maxLength: 20
                },
                {
                    title: "Текст выговора",
                    placeholder: "Введите текст",
                    maxLength: 100,
                    textarea: true
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onReprimandCallback
        })
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string") {
                settings = JSON.parse(result);

                if (settings.isLeader)
                    executeClientToGroup('rankAccessInit', JSON.stringify(selectMember.access), JSON.stringify(selectMember.lock))
            }
        });
    }
    getSettings();

</script>

<svelte:window on:keyup={HandleKeyDown} />
<div class="box-between mt-20">
    <div class="box-column">

        <div class="box-between">
            {#if settings.setRank}
            <div class="fractions__main_button extra" on:click={() => onUpdateRank ("Список рангов", "fractionsicon-members", "Выберите ранг", 'updatePlayerRank', selectMember.uuid)}>
                {translateText('player1', 'Сменить ранг')}
            </div>
            {/if}
            {#if settings.isLeader || (member.departmentRank === 3)}
            <div class="fractions__main_button extra" on:click={onUpdateDepartment}>
                {translateText('player1', 'Сменить отряд')}
            </div>
            {/if}
            {#if settings.reprimand}
            <div class="fractions__main_button extrasmall" on:click={onReprimand}>
                {translateText('player1', 'Выговор')}
            </div>
            {/if}
        </div>

        {#if settings.isLeader}
            <Access title={{
                    icon: 'fractionsicon-fractionsicon-stats',
                    name: translateText('player1', 'Управление доступом')
                }}
                itemId={selectMember.uuid}
                executeName="updateMemberRankAccess"
                isSelector={true}
                mainScroll="h-430"
                clsScroll=""
                isSkip={true} />
        {:else}
            <div class="fractions__main_scroll w-399 h-480" />
        {/if}
    </div>
    <div class="fractions__main_box w-477 h-484">
        <div class="fractions__main_head box-between">
            <div class="box-flex">
                <span class="fractionsicon-stats"></span>
                <div class="fractions__main_title">{translateText('player1', 'Лог действий')}</div>
            </div>
            <div class="fractions__input small">
                <div class="fractionsicon-loop"></div>
                <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={searchText} type="text" placeholder={translateText('player1', 'Поиск..')}>
            </div>
        </div>
        <div class="box-flex">
            <div class="fractions__selector" class:active={logType === 0} on:click={() => onGetLogs (0)}>
                {translateText('player1', 'Общий')}
            </div>
            <div class="fractions__selector" class:active={logType === 1}  on:click={() => onGetLogs (1)}>
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
</div>