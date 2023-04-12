<script>
    import { translateText } from 'lang'
    import {executeClientAsyncToGroup} from "api/rage";
    import { selectNumber, selectedImageFunc, selectedImage } from "@/views/player/hudevo/phonenew/stores";


    export let sendGeo;
    export let sendImage;
    export let onClosePopup;
    export let selectedNumber;

    let contactData = {};

    executeClientAsyncToGroup("getContact", selectedNumber).then((result) => {
        if (result && typeof result === "string") {
            contactData = JSON.parse(result);
        }
    });
    //

    const onAddBlackList = () => {
        executeClientAsyncToGroup("addBlackList", selectedNumber).then((result) => {
            if (result)
                contactData.IsBlackList = true;
        });
    }

    const onDellBlackList = () => {
        executeClientAsyncToGroup("dellBlackList", selectedNumber).then((result) => {
            if (result)
                contactData.IsBlackList = false;
        });
    }

    const onSendImage = () => {
        selectedImage.set(true)
        selectedImageFunc.set(sendImage)
        onClosePopup ();
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__popup" in:fade>
    <div class="newphone__popup_element" on:click={sendGeo}>{translateText('player2', 'Отправить геопозицию')}</div>
    <div class="newphone__popup_element" on:click={onSendImage}>{translateText('player2', 'Отправить фото')}</div>

    {#if contactData.IsBlackList}
        <div class="newphone__popup_element red" on:click={onDellBlackList}>{translateText('player2', 'Разблокировать контакт')}</div>
    {:else}
        <div class="newphone__popup_element" on:click={onAddBlackList}>{translateText('player2', 'Заблокировать контакт')}</div>
    {/if}

    <div class="newphone__popup_element" on:click={onClosePopup}>{translateText('player2', 'Отмена')}</div>
</div>