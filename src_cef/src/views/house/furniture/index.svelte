<script>
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import './main.sass'
    import './fonts/style.css'
    import { ItemType, itemsInfo } from 'json/itemsInfo.js'

    export let viewData;

    if (!viewData)
        viewData = [];

    $: if (viewData && typeof viewData === "string") {
        viewData = JSON.parse (viewData)
    }

    const onBuy = (furniture, type = 0) => {
        if (!window.loaderData.delay ("furniture.buy", 1))
            return;

        executeClient ("client.furniture.buy", furniture.name, type);
    }

    const onExit = () => {        
        executeClient ("client.furniture.close");      
    }

    const onKeyUp = (event) => {
        switch(event.which) {
            case 27:
                onExit ();
                break;
        }
    }
</script>
<svelte:window on:keyup={onKeyUp} />
<!--
    <HousePopup/>
-->
<div id="furniture">
    <div class="house__header">Мебельный магазин</div>
    <div class="house__furniture">
        {#each viewData as furniture}
            <div class="house__furniture_element">
                <div class="box-between">
                    <div class="box-column">
                        <div class="house__furniture_title">{furniture.name}</div>
                        <div class="house__furniture_text money">${format("money", furniture.price)}</div>
                        {#if furniture.items}
                            {#each furniture.items as itemData}
                            <div class="box-flex">
                                <div class="house__furniture_smallimage" style="background-image: url({document.cloud + "inventoryItems/items" + `/${itemData.itemId}.png`})"></div>
                                <div class="house__furniture_text">{itemData.price} {window.getItem (itemData.itemId).Name}</div>
                            </div>
                            {/each}
                        {/if}
                        <!--<div class="box-flex">
                            <div class="house__furniture_smallimage"></div>
                            <div class="house__furniture_text">10 сосны</div>
                        </div>
                        <div class="box-flex">
                            <div class="house__furniture_smallimage"></div>
                            <div class="house__furniture_text">10 сосны</div>
                        </div>
                        <div class="box-flex">
                            <div class="house__furniture_smallimage"></div>
                            <div class="house__furniture_text">10 сосны</div>
                        </div>-->
                    </div>
                    <div class="house__furniture_image" style="background-image: url({document.cloud + "inventoryItems/furniture" + `/${furniture.model}.png`});"></div>
                </div>
                <div class="box-between">
                    <div class="house__element_button" on:click={() => onBuy (furniture)}>
                        <div class="houseicon-safe house__furniture_icon"></div>
                        Купить
                    </div>
                    <div class="house__element_button" on:click={() => onBuy (furniture, 1)}>
                        <div class="houseicon-garage house__furniture_icon"></div>
                        Скрафтить самому
                    </div>
                </div>
            </div>
        {/each}
    </div>
    <div class="box-between" style="margin-top: auto">
        <div class="house_bottom_buttons esc" on:click={onExit}>
            <div>Выйти</div>
            <div class="house_bottom_button">ESC</div>
        </div>
    </div>
</div>