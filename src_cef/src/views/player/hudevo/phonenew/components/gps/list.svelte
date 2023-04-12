<script>
    import { translateText } from 'lang'
    export let updateHeightMap;

    export let onSelectedList;
    export let closeMenu;

    import { executeClientAsyncToGroup } from 'api/rage'

    let data = []

    export let selectedCategory;

    executeClientAsyncToGroup("gps.getList", selectedCategory).then((result) => {
        if (result && typeof result === "string")
            data = JSON.parse(result);

        updateHeightMap ();
    });

    //data = JSON.parse(`{"name":"Р“РѕСЃ. СЃС‚СЂСѓРєС‚СѓСЂС‹","icon":"gos","content":[{"name":"City Hall","pos":{"x":-1290.6262,"y":-571.5192,"z":0},"dist":6406},{"name":"LSPD","pos":{"x":479.0797,"y":-996.76697,"z":0},"dist":6572},{"name":"EMS","pos":{"x":297.672,"y":-583.996,"z":0},"dist":6166},{"name":"FIB","pos":{"x":2469.7378,"y":-384.15598,"z":0},"dist":6280},{"name":"NEWS","pos":{"x":-577.5707,"y":-922.6645,"z":0},"dist":6587},{"name":"SHERIFF 1","pos":{"x":1846.0203,"y":3692.7175,"z":0},"dist":2405},{"name":"SHERIFF 2","pos":{"x":-445.67395,"y":6013.7183,"z":0},"dist":1318}]}`);
    import { fade } from 'svelte/transition'


    const subList = (index) => {
        executeClientAsyncToGroup("gps.getSubList", {
            index: selectedCategory,
            id: index
        }).then((result) => {
            if (result && typeof result === "string")
                data = JSON.parse(result);

            updateHeightMap ();
        });
    }
</script>


<div class="box-between newphone__project_padding20" in:fade>
    <div class="newphone__maps_header">{data.name}</div>
    <div class="phoneicons-add1" on:click={closeMenu}></div>
</div>

<div class="newphone__maps_list" in:fade>
    {#if data.content}
        {#each data.content as item, index}
            {#if item.isSub}
                <div class="newphone__maps_selected" on:click={() => subList (index)}>
                    <div class="newphone__maps_square">
                        <div class="phoneicons-{data.icon} newphone__maps_elementicon"></div>
                    </div>
                    <div class="box-column">
                        <div class="newphone__maps_headertitle">{item.name}</div>
                        <div class="newphone__maps_headersubtitle">{translateText('player2', 'Открыть')}</div>
                    </div>
                    <div class="phoneicons-Button newphone__chevron-back"></div>
                </div>
            {:else}
                <div class="newphone__maps_selected" on:click={() => onSelectedList (index)}>
                    <div class="newphone__maps_square">
                        <div class="phoneicons-{data.icon} newphone__maps_elementicon"></div>
                    </div>
                    <div class="box-column">
                        <div class="newphone__maps_headertitle">{item.name}</div>
                        <div class="newphone__maps_headersubtitle">{item.dist} м.</div>
                    </div>
                    <div class="phoneicons-Button newphone__chevron-back"></div>
                </div>
            {/if}
        {/each}
    {/if}
</div>