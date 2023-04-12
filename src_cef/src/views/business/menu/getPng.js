
import { ItemType, ItemId } from 'json/itemsInfo.js'

const Bool = (text) => {
    return String(text).toLowerCase() === "true";
}

export const getPngToItemId = (data) => {
    let title = data.Name;
    let png = data.Png;

    switch (data.Type)
    {
        case 0:
            const itemInfo = window.getItem (data.ItemId)
            png = getPng (data, itemInfo);

            if (title.length < 1) {
                if (data.ItemId === ItemId.CarCoupon) {
                    title = data.Data.toLowerCase();
                } else {
                    title = itemInfo.Name;
                }
            }
            break;
        case 1:
            switch (data.ItemId)
            {
                case 1:
                    png = document.cloud + "img/roulette/items_13.png";
                    if (title.length < 1)
                        title = "VIP Silver";
                    break;
                case 2:
                    png = document.cloud + "img/roulette/items_14.png";
                    if (title.length < 1)
                        title = "VIP Gold";
                    break;
                case 3:
                    png = document.cloud + "img/roulette/items_15.png";
                    if (title.length < 1)
                        title = "VIP Platinum";
                    break;
                case 4:
                    png = document.cloud + "img/roulette/items_16.png";
                    if (title.length < 1)
                        title = "VIP Diamond";
                    break;
            }
            break;
        case 2:
            png = document.cloud + "img/roulette/items_0.png";
            if (title.length < 1)
                title = "Игровая валюта";
            break;
        case 3:
            png = document.cloud + "img/roulette/items_5.png";
            if (title.length < 1)
                title = "RedBucks";
            break;
    }
    return {
        name: title,
        png: png
    }
}

export const isPng = (url) => {
    return /^((ftp|http|https):\/\/)?(www\.)?([A-Za-zА-Яа-я0-9]{1}[A-Za-zА-Яа-я0-9\-]*\.?)*\.{1}[A-Za-zА-Яа-я0-9-]{2,8}(\/([\w#!:.?+=&%@!\-\/])*)?/.test(url);
}

export const getPng = (localItem, iconInfo) => {
    try {
        if (localItem.ItemId !== ItemId.BodyArmor && iconInfo.functionType === ItemType.Clothes) {
            let pngDirectory = "inventoryItems/clothes";
            
            let dataParse;
            if (localItem.Data.split("_").length) 
                dataParse = localItem.Data.split("_");
    
            
            let drawableId = 0;
            let textureId = 0;

            if (dataParse[0] != undefined)
                drawableId = Number (dataParse[0]);

            if (dataParse[1] != undefined)
                textureId = Number (dataParse[1]);

            if (Bool (dataParse[2])) 
                pngDirectory += "/male"
            else 
                pngDirectory += "/female"
            
            switch (localItem.ItemId) {
                case ItemId.Mask:
                    pngDirectory += "/masks"
                    break;
                case ItemId.Glasses:
                    pngDirectory += "/glasses"
                    break;                 
                case ItemId.Ears:
                    pngDirectory += "/ears"
                    break;
                case ItemId.Jewelry:
                    pngDirectory += "/accessories"
                    break;                   
                case ItemId.Bracelets:
                    pngDirectory += "/bracelets"
                    break;
                case ItemId.Hat:
                    pngDirectory += "/hats"
                    break;
                case ItemId.Leg:
                    pngDirectory += "/legs"
                    break;
                case ItemId.Feet:
                    pngDirectory += "/shoes"
                    break;  
                case ItemId.Top:
                    pngDirectory += "/tops"
                    break;
                case ItemId.Undershit:
                    pngDirectory += "/undershit"
                    break;
                case ItemId.Watches:
                    pngDirectory += "/watches"
                    break;                    
                case ItemId.Bag:
                    pngDirectory += "/bags"
                    break;
                case ItemId.Gloves:
                    pngDirectory += "/gloves"
                    break;
            }
            pngDirectory += `/${drawableId}_${textureId}`
            return document.cloud + pngDirectory + '.png';
        } else if (localItem.ItemId === ItemId.CarCoupon) {
            return document.cloud + "inventoryItems/vehicle" + `/${localItem.Data.toLowerCase()}.png`;
        }
        return document.cloud + "inventoryItems/items" + `/${localItem.ItemId}.png`;
    }
    catch (e) 
    {
        console.log("asdas - " + e.toString())
    }
}