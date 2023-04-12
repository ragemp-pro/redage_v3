<script>
    import { translateText } from 'lang'
    import Donate from './elements/donate.svelte';
    export let onSetLoad;
    import { executeClient } from "api/rage";
    executeClient ("client.rewardslist.donate.load");

    let rewardList = [];

    onSetLoad (true);
    const updateLoad = (json) => {
        onSetLoad (false);

        rewardList = JSON.parse(json)
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent("rewardList.donate.init", updateLoad);

    const onTake = (item) => {
        executeClient ("client.rewardslist.donate.take", item.index);
    }
</script>


<div class="rewards">
    <div class="rewards__title">{translateText('player1', 'Бонусы за пополнение')}</div>
    <div class="rewards__subtitle">{translateText('player1', 'Особые еженедельные бонусы за пополнение. Донатить одно удовольствие...')}</div>
    <div class="rewards__elements">
        {#each rewardList as item}
            <Donate {item} on:click={() => onTake (item)}/>
        {/each}
    </div>
</div>