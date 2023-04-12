<script>
    import { translateText } from 'lang'
    import {executeClient, executeClientAsync} from 'api/rage'
    import { selectCase } from './state.js'
    import { format } from 'api/formatter'
    import { accountUnique } from 'store/account'

    export let SetView;



    const onOpenCase = (index) => {
        selectCase.set (index);
        SetView("Roulette")
    }

    let selectIndex = 0;
    let caseData = [];
    const onSelectCases = (index) => {
        selectIndex = index;
        caseData = [];
        shopList[selectIndex].cases.forEach((index) => {
            const cs = casesData.find(c => c.index === index);
            if (cs)
                caseData.push(cs);
        })
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


    let shopList = [];
    let casesData = [];

    executeClientAsync("donate.roulette.getList").then((result) => {
        if (result && typeof result === "string") {
            result = JSON.parse(result);

            shopList = result.shopList;
            casesData = result.caseData;
            onSelectCases(0);
            isLoad = true;
        }
    });

    let isLoad = false;

</script>

{#if isLoad}
<div id="newdonate__shop">
    <div class="shop-elements">
        {#each caseData as item}
            <div class="shop-element" on:click={() => onOpenCase (item.index)}>
                <div class="star-img" style="background-image: url({document.cloud + `img/roulette/${item.image}.png`})" />
                <div class="shop-element__info">
                    <!--<div class="shop-element__condition">До 3 уровня</div>-->
                    <div class="shop-element__title">{item.name}</div>
                    <div class="shop-element__paragraph">{item.desc}</div>
                    <div class="shop-element__button-box">
                        <div class="newdonate__button_small shop-element__button">
                            <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", getPrice (item.price, item.index, $accountUnique))} RB</div>
                        </div>

                    </div>
                </div>
            </div>
        {/each}
    </div>
    <div class="shop-categorie">
        {#each shopList as item, index}        
        <div class="shop-categorie__element" class:active={selectIndex === index} on:click={() => onSelectCases (index)}>
            <div class="shop-categorie__info">
                <div class="shop-categorie__checkbox">
                    <div class="shop-categorie__checkbox_active"/>
                </div>
                <div class="shop-element__title">{item.title}</div>
            </div>
            <div class="star-img" style="background-image: url({document.cloud + `img/roulette/${item.image}.png`})"/>
        </div>
        {/each}
    </div>
</div>
{/if}