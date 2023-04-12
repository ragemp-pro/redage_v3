<script>
    import { translateText } from 'lang'
    import { executeClientAsyncToGroup } from "api/rage";
    import { format } from 'api/formatter'

    let richList = [];

    executeClientAsyncToGroup("forbes.getList").then((result) => {
        if (result && typeof result === "string")
            richList = JSON.parse(result);
    });

    export let onSelectIndex;

    const onSelectItem = (item, index) => {
        //if (!item.IsShowForbes)
        //    return;

        onSelectIndex(index)
    }
</script>

<div class="newphone__project_title">{translateText('player2', 'Топ богатейших Forbes')}:</div>
<div class="newphone__forbes_list">
    {#each richList as item, index}
        <div class="newphone__forbes_element hover" on:click={() => onSelectItem(item, index)}>
            <div class="newphone__forbes_position">{index + 1}</div>
            <div class="box-column">
                <div>{item.Name}</div>
                <div class="gray">{translateText('player2', 'Состояние')}</div>
                <div>${format("money", item.Money)}</div>
            </div>
            <div class="phoneicons-Button newphone__chevron-back"></div>
        </div>
    {/each}
</div>