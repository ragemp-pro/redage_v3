import { writable } from 'svelte/store';


function inputStore() {

    let state = {};

    const { subscribe, set, update } = writable(state);
    window.inputData = (data, text = null) => {
        if (!state [data] && !text) return "";
        else if (state [data] && !text) return state [data];
        else if (text) {
            state [data] = text;
            set(state);
            return text;
        }
    }

    window.inputDelete = (data) => {
        if (state [data]) {
            delete state [data];
            set(state);
        }
    }

	return {
		subscribe
	};
}

export default inputStore();