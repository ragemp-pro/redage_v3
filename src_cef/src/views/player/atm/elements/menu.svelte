<script>
    import { translateText } from 'lang'
    export let onSelectMain;
    export let menuItem;


</script>
<ul class="info_atm_main">
    {#each menuItem as item, index}
        <li on:click={() => onSelectMain (index)}>
            <span class={'atm_ic ' + item.icon}></span>
            <span class="info_head">{item.title}</span>
        </li>
    {/each}
</ul>

<ul class="info_atm_button">
    <li on:click={() => onSelectMain(-1)}>
        <span class="info_head">{translateText('player', 'Выйти')}</span>
    </li>
</ul>