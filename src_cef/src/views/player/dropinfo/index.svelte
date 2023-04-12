<script>
    import { translateText } from 'lang'
    import './main.sass'

    export let viewData;

    if (!viewData)
        viewData = false;

    let isUpdateType = viewData;

    import { executeClient } from 'api/rage'

    const HandleKeyUp = (event) => {
        const { keyCode } = event;
        
        switch (keyCode) {
            case 9:
                UpdateType ();
                break;
            case 13:
                onEnter ();
                break;
            case 27: 
                onClose ();
                break;
        }
    }

    const UpdateType = () => {
        if (isUpdateType)
            return;
        viewData = !viewData;
        executeClient ("client.dropinfo.updateType", viewData);
    }
    const onEnter = () => {
        executeClient ("client.dropinfo.enter");
    }
    const onClose = () => {
        executeClient ("client.dropinfo.close");
    }
</script>

<svelte:window on:keyup={HandleKeyUp} />

<div class="box-dropinfo">
    {#if !viewData}
    <div class="box-KeyInfo" on:click={onEnter}>
        <div class="KeyInfo text">{translateText('player', 'ЛКМ')}</div>
        {translateText('player', 'Поставить')}
    </div>
    <div class="box-KeyInfo">
        <div class="KeyInfo text">{translateText('player', 'ПКМ')}</div>
        {translateText('player', 'Отменить')}
    </div>
    {:else}
    <div class="box-KeyInfo" on:click={onEnter}>
        <div class="KeyInfo text">ENTER</div>
        {translateText('player', 'Поставить')}
    </div>
    <div class="box-KeyInfo">
        <div class="KeyInfo text">{translateText('player', 'Пробел')}</div>
        {translateText('player', 'Свободный режим')}
    </div>
    <div class="box-KeyInfo" on:click={onClose}>
        <div class="KeyInfo text">Esc</div>
        {translateText('player', 'Отменить')}
    </div>
    <div class="box-KeyInfo">
        <div class="KeyInfo text">{translateText('player', 'ПКМ')}</div>
        {translateText('player', 'Сменить режим')}
    </div>
    {/if}
    {#if !isUpdateType}
    <div class="box-KeyInfo" on:click={UpdateType}>
        <div class="KeyInfo text">TAB</div>
        {translateText('player', 'Сменить режим редактирования')}
    </div>
    {/if}
    <div class="box-KeyInfo">
        <div class="KeyInfo text">V</div>
        {translateText('player', 'Позиция камеры')}
    </div>
</div>