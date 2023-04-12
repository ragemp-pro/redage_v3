<script>
    import { translateText } from 'lang'
    import { getPngUrl, getTimeFromMins } from './data'
    import { getPngToItemId } from '@/views/player/menu/elements/inventory/getPng.js'
    export let item;
    
    let intervalId = null;
    if (!item.status) {
        intervalId = setInterval(() => {
            if (--item.time <= 0)
                onDestroyInterval ();
        }, 1000);
    }

    const onDestroyInterval = () => {
        if (intervalId !== null)
            clearInterval(intervalId);

        intervalId = null;
    }

    import { onDestroy } from 'svelte';
    onDestroy(onDestroyInterval)

    const getProgress = (value, max) => {
        let progress = value / max * 100;

        if (progress > 100)
            progress = 100;

        return progress;
    }

</script>

<div class="fractionsreward__element">
    <div class="box-column">
        <!-- <div class="fractionsreward__element_title">{item.name}</div> -->
        <div class="fractionsreward__element_subtitle">{item.desc}</div>
    </div>
    <div class="fractionsreward__element_box">
        {#each item.awards as award}
            <div class="fractionsreward__element_reward">
                <span class="tooltiptext">{getPngToItemId ({
                    Name: "",
                    Png: "",
                    Type: award.type,
                    ItemId: award.itemId,
                    Data: award.data,

                }).name}</span>
                <div class="fractionsreward__element_img" style="background-image: url({getPngToItemId ({
                    Name: "",
                    Png: "",
                    Type: award.type,
                    ItemId: award.itemId,
                    Data: award.data,

                }).png})"></div>
            </div>
        {/each}
    </div>
    <div class="fractionsreward__element_progress">
        <div class="fractionsreward__progress_count">{item.count}/{item.maxCount}</div>
        <div class="fractionsreward__progressbar">
            <div class="fractionsreward__progress" style="width: {getProgress(item.count, item.maxCount)}%"></div>
        </div>
    </div>
    {#if item.status}
        <div class="fractionsreward__element_button taked">
            {translateText('player1', 'Выполнено')}
        </div>
    {:else}
        <div class="fractionsreward__element_button time">
            &#128338; {getTimeFromMins (item.time)}
        </div>
    {/if}
</div>