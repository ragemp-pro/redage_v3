<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import Access from '../access/index.svelte'

    let ranks = []
    executeClientAsyncToGroup("getRanks").then((result) => {
        if (result && typeof result === "string") {
            ranks = JSON.parse(result);
            onSelectId (ranks[0])
        }
    });


    let selectId = -1;
    const onSelectId = (item) => {
        executeClientToGroup('rankAccessLoad', item.id)
        selectId = item.id;
    }
</script>
<div class="box-column mr-20 align-startflex">
    <div class="fractions_stats_title mt-20">{translateText('player1', 'Ранг')}:</div>
    <div class="fractions__main_scroll w-482 h-480">
        {#each ranks as item, index}
            {#if (ranks.length - 1) !== index}
                <div class="fractions__scroll_element hover p-20 fw-bold" on:click={() => onSelectId(item)}>
                    <div class="box-flex">
                        <div class="mr-40">{item.id}</div>
                        <div>{item.name}</div>
                    </div>
                    {#if selectId === item.id}
                        <div class="fs-36">&#8250;</div>
                    {/if}
                </div>
            {/if}
        {/each}
    </div>
</div>

<div class="box-column">
    <div class="fractions_stats_title mt-20">{translateText('player1', 'Доступы')}:</div>

    {#if selectId >= 0}
        <Access
            itemId={selectId}
            executeName="updateRankAccess"
            isSelector={false}
            clsScroll="w-399 h-480" />
    {:else}
        <div class="fractions__main_scroll w-399 h-480" />
    {/if}

</div>
<div class="box-column">
</div>