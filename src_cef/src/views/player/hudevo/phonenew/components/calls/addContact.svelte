<script>
    import { translateText } from 'lang'
    import { validate } from 'api/validation';
    import { executeClientToGroup } from 'api/rage'
    import { selectNumber } from './../../stores'

    export let updateView;
    export let updateListContacts;

    export let numberValue;
    let nameValue;

    const onAddContact = () => {
        let check;

        check = validate("phonename", nameValue);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        const number = Number (numberValue);
        check = validate("phonenumber", number);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup ("addContact", number, nameValue);

        selectNumber.set (number);

        updateView ("contacts");

        if (typeof updateListContacts === "function")
            updateListContacts ();
    }
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>


<div class="newphone__dial_popup" in:fade>
    <div class="newphone__dial_popupback">
        <div class="box-between">
            <div></div>
            <div class="phoneicons-add1" on:click></div>
        </div>
        <div class="box-center">
            <div class="newphone__addcontact_image"></div>
        </div>
        <div class="newphone__addcontact_element">
            <div class="gray">{translateText('player2', 'Имя')}</div>
            <input type="text" class="newphone__addcontact_input" placeholder="Введите.." bind:value={nameValue} on:focus={onInputFocus} on:blur={onInputBlur}>
        </div>
        <div class="newphone__addcontact_element">
            <div class="gray">{translateText('player2', 'Номер')}</div>
            <input type="text" class="newphone__addcontact_input" placeholder="Введите.." bind:value={numberValue} on:focus={onInputFocus} on:blur={onInputBlur}>
        </div>
        <div class="newphone__addcontact_button" on:click={onAddContact}>
            <div class="phoneicons-add1"></div>
            {translateText('player2', 'Добавить контакт')}
        </div>
    </div>
</div>