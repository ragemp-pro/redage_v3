<script>
    import { translateText } from 'lang'
    import { gender, updateGender, updateIndex, initCustom } from 'store/customization';
    import Create from '@/views/player/newauthentication/chars/create/index.svelte';
    import CreateNewCustomization from 'store/random/index.js'
    import { executeClient } from 'api/rage'

    updateIndex (0);
    CreateNewCustomization (false);
    CreateNewCustomization (true);
    updateGender (true);

    setTimeout(() => {
        initCustom (0);
    }, 50)

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }

</script>

<div id="newauthentication">
    <div class="newauthentification__characters">

        <div class="box-flex">
            <div class="main__scroll big"></div>
            <div class="auth__characters" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                <div class="auth__characters_block" class:active={true}>
                    <div class="auth__characters_character">
                        <div class="box-column">
                            <b>{translateText('player', 'Создать')}</b>
                            <div>{translateText('player', 'Персонажа')}</div>
                        </div>
                        <div class="auth__characters_circle orange">
                            <div class="auth__characters_circle_text">
                                <b class="auth-plus"></b>
                            </div>
                        </div>
                    </div>
                    <div class="auth__characters_hover column">
                        <div class="box-between w-100">
                            <div class="auth__characters_button" class:active={$gender} on:click={() => updateGender (0, true)}>
                                <div class="box red box-center">
                                    <span class="auth-female"></span>
                                </div>
                                <div>{translateText('player', 'Мужчина')}</div>
                            </div>
                            <div class="auth__characters_button" class:active={!$gender} on:click={() => updateGender (0, false)}>
                                <div class="box box-center">
                                    <span class="auth-man"></span>
                                </div>
                                <div>{translateText('player', 'Женщина')}</div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <Create isSendCreator={true} />
    </div>
</div>