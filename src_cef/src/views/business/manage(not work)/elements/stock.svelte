<script>
    import ProdInfo from './prodInfo.svelte';

    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'

    let stocks = []
    executeClientAsyncToGroup("getStocks").then((result) => {
        if (result && typeof result === "string")
            stocks = JSON.parse(result);
    });

    let selectProd = {};
    const onSelectProd = (item) => {
        selectProd = item;
    }
</script>
{#if selectProd === null}
    <div class="bizmanage__grid">
        {#each stocks as item}
            <div class="bizmanage__grid_element stock" on:click={() => onSelectProd (item)}>
                <div class="bizmanage__grid_image"></div>
                <div class="bizmanage__grid_title">{item.Name}</div>
                <div class="bizmanage__grid_subtitle">На складе {item.Count}/{item.MaxCount}</div>
                <div class="green">{item.Perc} наценка</div>
            </div>
        {/each}
    </div>
    <div class="bizmanage__button_green box-center">Заполнить склад за 5000$</div>
{:else}
    <ProdInfo {selectProd} />
{/if}