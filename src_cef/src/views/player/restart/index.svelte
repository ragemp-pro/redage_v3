<script>
    import './main.sass'
    import {format} from "api/formatter";
    import { executeClient } from "api/rage";
    export let viewData;


    let InputValue = ""

    const OnSubmitMessage = () => {
        if(InputValue) {
            let message = format("stringify", InputValue);
            executeClient ("restart.add", message);
            InputValue = "";
        }
    }

    const handleKeyUp = (e) => {
        switch(e.keyCode) {
            case 13:
                OnSubmitMessage();
                break;
        }
    }

    let Messages = []
    const AddChatMessage = (name, text) => {
        text = format("parse", text);

        if (Messages.length > 150) {
            Messages.shift();
        }

        Messages = [...Messages, {name: name, text: text}];

        setTimeout(() => OnScrollDown(), 0);
    };

    import { addListernEvent } from 'api/functions';
    addListernEvent ("restart.addMessage", AddChatMessage)

    let chatElement;
    const OnScrollDown = () => {
        if (chatElement) {
            chatElement.scrollTop = chatElement.scrollHeight;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp}/>

<div id="restartchat">
   <div class="restartchat__block">
       <div class="restartchat__header">
           {viewData}
       </div>
       <div class="restartchat__chat">
           <div class="restartchat__chat_messages" bind:this={chatElement}>
                {#each Messages as item}
                    <div class="restartchat__chat_message">
                        <b>{@html item.name}</b>: {@html item.text}
                    </div>
                {/each}
           </div>
           <div class="restartchat__chat_input">
               <input type="text" class="restartchat__input" bind:value={InputValue} spellCheck={false} maxLength={144} placeholder="Введите сообщение">
               <div class="restartchat__sendbutton" on:click={OnSubmitMessage}>&#9993;</div>
           </div>
       </div>
   </div>
</div>