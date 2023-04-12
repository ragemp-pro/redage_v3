<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { executeClientAsyncToGroup } from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPng } from './../data'

    export let onSelectedOrderName;
    export let onSelectedViewOrder;
    export let onSelectedViewBusiness;

    let ordersList = [];
    const updateData = () => {
        executeClientAsyncToGroup("business.getOrders").then((result) => {
            if (hasJsonStructure(result))
                ordersList = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    const onSelectItem = (name) => {
        onSelectedViewOrder ("Item");
        onSelectedOrderName (name);
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__rent_list" in:fade>
    <div class="newphone__project_title w-100 fs-18">{translateText('player2', 'Заказы')}:</div>
    {#each ordersList as order}
        <div class="newphone__rent_none hover" on:click={() => onSelectItem(order.name)}>
            <div class="box-column">
                <div class="greenid">№{order.uid}</div>
                <div class="box-flex">
                    <div class="violet">{order.name}</div>
                </div>
                <div class="gray">{translateText('player2', 'Количество')}: <span class="violet">{order.count}</span></div>
                <div class="gray mt-5">{translateText('player2', 'Потрачено')}: <span class="green">${format("money", order.price)}</span></div>
            </div>
            <div class="newphone__rent_noneimage person" style="background-image: url({getPng(order.productType, order.name, order.itemId)})"></div>
        </div>
    {/each}
    <div class="violet box-center m-top10" on:click={() => onSelectedViewBusiness ("Menu")}>{translateText('player2', 'Назад')}</div>
</div>