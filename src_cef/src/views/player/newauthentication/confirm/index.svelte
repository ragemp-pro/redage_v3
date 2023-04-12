<script>
    import { translateText } from 'lang'
    import './main.sass'
    import { executeClient } from 'api/rage'
    import { fly } from 'svelte/transition';
    export let onClickRandom;
    export let onPopupToogle;

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 27) onPopupToogle()
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div class="popup__newhud">
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="hud__icon-info popup__newhud_icon"/> {translateText('player2', 'Подтверждение')}
        </div>
        <div class="popup__newhud_text">{translateText('player2', 'Вы уверены, что хотите зарандомить внешность?')}</div>
        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={() => onClickRandom()}>{translateText('player2', 'Подтвердить')}</div>
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:click={() => onPopupToogle()}>{translateText('player2', 'Отмена')}</div>
        </div>
    </div>
</div>