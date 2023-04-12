<script>
    import './css/main.sass'
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { charMoney } from 'store/chars'
    import { onMount } from 'svelte'
    let
        animateClass = ['animated fadeInDown', 'animated fadeInUp', 'animated fadeInDown'],
        players = [
            {id: 1, name: 'Sokolyansky', class: 'red'},
            {id: 2, name: 'Maya', class: 'purple'},
            {id: 3, name: 'Source', class: 'blue'},
            {id: 4, name: 'Shaman', class: 'darkblue'},
            {id: 5, name: 'Deluxe', class: 'white'},
            {id: 6, name: 'Mip', class: 'green'}
        ],
        active = 0,
        value = "500",
        bet = 0,
        betWin = 0,
        isBet = true,
        btnExit = true,
        betMax = 50000;

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
        } else if (Number (value) < 500) {            
            value = 500;
            window.notificationAdd(4, 9, `Минимальная ставка составляет $${format("money", 500)}`, 3000);
            return;
        } else if (Number (value) > Number (betMax)) {            
            value = betMax;
            window.notificationAdd(4, 9, `Максимальная ставка на данном столе составляет $${format("money", betMax)}`, 3000);
            return;
        }
        executeClient ("client.horse.setBet", Number (active + 1), Number (value));
        bet = Number (value);
    }

    const onExit = () => {
        //if (!btnExit) return;
        executeClient ("client.horse.exit");
    }

    const setActive = (id) => {
        active = id;
        executeClient ("client.horse.GET_HORSE", Number (id + 1));        
    }

    window.events.addEvent("cef.horse.isBtn", (type) => {
        isBet = type;
    });

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        window.events.removeEvent("cef.horse.isBtn", (type) => {
            isBet = type;
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
        else if (text > 999999) text = 999999;

        value = text;
    }
</script>

<main class="display-area" id="horse">

    <div class="rightColumn animated fadeInLeft">


        <ul class="players">
            {#each players as value, index}
                <li class={value.class} class:active={active === index} on:click={() => setActive(index)}>
                    <div class="image"/>
                    <div class="name">{value.name}</div>
                    <div class="id">{value.id}</div>
                </li>
            {/each}
        </ul>

    </div>

    <div class="fixed animated fadeInRight">
        <div class="userBlock">
            <div class="avatar player {players[active].class}" />
            <div class="text">Баланс : <span>${format("money", $charMoney)}</span></div>
        </div>

        <div class="rate">
            <div class="label">{translateText('casino', 'Сделать ставку')}</div>
            <ul class="rateNum">
                <li on:click={() => onBet (0)}>X2</li>
                <li on:click={() => onBet (1)}>X5</li>
                <li on:click={() => onBet (2)}>1/2</li>
                <li on:click={() => onBet (3)}>1/4</li>
                <li on:click={() => onBet (4)}>FULL</li>
            </ul>

            <input type="text" bind:value={value} on:input={(event) => onHandleInput (event.target.value)} placeholder="Кол-во для ставки" maxLength="6" class="inputRate" />
            <div class="button" on:click={onSetBet} style="opacity: {isBet ? 1 : '0.65'}">{translateText('casino', 'Сделать ставку')}</div>
            <div class="button black" on:click={onExit} style="opacity: {btnExit ? 1 : '0.65'}">{translateText('casino', 'Выйти')}</div>
        </div>
    </div>

</main>