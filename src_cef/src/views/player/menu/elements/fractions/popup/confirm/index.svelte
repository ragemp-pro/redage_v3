<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    import { setPopup, getPopupData } from "../../data";

    const popupData = getPopupData();

    const onSend = () => {
        if (typeof popupData.callback === "function")
            popupData.callback();

        setPopup ();
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13)
            onSend ()
    }

</script>

<svelte:window on:keyup={HandleKeyDown} />

<div class="popup__newhud">
    <div class="popup__newhud_box">
        {#if popupData.headerTitle && popupData.headerTitle.length > 0}
            <div class="popup__newhud_title">
                <span class="popup__newhud_icon {popupData.headerIcon}"/> {popupData.headerTitle}
            </div>
        {/if}

        {#if popupData.text && popupData.text.length > 0}
            <div class="popup__newhud_text">{@html popupData.text}</div>
        {/if}

        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={onSend}>{popupData.button}</div>
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:click={() => setPopup ()}>{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>