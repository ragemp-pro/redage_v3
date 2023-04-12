<script>
    import { translateText } from 'lang'
    import moment from 'moment';
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let members;
    export let onSelectMember;
    export let onlineData;
    export let onPlayerDelete;
    export let playerUuid;

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }

    const onDeletePlayer = (event, item) => {
        event.stopPropagation();
        onPlayerDelete (item)
    }

    export let settings;

    const getProgress = (value, max) => {
        let perc = Math.round(value / max * 100);

        if (!perc || perc < 0)
            perc = 0;
        else if (perc > 100)
            perc = 100;

        return perc;
    }
</script>
<div class="fractions__main_box large">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-person"></span>
            <div class="fractions__main_title">{translateText('player1', 'Участники')}</div>
        </div>
        <div class="box-between w-506">
            <div class="fractions__stats_element mb-0">
                <div class="fractions__stats_circle online">
                    <div class="fractions__stats_smallcircle"></div>
                </div>
                <div>{onlineData.online} {translateText('player1', 'человек онлайн')}</div>
            </div>
            <div class="fractions__stats_element mb-0">
                <div class="fractions__stats_circle offline">
                    <div class="fractions__stats_smallcircle"></div>
                </div>
                <div>{onlineData.offline} {translateText('player1', 'человек оффлайн')}</div>
            </div>
            <div class="fractions__stats_element mb-0">
                <div class="fractions__stats_circle neutral">
                    <div class="fractions__stats_smallcircle"></div>
                </div>
                <div>{onlineData.all} {translateText('player1', 'человек всего')}</div>
            </div>
        </div>
        <div class="fractions__input small">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder="Поиск.." bind:value={searchText}>
        </div>
    </div>
    <div class="box-between pr-22 pl-8 pr-96">
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Имя Фамилия')}:</div>
        <div class="fractions_stats_title fs-14 rank">{translateText('player1', 'Ранг')}:</div>
        <div class="fractions_stats_title fs-14 rank">{translateText('player1', 'Отряд')}:</div>
        <div class="fractions_stats_title fs-14 rank">{translateText('player1', 'Очки')}:</div>
        <div class="fractions_stats_title fs-14 date">{translateText('player1', 'Дата входа')}:</div>
    </div>
    <div class="fractions__main_scroll big">
        {#each members.filter(el => filterCheck(el.name, searchText)) as item}
            {#if playerUuid !== item.uuid}
                <div class="fractions__scroll_element small hover" on:click={() => onSelectMember (item)}>
                    <div class="fractions_stats_title whitecolor name fs-16 box-flex m-0">
                        <div class="fractions__stats_circle" class:online={item.isOnline} class:offline={!item.isOnline}>
                            <div class="fractions__stats_smallcircle"></div>
                        </div>
                        <div class="longtextparams">{item.name}</div>
                    </div>
                    <div class="fractions_stats_title whitecolor rank fs-16 m-0 longtextparams">{item.rankName}</div>
                    <div class="fractions_stats_title rank fs-16 m-0">{item.departmentName}</div>
                    <div class="fractions_stats_title rank fs-16 m-0">{getProgress(item.score, item.maxScore)}%</div>
                    <div class="box-flex date fs-16">
                        {#if item.isOnline}
                            <div class="fractions_stats_title whitecolor mb-0 mr-5">В сети ID <span class="green">{item.playerId}</span></div>
                        {:else}
                            <div class="fractions_stats_title whitecolor mb-0 mr-5">{moment(item.date).format('DD.MM.YYYY')}</div>
                            <div class="fractions_stats_title m-0">{moment(item.date).format('HH:mm')}</div>
                        {/if}
                    </div>
                    {#if settings.unInvite}
                    <div class="box-flex hidden">
                        <div class="fractionsicon-fired" on:click={(event) => onDeletePlayer (event, item)}></div>
                    </div>
                    {:else}
                        <div class="hiddenbox14"></div>
                    {/if}
                </div>
            {/if}
        {/each}
    </div>
</div>