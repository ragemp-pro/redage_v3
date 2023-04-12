<script>
    import { executeClient } from 'api/rage'
    import { charIsPet } from 'store/chars'
    import keysName from 'json/keys.js'
    import keys from 'store/keys'
    import { isInputToggled } from 'store/hud'
    import CustomKey from './Key.svelte'
	import router from 'router';
    import { fly } from 'svelte/transition';

    let commandsArray = []
    const pet = 'cat';

    const keyToBind = {
		48: 9,
		49: 0,
		50: 1,
		51: 2,
		52: 3,
		53: 4,
		54: 5,
		55: 6,
		56: 7,
		57: 8,
	}

    let playerData = [];
    let playerEvent = "";

    let isAnimal = false;
    window.hudStore.isAnimal = (value) => isAnimal = value;
	let animalName = "Питомец";
	window.hudStore.animalName = (value) => animalName = value;

    const handleKeydown = (event) => {
        if (!$router.PlayerHud)
            return;
        else if ($isInputToggled)
            return;
        else if (!isAnimal)
            return;
        
        const { keyCode } = event;

        for (let i = 0; i < 11; i++) {
            if (keyCode !== (48 + i))
                continue;
            else if (keyToBind [48 + i] === undefined)
                continue;
            const command = playerData.length ? playerData[ keyToBind [48 + i] ] : commandsArray[ keyToBind [48 + i] ];

            if (command) {
                if (playerData.length) {
                    if (!command.isEnd) 
                        executeClient (playerEvent, command.pId)
                    playerData = []
                    playerEvent = "";
                }
                else {
                    executeClient (command.event);
                }
                return
            }
        }
	}

    const SetPlayers = (players, event) => {
        playerData = JSON.parse (players);
        playerEvent = event;
    }

    const SetMenu = (menu) => {
        commandsArray = JSON.parse (menu);
    }
        
    import { onMount, onDestroy } from 'svelte'

    let health = 100;
    const SetHealth = (value) => {
        health = value;
    }

    onMount(() => {
        window.events.addEvent("cef.pet.menu", SetMenu);
        window.events.addEvent("cef.pet.player", SetPlayers);
        window.events.addEvent("cef.pet.health", SetHealth);
    });

    onDestroy(() => {
        window.events.removeEvent("cef.pet.menu", SetMenu);
        window.events.removeEvent("cef.pet.player", SetPlayers);
        window.events.removeEvent("cef.pet.health", SetHealth);
    });
</script>
<svelte:window on:keydown={handleKeydown}/>
{#if $charIsPet}
<div class="hudevo__petcontrol" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
    <div class="hudevo__petbox hudevo__elementparams">
        <div class="buttonsinfo__button">{keysName[$keys[55]]}</div>
        <div class="petcontrol__name">{animalName}</div>
        <div class="petcontrol__pet">
            <div class="petcontrol__pet_img" style="background-image: url({document.cloud}img/pets/{$charIsPet})"></div>
        </div>
    </div>
    <div class="hudevo__lobby_health">
        <div class="hudevo__lobby_health_current" style="width: {health}%"></div>
    </div>
    
    <div class="petcontrol__buttons" style="opacity: {isAnimal ? 1 : 0};">
        {#each (playerData.length ? playerData : commandsArray) as command, index}
            <div class="box-flex">
                {#if index == 9}
                <div class="petcontrol__button">{keysName[48]}</div>
                {:else}
                <div class="petcontrol__button">{keysName[49 + index]}</div>
                {/if}
                <div class="petcontrol__button_text">{command.name}</div>
            </div>
        {/each}
    </div>
</div>
{/if}