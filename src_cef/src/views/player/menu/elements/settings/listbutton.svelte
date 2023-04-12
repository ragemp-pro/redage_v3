<script>

    export let active = false;
    export let name = "";
    export let onChange;

    const OnChangUse = (change) => {
        if (onChange)
            onChange(change);
    }

    const handleKeyUp = event => {
        if (!active) return;
        const { keyCode } = event;
        switch (keyCode) {
            case 37: // left
                OnChangUse(-1);
                break;
            case 39: // right
                OnChangUse(1);
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />
<div class="list__element" on:click>
    <i class="list__element__left" on:click={() => OnChangUse(-1)} />
    <div class="list__title">{name}</div>
    <i class="list__element__right" on:click={() => OnChangUse(1)} />
</div>