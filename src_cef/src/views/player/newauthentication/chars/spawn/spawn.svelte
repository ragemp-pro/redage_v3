<script>
    import { executeClient } from 'api/rage'

    export let UUID;
    export let FractionID;
    export let OrganizationID;
    export let houseId;
    export let isBan;
    export let DeleteData;
    //export let BankMoney;
    //export let CustomIsCreated;
    
    $: {
        if (FractionID || OrganizationID || houseId) {
            if (onSelectSpawnId (spawnData.pos) === spawnData.none) 
                onSelectSpawnIdToKey (+1);
        }
    }

    let selectSpawnId = 0;
    let spawnConfirm = false;

    const spawnData = {
        none: -1,
        pos: 0,
        org: 1,
        house: 2,
    }

    const onSelectSpawnId = (index) => {
        if (index === selectSpawnId) 
            return selectSpawnId;
        
        if (index == spawnData.house && houseId !== "-") {
            selectSpawnId = spawnData.house;
            spawnConfirm = false;
            return spawnData.house;
        } else if (index == spawnData.org && (FractionID > 0 || OrganizationID > 0)) {
            selectSpawnId = spawnData.org;
            spawnConfirm = false;
            return spawnData.org;
        } else if (index == spawnData.pos) {
            selectSpawnId = spawnData.pos;
            spawnConfirm = false;
            return spawnData.pos;
        }
        return spawnData.none;
    }

    const onKeyUp = (event) => {
		const { keyCode } = event;

        switch (keyCode) {
            case 13:
                onSpawn ()
                break;
            case 37:
                onSelectSpawnIdToKey (-1);
                break;
            case 39:
                onSelectSpawnIdToKey (+1);
                break;
        }
    }

    const onSpawn = () => {
        if (isBan)
            return;
        else if (isBan)
            return;
        else if (DeleteData !== "-")
            return;

        executeClient ("client:OnSelectCharacterv2", UUID, selectSpawnId);
    }

    const onSelectSpawnIdToKey = (number) => {
        let count = selectSpawnId + number;
        let returnId = onSelectSpawnId (count);
        while (returnId === spawnData.none) {
            if (number > 0) {
                count++;
                if (count > spawnData.org)
                    count = spawnData.pos;
            } else {
                count--;                
                if (count < spawnData.pos)
                    count = spawnData.org;
            }
            returnId = onSelectSpawnId (count);
        }
    }

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }
</script>

<svelte:window on:keyup={onKeyUp} />

{#if !isBan && DeleteData === "-"}
<div class="auth__center">
    <div class="auth__spawn_elements" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
        <div class="auth__spawn_element" on:click={() => onSelectSpawnId (spawnData.pos)} class:use={true} class:active={selectSpawnId == spawnData.pos}>
            <div>Место выхода</div>
            <div class="auth__small_text">Вы ничего не потеряете.</div>
            <span class="auth-exit-place"></span>
        </div>
        {#if houseId !== "-"}
        <div class="auth__spawn_element" on:click={() => onSelectSpawnId (spawnData.house)} class:use={true} class:active={selectSpawnId == spawnData.house}>
            {#if !spawnConfirm}
            <div class="box-column">
                <div>Дома</div>
                <div class="auth__small_text">Вы потеряете нелегальные предметы.</div>
            </div>
            <span class="auth-home"></span>
            {:else}
            <div class="auth__small_text">Вы потеряете нелегальные предметы</div>
            <div class="box-flex">
                <div class="auth__spawn_btn">
                    Да
                </div>
                <div class="auth__spawn_btn">
                    Нет
                </div>
            </div>
            {/if}
        </div>
        {/if}
        {#if FractionID > 0 || OrganizationID > 0}
        <div class="auth__spawn_element" on:click={() => onSelectSpawnId (spawnData.org)} class:use={true} class:active={selectSpawnId == spawnData.org}>
            <div class="box-column">
                <div>Во фракции</div>
                <div class="auth__small_text">Вы потеряете нелегальные предметы.</div>
            </div>
            <span class="auth-fraction"></span>
        </div>
        {/if}
    </div>
    <div class="auth__buttons" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
        <div class="main__button_square box-center">
            <b>&#8592;</b>
        </div>
        <div class="main__button main_button_size_large" style="margin-right: 0" on:click={onSpawn}>
            <div class="main__button_left box-center">Войти</div>
            <div class="main__button_right box-center">
                <div class="main__button_square box-center">
                    <span class="auth-arrow"/>
                </div>
            </div>
        </div>
        <div class="main__button_square box-center">
            <b>&#8594;</b>
        </div>
    </div>
</div>
{/if}