<script>
    import { translateText } from 'lang'
    import Header from './../header.svelte'
    import HomeButton from './../homebutton.svelte'
    import { currentPage } from './../../stores'
    import { charData } from 'store/chars';

    import Map from './../map/index.svelte'

    const closeMenu = () => {
        if(selectedCategory === null) {
            currentPage.set("mainmenu");
        } else if (selectedList !== null) {
            selectedList = null;
            getPosition ();
            updateHeightMap ();
        } else {
            selectedCategory = null;
            updateHeightMap ();
        }
    }


    import Categories from './categories.svelte'
    let selectedCategory = null;
    const onSelectedCategory = (index) => selectedCategory = index;


    import List from './list.svelte'
    let selectedList = null;
    const onSelectedList = (index) => selectedList = index;

    import Item from './item.svelte'

    import { executeClient, executeClientAsync, executeClientToGroup } from 'api/rage'


    let streetName = "";
    let areaName = "";
    const getStreetAndArea = (pos) => {

        executeClientAsync("getStreetName", pos).then((result) => {
            streetName = result;
        })

        executeClientAsync("getAreaName", pos).then((result) => {
            areaName = result;
        });
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

                getStreetAndArea (position);
            }
        });
    }

    getPosition ();

    const setPosition = (pos) => {
        position = pos;

        getStreetAndArea (pos);
    }

    const onDefaultPoint = (index) => {
        executeClient ("gps.pointDefault", index);
        executeClientToGroup ("close");
    }


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

    import { onMount } from 'svelte';
    onMount (() => {
        updateHeightMap ();
    });
    import { fade } from 'svelte/transition'
</script>

<div class="newphone__maps" bind:this={mainElement} in:fade>
    {#if position && elementHeight}
        <div class="newphone__maps_box" style={elementHeight ? `height: ${elementHeight}px`: ""}>
            <Map getPosition={[position.x, position.y]} {elementWidth} {elementHeight} />
        </div>
    {/if}

    <Header />

    <div class="newphone__maps_content">
        <div class="box-flex mb-10">
            <div class="phoneicons-locator newphone__maps_icon"></div>
            <div class="box-column">
                <div class="newphone__maps_headertitleGPS newphone__shadow">{streetName}</div>
                <div class="newphone__maps_headersubtitleGPS newphone__shadow">{areaName}</div>
            </div>
        </div>
        {#if selectedCategory == null}
            <div class="newphone__maps_importannt">
                {#if $charData.houseId}
                <div class="newphone__important_element" on:click={() => onDefaultPoint ("house")}>
                    <div class="phoneicons-home newphone__important_icon"></div>
                    {translateText('player2', 'Дом')}
                </div>
                {/if}
                {#if $charData.BizId}
                    <div class="newphone__important_element" on:click={() => onDefaultPoint ("biz")}>
                        <div class="phoneicons-fraction newphone__important_icon"></div>
                        {translateText('player2', 'Бизнес')}
                    </div>
                {/if}
                {#if $charData.FractionID > 0}
                <div class="newphone__important_element" on:click={() => onDefaultPoint ("frac")}>
                    <div class="phoneicons-jobs newphone__important_icon"></div>
                    {translateText('player2', 'Фракция')}
                </div>
                {/if}
                {#if $charData.OrganizationID > 0}
                <div class="newphone__important_element" on:click={() => onDefaultPoint ("org")}>
                    <div class="phoneicons-fraction newphone__important_icon"></div>
                    {translateText('player2', 'Организация')}
                </div>
                {/if}
            </div>
        {/if}
    </div>
    <div class="newphone__maps_categories" bind:this={otherElement}>
        <div class="newphone__maps_subcategories">
            {#if selectedCategory == null && selectedList == null}
                <Categories {onSelectedCategory} {closeMenu} {updateHeightMap} />
            {:else if selectedList == null}
                <List {onSelectedList} {selectedCategory} {closeMenu} {updateHeightMap} />
            {:else}
                <Item {setPosition} {selectedCategory} {selectedList} {closeMenu} {updateHeightMap} />
            {/if}
        </div>
        <HomeButton />
    </div>
</div>