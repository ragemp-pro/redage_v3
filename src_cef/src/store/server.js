import { writable } from 'svelte/store';
import { GetTime } from 'api/moment'
import { executeClient } from 'api/rage'

window.serverStore = {};

export const serverId = writable(0);
window.serverStore.serverId = (value) => serverId.set (value);

export const serverDonatMultiplier = writable(1);
window.serverStore.serverDonatMultiplier = (value) => serverDonatMultiplier.set (value);

export const serverDonateDoubleConvert = writable(1.5);
window.serverStore.serverDonateDoubleConvert = (value) => serverDonateDoubleConvert.set (value);

export const serverDateTime = writable(new Date().getTime());

let localDateTime = "2021-08-17T00:44:10.8644836+03:00";
window.serverStore.serverDateTime = (dateTime) => {
    serverDateTime.set (dateTime);
    localDateTime = dateTime;
    
    const moment = GetTime (dateTime);

    executeClient ("SetTime", moment.hours(), moment.minutes(), moment.unix());
}

window.serverStore.getDateTime = () => {
    return localDateTime;
}

export const isEvent = writable(false);
window.serverStore.isEvent = (value) => isEvent.set (value);