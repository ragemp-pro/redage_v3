<script>
    export let placeholder;
    export let type;
    export let setValue;
    export let value;
    export let cl;
    import { isInput } from '@/views/player/newauthentication/store.js';

    let TextInput;

    let focusInput = false;

    const onFuncFocus = () => {
        focusInput = true;
        isInput.set(true);
    }

    const onFuncBlur = () => {
        focusInput = false;
        isInput.set(false);
    }
</script>

    {#if type === "password"}
    <input
        type="password"
        bind:value={value}
        bind:this={TextInput}
        on:input={(event) => {if (setValue) setValue (event.target.value)}}
        placeholder={placeholder}
        class={cl}
        on:focus={ onFuncFocus }
        on:blur={ onFuncBlur } />
    {:else}
    <input
        type="text"
        bind:value={value}
        bind:this={TextInput}
        on:input={(event) => {if (setValue) setValue (event.target.value)}}
        placeholder={placeholder}
        class={cl}
        on:focus={ onFuncFocus }
        on:blur={ onFuncBlur } />
    {/if}