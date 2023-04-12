<script>
    import {executeClient, executeClientAsync} from 'api/rage'
    import { format } from 'api/formatter'
	import { selectPopup, popudData } from '@/views/house/menu/stores.js';
    import {hasJsonStructure} from "api/functions";
    //
    //

    let type = 0;
    let nextType = 0;

    executeClientAsync("phone.house.houseData").then((result) => {
        if (hasJsonStructure(result)) {
            result = JSON.parse(result);

            type = result.garageType;
            nextType = (type + 1 == 6) ? type + 2 : type + 1;
        }
    });

    let garagesData = [];
    executeClientAsync("phone.house.garagesData").then((result) => {
        if (hasJsonStructure(result))
            garagesData = JSON.parse(result);
    });
    
    const handleKeyUp = (event) => {            
        const { keyCode } = event;
        switch(keyCode) {
            case 27: // down
                onExit ();
                break;
            case 13:
                onBuy ();
                break;
        }
    }

    const onBuy = () => {
        if (!window.loaderData.delay ("onGarageUpdate", 5))
            return;

        executeClient ("client.garage.update");
        onExit ();
    }

    const onExit = () => {        
        selectPopup.set (null)
        popudData.set ({})
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

{#if garagesData && garagesData [nextType] && garagesData [type]}
<div id="house__popup">
    <div class="house__popup_block">
        <div class="houseicon-garage house__popup_image"></div>
        <div class="house__popup_header">ГАРАЖ</div>
        <div class="house__popup_header"><span class="orange">+{garagesData [nextType].MaxCars - garagesData [type].MaxCars}</span> МЕСТ</div>
        <div class="house__gray">Добавит несколько слотов для транспорта</div>
        <div class="house__white">Стоимость улучшения</div>
        <div class="box-flex">
            <div class="box-column">
                <div class="house__popup_top">
                    {nextType}
                    <div class="house__popup_icon"></div>
                </div>
                <div class="house__gray">Уровень</div>
            </div>
            <div class="house__and">и</div>
            <div class="box-column">
                <div class="house__popup_top">{format("money", garagesData [nextType].Price)}</div>
                <div class="house__gray">{!garagesData [nextType].IsDonate ? "Валюты" : "RedBucks"}</div>
            </div>
        </div>
    </div>
    <div class="box-flex">
        <div class="house_bottom_buttons back" on:click={onBuy}>
            <div class="house_bottom_button">Enter</div>
            <div>Купить</div>
        </div>
        <div class="house_bottom_buttons back ml-20" on:click={onExit}>
            <div>Выйти</div>
            <div class="house_bottom_button">ESC</div>
        </div>
    </div>
</div>
{/if}