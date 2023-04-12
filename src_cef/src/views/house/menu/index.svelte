<script>
    import { executeClient } from 'api/rage'

    import './main.sass'
    import './fonts/style.css'

    //
    
	import { selectPopup, popudData } from '@/views/house/menu/stores.js';


    import HouseParking from './elements/parking.svelte';


    import popupUpdateGarage from './elements/popupUpdateGarage.svelte';

    const popupView = {
        popupUpdateGarage
    }


    const handleKeyUp = (event) => {            
        const { keyCode } = event;
        switch(keyCode) {
            case 27: // up
                onExit ()
                break;
        }
    }

    const onExit = () => {
       
        if (!$selectPopup) {
            executeClient ("client.parking.close");
            
        } else {
            selectPopup.set (null)
            popudData.set ({})
        }
    }
</script>
<svelte:window on:keyup={handleKeyUp} />

{#if popupView[$selectPopup]}
    <svelte:component this={popupView[$selectPopup]} {onExit} />
{/if}

<div id="house">
    <HouseParking {onExit} />
</div>