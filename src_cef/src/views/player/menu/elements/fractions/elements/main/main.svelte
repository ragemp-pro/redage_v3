<script>
    import { translateText } from 'lang'
    import { setGroup, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('mainLoad')



    import Leader from './elements/leaders.svelte'
    import Stats from './elements/stats.svelte'
    import Settings from './elements/settings.svelte'
    import News from './elements/news.svelte'
    import Log from './elements/log.svelte'
    import Stock from './elements/stock.svelte'


    import { addListernEvent } from "api/functions";
    import {setPopup, isOrganization} from "../../data";
    import Upgrade from "../../popups/upgrade/index.svelte";
    let isLoad = false;
    const onInit = () => {
        getOrgUpgrate();
        getSettings();
        isLoad = true;
    }
    addListernEvent ("table.mainInit", onInit)

    //

    let isOrgUpgrate = false;

    const getOrgUpgrate = () => {
        executeClientAsyncToGroup("isOrgUpgrate").then((result) => {
            isOrgUpgrate = result;
        });
    }
    getOrgUpgrate();

    addListernEvent ("table.isOrgUpgrate", getOrgUpgrate)

    const onUpgrade = () => {
        setPopup ("Upgrade");
    }

    import Logo from '../other/logo.svelte'

    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
    
</script>

{#if isLoad}
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-fractions"></span>
                <div class="fractions__header_title">{translateText('player1', 'Главная страница')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Общая информация, новости и лог действий')}</div>
        </div>
    </div>
    {#if isOrgUpgrate}
        <div class="fractions__main_button" on:click={onUpgrade}>{translateText('player1', 'Улучшения')}</div>
    {/if}
</div>
<div class="fractions__content">
    <div class="box-between">
        <div class="fractions__main_box w-268">
            <div class="fractions__main_head">
                <span class="fractionsicon-crown"></span>
                <div class="fractions__main_title">{translateText('player1', 'Лидер')}</div>
            </div>
            <Leader />
        </div>
        <div class="fractions__main_box w-408">
            <div class="fractions__main_head">
                <span class="fractionsicon-stats"></span>
                <div class="fractions__main_title">{translateText('player1', 'Общая статистика')}</div>
            </div>
            <Stats {settings} />
        </div>
        <Stock />
    </div>
    <div class="box-between not-center mt-20">
        <div class="box-column">
            {#if isOrganization()}
                <Settings {settings} />
                <News {settings} />
            {:else}
                <News {settings} />
            {/if}
        </div>
        <Log />
    </div>
</div>
{/if}