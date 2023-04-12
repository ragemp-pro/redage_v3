<script>
    import { houseType } from 'json/realEstate.js'
    import { format } from 'api/formatter'
    export let houseData;
    export let onSelectData;

    let sortType = [
        0
    ]

    const addSort = (index) => {        
        const i = sortType.findIndex(a => a == index);

        if (i != -1)        
            sortType.splice(i, 1); 
        else 
            sortType.push (index);
        
        sortType = sortType;
    }

    const filterCheck = (type, data) => {
        return data.includes (type);
    }

    let sortData = [];
    $: if (sortType || houseData)
        sortData = houseData.filter(el => filterCheck(el[1], sortType))
</script>



<div class="rielt__checkbox_menu">
    {#each houseType as name, index}
        {#if name !== "Парковка"}
        <div class="box-flex" on:click={() => addSort(index)}>
            <input class="rielt__checkbox" type="checkbox" id="scales_{index}" name="scales_{index}" checked={sortType.includes (index)}>
            <label for="scales_{index}">{name}</label>
        </div>
        {/if}
    {/each}
</div>

{#if sortData.length}
    <div class="rielt__rielt_grid">
        {#each sortData as house, Index}
            <div class="houseicon-check rielt__rielt_element" on:click={() => onSelectData (house, "house")}>
                <div class="absolute">
                    <div class="rielt__rielt_number">#{house[0]}</div>
                    <div class="houseicon-house rielt__rielt_icon"></div>
                    <div class="rielt__gray">{houseType [house[1]]}</div>
                    <div class="rielt__rielt_price">${format("money", house[2])}</div>
                </div>
            </div>
        {/each}
    </div>
{:else}
    <div class="houseicon-time rielt__rielt_none">
        <div class="absolute">
            <div class="rielt__rielt_title font-36">Домов выбранных типов в продаже: 0</div>
            <div class="rielt__rielt_subtitle">Ожидайте</div>
        </div>
    </div>
{/if}