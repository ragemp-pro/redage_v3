<script>
    import { translateText } from 'lang'
    import Players from './players.svelte'
    import Ranks from './ranks.svelte'
    import Squad from './squad.svelte'

    const Views = {
        Players,
        Ranks,
        Squad
    }
    let selectView = "Ranks"

    const onSelectView = (view) => {
        selectView = view
    }
    import Logo from '../other/logo.svelte'
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-squads"></span>
                <div class="fractions__header_title">{translateText('player1', 'Доступы')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Меню управления доступами')}</div>
        </div>
    </div>
</div>
<div class="fractions__content backgr not-center">
    <div class="fractions__main_head box-between">
        <div class="box-flex">
            <span class="fractionsicon-complaints"></span>
            <div class="fractions__main_title">{translateText('player1', 'Управление доступами')}</div>
        </div>
    </div>
    <div class="box-flex">
        <div class="fractions__selector" class:active={selectView === "Ranks"} on:click={() => onSelectView ("Ranks")}>
            {translateText('player1', 'Ранга')}
        </div>
        <div class="fractions__selector" class:active={selectView === "Squad"} on:click={() => onSelectView ("Squad")}>
            {translateText('player1', 'Отряда')}
        </div>
        <div class="fractions__selector" class:active={selectView === "Players"} on:click={() => onSelectView ("Players")}>
            {translateText('player1', 'Персональный')}
        </div>
    </div>
    <div class="box-between align-startflex">
        <svelte:component this={Views[selectView]}  />
    </div>
</div>