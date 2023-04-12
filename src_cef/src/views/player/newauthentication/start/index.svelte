<script>
    import { translateText } from 'lang'
	import Authentication from './authentication/index.svelte';
	import Registration from './registration/index.svelte';
	import News from './../news/index.svelte';
    import Restore from './restore/index.svelte';
    import { addListernEvent } from 'api/functions'

    import { isInput, isSend } from '@/views/player/newauthentication/store.js';
    import {accountLogin} from "store/account";

    let PagesSorted = ["News", "Authentication", "Registration", "Restore"];

    const Views = {
        Authentication,
        Registration,
        News,
        Restore
    }
    let SelectViews = "Authentication";

    const OnSelectViews = (view) => {
        if (SelectViews === view)
            return;
        
        SelectViews = view;
    }

    function onClickQ() {
        let index = PagesSorted.findIndex (p => p === SelectViews)
        
        if(--index >= 0) {
            SelectViews = PagesSorted [index];
        }
    }

    function onClickE() {
        let index = PagesSorted.findIndex (p => p === SelectViews)

        if (++index < PagesSorted.length) {
            SelectViews = PagesSorted [index];
        }
    }
    
    const onKeyUp = (event) => {
        if ($isInput)
            return;
        const { keyCode } = event;
        
        if(keyCode == 81) {
            onClickQ ();
        } else if(keyCode == 69) { 
            onClickE ();
        }
    }

    addListernEvent ("isSendEmailMessage", (toggled) => {
        isSend.set (toggled)
    })

    accountLogin.subscribe((value) => {
        if (value == -99) return;
        if (value == -1 || value == "-1") {
            SelectViews = "Registration";
            PagesSorted = ["News", "Registration"];
        }
        else {
            SelectViews = "Authentication";
            PagesSorted = ["News", "Authentication", "Restore"];
        }
    });
</script>
<svelte:window on:keyup={onKeyUp} />
<div id="newauthentication">
    {#if $isSend}
    <div class="newauthentication__popup">
        <div class="newauthentication__popup_title">{translateText('player2', 'Мы отправили письмо вам на почту!')}</div>
        <div class="newauthentication__popup_subtitle">{translateText('player2', 'Подтвердите адрес электронной почты, чтобы перейти к созданию персонажа.')}</div>
        <div class="main__button main__button_mail" on:click={() => isSend.set (false)}>
            {translateText('player2', 'Я перепутал почту..')}
        </div>
        <div class="auth-mail newauthentication__popup_iconmail"></div>
    </div>
    {/if}

    <div class="header">
        <div class="header__logo"/>
        <div class="box-flex">
            <div class="header-key box-center" class:active={false} on:click={onClickQ}>
                Q
            </div>
            <div class="header__element" class:active={SelectViews === "News"} on:click={() => OnSelectViews("News")}>
                {translateText('player2', 'Новости')}
            </div>
            {#if PagesSorted.includes("Authentication")}
            <div class="header__element" class:active={SelectViews === "Authentication"} on:click={() => OnSelectViews("Authentication")}>
                {translateText('player2', 'Авторизация')}
            </div>
            {:else if PagesSorted.includes("Registration")}
            <div class="header__element" class:active={SelectViews === "Registration"} on:click={() => OnSelectViews("Registration")}>
                {translateText('player2', 'Регистрация')}
            </div>
            {/if}
            <div class="header-key box-center" class:active={false} on:click={onClickE}>
                E
            </div>
        </div>
        <div class="header__money" />
    </div>
    <svelte:component this={Views[SelectViews]} OnSelectViews={OnSelectViews}/>
</div>