
<script>
    import './main.sass'
    export let visible = false;
    export let opacity;
</script>
<div id="viewcontainer" class:active={!visible} style="opacity: {opacity}">
    <slot />
</div>