
<script>
    import { translateText } from 'lang'

    import {validate} from "api/validation";
    import {executeClientToGroup} from "api/rage";
    export let onSelectNumber;
    export let closePopup;

    let numberValue;
    let inputValue;

    import { messageType, getMessageUniqueKey, inputMaxLength } from './data.js'
    import { format } from "api/formatter";
    import { fade } from 'svelte/transition'

    const onSend = (type = messageType.text) => {

        let check;

        const number = Number (numberValue);
        check = validate("phonenumber", number);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        if (inputValue.trim() && inputValue.length > 0 && inputValue.length < inputMaxLength + 1) {

            const key = getMessageUniqueKey();

            if (type === messageType.text)
                inputValue = format("stringify", inputValue);

            executeClientToGroup ('sendPopupMsg', number, key, inputValue, type);

            onSelectNumber (number)
        }
    }
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>

<div class="newphone__dial_popup" in:fade>
    <div class="newphone__dial_popupback">
        <div class="box-between">
            <div />
            <div class="phoneicons-add1" on:click={closePopup}></div>
        </div>
        <div class="newphone__addcontact_element mt-15">
            <div class="gray">{translateText('player2', 'Кому')}</div>
            <input type="text" class="newphone__addcontact_input" placeholder={translateText('player2', 'Номер телефона...')} bind:value={numberValue} on:focus={onInputFocus} on:blur={onInputBlur}>
        </div>
        <div class="newphone__addcontact_element">
            <div class="gray">{translateText('player2', 'Сообщение')}</div>
            <textarea placeholder={translateText('player2', 'Введите текст сообщения...')} class="newphone__message_textarea" bind:value={inputValue} on:focus={onInputFocus} on:blur={onInputBlur}></textarea>
        </div>
        <div class="newphone__addcontact_button" on:click={onSend}>
            {translateText('player2', 'Отправить сообщение')}
        </div>
    </div>
</div>