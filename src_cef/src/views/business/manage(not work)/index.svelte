<script>

    import './main.sass';
    import './fonts/gamemenu/style.css';

    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    export let visible = false;
    
    let selectView = "Main";

    import Main from "./elements/main.svelte";
    import Stock from "./elements/stock.svelte";
    import Orders from "./elements/orders.svelte";

    const Views = {
        Main,
        Stock,
        Orders,
    }

    let PagesSorted = [];

    let UseVisible = visible;
    let TimeQE = 0;

    $: {
        if (UseVisible != visible) {
            UseVisible = visible;
            TimeQE = new Date().getTime() + 250;
            if (!visible) 
                selectView = "Main";
        }
    }

    function onClickQ() {
        let index = PagesSorted.findIndex (p => p === selectView)
        
        if(--index >= 0) {
            const page = PagesSorted [index];
            selectView = PagesSorted [index];
        }
    }

    function onClickE() {
        let index = PagesSorted.findIndex (p => p === selectView)

        if (++index < PagesSorted.length) {
            const page = PagesSorted [index];
            selectView = PagesSorted [index];
        }
    }
    
    const onKeyUp = (event) => {
        if (!visible) return;
        else if (TimeQE > new Date().getTime()) return;

        const { keyCode } = event;
        
        if(keyCode == 81) {
            onClickQ ();
        } else if(keyCode == 69) { 
            onClickE ();
        }
    }

    import { format } from 'api/formatter'
    import { setGroup, executeClientAsyncToGroup } from 'api/rage'

    setGroup (".businessmanage.");

    let stats = {}
    executeClientAsyncToGroup("getStats").then((result) => {
        if (result && typeof result === "string")
            stats = JSON.parse(result);
    });
</script>
<svelte:window on:keyup={onKeyUp} />
<div id="bizmanage">
    <div class="box-nav">
        <div class="header" />
        <div class="nav">
            <div class="box-key">Q</div>
            <div class="nav-lists">
                <div class="item" class:active={selectView === "Main"} on:click={() => selectView = "Main"}>
                    Главная
                    <span class="businessmanage-news gamemenu__item_absolute"></span>
                </div>
                <div class="item" class:active={selectView === "Stock"} on:click={() => selectView = "Stock"}>
                    Склад
                    <span class="businessmanage-orders gamemenu__item_absolute"></span>
                </div>
                <div class="item" class:active={selectView === "Orders"} on:click={() => selectView = "Orders"}>
                    Заказы
                    <span class="businessmanage-inventory gamemenu__item_absolute"></span>
                </div>
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
    <div class="bizmanage__box">
        <div class="bizmanage__box_left">
            <div class="bizmanage__title">Магазин 24/7 №10</div>
            <div class="bizmanage__info">Информация о бизнесе</div>
            <div class="box-flex">
                <span class="businessmanage-nalog bizmanage__mr-15"></span>
                <div class="box-column">
                    <div class="bizmanage__subtitle">Налог в час</div>
                    <div class="bizmanage__text">
                        <span class="green">${format("money", stats.tax)}</span> час
                    </div>
                </div>
            </div>
            <div class="box-flex">
                <span class="businessmanage-balance bizmanage__mr-15"></span>
                <div class="box-column">
                    <div class="bizmanage__subtitle">Баланс бизнеса</div>
                    <div class="bizmanage__text">
                        <span class="green">${format("money", stats.cash)}</span>
                    </div>
                </div>
            </div>
            <div class="box-flex">
                <span class="businessmanage-orders bizmanage__mr-15"></span>
                <div class="box-column">
                    <div class="bizmanage__subtitle">Склад</div>
                    <div class="bizmanage__text">
                        { format("money", stats.whCount) } / <span class="gray">{ format("money", stats.whMaxCount) }</span> шт.
                    </div>
                </div>
            </div>
            <div class="bizmanage__button_block">
                <div class="bizmanage__button box-center w-1">ПРОДАТЬ БИЗНЕС</div>
            </div>
        </div>
        <div class="box-column">
            <svelte:component this={Views[selectView]} />
        </div>
        <div class="bizmanage__button_block reverse">
            <div class="bizmanage__button box-center">ESC</div>
            <div>Выйти</div>
        </div>
        <div class="bizmanage__box_left">
            
        </div>
    </div>
</div>