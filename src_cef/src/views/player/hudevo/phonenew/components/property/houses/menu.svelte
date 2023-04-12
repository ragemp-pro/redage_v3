<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClient, executeClientAsyncToGroup } from 'api/rage'
    import { format } from 'api/formatter'
    import { hasJsonStructure } from "api/functions";

    let menuAccess = []
    executeClientAsyncToGroup("house.getStats").then((result) => {
        if (hasJsonStructure(result))
            menuAccess = JSON.parse(result);
    });

    let houseData = {}
    executeClientAsyncToGroup("house.houseData").then((result) => {
        if (hasJsonStructure(result))
            houseData = JSON.parse(result);
    });

    let residentsCount = 0;
    executeClientAsyncToGroup("house.residentsData").then((result) => {
        if (hasJsonStructure(result)) {
            result = JSON.parse(result);
            residentsCount = Object.keys (result).length;
        }
    });

    const onAction = (text) => {
        if (!window.loaderData.delay ("house.action", 1.5))
            return;

        executeClientToGroup ("house.action", text)
    }

    const onOpenPark = () => {
        executeClientToGroup ("house.openPark");
    }

    export let onSelectedViewHouse;
    export let onSelectedView;
    export let selectedId;
</script>
<div class="newphone__rent_list">
    <div class="newphone__rent_none hover">
        <div class="box-column">
            <div class="box-flex">
                <div class="violet">{translateText('player2', 'Дом')} №{selectedId}</div>
                {#if houseData.isOwner}
                    <div class="newphone__rent_status">{houseData.isOwner ? translateText('player2', 'Личный') : translateText('player2', 'Подселенный')}</div>
                {/if}
            </div>
            <div class="gray">{translateText('player2', 'Гос. стоимость')}:</div>
            <div class="date">
                ${format("money", houseData.price)}
            </div>
        </div>
        <div class="newphone__rent_noneimage house"></div>
    </div>
    {#if menuAccess.includes ("lock")}
        <div class="newphone__project_button property auction" on:click={() => onAction ("leavehouse")}>{translateText('player2', 'Открыть/закрыть')}</div>
    {/if}
    {#if menuAccess.includes ("leavehouse")}
        <div class="newphone__project_button property" on:click={() => onAction ("leavehouse")}>{translateText('player2', 'Выселиться')}</div>
    {/if}
    {#if menuAccess.includes ("removeall")}
        <div class="newphone__project_button property" on:click={() => onAction ("removeall")}>{translateText('player2', 'Выгнать посетителей')}</div>
    {/if}
    {#if menuAccess.includes ("roommates") && residentsCount > 0}
        <div class="newphone__project_button property" on:click={() => onSelectedViewHouse("Residents")}>{translateText('player2', 'Жильцы')}</div>
    {/if}
    {#if menuAccess.includes ("upgrades")}
        <div class="newphone__project_button property" on:click={() => onSelectedViewHouse("Upgrade")}>{translateText('player2', 'Улучшения дома')}</div>
    {/if}
    {#if menuAccess.includes ("furniture")}
        <div class="newphone__project_button property" on:click={() => onSelectedViewHouse("Furniture")}>{translateText('player2', 'Мебель')}</div>
    {/if}
    <div class="newphone__project_button property" on:click={() => executeClient ("gps.pointDefault", "house")}>{translateText('player2', 'Показать на карте')}</div>
    {#if menuAccess.includes ("inPark")}
        <div class="newphone__project_button property"  on:click={onOpenPark}>{translateText('player2', 'Выбор парковки')}</div>
    {/if}
    {#if menuAccess.includes ("sell") && houseData && Object.values (houseData) && Object.values (houseData).length}
        <div class="newphone__project_button"  on:click={() => onAction ("sell")}>{translateText('player2', 'Продать за')} ${format("money", houseData.sellPrice)}</div>
    {/if}
    <div class="violet box-center m-top10" on:click={() => onSelectedView ()}>{translateText('player2', 'Назад')}</div>
</div>