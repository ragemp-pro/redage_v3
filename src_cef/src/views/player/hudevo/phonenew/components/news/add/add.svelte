<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { executeClientAsyncToGroup } from "api/rage";

    export let onSelected;
    export let onBack;
    export let onAdd;
    export let isPremium;

    let maxSymbol = 150;
    let text = "";
    let priceOneSymbol = 50;

    executeClientAsyncToGroup("newsPriceOneSymbol").then((result) => {
        priceOneSymbol = result;
    });

    const updateCameraToggled = () => {
        if (cameraLink == true) {
            return window.notificationAdd(4, 9, translateText('player2', 'Фотография загружается'), 3000);
        }
        //return window.notificationAdd(4, 9, "Временно не работает", 3000);
        window.router.setPopUp("PopupCamera", updateCameraLink)
        executeClient ("camera.open")
    }

    let cameraLink = false;
    const updateCameraLink = (link) => {
        cameraLink = link;
    }

    import { addListernEvent } from 'api/functions';
    import {validate} from "api/validation";
    addListernEvent ("cameraLink", updateCameraLink);


    const onSend = () => {
        if (!window.loaderData.delay ("news.onSend", 1.5))
            return;

        let check;

        check = validate("textAd", text);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        onAdd(text, cameraLink);
    }
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";
</script>

<div in:fade class="box-100">
    <div class="newphone__maps_header m-top newphone__project_padding20 mb-10">{translateText('player2', 'Создание объявления')}:</div>
    <div class="newphone__ads_gray m-8 newphone__project_padding20">
        {translateText('player2', 'Все объявления проходят через модерацию редакторов LS News!')}
    </div>
    <div class="newphone__ads_gray m-8 newphone__project_padding20">
        {translateText('player2', 'Лимит')} - {maxSymbol - text.length}/{maxSymbol} {translateText('player2', 'символов')}.
    </div>
    <div class="newphone__ads_info small">
        <textarea class="newphone__ads_textarea" bind:value={text} maxlength={maxSymbol} on:focus={onInputFocus} on:blur={onInputBlur} placeholder="Поле для ввода текста объявления..." />
        <div class="newphone__ads_capture">
            {#if !cameraLink}
                <div class="newphone__ads_gray" on:click={updateCameraToggled}>{translateText('player2', 'Добавить фото+')}</div>
            {:else}
                {#if cameraLink !== true}
                <div class="newphone__ads_addimg" style="background-image: url({cameraLink})"></div>
                {/if}
                <div class="box-column">
                    <span class="phoneicons-reload" on:click={updateCameraToggled}></span>
                    <span class="phoneicons-trash" on:click={() => cameraLink = false}></span>
                </div>
            {/if}
        </div>
    </div>
    <div class="box-between newphone__project_padding20 mb-0">
        <div>{translateText('player2', 'Стоимость')}</div>
        <div class="red">${format("money", text.length * priceOneSymbol * (isPremium ? 2 : 1))}</div>
    </div>
    <div class="newphone__project_button" on:click={onSend}>{translateText('player2', 'Подать объявление')}</div>
    <div class="red box-center" on:click={onBack}>{translateText('player2', 'Назад')}</div>
</div>