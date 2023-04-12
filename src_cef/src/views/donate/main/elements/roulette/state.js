import { writable } from 'svelte/store';

export const CaseData = writable([0, 4, 5]);
export const selectCase = writable(0);