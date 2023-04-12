<script>
    let buttonEl;
    export let keyCode = 0;
    export let keyLocation = 0;
    export let callback;
    export let click;
    export let classData = "";
    export let style = "";
    export let disabled = false;
    export let nonactive = false;
    export let keyDetector = false;
    export let keyDonate = false;
    export let keyPet = false;
    export let bottom = false;
    let state = false;
    let laststate = false;

    const handleKeydown = (event) => {

        if(!nonactive) {
            if(!disabled && !state && keyCode === event.keyCode && keyLocation === event.location) {
                if(callback !== undefined) {

                    // 27 это клавиша Esc, мы отправляем её только после отжатия, чтобы не открывалась меню паузы gta
                    if(keyCode !== 27) {
                        callback(keyCode, false);
                    }
                }

                if(laststate) {
                    laststate = false;
                    setTimeout(() => laststate = true, 0);
                }
                else laststate = true; 
                state = true;
            }
        }
    }
    
    const handleKeyup = (event) => {
        if(state && keyCode === event.keyCode) {

            // 27 это клавиша Esc, мы отправляем её только после отжатия, чтобы не открывалась меню паузы gta
            if(keyCode === 27) {
                if (callback !== undefined) {
                    callback(keyCode, false);
                }
            }
            state = false;
        }
    }

    const onClick = (event) => {
        if(click !== undefined) {
            click();
        }

        if(!nonactive) {
            if(callback !== undefined)
                callback(keyCode, true);
        }
    }
</script>

<svelte:window on:keydown={handleKeydown} on:keyup={handleKeyup}/>

<div class="hudevo__playerinfo_button {classData}" class:bottom={bottom} class:active={laststate} class:keyPet={keyPet} class:keyDetector={keyDetector} class:keyDonate={keyDonate} class:pressed={state} class:disabled={disabled} class:nonactive={nonactive} on:click={onClick} bind:this={buttonEl} style={style}>
    <slot/>
</div>