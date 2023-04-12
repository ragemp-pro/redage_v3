<script>
    export let data;
</script>


{#if data.title}
    <div class="popup__newhud_subtitle">
        {data.title}
    </div>
{/if}
{#if data.textarea}
    <textarea value={!data.value ? "" : data.value} on:input class="popup__newhud_textarea" placeholder={data.placeholder} maxLength={!data.maxLength ? 100 : data.maxLength} />
{:else}
    <input type={!data.type ? "text" : data.type} value={!data.value ? "" : data.value} on:input class="popup__newhud_input" placeholder={data.placeholder} maxLength={!data.maxLength ? 20 : data.maxLength} min={data.min} max={data.max} />
{/if}


