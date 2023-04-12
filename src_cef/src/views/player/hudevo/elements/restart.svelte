<script>
    import { fly } from 'svelte/transition';
    let iconData = ""
    let text = ""
    window.UpdateButtonText = (_iconData, _text) => {
        iconData = _iconData;
        text = _text;
    }
</script>
{#if text.length > 1}
    <div class="hudevo__restart" in:fly={{ x: 50, duration: 500 }} out:fly={{ x: 50, duration: 250 }}>
        <span class="{iconData} restart__icon"/>   {text}
    </div>
{/if}