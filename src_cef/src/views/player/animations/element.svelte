<script>
    export let title;
    export let isEnterAnim;
    export let use;
    export let isFavorite;
    export let isBind = false;
    export let onDell;
    export let addFavorite;
    export let dellFavorite;
    export let onPlayAnimation;

    let currentTime;
    let duration;
    
    $: {
        if (isEnterAnim) {
            currentTime = 0;
            duration = 0;            
        }
    }
</script>

<div class="animations__grid_element" class:isBind={isBind} on:mousedown on:mouseenter on:mouseleave on:mouseup={() => { if (onPlayAnimation) onPlayAnimation(use) }}>
    {#if isEnterAnim}
    {#if use.length > 1}
    <video class="animations__element_img" 
        src={document.cloud + `animations/${use}.webm`}
        loop={true}
        autoplay={true}
        name="media"
        volume={0}
        bind:currentTime={currentTime}
        bind:duration={duration}>
        <track kind="captions">
    </video>
    {/if}
    {#if duration > 0}
    <div class="animations__progress">
        <div class="animations__progress_current" style="width: {currentTime * 100 / duration}%" />
    </div>
    {/if}
    {#if !isBind}
        {#if isFavorite}
        <div class="animationsicon-star animations__element_star active" on:click={(e) => dellFavorite(e, use)} />
        {:else}
        <div class="animationsicon-star animations__element_star" on:click={(e) => addFavorite(e, use)} />
        {/if}
    {:else}
    <div class="animations__dell" on:click={() => onDell(use)}>
        Удалить
    </div>
    {/if}
    {:else}
    {#if use.length > 1}
    <div class="animations__element_preview" />
    {/if}
    {#if !isBind}
        {#if isFavorite}
        <div class="animationsicon-star animations__element_star active" on:click={(e) => dellFavorite(e, use)} />
        {:else}
        <div class="animationsicon-star animations__element_star" on:click={(e) => addFavorite(e, use)} />
        {/if}
    <div class="animations__element_title">{title}</div>
    {/if}
    {/if}
</div>
