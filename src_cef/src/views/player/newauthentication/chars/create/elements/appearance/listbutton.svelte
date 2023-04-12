<script>
    import { gender } from 'store/customization';

    import appearances from './appearances.js';

    export let key = "";
    export let active = false;
    export let id = 0;
    export let onChange;

    const OnChangeAppearance = (change) => {
        if (onChange)
            onChange(change);
    }

    const handleKeyUp = event => {
        if (!active) return;
        const { keyCode } = event;
        switch (keyCode) {
            case 37: // left
                OnChangeAppearance(-1);
                break;
            case 39: // right
                OnChangeAppearance(1);
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div class="auth__box-arrows" on:click>
    <span class="auth-arrow-left auth__customization_icon" on:click={() => OnChangeAppearance(-1)} />
    <div class="auth__customization_text">{Array.isArray(appearances[key]) ? appearances[key][id].name : appearances[key][$gender][id].name}</div>
    <span class="auth-arrow-right auth__customization_icon" on:click={() => OnChangeAppearance(1)} />
</div>