<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import weaponsinfo from '../../business/weaponshop/assets/js/weaponsinfo'
    import '../../business/weaponshop/assets/sass/weaponshop.sass'

    export let viewData;

    let category = ['Пистолеты', 'Дробовики', 'Пистолеты пулеметы', 'Штурмовые винтовки'/*, 'Снайперские винтовки'*/],
        sumAmmo = 0,
        activeItemID = 0,
        activeCategory = 0,
        cntAmmo = 0,
        items = JSON.parse(viewData[0]),
        ammo = JSON.parse(viewData[1]);

    if (items.length >= 5) {
        category.push('Снайперские винтовки');
    }

    let selectItem = items[activeCategory][activeItemID];
    let activeItemInfo = weaponsinfo[selectItem.Name];

    const onClickCategory = (id) => {
        activeCategory = id;
        activeItemID = 0;
        cntAmmo = 0;
        sumAmmo = 0;
        selectItem = items[activeCategory][activeItemID];
        activeItemInfo = weaponsinfo[selectItem.Name];
    }

    const onClickItem = (id) => {
        activeItemID = id;
        cntAmmo = 0;
        sumAmmo = 0;
        selectItem = items[activeCategory][activeItemID];
        activeItemInfo = weaponsinfo[selectItem.Name];
    }

    const onBuy = () => {
        executeClient ('client.craft.create', activeCategory, activeItemID);
    }

    const onBuyAmmo = () => {
        executeClient ('client.craft.createAmmo', activeCategory, cntAmmo);
    }

    const Specifications = (num) => {
        let step;
        let array = [];
        for (step = 0; step < 5; step++) {
            array += (`<li class=${step >= num ? '' : 'active'}></li>`)
        }
        return array
    }
    
    const maxAmmo = [
        100,
        50,
        300,
        100,
        20,
    ]

    const onHandleInput = (value) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 1) value = 0;  
        const max = maxAmmo [activeWeaponCategory];
        if (value > max) {
            value = max
        }
        
        cntAmmo = value;
        sumAmmo = Math.round(value * ammo[activeWeaponCategory]);
    }
    
    const exit = () => {
        executeClient ('client.craft.close');
    }

    const getLengthFix = (length) => {
        let rLength = 6;
        switch (length) {
            case 7:
            case 8:
            case 9:
                rLength = 9;
                break;
            case 10:
            case 11:
            case 12:
                rLength = 12;
                break;
            case 13:
            case 14:
            case 15:
                rLength = 15;
                break;
        }
        return rLength;
    }       

</script>

<div id="weaponshop">
    <div class="box-ch">        
            <div class="box-info">
                <div class="l">
                    <div class="title">{translateText('fractions', 'Создание Оружия')}</div>
                    <p>{translateText('fractions', 'Используя материалы, можно создать любое из представленных оружий, а так же патроны к нему. Цены указаны в материалах.')}</p>
                </div>
                <div class="button-box">
                    <div class="btn red" on:click={exit}>{translateText('fractions', 'Выйти')}</div>
                </div>
            </div>
            
            <ul class="main">
                {#each category as value, index}
                    <li on:click={() => onClickCategory(index)} class="main-small" class:active={activeCategory === index}>{value}</li>
                {/each}
            </ul>
            
            <div class="item-info">
            
            <ul class="items">

                {#each new Array(getLengthFix (items.length)).fill(0) as _, index}
                    
                    <li class={items[activeCategory][index] ? 'item' : 'empty'} class:active={activeItemID === index} on:click={items[activeCategory][index] ? () => onClickItem(index) : null}>
                    
                    {#if items[activeCategory][index]}
                        <div class="box"><div class="item-title"><div>{items[activeCategory][index].Name}</div><div></div></div>
                        <span class={"item-img " + items[activeCategory][index].Icon} />
                        <div class={true ? 'price': 'disabled'}>{format("money", items[activeCategory][index].Mats)} <span>{translateText('fractions', 'мат')}</span></div></div>
                    {:else}
                        <div>{translateText('fractions', 'Пусто')}</div>
                    {/if}

                    </li>
                {/each}


            </ul>
            
            <div class="c">
            
                <div class="specification">
                    <div class="contein">
                        <span>{translateText('fractions', 'Урон')}</span>
                        <ul>{@html Specifications(activeItemInfo ? activeItemInfo.damage : 0)}</ul>
                    </div>
                    <div class="contein">
                        <span>{translateText('fractions', 'Скорострельность')}</span>
                        <ul>{@html Specifications(activeItemInfo ? activeItemInfo.ratefire : 0)}</ul>
                    </div>
                    <div class="contein">
                        <span>{translateText('fractions', 'Точность')}</span>
                        <ul>{@html Specifications(activeItemInfo ? activeItemInfo.accuracy : 0)}</ul>
                    </div>
                    <div class="contein">
                        <span>{translateText('fractions', 'Дальность')}</span>
                        <ul>{@html Specifications(activeItemInfo ? activeItemInfo.range : 0)}</ul>
                    </div>
                </div>
                <div class="title x2">
                    <span>{selectItem.Name}</span>
                </div>
            
                <p>{activeItemInfo ? activeItemInfo.desc : "Нет"}</p>
            
                <div class="title x3">
                <div>{translateText('fractions', 'Создать патроны')} <span>1 {translateText('fractions', 'патрон')} = {ammo[activeCategory]} {translateText('fractions', 'м')}.</span></div>
                    {#if sumAmmo > 0 }
                        <div><span class="priceAmmo">{sumAmmo} <b>{translateText('fractions', 'м')}</b></span> <span>{translateText('fractions', 'Материалов')}</span></div>
                    {/if}
                </div>
            
                <div class="inputblock">
                    <input bind:value={cntAmmo} on:input={(event) => onHandleInput (event.target.value)} class="input" placeholder="Введите кол-во патронов" maxLength="6"/>
                    <div class="butammo" on:click={onBuyAmmo}>{translateText('fractions', 'Создать')}</div>
                </div>
                {#if selectItem}
                    <div class='btn green' on:click={onBuy}>
                        {translateText('fractions', 'Создать за')} {format("money", selectItem.Mats)} м.
                    </div>
                {:else}
                    <div class='btn green disabled'>{translateText('fractions', 'Недоступно для покупки')}</div>
                {/if}
            </div>
        </div>
    </div>
</div>