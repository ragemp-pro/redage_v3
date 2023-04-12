<script>
    import './main.sass';
    
    export let isLine = true;
    
    export let key = "";
    export let x = 0;
    export let y = 0;

    export let xLeftName = "";
    export let xRightName = "";
    export let yTopName = "";
    export let yBottomName = "";

    export let onChange;

    let use = false;

    const onMouseDown = (e) => {
        if(e.which === 1) {
            use = true;
        }
    }

    const onMouseUp = (e) => {
        use = false;
    }

    let gridElement;
    let gridWidth;
    let gridHeight;
    let pointElement;
    let pointWidth;
    let pointHeight;

    let fixToPoint = 0;
    $: if (gridElement && pointElement) {
        const rect = gridElement.getBoundingClientRect();
        gridWidth = rect.width;
        gridHeight = rect.height;

        const point = pointElement.getBoundingClientRect();
        pointWidth = point.width - 1;
        pointHeight = point.height - 1;

        fixToPoint = (rect.width - pointWidth) / 2;
    }

    const GetTop = () => {

    }

    const GetLeft = () => {

    }

    const OnMouseMove = (event) => {
        if (use) {
            const rect = gridElement.getBoundingClientRect();
            
            let x = event.clientX - rect.left;
            let y = event.clientY - rect.top;

            if (!isLine) {
                if (x < (pointWidth / 2)) x = (pointWidth / 2);
                else if(x > (gridWidth - (pointWidth - 1))) x = (gridWidth - (pointWidth - 1)) + (pointWidth / 2);
                
                if(y < (pointHeight / 2)) y = (pointHeight / 2);
                else if(y > (gridHeight - (pointHeight - 1))) y = (gridHeight - (pointHeight - 1)) + (pointHeight / 2);

                x = (x - fixToPoint) / fixToPoint;
                y = (y - fixToPoint) / fixToPoint;
                
                if (onChange)
                    onChange(key, x, y);
            } else {
                if (x < (gridWidth - (pointWidth - 1)) && x >= 0) {
                    x = (x - fixToPoint) / fixToPoint;
                    if (onChange)
                        onChange(key, x);
                }
            }
        }
    }
</script>

<svelte:window on:mouseup={onMouseUp} on:mousemove={OnMouseMove}/>

<div class="auth__sliders">
    {#if !isLine}
    <div class="auth__square_range" on:mousedown={onMouseDown} bind:this={gridElement}>
        <div class="slider" style={"left:" + (x * fixToPoint + (fixToPoint * 0.9)) + "px;top:" + (y * fixToPoint + (fixToPoint * 0.9)) + "px" } bind:this={pointElement} />
        <div class="top">{yTopName}</div>
        <div class="left">{xLeftName}</div>
        <div class="right">{xRightName}</div>
        <div class="bottom">{yBottomName}</div>
    </div>
    {:else}
    <div class="auth__slider_text slider-left">{xLeftName}</div>
    <div class="auth__long_range" on:mousedown={onMouseDown} bind:this={gridElement}>
        <div class="slider" style={"left:" + (x * fixToPoint + (fixToPoint)) + "px;top:" + (gridHeight - pointHeight) / 2 + "px" } bind:this={pointElement} />
    </div>
    <div class="auth__slider_text slider-right">{xRightName}</div>
    {/if}
</div>