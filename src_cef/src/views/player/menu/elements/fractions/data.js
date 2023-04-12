import { writable } from 'svelte/store';

const tableType = {
    fraction: 0,
    organization: 1
}

let selectedType = tableType.fraction;

export const setTableType = (typeName) => {
    selectedType = typeName === "Fractions" ? tableType.fraction : tableType.organization;
}

export const isFraction = () => selectedType === tableType.fraction;
export const isOrganization = () => selectedType === tableType.organization;


export const selectPopup = writable(null);

let popupData = null;

export const setPopup = (name = null, data = null) => {
    popupData = data;
    selectPopup.set(name);

    if (!name)
        onInputBlur();
    else
        onInputFocus();
}
export const getPopupData = () => popupData;
//


export const selectTableView = writable(null);

export const setView = (name = null) => {
    selectTableView.set(name);
}

import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";

export const getLog = (uuid = -1, isStock = false, text = "", pageId = -1) => {
    executeClientToGroup("getLog", uuid, isStock, text, pageId)
}

export const accessType = {
    Add:  0,
    Remove: 1,
    Skip: 2
}

let rankId = null;
let rankRemoteName = null;
const onUpdateRankCallback = (rank) => {
    executeClientToGroup(rankRemoteName, rankId, rank);
}

export const onUpdateRank = (title, icon, text, remoteName, id, callback = null) => {
    rankRemoteName = remoteName;
    rankId = id;
    executeClientAsyncToGroup("getRanks").then((result) => {
        if (result && typeof result === "string") {
            result = JSON.parse(result);

            let ranks = []

            result.forEach((item) => {
                ranks.push({
                    id: item.id,
                    name: `${item.id}. ${item.name}`
                })
            })

            setPopup("List", {
                headerTitle: title,
                headerIcon: icon,
                listTitle: text,
                list: ranks,
                button: 'Выбрать',
                callback: callback ? callback : onUpdateRankCallback
            })
        }
    });
}

export const onInputFocus = () => {
    executeClientToGroup ('inputFocus', true);
    window.onFocusInput(true);
}

export const onInputBlur = () => {
    executeClientToGroup ('inputFocus', false);
    window.onFocusInput(false);
}