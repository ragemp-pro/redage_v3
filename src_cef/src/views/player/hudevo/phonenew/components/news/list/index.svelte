<script>
    import { translateText } from 'lang'
    import { TimeFormat } from 'api/moment'
    import { executeClientAsyncToGroup } from "api/rage";
    import { hasJsonStructure } from "api/functions";
    import { onCall, onMessage } from "@/views/player/hudevo/phonenew/data";

    let newsList = [];
    executeClientAsyncToGroup("getNews").then((result) => {//Запомнить на клиенте ласт чела которого открыл
        if (hasJsonStructure(result))
            newsList = JSON.parse(result);
    });

    export let onSelectedView;

    let isFilter = false;
    import Filter from './filter.svelte'

    let isTypeFilter = [];
    const updateFilter = (filter) => {
        isTypeFilter = filter;

        closeFilter ();
    }

    const openFilter = () => isFilter = true;
    const closeFilter = () => isFilter = false;
    const clearFilter = (event) => {
        event.stopPropagation();
        isTypeFilter = [];
    }

    //

    function getFilterWord(length){
        switch (length){
            case 1:
                return translateText('player2', 'фильтр')
            case 2:
                return translateText('player2', 'фильтра')
            case 3:
                return translateText('player2', 'фильтра')
            case 4:
                return translateText('player2', 'фильтра')
            case 5:
                return translateText('player2', 'фильтров')
            case 6:
                return translateText('player2', 'фильтров')
            case 7:
                return translateText('player2', 'фильтров')
            case 8:
                return translateText('player2', 'фильтров')
            case 9:
                return translateText('player2', 'фильтров')
            default:
                return translateText('player2', 'фильтров')
        }
    }

    import { fade } from 'svelte/transition'
</script>

{#if isFilter}
    <Filter {isTypeFilter} {updateFilter} {closeFilter} />
{:else}
    <div in:fade class="box-100">
        <div class="newphone__project_button" on:click={()=> onSelectedView ("Add")}>{translateText('player2', 'Подать объявление')}</div>
        {#if newsList && typeof newsList === "object" && newsList.length > 0}
            <div class="box-center box-between newphone__project_padding20">
                <div class="newphone__maps_header m-0">{translateText('player2', 'Объявления')}:</div>
                {#if !isTypeFilter.length}
                    <div class="newphone__button_filter box-center" on:click={openFilter}>
                        <span class="phoneicons-filter"></span>
                        {translateText('player2', 'Фильтры')}
                    </div>
                {:else}
                    <div class="newphone__button_filter box-center red__background" on:click={openFilter}>
                        {isTypeFilter.length} {getFilterWord(isTypeFilter.length)}
                        <span class="phoneicons-close" on:click={clearFilter}></span>
                    </div>
                {/if}
            </div>
            <div class="newphone__ads_list">
                {#each newsList as item}
                    {#if !isTypeFilter.length || isTypeFilter.includes(item.type)}
                        <div class="newphone__ads_element">
                            {#if item.isPremium}
                                <div class="newphone__ads_vip">
                                    <div>{translateText('player2', 'VIP-объявление')}</div>
                                    <div>{translateText('player2', 'VIP-объявление')}</div>
                                    <div>{translateText('player2', 'VIP-объявление')}</div>
                                    <div>{translateText('player2', 'VIP-объявление')}</div>
                                    <div>{translateText('player2', 'VIP-объявление')}</div>
                                </div>
                            {/if}
                            <div class="box-between">
                                <div class="newphone__ads_gray">{TimeFormat (item.time, "LT")}</div>
                                <div class="newphone__ads_gray">{TimeFormat (item.time, "L")}</div>
                            </div>
                            <div class="newphone__ads_text">{item.text}</div>
                            {#if item.link && /(?:jpg|jpeg|png)/g.test(item.link)}
                                <div class="newphone__ads_image" style="background-image: url({item.link})"></div>
                            {/if}
                            <div class="box-between">
                                <div class="red">{item.name}</div>
                                <div class="box-flex">
                                    <div class="newphone__ads_circle">
                                        <span class="phoneicons-call" on:click={() => onCall (item.number)}></span>
                                    </div>
                                    <div class="newphone__ads_circle">
                                        <span class="phoneicons-chat" on:click={() => onMessage (item.number)}></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    {/if}
                {/each}
            </div>
        {/if}
    </div>
{/if}