import { writable } from 'svelte/store';

const _keysDataStore = () => {

    let state = {}
    for(let i = 0; i < 100; i++) {
        state[i] = 85;
    }
    const { subscribe, set } = writable(state);
    
    window.keysStore = {
        updateName: (name, value) => {
            if (state[name] === value) return;
            state[name] = value;
            set(state);
        },
    }

	return {
		subscribe,
        updateName: (name, value) => window.keysStore.updateName (name, value),
	};
}

export default _keysDataStore();