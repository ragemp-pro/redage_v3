
<script>
    import './main.sass'
    export let visible = false;
    export let opacity;
</script>
<div id="popuscontainer" class:active={visible} style="opacity: {opacity}">
    <slot />
</div>