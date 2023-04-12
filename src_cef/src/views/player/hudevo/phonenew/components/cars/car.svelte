<script>
    import { TimeFormat } from 'api/moment'
    import { translateText } from 'lang'
    export let selectedCar;
    export let OnUpdatePage;

    import { format } from 'api/formatter'
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from 'api/rage'

    const functionData = [
        {
            name: translateText('player2', 'Восстановить'),
            func: "repair",
            isGarage: true,
            isPark: true
        },
        {
            name: translateText('player2', 'Получить дубликат ключа'),
            func: "key",
        },
        {
            name: translateText('player2', 'Сменить замки'),
            func: "changekey",
            isGarage: true,
            sell: true,
            isPark: true
        },
        {
            name: translateText('player2', 'Эвакуировать машину'),
            func: "evac",
            isGarage: true,
            isCarGarage: false,
            isPark: true
        },
        {
            name: translateText('player2', 'Отметить в GPS'),
            func: "gps",
            isCarGarage: false,
            isPark: true
        },
        {
            name: translateText('player2', 'Продать за $'),
            func: "sell",
            sell: true
        },
        //{
        //    name: "Тюнинговать",
        //    func: "tune"
        //},
    ];

    let inGarage = false

    executeClientAsyncToGroup("cars.inGarage").then((result) => {
        inGarage = result;
    });

    executeClientAsyncToGroup("cars.inGarage").then((result) => {
        inGarage = result;
    });

    const isVisible = (func, car, _inGarage) => {

        if (!func)
            return false;

        if (!car.isCreate && func.func !== "sell" && func.func !== "gps")
            return false;


        if (car.isAir) {
            //if (func.func === "evac")
            //    return false;

        } else {

            //if (func.func === "tune")
            //    return false;

            if (func.isGarage && !_inGarage)
                return false;

            if (func.isCarGarage != undefined && func.isCarGarage !== car.isCarGarage)
                return false;

            if (func.sell && !car.sell)
                return false;

            if (func.isPark != undefined && -1 === car.place)
                return false;

            if (func.func !== "gps" && !!car.ticket)
                return false;
        }

        return true;
    }

    const onEnter = (func, car) => {
        if (!window.loaderData.delay ("onVehicleAction", 1))
            return;
        if (!car)
            return;     
        else if (!isVisible (func, car, inGarage))
            return;

        executeClient ("client.vehicle.action", car.number, func.func);

        if (func.func === "sell")
            executeClientToGroup ("close")
    }	
    import { fade } from 'svelte/transition'


    const onEnterRent = (func) => {
        if (!window.loaderData.delay ("onVehicleAction", 1))
            return;

        executeClient ("client.rentcar.func", func);

        executeClientToGroup ("close")
    }
</script>
<div class="newphone__rent_list" in:fade>
    <div class="newphone__rent_none vehicle">
        <div class="box-column">
            <div class="box-flex">
                <div class="orange">{selectedCar.model}</div>
                <div class="newphone__rent_status">{selectedCar.header}</div>
            </div>
            {#if selectedCar.isRent && !selectedCar.isJob}
                <div class="gray">{translateText('player2', 'Продлено до')}:</div>
                <div class="date">
                    {TimeFormat (selectedCar.date, "H:mm DD.MM.YYYY")}
                </div>
            {:else}
                <div class="gray">{translateText('player2', 'Номер')}:</div>
                <div class="date">
                    {selectedCar.number}
                </div>
            {/if}
        </div>
        <div class="newphone__rent_noneimage rent" style="background-image: url('{document.cloud}inventoryItems/vehicle/{selectedCar.model.toLowerCase()}.png')"></div>
    </div>
   <!--{#if selectedCar.time == "Аренда"}
        <div class="newphone__project_button rent">Продлить аренду</div>
        <div class="newphone__project_button rent">Отказаться от аренды</div>
    {/if}
    <div class="newphone__project_button rent">Показать на карте</div>
    <div class="newphone__project_button rent">Продлить аренду</div>
    <div class="newphone__project_button rent">Отказаться от аренды</div>
    <div class="newphone__project_button rent">Показать на карте</div>-->
    {#if selectedCar.isRent}
        <div on:click={() => onEnterRent ("gpstrack")} class="newphone__project_button rent">{translateText('player2', 'Показать на карте')}</div>
        {#if !selectedCar.isJob}
        <div on:click={() => onEnterRent ("datetime")} class="newphone__project_button rent">{translateText('player2', 'Продлить время аренды за')} ${format("money", selectedCar.rentPrice)}</div>
        {/if}
        <div on:click={() => onEnterRent ("stoprent")} class="newphone__project_button rent">{translateText('player2', 'Отказаться от аренды')}</div>
    {:else}
        {#each functionData as func}
            {#if isVisible (func, selectedCar, inGarage)}
                <div on:click={() => onEnter (func, selectedCar)} class="newphone__project_button rent">
                    {func.name}
                    {#if func.func == "sell"}
                        {format("money", selectedCar.sell)}
                    {/if}
                </div>
            {/if}
        {/each}
    {/if}
    <div class="orange box-center m-top10" on:click={() => OnUpdatePage ("List")}>{translateText('player2', 'Назад')}</div>
</div>