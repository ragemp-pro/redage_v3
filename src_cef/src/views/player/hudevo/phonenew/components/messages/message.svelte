<script>
    import { translateText } from 'lang'
    import {executeClientAsync, executeClientToGroup, executeClientAsyncToGroup, executeClient} from 'api/rage'
    import { addListernEvent, hasJsonStructure, loadImage } from 'api/functions'
    import moment from 'moment';

    import { messageType, messageStatus, chatStatusName, formatMessage, getMessageUniqueKey, inputMaxLength } from './data.js'

    export let onSelectNumber;
    export let selectedNumber;

    let inputValue = "";
    executeClientAsyncToGroup("getDraftMessages", selectedNumber).then((result) => {
        inputValue = result;
    });

    let isSmile = false;
    let isFocus = false;

    let contactData = {};
    executeClientAsyncToGroup("getContact", selectedNumber).then((result) => {
        if (hasJsonStructure(result))
            contactData = JSON.parse(result);
    });

    let messages = [];

    let isLoad = false;


    executeClientAsyncToGroup("getMessage", selectedNumber).then((result) => {//Запомнить на клиенте ласт чела которого открыл
        if (hasJsonStructure(result))
            messages = JSON.parse(result);
    });

    addListernEvent ("messageInit", (result) => {
        if (hasJsonStructure(result))
            messages = JSON.parse(result)
    })

    //

    let chatElement;
    let IsScrolled = false;
    let ScrollDown = false;
    let isLoadMessage = false;
    let loadMessageTime = null;

    const OnScroll = e => {

        const chatHeight = chatElement.offsetHeight;
        const scrollHeight = e.target.scrollHeight;
        const scrollTop = e.target.scrollTop;

        IsScrolled = true;
        ScrollDown = (scrollHeight - scrollTop > chatHeight * 1.5);

        if (scrollTop === 0) {
            let messagesValues = Object.values(messages);
            if (!isLoadMessage && messagesValues.length >= 1) {
                isLoadMessage = true;
                //
                executeClientAsyncToGroup ("requestMessages", messagesValues[0].Id).then((result) => {
                    if (result && typeof result === "string") {
                        messages = [
                            ...JSON.parse(result),
                            ...messages
                        ];

                        loadMessageTime = setTimeout(() => {
                            isLoadMessage = false;
                        }, 500);
                    }

                });
            }
        }
    }


    const OnScrollDown = () => {
        if (chatElement) {
            chatElement.scrollTop = chatElement.scrollHeight;
        }
    }

    import {beforeUpdate, afterUpdate, onMount } from 'svelte';

    let autoscroll;

    beforeUpdate(() => {
        autoscroll = chatElement && (chatElement.offsetHeight + chatElement.scrollTop) > (chatElement.scrollHeight - 20);
    });

    afterUpdate(() => {
        if (autoscroll)
            chatElement.scrollTo(0, chatElement.scrollHeight);
    });


    $: if (messages) {
        if (!IsScrolled) {
            setTimeout(() => OnScrollDown(), 0);
        } else {
            if (chatElement && chatElement.scrollTop === 0) {
                chatElement.scrollTop = 1; // Фикс перекидывания в начало новых сообщений
            }
        }
    }

    //

    const handleUp = e => {
        switch (e.keyCode) {
            case 13:
                onSend(messageType.text);
                break;
        }
    }

    const onSend = (type = messageType.text) => {
        if (inputValue.trim() && inputValue.length > 0 && inputValue.length < inputMaxLength + 1) {


            const key = getMessageUniqueKey();

            if (type === messageType.text)
                inputValue = format("stringify", inputValue);

            messages.push({
                Key: key,
                Text: inputValue,
                Date: -1,
                Me: true,
                Type: type,
                Status: messageStatus.sent,
                Id: key,
            });

            executeClientToGroup ('sendMsg', key, inputValue, type);

            messages = messages;

            inputValue = "";
            executeClientToGroup ('draftMessages', inputValue);

            setTimeout(() => OnScrollDown(), 0);
        }
    }

    addListernEvent ("updMsgStatus", (key, date, status) => {
        const index = messages.findIndex(m => m.Key === key);

        if (messages [index]) {
            messages [index].Date = date;
            messages [index].Status = status;
        }

    })

    addListernEvent ("msgAdd", (text, date, type) => {
        messages.push({
            Text: text,
            Date: date,
            Me: false,
            Type: type,
            Status: messageStatus.received
        });

        messages = messages;
    })

    import { onDestroy } from 'svelte';
    onDestroy(async () => {

        if (loadMessageTime !== null)
            clearTimeout(loadMessageTime);

        onEndWrite (false);

        onInputBlur ();

        executeClientToGroup ('closeMessage');
    });

    //Статус

    let chatStatus = 0;

    executeClientToGroup ('getPhoneChatStatus', selectedNumber);

    addListernEvent ("phoneChatUpdStatus", (id) => {
        chatStatus = id;
    })

    let writeTime = null;
    let isWrite = false;
    const onStartWrite = () => {

        if (!isWrite) {
            isWrite = true;
            executeClientToGroup ('startWrite');
        }

        if (writeTime !== null)
            clearTimeout(writeTime);

        writeTime = setTimeout(() => onEndWrite(), 1000 * 10)
    }

    const onEndWrite = (isTime = true) => {

        if (isWrite) {
            isWrite = false;
            executeClientToGroup ('endWrite');
        }

        if (!isTime && writeTime !== null)
            clearTimeout(writeTime);

        writeTime = null;
    }

    //

    let isPopupOpened = false;

    import Popup from './popup.svelte'
    import { format } from "api/formatter";

    const sendGeo = () => {
        executeClientAsync("getPosition").then((result) => {
            if (result && typeof result === "string") {
                result = JSON.parse(result);
                inputValue = JSON.stringify([result.x, result.y]);
                onSend(messageType.map);
                onClosePopup();
            }
        });
    }

    const onClosePopup = () => isPopupOpened = false;

    const getPosition = (text) => {
        text = JSON.parse(text);

        return text;
    }

    import EmojiIndex from './emoji/index.svelte'

    let inputDiv;
    const addSmile = (smile) => {
        isSmile = false;
        inputValue += smile;

        if (inputDiv) {
            inputDiv.focus();
        }
    }

    const openPopup = () => {
        isPopupOpened = true;
        isSmile = false;
    }

    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    const onFocus = () => {
        isSmile = false;
        isFocus = true;
        onInputFocus ()
    }

    const onBlur = () => {
        isFocus = false;
        onInputBlur ()

        onEndWrite (false);
    }

    const onInput = () => {
        if (isFocus) {
            onStartWrite();

            if (inputValue === undefined)
                inputValue = "";

            executeClientToGroup ('draftMessages', inputValue);
        }
    }
    //window.listernEvent.messageInit ('[{"Text":"sfdd","Date":"2021-12-18T17:26:33","Me":true,"Type":0,"Status":1,"Id":99},{"Text":"dsfsdf:innocent:","Date":"2021-12-18T17:26:42","Me":true,"Type":0,"Status":1,"Id":100},{"Text":"sadsadasd:heart_eyes: asdasd aa:boom:","Date":"2021-12-18T17:27:03","Me":true,"Type":0,"Status":1,"Id":101},{"Text":":monkey_face: РђРІС‹РІР°С‹РІ","Date":"2021-12-18T20:58:18","Me":true,"Type":0,"Status":1,"Id":102},{"Text":"Р°С…СѓРµР»?","Date":"2021-12-18T20:58:35","Me":false,"Type":0,"Status":1,"Id":103},{"Text":"nS","Date":"2021-12-18T20:58:40","Me":true,"Type":0,"Status":1,"Id":104},{"Text":"Р«","Date":"2021-12-18T20:58:41","Me":true,"Type":0,"Status":1,"Id":105},{"Text":"РђС…СѓРµР»","Date":"2021-12-18T20:58:44","Me":true,"Type":0,"Status":1,"Id":106},{"Text":"СЏСЃРЅРѕ РґРµР±РёР»","Date":"2021-12-18T20:58:53","Me":false,"Type":0,"Status":1,"Id":107},{"Text":":blush:","Date":"2021-12-18T20:58:54","Me":true,"Type":0,"Status":1,"Id":108},{"Text":":heart_eyes:","Date":"2021-12-18T20:58:57","Me":false,"Type":0,"Status":1,"Id":109},{"Text":"С…РѕС‡Сѓ С‚РµР±СЏ С‚СЂР°С…РЅСѓС‚СЊР»","Date":"2021-12-18T20:59:01","Me":false,"Type":0,"Status":1,"Id":110},{"Text":":stuck_out_tongue:","Date":"2021-12-18T20:59:01","Me":true,"Type":0,"Status":1,"Id":111},{"Text":"РІ Р°РЅР°Р»СЊС‡РёРє","Date":"2021-12-18T20:59:03","Me":false,"Type":0,"Status":1,"Id":112},{"Text":":heart_eyes:","Date":"2021-12-18T20:59:06","Me":false,"Type":0,"Status":1,"Id":113}]');

    const sendImage = (link) => {
        if (link && typeof link === "string") {
            inputValue = link;
            onSend(messageType.img);
            onClosePopup();
        }
    }

    /*$: if (/((http|https):\/\/)?(\S)+\.(jpg|jpeg|png|gif)($|(#|\?))/i.test(inputValue)) {
        const img = new Image();
        img.onload = function () {
            inputValue = inputValue;
            onSend(messageType.img);
            onClosePopup();
        }
        img.src = inputValue;
    }*/

    addListernEvent ("cameraLink", sendImage);

    const getAvatar = (avatar) => {
        if (typeof avatar === "string" && avatar.length > 6)
            return avatar;

        return "";
    }

    //Действие при нажатии в чате

    const onMessage = (message) => {
        if (message && typeof message.Type !== "undefined") {
            if (message.Type === messageType.map) {
                const pos = getPosition (message.Text);
                executeClient("createWaypoint", pos[0], pos[1]);
            }
        }
    }


    //messages = JSON.parse(`[{"Text":"РєСѓ","Date":"2021-12-20T20:14:48","Me":false,"Type":0,"Status":1,"Id":140},{"Text":":innocent:","Date":"2021-12-20T20:14:49","Me":true,"Type":0,"Status":1,"Id":141},{"Text":"С„С‹РІС„С‹РІ","Date":"2021-12-20T20:14:51","Me":false,"Type":0,"Status":1,"Id":142},{"Text":":kissing_closed_eyes:","Date":"2021-12-20T20:14:54","Me":true,"Type":0,"Status":1,"Id":143},{"Text":"РєСѓ","Date":"2021-12-20T20:15:00","Me":false,"Type":0,"Status":1,"Id":144},{"Text":"[-2694.196044921875,-66.73029327392578]","Date":"2021-12-20T20:17:57","Me":true,"Type":1,"Status":1,"Id":145},{"Text":"[-2729.5,3.25022554397583]","Date":"2021-12-20T20:18:29","Me":true,"Type":1,"Status":1,"Id":146},{"Text":"РµР±Р»Р°РЅ?","Date":"2021-12-21T20:41:04","Me":false,"Type":0,"Status":1,"Id":147},{"Text":"РєСѓ","Date":"2021-12-21T20:41:15","Me":false,"Type":0,"Status":1,"Id":148},{"Text":"РµР±Р»Р°РЅ РѕРє","Date":"2021-12-21T20:41:25","Me":false,"Type":0,"Status":1,"Id":149},{"Text":"[386.09814453125,6616.53857421875]","Date":"2021-12-21T20:41:39","Me":false,"Type":1,"Status":1,"Id":150},{"Text":":heart_eyes:","Date":"2021-12-21T20:42:17","Me":false,"Type":0,"Status":1,"Id":151},{"Text":"https://api.redage.net/image/11_121/23/1640218493123.jpg","Date":"2021-12-23T03:14:51","Me":true,"Type":2,"Status":1,"Id":152},{"Text":"РїРµРґРёРє?)","Date":"2021-12-23T19:27:58","Me":false,"Type":0,"Status":1,"Id":153},{"Text":"С…РµР№ С…РµР№ С…Р° СЌС‚Рѕ РґРёСЃСЃ РЅР° РїРµС‚СѓС…Р° С…Р° С…Р°","Date":"2021-12-23T19:37:28","Me":false,"Type":0,"Status":1,"Id":154}]`)



    //

    import Map from './../map/index.svelte'


    let oneElementData = {};
    const addElementData = (node, type) => {
        node = node.getBoundingClientRect();
        if (node) {
            oneElementData [type] = {
                width: node.width,
                height: node.height,
            }
        }
    }



    import { TimeFormatStartOf } from 'api/moment'
    import { fade } from 'svelte/transition'
    import {currentView} from "@/views/player/hudevo/phonenew/components/calls/stores";
    import {currentPage} from "@/views/player/hudevo/phonenew/stores";


    const onContact = () => {
        currentView.set ("contacts");
        currentPage.set ("call");
    }
</script>

<svelte:window on:keyup={handleUp} />

{#if isPopupOpened}
    <Popup {selectedNumber} {sendGeo} {sendImage} {onClosePopup} />
{/if}

<div class="box-center box-between w-1 newphone__project_padding16" in:fade>
    <div class="phoneicons-Button" on:click={() => onSelectNumber(-1)}></div>
    <div class="box-column box-center">
        <div class="newphone__messages_name text-center w-162">{contactData.Name}</div>
        <div class="gray text-center">{chatStatusName [chatStatus]}</div>
    </div>
    <div class="newphone__settings_bigicon n-m" on:click={onContact} class:phoneicons-contacts={!getAvatar(contactData.Avatar).length} use:loadImage={getAvatar(contactData.Avatar)}></div>
</div>
<div class="newphone__messages_chat" on:scroll={OnScroll} bind:this={chatElement} in:fade>
    {#each messages as message}
        <div class="newphone__messages_sms" class:me={message.Me} class:map={message.Type === messageType.map} class:image={message.Type === messageType.img} on:click={() => onMessage (message)}>
            {#if [messageType.map].includes(message.Type) && typeof oneElementData[message.Type] === "undefined"}
                <div class="getElementData" use:addElementData={message.Type} />
            {/if}

            {#if message.Type === messageType.text}
                <div class="message">{@html formatMessage (message.Text)}</div>
            {:else if message.Type === messageType.map && oneElementData [messageType.map]}
                <Map getPosition={getPosition (message.Text)} elementWidth={oneElementData [messageType.map].width} elementHeight={oneElementData [messageType.map].height} />
            {:else if message.Type === messageType.img}
                <img class="message__img" src='' use:loadImage={message.Text} />
            {/if}

            {#if message.Status === messageStatus.sent}
                <div class="message__time">{translateText('player2', 'Отправка...')}</div>
            {:else if message.Status === messageStatus.error}
                <div class="message__time message__error">{translateText('player2', 'Ошибка')}</div>
            {:else if message.Date}
                <div class="message__time">{TimeFormatStartOf(message.Date, 'minute')}</div>
            {/if}
        </div>
    {/each}
</div>

<div class="newphone__messages_bottom newphone__project_padding16" in:fade>
    {#if !contactData.IsSystem}
    <div class="phoneicons-add1 newphone__messages_smallicon m-0" on:click={openPopup}></div>
    {/if}
    <input type="text" class="newphone__messages_input" placeholder={`Написать ${contactData.Name}`} bind:value={inputValue} maxlength={inputMaxLength} bind:this={inputDiv} on:focus={onFocus} on:blur={onBlur} on:input={onInput} />
    {#if !contactData.IsSystem}
    <div class="phoneicons-smile newphone__messages_smallicon m-0 mr-6" on:click={()=> isSmile = !isSmile}></div>
    {/if}
    {#if isSmile}
        <EmojiIndex {addSmile} />
    {/if}
    <div class="phoneicons-send1 newphone__messages_smallicon m-0 small" on:click={() => onSend (messageType.text)}></div>
</div>