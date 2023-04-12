import itemsJson from './items.json';
//itemsJson = JSON.parse(itemsJson);
export const getItems = (callback) => {
    callback(itemsJson);
}

export const getItem = (itemId) => {
    //itemsJson = JSON.parse(itemsJson);
    return itemsJson[itemId];
}