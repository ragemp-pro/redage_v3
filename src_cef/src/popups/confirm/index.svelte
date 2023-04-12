<script>
    import './main.sass'
    import { executeClient } from 'api/rage'
    import { fly } from 'svelte/transition';
    import { translateText } from 'lang'
    export let popupData;

    let title = popupData.title,
        text = popupData.text;
    
    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 27) executeClient ('client:OnDialogCallback', false)
        else if (keyCode == 13) executeClient ('client:OnDialogCallback', true)
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div class="popup__newhud">
    <div class="popup__newhud_box">
        {#if title.length > 0}
        <div class="popup__newhud_title">
            <span class="hud__icon-info popup__newhud_icon"/> {title}
        </div>
        {/if}
        
        {#if text.length > 0}
            <div class="popup__newhud_text">{@html text}</div>
        {/if}
        <!--<input class="popup__newhud_input" placeholder="Введите сумму которая нужна"/>-->
        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={() => executeClient ('client:OnDialogCallback', true)}>{translateText('popups', 'Подтвердить')}</div>
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:click={() => executeClient ('client:OnDialogCallback', false)}>{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>