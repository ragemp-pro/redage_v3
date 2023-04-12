<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { executeClientAsyncToGroup, executeClientToGroup } from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPng } from './../data'

    export let selectedOrderName;
    export let onSelectedOrderName;
    export let onSelectedViewOrder;

    let order = {};

    const updateData = () => {
        executeClientAsyncToGroup("business.getOrder", selectedOrderName).then((result) => {
            if (hasJsonStructure(result))
                order = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    const onSelectBack = () => {
        onSelectedViewOrder ("List");
        onSelectedOrderName ("");
    }

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    const cancelOrder = () => {
        if (!window.loaderData.delay ("business.cancelOrder", 1.5))
            return;

        executeClientToGroup("business.cancelOrder", order.uid);
        onSelectBack ();
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__rent_list" in:fade>
    <div class="newphone__rent_none hover">
        <div class="box-column">
            <div class="box-flex">
                <div class="violet">{order.name}</div>
            </div>
            <div class="gray">{translateText('player2', 'Количество')}: <span class="violet">{order.name}</span></div>
            <div class="gray mt-5">{translateText('player2', 'Потрачено')}: <span class="green">${format("money", order.price)}</span></div>
        </div>
        <div class="newphone__rent_noneimage person" style="background-image: url({getPng(order.productType, order.name, order.itemId)})"></div>
    </div>
    <div class="newphone__project_button" on:click={cancelOrder}>{translateText('player2', 'Отменить')}</div>
    <div class="violet box-center m-top10" on:click={onSelectBack}>{translateText('player2', 'Назад')}</div>
</div>