<script>
    export let isTypeFilter;
    export let updateFilter;
    export let closeFilter;


    let filter = isTypeFilter;

    const onUpdateFilter = (id) => {
        const index = filter.findIndex(e => e === id);

        if (typeof filter [index] !== "undefined")
            filter.splice(index, 1);
        else
            filter.push(id);

        filter = filter;
    }
    import { fade } from 'svelte/transition'
    import { translateText } from 'lang'
    import { executeClientAsyncToGroup } from "api/rage";
    import { hasJsonStructure } from "api/functions";


    let categorieName = [];
    executeClientAsyncToGroup("cars.filterData").then((result) => {
        if (hasJsonStructure(result))
            categorieName = JSON.parse(result);
    });

</script>
<div in:fade class="box-100">
    <div class="newphone__maps_header m-top newphone__project_padding20 mb-10">{translateText('player2', 'Фильтры')}:</div>
    <div class="newphone__ads_list filter">
        {#each categorieName as item}
            <div class="box-between w-100" on:click={() => onUpdateFilter (item)}>
                <div>{item}</div>

                <div class="newphone__checkbox">
                    <input class="styled-checkbox" id="styled-checkbox-1" type="checkbox" disabled checked={filter.includes(item)}>
                    <label class="styled-checkbox1" for="styled-checkbox-1"></label>
                </div>
            </div>
        {/each}
    </div>
    <div class="newphone__project_button rentnm" on:click={() => updateFilter (filter)}>{translateText('player2', 'Применить фильтр')}</div>
    <div class="orange box-center" on:click={closeFilter}>{translateText('player2', 'Назад')}</div>
</div>