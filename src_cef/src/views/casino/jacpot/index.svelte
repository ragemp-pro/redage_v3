<script>
    import './css/main.sass'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { translateText } from 'lang'
    import { charMoney } from 'store/chars'
    import { onMount } from 'svelte'
    import Logos from './logo.svg';

    let
        animateClass = ['animated fadeInDown', 'animated fadeInUp', 'animated fadeInDown'],
        value = "250",
        bet = 0,
        betWin = 0,
        isBet = true,
        btnExit = true,
        betMax = 10000;

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
        } else if (Number (value) < 25) {            
            value = 25;
            window.notificationAdd(4, 9, `Минимальная ставка составляет $${format("money", 25)}`, 3000);
            return;
        } else if (Number (value) > Number (betMax)) {            
            value = betMax;
            window.notificationAdd(4, 9, `Максимальная ставка на данном столе составляет $${format("money", betMax)}`, 3000);
            return;
        }
        executeClient ("client.spin.setBet", Number (value));
        isBet = false;
        bet = Number (value);
        btnExit = false;
    }

    const onExit = () => {
        if (!btnExit) return;
        executeClient ("client.spin.exit");
    }

    window.events.addEvent("cef.spin.btnExit", (type) => {
        isBet = type;
        btnExit = type;
    });
    
    import { onDestroy } from 'svelte'
    onDestroy(() => {
        window.events.removeEvent("cef.spin.btnExit", (type) => {
            isBet = type;
            btnExit = type;
        });
    });

    onMount(() => {
        setTimeout(() => {
            animateClass = ['animated heartBeat', 'animated heartBeat', 'animated heartBeat'];
        }, 2000);
    });
	const onHandleInput = (text) => {
        text = Math.round(text.replace(/\D+/g, ""));
        if (text < 1) text = 1;
        else if (text > 99999) text = 99999;

        value = text;
    }
</script>

<main class="display-area" id="jacpot">
    <div class="leftColumn">
        <div class="logo">
        <img src={Logos} alt="" />
        <div class="textLg animated flipInX">JACKPOT</div>

        <ul class="gamePlay">
            <li class={animateClass[0]}>7</li>
            <li class={animateClass[1]}>7</li>
            <li class={animateClass[2]}>7</li>
        </ul>

        </div>
        <ul class="slots green animated fadeInLeft" style="width: 100%">

            <li>
                <span class="badge">X2</span>
                {translateText('casino', 'Два одинаковых изображения')}
            </li>
        </ul>
        <ul class="slots yellow animated fadeInLeft" style="width: 100%">

            <li>
                <span class="badge">X4</span>
                {translateText('casino', 'Две семёрки')}
            </li>
        </ul>


        <ul class="slots purple animated fadeInLeft" style="width: 100%">

            <li>
                <span class="badge">X6</span>
                {translateText('casino', 'Три одинаковых изображения')}
            </li>
        </ul>
        <ul class="slots red animated fadeInLeft" style="width: 100%">

            <li>
                <span class="badge">X10</span>
                {translateText('casino', 'Три семёрки')}
            </li>
        </ul>
    </div>


    <div class="rightColumn animated fadeInRight">
        <div class="userBlock">
            <div class="dollarImg"/>
            <div class="text">{translateText('casino', 'Баланс')} : <span>${format("money", $charMoney)}</span></div>
        </div>


        <div class="rate">
            <div class="label">{translateText('casino', 'СДЕЛАЛ СТАВКУ')}</div>
            <ul class="rateNum">
                <li on:click={() => onBet (0)}>X2</li>
                <li on:click={() => onBet (1)}>X5</li>
                <li on:click={() => onBet (2)}>1/2</li>
                <li on:click={() => onBet (3)}>1/4</li>
                <li on:click={() => onBet (4)}>FULL</li>
            </ul>

            <input type="text" bind:value={value} on:input={(event) => onHandleInput (event.target.value)} placeholder="Кол-во для ставки" maxLength="5" class="inputRate" />
            <div class="button" on:click={onSetBet} style="opacity: {isBet ? 1 : '0.65'}">{translateText('casino', 'Сделать ставку')}</div>
            <div class="button black" on:click={onExit} style="opacity: {btnExit ? 1 : '0.65'}">{translateText('casino', 'Выйти')}</div>
        </div>
    </div>

</main>