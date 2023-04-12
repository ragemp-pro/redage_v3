<script>
    import { executeClient } from 'api/rage'
    import axios from 'axios';
    import qs from 'querystring';
    import { accountLogin } from 'store/account'
    import { serverDonatMultiplier, serverId } from 'store/server'

    let
        donateUrl = false,
        donateToggled = true,
        donateText = "1",
        donateSelect = "unitpay";
    export let popupData;

	const onHandleInput = (value) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 0) value = 0;
        else if (value > 999999) value = 999999;

        donateText = value;
    }

    const onDonate = () => {
        //if (!isLogin(player.login))
        //    return window.showTooltip("#donateInput", 2, "Что пошло не так...");
        if (donateText < 0 || donateText > 999999)
            return window.showTooltip("#donateInput", 2, "Неверный формат");
        donateToggled = false;
        const config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }

        axios.post('https://pay.redage.net/', qs.stringify({
            name: $accountLogin,
            sum: Math.round(donateText),
            srv: Math.round($serverId),
            type: donateSelect
        }), config)
        .then(function (response) {
            response = response.data;
            if(response.status === 'success') {
                executeClient ("client.opendonatesite", response.url);
            } else if(response.status == 'error') {
                window.notificationAdd(4, 9, response.msg, 3000);
                donateUrl = false;
                donateToggled = true;
            }
        });
    }

    if (popupData && popupData > 0) {
        donateText = String (popupData);
        donateToggled = false;
        onDonate ();
    }

    const getDonate = (text) => {
        if (text < 0) text = 0;
        else if (text > 999999) text = 999999;
        let tallage = 0;
        if ($serverDonatMultiplier > 1) {
            text = text * $serverDonatMultiplier;
        } else {
            if (text >= 20000) {
                tallage = 50;
            } else if (text >= 10000) {
                tallage = 30;
            } else if (text >= 3000) {
                tallage = 20;
            } else if (text >= 1000) {
                tallage = 10;
            }
        }

        return `${Math.round(text) + Math.round(text / 100 * tallage)}`;
    }
</script>
{#if donateToggled}
<div class="newdonate__payment">
    <div class="newdonate__payment-block" on:mouseenter on:mouseleave>
        <div class="newdonate__payment-img"/>
        <div class="newdonate__payment-title">Пополнение RB</div>
        <input class="newdonate__input margin-bottom-22" placeholder="Сумма" type="text" bind:value={donateText} on:input={(event) => onHandleInput (event.target.value)}>
        <div class="newdonate__input">
            <span class="redbucks-small-img"/>
            <div>{getDonate(donateText)} RB</div>
        </div>
        <div class="newdonate__payment-description">Вы получите</div>
        <div class="newdonate__payment-subtitle">Выберете платежную систему</div>
        <ul class="newdonate__payment-list">
            <li on:click={() => donateSelect = "unitpay"}><span class="check active" class:active={donateSelect === "unitpay"}/><span>Unitpay</span></li>
            <li on:click={() => donateSelect = "qiwi"}><span class="check" class:active={donateSelect === "qiwi"}/><span>Qiwi</span></li>
        </ul>

        <div class="newdonate__button_small">
            <div class="newdonate__button-text" on:click={onDonate}>Далее</div>
        </div>
    </div>
    <div class="newdonate__escape">
        <div class="box-flex">
            <span class="donateicons-esc"/>
            <div class="newdonate__escape-title">ESC</div>
        </div>
        <div class="newdonate__escape-text">
            Нажми, чтобы закрыть
        </div>
    </div>
</div>
{/if}