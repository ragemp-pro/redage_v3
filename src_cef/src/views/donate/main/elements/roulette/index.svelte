<script>
    import { translateText } from 'lang'
    import { onDestroy } from 'svelte';
    import {executeClient, executeClientAsync} from 'api/rage'
    import { selectCase } from './state.js'
    import { format } from 'api/formatter'
    import { accountRedbucks, accountUnique } from 'store/account'
    
    let isLoad = false;
    let caseData = {}
    executeClientAsync("donate.roulette.getCase", $selectCase).then((result) => {
        if (result && typeof result === "string") {
            caseData = JSON.parse(result);
            isLoad = true;
        }
    });
    
    let antiFlud = 0;

    let currentCount = 1;
    let toggledFast = false;

    const onOpen = (_toggledFast = false) => {
        if (antiFlud > new Date().getTime())
            return;
        else if ($accountRedbucks < getPrice (caseData.price * currentCount, caseData.index, $accountUnique))
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        antiFlud = new Date().getTime() + 2500;
        toggledFast = _toggledFast;
        executeClient ("client.roullete.buy", caseData.index, currentCount);
    }

    const getPrice = (price, index, unique) => {
        if (unique && unique.split("_")) {
            let getData = unique.split("_");
            if (getData[0] === "cases" && Number (getData[1]) === index && Number (getData[2]) === 0) {
                price = Math.round (price * 0.7);
            }
        }
        return price;
    }

    const onCurrentCount = (count) => {
        if (antiFlud > new Date().getTime())
            return;
            
        currentCount  = count;
    }

</script>
{#if isLoad}
<div id="newdonate__roulette">
    {#if caseData.price > 0}
    <div class="newdonate__roulette-title">{translateText('donate', 'Количество кейсов')}</div>
    <div class="newdonate__roulette-count">
        <div class="newdonate__roulette-circle" class:active={currentCount === 1} on:click={() => onCurrentCount (1)}>1</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 2} on:click={() => onCurrentCount (2)}>2</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 3} on:click={() => onCurrentCount (3)}>3</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 4} on:click={() => onCurrentCount (4)}>4</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 5} on:click={() => onCurrentCount (5)}>5</div>

        <div class="newdonate__roulette-circle" class:active={currentCount === 10} on:click={() => onCurrentCount (10)}>10</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 25} on:click={() => onCurrentCount (25)}>25</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 50} on:click={() => onCurrentCount (50)}>50</div>
        <div class="newdonate__roulette-circle" class:active={currentCount === 100} on:click={() => onCurrentCount (100)}>100</div>
    </div>
    {/if}
    <div class="newdonate__roulette_hidden-box">
        {#if caseData.price > 0}
        <div class="newdonate__roulette-buttons">
            <div class="newdonate__button_small" on:click={() => onOpen()}>
                <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", getPrice (caseData.price * currentCount, caseData.index, $accountUnique))} RB</div>
            </div>
        </div>
        {/if}
        <div class="newdonate__roulette-info">
            {translateText('donate', 'Что есть в кейсе')}?
        </div>
        <div class="newdonate__roulette-items">        
            {#each caseData.items as value, index}
            <div class="newdonate__roulette-element {value.color}" tooltip={value.title}>
                <div class="roulette-element__img" style="background-image: url({document.cloud + `img/roulette/${value.image}.png`})" />
            </div>
            {/each}
        </div>
    </div>
</div>
{/if}