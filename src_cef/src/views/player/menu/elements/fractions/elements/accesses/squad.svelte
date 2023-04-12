<script>
    import { translateText } from 'lang'
    import {executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import Access from '../access/index.svelte'
    import {setPopup} from "../../data";

    let departments = []
    executeClientAsyncToGroup("getDepartments").then((result) => {
        if (result && typeof result === "string") {
            departments = JSON.parse(result);
        }
    });


    let selectDepartmentId = -1;
    const onSelectDepartmentId = (item) => {
        executeClientToGroup('departmentRankLoad', item.id)
        selectDepartmentId = item.id;
        selectId = -1;
    }

    let ranks = []
    const departmentRankInit = (_ranks) => {
        if (_ranks && typeof _ranks === "string")
            ranks = JSON.parse(_ranks);

        onSelectId (ranks[0])
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.departmentRankInit", departmentRankInit)


    let selectId = -1;
    const onSelectId = (item) => {
        executeClientToGroup('departmentRankAccessLoad', selectDepartmentId, item.id)
        selectId = item.id;
    }
</script>
<div class="box-between mr-20 align-startflex">
    <div class="box-column">
        <div class="fractions_stats_title mt-20">{translateText('player1', 'Отряды')}:</div>
        <div class="fractions__main_scroll w-204 h-480">
            {#each departments as item}
                <div class="fractions__scroll_element hover p-20 fw-bold" on:click={() => onSelectDepartmentId(item)}>
                    <div>{item.name}</div>
                    {#if selectDepartmentId === item.id}
                        <div class="fs-36">&#8250;</div>
                    {/if}
                </div>
            {/each}
        </div>
    </div>
    <div class="box-column">
        <div class="fractions_stats_title mt-20">Ранги:</div>
        <div class="fractions__main_scroll w-240 h-480">
            {#if selectDepartmentId >= 0}
                {#each ranks as item}
                    <div class="fractions__scroll_element hover p-20 fw-bold" on:click={() => onSelectId(item)}>
                        <div class="box-flex">
                            <div class="mr-16">{item.id}</div>
                            <div>{item.name}</div>
                        </div>
                        {#if selectId === item.id}
                            <div class="fs-36">&#8250;</div>
                        {/if}
                    </div>
                {/each}
            {/if}
        </div>
    </div>
</div>

<div class="box-column">
    <div class="fractions_stats_title mt-20">{translateText('player1', 'Доступы')}:</div>
    {#if selectId >= 0}
        <Access
                itemId={`${selectDepartmentId}|${selectId}`}
                executeName="updateDepartmentRankAccess"
                isSelector={false}
                clsScroll="w-399 h-480"
                isSkip={true} />
    {:else}
        <div class="fractions__main_scroll w-399 h-480" />
    {/if}
</div>
<div class="box-column">
</div>