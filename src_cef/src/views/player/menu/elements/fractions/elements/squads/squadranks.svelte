<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let settings;
    export let onSettings;

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

    //
    import { setPopup } from "../../data";

    import SquadInfo from './squadinfo.svelte'

    let selectRank = null

    const onSelectRank = (item = null) => {
        selectRank = item
    }

    $: if (settings) {
        if (selectRank) {
            const index = settings.ranks.findIndex(a => a.id === selectRank.id)

            if (settings.ranks [index])
                onSelectRank(settings.ranks [index])
        }
    }
</script>

{#if selectRank}
    <SquadInfo departmentId={settings.id} {selectRank} {onSelectRank} />
{:else}
    <div class="fractions__stats_subtitle gray hover mt-20 large" on:click={() => onSettings()}>&#x2190; {translateText('player1', 'Вернуться к меню управления отрядом')}</div>

    <div class="fractions__content backgr extrabig">
        <div class="fractions__main_head box-between mb-0">
            <div class="box-flex">
                <span class="fractionsicon-squads"></span>
                <div class="fractions__main_title">{translateText('player1', 'Список рангов')}</div>
            </div>
            <div class="fractions__input large">
                <div class="fractionsicon-loop"></div>
                <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder={translateText('player1', 'Поиск..')} bind:value={searchText}>
            </div>
            <!--<div class="fractions__main_button extra">
                <span class="fractionsicon-plus mr-5"></span>
                {translateText('player1', 'Создать ранг')}
            </div>-->
        </div>
        <div class="box-between pr-22 pl-8 pr-276">
            <div class="fractions_stats_title fs-14 name mr-16">{translateText('player1', 'Ранг')}:</div>
            <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Название')}:</div>
            <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Участники')}:</div>
        </div>
        <div class="fractions__main_scroll big extrabig">
            {#each settings.ranks.filter(el => filterCheck(el.name, searchText)) as item, index}
                <div class="fractions__scroll_element hover p-20">
                    <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.id}</div>
                    <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.name}</div>
                    <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.playerCount} чел.</div>
                    <div class="box-flex hidden">
                        <div class="fractions__main_button black" on:click={() => onSelectRank (item)}>{translateText('player1', 'Редактировать')}</div>
                    </div>
                </div>
            {/each}
        </div>
    </div>
{/if}