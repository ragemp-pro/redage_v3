<script>
    import { translateText } from 'lang'
    import moment from 'moment';
    import { setTime, elapsedUp } from 'api/moment'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { pageBack } from './../../stores'

    let getComingPhone = false;

    const updateStatus = () => {
        executeClientAsyncToGroup("getComingPhone").then((result) => {
            if (result && typeof result === "string")
                getComingPhone = JSON.parse(result);
        });
    }

    updateStatus ()

    let isMute = false;

    const updateMute = () => {
        isMute = !isMute;
        executeClientToGroup ('mute', isMute);
    }

    const upPhone = () => {
        executeClientToGroup ('take');
    }

    const downPhone = () => {
        executeClientToGroup ('put');
        executeClientToGroup ('mute', false);
    }

    import { addListernEvent } from 'api/functions'
    addListernEvent ("callAccept", updateStatus)
    addListernEvent ("downPhone", pageBack)

    const getAvatar = (avatar) => {
        if (typeof avatar === "string" && avatar.length > 6)
            return `background-image: url(${avatar})`;

        return "";
    }

    import { onMessage } from "@/views/player/hudevo/phonenew/data";

    const onSystemMessage = (number) => {
        onMessage (number);
    }

    addListernEvent ("phone.call.onMessage", onSystemMessage)
    import { fade } from 'svelte/transition'

    let background = ""
    executeClientAsyncToGroup("settings.wallpaper").then((result) => {
        background = result;
    });
</script>
<div class="newphone__background over-hiden" in:fade style="background-image: url({background})">
    <div class="newphone__callview">
        <Header />
        <div class="newphone__callview_content">
            <div class="newphone__contacts_bigicon" class:phoneicons-contacts={!getAvatar(getComingPhone.Avatar).length} style={getAvatar(getComingPhone.Avatar)}></div>

            <div class="newphone__callview_name newphone__project_padding16 newphone__shadow">{getComingPhone.Name}</div>
            <div class="gray newphone__shadow">{!getComingPhone.isComing ? translateText('player2', 'Вызов..') : translateText('player2', 'Идет разговор..')}</div>
            <div class="newphone__calview_center">
                {#if getComingPhone.isComing}
                <div class="newphone__dial_element" class:active={isMute} on:click={updateMute}>
                    <div class="phoneicons-mic-off1"></div>
                </div>
                {/if}
            </div>
            <div class="newphone__calview_flex" class:isCall={!(!getComingPhone.isCall && !getComingPhone.isComing)}>
                <div class="box-column">
                    <div class="newphone__dial_element decline" on:click={downPhone}>
                        <div class="phoneicons-decline"></div>
                    </div>
                    <div>{translateText('player2', 'Отклонить')}</div>
                </div>
                {#if !getComingPhone.isCall && !getComingPhone.isComing}
                <div class="box-column">
                    <div class="newphone__dial_element call" on:click={upPhone}>
                        <div class="phoneicons-call"></div>
                    </div>
                    <div>{translateText('player2', 'Принять')}</div>
                </div>
                {/if}
            </div>
        </div>
        <HomeButton />
    </div>
</div>