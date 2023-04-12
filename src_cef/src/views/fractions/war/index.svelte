<script>
    import './main.sass'
    import { executeClient, executeClientAsync, executeClientAsyncToGroup } from "api/rage";
    export let viewData;

    $: if (viewData && typeof viewData === "string") 
        viewData = JSON.parse (viewData)
    

    if (!viewData) 
        viewData = {
            zones: [],
            topNames: [],
            wars: []
        }

    import Map from './map/index.svelte'

    let mainElement;

    let elementWidth = 0;
    let elementHeight = 0;


    const updateHeightMap = () => {
        setTimeout(() => {
            if (mainElement) {

                const main = mainElement.getBoundingClientRect();

                if (main) {
                    elementWidth = main.width;
                    elementHeight = main.height;
                }
            }
        }, 0);
    }

    import { onMount } from 'svelte';
    onMount (() => {
        updateHeightMap ();
    });

    let position = {
        x: -301.46353,
        y: 2785.5164,
        z: 60.438744
    };

    executeClientAsync("getPosition").then((result) => {
        if (result && typeof result === "string") {
            position = JSON.parse(result);
        }
    });

    let selectItem = null;

    const onSelectItem = (item) => {
        selectItem = item;
    }
    const onWar = () => {
        if (selectItem)
            executeClient("client.familyZoneAttack", selectItem.id)
    }

</script>

<div id="war" bind:this={mainElement}>

    <Map getPosition={[position.x, position.y]} {elementWidth} {elementHeight} {onSelectItem} zones={viewData.zones} {selectItem} />
    {#if selectItem}
        <div class="box-column zindex-10000">
            <div class="war__title">Карта территорий</div>
            <div class="war__subtitle">Завоюй все территории и стань королём сервера!</div>
            <div class="box-column mt-auto">
                {#if viewData.wars && viewData.wars.length}
                    <div class="war__subtitle white">Ближайшие стрелки:</div>
                    {#each viewData.wars as item}
                    <div class="war__notification small atack" class:atack={item.isAttack} class:protect={!item.isAttack}>
                        {item.text}
                    </div>
                    {/each}
                {/if}
            </div>
        </div>
        <!--<div class="war__notification zindex-10000">
            <div class="box-flex">
                <div class="fractionsicon-warning"></div>
                <div>Внимание!</div>
            </div>
            <div class="war__notification_text">
                За бизнес №24 забита война, в 20:00, будь готов!
            </div>
        </div>-->
        <div class="box-column fl-end zindex-10000">
            <div class="war__text_subtitle">Название объекта</div>
            <div class="war__text_title big">{selectItem.name}</div>
            <div class="war__text_subtitle">Владелец</div>
            <div class="war__text_title">{selectItem.owner}</div>
            <div class="war__text_subtitle">Описание</div>
            <div class="war__text_title">{selectItem.descr}</div>
            <!--<div class="war__text_subtitle">Охрана</div>
            <div class="war__text_title">25 человек</div>-->
            <div class="war__text_subtitle">Доход в час (получает каждый член фракции онлайн)</div>
            <div class="war__text_title green">$ 50</div>
             <!--<div class="war__text_subtitle">Бонусы за монополию:</div>
            <div class="war__text_title">Бесплатное лечение на базе</div>-->
            <div class="war__button active" on:click={onWar}>
                Объявить войну
            </div>
            <div class="box-column mt-auto fl-end">
                <div class="war__subtitle white">Рейтинг семей:</div>
                {#each viewData.topNames as item}
                <div class="war__notification megasmall">
                    {item}
                </div>
                {/each}
            </div>
        </div>
    {/if}
</div>