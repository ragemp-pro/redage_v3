import { writable } from 'svelte/store';

export const settings = {
    char: 0,
    create: 1,
    buy: 2
}

export const selectChar = writable(false);
export const selectType = writable(false);
export const selectIndex = writable(-1);