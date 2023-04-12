<script>
    import { translateText } from 'lang'
    import './main.sass'
    import { executeClient } from 'api/rage'
    
    export let isMerger;
    export let SetMerger;


    let password = "";

    let progressValue = 0;


    let serverId = 2;

    const onSubmitAuth = () => {
        if (!isMerger) 
            return;
        if (password && (serverId == 2 || serverId == 3)) {
            SetMerger (false);
            executeClient ("client.merger.auntification", password, serverId);
        }
    }
    
    const SetProgressBar = (value) => {
        if (value == -1) {
            SetMerger (true);
            window.notificationAdd(4, 9, translateText('player2', 'Данные введены неверно'), 3000);
        } else if (value == -2) {
            SetMerger (true);
            window.notificationAdd(4, 9, translateText('player2', 'Слишком быстро'), 3000);
        } else if (value == -3) {
            SetMerger (true);
            window.notificationAdd(4, 9, translateText('player2', 'Свободных слотов для переноса нет!'), 3000);
        } else {
            progressValue = value;
        }
    }
    
    import { onMount, onDestroy } from 'svelte'
    import {isInput} from "@/views/player/newauthentication/store";

    onMount(() => {
        window.events.addEvent("cef.merger.progress", SetProgressBar);
    });

    onDestroy(() => {
        window.events.removeEvent("cef.merger.progress", SetProgressBar);
    });

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }
    
    const onKeyUp = (event) => {
        if (!isMerger) 
            return;
        const { keyCode } = event;
        
        if(keyCode == 13) {
            onSubmitAuth ();
        }
    }
    const onFuncFocus = () => {
        isInput.set(true);
    }

    const onFuncBlur = () => {
        isInput.set(false);
    }
</script>

<svelte:window on:keyup={onKeyUp} />
<div class="popups-auth">
    <div class="box-auth" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
        <div class="title">{translateText('player2', 'Перенос персонажей с RedAge White и Red')}</div>
        {#if progressValue < 1}
        <div class="animated fadeIn" style="width: 100%;">
            <div class="box-flex">
                <label class="container">White
                    <input type="checkbox" checked={serverId === 2} on:click={() => serverId = 2}>
                    <span class="checkmark"></span>
                </label>
                <label class="container">Red
                    <input type="checkbox" checked={serverId === 3} on:click={() => serverId = 3}>
                    <span class="checkmark"></span>
                </label>
            </div>
            <div class="label-input">
                <input type="password" bind:value={password} class="entry-login" name="password" id="entry-login-id" placeholder={translateText('player2', 'Ваш пароль от выбранного сервера')} required on:focus={ onFuncFocus } on:blur={ onFuncBlur } />
                <div class="error"></div>
            </div>
            <div class="button">
                <input type="submit" class="btn red w-100" value="Начать процесс переноса" on:click|preventDefault={onSubmitAuth}/>
            </div>
        </div>
        {:else}
        <div class="progress">
            <progress max="100" value={progressValue}></progress>
            <div class="progress-value"></div>
            <div class="progress-bg"><div class="progress-bar"></div></div>
        </div>
        {/if}
    </div>
</div>