<script>
    import { executeClient } from 'api/rage'
    import './main.sass'
    import './fonts/style.css'

    import RieltHouses from './elements/rielt-house.svelte'
    import RieltBusiness from './elements/rielt-business.svelte'
    import RieltInfoHouses from './elements/hrielt-info.svelte'
    import RieltInfoBusiness from './elements/brielt-info.svelte'

    
    export let viewData;

    if (!viewData) viewData = {
        buyPrice: 5000,
        allHouse: 0,
        houseData: [],
        allBusiness: 0,
        businessData: [],
    }


    let selectData = null;
    let houseData = [];
    let businessData = [];


    $: if (viewData.houseData && typeof viewData.houseData === "string") {
        houseData = JSON.parse (viewData.houseData)
    }

    $: if (viewData.businessData && typeof viewData.businessData === "string") {
        businessData = JSON.parse (viewData.businessData)
    }
    const Views = {
        RieltHouses,
        RieltBusiness
    }

    let SelectViews = "RieltHouses";

    const OnUpdatePage = (page) => {
        selectData = null;
        SelectViews = page;
    }

    
    const onAddHouse = (data) => {
        data = JSON.parse (data)

        if (!data || !data.length)
            return;

        houseData = [
            ...data,
            ...houseData
        ]
    }

    const onAddBusiness = (data) => {
        data = JSON.parse (data)

        if (!data || !data.length)
            return;

        businessData = [
            ...data,
            ...businessData
        ]
    }

    window.events.addEvent("cef.rieltagency.addHouse", onAddHouse);
    window.events.addEvent("cef.rieltagency.addBusiness", onAddBusiness);

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        window.events.removeEvent("cef.rieltagency.addHouse", onAddHouse);
        window.events.removeEvent("cef.rieltagency.addBusiness", onAddBusiness);
    });

    const onSelectData = (data, type) => {
        selectData = data;
        selectData.typeData = type;
    }

    const handleKeyUp = (event) => {
        const { keyCode } = event;
        
        if (keyCode === 27) 
            onEsc ();
        else if (keyCode === 13)
            onBuy ();
    }

    const onEsc = () => {
        
        if (selectData)
            selectData = null;
        else
            executeClient ("client.rieltagency.close");
    }

    const onBuy = () => {
        if (selectData) {
            executeClient ("client.rieltagency.buy", selectData[0], selectData.typeData == "house" ? 0 : 1);
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div id="rielt">
    <div class="rielt__panel">
        <div class="rielt__header">Риэлторское агенство</div>
        <div class="rielt__mainmenu">
        <div class="rielt__mainmenu_block">
            <div class="rielt__mainmenu_categorie big" class:active={SelectViews == "RieltHouses"} on:click={() => OnUpdatePage ("RieltHouses")}>
                <div class="line"></div>
                <div class="box-column">
                    <div class="rielt__mainmenu_categorie-header">Дома</div>
                    <div class="rielt__gray">
                        Всего в штате: <span class="rielt__white">{viewData.allHouse}</span>
                    </div>
                    <div class="rielt__gray">
                        Сейчас в продаже: <span class="rielt__white">{houseData.length}</span>
                    </div>
                </div>
            </div>
            <div class="rielt__mainmenu_categorie big" class:active={SelectViews == "RieltBusiness"} on:click={() => OnUpdatePage ("RieltBusiness")}>
                <div class="line"></div>
                <div class="box-column">
                    <div class="rielt__mainmenu_categorie-header">Бизнесы</div>
                    <div class="rielt__gray">
                        Всего в штате: <span class="rielt__white">{viewData.allBusiness}</span>
                    </div>
                    <div class="rielt__gray">
                        Сейчас в продаже: <span class="rielt__white">{businessData.length}</span>
                    </div>
                </div>
            </div>
        </div>
        <div class="rielt__mainmenu_center rielt">
            {#if selectData && selectData.typeData == "house"}
                <RieltInfoHouses {selectData} buyPrice={viewData.buyPrice} />
            {:else if selectData && selectData.typeData == "business"} 
                <RieltInfoBusiness {selectData} buyPrice={viewData.buyPrice} />
            {:else if Views[SelectViews] && ((SelectViews === "RieltHouses" && houseData.length) || (SelectViews === "RieltBusiness" && businessData.length))} 
                <svelte:component this={Views[SelectViews]} {houseData} {businessData} {onSelectData} />
            {:else}
                <div class="houseicon-time rielt__rielt_none">
                    <div class="absolute">
                        <div class="rielt__rielt_title font-36">В продаже: 0</div>
                        <div class="rielt__rielt_subtitle">Ожидайте</div>
                    </div>
                </div>
            {/if}
        </div>
        <div class="rielt__mainmenu_block">
            <div class="rielt__rielt_block-info">
                <div class="rielt__rielt_title">{houseData.length + businessData.length}</div>
                <div class="rielt__rielt_subtitle">Объектов недвижимости доступно</div>
            </div>
        </div>
    </div>
    <div class="box-between">
        {#if selectData}
            <div class="house_bottom_buttons back" on:click={onBuy}>
                <div>Выбрать</div>
                <div class="house_bottom_button">Enter</div>
            </div>
        {/if}
        <div class="house_bottom_buttons esc" on:click={onEsc}>
            <div>Выйти</div>
            <div class="house_bottom_button">ESC</div>
        </div>
    </div>
    <div class="houseicon-house rielt__background"></div>
    </div>
</div>