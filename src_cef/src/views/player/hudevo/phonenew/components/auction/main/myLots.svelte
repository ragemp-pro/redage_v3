<script>
    import { translateText } from 'lang'
    import { TimeFormat } from 'api/moment'
    import { format } from "api/formatter";
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import { hasJsonStructure } from "api/functions";
    import { charUUID } from 'store/chars';
    import { getPng } from "@/views/player/hudevo/phonenew/components/auction/data";
    export let onSelectedView;

    let myList = []   
    const onUpdate = () => {
        executeClientAsyncToGroup("auction.getMyList").then((result) => {
            if (hasJsonStructure(result))
                myList = JSON.parse(result);
        });
    }

    onUpdate();

    import {addListernEvent} from "api/functions";
    addListernEvent ("auction.updateMyList", onUpdate)

    let activeBlockStatus = false;

    const onSelect = (id) => {
        executeClientToGroup ("auction.setItemId", id);
        onSelectedView("ListItem");
    }
</script>

{#if !myList || !myList.length}
    <div class="newphone__rent_none newphone__margin_left16">
        <div class="box-column">
            <div class="green">{translateText('player2', 'Нет активных лотов')}</div>
            <div class="gray">{translateText('player2', 'Примите участие в аукционе и предложите свой бид')}</div>
        </div>
        <div class="newphone__rent_noneimage auction"></div>
    </div>
{:else}
    <div class="box-center box-between w-1 m-top newphone__project_padding20 newphone__mb-10" on:click={()=> activeBlockStatus = !activeBlockStatus}>
        <div class="newphone__maps_header none-m">{translateText('player2', 'Активные лоты')}</div>
        {#if activeBlockStatus}
            <div class="phoneicons-up newphone__font_16 green"></div>
        {:else}
            <div class="phoneicons-down newphone__font_16 green"></div>
        {/if}
    </div>
    {#if activeBlockStatus}
        {#each myList as item}
            <div class="newphone__ads_element newphone__margin_left16" on:click={() => onSelect (item.id)}>
                <div class="box-between w-1 mb-4">
                    <div class="newphone__ads_gray green newphone__font_12">{item.betCount} {translateText('player2', 'участников')}</div>
                    <div class="newphone__ads_gray newphone__font_12">{translateText('player2', 'Закончится')} <span class="green">{TimeFormat(item.time, 'DD.MM в HH:mm')}</span></div>
                </div>
                {#if getPng(item.type, item.image)}
                    <div class="newphone__ads_image contain" style="background-image: url({getPng(item.type, item.image)})"></div>
                {/if}
                <div class="newphone__ads_titletext">{item.title}</div>
                <div class="newphone__ads_text">{item.text}</div>
                <div class="box-between w-1 mb-4">
                    <div class="newphone__ads_gray">{translateText('player2', 'Начальная ставка')}:</div>
                    <div class="newphone__ads_gray">${format("money", item.createPrice)}</div>
                </div>
                <div class="box-between w-1">
                    <div class="newphone__ads_gray green">{translateText('player2', 'Текущая ставка')}:</div>
                    <div class="newphone__ads_gray green">${format("money", item.lastPrice)}</div>
                </div>
                {#if item.createUuid !== $charUUID}
                <div class="box-between">
                    <div class="gray">{item.createName}</div>
                    <div class="gray"></div>
                </div>
                {:else}
                    <div class="box-between">
                        <div class="gray">{translateText('player2', 'Мой активный лот')}</div>
                        <div class="gray"></div>
                    </div>
                {/if}
            </div>
        {/each}
    {/if}
{/if}