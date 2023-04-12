<script>
    import { currentPage } from "../../stores";

    import Header from '../header.svelte'

    function closeMenu () {
        currentPage.set("mainmenu")
    }

    import { executeClientAsync, executeClientAsyncToGroup } from "api/rage";

    import { fade } from 'svelte/transition'


    import Map from './../map/index.svelte'

    let mainElement;
    let otherElement;

    let elementWidth = 0;
    let elementHeight = 0;

    const setOtherElement = (el) => {
        otherElement = el;
        updateHeightMap ();
    }

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
        }, 0);
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

                updateHeightMap ();
            }
        });
    }

    const setPosition = (pos) => position = pos;
    getPosition ();


    import Client from './client/index.svelte'
    import Driver from './driver/index.svelte'
    import List from './list.svelte'

    let selectView = "List"
    const onSelectView = (view) => selectView = view;

    const views = {
        List,
        Client,
        Driver
    }

    import { onMount } from 'svelte';
    onMount (() => {
        updateHeightMap ();
    });

    let isLoad = false;

    import Loader from './../loader.svelte'



    const getMenu = () => {
        executeClientAsyncToGroup("mech.getMenu").then((result) => {
            selectView = result;
            isLoad = true;
        });
    }

    getMenu ();

    import { addListernEvent } from 'api/functions';
    addListernEvent("phone.mech.getMenu", getMenu);

</script>

{#if !isLoad}
    <Loader />
{:else}
    <div class="newphone__maps mech" bind:this={mainElement} in:fade>
        <Header />

        {#if position && elementHeight}
            <div class="newphone__maps_box" style={elementHeight ? `height: ${elementHeight}px`: ""}>
                <Map getPosition={[position.x, position.y]} {elementWidth} {elementHeight} />
            </div>
        {/if}

        <div class="newphone__maps_content">
            <div class="box-flex">
                <div class="newphone__maps_headerimage mech"></div>
                <div class="newphone__maps_headertitleGPS newphone__shadow"><span class="mechcolor">Red</span>Age Mechanic</div>
            </div>
        </div>

        <svelte:component this={views[selectView]} {position} {onSelectView} {mainElement} {setOtherElement} {setPosition} {updateHeightMap} {closeMenu} />
    </div>
{/if}