<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    export let popupData;
    export let SetPopup;
    import { accountSubscribe } from 'store/account'

    const onBuy = () => {
        SetPopup ()
        executeClient ("client.donate.buy.premium", popupData.id);
    }

    const getPrice = (price, index, unique) => {
        if (!unique) {
            if (0 === index) {
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
            <div class="newdonate__special-sunduk h-premium" style="background-image: url({document.cloud + `donate/premium/${popupData.img}.svg`})" />
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
                <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {getPrice (popupData.price, popupData.id, $accountSubscribe)} RB</div>
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