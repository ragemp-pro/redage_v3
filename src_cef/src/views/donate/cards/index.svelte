<script>
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import './main.scss'
    import money from './images/prize/money.png'

    import canvasScratchCard from 'canvas-scratch-card';
    import scratch from './images/scratch_silver.svg'

    import Awards from './elements/awards.svelte';
    import Main from './elements/main.svelte';
    import Game from './elements/game.svelte';
    import Win from './elements/win.svelte';
    
    let SelectViews = 'Game';
    const Views = {
        Awards,
        Main,
        Game,
        Win
    };

    import { onMount } from 'svelte';
    let
        style = "gold",
        name = 'Silver',
        slots = [
            {},
            {},
            {},
            {},
            {},
            {},
            {},
            {},
        ],
        pageActive = 'win'; //main //game //awards //win

     onMount(() => {
        canvasScratchCard.init({
            dom: slots[0],
            r: 10, // radius default 20
            rate: 0.6, // when to call callback 0.1 - 1,
            image: scratch, // <img> dom or url
            //fillColor: '#b22', // fillColor, when this prop set, image will not render
            callback: () => {
                alert('gotovo');
            },
         });
    });
    

</script>


<div id="pageCard">
    <div class="cardGame {style}">
        <svelte:component this={Views[SelectViews]} />
        <div class="nav"> 
            <ul class="mainBottom">
                <li class="leftArr btn small"><span class="imgSett"/></li>
                <li class="rightArr btn small"><span class="imgSett"/></li>
                <li class="placeholder">{translateText('donate', 'Перевернуть билет')}</li>
            </ul>
            <ul class="mainBottom">
                <li class="btn sett-1">END</li>
                <li class="placeholder">{translateText('donate', 'Закрыть')}</li>
            </ul>
        </div>
    </div>
</div>