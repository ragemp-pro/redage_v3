<script>
    import './assets/css/iconsarenda.css'
    import './assets/css/main.sass'
	import { fade } from 'svelte/transition';
    import { accountVip } from 'store/account'
    import { charMoney, charLVL } from 'store/chars'
    import rangeslider from 'components/rangeslider/index'
    import { format } from 'api/formatter'
    import { executeClient } from 'api/rage'
    export let viewData;

    if (!viewData) viewData = '[]';

    let HourValue = 0;

    const VehicleArray = JSON.parse (viewData);

    let SelectVehicle = -1;

    const authColors =[
        "#000",
        "#fff",
        "#e60000",
        "#ff7300",
        "#f0f000",
        "#00e600",
        "#00cdff",
        "#0000e6",
        "#be3ca5",
    ];
    let colorId = 0;

    const setColor = (index) => {
        if (index === colorId) return;
        colorId = index;
    }

    const rangeslidercreate = () => {
        const max = 8;
        HourValue = 1;
        setTimeout(() => {
            rangeslider.create(document.getElementById("rangeslider"), {min: 1, max: max, value: 1, step: 1, onSlide: (value, percent, position) => {
                HourValue = Number(value);
            }});
        }, 0);
    }

    const onBuy = () => {
        if ($charMoney < GetRentCarCash (VehicleArray [SelectVehicle].Price * HourValue)) {            
            window.notificationAdd(1, 9, `У Вас не достаточно средств!`, 3000);
            return;
        }
        executeClient ('client.rentcar.buy', VehicleArray [SelectVehicle].Id, colorId, HourValue);
    }

    const onExit = () => {        
        executeClient ('client.rentcar.exit');
    }

    const GetRentCarCashToLevel = (Price) => {
        const level = $charLVL;

        if (level <= 2) Price = Math.round(Price * 1.0);
        else if (level <= 4) Price = Math.round(Price * 1.5);
        else if (level <= 6) Price = Math.round(Price * 2.0);
        else if (level <= 9) Price = Math.round(Price * 4.5);
        else if (level <= 19) Price = Math.round(Price * 6.0);
        else Price = Math.round(Price * 8.0);
        return Price;
    }

    const GetRentCarCash = (Price) => {

        switch ($accountVip)
        {
            case 1:
                Price = Math.round(Price * 0.95);
                break;
            case 2:
                Price = Math.round(Price * 0.9);
                break;
            case 3:
                Price = Math.round(Price * 0.85);
                break;
            case 4:
            case 5:
                Price = Math.round(Price * 0.8);
                break;
        }
        return GetRentCarCashToLevel (Price);
    }

    const handleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        if (SelectVehicle !== -1) SelectVehicle = -1;
        else onExit ();
    }

</script>

<svelte:window on:keyup={handleKeyDown}/>

<div id="arenda-page">
    <div class="arenda" class:nonactive={SelectVehicle !== -1}>
        <div class="arenda-header">
            <div class="arenda-header__text">
                <div class="arenda-header__title">Аренда</div>
                <div class="arenda-header__main-text">Необходимо транспортное средство для перемещения? По всему штату с помощью GPS можно найти похожие места, которые предоставляют разный транспорт в аренду: от лодок и мотоциклов до элитных автомобилей в любых цветах! Управлять арендой можно через телефон, при выхода со штата аренда не сбросится, если у Вас ещё осталось оплаченное время.</div>
            </div>
            <span class="arendaicon-off"  on:click={onExit} />
        </div>
        <div class="arenda-main">
            {#each VehicleArray as item, index}
            <div class="arenda-main__element">
                <div class="arenda-main__title">{item.Model}</div>
                <div class="arenda-main__price">
                    <span class="arendaicon-money"/>
                    ${format("money", GetRentCarCash (item.Price))} / час
                </div>
                <div class="arenda-main__img" style="background-image: url({document.cloud}inventoryItems/vehicle/{item.Model.toLowerCase()}.png)" />
                <div class="arenda-main__button" on:click={() => SelectVehicle = index}>
                    Арендовать
                </div>
            </div>
            {/each}
        </div>
    </div>
    {#if SelectVehicle !== -1}
    <div class="props-arenda">
        <div class="arenda-customize" transition:fade={{duration: 200}}>
            <div class="arenda-customize__title">Аренда</div>
            <div class="arenda-customize__subtitle">{VehicleArray [SelectVehicle].Model}</div>
            <div class="arenda-customize__img" style="background-image: url({document.cloud}inventoryItems/vehicle/{VehicleArray [SelectVehicle].Model.toLowerCase()}.png)" />
            {#if !VehicleArray [SelectVehicle].IsJob}
            <div class="arenda-customize__paragraph">Срок аренды / час</div>
            <div class="arenda-customize__rangeslider">
                <input type="range" id="rangeslider" use:rangeslidercreate />
            </div>
            <div class="arenda-customize__input-description">
                <div class="arenda-customize_gray">1</div>
                <div>${format("money", GetRentCarCash (VehicleArray [SelectVehicle].Price))} / час</div>
                <div class="arenda-customize_gray">8</div>
            </div>
            {/if}
            <div class="arenda-customize__paragraph">Цвет авто</div>
            <div class="adrenda-customize__radio-buttons">
                {#each authColors as value, index}
                    <i key={index} class="color" class:active={colorId === index} on:click={() => setColor (index)} style="background: {value}" />
                {/each}
            </div>
            <div class="arenda-customize__button" on:click={onBuy}>
                К оплате
                <div class="arenda-customize__money">${format("money", GetRentCarCash (VehicleArray [SelectVehicle].Price * HourValue))}</div>
            </div>
            <div class="arenda-customize__exit" on:click={() => SelectVehicle = -1}>Закрыть</div>
        </div>
    </div>
    {/if}
</div>