<script>
    import { translateText } from 'lang'
    import {currentPage} from '../../stores'
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    import { hasJsonStructure } from "api/functions";
    export let onSelectedView;
    export let onSelectedId;

    /*{
        type: 0,
        isOwner: false,
        id: 1
    }*/

    let propertyList = [{
        id: 5,
        type: 1
    }]

    executeClientAsyncToGroup("getProperty").then((result) => {
        if (hasJsonStructure(result))
            propertyList = JSON.parse(result);
    });

    const onSelectItem = (item) => {
        if (item.type === 0) {
            onSelectedId (item.id)
            onSelectedView ('Houses')
        } else if (item.type === 1) {
            onSelectedId (item.id)
            onSelectedView ('Business')
        }
    }
    import { fade } from 'svelte/transition'

    const setPoint = () => {
        executeClient ("gps.name", translateText('player2', 'Риэлторское агентство'));
        executeClientToGroup ("close");
    }
</script>
<div class="newphone__rent_list" in:fade>
    {#if propertyList && typeof propertyList === "object" && propertyList.length > 0}
        {#each propertyList as item}
            <div class="newphone__rent_none hover" on:click={() => onSelectItem(item)}>
                <div class="box-column">
                    <div class="box-flex">
                        <div class="violet">{item.type === 0 ? "Дом" : "Бизнес"}</div>
                        {#if item.isowner}
                            <div class="newphone__rent_status">{item.isOwner ? "Личный" : "Подселенный"}</div>
                        {/if}
                    </div>
                    <div class="gray">{translateText('player2', 'Номер')}:</div>
                    <div class="date">
                        №{item.id}
                    </div>
                </div>
                <div class="newphone__rent_noneimage" class:house={item.type === 0} class:business={item.type === 1}></div>
            </div>
        {/each}
    {:else}
        <div class="newphone__rent_none">
            <div class="box-column">
                <div class="violet mb-4">{translateText('player2', 'Имущества нет')}</div>
                <div class="gray">{translateText('player2', 'Но вы можете присмотреть что нибудь в риэлторском агенстве')}</div>
            </div>
            <div class="newphone__rent_noneimage"></div>
        </div>
        <div class="newphone__project_button property" on:click={setPoint}>{translateText('player2', 'Как добраться?')}</div>
        <div class="newphone__project_button property" on:click={()=> currentPage.set("taxi")}>{translateText('player2', 'Вызвать такси')}</div>
    {/if}
</div>