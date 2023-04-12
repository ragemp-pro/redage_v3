<script>
    import { translateText } from 'lang'
    export let charid;

    import InputCustom from 'components/input/oneInput.svelte'
    import { selectIndex, selectType, settings } from './../store.js';
    import { gender, updateGender, updateIndex, FirstName, updateFirstName, LastName, updateLastName } from 'store/customization';
    
    let FirstNameLocal = "";
    $: if (FirstNameLocal) {
        updateFirstName (FirstNameLocal)
    }

    FirstName.subscribe(value => {
        FirstNameLocal = value;
    });

    let LastNameLocal = "";
    $: if (LastNameLocal) {
        updateLastName (LastNameLocal)
    }

    LastName.subscribe(value => {
        LastNameLocal = value;
    });

    const onSelectChar = () => {
        updateIndex (charid);
        selectIndex.set (charid);
        selectType.set (settings.create);
    }
    
</script>

<div class="auth__characters_block" class:active={$selectIndex === charid} on:click={onSelectChar}>
    <div class="auth__characters_character">
        <div class="box-column">
            <b>{translateText('player2', 'Создать')}</b>
            <div>{translateText('player2', 'Персонажа')}</div>
        </div>
        <div class="auth__characters_circle orange">
            <div class="auth__characters_circle_text">
                <b class="auth-plus"></b>
            </div>
        </div>
    </div>
    <div class="auth__characters_hover column">
        <div class="box-between w-100">
            <div class="auth__characters_button" class:active={$gender} on:click={() => updateGender (charid, true)}>
                <div class="box red box-center">
                    <span class="auth-female"></span>
                </div>
                <div>{translateText('player2', 'Мужчина')}</div>
            </div>
            <div class="auth__characters_button" class:active={!$gender} on:click={() => updateGender (charid, false)}>
                <div class="box box-center">
                    <span class="auth-man"></span>
                </div>
                <div>{translateText('player2', 'Женщина')}</div>
            </div>
        </div>
        <InputCustom cl="auth__characters_input" setValue={(value) => FirstNameLocal = value} value={FirstNameLocal} placeholder="Имя" type="text" />
        <InputCustom cl="auth__characters_input" setValue={(value) => LastNameLocal = value} value={LastNameLocal} placeholder="Фамилия" type="text" />
        <!--<input class="auth__characters_input" placeholder="Имя" bind:value={FirstNameLocal}>
        <input class="auth__characters_input" placeholder="Фамилия" bind:value={LastNameLocal}>-->
    </div>
</div>