<script>

    import mapImage from './gta_map_8k.jpg'

    let isDown;

    let posXToMap,
        posYToMap;


    let m,
        g,
        offsetX,
        offsetY;
    const handleGlobalMouseMove = event => {
        if (isDown) {
            if (0 == ++m % 5) {
                offsetX = event.offsetX;
                offsetY = event.offsetY;
            }
            //if (7 > ++g) return;
            //g = 0;

            posXToMap = Math.max(0, Math.min(posXToMap - .05 * (event.offsetX - offsetX), 7592));
            posYToMap = Math.max(0, Math.min(posYToMap - .05 * (event.offsetY - offsetY), 7592));
        } else {

            offsetX = event.offsetX;
            offsetY = event.offsetY;
        }
    }

    const handleGlobalMouseUp = event => {

        isDown = false;
    }

    const handleGlobalMouseDown = event => {
        isDown = true;
    }


    //

    const getPosition = {
        x: 1305.0,
        y: 4237.0
    }

    const GetCoordsToMap = (posX, posY) => [3756 + posX / 1.51821820693, 5528 - posY / 1.51821820693];
    const GetMapPosToCoords = (posX, posY) => [1.51821820693 * (posX - 3756), -1.51821820693 * (posY - 5528)];


    ((posX, posY) => {
        const [x, y] = GetCoordsToMap(posX, posY);
        posXToMap = x - (367.5 / 2);
        posYToMap = y - (792.5 / 2);
    })(getPosition.x, getPosition.y);

    //setMapPoint(getPosition.x, getPosition.y);
</script>

<svelte:window on:mousemove={handleGlobalMouseMove} on:mouseup={handleGlobalMouseUp} on:mousedown={handleGlobalMouseDown} />

<div id="raceAppMapWrapper" style="position: relative;width: 367.5px; height: 792.5px; overflow: hidden;" on:mouseleave={handleGlobalMouseUp}>
    <div class="phoneicons-locator newphone__maps_icon"  style="font-size: 25px;color: #FB4F69;position: absolute; top: calc(50% - 2.5px); left: calc(50% - 2.5px); transform: translate(-50%, -50%); pointer-events: none;z-index: 1;"></div>
    <div id="raceAppMapContainer" style="position: relative; top: {-posYToMap}px; left: {-posXToMap}px; pointer-events: none;">
        <img src={mapImage} draggable="false" style="position: absolute; top: 0; left: 0; height: 8192px; width: 8192px; pointer-events: none;">
    </div>
</div>