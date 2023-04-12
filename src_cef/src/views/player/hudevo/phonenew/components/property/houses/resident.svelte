<script>
    import { translateText } from 'lang'
    import {popudData, selectPopup} from "@/views/house/menu/stores";

    export let onSelectedViewHouse;

    import { TimeFormat } from 'api/moment'
    import { hasJsonStructure } from "api/functions";
    import { executeClient, executeClientToGroup, executeClientAsyncToGroup } from "api/rage";

    let residentsData = []
    executeClientAsyncToGroup("house.residentsData").then((result) => {
        if (hasJsonStructure(result))
            residentsData = JSON.parse(result);
    });

    let selectedName = null;
    const onSelectedName = (name) => selectedName = name;

    const onSelectResident = () => {
        if ($selectPopup)
            return;
        else if (!residentsData || !Object.keys (residentsData).length)
            return;

        isConfirmed = true;
    }

    const onResidentAccess = (name, action) => {
        if (!window.loaderData.delay ("onResidentAccess", 1))
            return;

        executeClientToGroup ("house.rAccess", name, action);
        residentsData [name][action] = !residentsData [name][action];
        //window.router.setViewData(viewData)
    }

    let isConfirmed = false;

    const onEnter = () => {
        if (!window.loaderData.delay ("onResidentDell", 5))
            return;

        executeClientToGroup ("house.rDell", selectedName);

        if (residentsData [selectedName])
            delete residentsData [selectedName];

        selectedName = null;
    }
</script>


{#if !selectedName}
<div class="newphone__rent_list">

    {#each Object.keys (residentsData) as name, index}
    <div class="newphone__rent_none hover" on:click={() => onSelectedName(name)}>
        <div class="box-column">
            <div class="box-flex">
                <div class="violet">{name.replace(/_/g, ' ')}</div>
            </div>
            <div class="gray">{translateText('player2', 'Дата подселения')}:</div>
            <div class="date">
                {TimeFormat (residentsData [name].Date, "DD.MM.YYYY H:mm")}
            </div>
        </div>
        <div class="newphone__rent_noneimage person"></div>
    </div>
    {/each}
    
    <div class="violet box-center m-top10" on:click={() => onSelectedViewHouse ("Menu")}>{translateText('player2', 'Назад')}</div>
</div>
{:else}
<div class="newphone__rent_list">
    <div class="newphone__rent_none hover">
        <div class="box-column">
            <div class="box-flex">
                <div class="violet">{selectedName.replace(/_/g, ' ')}</div>
            </div>
            <div class="gray">{translateText('player2', 'Дата подселения')}:</div>
            <div class="date">
                {TimeFormat (residentsData [selectedName].Date, "DD.MM.YYYY H:mm")}
            </div>
        </div>
        <div class="newphone__rent_noneimage person"></div>
    </div>

    {#if isConfirmed === false}
        <div class="newphone__project_button" on:click={onSelectResident}>{translateText('player2', 'Выселить')}</div>
    {:else}
        <div class="newphone__ads_title newphone__project_padding20">{translateText('player2', 'Вы действительно хотите выселить')} {selectedName.replace(/_/g, ' ')}?</div>
        <div class="newphone__project_button property mb-6" on:click={onEnter}>{translateText('player2', 'Я уверен')}</div>
        <div class="newphone__project_button property mb-0" on:click={() => isConfirmed = false}>{translateText('player2', 'Я не уверен')}</div>
    {/if}
    <div class="box-between w-100 mb-10">
        <div>{translateText('player2', 'Доступ к мебели')}</div>
        <div class="newphone__checkbox" on:click={() => onResidentAccess (selectedName, "isFurniture")}>
            <input class="styled-checkbox viol" id="isFurniture" type="checkbox" checked={residentsData [selectedName].isFurniture} disabled>
            <label class="styled-checkbox1" for="isFurniture"></label>
        </div>
    </div>
    <div class="box-between w-100 mb-10">
        <div>{translateText('player2', 'Доступ к парковке')}</div>
        <div class="newphone__checkbox" on:click={() => onResidentAccess (selectedName, "isPark")}>
            <input class="styled-checkbox viol" id="isPark" type="checkbox" checked={residentsData [selectedName].isPark} disabled>
            <label class="styled-checkbox1" for="isPark"></label>
        </div>
    </div>
    <div class="violet box-center m-top10" on:click={() => onSelectedName(null)}>{translateText('player2', 'Назад')}</div>
</div>
{/if}