<script>
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'
    import { categoriesList, currentPage } from '../../stores'
    import { format } from "api/formatter";
    import {executeClientToGroup, executeClientAsyncToGroup, executeClientAsync} from "api/rage";

    function closeMenu () {
        currentPage.set("mainmenu")
    }


    import Loader from './../loader.svelte'

    executeClientToGroup ("truck.load");

    let isLoad = false;

    let isSelect = false;
    let selectTrucker = {};
    const getData = () => {
        executeClientAsyncToGroup("truck.getSelect").then((result) => {
            if (result && typeof result === "string") {
                selectTrucker = JSON.parse(result);
                isSelect = selectTrucker && selectTrucker.uid;

                updateHeightMap ();

                if (isSelect)
                    position = selectTrucker.pos;
                else
                    getPosition ();
            }
        });

        isLoad = true;
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("phoneTruckerLoad", getData);

    import Select from './select.svelte'


    import List from './list.svelte'

    import Map from './../map/index.svelte'

    let mainElement;
    let otherElement;

    let elementWidth = 0;
    let elementHeight = 0;

    const updateHeightMap = () => {
        setTimeout(() => {
            if (mainElement && otherElement) {
                const defaultMainHeight = 634;

                const main = mainElement.getBoundingClientRect();
                const other = otherElement.getBoundingClientRect();

                if (main && other) {
                    elementWidth = main.width;
                    elementHeight = main.height - other.height;
                    elementHeight += (defaultMainHeight / main.height) * 45;
                }
            }
        }, 0)
    }

    let position = {
        x: -301.46353,
        y: 2785.5164,
        z: 60.438744
    };

    const getPosition = () => {
        executeClientAsync("getPosition").then((result) => {
            if (result && typeof result === "string") {
                position = JSON.parse(result);
            }
        });
    }

    getPosition ();
    import { fade } from 'svelte/transition'
</script>

{#if !isLoad}
    <Loader/>
{:else}
    <div class="newphone__maps mech" bind:this={mainElement} in:fade>

        {#if position && elementHeight}
            <div class="newphone__maps_box" style={elementHeight ? `height: ${elementHeight}px`: ""}>
                <Map getPosition={[position.x, position.y]} {elementWidth} {elementHeight} />
            </div>
        {/if}

        <Header />

        <div class="newphone__maps_content">
            <div class="box-flex">
                <div class="newphone__maps_headerimage trucker"></div>
                <div class="newphone__maps_headertitleGPS newphone__shadow"><span class="truckercolor">Red</span>Age Trucker</div>
            </div>
        </div>

        <div class="newphone__maps_categories" style="background: linear-gradient(90deg, #676869 0%, #141414 94.41%)" bind:this={otherElement}>
            {#if isSelect}
                <div class="newphone__maps_price">
                    <div class="box-column">
                        <div class="newphone__maps_pricetitle">Клиент:</div>
                        <div class="newphone__maps_pricesubtitle">{selectTrucker.name}</div>
                    </div>
                    <div class="newphone__maps_car truck"></div>
                </div>
            {/if}
            <div class="newphone__maps_subcategories">
                {#if isSelect}
                    <Select {selectTrucker} {closeMenu} />
                {:else}
                    <List {closeMenu} />
                {/if}
                <HomeButton />
            </div>
        </div>
    </div>
{/if}