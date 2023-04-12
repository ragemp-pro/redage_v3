<script>
    import { translateText } from 'lang'
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

    const onAction = (number, func) => {
        executeClient ("client.vehicle.action", number, func);
        onExit ();
    }

    const onExit = () => {        
        executeClient ('client.vehicleair.exit');
    }



    const handleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        if (SelectVehicle !== -1) SelectVehicle = -1;
        else onExit ();
    }

</script>

<svelte:window on:keyup={handleKeyDown}/>

<div id="air-page">
    <div class="arenda" class:nonactive={SelectVehicle !== -1}>
        <div class="arenda-header">
            <div class="arenda-header__text">
                <div class="arenda-header__title">{translateText('vehicle', 'Покупка воздушного транспорта')}</div>
                <div class="arenda-header__main-text">{translateText('vehicle', 'Приехали за своим личным воздушным транспортным средством? Как ни странно - вы по адресу! Если Вы владеете личным вертолетом или самолетом - вы можете заспавнить его тут.')}</div>
            </div>
            <span class="arendaicon-off"  on:click={onExit} />
        </div>
        <div class="arenda-main">
            {#each VehicleArray as item, index}
            <div class="arenda-main__element">
                <div class="arenda-main__title">{item.Model}</div>
                <div class="arenda-main__price">{item.IsSpawn ? "Вызван" : ""}</div>
                <div class="arenda-main__img" style="background-image: url({document.cloud}inventoryItems/vehicle/{item.Model.toLowerCase()}.png)" />
                <div class="arenda-main__button" on:click={() => SelectVehicle = index}>
                    {translateText('vehicle', 'Действие')}
                </div>
            </div>
            {/each}
        </div>
    </div>
    {#if SelectVehicle !== -1}
    <div class="props-arenda">
        <div class="arenda-customize" transition:fade={{duration: 200}}>
            <div class="arenda-customize__title">{translateText('vehicle', 'Действие')}</div>
            <div class="arenda-customize__subtitle">{VehicleArray [SelectVehicle].Model}</div>
            <div class="arenda-customize__img" style="background-image: url({document.cloud}inventoryItems/vehicle/{VehicleArray [SelectVehicle].Model.toLowerCase()}.png)" />
            {#if !VehicleArray [SelectVehicle].IsSpawn}
                <div class="arenda-customize__button" on:click={() => onAction (VehicleArray [SelectVehicle].Number, "spawn")}>
                    {translateText('vehicle', 'Вызвать')}
                </div>
            {:else}
                <div class="arenda-customize__button"
                     on:click={() => onAction (VehicleArray [SelectVehicle].Number, "tune")}>
                    {translateText('vehicle', 'Тюнинговать')}
                </div>
                <!--<div class="arenda-customize__button"
                     on:click={() => onAction (VehicleArray [SelectVehicle].Number, "repair")}>
                    Восстановить (10$)
                </div>
                <div class="arenda-customize__button"
                     on:click={() => onAction (VehicleArray [SelectVehicle].Number, "key")}>
                    Получить дубликат ключа
                </div>
                <div class="arenda-customize__button"
                     on:click={() => onAction (VehicleArray [SelectVehicle].Number, "changekey")}>
                    Сменить замки (100$)
                </div>
                <div class="arenda-customize__button"
                     on:click={() => onAction (VehicleArray [SelectVehicle].Number, "evac")}>
                    Эвакуировать машину (25$)
                </div>
                <div class="arenda-customize__button"
                     on:click={() => onAction (VehicleArray [SelectVehicle].Number, "gps")}>
                    Отметить в GPS
                </div>-->
            {/if}
            <div class="arenda-customize__button" on:click={() => onAction (VehicleArray [SelectVehicle].Number, "sell")}>
                {translateText('vehicle', 'Продать')}
                <div class="arenda-customize__money">${format("money", VehicleArray [SelectVehicle].Price)}</div>
            </div>
            <div class="arenda-customize__exit" on:click={() => SelectVehicle = -1}>Закрыть</div>
        </div>
    </div>
    {/if}
</div>