import { writable } from 'svelte/store';


function loaderStore() {

    const disabledLoader = {
        toggle: false
    };

    let state = {};

    const { subscribe, set, update } = writable(state);

    window.loaderData = {
        set: (name, toggle, onTimeout = null) => {
            if (!state [name]) state [name] = disabledLoader;
    
            if(state[name].IntervalId) {
                clearInterval(state[name].IntervalId);
                state[name].IntervalId = null;
            }
    
            let IntervalId = null;
            if(toggle) {
                IntervalId = setTimeout(() => {
                    state[name] = {toggle: false, IntervalId: null};
                    set(state);
                    if(onTimeout !== null) onTimeout();
                }, 10000);
            }
    
            state[name] = {toggle: toggle, IntervalId: IntervalId};
            set(state);
        },
        get: (name) => {
            if (!state [name]) return disabledLoader;
            return state [name];
        },
        delay: (name, timeSecond = 1, isMessage = true) => {
            if (state[name] && state[name] > new Date().getTime()) {
                if (isMessage) 
                    window.notificationAdd(4, 9, `Попробуйте через ${Math.round ((state[name] - new Date().getTime()) / 1000)} секунд!`, 3000);
                return false;
            }                
            state[name] = new Date().getTime() + (1000 * timeSecond);
            set(state);
            return true;
        },
        delayGet: (name) => {
            if (!state[name])
                return true;
            if (state[name] > new Date().getTime()) {
                window.notificationAdd(4, 9, `Попробуйте через ${Math.round ((state[name] - new Date().getTime()) / 1000)} секунд!`, 3000);
                return false;
            }
            return true;
        },

    }

	return {
		subscribe,
        init: () => window.loaderData (),
	};
}

export default loaderStore();