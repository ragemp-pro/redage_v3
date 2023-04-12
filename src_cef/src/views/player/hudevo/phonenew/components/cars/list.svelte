<script>
    import { translateText } from 'lang'
    import { TimeFormat } from 'api/moment'
    import {currentPage} from '../../stores'

    import { executeClient, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import {hasJsonStructure} from "api/functions";
    export let OnUpdatePage;
    let carsList = [{
        number: "222133218 2132",
        model: "adder",
        header: "Домовладельца"
        },
        {
        number: "229",
        model: "adder",
        },
    ];
    let inputText = "";

    executeClientAsyncToGroup("cars.getCarsList").then((result) => {
        if (hasJsonStructure(result))
            carsList = JSON.parse(result);
    });


    function isFilter(value, text){
        if(text === null){
            return true;
        }
        text = text.toLowerCase();
        return (value.number.toLowerCase().includes(text) || value.model.toLowerCase().includes(text) || value.header.toLowerCase().includes(text));
    }

    import { fade } from 'svelte/transition'










    import Filter from './filter.svelte'

    let isTypeFilter = [];
    const updateFilter = (filter) => {
        isTypeFilter = filter;

        closeFilter ();
    }

    let isFilterOpen;

    const openFilter = () => isFilterOpen = true;
    const closeFilter = () => isFilterOpen = false;
    const clearFilter = (event) => {
        event.stopPropagation();
        isTypeFilter = [];
    }

    const setPointArenda = () => {
        executeClient ("gps.name", translateText('player2', 'Ближайшая аренда авто'));
        executeClientToGroup ("close");
    }

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
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>
{#if isFilterOpen}
    <Filter {isTypeFilter} {updateFilter} {closeFilter} />
{:else}
    {#if carsList && typeof carsList === "object" && carsList.length > 0}
        <div class="box-between w-100 mb-10 newphone__project_padding20">
            <input type="text" class="newphone__ads_input small170 mb-0 mr-6" placeholder="Поиск..." bind:value={inputText} on:focus={onInputFocus} on:blur={onInputBlur}>
            {#if !isTypeFilter.length}
                <div class="newphone__button_filter box-center" on:click={openFilter}>
                    <span class="phoneicons-filter"></span>
                    {translateText('player2', 'Фильтры')}
                </div>
            {:else}
                <div class="newphone__button_filter box-center orange__background" on:click={openFilter}>
                    {isTypeFilter.length} {getFilterWord(isTypeFilter.length)}
                    <span class="phoneicons-close" on:click={clearFilter}></span>
                </div>
            {/if}
        </div>
        <div class="newphone__rent_list small" in:fade>
            {#each carsList.filter((el) => isFilter(el, inputText)) as item}
                {#if !isTypeFilter.length || isTypeFilter.includes(item.header)}
                <div class="newphone__rent_none hover vehicle" on:click={() => OnUpdatePage("Car", item)}>
                    <div class="box-column">
                        <div class="box-flex">
                            <div class="orange">{item.number}</div>
                            <div class="newphone__rent_status">{item.header}</div>
                        </div>
                        {#if item.isRent && !item.isJob}
                            <div class="gray">{translateText('player2', 'Осталось')}:</div>
                            <div class="date">
                                {TimeFormat (item.date, "H:mm DD.MM.YYYY")}
                            </div>
                        {:else}
                        <div class="gray">{translateText('player2', 'Модель')}:</div>
                        <div class="date">
                            {item.model}
                        </div>
                        {/if}
                    </div>
                    <div class="newphone__rent_noneimage pos-center rent" style="background-image: url('{document.cloud}inventoryItems/vehicle/{item.model.toLowerCase()}.png')"></div>
                </div>
                {/if}
            {/each}
        </div>
    {:else}
        <div class="newphone__rent_none" in:fade>
            <div class="box-column">
                <div class="orange">{translateText('player2', 'Транспорта нет')}</div>
                <div class="gray">{translateText('player2', 'Но вы можете оформить аренду транспортного средства')}</div>
            </div>
            <div class="newphone__rent_noneimage"></div>
        </div>
        <div class="newphone__project_button rent" on:click={setPointArenda}>{translateText('player2', 'Найти аренду')}</div>
        <div class="newphone__project_button rent" on:click={()=> currentPage.set("taxi")}>{translateText('player2', 'Вызвать такси')}</div>
    {/if}
{/if}