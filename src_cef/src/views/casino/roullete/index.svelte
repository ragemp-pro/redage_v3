<script>
    import './css/main.sass'
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'

    import { charMoney } from 'store/chars'
    export let viewData;
    let
        bet = viewData,
        betWin = 0,
        time = 0;
    
    window.events.addEvent("cef.roullete.time", (value) => {
        time = value;
    });
    window.events.addEvent("cef.roullete.bet", (value) => {
        bet = value;
    });
    window.events.addEvent("cef.roullete.betWin", (value) => {
        betWin = value;
    });
    import { onDestroy } from 'svelte'
    onDestroy(() => {
        window.events.removeEvent("cef.roullete.time", (value) => {
            time = value;
        });
        window.events.removeEvent("cef.roullete.bet", (value) => {
            bet = value;
        });
        window.events.removeEvent("cef.roullete.betWin", (value) => {
            betWin = value;
        });
    });
</script>
<main class="display-area" id="roulette">
    <div class="box-date">
        <div class="box-time">
            <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
            {TimeFormat ($serverDateTime, "DD.MM.YYYY")}
        </div>
    </div>
    <div class="header animated fadeInDown">
        <div class="infoButton">
            <ul class="button">
                <li><div class="btn">{translateText('casino', 'ЛКМ')}</div><span>{translateText('casino', 'Поставить')}</span></li>
                <li><div class="btn">{translateText('casino', 'ПКМ')}</div><span>{translateText('casino', 'Убрать')}</span></li>
                <li><div class="btn">⟵</div><span>{translateText('casino', 'Уменьшить ставку')}</span></li>
                <li><div class="btn">⟶</div><span>{translateText('casino', 'Увеличить ставку')}</span></li>
                <li><div class="btn">H</div><span>{translateText('casino', 'Камера')}</span></li>
            </ul>
        </div>
        <div class="timer">
            <div class="icon"><span class="iconTime"/></div>
            <div class="md">
                <div class="label">{translateText('casino', 'ОСТАЛОСЬ ДО')} <span class="red">{translateText('casino', 'НАЧАЛА')}</span></div>
                <div class="time">00:{time < 10 ? "0" + time : time}</div>
            </div>
        </div>
    </div>

    <div class="footer animated fadeInUp">
            
        <ul class="infoStats">
            <li><span class="ics iconDollar"/> <div class="text">{translateText('casino', 'Баланс')} : <b>${format("money", $charMoney)}</b></div></li>
            <li><span class="ics iconCircles"/> <div class="text">{translateText('casino', 'Ставка')} : <b>${format("money", bet)}</b></div></li>
            {#if betWin}
                <li>
                    <span class="ics iconProfit"/>
                    <div class="text">{translateText('casino', 'Выигрыш')} : <b>${format("money", betWin)}</b></div>
                </li>
            {/if}
        </ul>
    </div>


</main>