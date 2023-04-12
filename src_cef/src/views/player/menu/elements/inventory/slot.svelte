<script>
    import { ItemType, itemsInfo, ItemId } from 'json/itemsInfo.js'
    import { getPng } from './getPng.js'

    export let item;
    export let defaultIcon;
    export let defaultName;
    export let defaultStyle;

    let iconInfo = itemsInfo [item.ItemId];

    $: if (item) 
        iconInfo = itemsInfo [item.ItemId];
        
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }

    const GetClothesClass = (_iconInfo, _item) => {
        if (_iconInfo.functionType === ItemType.Clothes && _item.ItemId != -5 && _item.ItemId != -9 && _item.ItemId != -1 && _item.Data.split("_").length >= 2) {
            return Bool(item.Data.split("_")[2]) ? "clothesM" : "clothesF"
        }
        return "";
    }

</script>
{#if item && item.ItemId != 0}
    <div class="box-item {defaultStyle} {GetClothesClass (iconInfo, item)}" class:active={item.active} class:anim={item.anim} class:noAnim={!item.anim} class:noUse={!item.use} on:mousedown on:mouseup on:mouseenter on:mouseleave>
        <!-- {#if index != -1}<div class="id">{index}</div>{/if} -->
        <span class="item-png" style="background-image: url({getPng(item, iconInfo)})" />
        <!-- {#if iconInfo.Icon !== "-"}<span class={`${iconInfo.Icon} icon`} />{/if} -->
        {#if defaultIcon === undefined && item.Count > 1 && item.ItemId !== 19 && iconInfo.functionType !== ItemType.Clothes && iconInfo.functionType !== ItemType.Weapons}<div class="count">{item.Count}</div>{/if}
        {#if (item.ItemId === 19) && item.Data.split("_") && item.Data.split("_").length >= 1}<div class="count">{item.Data.split("_")[0]}</div>{/if}
        {#if defaultIcon === undefined && iconInfo.functionType === 1 && item.ItemId != -5 && item.ItemId != -9 && item.ItemId != -1 && item.Data.split("_").length >= 2}<div class="count">{Bool(item.Data.split("_")[2]) ? "М" : "Ж"}</div>{/if}
        {#if defaultIcon === undefined && item.ItemId == -9}<div class="count">x{item.Data}</div>{/if}
        {#if defaultIcon === undefined && item.ItemId == ItemId.SimCard}<div class="count">{item.Data}</div>{/if}
        {#if defaultIcon === undefined && item.ItemId == ItemId.VehicleNumber}<div class="count">{item.Data}</div>{/if}
    </div>
{:else}
    <div class="box-item gray {defaultStyle}" class:active={item.active} class:anim={item.anim} class:noAnim={!item.anim} class:noUse={!item.use} class:hover={item.hover} on:mousedown on:mouseup on:mouseenter on:mouseleave>
        <!-- {#if index != -1}<div class="id">{index}</div>{/if} -->
        {#if defaultIcon != undefined}<span class="{defaultIcon} icon" />{/if}
        {#if defaultName != undefined}<div class="box-item-name">{defaultName} </div>{/if}
    </div>
{/if}
