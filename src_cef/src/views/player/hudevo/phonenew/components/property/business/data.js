export const getPrefix = (productName, bizType)=> {
    return (bizType == 7 || bizType == 11 || bizType == 12 || productName == "Татуировки" || productName == "Парики" || productName == "Патроны" || productName == "Модификации") ? "%" : "$";
}

export const getPng = (bizType, name, itemId) => {
    switch (name) {
        //case "Расходники":
        //    return document.cloud + "inventoryItems/vehicle/sultan3.png";
    }


    if (bizType < 0)
        return document.cloud + "inventoryItems/vehicle" + `/${String(name).toLowerCase()}.png`;

    if (itemId > 0)
        return document.cloud + "inventoryItems/items" + `/${itemId}.png`;


    return document.cloud + `img/iphone/bizicons/${String(name).toLowerCase()}.png`;
}