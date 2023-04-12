<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { fly } from 'svelte/transition';
        
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
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="hud__icon-info popup__newhud_icon"/> Информация
        </div>
        <div class="popup__newhud_info mt-10">
            <div class="gray">Владелец:</div>
            <div>Zdobich Family</div>
        </div>
        <div class="popup__newhud_info">
            <div class="gray">Доход в час:</div>
            <div>50$</div>
        </div>
        <div class="popup__newhud_info">
            <div class="gray">Дата захвата:</div>
            <div>27.10.11 15:56</div>
        </div>
        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button mr-10" in:fly={{ y: 50, duration: 350 }} on:click={onSend}>{translateText('popups', 'Захватить')}</div>
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={onSend}>{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>