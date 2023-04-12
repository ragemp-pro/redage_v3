<script>
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'


    import List from './list.svelte'
    import Profile from './profile.svelte'
    import Matches from './matches.svelte'

    let selectedView = "Profile"
    let views = {
        List,
        Profile,
        Matches
    }

    import Loader from './../loader.svelte'

    let isCreate = false;
    let isLoad = false;
    const updateLoad = (toggled) => {
        isCreate = toggled;
        isLoad = true;

        if (isCreate)
            selectedView = "List";
    }

    const setLoad = () => isLoad = false;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("tinder.load");

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phone.tinder.load", updateLoad)
    import { fade } from 'svelte/transition'

    import './style.css'
</script>

{#if !isLoad}
    <Loader />
{:else}
<div class="newphone__forbes" in:fade>
    <Header />
    <div class="newphone__forbes_content">
        <div class="box-flex newphone__project_padding16 mb-10">
            <div class="newphone__maps_headerimage tinderimg"></div>
            <div class="newphone__maps_headertitleGPS">Tinder</div>
        </div>
        <div class="box-between newphone__project_padding16 mb-10">
            {#if isCreate}
                <span class="tindericons-users tinder__button" on:click={() => selectedView = "List"}></span>
            {:else}
                <span/>
            {/if}
            <div class="box-flex">
                {#if isCreate}<span class="tindericons-heart tinder__button mr-6" on:click={() => selectedView = "Matches"}></span>{/if}
                <span class="tindericons-person tinder__button" on:click={() => selectedView = "Profile"}></span>
            </div>
        </div>
        <svelte:component this={views[selectedView]} {isCreate} {setLoad} />
    </div>
    <HomeButton />
</div>
{/if}