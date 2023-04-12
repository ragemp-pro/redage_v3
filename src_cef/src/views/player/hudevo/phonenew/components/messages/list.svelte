<script>
    import { translateText } from 'lang'
    import { executeClientAsyncToGroup } from 'api/rage'
    import { loadImage } from 'api/functions'
    import { TimeFormatStartOf } from 'api/moment'
    import { messageType, formatMessage } from './data.js'

    export let onSelectNumber;

    let messages = [];

    //messages = JSON.parse(`[{"Number":41570,"Avatar":"https://api.redage.net/image/11_121/23/1640267825898.jpg","Name":"Vitala","Date":"2021-12-23T19:37:28","Text":"С…РµР№ С…РµР№ С…Р° СЌС‚Рѕ РґРёСЃСЃ РЅР° РїРµС‚СѓС…Р° С…Р° С…Р°","Type":0,"IsMe":true,"Status":true}]`)

    executeClientAsyncToGroup("getMessages").then((result) => {
        if (result && typeof result === "string")
            messages = JSON.parse(result);
    });

    const filterCheck = (data, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();
        let success = false;

        if (data.Name.toString().toUpperCase().includes(text)) {
            success = true;
        }
        if (data.Number.toString().toUpperCase().includes(text)) {
            success = true;
        }
        return success;
    }

    let searchText;


    const getAvatar = (avatar) => {
        if (typeof avatar === "string" && avatar.length > 6)
            return avatar;

        return "";
    }
    let isPopup = false;

    const closePopup = () => isPopup = false;

    import PopupMessag from './popupMessage.svelte'
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>

<div class="box-center box-between w-1 newphone__project_padding16" in:fade>
    <div class="newphone__maps_header">{translateText('player2', 'Чаты')}</div>
    <div class="newphone__contacts_circle" on:click={() => isPopup = true}>+</div>
</div>
<div class="newphone__contacts_inputblock newphone__project_padding16" in:fade>
    <div class="phoneicons-loop"></div>
    <input type="text" class="newphone__contacts_input" placeholder="Поиск" bind:value={searchText} on:focus={onInputFocus} on:blur={onInputBlur}>
</div>
<div class="newphone__messages_list" in:fade>
    {#if isPopup}
        <PopupMessag {onSelectNumber} {closePopup} />
    {/if}
    {#if messages && typeof messages === "object" && messages.length}
        {#each messages.filter(el => filterCheck(el, searchText)) as message}
            <div class="newphone__messages_element" on:click={() => onSelectNumber(message.Number)}>

                <div class="newphone__circle_count" class:isNotVisible={message.Status}></div>

                <div class="newphone__settings_bigicon" class:phoneicons-contacts={!getAvatar(message.Avatar).length} use:loadImage={getAvatar(message.Avatar)}></div>

                <div class="box-column pr-13">
                    <div class="box-between">
                        <div class="newphone__messages_name">{message.Name}</div>
                        <div class="newphone__messages_time">
                            {TimeFormatStartOf(message.Date, 'minute')}
                        </div>
                    </div>
                    {#if message.IsWrite}
                        <div class="newphone__messages_message">{translateText('player2', 'Что-то печатает..')}</div>
                    {:else if message.DraftText}
                        <div class="newphone__messages_message">{translateText('player2', 'Черновик')}: {message.DraftText}</div>
                    {:else}
                        {#if message.Type === messageType.text}
                            <div class="newphone__messages_message">{@html formatMessage (message.Text)}</div>
                        {:else if message.Type === messageType.map}
                            <div class="newphone__messages_message">{translateText('player2', 'Геопозиция')}</div>
                        {:else if message.Type === messageType.img}
                            <div class="newphone__messages_message">{translateText('player2', 'Фотография')}</div>
                        {/if}
                    {/if}
                </div>
            </div>
        {/each}
    {/if}
</div>