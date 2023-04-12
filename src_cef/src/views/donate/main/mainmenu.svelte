<script>
    import { translateText } from 'lang'
    export let SetView;
    export let SetPopup;

    import ImgShop from './assets/images/menu/shop.png'
    import ImgPackages from './assets/images/menu/packages.png'
    import ImgCases from './assets/images/menu/cases.png'
    import ImgPremium from './assets/images/menu/premium.png'

    import Unique from './bonus/unique.svelte';
    import UniqueNew from './bonus/uniquenew.svelte';
</script>
<div id="newdonate__main-menu">
    <div class="main-menu__header-elements">
        <UniqueNew {SetPopup} />
        <Unique {SetView} {SetPopup} />
        <!--<div class="main-menu__header-element main-menu__header-element_large">
            <div class="main-menu__slider">
                <div class="main-menu__slider-line active"/>
                <div class="main-menu__slider-line"/>
                <div class="main-menu__slider-line"/>
            </div>
            <div class="main-menu__element-info">
                <div class="main-menu__timer-box">
                    <div>Ограничено до</div>
                    <div class="main-menu__timer box-center">12.23.34</div>
                </div>
                <div class="main-menu__title_big">Личное</div>
                <div class="main-menu__subtitle">Предложение для вас</div>
                <div class="main-menu__paragraph">Lorem ipsum dolor sit amet, consectetur adipisicing elit. Unde sed eveniet ipsum voluptatum! Eius sequi quos reprehender.</div>
                <div class="newdonate__button_small">
                    <div class="newdonate__button-text">Подробнее</div>
                </div>
                <div class="newdonate__ogran">Данный товар можно купить 1 раз</div>
            </div>
            <div class="main-menu__star-block">
                <div class="star-img"/>
            </div>
        </div>-->
    </div>
    <div class="main-menu__main-elements">
        <div class="main-menu__main-element" on:click={() => SetView("Shop")}>
            <div class="main-menu__main-element-img star-img" style="background-image: url({ImgShop})" />
            <div class="main-menu__main-element-text">{translateText('donate', 'Магазин')}</div>
        </div>
        <div class="main-menu__main-element" on:click={() => SetView("Personal")}>
            <div class="main-menu__main-element-img star-img" style="background-image: url({ImgPackages})" />
            <div class="main-menu__main-element-text">{translateText('donate', 'Донат-пакеты')}</div>
        </div>
        <div class="main-menu__main-element" on:click={() => SetView("Cases")}>
            <div class="main-menu__main-element-img star-img" style="background-image: url({ImgCases})" />
            <div class="main-menu__main-element-text">{translateText('donate', 'Кейсы')}</div>
        </div>
        <div class="main-menu__main-element" on:click={() => SetView("Premium")}>
            <div class="main-menu__main-element-img star-img" style="background-image: url({ImgPremium})" />
            <div class="main-menu__main-element-text">{translateText('donate', 'Подписки и премиум')}</div>
        </div>
    </div>
</div>