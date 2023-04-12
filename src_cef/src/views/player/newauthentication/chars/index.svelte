<script>
    import { translateText } from 'lang'
    import {executeClient, executeClientAsync} from 'api/rage'
	import News from './../news/index.svelte';
	import Chars from './chars.svelte';
	import Merger from './merger/index.svelte';
    import { isInput } from '@/views/player/newauthentication/store.js';
    import { accountRedbucks } from 'store/account'
    import { format } from 'api/formatter'
	//import Registration from './registration/index.svelte';
    let PagesSorted = ["News", "Chars"];

    const Views = {
        News,
        Chars,
        Merger,
    }

    let SelectViews = "Chars";


    let isMerger = true;

    const SetMerger = (toggled) => {
        isMerger = toggled;
    }

    const OnSelectViews = (view) => {
        if (!isMerger) 
            return;
        if ($isInput)
            return;
        if (SelectViews === view)
            return;
        
        SelectViews = view;
    }

    function onClickQ() {
        if (!isMerger) 
            return;
        let index = PagesSorted.findIndex (p => p === SelectViews)
        
        if(--index >= 0) {
            SelectViews = PagesSorted [index];
        }
    }

    function onClickE() {
        if (!isMerger) 
            return;
        let index = PagesSorted.findIndex (p => p === SelectViews)

        if (++index < PagesSorted.length) {
            SelectViews = PagesSorted [index];
        }
    }
    
    const onKeyUp = (event) => {
        if (!isMerger) 
            return;
        if ($isInput)
            return;
        const { keyCode } = event;
        
        if(keyCode == 81) {
            onClickQ ();
        } else if(keyCode == 69) { 
            onClickE ();
        }
    }

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }

    let isMergerToggled = false;
    executeClientAsync("isMerger").then((result) => {
        isMergerToggled = result;
        if (isMergerToggled)
            PagesSorted = ["News", "Chars", "Merger"];
        else
            PagesSorted = ["News", "Chars"];
    });
</script>
<svelte:window on:keyup={onKeyUp} />

<div id="newauthentication">
    <div class="header box-center">
        <div class="header__logo"/>
        <div class="box-flex" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
            <div class="header-key box-center" on:click={onClickQ}>
                Q
            </div>
            <div class="header__element" class:active={SelectViews === "News"} on:click={() => OnSelectViews("News")}>
                {translateText('player2', 'Новости')}
            </div>
            <div class="header__element" class:active={SelectViews === "Chars"} on:click={() => OnSelectViews("Chars")}>
                {translateText('player2', 'Авторизация')}
            </div>
            {#if isMergerToggled}
            <div class="header__element" class:active={SelectViews === "Merger"} on:click={() => OnSelectViews("Merger")}>
                {translateText('player2', 'Перенести персонажей с White/Red')}
            </div>
            {/if}
            <div class="header-key box-center" on:click={onClickE}>
                E
            </div>
        </div>
        <div class="header__money">
            {format("money", $accountRedbucks)} <span class="header__money_redbucks"></span>
        </div>
    </div>
    <div class="newauthentification__characters">
        <svelte:component this={Views[SelectViews]} {SetMerger} {isMerger} />
    </div>
</div>