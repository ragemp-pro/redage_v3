<script>
    import { translateText } from 'lang'
    import KeyAnimation from '@/components/keyAnimation/index.svelte';
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    export let viewData;

    let nameValue = "";
    let textValue = "";

    let focusInput = false;
    const onFuncFocus = () => {
        focusInput = true;
    }
    const onFuncBlur = () => {
        focusInput = false;
    }

    const handleArrowKeys = (events) => {
        if (!viewData)
            return;
        const { keyCode } = events;
        if (viewData && keyCode === 27) {
            OnClose ();
        } else if (viewData && !viewData.Text && keyCode === 13) {
            onSend ();
        }
    }

    const onSend = () => {    
        if (focusInput) {
            window.notificationAdd(4, 9, translateText('events', 'Вы не можете отправить сообщение, пока находитесь на поле ввода'), 3000);
            return;    
        }
        let check = format("name", nameValue);
        
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }
        
        check = format("text", textValue);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        textValue = textValue.replace(/\n/g, '<br/>').trim();
        textValue = textValue.replace(/\s+/g, ' ').trim();

        executeClient ("client.note.create", 1, viewData.ItemId, nameValue.replace(/\s+/g, ' ').trim(), textValue);
    }

    let NameText = [];
    if (viewData && viewData.Name) {
        if (viewData.Name.indexOf('_') >= 0) {
            NameText = viewData.Name.split('_');
        } else if (viewData.Name.indexOf('.') >= 0) {
            NameText = viewData.Name.split('.');
        } else if (viewData.Name.indexOf('-') >= 0) {
            NameText = viewData.Name.split('-');
        } else if (viewData.Name.indexOf('|') >= 0) {
            NameText = viewData.Name.split('|');
        } else if (viewData.Name.indexOf(' ') >= 0) {
            NameText = viewData.Name.split(' ');
        } else {
            if (viewData.Name.length > 24) {
                NameText.push (viewData.Name.substr(0, Math.round (viewData.Name.length / 2)));
                NameText.push (viewData.Name.substr(Math.round (viewData.Name.length / 2), viewData.Name.length));
            } else {                
                NameText.push (viewData.Name);
                NameText.push ("");
            }
        }
    }

    const OnClose = () => {
        executeClient ("client.note.close")
    }
</script>

<svelte:window on:keyup={handleArrowKeys} />

{#if !viewData.Text}
    <div class="valentine__heart_img">
        <input class="valentine__input" placeholder="Кому" bind:value={nameValue} on:focus={ onFuncFocus } on:blur={ onFuncBlur } />
        <textarea class="valentine__input_big" placeholder="Написать текст" bind:value={textValue} on:focus={ onFuncFocus } on:blur={ onFuncBlur } />
    </div>
    <div class="box-flex">
        <div class="box-KeyAnimation margin-right-35" on:click={onSend}>
            <div>{translateText('events', 'Отправить')}</div>
            <KeyAnimation keyCode={13}>ENTER</KeyAnimation>
        </div>
        <div class="box-KeyAnimation" on:click={OnClose}>
            <div>{translateText('events', 'Выйти')}</div>
            <KeyAnimation keyCode={27}>ESC</KeyAnimation>
        </div>
    </div>
{:else}
    <div class="valentine__hearts_img">
        <div class="valentine__hearts_name">
            <span>{NameText[0]}</span>
            <span>{NameText[1]}</span>
            <div class="valentine__hearts_absolute">
                <span>{NameText[0]}</span>
                <span>{NameText[1]}</span>
            </div>
        </div>
        <div class="valentine__hearts_text">{@html viewData.Text}</div>
    </div>
    <div class="box-KeyAnimation" on:click={OnClose}>
        <div>{translateText('events', 'Выйти')}</div>
        <KeyAnimation keyCode={27}>ESC</KeyAnimation>
    </div>
{/if}