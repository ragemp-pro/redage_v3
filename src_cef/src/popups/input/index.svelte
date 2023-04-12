<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import './main.sass'
    import { fly } from 'svelte/transition';

    export let popupData;

    let title = popupData.title,
        plholder = popupData.plholder,
        input = "",
        len = popupData.length;
        
    const onSend = () => {
        executeClient ('input', input);
        input = "";
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13) onSend ()
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div class="popup__newhud_boxinput">
    <!--<div class="popup__newhud_esc">
        <div class="popup__newhud_escbutton box-center">ESC</div>
        <div>Закрыть</div>
    </div>-->
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="hud__icon-info popup__newhud_icon"/> {title}
        </div>
        <!--<div class="popup__newhud_subtitle">
            {title}
        </div>-->
        <input class="popup__newhud_input" placeholder={plholder} maxLength={len} bind:value={input} />
        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={onSend}>{translateText('popups', 'Подтвердить')}</div>
        </div>
    </div>
</div>