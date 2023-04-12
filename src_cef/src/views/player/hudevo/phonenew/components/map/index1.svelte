
<script>

    import mapImage from './map.jpg'

    import { loadImage } from 'api/functions'

    export let getPosition = false;

    export let mainElement = false;
    export let otherElement = false;

    let posXToMap,
        posYToMap;

    let elementWidth,
        elementHeight;

    const GetCoordsToMap = (posX, posY) => [3756 + posX / 1.51821820693, 5528 - posY / 1.51821820693];
    const GetMapPosToCoords = (posX, posY) => [1.51821820693 * (posX - 3756), -1.51821820693 * (posY - 5528)];

    const setMapCoords = (posX, posY) => {
        if (elementWidth && elementHeight) {
            const [x, y] = GetCoordsToMap(posX, posY);
            posXToMap = x - (elementWidth / 2);
            posYToMap = y - (elementHeight / 2);
        }
    }

    let height = 0;

    $: if (elementWidth)
        setMapCoords (getPosition[0], getPosition[1]);

    $: if (!height && elementHeight)
        setMapCoords (getPosition[0], getPosition[1]);

    if (mainHeight) {

        $: if (getPosition)
            setMapCoords (getPosition[0], getPosition[1]);

        const updateHeight = () => {
            height = mainHeight - otherHeight;
            height += height * 0.35;

            elementHeight = height;

            setMapCoords (getPosition[0], getPosition[1]);
        }

        $: if (mainHeight)
            updateHeight();

        $: if (otherHeight)
            updateHeight();
    }

    import { fade } from 'svelte/transition'
</script>

{#if getPosition}
    <div class="newphone__maps_wrapper" style={height ? `height: ${height}px`: ""} in:fade>
        <div class="phoneicons-locator newphone__maps_icon" />

        <div class="newphone__maps_container" style="top: {-posYToMap}px; left: {-posXToMap}px;">
            <div class="newphone__maps_image" use:loadImage={mapImage} />
        </div>
    </div>

{/if}