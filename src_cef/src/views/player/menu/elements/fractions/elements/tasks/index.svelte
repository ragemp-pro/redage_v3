<script>
    import { translateText } from 'lang'
    import Missions from './missions.svelte'
    import Personal from './myList.svelte'
    import Fraction from './fractionList.svelte'
    import Logo from '../other/logo.svelte'
    
    const Views = {
        Missions,
        Personal,
        Fraction
    }

    let selectView = "Missions";

    let isLoad = false;

    const onSelectView = (view) => {
        if (isLoad)
            return;

        selectView = view;
    }
    const onSetLoad = (toggled) => {
        isLoad = toggled;
    }
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-tasks"></span>
                <div class="fractions__header_title">{translateText('player1', 'Задания')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Здесь отображается информация о текущих заданиях')}</div>
        </div>
    </div>
</div>
<div class="box-flex mt-20">
    <div class="fractions__selector" class:active={selectView === "Missions"} on:click={() => onSelectView ("Missions")}>
        {translateText('player1', 'Задания')}
    </div>
    <div class="fractions__selector" class:active={selectView === "Personal"} on:click={() => onSelectView ("Personal")}>
        {translateText('player1', 'Персональные')}
    </div>
    <div class="fractions__selector" class:active={selectView === "Fraction"} on:click={() => onSelectView ("Fraction")}>
        {translateText('player1', 'Фракционные')}
    </div>
</div>
<svelte:component this={Views[selectView]} {onSetLoad} />