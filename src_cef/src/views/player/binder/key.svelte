<!--import React, { useState, useEffect } from 'react';
import { useStore } from "../../store/useStore";

const Key = ({ title, style = "", disabled = false, code = -1 }) => {
    //const { state: { bindList }} = useStore();
    const { state: { bindList } } = useStore();
    
    const [active, setActive] = useState(false); 
    const [use, setUse] = useState(false); 

    const handleKeyDown = (event) => {
        if (!active && !disabled) {
            const { keyCode } = event;
            if (keyCode !== code) return;
            setUse (true);
        }
    }

    const handleKeyUp = (event) => {
        if (use) {
            setUse (false);
        }
    }
    useEffect(() => {
        if (bindList) {
            let success = false;
            if (bindList[code]) {                
                setActive (true);
                success = true;
            }
            if (active && !success) {
                setActive (false);
            }
        }
        //document.addEventListener("keydown", handleKeyDown);
        //document.addEventListener("keyup", handleKeyUp);
        return () => {
            //document.removeEventListener("keydown", handleKeyDown);
            //document.removeEventListener("keyup", handleKeyUp);
        };
    });
    return (<React.Fragment>
        {(active && bindList[code]) ? (    
        <div tooltip={bindList[code]} class={`key ${style} DCS${disabled ? " disabled" : " "} ${active ? " active" : " "} ${use ? " down" : " "}`}>
            <div class="keycap">
                {title}
            </div>
        </div>
        ) : (
        <div class={`key ${style} DCS${disabled ? " disabled" : " "} ${active ? " active" : " "} ${use ? " down" : " "}`}>
            <div class="keycap">
                {title}
            </div>
        </div>
        )}
    
        </React.Fragment>
    );
}

export default Key;-->

<script>

    import { bindList } from './state.js'

    export let title;
    export let style = "";
    export let disabled = false;
    export let code = -1;

    let active = false; 
    let use = false; 

    $: {
        if ($bindList) {
            let success = false;
            if ($bindList[code]) {                
                active = true;
                success = true;
            }
            if (active && !success) {
                active = false;
            }
        }
    }
    const handleKeyDown = (event) => {
        if (!active && !disabled) {
            const { keyCode } = event;
            if (keyCode !== code) return;
            use = true; 
        }
    }

    const handleKeyUp = (event) => {
        if (use) {
            use = false; 
        }
    }
</script>
<svelte:window on:keydown={handleKeyDown} on:keyup={handleKeyUp} />

{#if (active && $bindList[code])}    
<div tooltip={$bindList[code]} class="key DCS {style}" class:disabled={disabled} class:active={active} class:down={use}>
    <div class="keycap">
        {title}
    </div>
</div>
{:else}
<div class="key DCS {style}" class:disabled={disabled} class:active={active} class:down={use}>
    <div class="keycap">
        {title}
    </div>
</div>
{/if}