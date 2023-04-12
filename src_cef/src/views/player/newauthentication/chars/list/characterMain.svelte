<script>
    export let charid;
    export let char;

    import CreatedByChar from './createdByChar.svelte'
    import ToCreateChar from './toCreateChar.svelte'
    import UnlockSlot from './unlockSlot.svelte'
</script>

{#if char === -1 && charid <= 2}
    <ToCreateChar {charid} />
{:else if char === -2}
    {#if charid == 2}
    <UnlockSlot {charid} />
    {/if}
{:else if char !== 0 && char && char.Data}
    <CreatedByChar {charid} {char} />
{/if}