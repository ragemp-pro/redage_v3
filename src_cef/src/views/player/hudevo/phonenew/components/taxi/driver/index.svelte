<script>
    import Header from './../../header.svelte'
    import HomeButton from './../../homebutton.svelte'
    import { categoriesList, currentPage } from './../../../stores'
    import { format } from "api/formatter";
    import {executeClientToGroup, executeClientAsyncToGroup, executeClientAsync} from "api/rage";

    import { fade } from 'svelte/transition'

    export let getPosition;
    export let setPosition;
    export let setOtherElement;

    function closeMenu () {
        currentPage.set("mainmenu")
    }


    import LoaderSmall from './../../loadersmall.svelte'

    executeClientToGroup ("taxijob.load");

    let isLoad = false;

    let isSelect = false;
    let selectTaxi = {};
    const getData = () => {
        executeClientAsyncToGroup("taxijob.getSelect").then((result) => {
            if (result && typeof result === "string") {
                selectTaxi = JSON.parse(result);
                isSelect = selectTaxi && selectTaxi.name;

                if (isSelect)
                    setPosition (selectTaxi.pos);
                else
                    getPosition ();

                setOtherElement (otherElement);
            }
        });

        isLoad = true;
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("phone.taxijob.load", getData);

    import Select from './select.svelte'

    import List from './list.svelte'


    let otherElement;

</script>

{#if !isLoad}
    <LoaderSmall />
{:else}
    <div class="newphone__maps_categories" style="background: linear-gradient(90deg, #FF8A00 0%, #D14E04 94.41%)" in:fade bind:this={otherElement} use:setOtherElement>
        {#if isSelect}
            <div class="newphone__maps_price">
                <div class="box-column">
                    <div class="newphone__maps_pricetitle">Клиент:</div>
                    <div class="newphone__maps_pricesubtitle">{selectTaxi.name}</div>
                </div>
                <div class="newphone__maps_car taxi"></div>
            </div>
        {/if}

        <div class="newphone__maps_subcategories">
            {#if isSelect}
                <Select {selectTaxi} {closeMenu} />
            {:else}
                <List {closeMenu} />
            {/if}
            <HomeButton />
        </div>
    </div>
{/if}