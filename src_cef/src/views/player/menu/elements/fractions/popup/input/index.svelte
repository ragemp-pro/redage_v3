<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    import { setPopup, getPopupData } from "../../data";

    import Color from './color/color.svelte'

    const popupData = getPopupData();

    let valueElement = {}

    if (popupData.input) {
        valueElement[0] = popupData.input.value;
    } else {
        popupData.inputs.forEach((item, index) => {
            valueElement[index] = item.value;
        })
    }

    import Element from './element.svelte'

    const onSend = () => {
        if (typeof popupData.callback === "function")
            popupData.callback(...Object.values(valueElement));

        setPopup ();
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode == 13)
            onSend ()
    }

    const onHandleInput = (index, value) => {
        valueElement [index] = value;
    }
    let colorListsId = 0;

</script>

<svelte:window on:keyup={HandleKeyDown} />

<div class="popup__newhud_boxinput">
    <div class="popup__newhud_box w-428">
        {#if popupData.headerTitle && popupData.headerTitle.length > 0}
            <div class="popup__newhud_title">
                <span class="popup__newhud_icon {popupData.headerIcon}"/> {popupData.headerTitle}
            </div>
        {/if}
        {#if popupData.input}
            <Element data={popupData.input} on:input={(event) => onHandleInput (0, event.target.value)} />
        {:else}
            {#each popupData.inputs as item, index}
                {#if item.type === "color"}
                    <div class="popup__newhud_subtitle">Выбор цвета:</div>
                    <Color title={"Проверка"} lists={colorListsId} defaultColor={item.value} setHandleColor={(color) => onHandleInput (index, color)}/>
                {:else}
                    <Element data={item} on:input={(event) => onHandleInput (index, event.target.value)} />
                {/if}
            {/each}
        {/if}

        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={onSend}>{popupData.button}</div>
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:click={() => setPopup ()}>{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>