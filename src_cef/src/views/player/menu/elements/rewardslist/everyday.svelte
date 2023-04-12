<script>
    import { translateText } from 'lang'
    import Days from './elements/days.svelte';
    export let onSetLoad;
    import { executeClient } from "api/rage";
    executeClient ("client.rewardslist.day.load");

    let rewardList = [];

    onSetLoad (true);
    const updateLoad = (json) => {
        onSetLoad (false);

        rewardList = JSON.parse(json)
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("rewardList.day.init", updateLoad);

    const onTake = (item) => {
        executeClient ("client.rewardslist.day.take", item.day);
    }
</script>


<div class="rewards">
    <div class="rewards__title">{translateText('player1', 'Ежедневные награды')}</div>
    <div class="rewards__subtitle">{translateText('player1', 'На этой странице можно каждый день забирать подарки за время, отыгранное на сервере. Как же здорово!')}</div>
    <div class="rewards__elements">
        {#each rewardList as item}
            <Days {item} on:click={() => onTake (item)}/>
        {/each}
    </div>
</div>