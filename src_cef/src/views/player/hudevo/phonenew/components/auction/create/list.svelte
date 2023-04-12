<script>
    import { translateText } from 'lang'
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {hasJsonStructure} from "api/functions";
    export let onSelectedView;

    let itemList = [];
    executeClientAsyncToGroup("auction.getItem").then((result) => {
        if (hasJsonStructure(result))
            itemList = JSON.parse(result);
    });

    const onSelect = (id) => {
        executeClientToGroup ("auction.setItemId", id);
        onSelectedView("CreateItem", true);
    }
</script>

<div class="newphone__maps_header m-top newphone__project_padding20">{translateText('player2', 'Список')}:</div>
<div class="newphone__ads_list">
    {#if itemList && itemList.length > 0}
        {#each itemList as item, index}
            <div class="newphone__project_categorie" on:click={()=> onSelect(item.id)}>
                <div>{item.name}</div>
                <div class="phoneicons-Button newphone__chevron-back green"></div>
            </div>
        {/each}
    {:else}
        <div class="newphone__forbes_anon newphone__project_padding20">
            <div class="phoneicons-clubs"></div>
            <div class="gray">{translateText('player2', 'У Вас нет имущества, которое можно было бы выставить в этом разделе')}.</div>
        </div>
    {/if}
</div>
<div class="green box-center mt-5" on:click={() => onSelectedView("Create")}>{translateText('player2', 'Назад')}</div>