<script>
    import { translateText } from 'lang'
    import moment from 'moment'
    import { TimeFormat } from 'api/moment'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('membersLoad')

    let member = {};
    let members = [];
    let selectMember = false;

    let onlineData = {
        online: 0,
        offline: 0,
        all: 0
    };

    let ranks = []

    const getRanks = () => {
        executeClientAsyncToGroup("getRanks").then((result) => {
            if (result && typeof result === "string")
                ranks = JSON.parse(result);
        });
    }
    getRanks();

    const getMembers = (_member, _members, _onlineData) => {
        if (_member && typeof _member === "string")
            member = JSON.parse(_member);

        selectMember = member;

        if (_members && typeof _members === "string")
            members = JSON.parse(_members);

        onlineData = JSON.parse(_onlineData);
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.members", getMembers)

    const UpdateMember = (_member) => {
        _member = JSON.parse(_member);

        let index = members.findIndex((m) => m.uuid === _member.uuid);

        if (members [index]) {
            members [index] = _member;
            if (selectMember.uuid === _member.uuid)
                onSelectMember (_member)
        }
    }

    addListernEvent ("table.updateMember", UpdateMember)

    import Members from './list.svelte'
    import Member from './member.svelte'


    const onSelectMember = (item) => {
        selectMember = item;
    }


    import { setPopup } from "../../data";

    let deleteUuid;
    const onPlayerDeleteCallback = () => {
        executeClientToGroup('deletePlayer', deleteUuid);
    }

    const onPlayerDelete = (item) => {
        deleteUuid = item.uuid;
        setPopup ("Confirm", {
            headerTitle: "Увольнение",
            headerIcon: "fractionsicon-members",
            text: `Вы действительно хотите уволить '${item.name}'?`,
            button: 'Уволить',
            callback: onPlayerDeleteCallback
        })
    }

    const onPlayerAddScoreCallback = (value) => {
        if (selectMember !== member) {
            executeClientToGroup('addPlayerScore', selectMember.uuid, value);
        }
    }
    const onPlayerAddScore = () => {
        setPopup ("Input", {
            headerTitle: "Выдать очки",
            headerIcon: "fractionsicon-members",
            input:
            {
                title: "Количество очков:",
                placeholder: "Введите нужное кол-во",
                maxLength: 36,
                type: 'number'
            },
            button: 'Выдать',
            callback: onPlayerAddScoreCallback
        })
    }

    const getProgress = (value, max) => {
        let perc = Math.round(value / max * 100);

        if (!perc || perc < 0)
            perc = 0;
        else if (perc > 100)
            perc = 100;

        return perc;
    }

    const getProgressBackground = (value) => {
        if (value < 35)
            return '#FB4F69';
        else if (value < 70)
            return '#fc0';

        return '#05BB63';
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();

    import Avatar from '../avatar/index.svelte'

    const setAvatar = (png) => {
        member.avatar = png;

        if (selectMember.uuid === member.uuid)
            onSelectMember (member)
    }
    import Logo from '../other/logo.svelte'
</script>
{#if selectMember}
    <div class="fractions__header">
        <div class="box-flex">
            <Logo />
            <div class="box-column">
                <div class="box-flex">
                    <span class="fractionsicon-members"></span>
                    <div class="fractions__header_title">{translateText('player1', 'Состав')}</div>
                </div>
                <div class="fractions__header_subtitle">{translateText('player1', 'Меню просмотра и управления составом')}</div>
            </div>
        </div>
        {#if selectMember !== member}
            <div class="box-flex">
                <!--<div class="fractions__main_button extra mr-10" on:click={onPlayerAddScore}>{translateText('player1', 'Выдать очки')}</div>-->
                {#if settings.unInvite}
                <div class="fractions__main_button extra" on:click={() => onPlayerDelete (selectMember)}>{translateText('player1', 'Уволить')}</div>
                {/if}
            </div>
        {/if}
    </div>
    {#if selectMember !== member}
        <div class="fractions__stats_subtitle gray hover mt-20 large" on:click={() => onSelectMember (member)}>&#x2190; {translateText('player1', 'Вернуться к общей информации')}</div>
    {/if}
    <div class="fractions__content">
        <div class="box-between">
            <div class="fractions__main_box w-440">
                <div class="fractions__main_head">
                    <span class="fractionsicon-person"></span>
                    <div class="fractions__main_title">{selectMember !== member ? translateText('player1', 'Информация') : translateText('player1', 'Ваша информация')}</div>
                </div>
                <div class="box-flex">
                    <Avatar url={selectMember.avatar} uuid={selectMember.uuid} cls="big" {setAvatar} />
                    <div class="box-column">
                        <div class="fractions__main_grid mini">
                            <div class="fractions__main_element mb-24">
                                <div class="fractions_stats_title">{translateText('player1', 'Имя Фамилия')}:</div>
                                <div class="fractions__stats_subtitle">{selectMember.name}</div>
                            </div>
                            <div class="fractions__main_element mb-24">
                                <div class="fractions_stats_title">{translateText('player1', 'Ранг')}:</div>
                                <div class="fractions__stats_subtitle">{selectMember.rankName}</div>
                            </div>
                            <div class="fractions__main_element">
                                <div class="fractions_stats_title">{translateText('player1', 'Дата вступления')}:</div>
                                <div class="fractions__stats_subtitle">{TimeFormat (selectMember.date, "HH:mm DD.MM.YY")}</div>
                            </div>
                            <div class="fractions__main_element mb-24">
                                <div class="fractions_stats_title">{translateText('player1', 'Отряд')}:</div>
                                <div class="fractions__stats_subtitle">{selectMember.departmentName}</div>
                            </div>
                        </div>
                        <div class="box-column">
                            <div class="fractions_stats_title">
                                {translateText('player1', 'До следующего ранга')} <span class="white">{selectMember.score}</span>/ {selectMember.maxScore} XP
                            </div>
                            <div class="fractions__progressbar">
                                <div class="fractions__progressbar_current" style="width: {getProgress(selectMember.score, selectMember.maxScore)}; background: {getProgressBackground(getProgress(selectMember.score, selectMember.maxScore))}"></div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="fractions__main_box w-477">
                <div class="fractions__main_head">
                    <span class="fractionsicon-stats"></span>
                    <div class="fractions__main_title">{selectMember !== member ? translateText('player1', 'Статистика') : translateText('player1', 'Ваша статистика')}</div>
                </div>
                <div class="fractions__main_grid">
                    <div class="fractions__main_element">
                        <div class="fractions_stats_title">{translateText('player2', 'Онлайн сегодня')}:</div>
                        <div class="fractions__stats_subtitle">{moment.duration(selectMember.todayTime, "minutes").format("h[ч.] m[м.]")}</div>
                    </div>
                    <div class="fractions__main_element">
                        <div class="fractions_stats_title">{translateText('player2', 'Онлайн за неделю')}:</div>
                        <div class="fractions__stats_subtitle">{moment.duration(selectMember.weekTime, "minutes").format("h[ч.] m[м.]")}</div>
                    </div>
                    <div class="fractions__main_element">
                        <div class="fractions_stats_title">{translateText('player2', 'Онлайн за месяц')}:</div>
                        <div class="fractions__stats_subtitle">{moment.duration(selectMember.monthTime, "minutes").format("h[ч.] m[м.]")}</div>
                    </div>
                    <div class="fractions__main_element">
                        <div class="fractions_stats_title">{translateText('player2', 'Общий онлайн')}:</div>
                        <div class="fractions__stats_subtitle">{moment.duration(selectMember.totalTime, "minutes").format("h[ч.] m[м.]")}</div>
                    </div>
                    <div class="fractions__main_element">
                        <div class="fractions_stats_title">{translateText('player2', 'Паспорт')}:</div>
                        <div class="fractions__stats_subtitle">{selectMember.uuid}</div>
                    </div>
                </div>
            </div>
        </div>
        {#if selectMember === member}
            <Members playerUuid={member.uuid} {onlineData} {members} {onSelectMember} {onPlayerDelete} {settings} />
        {:else}
            <Member {selectMember} {member} />
        {/if}
    </div>
{/if}