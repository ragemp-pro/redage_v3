<script>
    import { executeClientAsyncToGroup } from 'api/rage'
    import { translateText } from 'lang'

    let tasksDay = [];
    executeClientAsyncToGroup("getTasksDay").then((result) => {
        if (result && typeof result === "string")
            tasksDay = JSON.parse(result);
    });

    let tasksWeek = [];
    executeClientAsyncToGroup("getTasksWeek").then((result) => {
        if (result && typeof result === "string")
            tasksWeek = JSON.parse(result);
    });
</script>

<div class="battlepass__tasks">
    <div class="battlepass__tasks__header">{translateText('player', 'Задания уровня')}</div>
    <div class="battlepass__tasks_block">
        {#each tasksDay as item}
            <div class="battlepass__tasks_element" class:done={item.exp === -1}>
                <div class="box-between w-1 b-11">
                    <div class="battlepass__tasks_height">{item.name}</div>
                    <div class="small">{translateText('player', 'Ежедневное')}</div>
                </div>
                <div class="box-between w-1">
                    {#if item.exp === -1}
                        <div class="green">{translateText('player', 'Выполнено')}</div>
                    {:else}
                        <div class="battlepass__tasks_exp">+{item.exp} XP</div>
                    {/if}
                    <div class="gray">{item.count}/{item.maxCount}</div>
                </div>
            </div>
        {/each}
        {#each tasksWeek as item}
            <div class="battlepass__tasks_element" class:done={item.exp === -1} class:week={true}>
                <div class="box-between w-1 b-11">
                    <div class="battlepass__tasks_height">{item.name}</div>
                    <div class="small">{translateText('player', 'Еженедельное')}</div>
                </div>
                <div class="box-between w-1">
                    {#if item.exp === -1}
                        <div class="green">{translateText('player', 'Выполнено')}</div>
                    {:else}
                        <div class="battlepass__tasks_exp">+{item.exp} XP</div>
                    {/if}
                    <div class="gray">{item.count}/{item.maxCount}</div>
                </div>
            </div>
        {/each}
    </div>
</div>