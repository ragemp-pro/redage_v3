<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    import { getQuest } from 'json/quests/quests.js';
    import { storeQuests, selectQuest } from 'store/quest'
    import keys from 'store/keys'
    import keysName from 'json/keys.js'
    import { executeClient } from 'api/rage'
    import { isHelp } from 'store/hud'

    let QuestsList = [];
    let OldQuest = [];

    let quest = false;
    const onSelectQuest = (actorName) => {

        const listIndex = QuestsList.findIndex(q => q.ActorName == actorName);
        if (QuestsList [listIndex]) {
            quest = QuestsList [listIndex];
            return;
        }
        quest = false;
    }

    storeQuests.subscribe((value) => {
        if (value && value.length && OldQuest != value) {
            executeClient ("client.quest.selectQuest.Clear");

            OldQuest = value;
            QuestsList = [];

            value.forEach(questData => {
                const questInfo = getQuest(questData.ActorName, questData.Line);

                if (questInfo && questInfo.Title && questInfo.Tasks && questInfo.Tasks [questData.Stage]) {
                    executeClient ("client.quest.selectQuest.Add", questData.ActorName, (questInfo.Tasks.length - 1) === questData.Stage);

                    QuestsList.push ({
                        ActorName: questData.ActorName,
                        Title: questInfo.Title,
                        Text: questInfo.Tasks [questData.Stage]
                    });
                }
            });
            QuestsList = QuestsList;

            if (!quest && QuestsList.length && typeof QuestsList [0] === "object" && typeof $selectQuest !== "string") {
                quest = QuestsList [0];
                selectQuest.set (quest.ActorName);
                return;
            }

            onSelectQuest ($selectQuest);
        }
    });

    selectQuest.subscribe((value) => {
        onSelectQuest (value);
    });
</script>
{#if quest && quest.Title && $isHelp}
    <div class="hudevo__quests" in:fly={{ y: 50, duration: 500 }} out:fly={{ y: 50, duration: 250 }}>
        <div class="hudevo__quest">
            <div class="box-flex mb-6">
                <div class="hudevo__quest_circle"><div class="questcircle"></div></div>
                <div class="hudevo__quest_title">{@html quest.Title}</div>
            </div>
            <div class="box-flex ml-13 mb-6">
                <div class="hudevo__quest_square"></div>
                <div class="hudevo__quest_subtitle">{@html quest.Text}</div>
            </div>
            <div class="box-flex ml-13">
                <div class="hudevo__quest_button">{keysName[$keys[12]]}</div>
                <div class="hudevo__quest_info">Информация</div>
            </div>
        </div>
    <!--     <div class="hudevo__quest">
            <div class="box-flex mb-6">
                <div class="hudevo__quest_circle"><div class="questcircle purple"></div></div>
                <div class="hudevo__quest_title">Боевой пропуск</div>
            </div>
            <div class="box-flex ml-13 mb-6">
                <div class="hudevo__quest_square"></div>
                <div class="hudevo__quest_subtitle">Доберись до Виталия Дебича</div>
            </div>
            <div class="box-flex ml-13">
                <div class="hudevo__quest_button purple">F2</div>
                <div class="hudevo__quest_info">Информация</div>
            </div>
        </div> -->
    </div>
{/if}