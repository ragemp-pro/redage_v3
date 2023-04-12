<script>
    import { translateText } from 'lang'
    import './main.sass';

    import Everyday from './everyday.svelte'
    import Donates from './donates.svelte'

    const Views = {
        Everyday,
        Donates
    }

    let selectView = "Everyday";
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

<div id ="rewardslist">
    <div class="rewardslist__nav">
        <div class="rewardslist__title">{translateText('player1', 'Награды и достижения')}</div>
        <div class="rewardslist__subtitle" class:active={selectView === "Everyday"} on:click={() => onSelectView ("Everyday")}>— {translateText('player1', 'Ежедневные награды')}</div>
        <div class="rewardslist__subtitle" class:active={selectView === "Donates"} on:click={() => onSelectView ("Donates")}>— {translateText('player1', 'Бонусы за пополнение')}</div>
        <!--<div class="rewardslist__subtitle" class:active={selectView === "Achievments"} on:click={() => onSelectView ("Achievments")}>— Достижения</div>-->
    </div>
    <svelte:component this={Views[selectView]} {onSetLoad} />
    <div class="rewardslist__nav"></div>
</div>