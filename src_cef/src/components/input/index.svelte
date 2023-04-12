<script>
    import './main.sass';
    export let placeholder;
    export let type;
    export let icon;
    export let setValue;
    export let value;
    export let isFocus = false;
    export let updateLang;
    export let settingsClass;
    export let settingsMargin;
    import { isInput } from '@/views/player/newauthentication/store.js';

    let TextInput;

    $: {
        if (isFocus) {
            TextInput.focus();
        }
    }

    const OnClick = () => {
        TextInput.focus();
    }

    let focusInput = false;

    const onFuncFocus = () => {
        focusInput = true;
        isInput.set(true);
    }

    const onFuncBlur = () => {
        focusInput = false;
        isInput.set(false);
    }

    const enLower = 'abcdefghijklmnopqrstuvwxyz'
    const rusLower = 'абвгдеёжзийклмнопрстуфхцчшщъыьэюя'

    $: if (updateLang && focusInput && value && value.length) {
        if (enLower.indexOf (value [value.length - 1].toLowerCase()) !== -1) 
            updateLang ("en")
        else if (rusLower.indexOf (value [value.length - 1].toLowerCase()) !== -1) 
            updateLang ("ru")
    }

</script>

<div class="main__input" on:click={OnClick} class:hover={focusInput} class:settings={settingsClass} class:m-0={settingsMargin}>
    {#if icon}
        <span class="main__input_icon {icon}" />
    {/if}
    {#if type === "password"}
    <input
        type="password"
        bind:value={value}
        bind:this={TextInput}
        on:input={(event) => {if (setValue) setValue (event.target.value)}}
        placeholder={placeholder}
        class="main__input_text" 
        on:focus={ onFuncFocus }
        on:blur={ onFuncBlur } />
    {:else}
    <input
        type="text"
        bind:value={value}
        bind:this={TextInput}
        on:input={(event) => {if (setValue) setValue (event.target.value)}}
        placeholder={placeholder}
        class="main__input_text" 
        on:focus={ onFuncFocus }
        on:blur={ onFuncBlur } />
    {/if}
</div>