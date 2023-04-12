import { writable } from 'svelte/store';

window.questStore = {};

export const storeQuests = writable({});

window.questStore.init = (json) => {
    storeQuests.set (JSON.parse (json));
}

export const selectQuest = writable(0);
window.questStore.selectQuest = (value) => selectQuest.set (value);