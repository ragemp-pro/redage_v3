<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from 'api/rage'

    export let selectedName;
    export let closePopup;

    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch(keyCode) {
            case 13: // up
                onEnter ()
                break;
        }
    }

    const onEnter = () => {
        if (!window.loaderData.delay ("onResidentDell", 5))
            return;

        executeClientToGroup ("client.house.rDell", selectedName);

        closePopup (true);
    }

</script>
<svelte:window on:keyup={handleKeyUp} />
<div id="house__popup">
    <div class="house__popup_block">
        <div class="houseicon-garage house__popup_image"></div>
        <div class="house__popup_header">{translateText('player2', 'ВЫСЕЛЕНИЕ')}</div>
        <div class="house__popup_header">{translateText('player2', 'Выселение')} <span class="orange">{selectedName.replace(/_/g, ' ')}</span></div>
        <div class="house__gray">{translateText('player2', 'Вы действительно хотите выселить')} {selectedName.replace(/_/g, ' ')}?</div>
    </div>
    <div class="box-flex">
        <div class="house_bottom_buttons back" on:click={onEnter}>
            <div>{translateText('player2', 'Выселить')}</div>
            <div class="house_bottom_button">Enter</div>
        </div>
        <div class="house_bottom_buttons back ml-20" on:click={closePopup}>
            <div>{translateText('player2', 'Выйти')}</div>
            <div class="house_bottom_button">ESC</div>
        </div>
    </div>
</div>