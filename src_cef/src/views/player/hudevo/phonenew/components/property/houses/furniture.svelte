<script>
    import { translateText } from 'lang'
    import {currentPage} from '../../../stores'

    export let onSelectedViewHouse;

    import {addListernEvent, hasJsonStructure} from "api/functions";
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from "api/rage";

    let houseFurnitures = []
    const updateData = () => {
        executeClientAsyncToGroup("house.houseFurnitures").then((result) => {
            if (hasJsonStructure(result))
                houseFurnitures = JSON.parse(result);
        });
    }

    updateData ();
    addListernEvent ("phoneHouseUpdate", updateData);
    let selectedFurniture = null;
    const onSelectedFurniture = (item) => selectedFurniture = item;

    const onFurnitureBuy = (type) => {
        if (!window.loaderData.delay ("onFurnitureBuy", 1))
            return;

        const index = selectedFurniture;
        selectedFurniture = null;

        executeClientToGroup ("house.fUse", houseFurnitures[index].Id, type);

        if (type == 0) {
            houseFurnitures.splice(index, 1);
            houseFurnitures = houseFurnitures;
        }
    }

    const setPoint = () => {
        executeClient ("gps.name", translateText('player2', 'Мебельный магазин'));
        executeClientToGroup ("close");
    }

</script>
{#if selectedFurniture === null}
    <div class="newphone__rent_list">
        {#if houseFurnitures && typeof houseFurnitures === "object" && houseFurnitures.length > 0}
            {#each houseFurnitures as furniture, index}
            <div class="newphone__rent_none hover" on:click={() => onSelectedFurniture (index)}>
                <div class="box-column">
                    <div class="box-flex">
                        <div class="violet">{translateText('player2', 'Мебель')}</div>
                    </div>
                    <div class="gray">{furniture.Name}</div>
                </div>
                <div class="newphone__rent_noneimage heal" style="background-image: url({document.cloud + `inventoryItems/furniture/${furniture.Model}.png`});"></div>
            </div>
            {/each}
        {:else}
            <div class="newphone__rent_none">
                <div class="box-column">
                    <div class="violet mb-4">{translateText('player2', 'Мебели нет')}</div>
                    <div class="gray">{translateText('player2', 'Но вы можете купить её у Ивана в мебельном магазине')}</div>
                </div>
                <div class="newphone__rent_noneimage"></div>
            </div>
            <div class="newphone__project_button property" on:click={setPoint}>{translateText('player2', 'Как добраться')}?</div>
            <div class="newphone__project_button property" on:click={()=> currentPage.set("taxi")}>{translateText('player2', 'Вызвать такси')}</div>
        {/if}
        <div class="violet box-center m-top10" on:click={() => onSelectedViewHouse ("Menu")}>{translateText('player2', 'Назад')}</div>
    </div>
{:else}
    <div class="newphone__rent_list">
        <div class="newphone__rent_none hover">
            <div class="box-column">
                <div class="box-flex">
                    <div class="violet">{translateText('player2', 'Мебель')}</div>
                </div>
                <div class="gray">{houseFurnitures [selectedFurniture].Name}</div>
            </div>
            <div class="newphone__rent_noneimage heal" style="background-image: url({document.cloud + `inventoryItems/furniture/${houseFurnitures [selectedFurniture].Model}.png`});"></div>
        </div>

        <div class="newphone__project_button" on:click={() => onFurnitureBuy (1)} class:auction={!houseFurnitures [selectedFurniture].IsSet} class:property={houseFurnitures [selectedFurniture].IsSet}>{!houseFurnitures [selectedFurniture].IsSet ? translateText('player2', 'Установить') : translateText('player2', 'Убрать')}</div>
        <div class="newphone__project_button" on:click={() => onFurnitureBuy (0)}>{translateText('player2', 'Продать')}</div>
        <div class="violet box-center m-top10" on:click={() => onSelectedFurniture (null)}>{translateText('player2', 'Назад')}</div>
    </div>
{/if}