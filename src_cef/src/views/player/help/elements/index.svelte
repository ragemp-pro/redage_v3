<script>
    import { format } from 'api/formatter'
    import { slide, fade } from 'svelte/transition';
    export let selectDropMain;
    export let active;
    export let menu;

    const selectdropMain = (id) => {
        selectDropMain(id);
    }

</script>

<div class="scroll">
    <div class="content" in:fade="{{ duration: 500 }}" out:fade="{{ duration: 0 }}">
        <ul class="accordionContent">
        {#each menu[active.mainID].items as item, index}
            <li>
                <div class="list" class:active={index === active.dropID} on:click={() => selectDropMain(index, active.mainID)}>{item.name} <span class="icon arrow" class:active={index === active.dropID}/></div>
                {#if active.dropID== index}
                    <div in:slide|local out:slide|local="{{ duration: 150 }}" class="info">
                        <svelte:component this={item.component} />
                    </div>
                {/if}
            </li>
        {/each}
        </ul>
    </div>
</div>