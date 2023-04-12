<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";

    let stock = {}

    const getStock = () => {
        executeClientAsyncToGroup("getStock").then((result) => {
            if (result && typeof result === "string")
                stock = JSON.parse(result);
        });
    }
    getStock();

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.isStock", getStock)

    const updateGunStock = () => {
        executeClientToGroup("updateGunStock")
    }

    const updateStock = () => {
        executeClientToGroup("updateStock")
    }
</script>

<div class="fractions__main_box w-215">
    <div class="fractions__main_head">
        <span class="fractionsicon-stock"></span>
        <div class="fractions__main_title">{translateText('player1', 'Склад')}</div>
    </div>
    {#if stock.isGunAccessStock}
        {#if stock.isGunAccessStock}
            <div class="fractions__main_button medium" on:click={updateGunStock}>{stock.isGunStock ? translateText('player1', 'Закрыть склад') : translateText('player1', 'Открыть склад')}</div>
        {/if}

        {#if stock.isAccessStock}
            <div class="fractions__main_button medium" on:click={updateStock}>{stock.isStock ? translateText('player1', 'Запретить крафт') : translateText('player1', 'Разрешить крафт')}</div>
        {/if}
    {:else}
        <div class="fractions__main_element">
            <div class="fractions_stats_title">{translateText('player1', 'Крафт')}:</div>
            <div class="fractions__stats_subtitle">{stock.isStock ? translateText('player1', 'Разрешён') : translateText('player1', 'Запрещён')}</div>
        </div>

        {#if stock.isAccessStock}
            <div class="fractions__main_button medium" on:click={updateStock}>{stock.isStock ? translateText('player1', 'Запретить крафт') : translateText('player1', 'Разрешить крафт')}</div>
        {/if}
    {/if}
</div>