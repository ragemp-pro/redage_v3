<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    import keys from 'store/keys'
    import keysName from 'json/keys.js'

    const InfoData = {
        Warehouse: translateText('player2', 'Открыть склад'),
        DriveSchool: translateText('player2', 'Открыть меню автошколы'),
        RentCar: translateText('player2', 'Открыть меню аренды'),
        Festive: translateText('player2', 'Открыть обмен Coin'),
        SaluteShop: translateText('player2', 'Открыть магазин фейерверков'),
        BlackMarket: translateText('player2', 'Открыть чёрный рынок'),
        TankRoyale: translateText('player2', 'Регистрация Tank Royale'),
        Casino: translateText('player2', 'двери в казино'),
        Atm: translateText('player2', 'Открыть меню банкомата'),
        Electrician: translateText('player2', 'Переодеться'),
        Repair: translateText('player2', 'Ремонтировать транспорт'),
        SeatingArrangements: translateText('player2', 'Взаимодействовать'),
        BuyPoints: translateText('player2', 'Купить алкоголь'),
        EnterGarage: translateText('player2', 'Войти в гараж'),
        ExitGarage: translateText('player2', 'Выйти из гаража'),
        EnterHouse: [
            {
                text: translateText('player2', 'Войти в дом'),
                key: 9
            },
            {
                text: translateText('player2', 'Открыть/закрыть дом'),
                key: 39
            }
        ],
        ExitHouse: translateText('player2', 'Выйти из дома'),
        HealkitHouse: translateText('player2', 'Использовать аптечку'),
        EnterHotel: translateText('player2', 'Войти в отель'),
        ExitHotel: translateText('player2', 'Выйти из отеля'),
        CarRentHotel: translateText('player2', 'Арендовать скутер'),
        FractionCityhallBeginWorkDay: translateText('player2', 'Раздевалка CityHall'),
        FractionCityhallGunMenu: translateText('player2', 'Оружейный склад CityHall'),
        JobGoPosta: translateText('player2', 'Начать работу почтальона'),
        JobGoPostaCar: translateText('player2', 'Взять рабочий транспорт'),
        TakeMoney: translateText('player2', 'Взять мешок с деньгами'),
        ChangeAutoNumber: translateText('player2', 'Открыть меню смены номеров'),
        ElectionPoint: translateText('player2', 'Открыть меню выборов'),
        Bear: translateText('player2', 'Нажмите чтобы поднять мишку'),

        BoatFix: translateText('player2', 'Нажмите чтобы починить лодку'),

        buyMetro: translateText('player2', 'Купить билет'),
        exitMetro: translateText('player2', 'Для выхода'),
        Seating: translateText('player2', 'Чтобы сесть'),
        SeatingUp: translateText('player2', 'Встать'),
    }

    let visible = false;
    window.hudEnter = (text) => {
        if (text === -1)
            visible = false;
        else
            visible = InfoData[text] ? InfoData[text] : translateText('player2', 'Нажмите для взаимодействия');
    }
</script>
{#if visible}
    {#if typeof visible == "string"}
    <div class="box-flex box-center">
        <div class="buttonsinfo__button mr-6">{keysName[$keys[9]]}</div>
        <div class="hudevo-usebutton__text">{visible}</div>
    </div>
    {:else}    
        <div class="box-column box-center hudevo__enters">
            {#each visible as item}            
                <div class="hudevo__enter mb-5">
                    <div class="buttonsinfo__button mr-6">{keysName[$keys[item.key]]}</div>
                    <div class="hudevo-usebutton__text">{item.text}</div>
                </div>
            {/each}
        </div>
    {/if}
{/if}