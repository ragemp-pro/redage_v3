<script>
    import { translateText } from 'lang'
    export let onSelectedCategory;
    export let closeMenu;
    export let updateHeightMap;


    import { executeClientAsyncToGroup } from 'api/rage'

    let data = []

    executeClientAsyncToGroup("gps.getRoutes").then((result) => {
        if (result && typeof result === "string")
            data = JSON.parse(result);

        updateHeightMap ();
    });


    //data = JSON.parse(`[{"name":"Р“РѕСЃ. СЃС‚СЂСѓРєС‚СѓСЂС‹","icon":"gos"},{"name":"Р‘Р°РЅРґС‹","icon":"weapons"},{"name":"РњР°С„РёРё","icon":"mafia"},{"name":"Р Р°Р±РѕС‚С‹","icon":"licenses"},{"name":"РџРѕРґСЂР°Р±РѕС‚РєР°","icon":"jobs"},{"name":"Р‘Р»РёР¶Р°Р№С€РёРµ РјРµСЃС‚Р°","icon":"recent"},{"name":"РџСЂРѕС‡РµРµ","icon":"clubs"}]`);

    import { fade } from 'svelte/transition'
</script>

<div class="box-between newphone__project_padding20" in:fade>
    <div class="newphone__maps_header">{translateText('player2', 'Категории')}</div>
    <div class="phoneicons-add1" on:click={closeMenu}></div>
</div>

<div class="newphone__maps_grid newphone__project_padding20" in:fade>
    {#each data as item, index}
        <div class="newphone__maps_element" on:click={() => onSelectedCategory (index)}>
            <div class="phoneicons-{item.icon} newphone__maps_elementicon"></div>
            <div class="newphone__maps_elementname">{item.name}</div>
        </div>
    {/each}
</div>