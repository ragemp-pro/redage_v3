<script>
    import { gender } from 'store/customization';
    import clothes from './clothes.js';


    export let id = 0;
    export let key = "";
    export let active = false;
    export let onChange;

    const OnChangeCloth = (change) => {
        if (onChange)
            onChange(change);
    }

    const handleKeyUp = event => {
        if (!active) return;
        const { keyCode } = event;
        switch (keyCode) {
            case 37: // left
                OnChangeCloth(-1);
                break;
            case 39: // right
                OnChangeCloth(1);
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div class="auth__box-arrows" on:click>
    <span class="auth-arrow-left auth__customization_icon" on:click={() => OnChangeCloth(-1)} />
    <div class="auth__customization_text">{clothes[$gender][key][id].name}</div>
    <span class="auth-arrow-right auth__customization_icon" on:click={() => OnChangeCloth(1)} />
</div>