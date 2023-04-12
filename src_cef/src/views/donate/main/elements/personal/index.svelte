<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { accountUnique } from 'store/account'

    export let SetPopup;

    const onToServer = (item) => {
        SetPopup ("PopupDpPopup", item);
    }

    const getPrice = (price, index, unique) => {
        if (unique && unique.split("_")) {
            let getData = unique.split("_");
            if (getData[0] === "packages" && Number (getData[1]) === index && Number (getData[2]) === 0) {
                price = Math.round (price * 0.7);
            }
        }
        return price;
    }

    let isLoad = false;

    import { executeClientAsync } from 'api/rage'

    let shopList = [];
    executeClientAsync("donate.getPack").then((result) => {
        if (result && typeof result === "string") {
            shopList = JSON.parse(result);

            isLoad = true;
        }
    });
</script>

{#if isLoad}
<div id="newdonate__donatelist">
    <div class="donatelist-elements">
        {#each shopList as item, index}
        <div class="donatelist-element">
            <div class="star-img" style="background-image: url({document.cloud + `donate/personal/${item.id + 1}.png`})" />
            <div class="donatelist-element__info">
                <div class="donatelist-element__title">{item.name}</div>
                <div class="donatelist-element__paragraph">{item.desc}</div>
                <div class="donatelist-element__button-box">
                    <div class="newdonate__button_small donatelist-element__button" on:click={() => onToServer (item)}>
                        <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", getPrice (item.price, index, $accountUnique))} RB</div>
                    </div>
                </div>
            </div>
        </div>
        {/each}
    </div>
</div>
{/if}