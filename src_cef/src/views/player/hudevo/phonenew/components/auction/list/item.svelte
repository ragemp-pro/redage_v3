<script>
    import { translateText } from 'lang'
    import { TimeFormat } from 'api/moment'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";
    import { format } from "api/formatter";
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { categorieName, getPng } from "@/views/player/hudevo/phonenew/components/auction/data";
    import { charUUID } from 'store/chars';

    export let onSelectedView;

    let selectItem = {}
    const onUpdate = () => {
        executeClientAsyncToGroup("auction.getListItem").then((result) => {
            if (hasJsonStructure(result))
                selectItem = JSON.parse(result);
        });
    }
    onUpdate ();

    addListernEvent ("auction.updateItem", onUpdate)
    import { onDestroy } from 'svelte'

    onDestroy(() => {
        executeClientToGroup ("auction.setItemId", -1);
        onInputBlur ();
    });
    let price = "";
    const onBet = () => {
        if (!window.loaderData.delay ("auction.onBet", 1.5))
            return;

        const priceVal = Number (String(price).replace(/[^0-9,\s]/g,""));

        if(!priceVal || typeof priceVal !== "number") {
            window.notificationAdd(4, 9, translateText('player2', 'Неверная цена'), 3000);
            return;
        }
        if(selectItem.lastPrice >= priceVal) {
            window.notificationAdd(4, 9, translateText('player2', 'Цена не может быть меньше нынешней'), 3000);
            return;
        }
        executeClientToGroup ("auction.bet", selectItem.id, priceVal)
    }

    let isBet = false;

    const onBack = () => {
        if (isBet)
            isBet = false
        else
            onSelectedView("List")
    }

    let isConfirmed = false;
</script>

<div class="newphone__ads_list auction big">
    <div class="newphone__ads_element">
        <div class="greenid auc">№{selectItem.id}</div>
        <div class="box-between w-1">
            <div class="newphone__ads_gray green newphone__font_12">{selectItem.betCount} {translateText('player2', 'участников')}</div>
            <div class="newphone__ads_gray newphone__font_12">{translateText('player2', 'Закончится')} <span class="green">{TimeFormat(selectItem.time, 'DD.MM в HH:mm')}</span></div>
        </div>
        {#if getPng(selectItem.type, selectItem.image)}
            <div class="newphone__ads_image contain" style="background-image: url({getPng(selectItem.type, selectItem.image)})"></div>
        {/if}
        <div class="newphone__ads_titletext">{selectItem.title}</div>
        <div class="newphone__ads_text">{selectItem.text}</div>
        <div class="box-between w-1">
            <div class="newphone__ads_gray">{translateText('player2', 'Начальная ставка')}:</div>
            <div class="newphone__ads_gray">${format("money", selectItem.createPrice)}</div>
        </div>
        <div class="box-between w-1">
            <div class="newphone__ads_gray green">{translateText('player2', 'Текущая ставка')}:</div>
            <div class="newphone__ads_gray green">${format("money", selectItem.lastPrice)}</div>
        </div>
    </div>
    {#if !isBet}
        {#if selectItem.betsList && selectItem.betsList.length > 0}
            <div class="newphone__maps_header m-top newphone__project_padding20">{translateText('player2', 'Последние биды')}:</div>
            {#each selectItem.betsList.reverse() as item, index}
                <div class="newphone__project_categorie">
                    <div>{item.name}</div>
                    <div class="green">${format("money", item.bet)}</div>
                </div>
            {/each}
        {/if}
        {#if selectItem.createUuid !== $charUUID}
            <div class="newphone__project_button auction" on:click={() => isBet = true}>{translateText('player2', 'Предложить бид')}</div>
        {/if}
    {:else}
        {#if isConfirmed === false}
            <div class="newphone__maps_header m-top newphone__project_padding20 mb-4">{translateText('player2', 'Цена бида')}:</div>
            <input type="text" class="newphone__ads_input mt-5" bind:value={price} placeholder={translateText('player2', 'Введите цену')}
                   on:focus={onInputFocus} on:blur={onInputBlur}>
            <div class="newphone__project_button auction" on:click={() => isConfirmed = true}>{translateText('player2', 'Принять участие')}</div>
        {:else}
            <div class="newphone__ads_title newphone__project_padding20">{translateText('player2', 'Сделать ставку')} ${format("money", price)}? {translateText('player2', 'Помните, что ставку нельзя отменить и что вы заплатите комиссию в размере')} ${format("money", Math.round(price / 100))}.</div>
            <div class="newphone__project_button auction mb-6" on:click={onBet}>{translateText('player2', 'Я уверен')}</div>
            <div class="newphone__project_button auction mb-0" on:click={() => isConfirmed = false}>{translateText('player2', 'Я не уверен')}</div>
        {/if}
    {/if}
    <div class="green box-center mt-5" on:click={onBack}>{translateText('player2', 'Назад')}</div>
</div>