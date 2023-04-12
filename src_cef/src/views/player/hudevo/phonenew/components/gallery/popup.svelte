<script>
    import { translateText } from 'lang'
    import { loadImage } from 'api/functions'

    import {executeClient, executeClientAsyncToGroup} from "api/rage";
    import { selectedImage, selectedImageFunc } from './../../stores'

    let data = []

    executeClientAsyncToGroup("getGallery").then((result) => {
        if (result && typeof result === "string")
            data = JSON.parse(result);
    });

    let useImage = false;

    const onCancel = () => {
        selectedImage.set(false);
        selectedImageFunc.set(false);
    }

    const selectImage = (link) => {
        if (typeof $selectedImageFunc === "function")
            $selectedImageFunc (link);

        onCancel ();
    }

    const onCamera = () => {
        const phoneClass = document.querySelector('.newphone__image');
        phoneClass.classList.add('phone__camera');

        setTimeout(() => {

            window.router.setPopUp("PopupCamera", $selectedImageFunc)
            executeClient ("camera.open");
            phoneClass.classList.remove('phone__camera');

            onCancel ();
        }, 150)
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__popup b-2" in:fade>
    {#if !useImage}
        <div class="newphone__popup">
            <div class="newphone__popup_element" on:click={onCamera}>{translateText('player2', 'Сделать фото')}</div>
            <div class="newphone__popup_element" on:click={() => useImage = true}>{translateText('player2', 'Выбрать из галереи')}</div>
            <div class="newphone__popup_element red" on:click={onCancel}>{translateText('player2', 'Отмена')}</div>
        </div>
    {:else}
        <div class="newphone__gallery newphone__popup_border">
            <div class="box-between w-1 newphone__project_padding20 pt-20">
                <div class="newphone__maps_header">{translateText('player2', 'Последние фото')}</div>
                <div class="phoneicons-add1" on:click={() => useImage = false}></div>
            </div>
            {#if data.length}
            <div class="newphone__gallery_grid">
                {#each data as item}
                    <div class="newphone__gallery_item" use:loadImage={item[0]} on:click={() => selectImage (item[0])} />
                {/each}
            </div>
            {:else}
                <div class="newphone__gallery_nophoto">{translateText('player2', 'У вас нет сохраненных фото')}</div>
            {/if}
        </div>
    {/if}
</div>