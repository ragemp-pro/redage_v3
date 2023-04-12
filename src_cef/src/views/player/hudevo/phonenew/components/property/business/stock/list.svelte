<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPng } from './../data'

    export let onSelectedViewProduct;
    export let onSelectedProductName;
    export let onSelectedViewBusiness;


    let stats = {};
    let productsList = [];

    const updateData = () => {
        executeClientAsyncToGroup("business.getStats").then((result) => {
            if (hasJsonStructure(result))
                stats = JSON.parse(result);
        });

        executeClientAsyncToGroup("business.getProducts").then((result) => {
            if (hasJsonStructure(result))
                productsList = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);


    const onSelectItem = (name) => {
        onSelectedViewProduct ("Item");
        onSelectedProductName (name);
    }

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    const onMaxProducts = () => {
        if (!window.loaderData.delay ("business.maxProducts", 2.5))
            return;

        executeClientToGroup("business.maxProducts");
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__rent_list" in:fade>
    <div class="newphone__project_title w-100 fs-18">{translateText('player2', 'Склад')}:</div>
    {#if stats.whCount < stats.whMaxCount && stats.whPriceMaxCount > 0}
        <div class="newphone__project_button property" on:click={onMaxProducts}>{translateText('player2', 'Заказать все за')} ${format("money", stats.whPriceMaxCount)}</div>
    {/if}
    {#each productsList as product}
        <div class="newphone__rent_none hover" on:click={() => onSelectItem(product.name)}>
            <div class="box-column">
                <div class="box-flex">
                    <div class="violet">{product.name}</div>
                </div>
                <div class="gray">{translateText('player2', 'На складе')}: <span class="violet">{product.count}/{product.maxCount}</span></div>
                <div class="gray mt-5">{translateText('player2', 'Текущая цена')}: <span class="green">${format("money", product.price)}</span></div>
            </div>
            <div class="newphone__rent_noneimage person" style="background-image: url({getPng(product.productType, product.name, product.itemId)})"></div>
        </div>
    {/each}
    <div class="violet box-center mt-15" on:click={() => onSelectedViewBusiness ("Menu")}>{translateText('player2', 'Назад')}</div>
</div>