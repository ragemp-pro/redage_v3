<script>
    import Element from './elements/index.svelte';
    export let onSetLoad;

    let rewardList = [];

    onSetLoad (true);
    const updateLoad = (json) => {
        onSetLoad (false);

        rewardList = JSON.parse(json)
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("table.tasks", updateLoad);
</script>

<div class="fractionsrewards__elements">
    {#each rewardList as item}
        <Element {item} />
    {/each}
</div>