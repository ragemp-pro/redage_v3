<script>
    import { translateText } from 'lang'
    import './main.sass'
    import './fonts/style.css'
    import Item from './item.svelte'
    import { format } from 'api/formatter';
    import Color from './color.svelte'
    import { executeClient } from 'api/rage'
    import { charMoney } from 'store/chars'

    let
        color = false,
        colorListsId = 0, // 0 - default, 1 - (0 - 159), 2 - (-1 - 12 Headlight Colors)
        category = [],
        selectCategory = -1,
        BackCategory = false,
        lists = [],
        mstats_speed = 100,
        mstats_brakes = 100,
        mstats_boost = 100,
        mstats_clutch = 100,
        stats_speed = 12,
        stats_brakes = 52,
        stats_boost = 50,
        stats_clutch = 50,
        selectItem = -1
    window.events.addEvent("cef.custom.color", (toggled, type) => {
        color = toggled;
        colorListsId = type;
    });

    window.events.addEvent("cef.custom.categories", (_category) => {
        document.querySelector("ul.items").scrollTop = 0;
        category = JSON.parse (_category);
        selectCategory = -1;
        color = false;
        colorListsId = 0;
        lists = [];
        selectItem = -1;
    });

    window.events.addEvent("cef.custom.vehicleMaxStats", (_stats_speed, _stats_brakes, _stats_boost, _stats_clutch) => {
        mstats_speed = _stats_speed;
        mstats_brakes = _stats_brakes;
        mstats_boost = _stats_boost;
        mstats_clutch = _stats_clutch;
    });

    window.events.addEvent("cef.custom.vehicleStats", (_stats_speed, _stats_brakes, _stats_boost, _stats_clutch) => {
        stats_speed = _stats_speed;
        stats_brakes = _stats_brakes;
        stats_boost = _stats_boost;
        stats_clutch = _stats_clutch;
    });
    
    window.events.addEvent("cef.custom.lists", (_lists) => {
        lists = JSON.parse (_lists);
        selectItem = -1;
        setTimeout(() => {                
            document.querySelector("ul.items.select").scrollTop = 0;
        }, 0)
    });
    import { onDestroy } from 'svelte'
    onDestroy(() => {

        window.events.removeEvent("cef.custom.color", (toggled, type) => {
            color = toggled;
            colorListsId = type;
        });

        window.events.removeEvent("cef.custom.categories", (_category) => {
            document.querySelector("ul.items").scrollTop = 0;
            category = JSON.parse (_category);
            selectCategory = -1;
            color = false;
            colorListsId = 0;
            lists = [];
            selectItem = -1;
        });

        window.events.removeEvent("cef.custom.vehicleMaxStats", (_stats_speed, _stats_brakes, _stats_boost, _stats_clutch) => {
            mstats_speed = _stats_speed;
            mstats_brakes = _stats_brakes;
            mstats_boost = _stats_boost;
            mstats_clutch = _stats_clutch;
        });

        window.events.removeEvent("cef.custom.vehicleStats", (_stats_speed, _stats_brakes, _stats_boost, _stats_clutch) => {
            stats_speed = _stats_speed;
            stats_brakes = _stats_brakes;
            stats_boost = _stats_boost;
            stats_clutch = _stats_clutch;
        });
        
        window.events.removeEvent("cef.custom.lists", (_lists) => {
            lists = JSON.parse (_lists);
            selectItem = -1;
            setTimeout(() => {
                document.querySelector("ul.items.select").scrollTop = 0;
            }, 0)
        });
    });

    const GetSpec = (num, max) => {
        
        let step;
        let array = [];

        let percentArray = [];
        for (step = 1; step <= 10; step++) {
            let progress = 0;
            if (num >= (step * 10)) progress = 100;
            else if (num < (step * 10) && num >= (10 * (step - 1))) progress = num - (((step - 1) * 10) * 100 / max);
            percentArray.push (progress);
        }
        
        percentArray.forEach((step, index) => {
            array.push (`<li class="sort" style="background: linear-gradient(to right, #FFFFFF ${step}%, #434A5B 0%)"></li>`)
        })    
        return percentArray
    }

    const onSelectCategory = (index) => {
        selectCategory = index;
        lists = [];
        color = false;
        colorListsId = false;
        executeClient ('client.custom.category', category[index].category);
    }

    const onSelectItem = (index) => {
        selectItem = index;
    }
</script>
<div id="lscustoms">
    <div class="leftSide">
        <div class="box-flex">
            <div class="categoryBlock" class:min={(lists.length > 0 && category[selectCategory] !== undefined)}>
                <div class="box-header" style="flex-direction: column">
                    <div class="img-logo" />
                    <div class="flex">
                        <div class="desc">
                            <span class="icon-money cash" />
                            {#if !(lists.length > 0 && category[selectCategory] !== undefined)}
                                {translateText('vehicle', 'Баланс')}:
                            {/if}
                            
                            <span class="money">${format("money", $charMoney)}</span>
                        </div>
                    </div>
                </div>

                <ul class="items" on:mouseenter={() => executeClient("client.camera.toggled", false)} on:mouseleave={() => executeClient("client.camera.toggled", true)}>
                    {#each category as item, index}
                    <li class="listitems" class:active={selectCategory === index} on:click={() => onSelectCategory (index)}>
                        {#if (lists.length > 0 && category[selectCategory] !== undefined) && typeof item.category == "number"}
                            <div class="listitems-title">
                                {item.title}
                            </div>
                        {:else}
                        <i class="icon ilsc-{item.category}" />
                        {/if}                                        
                        <div class="flex">
                            <div class="title">
                                {item.title}
                            </div>
                            <div class="desc">
                                {item.desc}
                            </div>
                        </div>
                    </li>
                    {/each}
                </ul>
            </div>
            {#if (lists.length > 0 && category[selectCategory] !== undefined)}
            <div class="categoryBlock popups">
                <div class="box-header">
                    <i class={'icon ilsc-' + category[selectCategory].category} />
                    <div class="flex">
                        <div class="title">
                            {category[selectCategory].title}
                        </div>
                    </div>
                </div>
                <ul class="items select" on:mouseenter={() => executeClient("client.camera.toggled", false)} on:mouseleave={() => executeClient("client.camera.toggled", true)}>
                    {#each lists as item, index}
                        <Item
                            id={item.index}
                            icon={category[selectCategory].category}
                            text={item.name}
                            price={item.price}
                            onSelectItem={onSelectItem}
                            selectItem={selectItem} />
                    {/each}
                </ul>
            </div>
            {/if}
        </div>
        <div class="box-button">
            <div class="button" on:click={() => executeClient("client.custom.exit")}>
                {translateText('vehicle', 'Выйти')}
            </div>
        </div>

    </div>
    <div class="centerSide">
        <div class="buttonInfo">
            <div class="btn">ESC</div>
            <div class="text">{translateText('vehicle', 'Выйти/Назад')}</div>
        </div>
        <div class="buttonInfo">
            <div class="btn"><i class="range"/></div>
        </div>
    </div>
    <div class="rightSide">
        <div class="spec">
            <div class="head">{translateText('vehicle', 'Характеристики')}</div>
            <div class="lb">
                <div class="title"><i class="ic1"/>{translateText('vehicle', 'Скорость')}</div>
                <ul class="specProc">
                    {#each GetSpec(stats_speed, mstats_speed) as step, _} 
                        <li class="sort" style="background: linear-gradient(to right, #FFFFFF {step}%, #434A5B 0%)" />
                    {/each}
                </ul>
            </div>
            <div class="lb">
                <div class="title"><i class="ic2"/>{translateText('vehicle', 'Ускорение')}</div>
                <ul class="specProc">
                    {#each GetSpec(stats_boost, mstats_boost) as step, _} 
                        <li class="sort" style="background: linear-gradient(to right, #FFFFFF {step}%, #434A5B 0%)" />
                    {/each}
                </ul>
            </div>
            <div class="lb">
                <div class="title"><i class="ic3"/>{translateText('vehicle', 'Торможение')}</div>
                <ul class="specProc">
                    {#each GetSpec(stats_brakes, mstats_brakes) as step, _} 
                        <li class="sort" style="background: linear-gradient(to right, #FFFFFF {step}%, #434A5B 0%)" />
                    {/each}
                </ul>
            </div>
            <div class="lb">
                <div class="title"><i class="ic4"/>{translateText('vehicle', 'Сцепление')}</div>
                <ul class="specProc">
                    {#each GetSpec(stats_clutch, mstats_clutch) as step, _} 
                        <li class="sort" style="background: linear-gradient(to right, #FFFFFF {step}%, #434A5B 0%)" />
                    {/each}
                </ul>
            </div>
        </div>
        {#if color}
        <Color title={category[selectCategory] ? category[selectCategory].title : ""} lists={colorListsId} />
        {/if}
            
    </div>
</div>