<script>
    import { translateText } from 'lang'
    import {executeClientAsyncToGroup} from "api/rage";
    import { format } from "api/formatter";
    import copy from 'copy-to-clipboard';

    let stats = {}

    const getStats = () => {
        executeClientAsyncToGroup("getStats").then((result) => {
            if (result && typeof result === "string")
                stats = JSON.parse(result);
        });
    }
    getStats ();
    import { isFraction } from "../../../data";

    export let settings;
    
    const onCopyDiscord = () => {
        copy(`https://discord.gg/${settings.discord}`)
        window.notificationAdd(4, 9, `Вы скопировали ссылку на Discord`, 3000);
    }
</script>
<div class="fractions__main_grid">
    <div class="fractions__main_element">
        <div class="fractions_stats_title">{translateText('player1', 'Материалы')}:</div>
        <div class="fractions__stats_subtitle">{stats.mats} / {stats.maxMats}</div>
    </div>
    <div class="fractions__main_element">
        <div class="fractions_stats_title">{translateText('player1', 'Аптечки')}:</div>
        <div class="fractions__stats_subtitle">{stats.medKits} шт.</div>
    </div>
    <!--<div class="fractions__main_element">
        <div class="fractions_stats_title">{translateText('player1', 'Очки фракции')}:</div>
        <div class="fractions__stats_subtitle">1200 XP</div>
    </div>-->
    <div class="fractions__main_element">
        <div class="fractions_stats_title">{translateText('player1', 'Деньги')}:</div>
        <div class="fractions__stats_subtitle">${format("money", stats.money)}</div>
    </div>
    <div class="fractions__main_element">
        <div class="fractions_stats_title">{translateText('player1', 'Наркотики')}:</div>
        <div class="fractions__stats_subtitle">{stats.drugs} шт.</div>
    </div>
    {#if stats.warZonesCount >= 0}
        <div class="fractions__main_element">
            <div class="fractions_stats_title">Территорий:</div>
            <div class="fractions__stats_subtitle">{stats.warZonesCount} шт.</div>
        </div>
    {/if}
    {#if stats.gangZonesCount >= 0}
        <div class="fractions__main_element">
            <div class="fractions_stats_title">Территорий:</div>
            <div class="fractions__stats_subtitle">{stats.gangZonesCount} шт.</div>
        </div>
    {/if}
    {#if stats.bizCount >= 0}
        <div class="fractions__main_element">
            <div class="fractions_stats_title">Бизнесов:</div>
            <div class="fractions__stats_subtitle">{stats.bizCount} шт.</div>
        </div>
    {/if}
    {#if isFraction ()}
        <div class="fractions__main_element">
            <div class="fractions_stats_title">Discord:</div>
            <div class="fractions__stats_subtitle">
                <div class="box-flex">
                    {settings.discord}
                    <span class="fractionsicon-copy" on:click={onCopyDiscord}></span>
                </div>
            </div>
        </div>
    {/if}
</div>