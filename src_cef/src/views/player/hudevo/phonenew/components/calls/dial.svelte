<script>
    import { translateText } from 'lang'
    import { executeClientToGroup } from 'api/rage'
    import { validate } from 'api/validation';

    export let updateView;

    import AddContact from "./addContact.svelte";

    import { currentPage } from "../../stores";

    let numberValue = "";

    function onButton(value){
        numberValue = numberValue + value;
    }

    function onRemoveButton(){
        numberValue = numberValue.split('').slice(0, numberValue.length - 1).join('')
    }
    
    let isPopup = false;

    const onCall = (number) => {
        number = Number (number);

        let check;

        check = validate("phonenumber", number);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }
        executeClientToGroup ("call", number)
        currentPage.set ("callView");
    }
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>
<div class="newphone__dial" in:fade>
    {#if isPopup}
        <AddContact on:click={() => isPopup = false} {numberValue} {updateView} />
    {/if}
    <input type="text" class="newphone__dial_input" placeholder={translateText('player2', 'Введите номер')} bind:value={numberValue} on:focus={onInputFocus} on:blur={onInputBlur}>
    <div class="newphone__dial_button" on:click={() => isPopup = true}>{translateText('player2', 'Добавить контакт')}</div>
    <div class="newphone__dial_grid">
        <div class="newphone__dial_element" on:click={()=> onButton("1")}>1</div>
        <div class="newphone__dial_element" on:click={()=> onButton("2")}>2</div>
        <div class="newphone__dial_element" on:click={()=> onButton("3")}>3</div>
        <div class="newphone__dial_element" on:click={()=> onButton("4")}>4</div>
        <div class="newphone__dial_element" on:click={()=> onButton("5")}>5</div>
        <div class="newphone__dial_element" on:click={()=> onButton("6")}>6</div>
        <div class="newphone__dial_element" on:click={()=> onButton("7")}>7</div>
        <div class="newphone__dial_element" on:click={()=> onButton("8")}>8</div>
        <div class="newphone__dial_element" on:click={()=> onButton("9")}>9</div>
        <div class="newphone__dial_element">*</div>
        <div class="newphone__dial_element" on:click={()=> onButton("0")}>0</div>
        <div class="newphone__dial_element">#</div>
        <div class="newphone__dial_element hidden"></div>
        <div class="newphone__dial_element call" on:click={() => onCall (numberValue)}><div class="phoneicons-call"></div></div>
        <div class="newphone__dial_element hidden" on:click={onRemoveButton}>
            <div class="newphone__dial_remove">&#10006;</div>
        </div>
    </div>
</div>