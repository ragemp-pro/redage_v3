<script>
    import { executeClient } from 'api/rage'

    import HouseBuyPanel from './elements/housebuypanel.svelte';
    import HousePopup from './elements/popup.svelte';

    export let viewData;

    if (!viewData)
        viewData = {
            id: 0,
            class: 0,
            price: 0,
            gosPrice: 0,
        }

    let buyConfirm = false;

    const onKeyUp = (event) => {
        switch(event.which) {
            case 13:
                onBuy ();
                break;
            case 27:
                onExit ();
                break;
        }
    }

    const onBuy = () => {
        if (!buyConfirm)
            buyConfirm = true;
        else {
			executeClient ("client.house.buy");
        }
    }

    const onExit = () => {
        
        if (buyConfirm)
            buyConfirm = false;
        else {
			executeClient ("client.house.buy.exit");            
        }
    }
</script>


<svelte:window on:keyup={onKeyUp} />

{#if buyConfirm}
    <HousePopup {viewData} {onBuy} {onExit} />
{/if}

<div id="house">
    <HouseBuyPanel {viewData} {onBuy} {onExit}  />
</div>