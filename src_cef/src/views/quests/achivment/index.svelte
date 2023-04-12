<script>
    import { translateText } from 'lang'
    import './main.sass';
    import './fonts/style.css';
    import KeyAnimation from '@/components/keyAnimation/index.svelte';
    import { getQuest, getActors } from 'json/quests/quests.js';
    //questsnewdialogicon-
	import { fade, fly } from 'svelte/transition';
    import { format } from 'api/formatter'
    export let questData;
    export let onEnter;
</script>
<div id="questsprise">
    <div class="questsprise-header" in:fly={{y: -200, duration: 1000}}>
        <div class="box-between">
            <div class="questsnewdialogicon-star icon-star-small"></div>
            <div class="questsnewdialogicon-star icon-star-big"></div>
            <div class="questsnewdialogicon-star icon-star-small"></div>
        </div>
        <div>{translateText('quests', 'Квест завершен')}</div>
    </div>
    <div class="questsprise-questname-main">
        <svg class="questprise-backgroundblock" in:fly={{x: -200, duration: 1000}}>
            <text y="50%" x="50%" class="questprise-backgroundtext">{getActors (questData.actorName).name}</text>
        </svg>
        <div class="questsprise-questname-text" in:fly={{x: 200, duration: 1000}}>{getActors (questData.actorName).name}</div>
    </div>
    <div class="questprise-description">{questData.Desc}</div>
    <div class="questprise-prise">
        <div class="box-column" in:fly={{x: -200, duration: 1000}}>
            <div class="questprise-center-maintext">{translateText('quests', 'Основная награда')}</div>
            <div class="box-money">
                {#if questData.Reward && questData.Reward.Money}
                <span class="questsnewdialogicon-money"/> {format("money", questData.Reward.Money)}
                {:else}
                <span class="questsnewdialogicon-money"/> 0
                {/if}
            </div>
            <div class="box-exp">
                {#if questData.Reward && questData.Reward.Exp}
                    <span class="questsnewdialogicon-exp"/> {questData.Reward.Exp}
                {:else}
                <span class="questsnewdialogicon-exp"/> 0
                {/if}
            </div>
        </div>
        <div class="box-column"  in:fly={{x: 200, duration: 1000}}>
            <div class="questprise-center-maintext">{translateText('quests', 'Предметы')}</div>
            <div class="questprise-grid">
                {#each new Array(4) as _, index}
                    {#if questData.Reward && questData.Reward.Items && questData.Reward.Items[index] && window.getItem (questData.Reward.Items[index])}
                        <div class="questprise-grid__item">
                            <span class="{window.getItem (questData.Reward.Items[index]).Icon} questprise-grid__item_icon"/>
                        </div>
                    {:else}
                        <div class="questprise-grid__item" />
                    {/if}
                {/each}

            </div>
        </div>
    </div>
    <div class="box-flex" in:fly={{y: 200, duration: 1000}}>
    
        <div class="box-KeyAnimation" on:click={onEnter}>
            <KeyAnimation keyCode={13}>ENTER</KeyAnimation>
            <div>{translateText('quests', 'Забрать награду')}</div>
        </div>
    </div>
</div>