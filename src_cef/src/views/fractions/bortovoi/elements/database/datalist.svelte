<script>
    import { translateText } from 'lang'
    import {executeClientAsyncToGroup, executeClientToGroup} from 'api/rage'
    import { addListernEvent } from 'api/functions'

    const getList = () => {

        executeClientAsyncToGroup("getDataBaseList").then((result) => {
            if (result && typeof result === "string")
                adsList = JSON.parse(result);
        });
    }
    getList();

    addListernEvent ("updateDataList", getList) 
</script>
<div class="policecomputer__title">DataBase</div>
<div class="policecomputer__input">
    <div class="bortovoiicon-loop"></div>
    <input placeholder="Введите имя и фамилию..">
</div>
<div class="policecomputer__info_grid database mt-24">
    {#each new Array(30) as item}
        <div class="policecomputer__database_element">
            <div class="policecomputer__database_image"></div>
            <div class="policecomputer__database_number">{translateText('fractions', 'Досье')} №43332</div>
            <div class="policecomputer__database_name">Vitaliy Zdobich</div>
            <div class="policecomputer__database_info">
                {translateText('fractions', 'УК')} <span class="white">12.6, 13.4</span> {translateText('fractions', 'АК')} <span class="white">12.6, 13.4</span>
            </div>
        </div>
    {/each}
</div>