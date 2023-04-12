<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import './main.sass'
    import { fly } from 'svelte/transition';

    export let popupData;

    let title = popupData.title,
        elements = [];

    $: if (popupData.elements && typeof popupData.elements === "string")
        elements = JSON.parse(popupData.elements);

    let listId = 0;

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        switch (keyCode) {
            case 13:
                onSelected (elements[listId][1]);
                break;

            case 38: // up
                if(--listId < 0)
                    listId = elements.length - 1;
                break;
            case 40: // down
                if(++listId >= elements.length)
                    listId = 0;
                break;

            case 27:
                onSelected("null");
                break;
        }
    }

    const onSelected = (listItem) => {
        executeClient ('popup.list.selected', listItem)
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />


<div class="popup__newhud_boxinput">
    <div class="popup__newhud_esc">
        <div class="popup__newhud_escbutton box-center">ESC</div>
        <div>{translateText('player2', 'Закрыть')}</div>
    </div>
    <div class="popup__newhud_box">
        <div class="popup__newhud_title">
            <span class="hud__icon-info popup__newhud_icon"/> {title}
        </div>  
        <div class="popup__select_elements">
            {#each elements as item, index}
                <div class="popup__select_element" class:active={listId === index} on:click={() => onSelected (item[1])}>
                    {item [0]}
                </div>
            {/each}
        </div>
        <div class="popup__newhud__buttons">
            <div class="popup__newhud_button" in:fly={{ y: 50, duration: 350 }} on:click={() => onSelected("null")}>Отмена</div>
        </div>
    </div>
</div>