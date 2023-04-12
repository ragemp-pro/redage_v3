<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    export let id;
    export let icon;
    export let text;
    export let price;
    export let onSelectItem;
    export let selectItem;
    export let lvl = false;

    const onSelect = (index) => {
        onSelectItem (index);
        executeClient ('client.custom.item', index);
    }

    const onBuy = () => {
        executeClient ('client.custom.buy')
    }
</script>

<li class={"listitems " + (selectItem !== id || "active")} id={id} on:click={onBuy} on:mouseenter={e => onSelect (id)}>
    <i class={'icon ilsc-' + icon}></i>
    <div class="flex un">
        {#if lvl}
        <div class="title headersm">
            <span>{translateText('vehicle', 'Уровень')}</span>
            <ul class="lvl">
                {#each new Array(4).fill(0) as _, index}
                    <li class="star" class:active={index >= lvl}></li>
                {/each}
            </ul>
            
        </div>
        {/if}
    
        <div class="desc">{@html text}</div>
        <div class="price">{price} <span>$</span></div>                        
    </div>                        
</li>