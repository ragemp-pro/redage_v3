<script>
    import { translateText } from 'lang'
    export let visible;
    import { charGender, charMoney } from 'store/chars'
    import { executeClient } from 'api/rage'
    import { ItemType, ItemId, itemsInfo } from 'json/itemsInfo.js'
    import { clothesData, ItemToWeaponHash, WeaponHashToItem, stageItem, clearSlot, defaulSelectItem, defaulHoverItem, maxSlots, otherName, otherType, clothes, clothesId, clothesName, itemIdCaseToId } from './functions.js';
    import { format } from 'api/formatter'
    import wComponents from './wComponents.js';
    import wMaxHP from './wMaxHP.js';
    import rangeslider from 'components/rangeslider/index'
    import Slot from "./slot.svelte";
    import { onMount } from 'svelte';
    import { spring } from 'svelte/motion';
    import '../../fonts/newinv/style.css'
    import { getPng } from './getPng.js'

    import inventoryWeapons from 'json/inventoryweapons.js'


    let
        fastSlots = [1, 2, 3, 4, 5],
        clickTime = 0,
        invOpacity = 1,
        invOldOpacity = -1,
        ItemStack = -1,
        StackValue = 1,
        tradeMoney = "",
        useInventoryArea = false,
        mouseLeaveSelectedItem = false,
        mainInventoryArea = false,
        selectItem = defaulSelectItem,
        hoverItem = defaulHoverItem,
        infoItem = defaulHoverItem,
        isMoveBlock = false,
        moveBlock = {
            accessories: [null, null],
            inventory: [null, null],
            backpack: [null, null],
            other: [null, null],
            fastSlots: [null, null],
            trade: [null, null],
            with_trade: [null, null],
        },
        ItemsData = {
            accessories: Array(maxSlots.accessories).fill(clearSlot),
            inventory: Array(maxSlots.inventory).fill(clearSlot),
            backpack: Array(maxSlots.backpack).fill({ ...clearSlot, use: false }),
            other: Array(maxSlots.other).fill(clearSlot),
            fastSlots: Array(maxSlots.fastSlots).fill(clearSlot),
            trade: Array(maxSlots.trade).fill(clearSlot),
            with_trade: Array(maxSlots.with_trade).fill(clearSlot),
        },
        SlotToPrice = [],
        tradeInfo = {
            Active: false,
            
            YourStatus: false,		 	// Статус готовности обмена
            YourStatusChange: false, 	// Нажата кнопка "Обмен"
            YourMoney: 0,
    
            WithName: "Deluxe",			// Имя игрока, с которым вы обмениваетесь
            WithStatus: false,			// Статус готовности обмена игрока, с которым вы обмениваетесь
            WithStatusChange: false, 	// Нажата кнопка "Обмен"
            WithMoney: 0
        },
        PlayerInfo = {
            Sex: 0,
            Name: "",
            Backpack: false
        },
        OtherInfo = {
            Id: 0,
            Name: "",
        },
        OtherItemId = 0,
        OtherSqlId = 0,
        isArmyCar = false,
        isInVehicle = false;
    
    /* Functions */
    let coords = spring({ x: 0, y: 0 }, {
        stiffness: 1.0,
        damping: 1.0
    });
    
    const Close = () => {
        tradeInfo = {
            Active: false,
            
            YourStatus: false,		 	// Статус готовности обмена
            YourStatusChange: false, 	// Нажата кнопка "Обмен"
            YourMoney: "",
    
            WithName: "Deluxe",			// Имя игрока, с которым вы обмениваетесь
            WithStatus: false,			// Статус готовности обмена игрока, с которым вы обмениваетесь
            WithStatusChange: false, 	// Нажата кнопка "Обмен"
            WithMoney: ""
        }
        OtherInfo.Id = otherType.None;
        OtherInfo.Name = "";
        ItemsData.other = Array(maxSlots.other).fill(clearSlot);
        ItemsData.trade = Array(maxSlots.trade).fill(clearSlot);
        ItemsData.with_trade = Array(maxSlots.with_trade).fill(clearSlot);

        if (invOldOpacity != -1) {
            invOpacity = invOldOpacity;
            invOldOpacity = -1;
        }
        itemNoUse (1);
    }
    window.getItemToCount = (_ItemId) => {
        let count = 0;
        for (let arrayName in ItemsData) {
            if (arrayName !== "other" && arrayName !== "trade" && arrayName !== "with_trade" && arrayName !== "backpack") 
            {
                ItemsData[arrayName].forEach((i) => {
                    if (i.ItemId == _ItemId) {
                        count += Math.round (i.Count);
                    }
                })
            }
        }
        return count;
    }
    window.isItem = (_ItemsId) => {
        _ItemsId = JSON.parse (_ItemsId);
        let rItemId = [];
        for (let arrayName in ItemsData) {
            if (arrayName !== "other" && arrayName !== "trade" && arrayName !== "with_trade" && arrayName !== "backpack") 
            {
                ItemsData[arrayName].forEach((i) => {
                    if (_ItemsId.includes(i.ItemId)) {
                        rItemId.push(i.ItemId);
                    }
                })
            }
        }
        if (rItemId.length > 0) {
            executeClient ("client.inventory.GetItem", JSON.stringify(rItemId), true);
            return true;
        } else {
            executeClient ("client.inventory.GetItem", _ItemsId, false);
            return false;
        }
    }
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }

    //Выгрука

    const InitData = (json, use = true) => {
        let itemsArray = JSON.parse(json);
        for (let arrayName in itemsArray) {
            ItemsData[arrayName] = LoadData (maxSlots [arrayName], itemsArray [arrayName], use);
        }
    }
    
    let maxSlotBackpack = 0;
	const InitMyData = (maxSlot, json, use = true) => {
        maxSlotBackpack = maxSlot;
		ItemsData["backpack"] = LoadData (maxSlot, JSON.parse(json), use);
    }

    const UpdateSpecialVars = (isInVehicle_info = false) => {
        isInVehicle = isInVehicle_info;
    }
    
    const LoadData = (maxSlot, json, use = true) => {
        let returnArray = [];
        let indexItem;
        let itemsIndex = 0;
        let itemsArray = json;
        
        Array(maxSlot).fill(0).forEach((item, index) => {
            if (itemsArray && itemsArray.length && itemsArray[itemsIndex]) {
                item = itemsArray[itemsIndex];
                indexItem = item.Index;
            } else {
                indexItem = -1;
            }

            if (indexItem === index) {
                itemsIndex++;
                returnArray = [ ...returnArray, {
                    ...clearSlot,
                    ...item,
                    use: use,
                }];
            } else {
                returnArray = [ ...returnArray, {
                    ...clearSlot,
                    use: use,
                }];
            }
        });
        
        return returnArray;
    }

    const InitSlotToPrice = (json = "[]") => {
        SlotToPrice = JSON.parse(json);
    }
    
    const InitOtherData = (otherId, otherName, json, maxSlot = 20, selectItemId = 0, isArmyCar_info = false, _isMyTent = false, _SlotToPrice = "[]") => {
        if (otherId == otherType.None) {
            if (OtherInfo.Id == otherType.None) return;
            closeOther ();
            return;
        }
        OtherInfo.Id = otherId;
        OtherInfo.Name = otherName;
        OtherInfo.IsMyTent = _isMyTent;
        if (selectItemId != 0 && selectItemId.split("_").length) {
            OtherItemId = selectItemId.split("_")[0];
            OtherSqlId = selectItemId.split("_")[1];
        }
    
        let returnArray = [];
        let indexItem;
        let itemsIndex = 0;
        let itemsArray = JSON.parse(json);

        SlotToPrice = JSON.parse(_SlotToPrice);

        /*if (OtherInfo.Id === otherType.Storage) {
            itemsArray.forEach(item => {
                returnArray = [ ...returnArray, {
                    ...clearSlot,
                    ...item,
                }];
            });
        } else {*/
            Array(maxSlot).fill(0).forEach((item, index) => {            
                if (itemsArray && itemsArray.length && itemsArray[itemsIndex]) {
                    item = itemsArray[itemsIndex];
                    indexItem = item.Index;
                } else {
                    indexItem = -1;
                }

                if (indexItem === index) {
                    itemsIndex++;
                    returnArray = [ ...returnArray, {
                        ...clearSlot,
                        ...item,
                    }];
                } else {
                    returnArray = [ ...returnArray, {
                        ...clearSlot
                    }];
                }
            });
        //}

        ItemsData.other = returnArray;
        isArmyCar = isArmyCar_info;
    }

    const InitTradeData = (Name) => {
        tradeInfo.Active = true;

        tradeInfo.YourStatus = false;
        tradeInfo.YourStatusChange = false;
        ItemsData.trade = Array(maxSlots ["trade"]).fill(clearSlot);
        tradeInfo.YourMoney = "";

        tradeInfo.WithStatus = false;
        tradeInfo.WithStatusChange = false;
        tradeInfo.WithName = Name;
        tradeInfo.WithMoney = "";
        
        ItemsData.with_trade = Array(maxSlots ["with_trade"]).fill(clearSlot);
    }
    
    const UpdateSlot = (inventoryType, inventoryIndex, json, isInfo) => {
        const item = JSON.parse(json);
        if (isInfo && (inventoryType === "inventory" || inventoryType === "backpack")) {
            window.hudItem.drop (item.ItemId, item.Count, item.Data, true)
        }

        window.events.callEvent ("cef.events.UpdateSlot", json);
        
        const oldItem = ItemsData[inventoryType][inventoryIndex];

        ItemsData[inventoryType][inventoryIndex] = {
            ...oldItem,
            ...item
        }
        let hoverIndex = -1,
            hoverArrayName = -1;
            
        if (hoverItem !== defaulHoverItem) {
            hoverIndex = hoverItem.index;
            hoverArrayName = hoverItem.arrayName;
        }
        //hoverItem = defaulHoverItem;
        if (hoverIndex === -1 && hoverArrayName === -1) infoItem = defaulHoverItem;
        else {            
            const _Item = getItemToIndex (hoverIndex, hoverArrayName);
            if (_Item.ItemId != 0) {
                infoItem = {
                    ..._Item,
                    index: hoverIndex,
                    arrayName: hoverArrayName
                };
            } else infoItem = defaulHoverItem;
        }

        
        /*if (res.name === "weapons" && temsArray["fastSlots"][res.index].active) {
            dataUser.updateCharName ("weapon", {
                icon: window.getItem (temsArray["fastSlots"][res.index].ItemId).icon,
                ammo: ItemsData.basic["fastSlots"][res.index].item_amount
            });
        }*/
    }

    window.getItem = (item) => {
        if (itemsInfo [item]) {
            return itemsInfo [item];
        }
        return {
            Name: "",
            icon: "",
            Type: "",
            Text: "",
            functionType: 0,
        }
    }

    const FastSlots = (json) => {
        fastSlots = JSON.parse(json);
    }
    onMount(() => {

        // Инициализация инвентаря игрока
        window.events.addEvent("cef.inventory.InitData", InitData);

        window.events.addEvent("cef.inventory.InitMyData", InitMyData);

        window.events.addEvent("cef.inventory.UpdateSpecialVars", UpdateSpecialVars);
        
        // Инициализация инвентаря при взаимодействии с чем то
        window.events.addEvent("cef.inventory.InitOtherData", InitOtherData);
        
        // Инициализация инвентаря при трейде
        window.events.addEvent("cef.inventory.InitTradeData", InitTradeData);

        // Обновление слота в любом ивентаре
        window.events.addEvent("cef.inventory.UpdateSlot", UpdateSlot);

        // Инициализация инвентаря игрока
        window.events.addEvent("cef.inventory.TradeUpdate", TradeUpdate);
        
        // Инициализация инвентаря игрока
        window.events.addEvent("cef.inventory.tradeMoney", handleInputChange);
        
        // Обновление информации о бучтрых слотах
        window.events.addEvent("cef.inventory.fastSlots", FastSlots);

        // Закрытие инвентаря
        window.events.addEvent("cef.inventory.Close", Close);

        window.events.addEvent("cef.inventory.SlotToPrice", InitSlotToPrice);
    });

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        // Инициализация инвентаря игрока
        window.events.removeEvent("cef.inventory.InitData", InitData);
        
        window.events.removeEvent("cef.inventory.InitMyData", InitMyData);

        window.events.removeEvent("cef.inventory.UpdateSpecialVars", UpdateSpecialVars);
        
        // Инициализация инвентаря при взаимодействии с чем то
        window.events.removeEvent("cef.inventory.InitOtherData", InitOtherData);
        
        // Инициализация инвентаря при трейде
        window.events.removeEvent("cef.inventory.InitTradeData", InitTradeData);

        // Обновление слота в любом ивентаре
        window.events.removeEvent("cef.inventory.UpdateSlot", UpdateSlot);

        // Обновление информации о items
        window.events.removeEvent("cef.inventory.Init", Init);

        // Инициализация инвентаря игрока
        window.events.removeEvent("cef.inventory.TradeUpdate", TradeUpdate);
        
        // Инициализация инвентаря игрока
        window.events.removeEvent("cef.inventory.tradeMoney", handleInputChange);
        
        // Обновление информации о бучтрых слотах
        window.events.removeEvent("cef.inventory.fastSlots", FastSlots);

        // Закрытие инвентаря
        window.events.removeEvent("cef.inventory.Close", Close);

        window.events.removeEvent("cef.inventory.SlotToPrice", InitSlotToPrice);
    });
    
    const onKeyDown = (event) => {
        if (!visible) return;
        if (event.which === 17 && invOldOpacity === -1) {
            invOldOpacity = invOpacity;
            invOpacity = 0;
        }
    }

    const onKeyUp = (event) => {  
        if (!visible) return;
        if (event.which === 17 && invOldOpacity != -1) {
            invOpacity = invOldOpacity;
            invOldOpacity = -1;
        }
    }
    // Слот
	const handleSlotMouseEnter = (event, index, arrayName) => {
        if (selectItem.use === stageItem.move && hoverItem === defaulHoverItem) {            
            hoverItem = {
                index: index,
                arrayName: arrayName
            }
        }
        
        if (selectItem.use !== stageItem.useItem && selectItem.use !== stageItem.move && getItemToIndex (index, arrayName).ItemId) {
            const target = event.target.getBoundingClientRect();
            coords.set({ x: (target.x + target.width/2), y: target.y });
            infoItem = {
                ...getItemToIndex(index, arrayName),
                index: index,
                arrayName: arrayName
            };
        }
    }
	
	// Когда выходим из зоны ячейки
	const handleSlotMouseLeave = (e) => {
		if (hoverItem !== defaulHoverItem) hoverItem = defaulHoverItem;
        if (infoItem !== defaulHoverItem) infoItem = defaulHoverItem;
        
        if (mouseLeaveSelectedItem === false) mouseLeaveSelectedItem = true;
        
    }
    //

    const closeOther = () => {
        OtherInfo.Id = otherType.None;
        OtherItemId = 0;
        OtherSqlId = 0;
        OtherInfo.Name = "";
        ItemsData.other = Array(maxSlots.other).fill(clearSlot);
        executeClient ("client.inventory.OtherClose");
    }

    const handleSlotMouseUp = () => {
        if (selectItem.use === stageItem.info && clickTime >= new Date().getTime()) {
            const index = selectItem.index;
            const arrayName = selectItem.arrayName;
            const _sItem = getItemToIndex (index, arrayName);
            const _sInfoItem = window.getItem (_sItem.ItemId);

            if (selectItem.arrayName === "other" || selectItem.arrayName === "backpack") {
                let MaxStakcItems = 0;
                if ((MaxStakcItems = getMaxStakcItems (_sItem, _sInfoItem)) == -1) {
                    itemNoUse (2);
                    return;
                }
                if (MaxStakcItems > 0) executeClient ("client.gamemenu.inventory.stack", arrayName, index, 2, MaxStakcItems);
                else executeClient ("client.gamemenu.inventory.stack", arrayName, index, 2, _sItem.Count);
            } else if (_sInfoItem.functionType === ItemType.Cases && itemIdCaseToId [Number (_sItem.ItemId)] !== undefined) {
                window.router.setPopUp("PopupRoulette", itemIdCaseToId [Number (_sItem.ItemId)]);
            } else if (OtherSqlId && Number (OtherSqlId) === Number (_sItem.SqlId)) {
                closeOther ();
                executeClient ("client.gamemenu.inventory.use", arrayName, index);                
            } else executeClient ("client.gamemenu.inventory.use", arrayName, index);
            itemNoUse (3);
        }
        else if (selectItem.use === stageItem.get || selectItem.use === stageItem.info) {
            //if (getItemsUse (selectItem) !== false && (arrayName === "accessories" || arrayName === "inventory" || arrayName === "backpack" || arrayName === "other" || arrayName === "other")) {
                const index = selectItem.index;

                const arrayName = selectItem.arrayName;

                if (updateItem(index, arrayName, "hover")) {
                    selectItem = {
                        ...getItemToIndex(index, arrayName),
                        use: stageItem.info,
                        index: index,
                        arrayName: arrayName
                    }
                } else selectItem = defaulSelectItem;
            //} else itemNoUse (4);
        }
    }

    const setAccessories = () => {
        if (selectItem === defaulSelectItem) return;
        else if (hoverItem === defaulHoverItem && selectItem.use === stageItem.move && selectItem.arrayName !== "accessories") {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            const _sItem = getItemToIndex (selectIndex, selectArrayName);
            const _sInfoItem = window.getItem (_sItem.ItemId);
            if (selectArrayName !== "inventory") {
                itemNoUse (5);
                window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                return;
            }
            
            let hoverIndex = setClothes (_sItem.ItemId);

            if (hoverIndex == -2) return itemNoUse (6);

            hoverIndex = hoverIndex.slotId;
            const hoverArrayName = "accessories";

            const _hItem = getItemToIndex (hoverIndex, hoverArrayName);

            const _hInfoItem = window.getItem (_hItem.ItemId);

            if (isMove (hoverIndex, hoverArrayName, _sItem, _sInfoItem) == -2) {
                itemNoUse (7);
                return;
            }

            executeClient ("client.gamemenu.inventory.move", selectArrayName, selectIndex, hoverArrayName, hoverIndex);

            //{"Name":"Маска","Description":"","Icon":"item-pizza","Type":"Одежда","Model":3887136870,"Stack":1,"functionType":1}
            if (_hItem.ItemId === _sItem.ItemId && Number (_hInfoItem.Stack) > 1) {
                const amount = (_hItem.Count === undefined || _hItem.Count < 2 || !isNumber(_hItem.Count)) ? 1 : _hItem.Count;

                if (Number (_hInfoItem.Stack) >= (amount + _sItem.Count)) {
                    _sItem.Count += amount;
                    setItem (hoverIndex, hoverArrayName, _sItem);
                    setItem (selectIndex, selectArrayName, clearSlot);

                } else {
                    _hItem.Count = (amount + _sItem.Count) - _hInfoItem.Stack;
                    _sItem.Count = _hInfoItem.Stack;
                    setItem (hoverIndex, hoverArrayName, _sItem);
                    setItem (selectIndex, selectArrayName, _hItem);
                }
            } else {
                setItem (hoverIndex, hoverArrayName, _sItem);
                setItem (selectIndex, selectArrayName, _hItem);
            }
            itemNoUse (8);
        }
    }
    
    const UpdateClothes = (event, componentId, drawableId, textureId) => {
        
        executeClient (event, componentId, drawableId, textureId);
    }

    const handleMouseDown = (event, index, arrayName) => {
        if (event.which == 1) {
            executeClient ("sounds.playInterface", "inventory/keys", 0.005);
            
            const item = getItemToIndex(index, arrayName);

            if (((selectItem.use === stageItem.info && (selectItem.index !== index || selectItem.arrayName !== arrayName)) ||
                selectItem.use !== stageItem.info) && item.ItemId != 0 && item.use) {

                if (arrayName === "other" && OtherInfo.Id === otherType.Nearby) {
                    if (OtherInfo.Id === otherType.Nearby && item.remoteId) 
                        return executeClient ("client.gamemenu.inventory.nearby", item.remoteId);
                } else if (arrayName === "other" && OtherInfo.Id === otherType.Tent) {

                    let _infoItem = window.getItem (item.ItemId);

                    const _selectItem = {
                        ...item,
                        use: stageItem.useItem,
                        index: index,
                        arrayName: arrayName,
                        tent: true,
                        info: _infoItem
                    }

                    unHoverAll ();
                    updateItem(index, arrayName, "hover", true);
                    infoItem = defaulHoverItem;
                    
                    coords.set({ x: event.clientX, y: event.clientY });

                    clickTime = new Date().getTime() + 200;
                    
                    StackValue = 1;

                    selectItem = _selectItem;
                    if (item.Count > 1) {
                        rangeslidercreate (item.Count);
                    }

                    return;
                }
                if (OtherSqlId && Number (OtherSqlId) === Number (getItemToIndex(index, arrayName).SqlId)) {
                    closeOther ();
                }

                itemNoUse (9);

                const target = event.target.getBoundingClientRect();

                const offsetInElementX = (target.width - (target.right - event.clientX));
                const offsetInElementY = (target.height - (target.bottom - event.clientY));

                coords.set({ x: event.clientX, y: event.clientY });

                clickTime = new Date().getTime() + 1000;
                selectItem = {
                    ...getItemToIndex(index, arrayName),
                    use: stageItem.get,
                    width: target.width,
                    height: target.height,
                    offsetInElementX: offsetInElementX,
                    offsetInElementY: offsetInElementY,
                    clientX: event.clientX,
                    clientY: event.clientY,
                    index: index,
                    arrayName: arrayName
                }

                mouseLeaveSelectedItem = false;

            }
        } else if (event.which == 3 && (arrayName !== "other" || (arrayName === "other" && OtherInfo.Id !== otherType.Nearby && OtherInfo.Id !== otherType.Tent)) && ItemsData[arrayName][index].ItemId != 0 && getItemToIndex(index, arrayName).use) {
            const item = getItemToIndex(index, arrayName);

            const _selectItem = {
                ...item,
                use: stageItem.useItem,
                index: index,
                arrayName: arrayName
            }
            if (getItemsUse (selectItem) === false && OtherInfo.Id <= otherType.None && item.Count <= 0 && getDropItem (arrayName, item.ItemId) === false) return;

            unHoverAll ();
            updateItem(index, arrayName, "hover", true);
            infoItem = defaulHoverItem;
            
            coords.set({ x: event.clientX, y: event.clientY });

            selectItem = _selectItem;
        }
    }

    const handleInputChange = (name, value) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 0) value = 0;
        else if (value > 9999999) value = 9999999;
        tradeInfo[name] = value;
        
        if (name === "YourMoney") executeClient ("client.gamemenu.inventory.tradeMoney", value);
    }

    const onBlur = () => {
        if (tradeInfo.YourMoney < 0) {
            window.notificationAdd(4, 9, translateText('player1', 'Сумма не может быть меньше 0.'), 3000);
            tradeInfo.YourMoney = 0;
            //return;
        } else if (tradeInfo.YourMoney > 9999999) {
            window.notificationAdd(4, 9, translateText('player1', 'Сумма не может быть больше $9.999.999.'), 3000);
            tradeInfo.YourMoney = 9999999;
            //return;
        } else if (Number($charMoney) < tradeInfo.YourMoney) {
			tradeInfo.YourMoney = Number($charMoney);
            window.notificationAdd(4, 9, `${translateText('player1', 'Сумма не может быть больше')} ${format("money", tradeInfo.YourMoney)}.`, 3000);
            //return;
        }
        executeClient ("client.gamemenu.inventory.tradeMoney", Math.round(tradeInfo.YourMoney));
    }
    
    //Глобальные
    const handleGlobalMouseMove = (event) => {
        if (!visible) return;
        else if (isMoveBlock) {
            moveBlock[isMoveBlock] = [
                event.clientY - selectItem.offsetInElementY,
                event.clientX - selectItem.offsetInElementX
            ]
            
            if (moveBlock[isMoveBlock][0] + selectItem.height > window.innerHeight) moveBlock[isMoveBlock][0] = window.innerHeight - selectItem.height;
            else if (moveBlock[isMoveBlock][0] < 0) moveBlock[isMoveBlock][0] = 0;

            if (moveBlock[isMoveBlock][1] + selectItem.width > window.innerWidth) moveBlock[isMoveBlock][1] = window.innerWidth - selectItem.width;
            else if (moveBlock[isMoveBlock][1] < 0) moveBlock[isMoveBlock][1] = 0;
        }
        else if (infoItem !== defaulHoverItem) {
            boxInfoLeft = fixOutToCenter ($coords.x, boxItemInfo);
            boxInfoTop = fixOutToTop ($coords.y, boxItemInfo);
        }
        else if ((selectItem.use === stageItem.move && infoItem === defaulHoverItem) || (selectItem.use !== stageItem.useItem && selectItem.use !== stageItem.get && infoItem === defaulHoverItem)) {
            let clientX = event.clientX;
            let clientY = event.clientY;
            
            if (clientY + selectItem.height > window.innerHeight) clientY = window.innerHeight - selectItem.height;
            else if (clientY < 0) clientY = 0;

            if (clientX + selectItem.width > window.innerWidth) clientX = window.innerWidth - selectItem.width;
            else if (clientX < 0) clientX = 0;
            
            coords.set({ x: clientX, y: clientY });

        } else if (selectItem.use === stageItem.get && (selectItem.clientX !== event.clientX || selectItem.clientY !== event.clientY)) {
            unHoverAll ();
            selectItem = {
                ...selectItem,
                use: stageItem.move,
            }
            let clientX = event.clientX;
            let clientY = event.clientY;
            
            if (clientY + selectItem.height > window.innerHeight) clientY = window.innerHeight - selectItem.height;
            else if (clientY < 0) clientY = 0;

            if (clientX + selectItem.width > window.innerWidth) clientX = window.innerWidth - selectItem.width;
            else if (clientX < 0) clientX = 0;

            coords.set({ x: clientX, y: clientY });            
        }
    }

    const onUseItem = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;

            const Item = getItemToIndex(selectIndex, selectArrayName);
            const InfoItem = window.getItem (Item.ItemId);

            if (InfoItem.functionType === ItemType.Cases && itemIdCaseToId [Number (Item.ItemId)] !== undefined) {
                window.router.setPopUp("PopupRoulette", itemIdCaseToId [Number (Item.ItemId)] );
            } else if (OtherSqlId && Number (OtherSqlId) === Number (Item.SqlId)) {
                closeOther ();
            } else {
                executeClient ("client.gamemenu.inventory.use", selectArrayName, selectIndex);
            }
            itemNoUse (10);
        }
    }

    const onDropItem = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;

            if (selectItem.Count > 1) {
                ItemStack = 1;
                rangeslidercreate (selectItem.Count);
            } else {
                itemNoUse (11);
                executeClient ("client.gamemenu.inventory.drop", selectArrayName, selectIndex);
            }
        }
    }

    const onTransfer = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;

            if (selectItem.Count > 1) {
                ItemStack = 2;
                rangeslidercreate (selectItem.Count);
            } else {            
                const _sItem = getItemToIndex (selectIndex, selectArrayName);
                const _sInfoItem = window.getItem (_sItem.ItemId);
                if (selectItem.arrayName !== "other" && isMove (selectIndex, "other", _sItem, _sInfoItem) == -2) {
                    itemNoUse (12);
                    return;
                } else if ((selectItem.arrayName === "other" || selectItem.arrayName === "backpack") && getMaxStakcItems (_sItem, _sInfoItem) != 0) {
                    itemNoUse (13);
                    return;
                }
                executeClient ("client.gamemenu.inventory.stack", selectArrayName, selectIndex, 2, 1);
                itemNoUse (14);
            }
        }
    }

    const handleInputStackChange = (value) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 0) value = 0;
        else if (ItemStack === 0 && value > selectItem.Count - 1) value = selectItem.Count - 1;
        else if (ItemStack !== 0 && value > selectItem.Count) value = selectItem.Count;
        StackValue = value;
    }
    
    const onBlurStack = () => {
        if (StackValue < 1) StackValue = 1;
        else if (StackValue > selectItem.Count - 1) StackValue = selectItem.Count - 1;
    }

    const onStack = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            if (ItemStack == 2) {
                const _sItem = getItemToIndex (selectIndex, selectArrayName);
                const _sInfoItem = window.getItem (_sItem.ItemId);
                if (selectItem.arrayName !== "other" && isMove (selectIndex, "other", _sItem, _sInfoItem) == -2) {
                    itemNoUse (15);
                    return;
                }
                let MaxStakcItems = 0;
                if ((selectItem.arrayName === "other" || selectItem.arrayName === "backpack") && (MaxStakcItems = getMaxStakcItems (_sItem, _sInfoItem)) == -1) {
                    itemNoUse (16);
                    return;
                }
                if (MaxStakcItems > 0) StackValue = MaxStakcItems;
            }
            executeClient ("client.gamemenu.inventory.stack", selectArrayName, selectIndex, ItemStack, StackValue);
            itemNoUse (17);
        }
    }

    const onBuy = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;
            executeClient ("client.gamemenu.inventory.buy", selectArrayName, selectIndex, StackValue);
            itemNoUse (17);
        }
    }
    
    const setClothes = (ItemId) => {
        if (ItemId === 12 || ItemId === 15) {
            return clothes["Bags"];
        } else if (ItemId === -15) {
            return clothes["Watches"];
        }
        let returnSlotId = -2;
        clothesId.forEach((item) => {
            if (clothes[item].itemId === ItemId) returnSlotId = clothes[item];
        });
        return returnSlotId;
    }
 
    const isMove = (index, arrayName, item, itemInfo) => {
        const OtherInfoId = OtherInfo.Id;
        let dataParse;
        if (item.ItemId != 0 && item.Data.split("_").length) dataParse = item.Data.split("_");

        if (arrayName === "accessories" && index === 8 && (getItemToIndex (8, "accessories").ItemId === 12 || getItemToIndex (8, "accessories").ItemId === 15))
        {
            window.notificationAdd(4, 9, translateText('player1', 'Это действие недоступно'), 3000);
            return -2;
        }
        else if (arrayName === "accessories" && item.ItemId != 0 && item.ItemId != -9 && item.ItemId != -5 && item.ItemId != -1 && itemInfo.functionType === ItemType.Clothes && dataParse && dataParse.length >= 2 && Bool(dataParse[2]) !== Bool($charGender)) {
            window.notificationAdd(4, 9, `Это ${Bool(dataParse[2]) ? translateText('player1', 'мужская') : translateText('player1', 'женская')} ${translateText('player1', 'одежда')}`, 3000);
            return -2;
        } else if (arrayName === "accessories" && item.ItemId != 0 && (clothes[clothesId[index]].itemId !== item.ItemId || (index === 8 && item.ItemId !== 12 && item.ItemId !== 15) || (index === 11 && item.ItemId !== -15))) {
            const _id = setClothes (item.ItemId);
            if (_id == -2) {
                window.notificationAdd(4, 9, translateText('player1', 'Данный слот не доступен!'), 3000);
                return -2;
            }
            //else if (_id.itemId == item.ItemId) return -2;
            return _id.slotId;
        } else if (arrayName === "accessories" && item.ItemId != 0 && index === 1 && !$charGender && (item.Data === "127_0_True" || item.Data === "127_2_True")) {
            window.notificationAdd(4, 9, translateText('player1', 'Вы не можете надеть этот уникальный аксессуар'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Safe && item.ItemId != 0 && itemInfo.functionType !== ItemType.Weapons &&  itemInfo.functionType !== ItemType.MeleeWeapons) {//Оружейный сейф
            window.notificationAdd(4, 9, translateText('player1', 'В данный сейф можно положить только оружие'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Chiffonier && item.ItemId != 0 && itemInfo.functionType !== ItemType.Clothes) {//Cейф под одежду
            window.notificationAdd(4, 9, translateText('player1', 'В данный шкаф можно положить только одежду'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Wardrobe && item.ItemId != 0 && (item.ItemId == 109 || item.ItemId == 12 || item.ItemId == 15 || item.ItemId == 19 || item.ItemId == 40 || item.ItemId == 41 || itemInfo.functionType === ItemType.Clothes || itemInfo.functionType === ItemType.Weapons)) {
            window.notificationAdd(4, 9, translateText('player1', 'Эта вещь не предназначена для этого шкафа'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Fraction && item.ItemId != 0 && itemInfo.functionType !== ItemType.Weapons && itemInfo.functionType !== ItemType.Ammo && item.ItemId != -9) {
            window.notificationAdd(4, 9, translateText('player1', 'На склад можно положить только оружие или патроны'), 3000);
            return -2;
        } else if ((arrayName === "inventory" || arrayName === "backpack") && OtherInfoId === otherType.Fraction && item.ItemId != 0) {
            let checkItem = getItemToIndex (index, arrayName);
            if (checkItem.ItemId != 0) {
                window.notificationAdd(4, 9, translateText('player1', 'Переложить можно только в пустой слот'), 3000);
                return -2;
            }

            /*let success = true;
            let count = item.Count;
            ItemsData[arrayName].forEach((i) => {
                if (success && item.ItemId == i.ItemId && item.SqlId != i.SqlId) {
                    count += i.Count;
                    if (count > itemInfo.Stack) {
                        success = false;
                        window.notificationAdd(4, 9, translateText('player1', 'Недостаточно места для такого количества'), 3000);
                    }
                }
            })*/
            if (getMaxStakcItems (item, itemInfo) === -1) return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Organization && item.ItemId != 0 && itemInfo.functionType !== ItemType.Weapons &&  itemInfo.functionType !== ItemType.Ammo && item.ItemId != -9) {
            window.notificationAdd(4, 9, translateText('player1', 'На склад можно положить только оружие или патроны'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Key && item.ItemId != 0 && item.ItemId != 19) {
            window.notificationAdd(4, 9, translateText('player1', 'Только ключи от т/c'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Nearby) {
            window.notificationAdd(4, 9, translateText('player1', 'Данное действие недоступно!'), 3000);
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.Tent && !OtherInfo.IsMyTent) {
            window.notificationAdd(4, 9, translateText('player1', 'Данное действие недоступно!'), 3000);
            return -2; 
        } else if (arrayName === "other" && OtherInfoId === otherType.Tent && OtherInfo.IsMyTent && item.ItemId != 0 && (item.ItemId == 19)) {
            window.notificationAdd(4, 9, translateText('player1', 'Нельзя продавать этот предмет.'), 3000);
            return -2;
        } else if (OtherInfoId === otherType.Vehicle && isArmyCar === true && item.ItemId != 0 && (item.ItemId == 237 || item.ItemId == 238 || item.ItemId == 239 || item.ItemId == 240 || item.ItemId == 241 || item.ItemId == 242)) {
            window.notificationAdd(4, 9, translateText('player1', 'Нельзя перекладывать этот предмет.'), 3000);
            return -2;
        } else if ((arrayName === "inventory" || arrayName === "accessories") && item.ItemId == -9 && isInVehicle === true) {
            window.notificationAdd(4, 9, translateText('player1', 'Невозможно снять бронежилет находясь в транспорте.'), 3000);
            executeClient ("checkClientSpecialVars");
            return -2;
        } else if (arrayName === "other" && OtherInfoId === otherType.wComponents && item.ItemId != 0) {            
            const weaponHash = ItemToWeaponHash [OtherItemId];
            let success = 0;
            if (!(item.ItemId >= 206 && 217 >= item.ItemId)) {
                window.notificationAdd(4, 9, translateText('player1', 'Этот слот предназначен только для модификаций'), 3000);
                return -2;
            }
            else if (item.ItemId >= 206 && 217 >= item.ItemId) {
                if (Number (dataParse[0]) == Number (weaponHash)) {
                    success = 1;
                }
                if (wComponents [weaponHash] && wComponents [weaponHash].Components) {
                    let componentHash;
                    let typeId = -1;
                    //Сначала проверяем есть ли он в списке компонентов
                    for (componentHash in wComponents [weaponHash].Components) {
                        if (Number (dataParse[1]) == Number (componentHash)) {
                            success = 1;
                            typeId = wComponents [weaponHash].Components [dataParse[1]].Type;
                        }
                    }
                    //Затем проверяемм есть ли уже такой тип 
                    if (success == 1) {
                        ItemsData["other"].forEach((i) => {
                            if (success != -1 && i.ItemId != 0 && i.Data.split("_").length) {
                                let dParse = i.Data.split("_");                    
                                if (wComponents [weaponHash].Components [dParse[1]] && wComponents [weaponHash].Components [dParse[1]].Type == typeId) {
                                    success = -1;
                                }
                            }
                        })
                    }
                }
            }
            if (success == 1) return -1;
            else if (success == -1) window.notificationAdd(4, 9, translateText('player1', 'Модификация такого типа уже установлена на данномм оружие!'), 3000);
            else window.notificationAdd(4, 9, translateText('player1', 'Данная модификация не подходит к этому оружию!'), 3000);
            return -2;
        } else if (arrayName === "fastSlots" && index === 4 &&
                item.ItemId != 0 &&
                item.ItemId != -1) {
            window.notificationAdd(4, 9, translateText('player1', 'Этот слот предназначен только для масок'), 3000);
            return -2;
        } else if (arrayName === "fastSlots" && index !== 4 &&
                    item.ItemId != 0 &&
                    itemInfo.functionType !== ItemType.Weapons &&
                    itemInfo.functionType !== ItemType.MeleeWeapons &&
                    item.ItemId != 6 &&
                    item.ItemId != 7 &&
                    item.ItemId != 8 &&
                    item.ItemId != 3 &&
                    item.ItemId != 5 &&
                    item.ItemId != 9 &&
                    item.ItemId != 10 &&
                    item.ItemId != 1 &&
                    item.ItemId != 280 &&
                    item.ItemId != 225 &&
                    item.ItemId != 226 &&
                    item.ItemId != 227 &&
                    item.ItemId != 228 &&
                    item.ItemId != 229 &&
                    item.ItemId != 230 &&
                    item.ItemId != 231 &&
                    item.ItemId != 232 &&
                    item.ItemId != 233 &&
                    item.ItemId != 388 &&
                    item.ItemId != 389 &&
                    item.ItemId != ItemId.VehicleNumber) {
            window.notificationAdd(4, 9, `${translateText('player1', 'Вы не можете положить сюда')} ${itemInfo.Name}`, 3000);
            return -2;
        }/* else if (arrayName === "other" && !getMaxStakcItems (item, itemInfo)) {
            window.notificationAdd(4, 9, `Вы не можете больше вместить ${itemInfo.Name}`, 3000);
            return -2;
        }*/
		else if ((arrayName === "backpack" && (item.ItemId == -5 || item.ItemId == 12)) || (arrayName === "other" && ((item.ItemId === -5 && Number(item.Data.split("_")[0]) == 40) || item.ItemId == 12))) 
		{
			window.notificationAdd(4, 9, `${translateText('player1', 'Вы не можете положить сюда')} ${itemInfo.Name}`, 3000);
            return -2;
        }
		else if ((arrayName !== "backpack" && arrayName !== "other") && (item.ItemId == 13 || item.ItemId == 1)) 
		{
            let success = true;
            let count = 0;
            ItemsData[arrayName].forEach((i) => {
                if (success && item.ItemId == i.ItemId && item.SqlId != i.SqlId) {
                    count += i.Count;
                    if (count >= itemInfo.Stack) {
                        success = false;
                        window.notificationAdd(4, 9, `${translateText('player1', 'Вы не можете положить')} ${itemInfo.Name}`, 3000);
                    }
                }
            })
            if (success) return -1;
            else return -2;
        } else if (arrayName === "other" && (OtherInfoId === otherType.Storage || OtherInfoId === otherType.Case) && item.ItemId != 0) {
            window.notificationAdd(4, 9, translateText('player1', 'Данный склад предназначен лишь для изъятия предметов из него.'), 3000);
            return -2;
        }
        return -1;
    }

    const maxItemCount = 3;
    const getMaxStakcItems = (item, itemInfo) => {
        if (item.ItemId == 0) return true;
        //if ([237, 238, 239, 240, 241, 242, 245, 246, 247].includes (item.ItemId)) 
        //    return item.Count;
        let countItems = 0;
        let maxStack = itemInfo.Stack;
        if (itemInfo.Stack <= 1) {
            if (itemInfo.functionType === ItemType.Weapons && item.ItemId != 109 && item.ItemId != 150) {
                const WeaponsAmmoTypes = {"100":200,"101":200,"102":200,"103":200,"104":200,"105":200,"106":200,"107":200,"108":200,"110":200,"111":200,"112":200,"113":200,"114":200,"151":200,"152":200,"115":201,"116":201,"117":201,"118":201,"119":201,"120":201,"121":201,"122":201,"123":201,"124":201,"125":201,"153":201,"126":202,"127":202,"128":202,"129":202,"130":202,"131":202,"132":202,"133":202,"134":202,"135":202,"136":203,"137":203,"138":203,"139":203,"140":203,"154":200,"155":200,"156":200,"157":200,"158":200,"159":200,"160":200,"161":200,"162":200,"141":204,"142":204,"143":204,"144":204,"145":204,"146":204,"147":204,"148":204,"149":204};
                const ammoType = WeaponsAmmoTypes[item.ItemId];
                let success = 0;
                for (let arrayName in ItemsData) {
                    if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
					{
						ItemsData[arrayName].forEach((i) => {
							if (!success && ((ammoType && ammoType == WeaponsAmmoTypes[i.ItemId]) || item.ItemId === i.ItemId)) {
                                if (++countItems >= maxItemCount) {
                                    success = -1;
                                    window.notificationAdd(4, 9, `Невозможно взять ${itemInfo.Name}, потому что в инвентаре уже есть оружие такого типа.`, 3000);
                                }
							}                   
						})
					}
                }
                return success;
            } else if (itemInfo.functionType === ItemType.MeleeWeapons || item.ItemId == -5 || item.ItemId == 41 || item.ItemId == 109 || item.ItemId == 150) {
                let success = 0;
                for (let arrayName in ItemsData) {
                    if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
					{
						ItemsData[arrayName].forEach((i) => {
							if (!success && item.ItemId == i.ItemId) {
								success = -1;
								window.notificationAdd(4, 9, `${translateText('player1', 'У Вас уже есть')} ${itemInfo.Name}`, 3000);
							}
						})
					}
                }
                return success;
            } else if (item.ItemId == 12 || item.ItemId == 15) {
				let success = 0;
                for (let arrayName in ItemsData) {
                    if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
					{
						ItemsData[arrayName].forEach((i) => {
							if (!success && (i.ItemId == 12 || i.ItemId == 15)) {
								success = -1;
								window.notificationAdd(4, 9, `${translateText('player1', 'У Вас уже есть')} ${itemInfo.Name}`, 3000);
							}
						})
					}
                }
				return success;
            } else if (item.ItemId == -9) {
				let success = 0;
                for (let arrayName in ItemsData) {
                    if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
					{
						ItemsData[arrayName].forEach((i) => {
							if (!success && i.ItemId == -9) {                                
                                if (++countItems >= maxItemCount) {
                                    success = -1;
                                    window.notificationAdd(4, 9, `У Вас уже есть ${itemInfo.Name}`, 3000);
                                }
							}
						})
					}
                }
				return success;
            }
        } else {
            let count = 0;
            for (let arrayName in ItemsData) {
                if (arrayName !== "other" && arrayName !== "backpack" && arrayName !== "trade" && arrayName !== "with_trade") 
                {
                    ItemsData[arrayName].forEach((i) => {
                        if (i.ItemId == item.ItemId) {
                            count += Math.round (i.Count);
                        }
                    })
                }
            }

            if (itemInfo.functionType == ItemType.Ammo)
                maxStack *= maxItemCount;

            if (Math.round (maxStack) === Math.round (count)) {
                window.notificationAdd(4, 9, `Нет места для ${itemInfo.Name}, максимум можно иметь при себе - ${maxStack} шт. | У вас ${count} шт.`, 3000);
                return -1;
            }
            else if (Math.round (maxStack) >= Math.round (count + item.Count)) return 0;
            else {
                return Math.round (maxStack) - count;
            }
        }
        return 0;
    }

    const handleGlobalMouseUp = (event) => {
        if (!visible) return;
        else if (event.which !== 1) return;
        if (isMoveBlock) {
            selectItem = defaulSelectItem;
            isMoveBlock = false;
            return;
        }
        else if (selectItem.use === stageItem.move) {
            if (hoverItem === defaulHoverItem && tradeInfo.Active === false && selectItem !== defaulSelectItem && getDropItem (selectItem.arrayName, selectItem.ItemId) !== false) {
                const selectIndex = selectItem.index;
                const selectArrayName = selectItem.arrayName;
                //const _Item = window.getItem (id, arrayName);
                
                // Если мышка покинула первоначальную ячейку, и перенос был в несуществубщую ячейку, то дропаем
                if (mouseLeaveSelectedItem === true && mainInventoryArea === false/* && (selectArrayName === "inventory" || selectArrayName === "backpack")*/) {
                    
                    executeClient ("client.gamemenu.inventory.drop", selectArrayName, selectIndex);                    
                }
                itemNoUse(18);
            } else if (hoverItem !== defaulHoverItem && (hoverItem.index !== selectItem.index || hoverItem.arrayName !== selectItem.arrayName)) {
                //Item на который наводимся
                let hoverIndex = hoverItem.index;
                const hoverArrayName = hoverItem.arrayName;
                let _hItem = getItemToIndex (hoverIndex, hoverArrayName);
                let _hInfoItem = window.getItem (_hItem.ItemId);
                //Выбранный
                let selectIndex = selectItem.index;
                const selectArrayName = selectItem.arrayName;
                let _sItem = getItemToIndex (selectIndex, selectArrayName);
                let _sInfoItem = window.getItem (_sItem.ItemId);

                let returnMove = -1;
                if (!_hItem.use || hoverArrayName === "with_trade") {
                    itemNoUse (19);
                    window.notificationAdd(4, 9, translateText('player1', 'Данный слот не доступен!'), 3000);
                    return;
                } else if ((hoverArrayName === "accessories" || hoverArrayName === "fastSlots") && selectArrayName !== "inventory") {
                    itemNoUse (20);
                    window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                    return;
                } else if ((selectArrayName === "accessories" || selectArrayName === "fastSlots") && hoverArrayName !== "inventory") {
                    itemNoUse (21);
                    window.notificationAdd(4, 9, translateText('player1', 'Сначала переложите предмет в собственный инвентарь!'), 3000);
                    return;
                } else if (hoverArrayName === "other" && OtherInfo.Id === otherType.Nearby) {
                    executeClient ("client.gamemenu.inventory.drop", selectArrayName, selectIndex);   
                    itemNoUse(18);
                    return;
                }          
                
                if (hoverArrayName !== selectArrayName && (returnMove = isMove (hoverIndex, hoverArrayName, _sItem, _sInfoItem)) == -2) {
                    itemNoUse (22);
                    return;
                }
                
                if (hoverArrayName === "other" && OtherInfo.Id === otherType.Tent && OtherInfo.IsMyTent) {
                    executeClient ("client.gamemenu.inventory.stack", selectArrayName, selectIndex, 2, _sItem.Count);
                    itemNoUse(18);
                    return;
                }  

                if (returnMove !== -1) {
                    hoverIndex = returnMove;
                    _hItem = getItemToIndex (hoverIndex, hoverArrayName);
                    _hInfoItem = window.getItem (_hItem.ItemId);
                    if (isMove (hoverIndex, hoverArrayName, _sItem, _sInfoItem) == -2) {
                        itemNoUse (23);
                        return;
                    }
                }
                returnMove = -1;
                
                if (hoverArrayName !== selectArrayName && (returnMove = isMove (selectIndex, selectArrayName, _hItem, _hInfoItem)) == -2) {
                    itemNoUse (24);
                    return;
                }

                if (returnMove !== -1) {
                    selectIndex = returnMove;
                    _sItem = getItemToIndex (selectIndex, selectArrayName);
                    _sInfoItem = window.getItem (_sItem.ItemId);
                
                    if (isMove (selectIndex, selectArrayName, _hItem, _hInfoItem) == -2) {
                        itemNoUse (25);
                        return;
                    }
                }

                let MaxStakcItems = 0;
                if ((hoverArrayName !== "other" && hoverArrayName !== "backpack") && (selectArrayName === "other" || selectArrayName === "backpack") && ![0, 237, 238, 239, 240, 241, 242, 245, 246, 247].includes (_sItem.ItemId) && (MaxStakcItems = getMaxStakcItems (_sItem, _sInfoItem)) == -1) {
                    //window.notificationAdd(4, 9, `Нет места для ${itemInfo.Name}, максимум можно иметь при себе - ${itemInfo.Stack} шт. | У вас ${count + item.Count} шт.`, 3000);
                    itemNoUse (26);
                    return;
                }

                if (MaxStakcItems > 0) {
                    if (_hItem.ItemId === _sItem.ItemId || _hItem.ItemId === 0) {
                        executeClient ("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);
                        /*if (_hItem.ItemId === _sItem.ItemId && _hItem.Count === _sInfoItem.Stack) {                            
                            setItem (hoverIndex, hoverArrayName, _sItem);
                            setItem (selectIndex, selectArrayName, _hItem);
                        }
                        else */if (_hItem.ItemId === _sItem.ItemId) {
                            _hItem.Count += MaxStakcItems;
                            _sItem.Count -= MaxStakcItems;
                            setItem (hoverIndex, hoverArrayName, _hItem);
                            setItem (selectIndex, selectArrayName, _sItem);
                            executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
                        } else {
                            _sItem.Count -= MaxStakcItems;
                            setItem (selectIndex, selectArrayName, _sItem);
                            _hItem = _sItem;
                            _hItem.Count = MaxStakcItems;
                            setItem (hoverIndex, hoverArrayName, _hItem);
                            executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
                        }
                    }
                    else window.notificationAdd(4, 9, `${translateText('player1', 'Нет места для')} ${_sInfoItem.Name}, ${translateText('player1', 'максимум можно иметь при себе')} - ${_sInfoItem.Stack} ${translateText('player1', 'шт.')}`, 3000);
                    itemNoUse (27);
                    return;
                }

                MaxStakcItems = 0;
                if ((selectArrayName !== "other" && selectArrayName !== "backpack") && (hoverArrayName === "other" || hoverArrayName === "backpack") && ![0, 237, 238, 239, 240, 241, 242, 245, 246, 247].includes (_hItem.ItemId) && _hItem.ItemId != _sItem.ItemId && (MaxStakcItems = getMaxStakcItems (_hItem, _hInfoItem)) == -1) {
                    itemNoUse (28);
                    return;
                }

                if (MaxStakcItems > 0) {
                    /*if (_hItem.Count === _hInfoItem.Stack) {      
                        executeClient ("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);                      
                        setItem (hoverIndex, hoverArrayName, _sItem);
                        setItem (selectIndex, selectArrayName, _hItem);
                    } else */if (_hItem.ItemId === _sItem.ItemId) {
                        executeClient ("client.gamemenu.inventory.move.stack", selectArrayName, selectIndex, hoverArrayName, hoverIndex, MaxStakcItems);
                        _sItem.Count += MaxStakcItems;
                        _hItem.Count -= MaxStakcItems;
                        setItem (hoverIndex, hoverArrayName, _hItem);
                        setItem (selectIndex, selectArrayName, _sItem);
                        executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
                    }
                    else window.notificationAdd(4, 9, `${translateText('player1', 'Нет места для')} ${_sInfoItem.Name}, ${translateText('player1', 'максимум можно иметь при себе')} - ${_sInfoItem.Stack} ${translateText('player1', 'шт.')}`, 3000);
                    itemNoUse (29);
                    return;
                }
                executeClient ("client.gamemenu.inventory.move", selectArrayName, selectIndex, hoverArrayName, hoverIndex);

                //{"Name":"Маска","Description":"","Icon":"item-pizza","Type":"Одежда","Model":3887136870,"Stack":1,"functionType":1}                
                if (_hItem.ItemId === _sItem.ItemId && Number (_hInfoItem.Stack) > 1 && Number (_hInfoItem.Stack) > _sItem.Count && Number (_hInfoItem.Stack) > _hItem.Count) {
                    const amount = (_hItem.Count === undefined || _hItem.Count < 2 || !isNumber(_hItem.Count)) ? 1 : _hItem.Count;

                    if (Number (_hInfoItem.Stack) >= (amount + _sItem.Count)) {
                        _sItem.Count += amount;
                        setItem (hoverIndex, hoverArrayName, _sItem);
                        setItem (selectIndex, selectArrayName, clearSlot);
                    } else {
                        _hItem.Count = (amount + _sItem.Count) - _hInfoItem.Stack;
                        _sItem.Count = _hInfoItem.Stack;
                        setItem (hoverIndex, hoverArrayName, _sItem);
                        setItem (selectIndex, selectArrayName, _hItem);
                    }
                } else {
                    setItem (hoverIndex, hoverArrayName, _sItem);
                    setItem (selectIndex, selectArrayName, _hItem);
                }
                executeClient ("sounds.playInterface", "inventory/drag_drop", 0.05);
            }
            itemNoUse (30, true);
        }
    }
    
    const handleGlobalMouseDown = (event) => {
        if (!visible) return;
        else if (event.which !== 1) return;
        else if (selectItem.tent && clickTime >= new Date().getTime()) return;
        else if (selectItem.use === stageItem.useItem && !useInventoryArea) {
            itemNoUse (31);
        }
    }

	const getItemToIndex = (index, arrayName) => {
        return ItemsData[arrayName][index];
    }

    const unHoverAll = () => {
        for (let arrayName in ItemsData) {
            ItemsData[arrayName].forEach((item, index) => {
                if (item.hover) updateItem(index, arrayName, "hover");                 
            })
        }
    }

    const isNumber = (value) => {
        return /^[-]?\d+$/.test(value);
    }

    const updateItem = (index, arrayName, name, value = null) => {
        if (!arrayName && !index) return;
        if (ItemsData[arrayName][index].ItemId != 0) {
            value = (value === null) ? !ItemsData[arrayName][index][name] : value;
            ItemsData[arrayName][index][name] = value;
            return value;
        }
        return -9;
    }

	// Обновление полного массива item`а
	const setItem = (index, arrayName, item) => {
        if (item.active) item.active = false;
        ItemsData[arrayName][index] = {
            ...item,
            index: index
        };
        
        //window.CallServer("UpdateSlotData", getInfo[arrayName], index, item.item_id, item.ItemId, item.item_amount, item.item_trade, item.item_money);
    }

    const itemNoUse = (hash, toggled = false) => {
        let hoverIndex = -1,
            hoverArrayName = -1;
        if (selectItem !== defaulSelectItem) updateItem(selectItem.index, selectItem.arrayName, "hover", false);

        clickTime = 0;
        
        selectItem = defaulSelectItem;

        if (hoverItem !== defaulHoverItem) {
            hoverIndex = hoverItem.index;
            hoverArrayName = hoverItem.arrayName;
        }
        //hoverItem = defaulHoverItem;
        if (hoverIndex === -1 && hoverArrayName === -1) infoItem = defaulHoverItem;
        else {            
            const _Item = getItemToIndex (hoverIndex, hoverArrayName);
            if (_Item.ItemId != 0) {
                infoItem = {
                    ..._Item,
                    index: hoverIndex,
                    arrayName: hoverArrayName
                };
            } else infoItem = defaulHoverItem;
        }
        //toggledSplitter = false;

        ItemStack = -1;
        StackValue = 1;
        
    }

    const fixOutToX = (coordsX, element) => {
        if (!element) return coordsX;
        else if (document.querySelector('.box-inventory')) {
            let mainWidth = document.querySelector('.box-inventory').getBoundingClientRect().width;
            let elementWidth = element.getBoundingClientRect().width;
            if ((elementWidth + coordsX) >= mainWidth) return coordsX - elementWidth;
            return coordsX;
        }
        return coordsX;
    }

    const fixOutToY = (coordsY, element) => {
        if (!element) return coordsY;
        else if (document.querySelector('.box-inventory')) {
            let mainHeight = document.querySelector('.box-inventory').getBoundingClientRect().height;
            let elementHeight = element.getBoundingClientRect().height;
            if ((coordsY - elementHeight) < 0) return coordsY;
            return coordsY - elementHeight;
        }
        return coordsY;
    }

    const fixOutToCenter = (coordsX, element) => {
        if (!element) return coordsX;
        let elementWidth = element.getBoundingClientRect().width / 2;
        return coordsX - elementWidth;
    }

    const fixOutToTop = (coordsY, element) => {
        if (!element) return coordsY;
        let elementHeight = element.getBoundingClientRect().height;
        return coordsY - (elementHeight + (elementHeight * 0.2));
    }
    
    const animItems = (index, arrayName) => {
        ItemsData[arrayName][index] = {
            ...ItemsData[arrayName][index],
            anim: false
        };
        
        setTimeout(() => {
            ItemsData[arrayName][index] = {
                ...ItemsData[arrayName][index],
                anim: true
            };
        }, 0);
    }

    /* Трейд */
    const TradeCancel = () => {
        if (tradeInfo.YourStatus) {
            tradeInfo.YourStatus = false;
            tradeInfo.YourStatusChange = false;
            ItemsData.trade.forEach((item, index) => {
                ItemsData.trade[index] = {
                    ...ItemsData.trade[index],
                    use: true
                };
            });
            executeClient ("client.gamemenu.inventory.trade", 0);
        }
    }

    const TradeSelect = () => {   
        if (!tradeInfo.YourStatus) {
            let seccuss = false;
            ItemsData.trade.forEach((item, index) => {
                if (selectItem.ItemId !== 0) {
                    seccuss = true;
                }
            });

            if (tradeInfo.WithStatus && !seccuss) {// Проверяем, если у нас пустые слоты и второй игрок подтвердил трейд то
                
                ItemsData.with_trade.forEach((item, index) => {
                    if (selectItem.ItemId !== 0) {
                        seccuss = true;
                    }
                });
                if (!seccuss) { //Если у второго тоже пустые слото то выдаем ошибку
                    window.notificationAdd(4, 9, translateText('player1', 'Для начала выберите предмет!'), 3000);
                    return;
                }
            }         
            executeClient ("client.gamemenu.inventory.trade", 1);
        } else if (tradeInfo.YourStatus && tradeInfo.WithStatus && !tradeInfo.YourStatusChange) {
            executeClient ("client.gamemenu.inventory.trade", 2);
        }
    }

    const TradeUpdate = (status) => {
        if (status == -1) {
            tradeInfo.YourStatus = true;
            ItemsData.trade.forEach((item, index) => {
                ItemsData.trade[index] = {
                    ...ItemsData.trade[index],
                    use: false
                };
            });   
            return;
        }
        if (status == -2) {
            tradeInfo.YourStatusChange = true;
            window.notificationAdd(4, 9, translateText('player1', 'Вы подтвердили свою готовность!'), 3000);
            return;
        }
        let lastWithStatus = tradeInfo.WithStatus;
        if (!status) {
            tradeInfo.WithStatus = false;
            tradeInfo.WithStatusChange = false;
            tradeInfo.YourStatusChange = false;
        }
        else if (status === 1) tradeInfo.WithStatus = true;
        else if (status === 2) tradeInfo.WithStatusChange = true;

        ItemsData.with_trade.forEach((item, index) => {
            ItemsData.with_trade[index] = {
                ...ItemsData.with_trade[index],
                use: tradeInfo.WithStatus ? false : true
            };
        });

        if (lastWithStatus !== tradeInfo.WithStatus) {            
            selectItem = defaulSelectItem;
            isMoveBlock = false;
            if (!tradeInfo.YourStatus) {
                ItemsData.trade.forEach((item, index) => {
                    ItemsData.trade[index] = {
                        ...ItemsData.trade[index],
                        use: tradeInfo.WithStatus ? false : true
                    };
                });
            }
        }
        if (status === 1 && !tradeInfo.YourStatus) {
            window.notificationAdd(4, 9, `${tradeInfo.WithName} ${translateText('player1', 'выбрал предметы для трейда')}!`, 3000);
        } else if (status === 1 && tradeInfo.YourStatus) {
            window.notificationAdd(4, 9, `${translateText('player1', 'Вы и')} ${tradeInfo.WithName} ${translateText('player1', 'выбрали предметы для трейда')}`, 3000);
        } else if (status === 0) {
            window.notificationAdd(4, 9, `${tradeInfo.WithName} ${translateText('player1', 'выбирает предметы')}`, 3000);
        } else if (status === 2) {
            window.notificationAdd(4, 9, `${tradeInfo.WithName} ${translateText('player1', 'готов к обмену')}`, 3000);
        }
    }

    const rangeslidercreate = (max) => {
        StackValue = Math.round(max / 2);
        setTimeout(() => {
            rangeslider.create(document.getElementById("stack"), {min: 1, max: max, value: Math.round(max / 2), step: 1, onSlide: (value, percent, position) => {
                StackValue = Number(value);
            }});
        }, 0);

    }

    const createRangeSlider = () => {
        rangeslider.create(document.getElementById("invOpacity"), {min: 0, max: 1, value: invOpacity, step: 0.1, onSlide: (value, percent, position) => {
            invOpacity = value;
        }});
    }
    const entityMap = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#39;',
        '/': '&#x2F;',
        '`': '&#x60;',
        '=': '&#x3D;'
    };
    
    const escapeHtml = (str) => {
        return String(str).replace(/[&<>"'`=\/]/g, function (s) {
            return entityMap[s];
        });
    }
    const getName = (Item, arrayName) => {
        const _infoItem = window.getItem (Item.ItemId);
        let name = escapeHtml (_infoItem.Name);
        if (Item.ItemId == -9 && arrayName !== "accessories") {
            name += `<span>Состояние: ${Item.Data}</span>`;
        }
        else if (Item.ItemId == 19 && Item.Data.split("_")) {
            name += `<span>${Item.Data.split("_")[0]} | ${Item.Data.split("_")[1]}</span>`;
        } 
        else if (_infoItem.Stack > 1 && Item.Count > 1) {
            name += ` | ${Item.Count} шт.`;
        } else if (_infoItem.functionType === ItemType.Modification && Item.Data.split("_").length) {
            let dataParse = Item.Data.split("_");
            if (WeaponHashToItem [dataParse[0]]) name += `<span>Weapon: ${window.getItem (WeaponHashToItem [dataParse[0]]).Name}</span>`;
        } else if (Item.ItemId == 220) {
            name += `<span>${Item.Data}</span>`;
        }
        else if (Item.ItemId == 234 || Item.ItemId == 235 || Item.ItemId == 236 || Item.ItemId == 244
            || Item.ItemId == 225 || Item.ItemId == 229 || Item.ItemId == 249) {
            let ItemValue = 300;

            switch (Item.ItemId) {
                case 234:
                    ItemValue = 300;
                    break;
                case 235:
                    ItemValue = 1248;
                    break;
                case 236:
                    ItemValue = 2250;
                    break;
                case 244:
                    ItemValue = 1250;
                    break;
                case 225:
                    ItemValue = 100;
                    break;
                case 229:
                    ItemValue = 420;
                    break;
                case 249:
                    ItemValue = 1000;
                    break;
            }
            
            name += `<span>${translateText('player1', 'Состояние')}: ${weaponCondition (Item.Data, wMaxHP [Item.ItemId])}%</span>`;
        }
        else if (Item.ItemId == ItemId.SimCard || Item.ItemId == ItemId.VehicleNumber)
            name += `<span>${Item.Data}</span>`;
        return name;
    }

    const getNameToData = (Item, arrayName) => {
        const _infoItem = window.getItem (Item.ItemId);
        let name = "";
        if (_infoItem.functionType === ItemType.Weapons) {
            return Item.Data;
        }
        return name;
    }

    const getDropItem  = (arrayName, ItemId) => {
        if (arrayName === "fastSlots") return false;
        switch (ItemId) {
            case 2: return false;
            //case 243: return false;
        }
        return true;
    }
    

    const getItemsUse  = (Item) => {
        if (Item.arrayName !== "inventory" && Item.arrayName !== "accessories" && Item.arrayName !== "fastSlots") return false;
        else if (Item.arrayName === "accessories" && (Item.ItemId === 12 || Item.ItemId === 15)) return false;
        const _infoItem = window.getItem (Item.ItemId);
        if (_infoItem.functionType === ItemType.Clothes || _infoItem.functionType === ItemType.Weapons || _infoItem.functionType === ItemType.MeleeWeapons || _infoItem.functionType === ItemType.Alco || _infoItem.functionType === ItemType.Cases) return true;
        else if (Item.ItemId == 41 && ((OtherInfo.Id !== otherType.None && OtherInfo.Id !== otherType.Key) || tradeInfo.Active)) return false;
        switch (Item.ItemId) {
            case 220:
            case 4:
            case 41:
            case 19:
            case 13:
            case 6:
            case 14:
            case 9:
            case 2:
            case 1:
            case 280:
            case 7:
            case 11:
            case 16:
            case 5:
            case 8:
            case 10:
            case 3:
            case 40:
            case 12:
            case 225:
            case 15:
            case 248:
            case 229:
            case ItemId.SimCard:return true;
            //case 249: return true;
        }
        return false;
    }

    const getItemsClickInfo = (info) => {
        let _infoItem = window.getItem (info.ItemId);
        if (info.arrayName === "accessories") return `<span class="inventoryicons-shipment inventoryicon-use"></span> ${translateText('player1', 'снять')}`;
        else if (_infoItem.functionType === ItemType.Clothes) return `<span class="inventoryicons-fashion inventoryicon-use"></span> ${translateText('player1', 'надеть')}`;
        else if (info.arrayName !== "fastSlots" && (_infoItem.functionType === ItemType.Weapons || _infoItem.functionType === ItemType.MeleeWeapons) && ItemToWeaponHash [info.ItemId] && wComponents [ItemToWeaponHash [info.ItemId]] && wComponents [ItemToWeaponHash [info.ItemId]].Components) return `<span class="inventoryicons-rifle inventoryicon-use"></span> ${translateText('player1', 'модификации')}`;
        else if (info.arrayName !== "fastSlots" && (_infoItem.functionType === ItemType.Weapons || _infoItem.functionType === ItemType.MeleeWeapons)) return `<span class="inventoryicons-hand inventoryicon-use"></span> ${translateText('player1', 'взять')}`;
        else if (_infoItem.functionType === ItemType.Alco || _infoItem.functionType === ItemType.Water) return `<span class="inventoryicons-soft-drink inventoryicon-use"></span> ${translateText('player1', 'выпить')}`;
        else if (_infoItem.functionType === ItemType.Eat) return `<span class="inventoryicons-restaurant inventoryicon-use"></span> ${translateText('player1', 'съесть')}`;
        else if (info.ItemId == 41 && OtherInfo.Id === otherType.None) return `<span class="inventoryicons-open-padlock inventoryicon-use"></span> ${translateText('player1', 'открыть')}`;
        else if ((OtherInfo.Id === otherType.wComponents || OtherInfo.Id === otherType.Key) && OtherSqlId && Number (OtherSqlId) === Number (info.SqlId)) return `<span class="inventoryicons-padlock inventoryicon-use"></span> ${translateText('player1', 'закрыть')}`;
        else if ((info.ItemId == 225 || info.ItemId == 229) && OtherInfo.Id === otherType.None) return `<span class="inventoryicons-smoking inventoryicon-use"></span> ${translateText('player1', 'курить')}`;
        else if (_infoItem.functionType === ItemType.Cases) return `<span class="inventoryicons-open-padlock inventoryicon-use"></span> ${translateText('player1', 'открыть')}`;
        else if (info.ItemId == ItemId.SimCard) return `<span class="inventoryicons-open-padlock inventoryicon-use"></span> ${translateText('player1', 'Вставить')}`;
        return `<span class="inventoryicons-settings inventoryicon-use"></span> ${translateText('player1', 'использовать')}`;
    }

    const onItemsHands = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;

            executeClient ("client.gamemenu.inventory.hands", selectArrayName, selectIndex);
            itemNoUse (32);
        }
    }

    const getToPut = (ItemId) => {
        switch (ItemId) {
            case 234:
            case 235:
            case 236:
            case 237:
            case 238:
            case 239:
            case 240:
            case 241:
            case 242:
            case 248:
            case 229: return false;
            //case 249: return false;
        }
        return true;
    }

    const onToPut = () => {
        if (selectItem.use === stageItem.useItem) {
            const selectIndex = selectItem.index;
            const selectArrayName = selectItem.arrayName;

            executeClient ("client.gamemenu.inventory.toput", selectArrayName, selectIndex);
            itemNoUse (33);
        }
    }
    
    const handleMouseDownBlock = (event, eClass, arrayName) => {
        /*if (event.which == 1) {
            if (selectItem.use === stageItem.none) {
                itemNoUse (34);

                const target = document.querySelector('.box-inventory .' + eClass).getBoundingClientRect();

                const offsetInElementX = (target.width - (target.right - event.clientX));
                const offsetInElementY = (target.height - (target.bottom - event.clientY));

                selectItem = {
                    use: stageItem.moveBlock,
                    width: target.width,
                    height: target.height,
                    offsetInElementX: offsetInElementX,
                    offsetInElementY: offsetInElementY,
                } 
                moveBlock[arrayName] = [
                    event.clientY - offsetInElementY,
                    event.clientX - offsetInElementX,
                    //event.clientX - offsetInElementX
                ]

                isMoveBlock = arrayName;

            }
        }*/
    }

    let boxItemInfo;
    let boxInfoLeft = 0;
    let boxInfoTop = 0;

    $: if (boxItemInfo) {
        boxInfoLeft = fixOutToCenter ($coords.x, boxItemInfo);
        boxInfoTop = fixOutToTop ($coords.y, boxItemInfo);
    }

    let boxPopup;

    const weaponCondition = (hp, maxHp) => {
        const condition = Math.floor((hp / maxHp) * 100);
        if (!condition || isNaN(condition)) {
            return 0;
        }
        return condition;
    }

    const onOpenBattlePass = () => {
        executeClient ("client.battlepass.open");
    }
</script>

<svelte:window on:mousemove={handleGlobalMouseMove} on:mouseup={handleGlobalMouseUp} on:mousedown={handleGlobalMouseDown} on:keyup={onKeyUp} on:keydown={onKeyDown} />

{#if visible}
<div class="box-inventory">
    {#if (selectItem.use === stageItem.move)}
        <div class="dragonDrop" style="width: {selectItem.width}px;height: {selectItem.height}px;top: {$coords.y - selectItem.offsetInElementY}px;left: {$coords.x - selectItem.offsetInElementX}px;">
            <Slot
                item={selectItem}
                iconInfo={window.getItem (selectItem.ItemId)} />
        </div>
    {/if}

    <!--Информация о предмете-->
    {#if (infoItem !== defaulHoverItem && selectItem.use !== stageItem.move)} 
        <div class="box-item-info" style="top: {boxInfoTop}px;left: {boxInfoLeft}px;" bind:this={boxItemInfo}>
            <div class="bg-info">
                <div class="title">{@html getName (infoItem, infoItem.arrayName)}</div>
                {#if wMaxHP [infoItem.ItemId] && infoItem.Data && infoItem.Data.split("_") && infoItem.Data.split("_").length > 1 && infoItem.Data.split("_")[1] != undefined}
                    <div class="type" style="color: #FFA07A"> | Состояние: {weaponCondition (infoItem.Data.split("_")[1], wMaxHP [infoItem.ItemId])}% </div>
                {/if}
<!--            <span class={`${window.getItem (infoItem.ItemId).Icon} icon`} />
                <div class="type">{window.getItem (infoItem.ItemId).Type}</div>
                <div class="desc">{window.getItem (infoItem.ItemId).Description}</div> -->
                {#if OtherInfo.Id == otherType.Tent && infoItem.Price}
                    <div class="type" style="color: #FFA07A">Цена за 1 ед. $ {format("money", infoItem.Price)}</div>
                {/if}
            </div>
        </div>
    {/if}

    {#if selectItem.use === stageItem.useItem && selectItem.tent && !OtherInfo.IsMyTent}
        <div bind:this={boxPopup} class="box-stack" 
            style="top: {fixOutToY ($coords.y, boxPopup)}px;left: {fixOutToX ($coords.x, boxPopup)}px;"
            on:mouseenter={e => useInventoryArea = true} on:mouseleave={e => useInventoryArea = false}>
            <div class="box-text">
                <span class='icon {selectItem.info.Icon}' />{@html getName (selectItem, selectItem.arrayName)}
            </div>
            <div class="box-number">
                {translateText('player1', 'Кол-во')}: <input type="number" bind:value={StackValue} class="box-number-input" on:input={(event) => handleInputStackChange (event.target.value)} onBlur={onBlurStack} />
            </div>
            {#if selectItem.Count > 1}
            <div class="slider box-slider">
                <input type="range" id="stack" />
            </div>
            {/if}
            <div class="btn-slap" on:click={onBuy}>Купить</div>
            <div class="box-cancel"><span class='icon inv-close' on:click={e => selectItem = defaulSelectItem} /></div>
        </div>
    {:else if selectItem.use === stageItem.useItem && ItemStack === -1}
    <div bind:this={boxPopup} class="box-use" 
        style="top: {fixOutToY ($coords.y, boxPopup)}px;left: {fixOutToX ($coords.x, boxPopup)}px;"
        on:mouseenter={e => useInventoryArea = true} on:mouseleave={e => useInventoryArea = false}>
        
        <div class="box-item-weapon">
            <div class="bg-info">
               <div class="box-column">
                    <div class="box-item-weapon__image" style="background-image: url({getPng(selectItem, itemsInfo [selectItem.ItemId])})"></div>
                    <div class="box-item-weapon__line"></div>
                    <div class="title">
                        {@html getName (selectItem, selectItem.arrayName)}
                    </div>
                    <div class="type">{itemsInfo [selectItem.ItemId].Type}</div>
                </div>
                <div class="title left">{translateText('player1', 'Описание')}</div>
                <div class="desc left">{itemsInfo [selectItem.ItemId].Description} [DEBUG-INFO: {getPng(selectItem, itemsInfo [selectItem.ItemId]).replace(document.cloud, '').replace(".png", '')}]</div>
                {#if itemsInfo [selectItem.ItemId].functionType === ItemType.Weapons && inventoryWeapons [selectItem.ItemId]}
                    <div class="title left">{translateText('player1', 'Характеристики')}</div>
                    {#if wMaxHP [selectItem.ItemId] && selectItem.Data && selectItem.Data.split("_") && selectItem.Data.split("_").length > 1 && selectItem.Data.split("_")[1] != undefined}
                        <div class="box-item-weapon__element">
                            <div>{translateText('player1', 'Состояние')}</div>
                            <div class="weapon__progress">
                                <div class="weapon__progress-line" style="width: {Math.floor((selectItem.Data.split("_")[1] / wMaxHP [selectItem.ItemId]) * 100)}%"></div>
                            </div>
                        </div>
                    {/if}
                    <div class="box-item-weapon__element">
                        <div>{translateText('player1', 'Урон')}</div>
                        <div class="weapon__progress">
                            <div class="weapon__progress-line" style="width: {inventoryWeapons [selectItem.ItemId].damage}%"></div>
                        </div>
                    </div>
                    <div class="box-item-weapon__element">
                        <div>{translateText('player1', 'Скорость')}</div>
                        <div class="weapon__progress">
                            <div class="weapon__progress-line" style="width: {inventoryWeapons [selectItem.ItemId].firerate}%"></div>
                        </div>
                    </div>
                    {#if inventoryWeapons [selectItem.ItemId].accuracy}
                        <div class="box-item-weapon__element">
                            <div>{translateText('player1', 'Точность')}</div>
                            <div class="weapon__progress">
                                <div class="weapon__progress-line" style="width: {inventoryWeapons [selectItem.ItemId].accuracy}%"></div>
                            </div>
                        </div>
                    {/if}
                    <div class="box-item-weapon__element">
                        <div>{translateText('player1', 'Дальность')}</div>
                        <div class="weapon__progress">
                            <div class="weapon__progress-line" style="width: {inventoryWeapons [selectItem.ItemId].range}%"></div>
                        </div>
                    </div>
                    {#if inventoryWeapons [selectItem.ItemId].ammo}
                        <div class="box-item-weapon__element">
                            <div>{translateText('player1', 'Патронов в магазине')}</div>
                            <div>{inventoryWeapons [selectItem.ItemId].ammo}</div>
                        </div>
                    {/if}

                {/if}
                <!--<div class="title left">Особенности</div>
                <div class="box-item-weapon__element">
                    <div>Масса</div>
                    <div> 2 кг</div>
                </div>-->
            </div>
            {#if itemsInfo [selectItem.ItemId].functionType === ItemType.Weapons}
            <div class="box-item-weapon__serial">
                <div class="weapon__serial_title">{translateText('player1', 'Серийный номер')}</div>
                <div class="weapon__serial_subtitle">{getNameToData (selectItem, selectItem.arrayName)}</div>
            </div>
            {:else if OtherInfo.Id == otherType.Tent && selectItem.Price}
            <div class="box-item-weapon__serial">
                <div class="weapon__serial_title">{translateText('player1', 'Цена за 1 ед')}</div>
                <div class="weapon__serial_subtitle">${format("money", selectItem.Price)}</div>
            </div>
            {/if}
        </div>
        {#if OtherInfo.Id == otherType.Tent && OtherInfo.IsMyTent && selectItem.arrayName === "other"}
            <div class="item" on:click={onTransfer}><span class="inventoryicons-backpack inventoryicon-use"></span>{translateText('player1', 'забрать')}</div>
        {:else}
            {#if getItemsUse (selectItem) !== false}
                <div class="item" on:click={onUseItem}>{@html getItemsClickInfo (selectItem)}</div>
            {/if}
            {#if getToPut (selectItem.ItemId) !== false && getDropItem (selectItem.arrayName, selectItem.ItemId) !== false && selectItem.arrayName === "inventory"}
                <div class="item" on:click={onToPut}><span class="inventoryicons-near inventoryicon-use"></span>{translateText('player1', 'поставить')}</div>
            {/if}
            {#if OtherInfo.Id != otherType.Tent && (OtherInfo.Id > otherType.None || (maxSlotBackpack > 0 && ItemsData["backpack"].length) || tradeInfo.Active === true) && selectItem.arrayName !== "fastSlots" && selectItem.arrayName !== "accessories"}
                <div class="item" on:click={onTransfer}><span class="inventoryicons-hand inventoryicon-use"></span>{(selectItem.arrayName === "other" || selectItem.arrayName === "backpack" || selectItem.arrayName === "trade") ? translateText('player1', 'взять') : translateText('player1', 'передать')}</div>
            {:else if OtherInfo.Id == otherType.Tent && OtherInfo.IsMyTent && (selectItem.arrayName === "inventory" || selectItem.arrayName === "backpack" )}
                <div class="item" on:click={onTransfer}><span class="inventoryicons-shop-cart inventoryicon-use"></span>{translateText('player1', 'продать')}</div>
            {/if}
            {#if selectItem.Count > 1 && selectItem.arrayName !== "fastSlots"}
                <div class="item" on:click={e => {
                    ItemStack = 0;
                    rangeslidercreate (selectItem.Count - 1);
                }}><span class="inventoryicons-razdel inventoryicon-use"></span>{translateText('player1', 'разделить')}</div>
            {/if}
            {#if getDropItem (selectItem.arrayName, selectItem.ItemId) !== false}
                <div class="item" on:click={onDropItem}><span class="inventoryicons-drop inventoryicon-use"></span>{translateText('player1', 'выбросить')}</div>
            {/if}
        {/if}
    </div>
    {:else if selectItem.use === stageItem.useItem && ItemStack !== -1}
    <div bind:this={boxPopup} class="box-stack" 
        style="top: {fixOutToY ($coords.y, boxPopup)}px;left: {fixOutToX ($coords.x, boxPopup)}px;"
        on:mouseenter={e => useInventoryArea = true} on:mouseleave={e => useInventoryArea = false}>
        <div class="box-text">
            <span class='icon inv-slap' />{translateText('player1', 'Разделение предмета')}
        </div>
        <div class="box-number">
            {translateText('player1', 'Кол-во')}: <input type="number" bind:value={StackValue} class="box-number-input" on:input={(event) => handleInputStackChange (event.target.value)} onBlur={onBlurStack} />
        </div>
        {#if selectItem.Count > 2}
        <div class="slider box-slider">
            <input type="range" id="stack" />
        </div>
        {/if}
        <div class="box-between" style="width: 100%">
            <div class="btn-slap" on:click={onStack}>{!ItemStack ? translateText('player1', 'Разделить') : ItemStack == 1 ? translateText('player1', 'Выбросить') : translateText('player1', 'Передать')}</div>
            <div class="box-cancel" on:click={e => ItemStack = -1}>{translateText('player1', 'Отмена')}</div>
        </div>
    </div>
    {/if}
    
    <div class="box-KeyInfo">
        <div class="KeyInfo text">Ctrl</div>
        {translateText('player1', 'Скрыть')}
    </div>
    <div class="box-between">
        <div class="box-width-457 accessories box-column" style="position: {moveBlock["accessories"][0] == null ? "unset" : "unset"}">
            <div class="battlepaass__box" on:click={onOpenBattlePass}>Нажмите, чтобы открыть зимний пропуск</div>
            <div class="box-accessories" style="top: {moveBlock["accessories"][0]}px;left: {moveBlock["accessories"][1]}px;" on:mousedown={(event) => handleMouseDownBlock(event, "box-accessories", "accessories")} on:mouseup={setAccessories} on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                <div class={"skin " + (Bool($charGender) || "women")} />
                <div class="inventory__accessories">
                    <div class="box-column">
                        <div class="inventory__title">{translateText('player1', 'Персонаж')}</div>
                        <div class="inventory__text">{translateText('player1', 'Одежда, надетая на персонажа')}.</div>
                    </div>
                    <div class="box-between box-align-center">
                        <Slot
                        item={ItemsData["accessories"][clothes.Masks.slotId]}
                        iconInfo={window.getItem (ItemsData["accessories"][clothes.Masks.slotId].ItemId)}
                        defaultIcon={clothes.Masks.icon}
                        defaultName={translateText('player1', 'маска')}
                        on:mousedown={(event) => handleMouseDown(event, clothes.Masks.slotId, "accessories")}
                        on:mouseup={handleSlotMouseUp}
                        on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Masks.slotId, "accessories")}
                        on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Armors.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Armors.slotId].ItemId)}
                            defaultIcon={clothes.Armors.icon}
                            defaultName={translateText('player1', 'бронежилет')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Armors.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Armors.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Bags.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Bags.slotId].ItemId)}
                            defaultIcon={clothes.Bags.icon}
                            defaultName={translateText('player1', 'рюкзак')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Bags.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Bags.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                    </div>
                    <div class="inventory__accessories_lines"></div>
                    
                    <div class="inventory_grid">
                        <Slot
                            item={ItemsData["accessories"][clothes.Ears.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Ears.slotId].ItemId)}
                            defaultIcon={clothes.Ears.icon}
                            defaultName={translateText('player1', 'уши')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Ears.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Ears.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />  
                        <Slot
                            item={ItemsData["accessories"][clothes.Hats.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Hats.slotId].ItemId)}
                            defaultIcon={clothes.Hats.icon}
                            defaultName={translateText('player1', 'шапка')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Hats.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Hats.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
    
                        <Slot
                            item={ItemsData["accessories"][clothes.Glasses.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Glasses.slotId].ItemId)}
                            defaultIcon={clothes.Glasses.icon}
                            defaultName={translateText('player1', 'очки')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Glasses.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Glasses.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Accessories.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Accessories.slotId].ItemId)}
                            defaultIcon={clothes.Accessories.icon}
                            defaultName={translateText('player1', 'шея')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Accessories.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Accessories.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Undershirts.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Undershirts.slotId].ItemId)}                               
                            defaultIcon={clothes.Undershirts.icon}
                            defaultName={translateText('player1', 'футболка')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Undershirts.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Undershirts.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Tops.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Tops.slotId].ItemId)}
                            defaultIcon={clothes.Tops.icon}
                            defaultName={translateText('player1', 'верх')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Tops.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Tops.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Watches.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Watches.slotId].ItemId)}
                            defaultIcon={clothes.Watches.icon}
                            defaultName={translateText('player1', 'часы')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Watches.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Watches.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Legs.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Legs.slotId].ItemId)}
                            defaultIcon={clothes.Legs.icon}
                            defaultName={translateText('player1', 'штаны')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Legs.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Legs.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Bracelets.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Bracelets.slotId].ItemId)}
                            defaultIcon={clothes.Bracelets.icon}
                            defaultName={translateText('player1', 'браслет')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Bracelets.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Bracelets.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />

                        <Slot
                            item={ItemsData["accessories"][clothes.Torsos.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Torsos.slotId].ItemId)}
                            defaultIcon={"inv-item-hand-right"}
                            defaultName={translateText('player1', 'перчатки')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Torsos.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Torsos.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                        <Slot
                            item={ItemsData["accessories"][clothes.Shoes.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Shoes.slotId].ItemId)}
                            defaultIcon={clothes.Shoes.icon}
                            defaultName={translateText('player1', 'обувь')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Shoes.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Shoes.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                    </div>
                    <div class="inventory__accessories_line"></div>
                    <div class="box-center">  
                        <Slot
                            item={ItemsData["accessories"][clothes.Suit.slotId]}
                            iconInfo={window.getItem (ItemsData["accessories"][clothes.Suit.slotId].ItemId)}
                            defaultIcon={clothes.Suit.icon}
                            defaultName={translateText('player1', 'украшения')}
                            on:mousedown={(event) => handleMouseDown(event, clothes.Suit.slotId, "accessories")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, clothes.Suit.slotId, "accessories")}
                            on:mouseleave={handleSlotMouseLeave} />
                    </div>
                </div>
            </div>
        </div>
        <div class="e-player">
            <div class="box-absolute-player" style="position: absolute;top: {moveBlock["inventory"][0]}px;left: {moveBlock["inventory"][1]}px;" on:mousedown={(event) => handleMouseDownBlock(event, "box-absolute-player", "inventory")}>
                <div class="box-column" style="opacity: {invOpacity}">
                    <div class="inventory__title">{translateText('player1', 'Инвентарь')}</div>
                    <div class="inventory__text"><span class="inventoryicons-user inventory__icon"></span>{translateText('player1', 'Предметы, которые находятся у персонажа')}.</div>
                </div>
                <div class="box-player" style="opacity: {invOpacity}" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    {#each ItemsData["inventory"] as item, index}
                        <Slot
                            key={index}
                            item={item}
                            iconInfo={window.getItem (item.ItemId)}
                            on:mousedown={(event) => handleMouseDown(event, index, "inventory")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "inventory")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/each}
                </div>
                {#if maxSlotBackpack > 0 && ItemsData["backpack"].length && ItemsData["backpack"][0] !== undefined && ItemsData["backpack"][0].use !== undefined} <!--  || (ItemsData["backpack"][0].use === false && OtherInfo.Id === otherType.None) -->
                    <div style="opacity: {invOpacity}">
                        <div class="box-column">
                            <div class="inventory__title">{translateText('player1', 'Рюкзак')}</div>
                            <div class="inventory__text"><span class="inventoryicons-backpack inventory__icon"></span>{translateText('player1', 'Переносное хранилище предметов')}.</div>
                        </div>
                        <div class="box-other box-between inventory__box_backpack" style="opacity: {!ItemsData["backpack"][0].use ? "0.5": 1}" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                            <div class="box-list backpack">
                            {#each ItemsData["backpack"] as item, index}
                                <Slot
                                    key={index}
                                    item={item}
                                    iconInfo={window.getItem (item.ItemId)}
                                    on:mousedown={(event) => handleMouseDown(event, index, "backpack")}
                                    on:mouseup={handleSlotMouseUp}
                                    on:mouseenter={(event) => handleSlotMouseEnter(event, index, "backpack")}
                                    on:mouseleave={handleSlotMouseLeave} />
                            {/each}
                            </div>
                        </div>
                    </div>
                {/if}
            </div>
        </div>
        {#if tradeInfo.Active === false}
            <div class="box-width-457 box-other-main">
                <div class="other">
                    
                    {#if OtherInfo.Id > otherType.None}
                        <!--<div class="box-between">
                            <div class="box-column">
                                <div class="inventory__title">Машина</div>
                                <div class="inventory__text"><span class="inventoryicons-car inventory__icon"></span>Toyota Supra (LV2281337)<span class="inventoryicons-padlock inventory__icon carlock" style="color: {true ? "#CDF15C" : "#E71D36"}"></span></div>
                            </div>
                            <div class="box-column box-control">
                                <span class="inventoryicons-top inventory__icon"></span>
                                <div class="box-flex inventory__text">
                                    <span class="inventoryicons-open-box inventory__icon"></span>
                                    12/15
                                </div>
                            </div>
                        </div> -->
                        <div class="box-other" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                            
                            <div class="box-between height-box">
                                <div class="box-column height-box">
                                    <div class="inventory__title">{otherName[OtherInfo.Id].name}</div>
                                    <div class="inventory__text"><span class="{otherName[OtherInfo.Id].icon} inventory__icon"></span>{otherName[OtherInfo.Id].descr}</div>
                                </div>
                            </div>
                            <div class="box-list">
                                {#each ItemsData["other"] as item, index}
                                    <Slot
                                        key={index}
                                        item={item}
                                        iconInfo={window.getItem (item.ItemId)}
                                        on:mousedown={(event) => handleMouseDown(event, index, "other")}
                                        on:mouseup={handleSlotMouseUp}
                                        on:mouseenter={(event) => handleSlotMouseEnter(event, index, "other")}
                                        on:mouseleave={handleSlotMouseLeave} />
                                {/each}
                            </div>
                        </div>
                    {/if}
                </div>
           </div>
        {:else}
        <div class="box-width-457 box-other-trade">
            <div class="box-width-356 trade">
                <div class="box-between" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    <div class="box-column">
                        <div class="inventory__title">{translateText('player1', 'Обмен с')} {tradeInfo.WithName}</div>
                        <div class="inventory__text"><span class="trade-square" class:active={tradeInfo.YourStatus}>{@html !tradeInfo.YourStatus ? "&#10006;" : "&#10003;"}</span>{!tradeInfo.YourStatus ? translateText('player1', 'Не готов к обмену') : translateText('player1', 'Готов к обмену')}</div>
                    </div>
                    <div class="box-column box-control">
                        <span class="inventoryicons-top inventory__icon" style="opacity:0"></span>
                        <div class="box-flex inventory__text">
                            <span class="inventoryicons-open-box inventory__icon"></span>
                            {translateText('player1', 'Вы отдаёте')}
                        </div>
                    </div>
                </div>
                <div class="box-trade" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    {#each ItemsData["trade"] as item, index}
                        <Slot
                            key={index}
                            item={item}
                            iconInfo={window.getItem (item.ItemId)}
                            on:mousedown={(event) => handleMouseDown(event, index, "trade")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "trade")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/each}
                </div>
                <div class="box-input margin-top-8" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    
                    <input type="number" bind:value={tradeInfo.YourMoney} class="input" on:input={(event) => handleInputChange ("YourMoney", event.target.value)} onBlur={onBlur} placeholder="Сумма для перевода" disabled={!(!tradeInfo.YourStatusChange && !tradeInfo.YourStatus)} />
                    <div class="box-icon">
                        <span class="icon-dollar" />
                    </div>
                </div>
                <div class="box-between" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    <div class="box-column">
                        <div class="inventory__title">{translateText('player1', 'Предметы')} {tradeInfo.WithName}</div>
                        <div class="inventory__text"><span class="trade-square" class:active={tradeInfo.WithStatus}>{@html !tradeInfo.WithStatus ? "&#10006;" : "&#10003;"}</span>{!tradeInfo.WithStatus ? translateText('player1', 'Не готов к обмену') : translateText('player1', 'Готов к обмену')}</div>
                    </div>
                    <div class="box-column box-control">
                        <span class="inventoryicons-top inventory__icon" style="opacity:0"></span>
                        <div class="box-flex inventory__text">
                            <span class="inventoryicons-delivery-box inventory__icon"></span>
                            {translateText('player1', 'Вы получите')}
                        </div>
                    </div>
                </div>
                <div class="box-trade margin-top-8" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    {#each ItemsData["with_trade"] as item, index}
                        <Slot
                            key={index}
                            item={item}
                            iconInfo={window.getItem (item.ItemId)}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "with_trade")}
                            on:mouseleave={handleSlotMouseLeave} />
                    {/each}
                </div>
                <div class="box-input margin-top-8" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                    <input type="number" value={tradeInfo.WithMoney} class="input" placeholder="0" disabled />
                    <div class="box-icon">
                        <span class="icon-dollar" />
                    </div>
                </div>
                {#if tradeInfo.Active}
                    <div class="btn-box-trade" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
                        {#if tradeInfo.YourStatus === true && tradeInfo.WithStatus === true}
                        <div class="btn-trade" style="opacity: {tradeInfo.YourStatusChange ? "0.5" : 1}" on:click={TradeSelect}>{translateText('player1', 'Подтвердить')}</div>
                        {:else}
                        <div class="btn-trade" style="opacity: {tradeInfo.YourStatus ? "0.5" : 1}" on:click={TradeSelect}>{translateText('player1', 'Готов')}</div>
                        {/if}
                        <div class="btn-trade red" style="opacity: {!tradeInfo.YourStatusChange && !tradeInfo.YourStatus ? "0.5" : 1}" on:click={TradeCancel}>{translateText('player1', 'Отменить')}</div>
                    </div>                    
                {/if}
            </div>
        </div>
        {/if}
    </div>
    <div class="box-footer">
        <div class="box-width-457" />
        <div class="box-width-617" style="opacity: {invOpacity}" on:mouseenter={e => mainInventoryArea = true} on:mouseleave={e => mainInventoryArea = false}>
            <div class="box-quickuse">
                {#each ItemsData["fastSlots"] as item, index}
                    <div class="fastslots">
                        {#if index != -1}<div class="id">{index+1}</div>{/if}
                        <Slot
                            key={index}
                            index={fastSlots [index]}
                            item={item}
                            iconInfo={window.getItem (item.ItemId)}
                            defaultStyle="smoll"
                            on:mousedown={(event) => handleMouseDown(event, index, "fastSlots")}
                            on:mouseup={handleSlotMouseUp}
                            on:mouseenter={(event) => handleSlotMouseEnter(event, index, "fastSlots")}
                            on:mouseleave={handleSlotMouseLeave} />
                    </div>
                {/each}
            </div>
        </div>
        
        <div class="box-width-457">
            
        </div>
    </div>
</div>
{/if}