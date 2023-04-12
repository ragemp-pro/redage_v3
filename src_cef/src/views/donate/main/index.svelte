<script>
    import { translateText } from 'lang'
    import './assets/main.sass';
	import { fade } from 'svelte/transition';
    import { accountRedbucks } from 'store/account'
    import { serverDateTime, serverDonatMultiplier } from 'store/server'
    import { format } from 'api/formatter'
    import { executeClient } from 'api/rage'
    import { TimeFormat } from 'api/moment'
    import MainMenu from './mainmenu.svelte';
    import Shop from './elements/shop/index.svelte';
    import Personal from './elements/personal/index.svelte';

    import Roulette from './elements/roulette/index.svelte';
    import Cases from './elements/roulette/list.svelte';

    import Premium from './elements/premium/index.svelte';

    let Views = {
        MainMenu,
        Shop,
        Personal,
        Roulette,
        Cases,
        Premium,
    }
    let SelectView = "MainMenu";

    const SetView = (view) => {
        SelectView = view;
    }

    import PopupChange from './popup/popupchange.svelte';
    import PopupPremium from './elements/premium/popup.svelte';
    import PopupPayment from './popup/popuppayment.svelte';
    import PopupDpPopup from './elements/personal/dpPopup.svelte';
    import PopupPPopup from './elements/shop/pPopup.svelte';
    import PopupCPopup from './elements/shop/cPopup.svelte';
    import PopupNomer from './popup/popupnomer.svelte';
    import PopupSim from './popup/popupsim.svelte';
    import PopupInfo from './popup/popupinfo.svelte';

    let Popups = {
        PopupChange,
        PopupPremium,
        PopupDpPopup,
        PopupPPopup,
        PopupInfo,
        PopupSim,
        PopupPayment,
        PopupCPopup,
        PopupNomer
    }

    let SelectPopup = "";
    let SelectPopupData = null;
    let AreaPopup = false;

    const SetPopup = (popup = null, data = null) => {
        if (popup === -1 && AreaPopup)
            return;
        SelectPopupData = data;
        SelectPopup = popup;
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;
        if (Popups[SelectPopup]) {
            SelectPopup = "";
            SelectPopupData = null;
        } else {
            if (SelectView === "MainMenu") {
                executeClient ("client.donate.close");
            }
            else SelectView = "MainMenu";
        }
    }

    let isLoad = false;
    const getData = () => {
        isLoad = true;
    }

    executeClient ("client.donate.load");
    import { addListernEvent } from 'api/functions';
    addListernEvent ("donate.init", getData)
</script>

<svelte:window on:keyup={HandleKeyDown} />

{#if isLoad}
    {#if Popups[SelectPopup]}
    <div id="newdonate__popup" class:active={Popups[SelectPopup]} on:click={() => SetPopup (-1)} in:fade={{duration: 250}} out:fade={{duration: 200}}>
        <svelte:component this={Popups[SelectPopup]} {SetPopup} popupData={SelectPopupData} on:mouseenter={e => AreaPopup = true} on:mouseleave={e => AreaPopup = false} />
    </div>
    {/if}

    <div id="newdonate" class:popupOpened={Popups[SelectPopup]}>
        <div class="box-date">
            <div class="box-time">
                <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
                {TimeFormat ($serverDateTime, "DD.MM.YYYY")}
            </div>
        </div>
        <div class="newdonate__header">
            <div class="newdonate__header-left">
                <div class="header__title-block">
                    <div class="shop-img"/>
                    <div class="newdonate__title">{translateText('donate', 'Донат-меню')}</div>
                    <div class="back-button" class:active={SelectView !== "MainMenu"} on:click={() => SetView("MainMenu")}>
                        <span class="back-img"/>
                        <div>{translateText('donate', 'Назад')}</div>
                    </div>
                </div>
                <div class="newdonate__subtitle">
                    {translateText('donate', 'Добро пожаловать! На этой странице представлены к приобретению за')}<b>RB</b>:{translateText('donate', 'кейсы, подписки, VIP-статусы, донат-пакеты, редкие вещи, смена имени/внешности, конвертация валюты и другие интересности')};
                </div>
            </div>
            <div class="newdonate__header-right">
                <div class="newdonate__header-coins">
                    <div class="newdonate__header-coin" on:click={() => SelectPopup = "PopupInfo" }>
                        <div class="header-coin__top">
                            <div class="newdonate__coin redbucks"/>
                            <div class="newdonate__count">{format("money", $accountRedbucks)}</div>
                        </div>
                        <div class="newdonate__header-info">{translateText('donate', 'Подробнее')}</div>
                    </div>
                    <!--<div class="newdonate__header-coin">
                        <div class="header-coin__top">
                            <div class="newdonate__coin bitcoin"/>
                            <div class="newdonate__count">0</div>
                        </div>
                        <div class="newdonate__header-info" on:click={() => SelectPopup = "PopupPrise" }>Подробнее</div>
                    </div>-->
                </div>
               <div class="newdonate__button">
                    <div class="newdonate__button-main">
                        <div class="newdonate__button-text">Донат в lk.redage.net</div>
                        {#if $serverDonatMultiplier > 1}<div class="newdonate__button-x2">x{$serverDonatMultiplier}</div>{/if}
                    </div>
                </div>
            </div>
        </div>
        <svelte:component this={Views[SelectView]} {SetView} {SetPopup} isPopup={Popups[SelectPopup]} />
    </div>
{/if}