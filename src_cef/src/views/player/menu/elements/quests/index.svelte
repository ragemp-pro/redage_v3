<script>
    import { translateText } from 'lang'
    import './main.sass';
    import './fonts/style.css';
    import KeyAnimation from '@/components/keyAnimation/index.svelte';
    import { getQuest } from 'json/quests/quests.js';
    import { format } from 'api/formatter'
    import { storeQuests, selectQuest } from 'store/quest'
    import { executeClient } from 'api/rage'
    export let visible;

    let QuestsList = [];
    let OldQuest = [];

    let quest = false;
    const onSelectQuest = (actorName) => {

        const listIndex = QuestsList.findIndex(q => q.ActorName == actorName);
        if (QuestsList [listIndex]) {
            window.questStore.selectQuest (actorName);
            executeClient ("client.quest.selectQuest", actorName);
            return;
        }
        quest = false;
    }


    storeQuests.subscribe((value) => {        
        if (value && value && value.length && OldQuest != value) {
            executeClient ("client.quest.selectQuest.Clear");

            OldQuest = value;
            QuestsList = [];

            value.forEach(questData => {
                const questInfo = getQuest(questData.ActorName, questData.Line);
                
                if (questInfo && questInfo.Title && questInfo.Desc && questInfo.Tasks && questInfo.Tasks [questData.Stage]) {

                    QuestsList.push ({
                        ActorName: questData.ActorName,
                        Title: questInfo.Title,
                        Desc: questInfo.Desc,
                        Tasks: questInfo.Tasks,
                        Reward: questInfo.Reward,
                        Stage: questData.Stage
                    });
                }
            });
            QuestsList = QuestsList;
            if (!quest && typeof $selectQuest === "string") {
                const listIndex = QuestsList.findIndex(q => q.ActorName == $selectQuest);
                if (QuestsList [listIndex]) {
                    quest = QuestsList [listIndex];
                    return;
                }
            }

        }
    });

    const handleArrowKeys = (events) => {
        if (!quest || !quest.ActorName)
            return;
        else if (!visible)
            return;
        const { keyCode } = events;
        if (keyCode === 13) {
            onRoute ();
        }
        if (keyCode === 90) {
            onSelect ();
        }
    }

    const onRoute = () => {
        if (!quest)
            return;

        executeClient ("client.quest.router", quest.ActorName);
    }

    const onSelect = (actorName) => {
        if (!quest)
            return;

        onSelectQuest (quest.ActorName);
        window.notificationAdd(2, 9, translateText('player1', 'Вы сменили закрепленный квест.'), 3000);
    }
</script>

<svelte:window on:keyup={handleArrowKeys} />

{#if QuestsList && QuestsList.length}
<div id="questsnewlist">
    <!--<div class="questsnewlist-top">
        <div class="questsnewdialogicon-loop2 questlist-loop"></div>
        <div class="box-column">
            <div class="questsnewdlist-top-title">Поиск дополнительных заданий</div>
            <div class="questsnewdlist-top-subtitle">В штате <span class="white">14</span> заданий</div>
        </div>
    </div>-->
    <div class="questsnewlist-main-center">
        <div class="questsnewlist-bottom__left">
            <div class="questsnewlist-maintext">{translateText('player1', 'Список заданий')}</div>
            <div class="questsnewlist-list">
                <!--<div class="questsnewlist-element active">
                    <div class="box-flex">
                        <div class="questsnewlist-count">1</div>
                        <div class="questsnewlist-list-title">Как убить Билла?</div>
                    </div>
                    <div class="questsnewlist-condit">
                        <span class="questsnewdialogicon-star"/> Необходим 3 уровень
                    </div>
                </div>-->
                {#each QuestsList as questData, index}
                    <div class="questsnewlist-element" class:active={questData == quest} on:click={() => quest = questData}>
                        <div class="box-flex">
                            <div class="questsnewlist-count">{index + 1}</div>
                            <div class="questsnewlist-list-title">{questData.Title}</div>
                        </div>
                    </div>
                {/each}
            </div>
        </div>
        <div class="questsnewlist-center">
            <div class="box-flex">
                <span class="questsnewdialogicon-question questsquestion-icon"></span>
                {#if quest.Title}
                <div class="questsnewlist-center-maintext">{quest.Title}</div>
                {/if}
            </div>
            <div class="questsnewlist-center-text">{quest.Desc}</div>
            {#if quest.Tasks}
            <div class="questsnewlist-center-tasks">
                <div class="tasks-title">{translateText('player1', 'Задачи')}</div>
                {#each quest.Tasks as task, index}
                <div class="task-element">
                    <span class:questsnewdialogicon-done={!(index >= quest.Stage)} class:questsnewdialogicon-undone={index >= quest.Stage}></span>
                    {task}
                </div>
                {/each}
            </div>
            {/if}
        </div>
        <div class="questnewlist-right">
            {#if quest.Reward}
            <div class="questsnewdialogicon-background-star right-backicon"></div>
            <div class="questsnewlist-center-maintext">{translateText('player1', 'Награда')}</div>
            <div class="prise-box">
                
                <div class="box-money">
                    {#if quest.Reward && quest.Reward.Money}
                    <span class="questsnewdialogicon-money"/> {format("money", quest.Reward.Money)}
                    {:else}
                    <span class="questsnewdialogicon-money"/> 0
                    {/if}
                </div>
                <div class="box-exp">
                    {#if quest.Reward && quest.Reward.Exp}
                        <span class="questsnewdialogicon-exp"/> {quest.Reward.Exp}
                    {:else}
                    <span class="questsnewdialogicon-exp"/> 0
                    {/if}
                </div>
                {#if quest.Reward.Items}
                    <div class="questsnewlist-smalltext">{translateText('player1', 'Предметы')}</div>
                    <div class="questsnewlist-grid">
                    {#each new Array(4) as _, index}
                        {#if quest.Reward.Items[index] && window.getItem (quest.Reward.Items[index])}
                            <div class="questsnewlist-grid__item">
                                <span class="{window.getItem (quest.Reward.Items[index]).Icon} questsnewlist-grid__item_icon"/>
                            </div>
                        {:else}
                            <div class="questsnewlist-grid__item" />
                        {/if}
                    {/each}
                    </div>
                {/if}
            </div>
            {/if}
        </div>
    </div>
    <div class="questsnewlist-bottom">
        <div class="box-KeyAnimation questsnewlist-margin-right" on:click={onRoute}>
            <div>{translateText('player1', 'Построить маршрут')}</div>
            <KeyAnimation keyCode={13}>ENTER</KeyAnimation>
        </div>
        <div class="box-KeyAnimation" on:click={onSelect}>
            <KeyAnimation keyCode={90}>Z</KeyAnimation>
            <div>{translateText('player1', 'Взять квест')}</div>
        </div>
    </div>
</div>
{:else}
<div id="questsnewlist" class="box-center">
    <div class="questsnewlist-top box-center" style="align-items: center">
        <div class="questsnewdialogicon-warning questlist-loop" style="color: #E71D36"></div>
        <div class="questsnewdlist-top-title">{translateText('player1', 'На данный момент нет активных квестов')}!</div>
    </div>    
</div>
{/if}