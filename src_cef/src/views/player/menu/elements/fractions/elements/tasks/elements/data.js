import { isPng, getPng } from "@/views/player/menu/elements/inventory/getPng";


const getPngToItemId = (item) => {
    const itemData = {
        ItemId: item.itemId,
        Data: item.data,
    }
    return getPng (itemData, window.getItem (item.itemId))
}

export const getPngUrl = (item) => {
    if (isPng(item.image))
        return item.image;
    else
        return getPngToItemId (item);
}

export const getTypeToTitle = (type, itemId) => {
    switch (type)
    {
        case 0:
            return window.getItem (itemId).Name;
        case 1:
            switch (itemId)
            {
                case 1:
                    return "VIP Silver";
                case 2:
                    return "VIP Gold";
                case 3:
                    return "VIP Platinum";
                case 4:
                    return "VIP Diamond";
            }
            break;
        case 2:
            return "Игровая валюта";
        case 3:
            return "RedBucks";
    }
    return "";
}

const formatIntZero = (num, length) => {
    return ("0" + num).slice(0 - length);
}

export const getTimeFromMins = (sec) => {
    const hours = Math.floor(sec / 60 / 60);
    const minutes = Math.trunc(sec / 60) % 60;
    const seconds = sec % 60;
    return formatIntZero(hours, 2) + ':' + formatIntZero(minutes, 2) + ':' + formatIntZero(seconds, 2);
}