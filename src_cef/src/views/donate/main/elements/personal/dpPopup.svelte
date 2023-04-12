<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { executeClient } from 'api/rage'
    import { accountRedbucks, accountUnique } from 'store/account'
    export let popupData;
    export let SetPopup;

    const onBuy = () => {
        if ($accountRedbucks < getPrice (popupData.price, popupData.id, $accountUnique))
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);

        SetPopup ()
        executeClient ("client.donate.buy.set", popupData.id);
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
</script>
<div class="newdonate__special">
    <div class="newdonate__special-block" on:mouseenter on:mouseleave>
        <div class="newdonate__specialblock-left">
            <div class="newdonate__special-mainimg" />
            <div class="newdonate__special-sunduk" style="background-image: url({document.cloud + `donate/personal/${popupData.id + 1}.png`})" />
        </div>
        <div class="newdonate__special-info">
            <div class="newdonate__special-title">{popupData.name}</div>
            <div class="newdonate__special-gray">{translateText('donate', 'Что входит в набор')}?</div>
            <div class="newdonate__special-nabor">
                {#each popupData.list as text, index}
                    <div class="newdonate__special-text"><b>- </b>{text}</div>
                {/each} 
            </div>
            <div class="newdonate__button_small" on:click={onBuy}>
                <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", getPrice (popupData.price, popupData.id, $accountUnique))} RB</div>
            </div>
        </div>
    </div>
    <div class="newdonate__escape">
        <div class="box-flex">
            <span class="donateicons-esc"/>
            <div class="newdonate__escape-title">ESC</div>
        </div>
        <div class="newdonate__escape-text">
            {translateText('donate', 'Нажми, чтобы закрыть')}
        </div>
    </div>
</div>