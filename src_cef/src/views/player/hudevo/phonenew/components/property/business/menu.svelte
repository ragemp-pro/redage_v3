<script>
    import { translateText } from 'lang'
    export let onMainPage;
    import { format } from 'api/formatter'
    export let selectedId;


    import { executeClient, executeClientAsyncToGroup, executeClientToGroup } from "api/rage";
    import { addListernEvent, hasJsonStructure } from "api/functions";

    let stats = {};
    let isOrder = false;
    let isProductList = false;

    const updateData = () => {
        executeClientAsyncToGroup("business.getStats").then((result) => {
            if (hasJsonStructure(result))
                stats = JSON.parse(result);
        });
        executeClientAsyncToGroup("business.getOrders").then((result) => {
            if (hasJsonStructure(result)) {
                result = JSON.parse(result);
                isOrder = result.length > 0;
            }
        });

        executeClientAsyncToGroup("business.getProducts").then((result) => {
            if (hasJsonStructure(result)) {
                result = JSON.parse(result);
                isProductList = result.length > 1;
            }
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    export let onSelectedViewBusiness;
    export let onSelectedView;

    const onSell = () => {
        if (!window.loaderData.delay ("business.sell", 1.5))
            return;

        executeClientToGroup ("business.sell")
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__rent_list" in:fade>
    <div class="newphone__rent_none hover">
        <div class="box-column">
            <div class="box-flex">
                <div class="violet">{translateText('player2', 'Бизнес')} #{selectedId}</div>
            </div>
            <div class="gray">{translateText('player2', 'Прибыль сегодня')}: <div class="green">${format("money", Math.round(stats.pribil - stats.zatratq))}</div></div>
            <div class="gray mt-5">{translateText('player2', 'На счету')}: <div class="green">${format("money", stats.cash)}</div></div>
        </div>
        <div class="newphone__rent_noneimage business"></div>
    </div>

    <div class="newphone__project_button property" on:click={() => onSelectedViewBusiness("Stocks")}>{translateText('player2', 'Склад')}</div>
    {#if isOrder}
        <div class="newphone__project_button property" on:click={() => onSelectedViewBusiness("Orders")}>{translateText('player2', 'Активные заказы')}</div>
    {/if}
    <div class="newphone__project_button property" on:click={() => onSelectedViewBusiness("Stats")}>{translateText('player2', 'Статистика за месяц')}</div>
    {#if isProductList}
        <div class="newphone__project_button property" on:click={() => onSelectedViewBusiness("TopOrders")}>{translateText('player2', 'Рейтинг покупок')}</div>
    {/if}
    <div class="newphone__project_button property" on:click={() => onSelectedViewBusiness("TopClients")}>{translateText('player2', 'Рейтинг покупателей')}</div>
    <div class="newphone__project_button property" on:click={() => executeClient ("gps.pointDefault", "biz")}>{translateText('player2', 'Показать на карте')}</div>
    <div class="newphone__project_button" on:click={onSell}>{translateText('player2', 'Продать за')} ${format("money", stats.sellPrice)}</div>
    <div class="violet box-center m-top10" on:click={() => onSelectedView ()}>{translateText('player2', 'Назад')}</div>
</div>