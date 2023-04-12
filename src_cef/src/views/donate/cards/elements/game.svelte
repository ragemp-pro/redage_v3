<script>
    import { translateText } from 'lang'
    //import canvasScratchCard from 'canvas-scratch-card';
    import money from '../images/prize/money.png'
    import scratch from '../images/scratch_silver.png'
    import scratch2 from '../images/scratch_silver.svg'
    import { ScratchCard, SCRATCH_TYPE } from 'scratchcard-js';

    let slotsData = [
        {},
        {},
        {},
        {},
        {},
        {},
        {},
        {},
    ];

    const CreateConvas = (node, bar) => {

        slotsData[bar] = new ScratchCard('#card_' + bar, {
        scratchType: SCRATCH_TYPE.SPRAY,
        containerWidth: 60,
        containerHeight: 60,
        imageForwardSrc: scratch,
        imageBackgroundSrc: money,
        htmlBackground: '<p class="test"><strong>Hello i am HTML content !</strong></p>',
        clearZoneRadius: 50,
        nPoints: 30,
        pointSize: 4,
        callback: function () {
            alert('Now the window will reload !')
            //window.location.reload()
        }
        });
    }

</script>    

<div class="content s2">
    <div class="title">{translateText('donate', 'Стирай и выигрывай')}</div>
    <p>{translateText('donate', 'Идейные соображения высшего порядка, а также укрепление и развитие структуры играет важную роль в формировании существенных финансовых и административных условий..')}</p>

    <ul class="awardsItems game">

        {#each slotsData as v, i}
            <li id="card_{i}"> 
                <div class="scratch" use:CreateConvas={i}></div>
            </li>
        {/each}
    </ul>

</div>