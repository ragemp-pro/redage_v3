<script>
    import { fade } from 'svelte/transition';
    export let SelectCategories;
</script>
<div class="newyearstats__main" class:year_2020={new Date().getFullYear() == 2020}>
    <div class="newyearstats__button-block" on:click={() => SelectCategories("Stats")}>
        <div class="newyearstats__button">Узнать свою статистику</div>
        <div class="newyearstats__subbutton"/>
    </div>
    <div class="newyearstats__lines"/>
</div>