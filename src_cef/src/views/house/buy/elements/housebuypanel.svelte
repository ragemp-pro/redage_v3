<script>
    export let viewData;
    export let onBuy;
    export let onExit;
    import { format } from 'api/formatter'
        
    const descToClass = [

    ]
</script>

<div class="house__buy">
    <div class="house__header">Дом №{viewData.id}</div>
    <div class="house__info">
        {descToClass [viewData.class]} в <span>Rockford Hills</span>
    </div>
    <div class="house__buy_flex">
        <div class="box-column">
            <div class="house__stat mint">${format("money", viewData.gosPrice)}</div>
            <div class="house__menu">Государственная стоимость:</div>
        </div>
        {#if viewData.price}
        <div class="house__line"></div>
        <div class="box-column">
            <div class="house__stat orange">${format("money", viewData.price)}</div>
            <div class="house__menu">Предложенная стоимость:</div>
        </div>
        {/if}
    </div>
    {#if viewData.price}
    <div class="house__buy_center">
        <div class="house__menu">Улучшения:</div>
        <div class="box-column">
            <div class="house__upgrade_element">Сигнализация</div>
            <div class="house__upgrade_element">Взломостойкий сейф</div>
            <div class="house__upgrade_element">Шкаф для вещей [x3]</div>
        </div>
    </div>
    {/if}
    <div class="house_bottom_buttons back" on:click={onBuy}>
        <div>Приобрести</div>
        <div class="house_bottom_button">ENTER</div>
    </div>
    <div class="house_bottom_buttons esc" on:click={onExit}>
        <div>Выйти</div>
        <div class="house_bottom_button">ESC</div>
    </div>
    <div class="houseicon-house house__background"></div>
</div>