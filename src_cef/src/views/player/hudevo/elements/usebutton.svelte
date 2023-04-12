<script>
    import ProgressBar from 'progressbar.js';
    import { fade } from 'svelte/transition';
    import { executeClient } from 'api/rage'
    import keysName from 'json/keys.js'
    let click = false;


    let ProgressBarId;

    const keyType = {
        0: "click",
        1: "holde",
    }

    const keyTypeToHelp = {
        "click": "Кликайте",
        "holde": "Нажмите один раз",
    }


    let state = {
        toggled: false,
        progress: 0, // прогресс если down:false - то 100
        keyCode: -1, // ID клавиши
        intervalId: null, 
        isTimerDown: false, // Тип таймера Если down:true  - то таймер идет с низу вверхи, если false то вниз
        isDown: false,
        keyType: keyType [0], //Если true то зажать
        InteravalSpeed: 8, //Скорость отнимания прогресса
        canStart: true,

        antiAutoClickTime: new Date().getTime()
    }
    
	const CreateProgressBar = () => {
        if (state.keyType === keyType[0] || state.keyType === keyType[1]) {
            ProgressBarId = new ProgressBar.Square("#ProgressBarKey", {
                color: '#F82847',
                strokeWidth: 8,
                trailWidth: 8,
                easing: 'easeInOut',
                duration: 900,
                trailColor: 'rgba(255, 255, 255, 0)', //цвет фонового кружка
                text: {
                    autoStyleContainer: false
                },
                from: { color: '#F82847', width: 8 },
                to: { color: '#F82847', width: 8 },
                // Set default step function for all animate calls
                step: function(state, circle) {
                    circle.path.setAttribute('stroke-width', state.width);
                }
            });
            ProgressBarId.setText(keysName [state.keyCode]);
            ProgressBarId.text.style.color = 'black';
        }
    };
    

    const StartTimer = () => {
        if (state.intervalId !== null) return;
        if (state.keyType === keyType[0] || state.keyType === keyType[1]) {
            state.intervalId = setInterval (() => {
                if (state.isTimerDown) {                    
                    if (state.keyType === keyType[0]) {
                        state.progress += 50;
                        state.isTimerDown = false;//Чо бы кликали а не зажимали
                    } else {
                        if (state.progress < 10) state.progress += 1;
                        else if (state.progress < 100) state.progress += (state.progress / 10);
                        else state.progress += (state.progress / 50);
                    }
                    if(click == false) { 
                        executeClient("cef.hud.game.startAnim");
                        click = true;
                    }
                } else {
                    state.progress -= 1; 
                    if (click == true && state.keyType === keyType[1]) {
                        executeClient("cef.hud.game.stopAnim");
                        click = false;
                    }
                }

                if (state.progress < 1) {
                    state.progress = 0;
                    if(click == true && state.keyType === keyType[0]) {
                        executeClient("cef.hud.game.stopAnim");
                        click = false;
                    }
                } 
                if (state.progress < 1000) {
                    ProgressBarId.set(state.progress / 1000);
                } else {
                    Clear ();
                    executeClient("cef.hud.game.end");
                    click = false;
                    //Конец
                }
            }, state.InteravalSpeed);
        }
    }


    const Clear = () => {
        if (state.intervalId !== null) clearInterval (state.intervalId);

        state = {
            toggled: false,
            progress: 0, // прогресс если down:false - то 100
            keyCode: 0, // ID клавиши
            intervalId: null,
            isTimerDown: true, // Тип таймера Если down:true  - то таймер идет с низу вверхи, если false то вниз
            isDown: false,
            keyType: keyType [0],
            InteravalSpeed: 8,
            canStart: true
        }
    }


    const handleKeyDown = (event) => {
        if(state.toggled && !state.isDown && state.keyCode === event.keyCode && state.canStart === true) {
            if (125 > (new Date().getTime() - state.antiAutoClickTime)) 
                return;
            state.antiAutoClickTime = new Date().getTime();
            state.isTimerDown = true;
            state.isDown = true;
            StartTimer ();
        }
    }
    
    const handleKeyUp = (event) => {
        if(state.toggled && state.keyCode === event.keyCode && state.canStart === true) {
            state.isTimerDown = false;
            state.isDown = false;
        }
    }

    const OpenData = (keyCode, holde, InteravalSpeed = 8, canStart = true) => {
        if (keyCode !== undefined && holde !== undefined) {
            if (state.intervalId !== null) clearInterval (state.intervalId);
            state.intervalId = null;
            state.progress = true;
            state.isTimerDown = false;
            state.isDown = false;

            state.keyType = keyType [holde];
            
            state.keyCode = keyCode;

            state.InteravalSpeed = !InteravalSpeed ? 8 : InteravalSpeed;

            state.toggled = true;
            
            state.canStart = canStart;
        }
    }

    const UpdateProgress = (progress) => {
        if (ProgressBarId) ProgressBarId.animate(progress);
    }

    const CloseData = (data) => {
        if (state.toggled) {
            Clear ();
        }
    }

	window.events.addEvent("cef.hud.game.open", OpenData);
	window.events.addEvent("cef.hud.game.updateProgress", UpdateProgress);
	window.events.addEvent("cef.hud.game.close", CloseData);
</script>
<svelte:window on:keydown={handleKeyDown} on:keyup={handleKeyUp}/>

{#if state.toggled}
<!--<div class="newhud__usebutton" transition:fade={{ duration: 500 }}>Удерживайте</div>-->
<div class="hudevo__bottom_usebutton" transition:fade={{delay: 20, duration: 0}}>
    <div id="ProgressBarKey" use:CreateProgressBar>
        <div class="hudevo-usebutton__button" class:anim={state.keyType === keyType[0]} class:animDown={state.isDown} />
    </div>
    <div class="hudevo-usebutton__text">{keyTypeToHelp[state.keyType]}</div>
</div>
{/if}