<script>
    import { executeClient } from 'api/rage'

    import './assets/sass/lift.sass'
    export let viewData;

    let floors = viewData,
        floor = 0;

    const start = () =>  {
        executeClient ('lift', 'start', floor);
    }

    const stop = () =>  {
        executeClient ('lift', 'stop');
    }

    const up = () =>  {
        floor++
        if (floor == floors.length)
        {
            floor = floors.length - 1;
        }
    }

    const down = () =>  {
        floor--;
        if (floor < 0)
        {
            floor = 0;
        }
    }




</script>

<div id="lift">
    <div class="module_lift">
        <h1>{floors[floor]}</h1>
            <div class="flex">
                <div class="button up">
                    <div id="btn-down" class="btn arrow" on:click={down}></div>
                    <div id="btn-stop" class="btn set" on:click={stop}>STOP</div>
                </div>
                <div class="button down">
                    <div id="btn-up" class="btn arrow" on:click={up}></div>
                    <div id="btn-start" class="btn set" on:click={start}>START</div>
                </div>
            </div>
     </div>
</div>