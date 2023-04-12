<script>
    import { translateText } from 'lang'
    import { fly } from 'svelte/transition';
    
    let 
        visible = false,
        timerStatus = false,
        gameName = translateText('player2', 'Командный бой'),
        timer = false,
        stats = []
    ;
    
    window.airsoftFunctions = (state, data) => {
        if (state == 0) {
            visible = false;
            timerStatus = false;
            stats = [];
        }
        else if (state == 1) { // 2x2, 3x3, 5x5
            visible = true;
            timerStatus = true;
            gameName = translateText('player2', 'Командный бой');
            stats = JSON.parse(data);
        }
        else if (state == 2) { // GunGame
            visible = true;
            timerStatus = true;
            stats = JSON.parse(data);
            gameName = `GunGame (${stats[3].score} lvl)`;
        }
        else if (state == 3) { // Timer
            visible = true;
            timerStatus = true;
            timer = ((Math.floor(data / 60) < 10 ? '0' : '') + Math.floor(data / 60) + ':' + (data % 60 < 10 ? '0' : '') + data % 60);
        }
        else if (state == 4) { // Airdrop
            visible = true;
            timerStatus = false;
            gameName = translateText('player2', 'Битва за Airdrop');
            stats = JSON.parse(data);
        }
    }
</script>

{#if visible}
    <div class="hudevo__bizwar" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
        {#if timer}
            <div class="hudevo__bizwar_title">{gameName} {timer}</div>
        {:else}
            <div class="hudevo__bizwar_title">{gameName}</div>
        {/if}
        {#if stats[0] !== undefined}
            <div class="hudevo__bizwar_element hudevo__elementparams paramsright">
                {stats[0].score} <span class="hudevo__bizwar_score">{stats[0].name}</span>
            </div>
        {/if}
        {#if stats[1] !== undefined}
            <div class="hudevo__bizwar_element hudevo__elementparams paramsright">
                {stats[1].score} <span class="hudevo__bizwar_score">{stats[1].name}</span>
            </div>
        {/if}
        {#if stats[2] !== undefined}
            <div class="hudevo__bizwar_element hudevo__elementparams paramsright">
                {stats[2].score} <span class="hudevo__bizwar_score">{stats[2].name}</span>
            </div>
        {/if}
        {#if stats[3] !== undefined}
        <div class="hudevo__bizwar_element hudevo__elementparams paramsright">
            {stats[3].score} <span class="hudevo__bizwar_score">{stats[3].name}</span>
        </div>
        {/if}
            {#if stats[4] !== undefined}
            <div class="hudevo__bizwar_element hudevo__elementparams paramsright">
                {stats[4].score} <span class="hudevo__bizwar_score">{stats[4].name}</span>
            </div>
        {/if}
        
    </div>
{/if}