<script>
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { charMoney } from 'store/chars'
    import './css/main.sass'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    export let viewData;

    let 
        value = "50",
        bet = 0,
        betWin = 0,
        isBet = viewData.isBet,
        isBtn = viewData.isBtn,
        btnDouble = viewData.btnDouble,
        btnSplit = viewData.btnSplit,
        time = 0,
        btnExit = 1,
        betMax = viewData.betMax;

    const onBet = (type) => {
        if (!isBet) return;
        switch (type) {
            case 0:
                value = value * 2;
                break;
            case 1:
                value = value * 5;
                break;
            case 2:
                value = Math.round (value / 2);
                break;
            case 3:
                value = Math.round (value / 4);
                break;
            case 4:
                value = betMax;
                break;
        }
        if (Number (value) > Number ($charMoney)) value = $charMoney;
        if (Number (value) > Number (betMax)) value = betMax;
    }

    const onSetBet = () => {
        if (!isBet) return;
        else if (Number (value) > Number ($charMoney)) {            
            value = $charMoney;
            window.notificationAdd(4, 9, `У Вас нет столько денег!`, 3000);
            return;
        } else if (Number (value) < 50) {            
            value = 50;
            window.notificationAdd(4, 9, `Минимальная ставка составляет $${format("money", 50)}`, 3000);
            return;
        } else if (Number (value) > Number (betMax)) {            
            value = betMax;
            window.notificationAdd(4, 9, `Максимальная ставка на данном столе составляет $${format("money", betMax)}`, 3000);
            return;
        }
        executeClient ("client.blackjack.setBet", Number (value));
        isBet = false;
        time = 0;
        bet = Number (value);
    }

    const onExit = () => {
        if (!btnExit) return;
        executeClient ("client.blackjack.exit");
        isBtn = false;
        time = 0;
    }

    const onBtn = (text) => {
        if (!isBtn) return;
        executeClient ("client.blackjack.btn", text);
        isBtn = false;
        time = 0;
    }


    const BtnInfo = (_isBet, _isBtn, _btnDouble, _btnSplit) => {
        isBet = _isBet;
        isBtn = _isBtn;
        btnDouble = _btnDouble;
        btnSplit = _btnSplit;
    }

    const Time = (Time) => {
        time = Time;
    }

    window.events.addEvent("cef.blackjack.btn", BtnInfo);
    window.events.addEvent("cef.blackjack.time", Time);
    window.events.addEvent("cef.blackjack.btnExit", (type, debug) => {
        btnExit = type;
    });
    window.events.addEvent("cef.blackjack.bet", (value) => {
        bet = value;
    });
    window.events.addEvent("cef.blackjack.betWin", (value) => {
        betWin = value;
    });

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        window.events.removeEvent("cef.blackjack.btn", BtnInfo);
        window.events.removeEvent("cef.blackjack.time", Time);
        window.events.removeEvent("cef.blackjack.btnExit", (type, debug) => {
            btnExit = type;
        });
        window.events.removeEvent("cef.blackjack.bet", (value) => {
            bet = value;
        });
        window.events.removeEvent("cef.blackjack.betWin", (value) => {
            betWin = value;
        });
    });
	const onHandleInput = (text) => {
        text = Math.round(text.replace(/\D+/g, ""));
        if (text < 1) text = 1;
        else if (text > 999999) text = 999999;

        value = text;
    }
</script>

<div id="casino-main">
    <div class="box-date">
        <div class="box-time">
            <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
            {TimeFormat ($serverDateTime, "DD.MM.YYYY")}
        </div>
    </div>
    <div class="box-KeyInfo">
        <div class="KeyInfo text">~</div>
        {translateText('casino', 'Показать/Скрыть курсор')}
    </div>
    <div class="bet-block">
        <div class="icon-dollar"/>
        <div class="info-area">
            <div class="gray-color">{translateText('casino', 'Баланс')}:</div>
            <div class="balance-money">${format("money", $charMoney)}</div> 
        </div>
        {#if betWin > 0}
        <div class="e-win info-area">                        
            <div class="gray-color">{translateText('casino', 'Выигрыш')}:</div>
            <div class="balance-money">${format("money", betWin)}</div> 
        </div>
        {:else}
        <div class="e-win info-area" />
        {/if}
        <div class="text">BLACKJACK</div>
        <div class="bet-list">
            <div class="bet" on:click={() => onBet (0)}>X2</div>
            <div class="bet" on:click={() => onBet (1)}>X5</div>
            <div class="bet" on:click={() => onBet (2)}>1/2</div>
            <div class="bet" on:click={() => onBet (3)}>1/4</div>
            <div class="bet bet-45" on:click={() => onBet (4)}>Full</div>
        </div>
        <input placeholder="Кол-во для ставки" maxLength="6" class="e-input" bind:value={value} on:input={(event) => onHandleInput (event.target.value)} disabled={!isBet} />
        <div class="bet-button" on:click={onSetBet} style="opacity: {isBet ? 1 : '0.65'}">{translateText('casino', 'Сделать ставку')}</div>
        <div class="bet-button black" on:click={onExit} style="opacity: {btnExit ? 1 : '0.65'}">{translateText('casino', 'Выйти')}</div>
        <div class="bottom-line"/>
    </div>
    <div class="info-block">
        <div class="bottom-line line-top"/>
        <div class="bet-money">
            <div class="icon-dollar"/>
            <div class="info-area">
                <div class="gray-color">{translateText('casino', 'Размер ставки')}:</div>
                <div class="balance-money">${format("money", bet)}</div> 
            </div>
        </div>
        {#if time > 0}
        <div class="info-area">
            <div class="gray-color">{translateText('casino', 'Время')}:</div>
            <div class="balance-money margin-5">{time} {translateText('casino', 'секунд')}.</div>
        </div>
        {:else}
        <div class="info-area" />
        {/if}
        {#if isBtn === 1}
        <div class="info-buttons">
            <div class="bet-button" on:click={() => onBtn ("hit")}>{translateText('casino', 'Взять')}</div>
            <div class="bet-button" on:click={() => onBtn ("stand")}>{translateText('casino', 'Оставить')}</div>
            {#if btnDouble != 0 && bet <= $charMoney}<div class="bet-button" on:click={() => onBtn ("double")}>{translateText('casino', 'Удвоить')}</div>{/if}
            {#if btnSplit != 0 && bet <= $charMoney}<div class="bet-button" on:click={() => onBtn ("split")}>{translateText('casino', 'Разделить')}</div>{/if}
        </div>
        {/if}
    </div>
</div>