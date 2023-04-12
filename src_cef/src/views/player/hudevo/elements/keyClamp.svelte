<script>
    import { translateText } from 'lang'
    import ProgressBar from 'progressbar.js';
    import { fade } from 'svelte/transition';
    import { executeClient } from 'api/rage'
    import keysName from 'json/keys.js'
    
    let toggled = false;
    let ProgressBarId;
    let keyCode = 0;

	const CreateProgressBar = () => {
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
        ProgressBarId.setText(keysName [keyCode]);
        ProgressBarId.text.style.color = 'black';
    };

    const UpdateProgressValue = (value) => {
        ProgressBarId.set(value / 100);
    }

    const OpenData = (_keyCode) => {
        keyCode = _keyCode;
        toggled = true;
    }

    const CloseData = () => {
        toggled = false;
    }
    
	window.events.addEvent("cef.KeyClamp.open", OpenData);
	window.events.addEvent("cef.KeyClamp.updateProgress", UpdateProgressValue);
	window.events.addEvent("cef.KeyClamp.close", CloseData);
</script>
{#if toggled}
<!--<div class="hudevo__usebutton" transition:fade={{ duration: 500 }}>Удерживайте</div>-->
<div class="hudevo__bottom_usebutton" transition:fade={{delay: 20, duration: 0}}>
    <div id="ProgressBarKey" use:CreateProgressBar>
        <div class="hudevo-usebutton__button" />
    </div>
    <div class="hudevo-usebutton__text">{translateText('player2', 'Зажмите кнопку')}</div>
</div>
{/if}