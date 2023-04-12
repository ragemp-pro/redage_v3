<script>
    export let OnSelectViews;

    import { executeClient } from 'api/rage'
    import InputCustom from 'components/input/index.svelte'
    import { accountLogin } from 'store/account'
    import { onMount } from 'svelte'

    const onKeyUp = (event) => {

		const { keyCode } = event;
        if (keyCode === 13)
            onSubmitRestore ();
    }

    let restoretype = false,
        placeholder = "Логин",
        restorelength = 50,
        restoreInput = "";

	const SetStep = (step) => {        
        if(step == 0) {
			restoretype = false;
			placeholder = "Логин";
			restorelength = 50;
			restoreInput = "";
		} else if(step == 1) {
			restoretype = true;
            restorelength = 6;
            placeholder = "Код из письма";
			restoreInput = "";
		} else if(step == 2) {
            
        }
		
    }
    window.events.addEvent("cef.authentication.restoreStep", SetStep);
	
	import { onDestroy } from 'svelte';

    onDestroy(() => {
        window.events.removeEvent("cef.authentication.restoreStep", SetStep);
    });
	
    const onSubmitRestore = event => {
        if(restoreInput) {
            if (restoretype == false) {
                if (restoreInput.length != 0) {
					executeClient ('restorepass', 0, restoreInput);
					restorelength = 0;
					placeholder = "Отправка сообщения...";
					restoreInput = "";
                }
            } else {
                if (restoreInput.length == 6) {
                    executeClient ('restorepass', 1, restoreInput);
					restorelength = 0;
					placeholder = "Происходит авторизация";
					restoreInput = "";
                } else restoreInput = "";
            }
            return false;
        }
    }

</script>
<svelte:window on:keyup={onKeyUp} />
<div class="main">
    <div class="main__box">
        <div class="main__forms">
            <InputCustom setValue={(value) => restoreInput = value} value={restoreInput} placeholder={placeholder} type="text" icon="auth-user"/>
            <div class="box-flex">
                <div class="main__button main_button_size_large" style="width:100%;" on:click={onSubmitRestore}>
                    <div class="main__button_left box-center">Восстановить</div>
                    <div class="main__button_right box-center">
                        <div class="main__button_square box-center">
                            <span class="auth-arrow"/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="main__button main_button_size_large" style="width:100%; margin-top: 30px" on:click={() => OnSelectViews("Authentication")}>
                <div class="main__button_left box-center none">Назад</div>
            </div>
        </div>
        <div class="main__scroll"/>
    </div>
</div>