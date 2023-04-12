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
        if (keyCode == 27)
            executeClient ('client:OnHospitalDialogCallback', 1)
    }
</script>


<!--<svelte:window on:keyup={HandleKeyDown} />-->

<div id="popup__death">
<!--     <div class="popup__death_timer">11:26</div> -->
    <div class="popup__death_title">{translateText('popups', 'Вы без сознания')}</div>
    <div class="popup__death_subtitle">{@html text}</div>
    <div class="popup__death__buttons">
        <div class="popup__death_button" in:fly={{ y: 50, duration: 250 }} on:click={() => executeClient ('client:OnHospitalDialogCallback', 1)}>{translateText('popups', 'Подождать (5 мин)')}</div>
        <div class="popup__death_button" in:fly={{ y: 50, duration: 500 }} on:click={() => executeClient ('client:OnHospitalDialogCallback', 2)}>{translateText('popups', 'Вызвать EMS (10 мин)')}</div>
        <div class="popup__death_button" in:fly={{ y: 50, duration: 750 }} on:click={() => executeClient ('client:OnHospitalDialogCallback', 3)}>{translateText('popups', 'В больницу (1 мин)')}</div>
    </div>
    <span class="hud__icon-skull popup__death_icon"/>
</div>