<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import {executeClient, executeClientAsync} from 'api/rage'
    import { accountRedbucks, accountSubscribe } from 'store/account'
    import { GetTime } from 'api/moment'
    import moment from 'moment';

    export let SetPopup;


    const Bool = (text) => {
        return String(text).toLowerCase() === "false";
    }
    const showPopup = (item, index) => {
        if (item.id === 0) {
            if (Bool ($accountSubscribe) && $accountRedbucks < item.price)
                return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        }
        else if ($accountRedbucks < item.price)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        SetPopup ("PopupPremium", vipLists [index]);
    }

    const onReward = () => {
        executeClient ("client.donate.reward");
    }

	import { onMount, onDestroy } from 'svelte';

    let unixTime = 0;
    let unixInterval = null;

    accountSubscribe.subscribe(value => {
        if (Bool (value))
            return;
        unixTime = (value !== false ? GetTime (value).diff(GetTime ()) : 0);
        if (unixTime > (86500 - (1000 * 60))) unixTime -= (1000 * 60);
    });

	onMount(() => {
		unixInterval = setInterval(() => {
            if (unixTime > 0) {
                unixTime = GetTime ($accountSubscribe).diff(GetTime ());
                if (unixTime > (86500 - (1000 * 60))) unixTime -= (1000 * 60);
            }            
        }, 1000)
    });
    
	onDestroy(() => {
        clearInterval (unixInterval);
        unixInterval = null;
	});

    let isLoad = false;
    let vipLists = []
    executeClientAsync("donate.vipLists").then((result) => {
        if (result && typeof result === "string") {
            vipLists = JSON.parse(result);

            isLoad = true;
        }
    });
</script>

{#if isLoad}
<div id="newdonate__premium">
    {#each vipLists as item, index}
    <div class="newdonate__premium-element {item.class}">
        <div class="star-img" style="background-image: url({document.cloud + `donate/premium/${item.img}.svg`})" />
        <div class="newdonate__premium-title">{item.name}</div>
        <div class="newdonate__premium-text">
            {#each item.list as text, index}
            <b>- </b>{text}<br />
            {/each}            
        </div>
        {#if item.id === 0}
            {#if Bool ($accountSubscribe)}
            <div class="newdonate__button_small" on:click={() => showPopup (item, index)}>
                <div class="newdonate__button-text">Купить за {format("money", item.price)} RB</div>
            </div>
            {:else if unixTime > 0}
            <div class="newdonate__button_small" on:click={onReward}>
                <div class="newdonate__button-text">{moment.utc(unixTime).format("HH:mm")}</div>
            </div>
            {:else}
            <div class="newdonate__button_small" on:click={onReward}>
                <div class="newdonate__button-text">{translateText('donate', 'Забрать')}</div>
            </div>
            {/if}
        {:else}
        <div class="newdonate__button_small" on:click={() => showPopup (item, index)}>
            <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", item.price)} RB</div>
        </div>
        {/if}
    </div>
    {/each}
</div>
{/if}