<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPng } from './../data'

    export let selectedProductName;
    export let onSelectedViewProduct;
    export let onSelectedProductName;

    let product = {};

    const updateData = () => {
        executeClientAsyncToGroup("business.getProduct", selectedProductName).then((result) => {
            if (hasJsonStructure(result))
                product = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    const getExtraCharge = (bType, price, defaultPrice) => {
        if (bType == 7 || bType == 9 || bType == 10 || bType == 11 || bType == 12){
            return `${price}%`;
        }
        return `${Math.round(price * 100 / defaultPrice)}% ($${format("money", price)})`;
    }

    const onChangePrice = () => {
        onSelectedViewProduct ("ChangePrice");
    }

    const onOrderProduct = () => {
        onSelectedViewProduct ("OrderProduct");
    }

    const onSelectBack = () => {
        onSelectedViewProduct ("List");
        onSelectedProductName ("");
    }

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    const cancelOrder = () => {
        if (!window.loaderData.delay ("business.cancelOrder", 1.5))
            return;

        executeClientToGroup("business.cancelOrder", product.uidOrder);
    }
    import { fade } from 'svelte/transition'
</script>

<div class="newphone__rent_list" in:fade>
    <div class="newphone__rent_none hover">
        <div class="box-column">
            <div class="box-flex">
                <div class="violet">{product.name}</div>
            </div>
            <div class="gray">{translateText('player2', 'На складе')}: <span class="violet">{product.count}/{product.maxCount}</span></div>
            <div class="gray mt-5">{translateText('player2', 'Текущая наценка')}: <span class="green">{getExtraCharge (bizType, product.price, product.defaultPrice)}</span></div>
        </div>
        <div class="newphone__rent_noneimage person" style="background-image: url({getPng(product.productType, product.name, product.itemId)})"></div>
    </div>

    {#if !["Лотерейный билет", "Расходники"].includes(product.name)}
        <div class="newphone__project_button auction mb-0" on:click={onChangePrice}>{translateText('player2', 'Изменить наценку')}</div>
    {/if}

    {#if !product.isOrder && product.maxCount > product.count}
        <div class="newphone__project_button property" on:click={onOrderProduct}>{translateText('player2', 'Заказать')}</div>
    {/if}

    {#if product.isOrder && product.uidOrder}
        <div class="newphone__project_button property" on:click={cancelOrder}>{translateText('player2', 'Отменить заказ')}</div>
    {/if}

    <div class="violet box-center m-top10" on:click={onSelectBack}>{translateText('player2', 'Назад')}</div>
</div>