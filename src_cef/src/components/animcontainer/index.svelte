<!--
<script>
	import { fade } from 'svelte/transition';
    import './main.sass'
    export let view = "";
    let useView = "";
    let visible = false;

    $: {
        if (useView !== view) {
            visible = false;
            useView = view;
            if (view !== "" && view !== false) {
                setTimeout(() => {
                    visible = true;
                }, 0)
            }
        }
    }
</script>
{#if visible}
<div id="animcontainer" in:fade={{duration: 200}}>
    <slot />
</div>
{/if}-->
<script>
    import './main.sass'
</script>
<div id="animcontainer">
    <slot />
</div>