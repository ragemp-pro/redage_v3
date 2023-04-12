<script>
    import { translateText } from 'lang'
    import './main.sass'
    import { executeClient } from 'api/rage'
    import { fly } from 'svelte/transition';
    export let popupData;
    
    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 27) executeClient ('client:OnDialogCallback', false)
        else if (keyCode == 13) executeClient ('client:OnDialogCallback', true)
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div id="popup__newhud_upgrade">
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="hud__icon-inventory popup__newhud_icon"/> {translateText('popups', 'Улучшения')}
        </div>
        <div class="popup__newhud_grid">
            {#each new Array(15) as item, index}
                <div class="popup__grid_element">
                    <div class="popup__grid_image">
                        <div>{translateText('popups', 'Мастерская')}</div>
                    </div>
                    <div class="box-between w-100">
                        <div class="gray">150000$</div>
                        <div class="popup__grid_button">{translateText('popups', 'Купить')}</div>
                    </div>
                </div>
            {/each}
        </div>
    </div>
</div>