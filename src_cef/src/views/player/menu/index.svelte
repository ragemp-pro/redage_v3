<script>
    import { translateText } from 'lang'
    import './main.sass';
    import './fonts/inv/style.css';
    import './fonts/items/style.css';
    import './fonts/gamemenu/style.css';

    import { serverDateTime, isEvent } from 'store/server'
    import { TimeFormat } from 'api/moment'

    export let visible = false;
    
    let selectView = "Inventory";
    let timerView = "Inventory";

    import Inventory from "./elements/inventory/inventory.svelte";
    import Stats from "./elements/stats.svelte";
    import Settings from "./elements/settings/index.svelte";
    import Quests from "./elements/quests/index.svelte";
    import Events from "./elements/events.svelte";
    import Support from "./elements/support/index.svelte";
    import RewardsList from "./elements/rewardslist/index.svelte";
    import Table from "./elements/fractions/index.svelte";

    const Views = {
        Stats,
        Settings,
        Quests,
        Events,
        Support,
        RewardsList,
        Fractions: Table,
        Organization: Table,
    }
    
    window.gameMenuView = (wiew) => {
        selectView = wiew;
        timerView = wiew;
    }
    const defaultSorted = ["Inventory", "Stats", "Settings", "Quests", "RewardsList", "Fractions", "Organization", "Events"];
    let _pagesSorted = ["Inventory", "Stats", "Settings", "Quests", "RewardsList"];
    let PagesSorted = ["Inventory", "Stats", "Settings", "Quests", "RewardsList"];

    const updatePage = (name, value) => {

        const index = _pagesSorted.indexOf(name)

        if (index !== -1 && !value)
            _pagesSorted.splice(index, 1);
        else if (index === -1 && value)
            _pagesSorted.push(name)

        let sorted = [];

        defaultSorted.forEach((value) => {
            if (_pagesSorted.includes (value))
                sorted.push(value)
        })

        PagesSorted = sorted;
    }

    import { charFractionID, charOrganizationID } from 'store/chars'


    isEvent.subscribe(value => {
        updatePage ("Events", value);
    });
    charFractionID.subscribe(value => {
        updatePage ("Fractions", value > 0);
    });
    charOrganizationID.subscribe(value => {
        updatePage ("Organization", value > 0);
    });


    let UseVisible = visible;
    let TimeQE = 0;

    import { onInputBlur } from './elements/fractions/data'

    $: {
        if (UseVisible != visible) {
            UseVisible = visible;
            TimeQE = new Date().getTime() + 250;
            if (!visible) {
                selectView = "Inventory";
                timerView = "Inventory";
                onInputBlur ();
            }
        }
    }

    let isFocusInput = false;

    window.onFocusInput = (value) => isFocusInput = value;

    let timerId = null;

    const setPage = () => {
        if (timerId !== null)
            clearTimeout(timerId);
            
        timerId = setTimeout(() => {
            selectView = timerView;
            timerId = null;
        }, 200)
    }

    function onClickQ() {
        let index = PagesSorted.findIndex (p => p === selectView)
        
        if(--index >= 0) {
            selectView = PagesSorted [index];
            //setPage ();
        }
    }

    function onClickE() {
        let index = PagesSorted.findIndex (p => p === selectView)

        if (++index < PagesSorted.length) {
            selectView = PagesSorted [index];
            //setPage ();
        }
    }
    
    const onKeyUp = (event) => {
        if (isFocusInput)
            return;

        if (!visible) return;
        else if (TimeQE > new Date().getTime()) return;

        const { keyCode } = event;
        
        if(keyCode == 81) {
            onClickQ ();
        } else if(keyCode == 69) { 
            onClickE ();
        }
    }
</script>
<svelte:window on:keyup={onKeyUp} />
<div id="box-menu" style="display: {visible ? 'flex' : 'none'}">
    <div class="box-nav">
        <div class="header" />
        <div class="nav">
            <div class="box-key">Q</div>
            <div class="nav-lists">
                <div class="item" class:active={selectView === "Inventory"} on:click={() => window.gameMenuView ("Inventory")}>
                    {translateText('player1', 'Инвентарь')}
                    <span class="gamemenu-inventory gamemenu__item_absolute"></span>
                </div>
                <div class="item" class:active={selectView === "Stats"} on:click={() => window.gameMenuView ("Stats")}>
                    {translateText('player1', 'Статистика')}
                    <span class="gamemenu-stats gamemenu__item_absolute"></span>
                </div>
                <div class="item" class:active={selectView === "Settings"} on:click={() => window.gameMenuView ("Settings")}>
                    {translateText('player1', 'Настройки')}
                    <span class="gamemenu-settings gamemenu__item_absolute"></span>
                </div>
                <div class="item" class:active={selectView === "Quests"} on:click={() => window.gameMenuView ("Quests")}>
                    {translateText('player1', 'Квесты')}
                    <span class="gamemenu-quest gamemenu__item_absolute"></span>
                </div>
                <div class="item" class:active={selectView === "RewardsList"} on:click={() => window.gameMenuView ("RewardsList")}>
                    {translateText('player1', 'Ежедневные награды')}
                    <span class="gamemenu-gift gamemenu__item_absolute"></span>
                </div>
               <div class="item" class:active={selectView === "Events"} on:click={() => window.gameMenuView ("Events")}>
                    {translateText('player1', 'Мероприятие')}
                    <span class="gamemenu-event gamemenu__item_absolute"></span>
                </div>
                {#if PagesSorted.includes("Fractions")}
                <div class="item" class:active={selectView === "Fractions"} on:click={() => window.gameMenuView ("Fractions")}>
                    {translateText('player1', 'Фракция')}
                    <span class="gamemenu-friends gamemenu__item_absolute"></span>
                </div>
                {/if}
                {#if PagesSorted.includes("Organization")}
                    <div class="item" class:active={selectView === "Organization"} on:click={() => window.gameMenuView ("Organization")}>
                        {translateText('player1', 'Организация')}
                        <span class="gamemenu-friends gamemenu__item_absolute"></span>
                    </div>
                {/if}
            </div>
            <div class="box-key">E</div>
        </div>
        <div class="box-date">
            <div class="box-time">
                <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
                {TimeFormat ($serverDateTime, "DD.MM.YYYY")}
            </div>
        </div>
    </div>
    <Inventory visible={selectView === "Inventory"} />
    <svelte:component this={Views[selectView]} {visible} {selectView} />
</div>