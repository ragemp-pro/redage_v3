import {ItemId, ItemType} from "json/itemsInfo";

export const categorieName = [
    "Недвижимость",
    "Бизнес",
    "Транспорт",
    //"Прочее",
]

export const getPng = (type, image) => {
    try {

        //window.getItem (item.ItemId)

        if (type === 2) {
            return document.cloud + "inventoryItems/vehicle" + `/${image.toLowerCase()}.png`;
        } else if (type === 3) {

        }


        return false




        /*if (localItem.ItemId !== ItemId.BodyArmor && iconInfo.functionType === ItemType.Clothes) {
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
        return document.cloud + "inventoryItems/items" + `/${localItem.ItemId}.png`;*/
    }
    catch (e)
    {
        console.log("asdas - " + e.toString())
    }
}