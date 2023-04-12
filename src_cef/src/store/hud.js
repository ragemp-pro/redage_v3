import { writable } from 'svelte/store';

window.hudStore = {};

export const isPlayer = writable(true);
window.hudStore.isPlayer = (value) => isPlayer.set (value);

export const isHelp = writable(true);
window.hudStore.isHelp = (value) => isHelp.set (value);

export const isWaterMark = writable(true);
window.hudStore.isWaterMark = (value) => isWaterMark.set (value);

export const isInputToggled = writable(false);
window.hudStore.isInputToggled = (value) => {
    isInputToggled.set (value);
    
    if (mp)
        mp.invoke('setTypingInChatState', value);
}

export const isPhone = writable(false);
window.hudStore.isPhone = (value) => isPhone.set (value);

export const inVehicle = writable(false);
window.hudStore.inVehicle = (value) => inVehicle.set (value);