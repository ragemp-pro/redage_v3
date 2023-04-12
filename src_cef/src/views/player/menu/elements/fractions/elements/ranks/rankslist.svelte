<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let ranks;

    import { format } from 'api/formatter'

    const onCreateCallback = (rankName, score) => {
        let check = format("rank", rankName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        score = Math.round(score.toString().replace(/\D+/g, ""));
        if (score < 0)
            score = 0;
        else if (score > 9999999)
            score = 9999999;

        executeClientToGroup('createRank', rankName, score)
    }

    const onCreate = () => {
        setPopup ("Input", {
            headerTitle: "Создать ранг",
            headerIcon: "fractionsicon-rank",
            inputs: [
                {
                    title: "Название ранга",
                    placeholder: "Название ранга",
                    maxLength: 25,
                },
                {
                    title: "Количество очков для получения ранга",
                    placeholder: "Количество очков для получения ранга",
                    maxLength: 23,
                    type: "number"
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onCreateCallback
        })
    }

    //

    export let onSettings;

    import { spring } from 'svelte/motion';

    let coords = spring({ x: 0, y: 0 }, {
        stiffness: 1.0,
        damping: 1.0
    });

    let dragonDropItem = false;
    let offsetInElementX = 0;
    let offsetInElementY = 0;
    let mainElements = {}
    let copyArray = []

    const onStartDragonDrop = (event, index, item) => {
        copyArray = ranks;

        const target = mainElements[index].getBoundingClientRect();

        offsetInElementX = (target.width - (target.right - event.clientX));
        offsetInElementY = (target.height - (target.bottom - event.clientY));

        coords.set({ x: event.clientX, y: event.clientY });
        dragonDropItem = item;
    }

    const handleGlobalMouseMove = (event) => {
        if (dragonDropItem !== false) {
            coords.set({ x: event.clientX, y: event.clientY });
        }
    }

    const handleGlobalMouseUp = () => {
        if (dragonDropItem !== false) {
            dragonDropItem = false;
        }
    }

    const onUpdateIndex = (item) => {
        if (dragonDropItem !== false) {
            const index1 = ranks.findIndex(i => i.id === dragonDropItem.id);
            const item1 = Object.assign({}, ranks [index1]);
            const id1 = item1.id;
            const maxScore1 = item1.maxScore;
            if (typeof item1.oldId === "undefined")
                item1.oldId = item1.id;
            item1.id = item.id;
            item1.maxScore = item.maxScore;

            const index2 = ranks.findIndex(i => i.id === item.id);
            const item2 = Object.assign({}, ranks [index2]);
            if (typeof item2.oldId === "undefined")
                item2.oldId = item2.id;
            item2.id = id1;
            item2.maxScore = maxScore1;

            ranks [index1] = item2;
            dragonDropItem = item1;
            ranks [index2] = item1;
        }

    }

    import { onDestroy } from 'svelte';
    import { setPopup, isOrganization } from "../../data";

    let isSave = false;

    const saveRanks = () => {
        if (isSave)
            return;
        isSave = true;

        const updateRankId = {};

        ranks.forEach((item) => {
            if (typeof item.oldId !== "undefined" && item.oldId !== item.id) {
                updateRankId [item.oldId] = item.id;
            }
        });

        if (updateRankId && Object.keys(updateRankId).length)
            executeClientToGroup('updateRanksId', JSON.stringify(updateRankId))
    }

    onDestroy(saveRanks)


    //search

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    const onSelectItem = (item) => {
        saveRanks ();
        onSettings (item);
    }

    let deleteId = 0;
    const onRankDeleteCallback = () => {
        executeClientToGroup('removeRank', deleteId)
    }

    const onRankDelete = (item) => {
        deleteId = item.id;
        setPopup ("Confirm", {
            headerTitle: "Удаление ранга",
            headerIcon: "fractionsicon-members",
            text: `Вы действительно хотите удалить '${item.id}. ${item.name}'?`,
            button: 'Удалить',
            callback: onRankDeleteCallback
        })
    }

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings ();
</script>

<svelte:window on:mouseup={handleGlobalMouseUp} on:mousemove={handleGlobalMouseMove} />

<div class="fractions__content backgr extrabig">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-rank"></span>
            <div class="fractions__main_title">{translateText('player1', 'Список рангов')}</div>
        </div>
        <div class="fractions__input large">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" bind:value={searchText} placeholder={translateText('player1', 'Поиск..')}>
        </div>
        {#if settings.isLeader && isOrganization()}
            <div class="fractions__main_button extra" on:click={onCreate}>
                <span class="fractionsicon-plus mr-5"></span>
                {translateText('player1', 'Создать ранг')}
            </div>
        {/if}
    </div>
    <div class="box-between pr-22 pl-8 pr-226">
        <div class="fractions_stats_title fs-14 name mr-16">{translateText('player1', 'Ранг')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Название ранга')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Требуемый опыт')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Численность')}:</div>
    </div>

    {#if dragonDropItem}
        <div class="fractions__dragondrop_element" style={`top:${$coords.y - offsetInElementY}px;left:${$coords.x - offsetInElementX}px;`}>
            <div class="fractions__scroll_element activeselected p-20">
                <div class="fractions_stats_title fs-14 name box-flex m-0">
                    <div class="fractionsicon-dots"></div>
                    {dragonDropItem.id}
                </div>
                <div class="fractions_stats_title fs-14 name m-0">{dragonDropItem.name}</div>
                <div class="fractions_stats_title fs-14 name m-0">{dragonDropItem.maxScore} XP</div>
                <div class="fractions_stats_title fs-14 name m-0">{dragonDropItem.playerCount} чел.</div>
                <div class="box-flex hidden">
                    <div class="fractions__main_button black">{translateText('player1', 'Редактировать')}</div>
                    <div class="fractionsicon-fired"></div>
                </div>
            </div>
        </div>
    {/if}

    <div class="fractions__main_scroll big extrabig">
        {#each ranks.filter(el => filterCheck(el.name, searchText)) as item, index}
            <div class="fractions__scroll_element hover p-20"
                 bind:this={mainElements[index]} on:mouseenter={() => onUpdateIndex (item)}>
                <div class="fractions_stats_title fs-14 name whitecolor box-flex m-0">
                    {#if isOrganization()}
                    <div class="fractionsicon-dots" on:mousedown={(e) => onStartDragonDrop (e, index, item)}></div>
                    {/if}
                    {item.id}
                </div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.name}</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.maxScore} XP</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.playerCount} чел.</div>
                <div class="box-flex hidden">
                    <div class="fractions__main_button black" on:click={() => onSelectItem (item)}>{translateText('player1', 'Редактировать')}</div>

                    {#if isOrganization()}
                        <div class="fractionsicon-fired" on:click={() => onRankDelete (item)}></div>
                    {/if}
                </div>
            </div>
        {/each}
    </div>    
</div>