<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    import { setPopup, getPopupData } from "../../data";

    const popupData = getPopupData();

    let listId = 0;

    const onSend = () => {
        if (typeof popupData.callback === "function")
            popupData.callback(popupData.list [listId].id);

        setPopup ();
    }


    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        switch (keyCode) {
            case 13:
                onSend ();
                break;

            case 38: // up
                if(--listId < 0)
                    listId = popupData.list.length - 1;
                break;
            case 40: // down
                if(++listId >= popupData.list.length)
                    listId = 0;
                break;
        }
    }

    const onSelected = (index) => {
        listId = index;
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />


<div class="popup__newhud_boxinput">
    <div class="popup__newhud_box">
        {#if popupData.headerTitle && popupData.headerTitle.length > 0}
            <div class="popup__newhud_title">
                <span class="popup__newhud_icon {popupData.headerIcon}"/> {popupData.headerTitle}
            </div>
        {/if}
        {#if popupData.listTitle}
            <div class="popup__newhud_subtitle">
                {popupData.listTitle}
            </div>
        {/if}
        <div class="popup__select_elements">
            {#each popupData.list as item, index}
                <div class="popup__select_element" class:active={listId === index} on:click={() => onSelected (index)}>
                    {item.name}
                </div>
            {/each}
        </div>

        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={onSend}>{popupData.button}</div>
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 750 }} on:click={() => setPopup ()}>{translateText('popups', 'Отмена')}</div>
        </div>
    </div>
</div>