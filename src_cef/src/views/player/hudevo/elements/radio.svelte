<script>
    import { translateText } from 'lang'
    import { fade } from 'svelte/transition';
    let playerData = [];
    window.voice = {
        add: (name, id) => {
            const index = playerData.findIndex(r => r.id == id);
            if (!playerData [index]) {
                playerData = [
                    ...playerData,
                    {
                        id: id,
                        name: name,
                        toggled: true
                    }
                ];
            }
        },
        dell: async (id) => {
            const index = playerData.findIndex(r => r.id == id);
            if (playerData [index]) {
                playerData [index].toggled = false
                await window.wait(500);
                playerData.splice(index, 1);
                playerData = playerData;
            }
        },
    }
</script>
{#if playerData.length > 0}
    <div class="hudevo__radio" transition:fade={{ duration: 500 }}>
        <div class="box-flex mb-7">
            <div class="hudevo__radio_title">{translateText('player2', 'Рация')}</div>
            <div class="hudevo__radio_subtitle"><div class="hudevoicon-radio"></div></div>
        </div>
        {#each playerData as item, index}
            <div class="hudevo__radio_element hudevo__elementparams paramsright">
                {item.name}
                <div class="hudevo__radio_absolute" class:active={item.toggled}></div>
            </div>
        {/each}
    </div>
{/if}