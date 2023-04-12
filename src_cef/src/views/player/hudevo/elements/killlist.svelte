<script>
    import { fly } from 'svelte/transition';
    
    let kills = [];
    window.kill = {
        show: (killName, Icon, Name, kColor, vColor) => {
            kills.unshift({
                killName: killName,
                Icon: Icon,
                Name: Name,
                killerColor: kColor,
                victimColor: vColor
            });
            
            if (kills.length > 4) kills.splice(5, 1);
            kills = kills;
        },
        clear: () => {
            kills = [];
            kills = kills;
        }
    }
</script>
<div class="hudevo__killlist">
    {#each kills as item, index}
        <div class="hudevo__killlist_element" in:fly={{ y: 10, duration: 100 }} out:fly={{ y: 10, duration: 150 }}>
            {#if item.killName.length > 2}
                <div class="hudevo__killlist_killer hudevo__elementparams">
                    {item.killName}
                    <div class="killlist_absolute" style="background: {item.killerColor}"></div>
                </div>
            {/if}
            <div class="hudevo__killlist_killed hudevo__elementparams paramsright">
                {item.Name}
                <div class="killlist_absolute" style="background: {item.victimColor}"></div>
                <div class="hudevo__killlist_sprite">
                    {#if item.Icon.length > 2}
                        <div class="{item.Icon}"/>
                    {:else}
                        <div class="hudevoicon-skull"/>
                    {/if}
                </div>
            </div>
        </div>
    {/each}
</div>
