<script>
    import { translateText } from 'lang'
    import axios from 'axios';
    import qs from 'querystring';
    import { executeClient } from 'api/rage'
    import { accountLogin } from 'store/account'
    import { serverDonatMultiplier, serverId } from 'store/server'
    import './main.sass'

    let
        donateUrl = false,
        donateToggled = true,
        donateText = "1";

    export let popupData;

	const onHandleInput = (value) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 1) value = 1;
        else if (value > 999999) value = 999999;

        donateText = value;
    }

    const onDonate = () => {
        //if (!isLogin(player.login))
        //    return window.showTooltip("#donateInput", 2, "Что пошло не так...");
        //else if (!isNumeric(textValue))
        //    return window.showTooltip("#donateInput", 2, "Неверный формат");
        
        donateToggled = false;
        const config = {
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            }
        }

        axios.post('https://pay.redage.net/', qs.stringify({
            name: $accountLogin,
            sum: Math.round(donateText),
            srv: Math.round($serverId)
        }), config)
        .then(function (response) {
            response = response.data;
            if(response.status === 'success') {
                donateUrl = true;                
                executeClient ("client.opendonatesite", response.url);
            } else if(response.status == 'error') {
                window.notificationAdd(4, 9, response.msg, 3000);
                window.router.setPopUp ("")
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
    
    const onKeyDown = (event) => {
        if (!donateUrl) return;
        if (event.which === 27) {
            window.router.setPopUp ("");
        }
    }
</script>
<svelte:window on:keydown={onKeyDown} />
{#if donateUrl}
    <!--<iframe sandbox="allow-same-origin || allow-forms || allow-scripts" src={donateUrl} scrolling="auto" frameborder="0" class="donate-iframe" title="" />-->
    <div class="donate-close" on:click={() => {
        window.router.setPopUp ("");
    }}>X</div>
{/if}
{#if donateToggled}
<div id="donatepopup">
    <div class="donatepopup__back"></div>
    {#if false}
    <div class="donatepopup__main">
        <div class="popup__title">{translateText('popups', 'Пополнение счета')}</div>
        <div class="popup__subtitle">{translateText('popups', 'Пополняйте RB-счёт прямо из игры!  За RedBucks можно сменить ник, снять варн, купить премиум-одежду, покрутить кейсов, купить подписку, вип-статус, обменять на игровую валюту и сделать еще много всего интересного. Спасибо тебе!')}</div>
        <input type="text" class="popup__input" placeholder="Введите сумму" bind:value={donateText} on:input={(event) => onHandleInput (event.target.value)}>
        <div class="popup__input">{translateText('popups', 'Вы получите')} {getDonate(donateText)} RB</div>
        <div class="box-between">
            <div class="popup__button big orange" on:click={onDonate}>{translateText('popups', 'Пополнить счет')}</div>
            <div class="popup__button" on:click={() => window.router.setPopUp ("")}>{translateText('popups', 'Назад')}</div>
        </div>
    </div>
    {:else}
        <div class="donatepopup__payments">
            <div class="box-between">
                <div class="box-column">
                    <div class="donatepopup__nickname">Sokolyansky</div>
                    <div class="donatepopup__small">{translateText('popups', 'Пополнение счета')}</div>
                    <div class="donatepopup__count">200<span class="red"> RB</span></div>
                    <div class="donatepopup__button"><span class="gray">{translateText('popups', 'К оплате')}:</span>20000 {translateText('popups', 'руб')}.</div>
                </div>
                <div class="donatepopup__logo"></div>
            </div>
            <div class="donatepopup__payments_title">{translateText('popups', 'Выберите способ оплаты')}</div>
            <div class="donatepopup__grid">
                <div class="donatepopup__element">
                    <div class="donatepopup__element_img" style="background-image: url('{document.cloud}img/roulette/items_5.png')"></div>
                </div>
                <div class="donatepopup__element">
                    <div class="donatepopup__element_img" style="background-image: url('{document.cloud}img/roulette/items_0.png')"></div>
                </div>
            </div>
            <div class="box-between popup__button_box">
                <div class="popup__button big orange" on:click={onDonate}>{translateText('popups', 'Подтвердить')}</div>
                <div class="popup__button" on:click={() => window.router.setPopUp ("")}>{translateText('popups', 'Назад')}</div>
            </div>
        </div>
    {/if}

</div>
{/if}