<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import './fonts/auth/auth.css';
    import './main.sass';

	import Start from './start/index.svelte';
	import Chars from './chars/index.svelte';

    const Views = {
        Start,
        Chars
    }

    let SelectViews = "Start";

    const OnSelectViews = (view) => {
        if (SelectViews === view)
            return;
        
        SelectViews = view;
    }

    import { onMount, onDestroy } from 'svelte'
    import { addListernEvent } from "api/functions";

    onMount(() => {
        window.events.addEvent("cef.authentication.setView", OnSelectViews);
		setTimeout(() => {
			executeClient ("client:AuthInit");
		}, 150)
    });

    onDestroy(() => {
        window.events.removeEvent("cef.authentication.setView", OnSelectViews);
    });

    let queueText = false;
    addListernEvent ("queueText", (text) => {
        queueText = text;
    })
</script>


{#if queueText}
    <div class="newauthentication__popup">
        <div class="newauthentication__popup_title">{translateText('player2', 'Вы находитесь в очереди')}</div>
        <div class="newauthentication__popup_subtitle">{queueText}</div>
        <div class="auth-login-error newauthentication__popup_iconmail"></div>
    </div>
{:else}
<svelte:component this={Views[SelectViews]} />
{/if}