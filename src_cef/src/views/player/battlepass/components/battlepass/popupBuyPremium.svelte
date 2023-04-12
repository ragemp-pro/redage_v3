<script>
    import { translateText } from 'lang'
    import { isPopupBuyOpened } from '../../stores.js'
    import { format } from 'api/formatter'
    import { accountRedbucks } from 'store/account'
    import { executeClientToGroup, executeClientAsyncToGroup} from 'api/rage'

    const pricePremium = 19999;

    let isPremium = 0;
    executeClientAsyncToGroup("getPremium").then((result) => {
        isPremium = result;
    });

    const onBuyPremium = () => {
        if (isPremium)
            return;
        else if (!window.loaderData.delay ("battlePass.onBuyPremium", 1))
            return;

        executeClientToGroup ("buyPremium");

        onClose ();
    }

    const onClose = () => isPopupBuyOpened.set(false);
</script>

<div class="battlepass__popup_info popupback">
    <div class="newproject__buttonblock" on:click={onClose}>
        <div class="newproject__button">ESC</div>
        <div>{translateText('player', 'Закрыть')}</div>
    </div>
    <div class="battlepass__popup_maintitle">{translateText('player', 'Разблокировать боевой пропуск')}</div>
    <div class="battlepass__popup_buytext">
        {translateText('player', 'Приобретая Премиум-пропуск, вы так же приобретаете возможность получить кучу уникальных призов в дополнение к обычному пропуску.')}
        <br>
        <br>
        {translateText('player', 'Призы, полученные с Премиум-пропуска можно забрать с почты. Купить Премиум-пропуск можно в любой период сезона. Каждый сезон данная опция обнуляется.')}
    </div>
    <div class="box-flex">
        <div class="battlepass__popup_pricetext">{translateText('player', 'Покупка БП')}</div>
        <div class="battlepass__button" on:click={onBuyPremium}>{translateText('player', 'РАЗБЛОКИРОВАТЬ')}</div>
    </div>
    <div class="battlepass__popup_heroes"></div>
</div>