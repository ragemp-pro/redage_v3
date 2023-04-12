<script>
    import { businessType } from 'json/realEstate.js'
    import { format } from 'api/formatter'
    export let businessData;
    export let onSelectData;
</script>

<div class="rielt__rielt_grid">
    {#each businessData as business, Index}
        <div class="houseicon-check rielt__rielt_element" on:click={() => onSelectData (business, "business")}>
            <div class="absolute">
                <div class="rielt__rielt_number">#{business[0]}</div>
                <div class="houseicon-garage rielt__rielt_icon"></div>
                <div class="rielt__gray">{businessType[business[1]]}</div>
                <div class="rielt__rielt_price">${format("money", business[2])}</div>
            </div>
        </div>
    {/each}
</div>