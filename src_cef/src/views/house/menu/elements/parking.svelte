<script>
    import { vehicleModelsToMoto, colorData } from '@/json/vehicles'


    import {executeClient, executeClientAsync, executeClientAsyncToGroup} from 'api/rage'
	import { selectPopup, popudData } from '@/views/house/menu/stores.js';

    let houseData = {}
    executeClientAsync("phone.house.houseData").then((result) => {
        if (hasJsonStructure(result))
            houseData = JSON.parse(result);

        updateMaxParkingPlaces ()
    });

    let garagesData = [];
    executeClientAsync("phone.house.garagesData").then((result) => {
        if (hasJsonStructure(result))
            garagesData = JSON.parse(result);

        updateMaxParkingPlaces ()
    });


    let maxParkingPlaces = 2;

    const updateMaxParkingPlaces = () => {
        if (houseData && houseData.garageType != undefined && garagesData && garagesData [houseData.garageType])
            maxParkingPlaces = garagesData [houseData.garageType].MaxCars;
    }





    let houseCars = {};
    let selectCar = {};
    let carsData = [];

    const updateLoad = () => {
        executeClientAsync("phone.cars.getCarsList").then((result) => {
            if (hasJsonStructure(result)) {
                carsData = JSON.parse(result);
                //
                if (selectCar && selectCar.place !== undefined) {

                    const placeId = selectCar.place;
                    const carIndex = carsData.findIndex(car => car.number == selectCar.number);
                    if (carIndex != -1 && carsData [carIndex]) {
                        selectCar = carsData [carIndex];
                        selectCar.place = placeId;
                    } else {
                        selectCar = {};
                        selectCar.place = placeId;
                    }
                }
                //

                let newCarsData = {};

                carsData.forEach(car => {
                    if (car && typeof car.place === "number" && car.place >= 0) {
                        newCarsData [car.place] = car;
                    }
                });

                houseCars = newCarsData;

                isUpdate = false;
                selectCar = {};
            }
        });
    }

    updateLoad ();
    import { addListernEvent } from 'api/functions';
    addListernEvent("phoneCarsLoad", updateLoad);


    let selectPage = 0;

    const onLeft = () => {
        if (--selectPage < 0)
            selectPage = 0;
    }

    const onRight = () => {
        const count = Math.ceil (maxParkingPlaces / 10);
        
        if (++selectPage >= count - 1)
            selectPage = count - 1;
    }


    const handleKeyUp = (event) => {
        
        const { keyCode } = event;

        switch(keyCode) {
            case 37:
                onLeft ();
                break;
            case 39:
                onRight ();
                break;
            case 27: // esc                    
                onExit ();
                break;
            case 13:
                onOpen ()
                break;
        }
    }

    const onSelectCar = (index) => {
        if (houseCars [index]) {
            selectCar = houseCars [index];
            selectCar.place = index;
        } else {                
            selectCar = {};
            selectCar.place = index;
        }
    }
    
    let isUpdate = false;

    const OnChangeCars = (sqlId) => {
        if (!window.loaderData.delay ("OnChangeCars", 1))
            return;
        if (isUpdate)
            return;
            
        isUpdate = true;
        executeClient ("client.garage.parking", sqlId, selectCar.place);
    
        selectCar = {};
    }

    import {hasJsonStructure} from "api/functions";


    const getColor = (color) => {
        if (typeof color == "number" || colorData[color] !== undefined) {
            return colorData[color];
        }
        return `rgba(${color.Red},${color.Green},${color.Blue},1)`;
    }

    const onOpen = () => {
        if (houseData && houseData.garageType != undefined && garagesData && garagesData [houseData.garageType + 1])
            selectPopup.set ("popupUpdateGarage")
    }

    export let onExit;
</script>
    
<svelte:window on:keyup={handleKeyUp} />

<div class="house__parking_box">
    <div class="house__mainmenu_block parking">
        {#if selectCar && selectCar.place !== undefined}
            {#each carsData as car, index}
                {#if car.number !== selectCar.number && !car.isAir}
                    <div class="house__mainmenu_categorie" on:click={() => OnChangeCars (car.sqlId)}>
                        <div class="line" style="background: {getColor (car.color)};"></div>
                        {car.model.toUpperCase()} [{car.number}] {car.sell ? " (Личная)" : ""}
                    </div>
                {/if}
            {/each}
        {/if}
    </div>
    <div class="house__parking">
        <div class="house__header">Управление парковкой</div>
        <div class="house__floor">
            <div class="house__floor_lines">
                {#each Array(Math.ceil (maxParkingPlaces / 10)).fill(0) as _, index}
                <div class="house__floor_line" class:active={selectPage === index} on:click={() => selectPage = index} />
                {/each}
            </div>
            <div class="box-flex">
                <div class="house__button_white" on:click={onLeft}>&#8592;</div>
                <div class="box-column">
                    <div class="house__current_floor">{selectPage + 1}</div>
                    <div class="house__gray">Текущий этаж</div>
                </div>
                <div class="house__button_white" on:click={onRight}>&#8594;</div>
            </div>
        </div>
        <div class="house__parking_names">
            {#each Array(5).fill(0) as _, index}
                {#if index + (10 * selectPage) < maxParkingPlaces}
                    {#if houseCars [index + (10 * selectPage)] != undefined}
                    <div class="house__car_name">{houseCars [index + (10 * selectPage)].model} [{houseCars [index + (10 * selectPage)].number}]</div>
                    {:else}                 
                    <div class="house__car_name">Свободно</div>
                    {/if}
                {/if}
            {/each}
        </div>
        <div class="house__parking_top">
            <div class="house__parking_places">
                {#each Array(5).fill(0) as _, index}
                    {#if index + (10 * selectPage) < maxParkingPlaces}
                        <div class="house__car_place top" on:click={() => onSelectCar (index + (10 * selectPage))}>
                            <div class="house__car_position">{index + (10 * selectPage) + 1}</div>
                            {#if houseCars [index + (10 * selectPage)] != undefined}
                                {#if vehicleModelsToMoto.includes (houseCars [index + (10 * selectPage)].model.toLowerCase())}
                                <div class="houseicon-bike-top house__car_image" style="color: {getColor (houseCars [index + (10 * selectPage)].color)}">
                                    <div class="house__bike_details"></div>
                                </div>
                                {:else}
                                <div class="houseicon-car-top house__car_image" style="color: {getColor (houseCars [index + (10 * selectPage)].color)}">
                                    <div class="house__car_details"></div>
                                </div>
                                {/if}
                            {:else}
                            <div class="house__circle_green">+</div>
                            {/if}
                        </div>
                    {/if}
                {/each}
            </div>
        </div>
        <div class="box-column box-center m-60">
            <div class="house__text_big">{maxParkingPlaces}</div>
            <div class="house__gray">Всего доступно мест</div>
        </div>
        <div class="house__parking_top bottom">
            <div class="house__parking_places bottom">
                {#each Array(5).fill(0) as _, index}
                    {#if index + (5) + (10 * selectPage) < maxParkingPlaces}
                        <div class="house__car_place top" on:click={() => onSelectCar (index + (5) + (10 * selectPage))}>
                            <div class="house__car_position">{index + (5) + (10 * selectPage) + 1}</div>
                            {#if houseCars [index + (5) + (10 * selectPage)] != undefined}

                            {#if vehicleModelsToMoto.includes (houseCars [index + (5) + (10 * selectPage)].model.toLowerCase())}                            
                            <div class="houseicon-bike-top house__car_image" style="color: {getColor (houseCars [index + (5) + (10 * selectPage)].color)}">
                                <div class="house__bike_details"></div>
                            </div>
                            {:else}
                            <div class="houseicon-car-top house__car_image" style="color: {getColor (houseCars [index + (5) + (10 * selectPage)].color)}">
                                <div class="house__car_details"></div>
                            </div>
                            {/if}
                            {:else}
                            <div class="house__circle_green">+</div>
                            {/if}
                        </div>
                    {/if}
                {/each}
            </div>
        </div>
        <div class="house__parking_names bottom">
            {#each Array(5).fill(0) as _, index}
                {#if index + (5) + (10 * selectPage) < maxParkingPlaces}
                    {#if houseCars [index + (5) + (10 * selectPage)] != undefined}
                    <div class="house__car_name">{houseCars [index + (5) + (10 * selectPage)].model} [{houseCars [index + (5) + (10 * selectPage)].number}]</div>
                    {:else}                
                    <div class="house__car_name">Свободно</div>
                    {/if}
                {/if}
            {/each}
        </div>
        <div class="houseicon-house house__background"></div>
    </div>
    <div class="house__mainmenu_block parking">
    
    </div>
</div>
<div class="box-between">
    {#if houseData && houseData.garageType != undefined && garagesData && garagesData [houseData.garageType + 1]}
    <div class="house_bottom_buttons back" on:click={onOpen}>
        <div>Улучшить гараж</div>
        <div class="house_bottom_button">ENTER</div>
    </div>
    {/if}
    <div class="house_bottom_buttons esc" on:click={onExit}>
        <div>Выйти</div>
        <div class="house_bottom_button">ESC</div>
    </div>
</div>