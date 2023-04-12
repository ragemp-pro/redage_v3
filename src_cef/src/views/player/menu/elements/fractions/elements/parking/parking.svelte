<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    executeClientToGroup('vehiclesLoad')

    let isVehicleUpdateRank = false;
    let isSellVehicle = false;
    let vehicles = [];

    const getVehicles = (_isVehicleUpdateRank, _vehicles, _isSellVehicle) => {
        isVehicleUpdateRank = _isVehicleUpdateRank;
        isSellVehicle = _isSellVehicle;

        if (_vehicles && typeof _vehicles === "string")
            vehicles = JSON.parse(_vehicles);

    }

    import { addListernEvent } from "api/functions";
    import { onUpdateRank } from "../../data";

    addListernEvent ("table.vehicles", getVehicles)


    const onEvacuation = (number) => {
        executeClientToGroup('evacuation', number);
    }

    const onGps = (number) => {
        executeClientToGroup('gps', number);
    }
    import Logo from '../other/logo.svelte'

    const onSellCar = (number) => {
        executeClientToGroup('sellCar', number);
    }

</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-rank"></span>
                <div class="fractions__header_title">{translateText('player1', 'Парковка')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Управление транспортом')}</div>
        </div>
    </div>
</div>
<div class="fractions__content backgr h-fit">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-parking"></span>
            <div class="fractions__main_title">{translateText('player1', 'Список транспорта')}</div>
        </div>
        <div class="fractions__input large">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder={translateText('player1', 'Поиск..')}>
        </div>
    </div>
    <div class="fractions__main_scroll extrabig h-fit fractions__main_grid mt-40">
        {#each vehicles as item, index}
           <div class="box-column">
                <div class="fractions__element_black newparams">
                    <div class="box-column">
                        <div class="fractions__black_title">{item.model}</div>
                        <div class="fractions__black_title gray">{item.number}</div>
                        <div class="fractions__black_title gray mb-18">{item.rankName}</div>
                        <!--<div class="fractions__black_subtitle silver">Состояние: <span class="whitecolor">100%</span></div>-->
                    </div>
                    <div class="fractions__black_img" style="background-image: url({document.cloud}inventoryItems/vehicle/{item.model.toLowerCase()}.png)"></div>
                </div>
                <div class="box-flex mt-8">
                    {#if item.isEvacuation}
                        <div on:click={() => onEvacuation(item.number)} class="fractions__main_button w-134 mr-10">Эвакуировать</div>
                    {/if}
                    {#if item.isGps}
                        <div on:click={() => onGps(item.number)} class="fractions__main_button w-134">Отметить в GPS</div>
                    {/if}
                </div>
               {#if isVehicleUpdateRank}
                    <div on:click={() => onUpdateRank("Список рангов", "fractionsicon-parking", "Выберите ранг, с которого будет доступен этот транспорт.", 'updateVehicleRank', item.number)} class="fractions__main_button w-100 mt-8">Настроить доступы</div>
               {/if}
               {#if isSellVehicle}
                    <div on:click={() => onSellCar(item.number)} class="fractions__main_button w-100 mt-8">Продать</div>
               {/if}
           </div>
        {/each}
    </div>
</div>