<script>
    import { translateText } from 'lang'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'
    import Loader from './../loader.svelte'

    import {currentPage} from '../../stores'

    
    import Car from './car.svelte'
    import List from './list.svelte'

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("cars.load");

    let isLoad = false;
    const updateLoad = () => isLoad = true;


    const Views = {
        Car,
        List
    }

    let SelectViews = "List";
    let selectedCar = "";
    const OnUpdatePage = (page, item) => {
        SelectViews = page;
        selectedCar = item;
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("phoneCarsLoad", updateLoad);

    import { fade } from 'svelte/transition'    

</script>
{#if !isLoad}
    <Loader />
{:else}
    <div class="newphone__rent" in:fade>
        <Header />
        <div class="newphone__rent_content">
            <div class="box-flex newphone__project_padding20 p-top">
                <div class="newphone__maps_headerimage rent"></div>
                <div class="newphone__maps_headertitle"><span class="orange">{translateText('player2', 'Управление')} </span>{translateText('player2', 'транспортом')}</div>
            </div>
            <svelte:component this={Views[SelectViews]} {OnUpdatePage} {selectedCar}/>
        </div>
        <HomeButton />
    </div>
{/if}