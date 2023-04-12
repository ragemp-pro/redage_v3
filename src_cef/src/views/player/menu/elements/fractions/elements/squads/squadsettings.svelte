<script>
    import { translateText } from 'lang'
    import List from './list.svelte'
    import moment from 'moment';


    export let onSelectDepartment;
    export let onDepartmentDelete;
    export let onSettings;

    export let department;
    export let members;
    export let onlineData;


    import { executeClientToGroup } from 'api/rage'


    import { setPopup } from "../../data";
    import { format } from 'api/formatter'

    const onUpdateCallback = (name, tag) => {
        let check = format("rank", name);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        check = format("tag", tag);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('updateDepartment', department.id, name, tag)
    }

    const onUpdate = () => {
        setPopup ("Input", {
            headerTitle: "Изменить данные отряда",
            headerIcon: "fractionsicon-squads",
            inputs: [
                {
                    title: "Название отряда",
                    placeholder: "Введите новое название",
                    maxLength: 36,
                    value: department.name
                },
                {
                    title: "Тег отряда",
                    placeholder: "Введите тег отряда",
                    maxLength: 5,
                    value: department.tag
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onUpdateCallback
        })
    }

    const onSquadRanksCallback = (json) => {
        executeClientToGroup('setLeadersDepartment', department.id, json)
    }

    const onSquadRanks = () => {
        setPopup ("SquadRanks", {
            id: department.id,
            ranks: department.ranks,
            myRank: department.myRank,
            callback: onSquadRanksCallback
        })
    }

    import { executeClientAsyncToGroup } from "api/rage";

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
</script>
<div class="box-between">
    <div class="fractions__stats_subtitle gray hover mt-20 large" on:click={() => onSelectDepartment()}>&#x2190; {translateText('player1', 'Вернуться к общей информации')}</div>
    <div class="box-flex">
        {#if settings.isLeader}
        <div class="fractions__main_button extra mr-10" on:click={() => onSettings (department)}>{translateText('player1', 'Настройки')}</div>
        {/if}
        {#if settings.deleteDepartment}
        <div class="fractions__main_button extra" on:click={() => onDepartmentDelete(department)}>{translateText('player1', 'Удалить отряд')}</div>
        {/if}
    </div>
</div>
<div class="box-between mt-20">
    <div class="fractions__main_box w-600">
        <div class="fractions__main_head">
            <span class="fractionsicon-squads"></span>
            <div class="fractions__main_title">{department.tag} - {translateText('player1', 'Управление отрядом')}</div>
        </div>
        <div class="fractions__main_grid">
            <div class="fractions__main_element">
                <div class="fractions_stats_title">{translateText('player1', 'Начальник отряда')}:</div>
                <div class="fractions__stats_subtitle mw-161">{department.chief}</div>
            </div>
            <div class="fractions__main_element">
                <div class="fractions_stats_title">{translateText('player1', 'Участников')}:</div>
                <div class="fractions__stats_subtitle mw-161">{department.playerCount} чел.</div>
            </div>
            <div class="fractions__main_element">
                <div class="fractions_stats_title">{translateText('player1', 'Заместитель')} №1:</div>
                <div class="fractions__stats_subtitle mw-161">{department.zam1}</div>
            </div>
            <div class="fractions__main_element">
                <div class="fractions_stats_title">{translateText('player1', 'Тег отряда')}:</div>
                <div class="fractions__stats_subtitle mw-161">#{department.tag}</div>
            </div>
            <div class="fractions__main_element">
                <div class="fractions_stats_title">{translateText('player1', 'Дата создания')}:</div>
                <div class="fractions__stats_subtitle mw-161">{moment(department.date).format('DD.MM.YYYY')}</div>
            </div>
            <div class="fractions__main_element">
                <div class="fractions_stats_title">{translateText('player1', 'Заместитель')} №2:</div>
                <div class="fractions__stats_subtitle mw-161">{department.zam2}</div>
            </div>
        </div>
    </div>
    {#if department.isSettings || settings.isLeader || department.myRank === 3}
    <div class="fractions__main_box w-317">
        <div class="fractions__main_head">
            <span class="fractionsicon-squads"></span>
            <div class="fractions__main_title">{translateText('player1', 'Команды')}</div>
        </div>
        <div class="box-column">
            {#if department.isSettings}
                <div class="fractions__main_button h-40 mb-16" on:click={onUpdate}>{translateText('player1', 'Изменить данные отряда')}</div>
            {/if}
            {#if settings.isLeader || department.myRank === 3}
                <div class="fractions__main_button h-40" on:click={onSquadRanks}>{translateText('player1', 'Назначить руководство')}</div>
            {/if}
        </div>
    </div>
    {/if}
</div>
<List departmentId={department.id} {members} {onlineData} ranks={department.ranks} />