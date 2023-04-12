<script>
    import { accountData, accountLogin } from 'store/account' 
    import { selectChar, selectType, selectIndex, settings } from './../store.js';
    import { updateIndex } from 'store/customization';
    import CharacterMain from './characterMain.svelte'
    import { executeClient } from 'api/rage'
    

    //Авто спавн
    let autoSpawnTimerId = null;
    let autoSpawnUUID = -1;
    let autoSpawnIndex = -1;
    const autoSpawnTime = 10;//Через сколько спавнить

    const createSpawnTimer = (index, uuid) => {
        autoSpawnIndex = index;
        autoSpawnUUID = uuid;
        //
        autoSpawnTimerId = setTimeout(() => {
            autoSpawnTimerId = null;

            if (autoSpawnUUID !== -1) 
                executeClient ("client:OnSelectCharacterv2", autoSpawnUUID, 0);
        }, 1000 * autoSpawnTime)
    }

    $: if (autoSpawnIndex != -1 && autoSpawnIndex !== $selectIndex) {
        autoSpawnIndex = -1;
        autoSpawnUUID = -1;
        if (autoSpawnTimerId != null)
            clearTimeout (autoSpawnTimerId);
    }

	import { onDestroy } from 'svelte';

    const onKeyUp = () => {
        
        if (autoSpawnTimerId != null) 
            clearTimeout (autoSpawnTimerId);
    }
    
    onDestroy (() => {
        if (autoSpawnTimerId != null) 
            clearTimeout (autoSpawnTimerId);
    })

    //

    const getCharData = (chars, charid, index) => {
        if (chars && chars [charid] && chars [charid].Data) {
            const char = chars [charid];
            let isSelectChar = autoSpawnUUID === -1 && char.Data.UUID === $accountData.LastSelectCharUUID && char.Data.DeleteData === "-";
            if ($selectIndex == -1 || $selectType != settings.char || isSelectChar) {
                selectIndex.set (index);
                updateIndex (index);
                selectType.set (settings.char);
                selectChar.set (char);
                if (isSelectChar) 
                    createSpawnTimer (index, char.Data.UUID)
            }
            return char;
        }
        if ($selectIndex == -1) {
            selectIndex.set (index);
            updateIndex (index);
            selectType.set (settings.create);
        }
        return charid;
    }

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }
</script>

<svelte:window on:keyup={onKeyUp} on:mouseup={onKeyUp} />

<div class="box-flex">
    <div class="main__scroll big"></div>
    <div class="auth__characters" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
        <div class="auth__characters_header">
            <span class="auth-logout"></span>
            {$accountLogin}
        </div>
        {#each $accountData.charsSlot as char, index}
            <CharacterMain charid={index} char={getCharData ($accountData.chars, char, index)}  />
        {/each}
    </div>
</div>