<script>
    import { translateText } from 'lang'
    import './main.sass'
    import './assets/style.css'
    import { TimeFormat, GetTime } from 'api/moment'
    import { charName, FractionLVL } from 'store/chars';
    import { serverDateTime } from 'store/server'
    
    import Search from './elements/search/search.svelte'
    import Database from './elements/database/database.svelte'
    import Wanted from './elements/wanted/wanted.svelte'
    import Calls from './elements/calls/calls.svelte'
    import PersonInfo from './elements/personinfo/personinfo.svelte'
    import VehicleInfo from './elements/vehicleinfo/vehicleinfo.svelte'
    import ArrestPerson from './elements/search/elements/arrestperson.svelte'
    import SuEdit from './elements/suedit/index.svelte'

    const Views = {
        Search,
        Database,
        Wanted,
        Calls,
        PersonInfo,
        VehicleInfo,
        ArrestPerson,
        SuEdit
    }

    let selectView = "SuEdit"
</script>

<div id="policecomputer">
    <div class="box-flex">
        <div class="newbuttons_button">ESC</div>
        <div class="whitecolor">{translateText('fractions', 'Закрыть')}</div>
    </div>
    <div class="policecomputer__planshet">
        <div class="policecomputer__head">
            <div>{TimeFormat ($serverDateTime, "HH:mm DD.MM.YYYY")}</div>
            <div class="policecomputer__head_img"></div>
        </div>
        <div class="box-flex w-100">
            <div class="policecomputer__nav">
                <div class="policecomputer__nav_logo"></div>
                <div class="line"></div>
                <div class="policecomputer__nav_element mt-24" class:active={selectView === "Search"} on:click={() => selectView = "Search"}>
                    <span class="bortovoiicon-loop"></span>
                    <div class="policecomputer__nav_text">{translateText('fractions', 'Поиск')}</div>
                </div>
                <div class="policecomputer__nav_element" class:active={selectView === "Database"} on:click={() => selectView = "Database"}>
                    <span class="bortovoiicon-shield"></span>
                    <div class="policecomputer__nav_text">{translateText('fractions', 'База данных')}</div>
                </div>
                <div class="policecomputer__nav_element" class:active={selectView === "Wanted"} on:click={() => selectView = "Wanted"}>
                    <span class="bortovoiicon-list"></span>
                    <div class="policecomputer__nav_text">{translateText('fractions', 'Розыск')}</div>
                </div>
                <div class="policecomputer__nav_element" class:active={selectView === "Calls"} on:click={() => selectView = "Calls"}>
                    <span class="bortovoiicon-call"></span>
                    <div class="policecomputer__nav_text">{translateText('fractions', 'Вызовы')}<span class="black">3</span></div>
                </div>
                <div class="policecomputer__nav_element" class:active={selectView === "SuEdit"} on:click={() => selectView = "SuEdit"}>
                    <span class="bortovoiicon-tasks"></span>
                    <div class="policecomputer__nav_text">{translateText('fractions', 'Законы')}</div>
                </div>
                <div class="line mt-24"></div>
                <div class="line mt-auto"></div>
                <div class="box-column">
                    <div class="policecomputer__name mt-24">{$charName}</div>
                    <div class="policecomputer__rank">{$FractionLVL}</div>
                </div>
                <div class="line mt-24"></div>
                <div class="policecomputer__flag"></div>
            </div>
            <div class="policecomputer__main">
                <svelte:component this={Views[selectView]} />
            </div>
        </div>
    </div>
</div>