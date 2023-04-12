<script>
    import { translateText } from 'lang'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'

    import Loader from './../loader.svelte'
    let isLoad = false;
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("auction.load");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("auction.load", updateLoad)
    import { fade } from 'svelte/transition'

    import Main from './main/index.svelte'
    //
    import Create from './create/index.svelte'
    import CreateList from './create/list.svelte'
    import CreateItem from './create/item.svelte'
    //
    import List from './list/index.svelte'
    import ListItem from './list/item.svelte'

    let selectedView = "Main";

    const onSelectedView = (view, _isLoad = true) => {
        selectedView = view;
        isLoad = _isLoad;
    }
    addListernEvent ("auction.view", onSelectedView)

    const views = {
        Main,
        //
        Create,
        CreateList,
        CreateItem,
        //
        List,
        ListItem
    }

    import { onDestroy } from 'svelte'

    onDestroy(() => {
        executeClientToGroup ("auction.close");
    });
</script>

{#if !isLoad}
    <Loader />
{:else}
    <div class="newphone__rent" in:fade>
        <Header />
        <div class="newphone__rent_content">
            <div class="box-flex newphone__project_padding20 p-top">
                <div class="newphone__maps_headerimage auction"></div>
                <div class="newphone__maps_headertitleGPS">{translateText('player2', 'Аукцион')}</div>
            </div>

            <svelte:component this={views[selectedView]} {onSelectedView} />

        </div>
        <HomeButton />
    </div>
{/if}