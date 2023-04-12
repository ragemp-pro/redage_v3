<script>
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import weaponsinfo from './assets/js/weaponsinfo'
    import { format } from 'api/formatter'
    import './assets/sass/weaponshop.sass'

    const wComponentsType = {
        1: "inv-item-Varmod",//-
        2: "inv-item-Clips",
        3: "inv-item-Suppressors", //Supp,
        4: "inv-item-Scopes", //Scope,
        5: "inv-item-Muzzle-Brakes", //Invalid,
        6: "inv-item-Barrels", //Clip2,
        7: "inv-item-Flashlights", //FlashLaser,
        8: "inv-item-Grips", //Scope2,
        9: "inv-item-Varmod", //Grip2,
    }

    let category = ['Пистолеты','Дробовики','Пистолеты пулеметы', 'Штурмовые винтовки'],
        sumAmmo = 0,
        activeWeaponId = 0,
        activeWeaponCategory = 0,
        cntAmmo = 0,
        weapons = [[{Name:"Pistol","Icon":"inv-item-Pistol","Mats":50},{Name:"SNS Pistol","Icon":"inv-item-SNS-Pistol","Mats":40}]],
        ammo = [],
        components = [],
        ctypes = [],
        activeComponentId = 0,
        activeComponentCategory = 0;

    let selectIWeapon = weapons[activeWeaponCategory][activeWeaponId];
    let activeWeaponInfo = weaponsinfo[selectIWeapon.Name];
    let activeComponentInfo = components[activeComponentId];
    
    const maxAmmo = [
        100,
        50,
        300,
        250,
        48,
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

    window.weaponshop = (weaponJson, ammoJson) => {
        weapons = JSON.parse(weaponJson);
        ammo = JSON.parse(ammoJson);
    }
    
    window.weaponshopcomponents = (componentsJson, ctypesJson) => {
        components = JSON.parse(componentsJson);
        ctypes = JSON.parse(ctypesJson);
        onClickComponentCategory (ctypes[0])
    }

    const Specifications = (num) => {
        let step;
        let array = [];
            for (step = 0; step < 5; step++) {
                array += (`<li class=${step >= num ? '' : 'active'}></li>`)
            }
        return array
    }

    const onSelectComponent = () => {
        executeClient('client.weaponshop.components', selectIWeapon.Name.replace(/\s/g, ''));
    }

    const onClickWeaponCategory = (id) => {
        activeWeaponCategory = id;
        activeWeaponId = 0;
        cntAmmo = 0;
        sumAmmo = 0;
        selectIWeapon = weapons[activeWeaponCategory][activeWeaponId];
        activeWeaponInfo = weaponsinfo[selectIWeapon.Name];
    }

    const onClickWeapon = (id) => {
        activeWeaponId = id; 
        cntAmmo = 0;
        sumAmmo = 0;
        selectIWeapon = weapons[activeWeaponCategory][activeWeaponId];
        activeWeaponInfo = weaponsinfo[selectIWeapon.Name];
    }
    
    const onClickComponentCategory = (id) => {
        activeComponentCategory = id;
        let updateComponentId = false;
        components.forEach((item, index) => {
            if (item.type == id && !updateComponentId) {
                activeComponentId = index;
                updateComponentId = true;
            }
        });
        activeComponentInfo = components[activeComponentId];
    }

    const onClickComponent = (id) => {
        activeComponentId = id;
        activeComponentInfo = components[activeComponentId];
    }

    const onBuy = () => {
        executeClient('client.weaponshop.buy', activeWeaponCategory, activeWeaponId);
    }

    const onBuyAmmo = () => {
        executeClient('client.weaponshop.buyAmmo', activeWeaponCategory,cntAmmo);
    }

    const onBuyComponent = () => {
        executeClient('client.weaponshop.buyComponent', activeWeaponCategory, activeWeaponId, activeComponentInfo.hash);
    }

    const onExit = () => {
        executeClient('client.weaponshop.close');
    }

    const getLengthFix = (length) => {
        let rLength = 6;
        switch (length) {
            case 1:
            case 2:
            case 3:
                rLength = 3;
                break;
            case 4:
            case 5:
            case 6:
                rLength = 6;
                break;
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

    const getLengthFixToComponents = (type) => {
        let pushData = [];
        components.forEach((item, index) => {
            if (item.type == type) pushData.push({...item, index: index});
        });
        if (getLengthFix (pushData.length) != pushData.length) {
            for (let i = 0; i <= getLengthFix (pushData.length) - pushData.length; i++) {
                pushData.push(0);
            }
        }
        return pushData;
    }
    const onBack = () => {
        components = [];
        ctypes = [];
        activeComponentId = 0;
        activeComponentCategory = 0;
        activeComponentInfo = components[activeComponentId];
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;
        onExit()
    }
</script>
<svelte:window on:keyup={HandleKeyDown} />


        
<div id="weaponshop">
    <div class="box-ch">
        
        <div class="box-info">
            <div class="l">
                <div class="title">{translateText('business', 'Магазин Оружия')}</div>
                <p>{translateText('business', 'Легальный магазин огнестрельного оружия, если вы имеете лицензию на ношение оружия, то сможете приобрести любое понравившееся вам оружие для самообороны')}.</p>
            </div>
            <div class="button-box">
                {#if (ctypes.length)}
                    <div class="btn blue" on:click={onBack}>{translateText('business', 'Назад')}</div>
                {:else}
                    <div class="btn red" on:click={onExit}>{translateText('business', 'Выйти')}</div>
                {/if}
            </div>
        
        </div>

        {#if (ctypes.length)}
        <ul class="main">
            {#each ctypes as value, index}
                <li on:click={() => onClickComponentCategory(value)} key={index} class={"main-icon" + (activeComponentCategory === value ? " active" : "")}><span class={`${wComponentsType [value]} iconData`} /></li>
            {/each}
        </ul>
        {:else}
        <ul class="main">
            {#each category as value, index}
                <li on:click={() => onClickWeaponCategory(index)} key={index} class={"main-small" + (activeWeaponCategory === index ? " active" : "")}>{@html value}</li>
            {/each}
        </ul>
        {/if}

        <div class="item-info">
                        
            {#if (ctypes.length)}
                <ul class="items">
                    {#each getLengthFixToComponents(activeComponentCategory) as item, index}
                    <li class={item != 0 ? 'item' : 'empty'} class:active={item && activeComponentId === item.index} key={index} on:click={item != 0 ? () => onClickComponent(item.index) : null}>
                        {#if item != 0}
                            <div class="box"><div class="item-title"><div>{@html item.Name}</div><div></div></div>
                            <span class="item-img {wComponentsType [activeComponentCategory]}" />
                            <div class={true ? 'price' : 'disabled'}><span>$</span>{format("money", item.Mats)}</div></div>
                        {:else}
                            <div>{translateText('business', 'Пусто')}</div>
                        {/if}
                    </li>
                    {/each}
                </ul>
                {:else}
                <ul class="items">
                    {#each new Array(getLengthFix (weapons.length)).fill(0) as _, index}
                    <li class={weapons[activeWeaponCategory][index] ? 'item' : 'empty'} class:active={activeWeaponId === index} on:click={weapons[activeWeaponCategory][index] ? () => onClickWeapon(index) : null}>
                        {#if weapons[activeWeaponCategory][index]}
                        <div class="box"><div class="item-title"><div>{@html weapons[activeWeaponCategory][index].Name}</div><div></div></div>
                        <span class={"item-img " + weapons[activeWeaponCategory][index].Icon} />
                        <div class={true ? 'price': 'disabled'}><span>$</span>{format("money", weapons[activeWeaponCategory][index].Mats)}</div></div>
                        {:else}
                        <div>{translateText('business', 'Пусто')}</div>
                        {/if}
                    </li>
                    {/each}
                </ul>
                {/if}


                {#if (ctypes.length)}

                <div class="c">
                
                    <div class="specification">
                        <div class="contein">
                            <span>{translateText('business', 'Урон')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.damage : 0)}</ul>
                        </div>
                        <div class="contein">
                            <span>{translateText('business', 'Скорострельность')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.ratefire : 0)}</ul>
                        </div>
                        <div class="contein">
                            <span>{translateText('business', 'Точность')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.accuracy : 0)}</ul>
                        </div>
                        <div class="contein">
                            <span>{translateText('business', 'Дальность')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.range : 0)}</ul>
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="title x2">
                            <span>{@html activeComponentInfo.Name}</span>
                        </div>
                    
                        <p>{@html activeComponentInfo.Desc}</p>
                    </div>
                    <div class='btn white' on:click={onBuyComponent}>
                        {translateText('business', 'Купить за')} {format("money", activeComponentInfo.Mats)}$
                    </div>
                </div>
                {:else}
                <div class="c">
                    <div class="specification">
                        <div class="contein">
                            <span>{translateText('business', 'Урон')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.damage : 0)}</ul>
                        </div>
                        <div class="contein">
                            <span>{translateText('business', 'Скорострельность')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.ratefire : 0)}</ul>
                        </div>
                        <div class="contein">
                            <span>{translateText('business', 'Точность')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.accuracy : 0)}</ul>
                        </div>
                        <div class="contein">
                            <span>{translateText('business', 'Дальность')}</span>
                            <ul>{@html Specifications(activeWeaponInfo ? activeWeaponInfo.range : 0)}</ul>
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="title x2">
                            <span>{selectIWeapon.Name}</span>
                        </div>
                        <p>{activeWeaponInfo ? activeWeaponInfo.desc : "Нет"}</p>
                    </div>
                
                    <div class="title x3">
                        <div>{translateText('business', 'Купить патроны')} <span>1 {translateText('business', 'патрон')} = {ammo[activeWeaponCategory]}$</span></div>
        
                        {#if sumAmmo > 0}
                            <div><span class="priceAmmo">{sumAmmo} <b>$</b></span> <span>{translateText('business', 'Цена')}</span></div>
                        {/if}
        
                    </div>
                    <div class="inputblock">
                        <input bind:value={cntAmmo} on:input={(event) => onHandleInput (event.target.value)} class="input" placeholder="Введите кол-во патронов" maxLength="6"/>
                        <div class="butammo" on:click={onBuyAmmo}>{translateText('business', 'Купить')}</div>
                    </div>
                    <div class='box-btn'>
                        <div class='btn white' on:click={onBuy} style='width: 58%'>
                            {translateText('business', 'Купить за')} {format("money", selectIWeapon.Mats)}$
                        </div>
                        <div class='btn min blue' on:click={onSelectComponent} style='width: 38%'>
                            {translateText('business', 'Модификации')}
                        </div>
                    </div>
                </div>
                {/if}
        </div>
    </div>
</div>