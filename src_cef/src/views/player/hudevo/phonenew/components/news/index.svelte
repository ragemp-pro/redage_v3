<script>
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'


    import Loader from './../loader.svelte'

    import Add from './add/index.svelte';
    import List from './list/index.svelte';

    const views = {
        Add,
        List
    }

    let selectedView = "List"

    const onSelectedView = (view) => selectedView = view;

    let isLoad = false;
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("loadNews");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phoneNewsLoad", updateLoad)
    import { fade } from 'svelte/transition'

</script>


{#if !isLoad}
    <Loader />
{:else}
    <div class="newphone__rent" in:fade>
        <Header />

        <div class="newphone__rent_content">
            <div class="box-flex newphone__project_padding20 p-top">
                <div class="newphone__maps_headerimage news"></div>
                <div class="newphone__maps_headertitle">Weazel<span class="red"> News</span></div>
            </div>

            <svelte:component this={views[selectedView]} {onSelectedView} />
        </div>

        <HomeButton />
    </div>
{/if}