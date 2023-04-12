<script>
    import { translateText } from 'lang'
    import BattlePass from './components/battlepass/index.svelte'
    import Missions from './components/missions.svelte'
    import { isPopupInfoOpened, isPopupLvlOpened, isPopupBuyOpened } from './stores.js'

    import BattlePassPopupBuyPremium from './components/battlepass/popupBuyPremium.svelte'
    import BattlePassPopupBuyLvl from './components/battlepass/popupBuyLvl.svelte'
    import BattlePassPopupInfo from './components/battlepass/popupInfo.svelte'

    import './main.sass' 
    import './assets/fonts/style.css' 

    import { setGroup, executeClientToGroup } from 'api/rage'


    setGroup (".battlepass.");

    let SelectViews = 'BattlePass';
    const Views = {
        BattlePass,
        Missions
    }

    const handleKeyUp = (event) => {
        const { keyCode } = event;

        if (keyCode === 27)
            onClose ();
    }

    const onClose = () => {
        if ($isPopupBuyOpened || $isPopupLvlOpened || $isPopupInfoOpened) {
            isPopupBuyOpened.set (false);
            isPopupLvlOpened.set (false);
            isPopupInfoOpened.set (false);
            return;
        }

        executeClientToGroup ("close")
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div id="battlepass">
    {#if $isPopupBuyOpened}
        <BattlePassPopupBuyPremium />
    {:else if $isPopupLvlOpened}
        <BattlePassPopupBuyLvl />
    {:else if $isPopupInfoOpened}
        <BattlePassPopupInfo />
    {/if}
    <div class="battlepass__content">
        <div class="newproject__buttonblock">
            <div class="newproject__button">ESC</div>
            <div>{translateText('player', 'Закрыть')}</div>
        </div>
        <div class="battlepass__header">
            <div class="battlepass__header_pages">
                <div class="battlepass__header_page" class:active={SelectViews == 'BattlePass'} on:click={() => SelectViews = 'BattlePass'}>{translateText('player', 'Боевой пропуск')}</div>
                <div class="battlepass__header_page" class:active={SelectViews == 'Missions'} on:click={() => SelectViews = 'Missions'}>{translateText('player', 'Сеть заданий')}</div>
            </div>
            <div class="battlepass__header_title">
                <div class="battlepass__title" class:missions={SelectViews !== 'BattlePass'}>{SelectViews == 'BattlePass' ? translateText('player', 'Боевой пропуск') : translateText('player', 'Сеть заданий')}</div>
            </div>
        </div>
        <svelte:component this={Views[SelectViews]} />
    </div>
    <!--<video class="bgvideo"
       src={document.cloud + `battlepass/bg_top.webm`}
       loop={true}
       autoplay={true}
       name="media"
       volume={0}>
        <track kind="captions">
    </video>

    <video class="bgvideo fire"
       src={document.cloud + `battlepass/bg_bottom.webm`}
       loop={true}
       autoplay={true}
       name="media"
       volume={0}>
        <track kind="captions">
    </video>-->

    {#if SelectViews == 'BattlePass'}
        <div class="battlepass__person"></div>
    {/if}
</div>