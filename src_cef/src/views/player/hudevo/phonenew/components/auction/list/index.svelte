<script>
    import { translateText } from 'lang'
    import { TimeFormat } from 'api/moment'
    import { format } from "api/formatter";
    import { categorieName, getPng } from "@/views/player/hudevo/phonenew/components/auction/data";
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import {hasJsonStructure} from "api/functions";
    import { charUUID } from 'store/chars';
    export let onSelectedView;

    let categoryId = 0;
    executeClientAsyncToGroup("auction.getCategory").then((result) => {
        categoryId = result;
    });

    let list = []
    const onUpdate = () => {
        executeClientAsyncToGroup("auction.getList").then((result) => {
            if (hasJsonStructure(result))
                list = JSON.parse(result);
        });
    }

    onUpdate();

    import {addListernEvent} from "api/functions";
    addListernEvent ("auction.updateList", onUpdate)

    const onSelect = (id) => {
        executeClientToGroup ("auction.setItemId", id);
        onSelectedView("ListItem");
    }
</script>

<div class="newphone__maps_header m-top newphone__project_padding20">{categorieName[categoryId]}</div>
<div class="newphone__ads_list auction big">
    {#if list && list.length > 0}
        {#each list as item}
            <div class="newphone__ads_element">
                <div class="greenid auc">№{item.id}</div>
                <div class="box-between w-1">
                    <div class="newphone__ads_gray green newphone__font_12">{item.betCount} {translateText('player2', 'участников')}</div>
                    <div class="newphone__ads_gray newphone__font_12">{translateText('player2', 'Закончится')} <span class="green">{TimeFormat(item.time, 'DD.MM в HH:mm')}</span></div>
                </div>
                {#if getPng(item.type, item.image)}
                    <div class="newphone__ads_image contain" style="background-image: url({getPng(item.type, item.image)})"></div>
                {/if}
                <div class="newphone__ads_titletext">{item.title}</div>
                <div class="newphone__ads_text">{item.text}</div>
                <div class="box-between w-1">
                    <div class="newphone__ads_gray">{translateText('player2', 'Начальная ставка')}:</div>
                    <div class="newphone__ads_gray">${format("money", item.createPrice)}</div>
                </div>
                <div class="box-between w-1">
                    <div class="newphone__ads_gray green">Текущая ставка:</div>
                    <div class="newphone__ads_gray green">${format("money", item.lastPrice)}</div>
                </div>
                {#if item.createUuid !== $charUUID}
                <div class="gray">{item.createName}</div>
                {:else}
                    <div class="gray">{translateText('player2', 'Мой активный лот')}</div>
                {/if}
                <div class="newphone__project_button auction small" on:click={()=> onSelect (item.id)}>{translateText('player2', 'Принять участие')}</div>
            </div>
        {/each}
    {:else}
        <div class="newphone__forbes_anon newphone__project_padding20">
            <div class="phoneicons-clubs"></div>
            <div class="gray">{translateText('player2', 'Кажется, тут пусто.. Поищем в других категориях?')}</div>
        </div>
    {/if}
</div>
<div class="green box-center mt-5" on:click={()=> onSelectedView ("Main")}>{translateText('player2', 'На главную')}</div>