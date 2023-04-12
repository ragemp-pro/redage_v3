
<script>
    import mapImage from './map.jpg'

    import { loadImage, loadAwaitImage } from 'api/functions'

    export let getPosition = false;

    export let elementWidth = 0;
    export let elementHeight = 0;

    let posXToMap,
        posYToMap;

    const GetCoordsToMap = (posX, posY) => [3756 + posX / 1.51821820693, 5528 - posY / 1.51821820693];
    const GetMapPosToCoords = (posX, posY) => [1.51821820693 * (posX - 3756), -1.51821820693 * (posY - 5528)];

    const setMapCoords = (posX, posY) => {
        if (elementWidth && elementHeight) {
            const [x, y] = GetCoordsToMap(posX, posY);
            posXToMap = x - (elementWidth / 2);
            posYToMap = y - (elementHeight / 2);
        }
    }

    $: if (elementHeight)
        setMapCoords (getPosition[0], getPosition[1]);

    $: if (getPosition)
        setMapCoords (getPosition[0], getPosition[1]);

    import { fade } from 'svelte/transition'
</script>

{#if getPosition}
    <div class="newphone__maps_wrapper" in:fade>
        <div class="phoneicons-locator newphone__maps_icon" />

        <div class="newphone__maps_container" style="top: {-posYToMap}px; left: {-posXToMap}px;">
            {#await loadAwaitImage(mapImage) then _}
                <div class="newphone__maps_image" style="background-image: url({mapImage})"/>
            {/await}
        </div>
    </div>
{/if}