<script>
    import { translateText } from 'lang'
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

    executeClientToGroup ("mechjob.load");

    let isLoad = false;

    let isSelect = false;
    let selectMech = {};
    const getData = () => {
        executeClientAsyncToGroup("mechjob.getSelect").then((result) => {
            if (result && typeof result === "string") {
                selectMech = JSON.parse(result);
                isSelect = selectMech && selectMech.name;

                if (isSelect)
                    setPosition (selectMech.pos);
                else
                    getPosition ();

                setOtherElement (otherElement);
            }
        });

        isLoad = true;
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("phone.mechjob.load", getData);

    import Select from './select.svelte'

    import List from './list.svelte'


    let otherElement;

</script>

{#if !isLoad}
    <LoaderSmall />
{:else}
    <div class="newphone__maps_categories" style="background: linear-gradient(90deg, #676869 0%, #141414 94.41%);" in:fade bind:this={otherElement} use:setOtherElement>
        {#if isSelect}
            <div class="newphone__maps_price">
                <div class="box-column">
                    <div class="newphone__maps_pricetitle">{translateText('player2', 'Клиент')}:</div>
                    <div class="newphone__maps_pricesubtitle">{selectMech.name}</div>
                </div>
                <div class="newphone__maps_car mech"></div>
            </div>
        {/if}

        <div class="newphone__maps_subcategories">
            {#if isSelect}
                <Select {selectMech} {closeMenu} />
            {:else}
                <List {closeMenu} />
            {/if}
            <HomeButton />
        </div>
    </div>
{/if}