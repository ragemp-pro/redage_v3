<script>
    import { translateText } from 'lang'
    import './main.sass'
    
    let CarStats = {
        name: "Elegy Super FT",
        descr: "Нестареющая классика",
        owner: "Vitaliy Zdobich",
        number: 2281337,
        gosprice: 50000,
        tune: [
            "Двигатель 4 уровень",
            "Тормоза 4 уровень",
            "Колеса 4 уровень",
            "Стёкла 4 уровень",
            "Труба 4 уровень"
        ],
        price: 100000
    }
</script>
<div id="carmarket">
    <div class="carmarket__panel">
        <div class="carmarket__header">{translateText('player', 'Покупка транспорта')}</div>
        <div class="carmarket__mainmenu">
        <div class="carmarket__mainmenu_center left">
            <div class="carmarket__rielt_info left">
                <div class="carmarket__rielt_header">{CarStats.name}</div>
                <div class="carmarket__gray">{CarStats.descr}</div>
                <div class="carmarket__rielt_stat">
                    <div class="carmarket__rielt_element">
                        <div class="carmarket__gray">{translateText('player', 'Владелец')}:</div>
                        <div>{CarStats.owner}</div>
                    </div>
                    <div class="carmarket__rielt_element">
                        <div class="carmarket__gray">{translateText('player', 'Номер телефона')}:</div>
                        <div>{CarStats.number}</div>
                    </div>
                    <div class="carmarket__rielt_element">
                        <div class="carmarket__gray">{translateText('player', 'Гос. цена')}:</div>
                        <div>{CarStats.gosprice}$</div>
                    </div>
                    <div class="carmarket__rielt_element">
                        <div class="carmarket__gray">{translateText('player', 'Тюнинг')}:</div>
                        <div class="box-column">
                            {#each CarStats.tune as detail, index}
                                <div>{detail}</div>
                            {/each}
                        </div>
                    </div>
                </div>
                <div class="box-column">
                    <div class="carmarket__rielt_price">${CarStats.price}</div>
                    <div class="carmarket__gray">{translateText('player', 'Примерная цена с тюнингом')}:</div>
                </div>
            </div>
        </div>
        <div class="carmarket__mainmenu_block">
            
        </div>
    </div>
    <div class="box-between">
        <div class="house_bottom_buttons back">
            <div>{translateText('player', 'Купить')}</div>
            <div class="house_bottom_button">Enter</div>
        </div>
        <div class="house_bottom_buttons esc">
            <div>{translateText('player', 'Выйти')}</div>
            <div class="house_bottom_button">ESC</div>
        </div>
    </div>
    </div>
</div>