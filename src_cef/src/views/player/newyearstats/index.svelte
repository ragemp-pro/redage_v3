<script>
    import './assets/main.sass';

    import Main from './elements/main.svelte';
    import Stats from './elements/stats.svelte';

    const SelectCategories = (index) => {
        Select = index;
    }

    let Views = {
        Main,
        Stats
    }

    let Select = "Main";


    const handleKeyDown = (event) => {
        window.router.updateStatic("PlayerGameMenu");
    }
</script>


<svelte:window on:keyup={handleKeyDown}/>
<div id="newyearstats">
    <svelte:component this={Views[Select]} {SelectCategories} /> 
</div>