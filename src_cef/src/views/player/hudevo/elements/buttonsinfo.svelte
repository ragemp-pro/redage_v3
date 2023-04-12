<script>
    import { translateText } from 'lang'
    import { charIsPet } from 'store/chars'
    import { fly } from 'svelte/transition';
    import { isInputToggled, isHelp } from 'store/hud'
    import keys from 'store/keys'
    import keysName from 'json/keys.js'
	import router from 'router';

    let isHint = false;
    window.hudStore.isHint = (value) => isHint = value;

    let HideHelp = false;
    window.hudStore.HideHelp = (value) => HideHelp = value;
</script>
{#if !$charIsPet && $isHelp}
    <div class="buttonsinfo" in:fly={{ x: -50, duration: 500 }} out:fly={{ x: -50, duration: 250 }} class:newhud__hide={HideHelp || !$isHelp}>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[8]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Анимации')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[43]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Биндер')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[45]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Указать пальцем')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[27]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Скрыть HUD')}</div>
        </div>
        <div class="box-flex">
            <div class="buttonsinfo__button">{keysName[$keys[49]]}</div>
            <div class="buttonsinfo__element hudevo__elementparams">{translateText ('player2' , 'Скрыть уведомление')}</div>
        </div>
    </div>
{/if}