<script>
    import './main.sass'
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    export let popupData;

    let title = popupData.title,
        text = popupData.text;
</script>

<div class="popup__newhud">
    {#if title.length > 0}
    <div class="popup__newhud_title">
        <span class="hud__icon-info popup__newhud_icon"/> {title}
    </div>
    {/if}
    <div class="popup__newhud_line"></div>
    
    {#if text.length > 0}
        <div class="popup__newhud_text" style="text-align: center">{@html text}</div>
        <div class="popup__newhud_line"></div>
    {/if}
    
    <div class="popup__newhud__many__buttons">
        <div class="popup__newhud_button" in:fly={{ y: 50, duration: 250 }} on:click={() => executeClient ('client:OnHospitalDialogCallback', 1)}>{translateText('popups', 'Подождать (5 мин)')}</div>
        <div class="popup__newhud_button" in:fly={{ y: 50, duration: 500 }} on:click={() => executeClient ('client:OnHospitalDialogCallback', 2)}>{translateText('popups', 'Вызвать EMS (10 мин)')}</div>
        <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:click={() => executeClient ('client:OnHospitalDialogCallback', 3)}>{translateText('popups', 'В больницу (1 мин)')}</div>
    </div>
</div>