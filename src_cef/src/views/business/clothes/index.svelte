<script>
    import './main.sass' 
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { serverDateTime } from 'store/server'
    import { TimeFormat } from 'api/moment'
    import { format } from 'api/formatter'
    import { charGender } from 'store/chars'
    import { menu, getClothesDictionary, getBarberDictionary, clothesEmpty, getTattooDictionary } from 'json/clothes.js'
    import { clothesName } from '@/views/player/menu/elements/inventory/functions.js';
    import InputBlock from './input_item.svelte';

    export let viewData;

    if (!viewData) {
        viewData = {
            type: 'clothes',
            menuList: '',
            priceType: 0,
            priceList: '{"Masks":[{"DrawableId":36,"Textures":[36]},{"DrawableId":46,"Textures":[46]},{"DrawableId":175,"Textures":[175]}],"Undershirts":[{"DrawableId":31,"Textures":[31]},{"DrawableId":32,"Textures":[32]},{"DrawableId":33,"Textures":[33,1]},{"DrawableId":34,"Textures":[34,1]},{"DrawableId":69,"Textures":[69,1]}],"Shoes":[{"DrawableId":1,"Textures":[1,1,2,13,14,15]},{"DrawableId":7,"Textures":[7,1,2]},{"DrawableId":9,"Textures":[9,1,2]},{"DrawableId":21,"Textures":[21,1,2,3,4,5,6,7,8,9,10,11]},{"DrawableId":24,"Textures":[24]},{"DrawableId":25,"Textures":[25]},{"DrawableId":36,"Textures":[36,1,2,3]},{"DrawableId":71,"Textures":[71,3,4]},{"DrawableId":73,"Textures":[73]}],"Legs":[{"DrawableId":10,"Textures":[10,1,2]},{"DrawableId":20,"Textures":[20]},{"DrawableId":24,"Textures":[24,1,5]},{"DrawableId":25,"Textures":[25,1,5]},{"DrawableId":28,"Textures":[28,1,3,6,8,10,14,15]},{"DrawableId":52,"Textures":[52]},{"DrawableId":129,"Textures":[129]}],"Accessories":[{"DrawableId":10,"Textures":[10,1,2]},{"DrawableId":11,"Textures":[11]},{"DrawableId":12,"Textures":[12,1,2]},{"DrawableId":36,"Textures":[36]},{"DrawableId":115,"Textures":[115,1]},{"DrawableId":127,"Textures":[127]},{"DrawableId":126,"Textures":[126]}],"Tops":[{"DrawableId":29,"Textures":[29,5,7]},{"DrawableId":31,"Textures":[31,5,7]},{"DrawableId":234,"Textures":[234]},{"DrawableId":337,"Textures":[337,5]},{"DrawableId":348,"Textures":[348,2,5,8,10,12]},{"DrawableId":349,"Textures":[349,2,5,8,10,12]}],"Decals":[{"DrawableId":57,"Textures":[57]},{"DrawableId":58,"Textures":[58,1]}],"Hat":[{"DrawableId":122,"Textures":[122,1]}],"Glasses":[{"DrawableId":18,"Textures":[18,2,3,5,6,7,9,10]},{"DrawableId":5,"Textures":[5,1,2,3,4,5,6,7,8,9,10]},{"DrawableId":25,"Textures":[25]},{"DrawableId":26,"Textures":[26]}]}',
            gender: false
        };
    }

    const gender = viewData.gender ? "Male" : "Female";
 
    let menuData = [];

    let selectMenu = {};

    let isFraction = viewData.priceType === 2;

    let isLoad = false;

    const onSelectMenu = data => {
        if (selectMenu == data)
            return;
        
        selectDictionary = false;
        isLoad = true;
        selectMenu = data;
        selectSort = 0;

        switch (viewData.type) {
            case "clothes":
                selectTexture = 0;
                executeClient ('client.clothes.getDictionary', getDictionary (selectMenu.dictionary, getClothesDictionary (gender, selectMenu.dictionary)));
                executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
                
                break;
            case "barber":
                selectColor = 0;
                selectColorHighlight = 0;
                selectOpacity = 100;
                executeClient ('client.clothes.getDictionary', getDictionary (selectMenu.dictionary, getBarberDictionary (gender, selectMenu.dictionary)));
                executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
                break;
            case "tattoo":
                executeClient ('client.clothes.getDictionary', getDictionary (selectMenu.dictionary, getTattooDictionary (selectMenu.dictionary)));
                executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
                break;
        }
    }

    const OnOpen = (type, menuList) => {
        if (menuList)
            menuList = JSON.parse (menuList);
        else
            menuList = false;

        let newMenu = [];
        menu.forEach(data => {
            if (data.type == type && (!menuList || menuList.includes (data.dictionary)) && (!data.gender || data.gender === gender)) {
                newMenu.push (data)
            }
        });

        menuData = newMenu;
        onSelectMenu (menuData [0]);
    }


    const getDictionary = (dictionary, clothesData) => {

        //

        clothesData = JSON.parse (clothesData);

        const priceList = JSON.parse (viewData.priceList);
        const priceType = viewData.priceType;


        let returnData = {};

        if (priceType === 2) {
            if (!["Tops", "Legs", "Shoes"].includes(dictionary))
                returnData [-1] = {"Id": -1, "Variation": 0, "Name": "Пусто", "Textures": [0]}

            if (dictionary === "Undershort")
                returnData [-1] = {"Id": -1, "Variation": 0, "Name": "Пусто", "Textures": [0]}

            if (dictionary === "Tops" && priceList && priceList && priceList ["Undershort"])
                returnData [-1] = {"Id": -1, "Variation": 0, "Name": "Пусто", "Textures": [0]}
        }

        if (priceList && clothesData && priceList [dictionary]) {
            priceList [dictionary].forEach((data) => {
                if (clothesData [data[0]]) {
                    returnData [data[0]] = clothesData [data[0]];

                    if (priceType === 0)
                        returnData [data[0]].Price = Number (data[1]);
                    else if (priceType === 1)
                        returnData [data[0]].Donate = Number (data[1]);
                    else if (priceType === 2)
                        returnData [data[0]].Textures = data[1].sort((a, b) => a - b);
                }
            })
        }

        return JSON.stringify (returnData);
    }

    import { onMount, onDestroy } from 'svelte'
    
    onDestroy(() => {
        window.events.removeEvent("cef.clothes.updateDictionary", UpdateDictionary);
        window.events.removeEvent("cef.clothes.setName", setName);
        window.events.removeEvent("cef.clothes.getTorso", GetTorso);
        window.events.removeEvent("cef.clothes.getColor", GetColor);
        window.events.removeEvent("cef.clothes.getTop", GetTop);
    });

    onMount(() => {
        OnOpen (viewData.type, viewData.menuList);
    });

    //

    let selectDictionary = false;
    let selectTexture = 0;
    let dictionaryData = [];  
    let textureSort = [];
    let selectSort = 0;
    
    let torso = clothesEmpty[gender][3];  
    let torsos = {};
    let torsosTexture = 0;

    const UpdateDictionary = (json) => {
        if (refCategory)
            refCategory.scrollTop = 0;
            
        dictionaryData = JSON.parse (json);
        OnSelectDictionary (dictionaryData[0]);
        OnSettingConditions ();

        window.loaderData.delay ("clothes.OnBuy", 1.5, false)
        isLoad = false;
        window.loaderData.delay ("clothes.OnBuy", 1.5, false)
    }

    window.events.addEvent("cef.clothes.updateDictionary", UpdateDictionary);

    const setName = (name) => {
        for(let i = 0; i < dictionaryData.length; i++) {
            if (dictionaryData [i] === selectDictionary) {
                dictionaryData [i].descName = name;
                break;
            }
        }
    }

    window.events.addEvent("cef.clothes.setName", setName);


    let refCategory;

    const OnSelectDictionary = data => {
        if (selectDictionary == data)
            return;

        selectDictionary = data;

        switch (viewData.type) {
            case "clothes":
                selectSort = 0;
                if (selectDictionary.Textures) {
                    
                    textureSort = selectDictionary.Textures.slice (0, length);
                    OnSelectClothes (selectDictionary.Textures [0]);
                }
                break;
            case "barber":
                selectSort = 1;
                if (selectMenu.dictionary == "Hair") {
                    OnSelectHair ();
                    OnSelectColor (selectColor);
                }
                else if (selectMenu.dictionary == "Eyes")
                    OnSelectEyes ();
                else {
                    OnSelectOverlay ();
                    OnSelectColor (selectColor);
                }
                break;
            case "tattoo":
                OnSetDecoration ();
                break;
        }        
    }

    const OnSelectClothes = (index) => {
        selectSort = 0;
        selectTexture = index;
        executeClient ('client.shop.getIndexToTextureName', selectDictionary.Name, selectDictionary.TName, selectTexture, selectDictionary.Id);

        const func = selectMenu.function;
        if (func && func[0] && func[0].event) {
            if (selectMenu.dictionary === "Torsos") {
                if (selectDictionary.Torsos [torso]) {
                    executeClient ('client.clothes.setComponentVariation', 
                                                    3, 
                                                    selectDictionary.Torsos [torso], 
                                                    selectTexture);
                }
            } else {
                let variation = selectDictionary.Variation;

                if (selectDictionary.Id == -1) {
                    if (func[0].event === "setComponentVariation") 
                        variation = clothesEmpty[gender][func[0].componentId];
                    else 
                        variation = -1;
                }

                executeClient ('client.clothes.' + func[0].event, 
                                                    func[0].componentId, 
                                                    variation, 
                                                    selectTexture);
            }
        }

        OnInitConditions ();
    }


    const OnSelectHair = () => {
        const func = selectMenu.function;
        if (func && func[0] && func[0].event)
            executeClient ('client.clothes.setComponentVariation', 
                                                func[0].componentId, 
                                                selectDictionary.Variation, 
                                                0);
        OnInitConditions ();
    }

    const OnSelectEyes = () => {

        const func = selectMenu.function;
        if (func && func[0] && func[0].event)
            executeClient ('client.clothes.setEyeColor', selectDictionary.Variation);

        OnInitConditions ();
    }

    const OnSelectOverlay = () => {
        const func = selectMenu.function;
        if (func && func[0] && func[0].event) 
            executeClient ('client.clothes.setHeadOverlay', 
                                                func[0].overlayID, 
                                                selectDictionary.Variation, 
                                                selectOpacity);

        OnInitConditions ();
    }


    const OnSetDecoration = () => {

        if (gender == "Male") 
            executeClient ('client.clothes.setDecoration', selectMenu.tattooId, JSON.stringify (selectDictionary.Slots), selectDictionary.Dictionary, selectDictionary.MaleHash);
        else 
            executeClient ('client.clothes.setDecoration', selectMenu.tattooId, JSON.stringify (selectDictionary.Slots), selectDictionary.Dictionary, selectDictionary.FemaleHash);

        OnInitConditions ();
    }

    let colorsData = [];
    let colorsDataSort = [];
    let selectColor = 0;
    let colorsHighlightData = [];
    let colorsHighlightDataSort = [];
    let selectColorHighlight = 0;  
    let selectOpacity = 100;  

    const OnSelectColor = (colorId) => {
        selectSort = 1;

        selectColor = colorId;
        const func = selectMenu.function;
        if (func && func[1] && func[1].event) {
            
            if (selectMenu.dictionary == "Hair")
                executeClient ('client.clothes.setHairColor', 
                                                    selectColor, 
                                                    selectColorHighlight);
            else if (func[1].overlayID)
                executeClient ('client.clothes.setHeadOverlayColor', 
                                                    func[1].overlayID, 
                                                    func[1].colorType,
                                                    selectColor);
        }
        OnInitConditions ();
    }

    const OnSelectColorHighlight = (colorId) => {
        
        selectSort = 2;
        selectColorHighlight = colorId;
        const func = selectMenu.function;
        if (func && func[1] && func[1].event) {
            
            if (selectMenu.dictionary == "Hair")
                executeClient ('client.clothes.setHairColor', 
                                                    selectColor, 
                                                    selectColorHighlight);
        }
        OnInitConditions ();
    }


    const OnSelectOpacity = (opacity) => {
        selectOpacity = opacity;  
        const func = selectMenu.function;
        if (func && func[0] && func[0].event) {
            executeClient ('client.clothes.setHeadOverlay', 
                                                func[0].overlayID, 
                                                selectDictionary.Variation, 
                                                selectOpacity);
                                            
            executeClient ('client.clothes.setHeadOverlayColor', 
                                                func[1].overlayID, 
                                                func[1].colorType,
                                                selectColor);
        }
        OnInitConditions ();
    }

    const getName = (name) => {
        if(typeof name == "number") 
        {
            if (clothesName [`${selectMenu.dictionary}_${name}_${gender}`]) name = clothesName [`${selectMenu.dictionary}_${name}_${gender}`];
            else if (clothesName [`${selectMenu.dictionary}_${name}`]) name = clothesName [`${selectMenu.dictionary}_${name}`];
            else name = `#${name}`;
        }
        return name;
    }

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }

    //

    const OnSettingConditions = () => {
        if (["Body", "Torso", "LeftArm", "RightArm"].includes (selectMenu.dictionary) || (selectDictionary && selectDictionary.Torso != undefined)) {//Если передеваем верх            
            executeClient ('client.clothes.getTorso');
        }

        if (selectDictionary && selectDictionary.Torsos != undefined) {//Если передеваем верх            
            executeClient ('client.clothes.getTop');
        }
        
        if (selectMenu.color != undefined) {
            executeClient ('client.clothes.getColor', selectMenu.isHair);
        }        
    }

    const GetTorso = (drawable, texture) => {
        torsos = {};
        torsosTexture = texture;
        const defaultTorsos = [0, 1, 2, 4, 5, 6, 8, 11, 12, 14, 15, 112, 113, 114];
        if (!defaultTorsos.includes (drawable)) {//15 дефолтный пустой торс
            const torsosData = JSON.parse (getClothesDictionary (gender, "Torsos"));
            Object.values (torsosData).forEach((data) => {
                if (data && data.Torsos && Object.values (data.Torsos)) {
                    Object.values (data.Torsos).forEach((torso) => {
                        if (torso === drawable) {
                            torsos = data.Torsos;
                            //torsosTexture = texture;
                        }
                    })
                }
            })
        }
        OnInitConditions ();
    }

    window.events.addEvent("cef.clothes.getTorso", GetTorso);

    const GetTop = (drawable) => {
        torso = clothesEmpty[gender][3];
        if (torso != drawable) {
            let isSuccess = false;
            const topsData = JSON.parse (getClothesDictionary (gender, "Tops"));
            Object.values (topsData).forEach((data) => {
                if (data && data.Variation == drawable) {
                    torso = data.Torso;
                    isSuccess = true;
                }
            });
            //
            if (!isSuccess) {
                const undershortData = JSON.parse (getClothesDictionary (gender, "Undershort"));
                Object.values (undershortData).forEach((data) => {
                    if (data && data.Variation == drawable) {
                        torso = data.Torso;
                    }
                })
            }
        }
        

        OnInitConditions ();
    }

    window.events.addEvent("cef.clothes.getTop", GetTop);

    const OnInitConditions = () => {
        if (selectDictionary && selectDictionary.Torso != undefined) {//Если передеваем верх

            executeClient ('client.clothes.setComponentVariation', 8, clothesEmpty[gender][8], 0, false);
            
            if (torsos [selectDictionary.Torso])
                executeClient ('client.clothes.setComponentVariation', 3, torsos [selectDictionary.Torso], torsosTexture, false);
            else
                executeClient ('client.clothes.setComponentVariation', 3, selectDictionary.Torso, torsosTexture, false);
        }

        if (selectDictionary.IsHair != undefined) {
            executeClient ('client.clothes.setComponentVariation', 2, 0, 0, false);
        }

        if (selectDictionary.IsHat != undefined) {
            executeClient ('client.clothes.clearProp', 0);
        }

        if (selectDictionary.IsGlasses != undefined) {
            executeClient ('client.clothes.clearProp', 1);
            executeClient ('client.clothes.clearProp', 2);
        }
        
        if (["Masks"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.clearMask');
        }

        if (["Hat", "Glasses", "Ears"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 1, clothesEmpty[gender][1], 0, false);
        }
        
        //Барбер
        if (["Hair", "Beard", "Eyebrows", "Eyes", "Lips", "Palette", "Makeup"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 1, clothesEmpty[gender][1], 0, false);
            executeClient ('client.clothes.clearProp', 0);
        }
        
        if (["Eyebrows", "Eyes", "Makeup"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.clearProp', 1);
        }

        if (["Body"].includes (selectMenu.dictionary)) {            
            if (torsos [clothesEmpty[gender][3]])
                executeClient ('client.clothes.setComponentVariation', 3, torsos [clothesEmpty[gender][3]], torsosTexture, false);
            else
                executeClient ('client.clothes.setComponentVariation', 3, clothesEmpty[gender][3], torsosTexture, false);

            executeClient ('client.clothes.setComponentVariation', 8, clothesEmpty[gender][8], 0, false);
            executeClient ('client.clothes.setComponentVariation', 11, clothesEmpty[gender][11], 0, false);
        }
        //Тату
        if (["Head"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 1, clothesEmpty[gender][1], 0, false);
            executeClient ('client.clothes.clearProp', 0);
        }

        if (["Torso", "LeftArm", "RightArm"].includes (selectMenu.dictionary)) {            
            if (torsos [clothesEmpty[gender][3]])
                executeClient ('client.clothes.setComponentVariation', 3, torsos [clothesEmpty[gender][3]], torsosTexture, false);
            else
                executeClient ('client.clothes.setComponentVariation', 3, clothesEmpty[gender][3], torsosTexture, false);

            executeClient ('client.clothes.setComponentVariation', 8, clothesEmpty[gender][8], 0, false);
            executeClient ('client.clothes.setComponentVariation', 11, clothesEmpty[gender][11], 0, false);
        }

        if (["LeftLeg", "RightLeg"].includes (selectMenu.dictionary)) {
            executeClient ('client.clothes.setComponentVariation', 4, clothesEmpty[gender][4], 0, false);
        }
    }

    const length = 8;

    //let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)
    const SplitColorsArray = (select, array,  sortArray) => {
        //.findIndex(a => a == anim);

        let index = array.findIndex(a => a == sortArray [0]);
        if (index != -1 && (index - 1) === select) {
            return array.slice ((index - 1), (index - 1) + length);
        } 
        
        index = array.findIndex(a => a == sortArray [length - 1]);
        if (index != -1 && (index + 1) === select) {
            index = array.findIndex(a => a == sortArray [0]);
            if (index != -1)
                return array.slice ((index + 1), (index + 1) + length);
        }
        return -1;
    }


    /*const SplitColorsArray = (select) => {
        //.findIndex(a => a == anim);

        let gtaid = colorsDataSort[0].gtaid - 1;
        if (gtaid === select) {
            colorsDataSort = colorsData.slice (gtaid, gtaid + length);
            return;
        } 
        
        gtaid = colorsDataSort[length - 1].gtaid + 1;
        if (gtaid === select) {
            gtaid = colorsDataSort[0].gtaid + 1;
            colorsDataSort = colorsData.slice (gtaid, gtaid + length);
        }
    }*/

    const GetColor = (json) => {
        colorsData = JSON.parse (json);

        colorsDataSort = colorsData.slice (0, length);

        if (selectMenu.colorHighlight) {
            colorsHighlightData = colorsData;
            colorsHighlightDataSort = colorsDataSort;
        }
    }

    window.events.addEvent("cef.clothes.getColor", GetColor);

    const handleKeyDown = (event) => {
        const { keyCode } = event;

        if (keyCode != 13) 
            window.loaderData.delay ("clothes.OnBuy", 1.5, false)

        switch (keyCode) {
            case 69: {
                if (!menuData)
                    return;

                let index = menuData.findIndex(a => a == selectMenu);
                if(menuData [index + 1] === undefined) 
                    return;
                
                index++;
                onSelectMenu (menuData [index])
                break;
            }
            case 81: {
                if (!menuData)
                    return;

                let index = menuData.findIndex(a => a == selectMenu);
                if(menuData [index - 1] === undefined) 
                    return;
                
                index--;
                onSelectMenu (menuData [index])
                break;
            }
            //
            case 38: {
                if (!dictionaryData)
                    return;

                let index = dictionaryData.findIndex(a => a == selectDictionary);
                if(dictionaryData [index - 1] === undefined) 
                    return;
                
                index--;
                OnSelectDictionary (dictionaryData [index]);

                const el = document.querySelector(`#menu_${index}`);
                const bounds = el.getBoundingClientRect();
                refCategory.scrollTop = (bounds.height * index) + ((bounds.height / 10) * index);
                break;
            }
            //
            case 40: {
                if (!dictionaryData)
                    return;

                let index = dictionaryData.findIndex(a => a == selectDictionary);
                if(dictionaryData [index + 1] === undefined) 
                    return;
                
                index++;
                OnSelectDictionary (dictionaryData [index]);

                const el = document.querySelector(`#menu_${index}`);
                const bounds = el.getBoundingClientRect();
                refCategory.scrollTop = (bounds.height * index) + ((bounds.height / 10) * index);
                break;
            }
            //
            case 37:
                switch (selectSort) {
                    case 0:
                        if (!selectDictionary.Textures)
                            return;

                        let index = selectDictionary.Textures.findIndex(a => a == selectTexture);
                        if(selectDictionary.Textures [index - 1] === undefined) 
                            return;
                        
                        OnSelectClothes (selectDictionary.Textures [index - 1]);

                        let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)

                        if (returnSort != -1)
                            textureSort = returnSort;
                        break;
                    case 1:
                        if(--selectColor < 0) 
                            selectColor = 0;
                        else {
                            OnSelectColor (selectColor);
                            let returnSort = SplitColorsArray (selectColor, colorsDataSort, colorsData);
                            if (returnSort != -1)
                                colorsDataSort = returnSort;
                        }
                        break;
                    case 2:
                        if(--selectColorHighlight < 0) 
                            selectColorHighlight = 0;
                        else {
                            OnSelectColorHighlight (selectColorHighlight);
                            let returnSort = SplitColorsArray (selectColorHighlight, colorsHighlightDataSort, colorsHighlightData);
                            if (returnSort != -1)
                                colorsHighlightDataSort = returnSort;
                        }
                        break;
                }
                break;
            case 39:
                switch (selectSort) {
                    case 0:
                        if (selectDictionary.Textures === undefined)
                            return;

                        let index = selectDictionary.Textures.findIndex(a => a == selectTexture);
                        if(selectDictionary.Textures [index + 1] === undefined) 
                            return;
                        
                        OnSelectClothes (selectDictionary.Textures [index + 1]);

                        let returnSort = SplitColorsArray (selectTexture, selectDictionary.Textures, textureSort)

                        if (returnSort != -1)
                            textureSort = returnSort;
                        break;
                    case 1:
                        if(++selectColor > colorsData.length - 1) 
                            selectColor = colorsData.length - 1;
                        else {
                            OnSelectColor (selectColor);
                            let returnSort = SplitColorsArray (selectColor, colorsDataSort, colorsData);
                            if (returnSort != -1)
                                colorsDataSort = returnSort;
                        }
                        break;
                    case 2:
                        if(++selectColorHighlight > colorsHighlightData.length - 1) 
                            selectColorHighlight = colorsHighlightData.length - 1;
                        else {
                            OnSelectColorHighlight (selectColorHighlight);
                            let returnSort = SplitColorsArray (selectColorHighlight, colorsHighlightDataSort, colorsHighlightData);
                            if (returnSort != -1)
                                colorsHighlightDataSort = returnSort;
                        }
                        break;
                }
                break;
            case 13:
                OnBuy ()
                break;

        }
    }

    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch (keyCode) {
            case 27:
                OnExit ()
                break;

        }
    }
    const OnExit = () => {
        executeClient ('client.shop.close');
    }

    const OnBuy = () => {
        if (!selectDictionary)
            return;

        if (isLoad)
            return;

        if (!window.loaderData.delay ("clothes.OnBuy", 1.5))
            return;

        switch (viewData.type) {
            case "clothes":
                if (!isFraction)
                    executeClient (`client.clothes.buy`, selectMenu.dictionary, selectDictionary.Id, selectTexture);
                else
                    executeClient (`client.table.editClothingSet`, selectMenu.dictionary, selectDictionary.Id, selectTexture);
                break;
            case "barber":
                executeClient (`client.barber.buy`, selectMenu.dictionary, selectDictionary.Id, selectColor, selectColorHighlight, selectOpacity);
                break;                
            case "tattoo":
                executeClient (`client.tattoo.buy`, selectMenu.dictionary, selectDictionary.Id);                
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} on:keydown={handleKeyDown} />

<div id="newbarbershop">
    <div class="box-date">
        <div class="box-time">
            <div class="time">{TimeFormat ($serverDateTime, "H:mm")}</div>
            {TimeFormat ($serverDateTime, "DD.MM.YYYY")}
        </div>
    </div>
    <div class="newbarbershop__top">
        <div class="newbarbershop__top_header" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
            <div class="newbarbershop__button">
                Q
            </div>
            {#each menuData as data, index}
                <div class="newbarbershop__category" class:active={selectMenu == data} on:click={() => onSelectMenu (data)}>
                    <div class="{data.icon} newbarbershop__category_icon"></div>
                </div>
            {/each}
            <div class="newbarbershop__button">
                E
            </div>
        </div>
        <div class="newbarbershop__text">
            {translateText('business', 'Выбор категории')}
        </div>
    </div>
    <div class="newbarbershop__center">
        <div class="newbarbershop__menu">
            <div class="box-flex">
                <div class="newbarbershop__button">
                    <span class="newbarbershopicons-updown"></span>
                </div>
                <div>{translateText('business', 'Выбор')} {selectMenu.title}</div>
            </div>
            <div class="newbarbershop__menu_info">{translateText('business', 'В ассортименте')} {dictionaryData.length} {translateText('business', 'шт.')}.</div>
            <div class="newbarbershop__menu_list" bind:this={refCategory} on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                {#each dictionaryData as data, index}
                    <div class="newbarbershop__menu_element" id="menu_{index}" class:active={data === selectDictionary} on:click={() => OnSelectDictionary (data)}>
                        <div class="newbarbershop__menu_element-absolute"></div>
                        <div class="newbarbershop__menu_name">{@html getName(data.descName)}</div>
                        {#if data.Donate > 0 && !isFraction}
                        <div class="newbarbershop__menu_price">{format("money", data.Donate)} RB</div>
                        {:else if data.Price > 0 && !isFraction}
                        <div class="newbarbershop__menu_price">$ {format("money", data.Price)}</div>
                        {/if}
                    </div>
                {/each}
            </div>
            {#if (selectDictionary.Textures && selectDictionary.Textures.length > 0)}
                <div class="newbarbershop__menu_info box-flex" style="opacity: {selectSort == 0 ? 1 : 0.5};">
                    <div class="newbarbershop__button">
                        <span class="newbarbershopicons-leftright"></span>
                    </div>
                    {translateText('business', 'Вариаций')}: {selectDictionary.Textures.length} {translateText('business', 'шт')}.
                </div>
                <div class="newbarbershop__menu_colors textures"  on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                    {#each textureSort as index, _}
                    <div class="newbarbershop__menu_color" 
                        class:active={selectTexture === index} 
                        id="texture_{index}" 
                        on:click={() => OnSelectClothes (index)}>{index}</div>                        
                    {/each}
                </div>
            {/if}
            {#if selectMenu.color && colorsData.length > 0}
                <div class="newbarbershop__menu_info box-flex box-flex" style="opacity: {selectSort == 1 ? 1 : 0.5};">
                    <div class="newbarbershop__button">
                        <span class="newbarbershopicons-leftright"></span>
                    </div>
                    {translateText('business', 'На выбор')} {colorsData.length} {translateText('business', 'цвета')}
                </div>
                <div class="newbarbershop__menu_colors" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                    {#each colorsDataSort as data, index}
                    <div class="newbarbershop__menu_color" 
                        style="background: rgba({data.r}, {data.g}, {data.b}, {data.a});"
                        class:active={selectColor === data.gtaid} 
                        id="colors_{index}" 
                        on:click={() => OnSelectColor (data.gtaid)}>{data.gtaid}</div> 
                    {/each}
                </div>
            {/if}
            {#if selectMenu.colorHighlight && colorsHighlightData.length > 0}
                <div class="newbarbershop__menu_info box-flex" style="opacity: {selectSort == 2 ? 1 : 0.5};">
                    <div class="newbarbershop__button">
                        <span class="newbarbershopicons-leftright"></span>
                    </div>
                    {translateText('business', 'Второй цвет - тоже')} {colorsHighlightData.length} {translateText('business', 'шт')}.
                </div>
                <div class="newbarbershop__menu_colors" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                    {#each colorsHighlightDataSort as data, index}
                    <div class="newbarbershop__menu_color" 
                        style="background: rgba({data.r}, {data.g}, {data.b}, {data.a});"
                        class:active={selectColorHighlight === data.gtaid}
                        id="colorsHighlight_{index}"
                        on:click={() => OnSelectColorHighlight (data.gtaid)}>{data.gtaid}</div> 
                    {/each}
                </div>
            {/if}
            {#if selectMenu.opacity}
                <InputBlock
                    id="selectOpacity"
                    leftText="0"
                    centerText="Насыщенность"
                    rightText="100"
                    step={0.1}
                    min={0}
                    max={1}
                    value={selectOpacity}
                    callback={newvalue => OnSelectOpacity (newvalue)} />
            {/if}
        </div>
        <!--
            Нет функционал дальше
        -->
        <div class="newbarbershop__help">
            <div class="newbarbershop__help_img"></div>
            <div class="newbarbershop__help_main">
                <div class="newbarbershop__help_text">{translateText('business', 'Используйте горячие клавиши для быстрого перемещения по интерфейсу')}:</div>
                <div class="box-between">
                    <div class="newbarbershop__button">
                        <span class="newbarbershopicons-leftright"></span>
                    </div>
                    <div class="newbarbershop__button">
                        <div class="newbarbershop__button">
                            <span class="newbarbershopicons-updown"></span>
                        </div>
                    </div>
                    <div class="newbarbershop__button">
                        Q
                    </div>
                    <div class="newbarbershop__button">
                        E
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="newbarbershop__bottom">
        <div class="box-flex">
            <div class="newbarbershop__button">Enter</div>
            <div on:click={OnBuy}>{!isFraction ? "Купить" : "Установить"}</div>
        </div>
        <div class="box-flex">
            <div>{translateText('business', 'Выйти')}</div>
            <div class="newbarbershop__button" on:click={OnExit}>ESC</div>
        </div>
    </div>
</div>