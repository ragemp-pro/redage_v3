<script>
    import { translateText } from 'lang'
    import { executeClientAsyncToGroup } from "api/rage";
    import { format } from 'api/formatter'

    export let selectIndex;
    export let onSelectIndex;

    let selectItem = {}

    executeClientAsyncToGroup("forbes.getId", selectIndex).then((result) => {
        if (result && typeof result === "string")
            selectItem = JSON.parse(result);
    });
</script>

<div class="box-column newphone__project_padding20">
    <div class="newphone__project_title">{selectItem.Name}</div>
    <div class="newphone__forbes_title">${format("money", selectItem.Money)}</div>
    <div class="box-between">
        <div class="box-flex">
            <span class="gray">{translateText('player2', 'Место')}:</span> {selectIndex + 1}
        </div>
        <div class="box-flex">
            <span class="gray">{translateText('player2', 'Уровень')}:</span> {selectItem.Lvl}
        </div>
    </div>
</div>
{#if selectItem.IsShowForbes}
    <div class="newphone__forbes_list small">
        {#if (selectItem.houses && selectItem.houses.length > 0) || (selectItem.biz && selectItem.biz.length > 0)}
            <div class="newphone__forbes_subtitle">{translateText('player2', 'Недвижимость')}:</div>
            {#if selectItem.houses && selectItem.houses.length > 0}
                {#each selectItem.houses as item}
                    <div class="newphone__forbes_element">
                        <div class="box-column">
                            {item.Name}
                            <div class="box-between w-1">
                                <div class="gray">{translateText('player2', 'Стоимость')}:</div>
                                <div>${format("money", item.Money)}</div>
                            </div>
                        </div>
                    </div>
                {/each}
            {/if}
            {#if selectItem.biz && selectItem.biz.length > 0}
                {#each selectItem.biz as item}
                    <div class="newphone__forbes_element">
                        <div class="box-column">
                            {item.Name}
                            <div class="box-between w-1">
                                <div class="gray">{translateText('player2', 'Стоимость')}:</div>
                                <div>${format("money", item.Money)}</div>
                            </div>
                        </div>
                    </div>
                {/each}
            {/if}
        {/if}
        {#if selectItem.vehicles && selectItem.vehicles.length > 0}
            <div class="newphone__forbes_subtitle">{translateText('player2', 'Транспорт')}:</div>
            {#each selectItem.vehicles as item}
                <div class="newphone__forbes_element">
                    <div class="box-column">
                        {item.Name}
                        <div class="box-between w-1">
                            <div class="gray">{translateText('player2', 'Стоимость')}:</div>
                            <div>${format("money", item.Money)}</div>
                        </div>
                    </div>
                </div>
            {/each}
        {/if}
    </div>
{:else}
    <div class="newphone__forbes_anon newphone__project_padding20">
        <div class="newphone__forbes_hidden"></div>
        <div class="gray">{translateText('player2', 'Пользователь запретил разглашать информацию о своём имуществе с помощью настройки телефона Приватность в Forbes.')}</div>
    </div>
{/if}
<div class="newphone__project_button forbes" on:click={()=> onSelectIndex(null)}>Назад</div>