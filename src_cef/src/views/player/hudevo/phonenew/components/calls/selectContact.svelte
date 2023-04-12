<script>
    import { translateText } from 'lang'
    import { addListernEvent } from 'api/functions'

    export let updateListContacts;

    import { validate } from 'api/validation';
    import { selectedImage, selectedImageFunc, selectNumber } from './../../stores'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    import { onCall, onMessage } from "@/views/player/hudevo/phonenew/data";

    let contactData = {};

    const getContact = () => {
        executeClientAsyncToGroup("getContact", $selectNumber).then((result) => {
            if (result && typeof result === "string") {
                contactData = JSON.parse(result);
            }
        });
    }

    getContact ();
    let isEdit = false;


    const onClose = () => {
        selectNumber.set(null);
    }

    let nameValue;
    let avatarValue;
    const updateEditStatus = () => {
        nameValue = contactData.Name;
        avatarValue = contactData.Avatar;
        isEdit = !isEdit;
    }


    const sendImage = (link) => {
        if (link && typeof link === "string") {
            avatarValue = link;
        }
    }

    const getAvatar = (avatar) => {
        if (typeof avatar === "string" && avatar.length > 6)
            return `background-image: url(${avatar})`;

        return "";
    }

    addListernEvent ("cameraLink", sendImage)

    const onPopupImage = () => {
        selectedImage.set(true)
        selectedImageFunc.set(sendImage)
    }

    const onUpdateContact = () => {

        let check;

        check = validate("phonename", nameValue);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup ("updateContact", $selectNumber, nameValue, avatarValue);
        updateEditStatus ();

        //
        getContact ();
        updateListContacts ();

    }

    //

    const onAddBlackList = () => {
        executeClientAsyncToGroup("addBlackList", $selectNumber).then((result) => {
            if (result)
                contactData.IsBlackList = true;
        });
    }

    const onDellBlackList = () => {
        executeClientAsyncToGroup("dellBlackList", $selectNumber).then((result) => {
            if (result)
                contactData.IsBlackList = false;
        });
    }

    //

    const onDellContact = () => {
        executeClientAsyncToGroup("dellContact", $selectNumber).then((result) => {
            if (result) {
                onClose ();
                //
                updateListContacts ();
            }
        });
    }

    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>


{#if !isEdit}
    <div class="box-between w-1 blue newphone__project_padding16" in:fade>
        <div class="box-flex" on:click={onClose}>
            <div class="phoneicons-Vector-Stroke"></div>
            <div>{translateText('player2', 'Контакты')}</div>
        </div>
        {#if !contactData.IsSystem && contactData.IsAdded}
        <div class="box-flex" on:click={updateEditStatus}>
            <div>{translateText('player2', 'Изменить')}</div>
        </div>
        {/if}
    </div>
    <div class="newphone__contacts_bigicon" class:phoneicons-contacts={!getAvatar(contactData.Avatar).length} style={getAvatar(contactData.Avatar)}></div>
    <div class="newphone__contacts_name newphone__project_padding16">{contactData.Name}</div>
    <div class="box-between box-center w-1 newphone__project_padding16">
        <div class="newphone__contacts_box" on:click={() => onMessage (contactData.Number)}>
            <div class="phoneicons-chat newphone__contacts_but"></div>
            <div>{translateText('player2', 'Написать')}</div>
        </div>
        {#if !contactData.IsSystem}
        <div class="newphone__contacts_box" on:click={() => onCall (contactData.Number)}>
            <div class="phoneicons-call newphone__contacts_but"></div>
            <div>{translateText('player2', 'Позвонить')}</div>
        </div>
        {/if}
    </div>
    <div class="newphone__contacts_number">
        <div class="newphone__project_padding16">{translateText('player2', 'Номер телефона')}</div>
        <div class="newphone__project_padding16 blue">{contactData.Number}</div>
    </div>
    {#if !contactData.IsSystem}
        {#if !contactData.IsAdded}
            <div class="newphone__contacts_button blue" on:click>{translateText('player2', 'Добавить контакт')}</div>
        {/if}
        {#if contactData.IsBlackList}
            <div class="newphone__contacts_button blue" on:click={onDellBlackList}>
                {translateText('player2', 'Разблокировать контакт')}
            </div>
        {:else}
            <div class="newphone__contacts_button blue" on:click={onAddBlackList}>
                {translateText('player2', 'Заблокировать контакт')}
            </div>
        {/if}
        {#if contactData.IsAdded}
        <div class="newphone__contacts_button red" on:click={onDellContact}>{translateText('player2', 'Удалить контакт')}</div>
        {/if}
    {/if}
{:else}
    <div class="w-1 box-between newphone__project_padding16" in:fade>
        <div></div>
        <div class="phoneicons-add1" on:click={updateEditStatus}></div>
    </div>
    <div class="newphone__contacts_bigicon hover" class:phoneicons-contacts={!getAvatar(avatarValue).length} style={getAvatar(avatarValue)} on:click={onPopupImage}></div>
    <div class="newphone__addcontact_element edit">
        <div class="gray">{translateText('player2', 'Имя')}</div>
        <input type="text" class="newphone__addcontact_input" placeholder={translateText('player2', 'Введите..')} bind:value={nameValue} on:focus={onInputFocus} on:blur={onInputBlur}>
    </div>
    <div class="newphone__addcontact_element edit">
        <div class="gray">{translateText('player2', 'Номер')}</div>
        <input type="text" class="newphone__addcontact_input" placeholder={translateText('player2', 'Введите.. ')} value={$selectNumber} disabled>
    </div>
    <div class="newphone__addcontact_button" on:click={onUpdateContact}>
        {translateText('player2', 'Изменить контакт')}
    </div>
{/if}