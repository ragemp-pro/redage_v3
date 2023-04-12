<script>
    import { translateText } from 'lang'
    import { categorieName } from "@/views/player/hudevo/phonenew/components/auction/data";
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from "api/rage";

    let categoryId = 0;
    executeClientAsyncToGroup("auction.getCategory").then((result) => {
        categoryId = Number(result);
    });

    let cameraLink = false;
    const updateCameraLink = (link) => {
        cameraLink = link;
    }

    import {addListernEvent, hasJsonStructure} from 'api/functions';
    import { validate } from "api/validation";

    addListernEvent ("cameraLink", updateCameraLink);

    const updateCameraToggled = () => {
        if (cameraLink == true) {
            return window.notificationAdd(4, 9, translateText('player2', 'Фотография загружается'), 3000);
        }
        //return window.notificationAdd(4, 9, "Временно не работает", 3000);
        window.router.setPopUp("PopupCamera", updateCameraLink)
        executeClient ("camera.open")
    }

    let title = "";
    let text = "";
    let price = "";

    const onSend = () => {
        if (!window.loaderData.delay ("auction.onSend", 1.5))
            return;

        let check;

        /*check = validate("titleAd", title);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }*/

        check = validate("textAd", text);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        const priceVal = Number (String(price).replace(/[^0-9,\s]/g,""));

        if(!priceVal || typeof priceVal !== "number") {
            window.notificationAdd(4, 9, translateText('player2', 'Неверная цена'), 3000);
            return;
        }

        executeClientToGroup ("auction.add", text, !cameraLink ? "" : cameraLink, priceVal)
    }

    export let onSelectedView;

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });

    let isConfirmed = false;
</script>
<div class="newphone__maps_header m-top newphone__project_padding20 mb-4">{categorieName[categoryId]}</div>
<!--<div class="newphone__ads_title newphone__project_padding20">Заголовок:</div>
<input type="text" class="newphone__ads_input" placeholder="Введите текст" on:focus={onInputFocus} on:blur={onInputBlur} bind:value={title}>-->
<div class="newphone__ads_info small mt-5">
    <textarea class="newphone__ads_textarea mt-5" placeholder={translateText('player2', 'Начните вводить сообщение')} on:focus={onInputFocus} on:blur={onInputBlur} bind:value={text}></textarea>
    <!--<div class="newphone__ads_capture">
        {#if !cameraLink}
            <div class="newphone__ads_gray" on:click={updateCameraToggled}>Добавить фото+</div>
        {:else}
            {#if cameraLink !== true}
            <div class="newphone__ads_addimg" style="background-image: url({cameraLink})"></div>
            {/if}
            <div class="box-column">
                <span class="phoneicons-reload green" on:click={updateCameraToggled}></span>
                <span class="phoneicons-trash green" on:click={() => cameraLink = false}></span>
            </div>
        {/if}
    </div>-->
</div>
{#if isConfirmed === false}
    <div class="newphone__ads_title newphone__project_padding20">Начальная цена:</div>
    <input type="text" class="newphone__ads_input" placeholder="Введите цену" on:focus={onInputFocus}
           on:blur={onInputBlur} bind:value={price}>
    <div class="newphone__project_button auction mb-0" on:click={() => isConfirmed = true}>{translateText('player2', 'Выставить лот')}</div>
{:else}
    <div class="newphone__ads_title newphone__project_padding20">{translateText('player2', 'Выставить лот за')} ${price}? {translateText('player2', 'Помните, что аукцион нельзя отменить')}.</div>
    <div class="newphone__project_button auction mb-6" on:click={onSend}>{translateText('player2', 'Я уверен')}</div>
    <div class="newphone__project_button auction mb-0" on:click={() => isConfirmed = false}>{translateText('player2', 'Я не уверен')}</div>
{/if}
<div class="green box-center mt-5" on:click={() => onSelectedView("CreateList")}>{translateText('player2', 'Назад')}</div>