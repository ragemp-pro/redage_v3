<script>
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'

    import Main from './main.svelte'
    import SoundList from './soundList.svelte'
    import SmsList from './soundSms.svelte'
    import Wallpaper from './wallpaper.svelte'
    const views = {
        Wallpaper,
        SoundList,
        SmsList,
    };

    let selectedView = null;

    import { fade } from 'svelte/transition'


    const onSelectedView = (view) => {
        selectedView = view;
    }
</script>
<div class="newphone__settings" in:fade>
    <Header />
    {#if selectedView == null}
        <Main {onSelectedView} />
    {:else}
        <svelte:component this={views[selectedView]} {onSelectedView} />
    {/if}
    <HomeButton />
</div>