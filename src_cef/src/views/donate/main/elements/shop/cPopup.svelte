<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { executeClient } from 'api/rage'
    import { accountRedbucks } from 'store/account'
    export let popupData;
    export let SetPopup;

    const onBuy = () => {
        if ($accountRedbucks < popupData.price)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        SetPopup ()
        executeClient ("client.donate.buy.clothes", popupData.id);
    }
</script>
<div class="newdonate__special">
    <div class="newdonate__special-block" on:mouseenter on:mouseleave>
        <div class="newdonate__specialblock-left">
            <div class="newdonate__special-mainimg" />
            <div class="newdonate__special-sunduk" style="background-image: url({popupData.img})" />
        </div>
        <div class="newdonate__special-info">
            <div class="newdonate__special-title">{popupData.name}</div>
            <div class="newdonate__special-gray">{translateText('donate', 'Что входит в набор')}?</div>
            <div class="newdonate__special-nabor">            
                <div class="newdonate__special-text">{popupData.text}</div>
            </div>
            <div class="newdonate__button_small" on:click={onBuy}>
                <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", popupData.price)} RB</div>
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