<script>
    import { translateText } from 'lang'
    import { setGroup, executeClientAsyncToGroup } from 'api/rage'
    import { charName, FractionLVL } from 'store/chars';
    import { TimeFormat, GetTime } from 'api/moment'
    import { serverDateTime } from 'store/server'
    setGroup (".advert.");

    let selectedAddId;

    executeClientAsyncToGroup("getSelected").then((result) => {
        if (result){
            selectedAddId = result;
            selectView = "Ads";
        }
    });

    let count = 0;

    const getCount = () => {
        executeClientAsyncToGroup("getAdsCount").then((result) => {
            count = result;
        });
    }
    getCount();


    const onSelectAdvert = (advertID) => {
        if (!window.loaderData.delay ("advert.onSelectAdvert", 1.5))
            return;

        executeClientAsyncToGroup("isAddByID", advertID).then((result) => {
            if (result) {
                selectedAddId = advertID;
                selectView = "Ads";
            }
        });
    }

    const onBackAdvert = () => {
        selectView = "List";
    }
    import './main.sass'
    import './assets/style.css'

    import Gov from './elements/gov.svelte'
    import Ads from './elements/ads/ads.svelte'
    import List from './elements/ads/list.svelte'
    import Live from './elements/live/index.svelte'

    const Views = {
        Gov,
        Ads,
        List,
        Live
    }

    let selectView = "List"
</script>

<div id="weazelnews">
    <div class="box-flex">
        <div class="newbuttons_button">ESC</div>
        <div class="whitecolor">{translateText('fractions', 'Закрыть')}</div>
    </div>
    <div class="weazelnews__planshet">
        <div class="weazelnews__head">
            <div>{TimeFormat ($serverDateTime, "HH:mm DD.MM.YYYY")}</div>
            <div class="weazelnews__head_img"></div>
        </div>
        <div class="box-flex w-100">
            <div class="weazelnews__nav">
                <div class="weazelnews__nav_logo"></div>
                <div class="line"></div>
                <!--<div class="weazelnews__nav_element mt-24" class:active={selectView === "Gov"} on:click={() => selectView = "Gov"}>
                    <span class="bortovoiicon-news"></span>
                    <div class="weazelnews__nav_text">Государственная волна</div>
                </div>-->
                <div class="weazelnews__nav_element mt-24" class:active={selectView === "Ads" || selectView === "List"} on:click={() => selectView = "List"}>
                    <span class="bortovoiicon-list"></span>
                    <div class="weazelnews__nav_text">{translateText('fractions', 'Объявления')} <span class="red">{count}</span></div>
                </div>
                <div class="weazelnews__nav_element" class:active={selectView === "Live"} on:click={() => selectView = "Live"}>
                    <span class="bortovoiicon-call"></span>
                    <div class="weazelnews__nav_text">{translateText('fractions', 'Прямой эфир')}</div>
                </div>
                <div class="line mt-24"></div>
                <div class="line mt-auto"></div>
                <div class="box-column">
                    <div class="weazelnews__name mt-24">{$charName}</div>
                    <div class="weazelnews__rank">{$FractionLVL}</div>
                </div>
                <div class="line mt-24"></div>
                <div class="weazelnews__flag"></div>
            </div>
            <div class="weazelnews__main">
                <svelte:component this={Views[selectView]} {onSelectAdvert} {selectedAddId} {onBackAdvert} {getCount} />
            </div>
        </div>
    </div>
</div>