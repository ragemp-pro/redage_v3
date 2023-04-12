<script>
    import { translateText } from 'lang'
    import { addListernEvent } from 'api/functions'

    import { getPngToItemId } from '@/views/player/menu/elements/inventory/getPng.js'

    import { isPopupBuyOpened } from '../../stores.js'

    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'

    export let selectPage = 0;
    export let currentLvl = 0;

    let awardsList = [];
    const getAwards = (index) => {
        executeClientAsyncToGroup("getAwards", index).then((result) => {
            if (result && typeof result === "string") {
                const _awardsList = JSON.parse(result);
                let data = [];

                _awardsList.forEach((item) => {
                    data.push({
                        index: item.index,
                        usual: {
                            ...item.usual,
                            ...getPngToItemId (item.usual),
                        },
                        premium: {
                            ...item.premium,
                            ...getPngToItemId (item.premium),
                        },
                    })
                });

                awardsList = data;
            }
        });
    }

    $: if (selectPage >= 0)
        getAwards (selectPage);

    //

    let isPremium = 0;
    const getPremium = () => {
        executeClientAsyncToGroup("getPremium").then((result) => {
            isPremium = result;
        });
    }
    getPremium ();

    addListernEvent ("battlePassBuyPremiumSuccess", () => {
        getPremium ();
    })

    //


    const onTake = (taked, index, isPrem) => {
        if (taked)
            return;
        if (!window.loaderData.delay ("battlePass.onTake", 1))
            return;

        executeClientToGroup ("take", index, isPrem)
    }

    addListernEvent ("battlePassTakeSuccess", () => {
        getAwards (selectPage);
    })

    const getCount = (item) => {
        if (typeof item === "undefined" || !item)
            return "";

        if (item.Count == undefined)
            return "";

        let count = item.Count.toString();

        switch (item.Type) {
            case 0:
                count += translateText('player', ' шт.')
                break;
            case 1:
                count += translateText('player', ' д.')
                break;
            case 2:
                count += "$"
                break;
            case 3:
                count += " RB"
                break;
        }

        return count;
    }
</script>


<div class="battlepass__blocks">
    {#if !isPremium}
        <div class="battlepass__button battlepass__block_unpremium" on:click={() => isPopupBuyOpened.set(true)}>{translateText('player', 'РАЗБЛОКИРОВАТЬ')}</div>
    {/if}
    <div class="battlepass__blocks_block">
        <div class="battlepass__block_lvl"></div>
        <div class="battlepass__block_prise free-start">
            <div class="battlepass__prise_image free">
                <div class="battlepass__prise_name bold big">{currentLvl}</div>
            </div>
            <div class="battlepass__prise_name bold">FREE PASS</div>
        </div>
        <div class="battlepass__block_prise premium-start">
            <div class="battlepass__prise_image prime"></div>
            <div class="battlepass__prise_name bold">PREMIUM PASS</div>
        </div>
    </div>
    {#each awardsList as item, index}
        <div class="battlepass__blocks_block" class:active={currentLvl > item.index} class:dontactive={!(currentLvl > item.index)}>
            <div class="battlepass__block_lvl">{item.index + 1} {translateText('player', 'УРОВЕНЬ')}</div>
            <div class="battlepass__block_prise" class:free-start={item.usual.Type >= 0 && !item.usual.taked} class:taked={item.usual.taked}>
                {#if item.usual.Type >= 0 && currentLvl > item.index}
                    <div class="battlepass__block_status" class:donttaked={!item.usual.taked} on:click={() => onTake (item.usual.taked, item.index, false)}>
                        {item.usual.taked ? translateText('player', 'Получено') : translateText('player', 'Получить')}
                    </div>
                {/if}
                {#if item.usual.png && item.usual.png.length > 5}
                    <div class="battlepass__prise_image free" style="background-image: url({item.usual.png})" />
                {:else}
                    <div class="battlepass__prise_image free" />
                {/if}
                {#if typeof item.usual.name === "string" && item.usual.name.length > 1}
                    <div class="battlepass__prise_name">{item.usual.name}</div>
                {/if}
                <div class="battlepass__prise_count">{getCount (item.usual)}</div>
            </div>
            <div class="battlepass__block_prise" class:premium-start={item.premium.Type >= 0 && !item.premium.taked} class:dontpremium={!isPremium} class:taked={item.premium.taked}>
                {#if item.premium.Type >= 0 && currentLvl > item.index && isPremium}
                    <div class="battlepass__block_status" class:donttaked={!item.premium.taked} on:click={() => onTake (item.premium.taked, item.index, true)}>
                        {item.premium.taked ? translateText('player', 'Получено') : translateText('player', 'Получить')}
                    </div>
                {/if}
                {#if item.premium.png && item.premium.png.length > 5}
                    <div class="battlepass__prise_image prime" style="background-image: url({item.premium.png})" />
                {:else}
                    <div class="battlepass__prise_image prime" />
                {/if}
                {#if typeof item.premium.name === "string" && item.premium.name.length > 1}
                    <div class="battlepass__prise_name">{item.premium.name}</div>
                {/if}
                <div class="battlepass__prise_count">{getCount (item.premium)}</div>
            </div>
        </div>
    {/each}
</div>