<script>
    import './assets/css/iconsarenda.css'
    import './assets/css/main.sass'

	import { fade } from 'svelte/transition';
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    
    import ore_coal from './assets/images/ore_coal.png'
    import ore_iron from './assets/images/ore_iron.png'
    import ore_gold from './assets/images/ore_gold.png'
    import ore_emerald from './assets/images/ore_emerald.png'
    import ore_ruby from './assets/images/ore_ruby.png'
    
    import wood_oak from './assets/images/wood_oak.png'
    import wood_maple from './assets/images/wood_maple.png'
    import wood_pine from './assets/images/wood_pine.png'
    
    export let viewData;

    let SellType = 0;
    if (viewData && viewData[0] >= 0 && viewData[1]) {
        SellType = viewData[0];
        viewData[1] = JSON.parse (viewData[1]);
    } else {
        viewData = [
            0,
            {
                0: {Price: 0, ItemId: 1},
                1: {Price: 0, ItemId: 1},
                2: {Price: 0, ItemId: 1},
                3: {Price: 0, ItemId: 1},
                4: {Price: 0, ItemId: 1}
            }
        ];
    }

    const ItemsInfoArray = [
        [
            {
                name: "Ископаемый уголь",
                price: 65,
                png: ore_coal
            },
            {
                name: "Железная руда",
                price: 95,
                png: ore_iron
            },
            {
                name: "Золотая руда",
                price: 120,
                png: ore_gold
            },
            {
                name: "Изумруд",
                price: 420,
                png: ore_emerald
            },
            {
                name: "Рубин",
                price: 700,
                png: ore_ruby
            }
        ],
        [
            {
                name: "Дуб",
                price: 100,
                png: wood_oak
            },
            {
                name: "Клен",
                price: 100,
                png: wood_maple
            },
            {
                name: "Сосна",
                price: 100,
                png: wood_pine
            }
        ]
    ];

    const SellInfoByType = [
        {
            title: 'Продажа ископаемых',
            text: 'Здесь можно продать имеющиеся у вас полезные ископаемые.'
        },
        {
            title: 'Продажа ресурсов',
            text: 'Здесь можно продать имеющиеся у вас ресурсы.'
        }
    ];

    let SelectedItem = -1;
    let ItemAmount = 0;

    const goSell = () => {
        if (!ItemAmount) return;

        executeClient ('resourceSell_playerSell', SellType, SelectedItem, ItemAmount);
        SelectedItem = -1;
        ItemAmount = 0;
    }

    const onExit = () => {        
        executeClient ('resourceSell_closeMenu');
    }

    const handleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        onExit ();
    }
</script>

<svelte:window on:keyup={handleKeyDown}/>

<div id="ores-sale">
    <div class="arenda" class:nonactive={SelectedItem !== -1}>
        <div class="arenda-header">
            <div class="arenda-header__text">
                <div class="arenda-header__title">{SellInfoByType[SellType].title}</div>
                <div class="arenda-header__main-text">{SellInfoByType[SellType].text}</div>
            </div>
            <span class="arendaicon-off" on:click={onExit} />
        </div>
        <div class="arenda-main">
            {#each ItemsInfoArray[SellType] as item, index}
                <div class="arenda-main__element">
                    <div class="arenda-main__title">{window.getItem (viewData[1][index].ItemId).Name}</div>
                    <div class="arenda-main__price">
                        <span class="arendaicon-money"/>
                        ${viewData[1][index].Price} / шт.
                    </div>
                    <div class="arenda-main__img" style="background-image: url('{item.png}')" />
                    <div class="arenda-main__amount_info">У вас есть: {window.getItemToCount (viewData[1][index].ItemId)} шт.</div>
                    <div class="arenda-main__button" on:click={() => SelectedItem = index}>
                        Продать
                    </div>
                </div>
            {/each}
        </div>
    </div>
    {#if SelectedItem !== -1}
        <div class="props-arenda">
            <div class="arenda-customize" transition:fade={{duration: 200}}>
                <div class="arenda-customize__title">Продажа</div>
                <div class="arenda-customize__subtitle">{window.getItem (viewData[1][SelectedItem].ItemId).Name}</div>
                <div class="arenda-customize__img" style="background-image: url('{ItemsInfoArray[SellType][SelectedItem].png}')" />
                <div class="arenda-customize__paragraph">Кол-во для продажи (шт.)</div>
                <div class="arenda-customize__rangeslider">
                    <input type="text" id="sell_ore_amount" on:input={(event) => {
                        if (!event.target.value || event.target.value < 0 || isNaN(event.target.value)) {
                            event.target.value = null;
                            ItemAmount = 0;
                            return;
                        } else if (event.target.value > 250) {
                            event.target.value = 250;
                        }

                        ItemAmount = parseInt(event.target.value);
                    }} />
                </div>
                <div class="arenda-customize__button" on:click={goSell}>
                    Продать за
                    <div class="arenda-customize__money">${format("money", (viewData[1][SelectedItem].Price * ItemAmount) ? (viewData[1][SelectedItem].Price * ItemAmount) : 0)}</div>
                </div>
                <div class="arenda-customize__exit" on:click={() => { SelectedItem = -1; ItemAmount = 0; }}>Закрыть</div>
            </div>
        </div>
    {/if}
</div>