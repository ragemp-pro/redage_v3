import { writable } from 'svelte/store';

function routerStore() {

    let state = {
        popup: "",
        popupData: null,
        popupFunc: null,
        view: "PlayerAuthentication",
        viewData: null,
        PlayerGameMenu: false,
        PlayerHud: false,
        opacity: 1
    };

    const { subscribe, set, update } = writable(state);
    
    window.router = {
        setView: (page, data = null) => {
            state = {
                popup: "",
                popupData: null,
                view: page,
                viewData: data,
                PlayerGameMenu: false,
                PlayerHud: false,
                opacity: 1
            }
            set(state);
        },
        setViewData: (data = null) => {
            state.viewData = data;
            set(state);
        },
        addViewData: (data = null) => {
            state.viewData = {
                ...state.viewData,
                ...data
            };
            set(state);
        },
        setPopUp: (page = "", data = null, func = null) => {
            state.popup = page;
            state.popupData = data;
            state.popupFunc = func;
            set(state);
        },
        setPopUpData: (data = null) => {
            state.popupData = data;
            set(state);
        },
        updateStatic: (page) => {
            if(page === undefined || page === null) page = "";
            state = {
                popup: "",
                popupData: null,
                view: "",
                viewData: null,
                PlayerGameMenu: false,
                PlayerHud: false,
                opacity: 1
            }
            
            if(page !== "") {
                state[page] = true;
            }

            set(state);
        },
        setHud: (page) => {
            if (!page || page === state.view) {
                state = {
                    popup: "",
                    popupData: null,
                    view: "",
                    viewData: null,
                    PlayerGameMenu: false,
                    PlayerHud: true,
                    opacity: 1
                }
                set(state);
            }
        },
        close: () => {
            state = {
                popup: "",
                popupData: null,
                view: "",
                viewData: null,
                PlayerGameMenu: false,
                PlayerHud: false,
                opacity: 1
            }
            set(state);
        },
        opacity: (opacity) => {
            state.opacity = opacity;
            set(state);
        }
    }

	return {
		subscribe
	};
}

export default routerStore();