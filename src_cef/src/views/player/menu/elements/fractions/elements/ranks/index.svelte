<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import RanksList from './rankslist.svelte'
    import RankInfo from './rankinfo.svelte'

    let settings = null;
    let ranks = []

    const getRanks = () => {
        executeClientAsyncToGroup("getRanks").then((result) => {
            if (result && typeof result === "string") {
                ranks = JSON.parse(result);

                if (settings) {
                    const index = ranks.findIndex(r => r.id === settings.id);

                    if (ranks [index]) {
                        settings = ranks [index];
                    }
                    else
                        settings = null;
                }
            }
        });
    }
    getRanks();

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.ranks", getRanks)


    const onSettings = (item = null) => {
        settings = item;
    }
    import Logo from '../other/logo.svelte'

    let isLeader = false;
    executeClientAsyncToGroup("getSettings").then((result) => {
        if (result && typeof result === "string") {
            result = JSON.parse(result);
            isLeader = result.isLeader;
        }
    });

    const defaultRanksCallback = () => {
        executeClientToGroup('defaultRanks');

    }

    import { setPopup } from "../../data";
    const defaultRanks = () => {
        setPopup ("Confirm", {
            headerTitle: "Сбросить ранги",
            headerIcon: "fractionsicon-rank",
            text: `Вы действительно хотите Сбросить ранги?`,
            button: 'Да',
            callback: defaultRanksCallback
        })
    }
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-rank"></span>
                <div class="fractions__header_title">{translateText('player1', 'Ранги')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Меню просмотра, настройки и создания рангов')}</div>
        </div>
    </div>
    {#if !settings && isLeader}
        <div class="box-flex">
            <div class="fractions__main_button extra" on:click={defaultRanks}>{translateText('player1', 'Сбросить настройки')}</div>
        </div>
    {/if}
</div>
{#if !settings}
    <RanksList {onSettings} {ranks} />
{:else}
    <RankInfo {settings} {onSettings} />
{/if}