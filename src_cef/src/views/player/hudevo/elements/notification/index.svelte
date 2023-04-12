<script>
    import { fly, fade } from 'svelte/transition';
    import './main.sass';
    import { format } from 'api/formatter'
    import { storeSettings } from 'store/settings'
    import { executeClient } from 'api/rage'

    let notifications = [];

/*     let notifications = [{
        type: "black",
		title: "ЕTitle",
		elements: "ХУй",
		timeout: 12,
		endtime: -1,
		percent: 100,
    }]; */

    window.notification = (json) => {
        if (json) {
            json = JSON.parse(json);
            for (let i = 0; i < json.length; i++) {
                if (typeof json [i] === "object")
                    json [i].text = format("parse", json [i].text);
            }
        }

        notifications = json;
    }

    window.notificationAdd = (type, _, msg, time) => {
        executeClient ("notify", type, _, msg, time);
    }

    const onDelete = (index) => {
        executeClient ("notifyClear", index);
    }

    export let night;

</script>

{#if notifications.length > 0}
    <div class="newhud__notification list" class:night={night}>
        <div class="newhud__notification_list">
            {#each new Array($storeSettings.notifCount <= 1 ? 1 : $storeSettings.notifCount) as _, index}
                {#if notifications[index]}
                <div class="hudevo__notification {notifications[index].type}" in:fly={{ y: 50, duration: 350 }} out:fly={{ y: 50, duration: 350 }} on:click={() => onDelete(index)}>
                    <!--<div class="hudevo__notification_title">Уведомление</div>-->
                    <div class="hudevo__notification_count">
                        <div class="activeline {notifications[index].type}" style={
                            `width: ${notifications[index].percent}%;` + `transition-duration:${notifications[index].percent === 0 ? notifications[index].timeout : 0}ms;`}></div>
                    </div>
                    <div class="hudevo__notification_text">{@html notifications[index].elements}</div>
                </div>
                {/if}
            {/each}
        </div>
    </div>
{/if}