<script>
    import { fly, fade } from 'svelte/transition';
    import './main.sass';
    import { format } from 'api/formatter'
    import { storeSettings } from 'store/settings'
    import { executeClient } from 'api/rage'

    let notifications = [];

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
</script>

{#if notifications.length > 0}
    <div class="newhud__notification" class:list={$storeSettings.notifCount > 1}>
        {#if $storeSettings.notifCount === 1}
        <div class="newhud__background1" transition:fade={{ duration: 250 }} on:click={() => onDelete(0)}>
            <div class="box-flex box-center">
                <div class="newhud__notification_title">{notifications[0].title}</div>
                {#if notifications.length > 1}
                <div class="newhud__notification_count">x{notifications.length}</div>
                {/if}
            </div>
            <div class="box-flex box-center newhud__notification_progress_box" style="filter: none">        
                <div class="newhud__notification_progress">
                    <i style={
                        `width: ${notifications[0].percent}%;` +
                        `transition-duration:${notifications[0].percent === 0 ? notifications[0].timeout : 0}ms;`
                    }/>
                </div>
            
                <div class="newhud__notification_progress">
                    <i style={
                        `width: ${notifications[0].percent}%;` +
                        `transition-duration:${notifications[0].percent === 0 ? notifications[0].timeout : 0}ms;`
                    }/>
                </div>
            </div>
            <div class="newhud__notification_subtitle">{@html notifications[0].elements}</div>
        </div>
        {:else}
        <div class="newhud__notification_list">
            {#each new Array($storeSettings.notifCount <= 1 ? 2 : $storeSettings.notifCount) as _, index}
                {#if notifications[index]}
                <div class="newhud__notification_box {notifications[index].type}" in:fly={{ y: 50, duration: 350 }} out:fly={{ y: 50, duration: 350 }} on:click={() => onDelete(index)}>
                    <div class="newhud__notification_box_subtitle">{@html notifications[index].elements}</div>
                    <div class="newhud__notification_box_progress">
                        <i style={
                            `width: ${notifications[index].percent}%;` +
                            `transition-duration:${notifications[index].percent === 0 ? notifications[index].timeout : 0}ms;`
                        }/>
                    </div>
                </div>
                {/if}
            {/each}
        </div>
        {/if}
    </div>
{/if}