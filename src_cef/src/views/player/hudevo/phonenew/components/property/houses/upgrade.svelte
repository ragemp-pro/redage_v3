<script>
    export let onSelectedViewHouse;
    import { translateText } from 'lang'

    import { format } from 'api/formatter'
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import {executeClientAsyncToGroup, executeClient, executeClientToGroup} from "api/rage";

    let houseData = {}

    const updateData = () => {
        executeClientAsyncToGroup("house.houseData").then((result) => {
            if (hasJsonStructure(result))
                houseData = JSON.parse(result);
        });
    }

    updateData ();
    addListernEvent ("phoneHouseUpdate", updateData);

    const furnituresData = [
        /*{
            name: "Оружейный сейф",
            desc: "Сейф, в котором можно хранить оружие. Может быть взломан другими игроками.",
            icon: "houseicon-safe",
            price: 1500
        },
        {
            name: "Шкаф с одеждой",
            desc: "Шкаф, в котором можно хранить одежду. Может быть взломан другими игроками.",
            icon: "houseicon-safe",
            price: 1500
        },
        {
            name: "Шкаф с предметами",
            desc: "Шкаф, в котором можно хранить предметы. Может быть взломан другими игроками.",
            icon: "houseicon-safe",
            price: 1500
        },
        {
            name: "Взломостойкий сейф",
            desc: "Сейф, в котором можно хранить оружие. Нельзя взломать.",
            icon: "houseicon-safe",
            price: 10000
        },*/
        {
            name: translateText('player2', 'Домашняя аптечка'),
            desc: translateText('player2', 'Пополняет количество аптечек в доме.'),
            icon: "heal",
            price: 2500,
            isOne: "Healkit"
        },
        {
            name: translateText('player2', 'Сигнализация'),
            desc: translateText('player2', 'При взломе вашего дома всем ближайшим полицейским будет отправлено уведомление.'),
            icon: "signal",
            price: 3000,
            isOne: "Alarm"
        }
    ]

    const onBuy = (furniture) => {
        if (furniture.isOne && houseData [furniture.isOne])
            return;

        executeClientToGroup ("house.fBuy", furniture.name);
    }
</script>
<div class="newphone__rent_list">
    {#each furnituresData as furniture, index}
    <div class="newphone__rent_none hover" on:click={() => onBuy (furniture)}>
        <div class="box-column">
            <div class="box-flex mb-4">
                <div class="violet">{furniture.name}</div>
            </div>
            <div class="gray">{furniture.desc}</div>

            {#if furniture.isOne && houseData [furniture.isOne]}
                <div class="green">{translateText('player2', 'Куплено')}</div>
            {:else}
                <div class="green">${format("money", furniture.price)}</div>
            {/if}
        </div>
        <div class="newphone__rent_noneimage {furniture.icon}"></div>
    </div>
    {/each}
    <div class="violet box-center m-top10" on:click={() => onSelectedViewHouse ("Menu")}>{translateText('player2', 'Назад')}</div>
</div>