<script>
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'


    import List from './list.svelte'
    import Item from './item.svelte'

    let selectedView = "List"
    let views = {
        List,
        Item
    }

    let selectIndex = null;
    const onSelectIndex = (index) => {
        selectIndex = index;

        if (selectIndex === null)
            selectedView = "List"
        else
            selectedView = "Item"
    }

    import Loader from './../loader.svelte'

    let isLoad = false;
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("forbes.load");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phone.forbes.load", updateLoad)
    import { fade } from 'svelte/transition'
</script>

{#if !isLoad}
    <Loader />
{:else}
<div class="newphone__forbes" in:fade>
    <Header />
    <div class="newphone__forbes_content">
        <div class="newphone__forbes_headerimage"></div>

        <svelte:component this={views[selectedView]} {onSelectIndex} {selectIndex} />
    </div>
    <HomeButton />
</div>
{/if}