<script>
    import { translateText } from 'lang'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'

    import Loader from './../loader.svelte'

    let isLoad = false;//false
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("loadProperty");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phoneMainPropertyLoad", updateLoad);

    import PropertyList from './list.svelte';
    import Houses from './houses/index.svelte';
    import Business from './business/index.svelte';
    import {onDestroy} from "svelte";

    const views = {
        PropertyList,
        Business,
        Houses
    }

    let selectedView = "PropertyList"

    const onSelectedView = (view = false) => selectedView = !view ? "PropertyList" : view;

    let selectedId = 0;
    const onSelectedId = (id) => selectedId = id;
    import { fade } from 'svelte/transition'
</script>

{#if !isLoad}
    <Loader />
{:else}
    <div class="newphone__rent" in:fade>
        <Header />

        <div class="newphone__rent_content">
            <div class="box-flex newphone__project_padding20 p-top">
                <div class="newphone__maps_headerimage property"></div>
                <div class="newphone__maps_headertitle"><span class="violet">{translateText('player2', 'Управление')} </span>{translateText('player2', 'имуществом')}</div>
            </div>

            <svelte:component this={views[selectedView]} {onSelectedView} {onSelectedId} {selectedId} />
        </div>

        <HomeButton />
    </div>
{/if}
