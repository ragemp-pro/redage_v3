<script>
    import { translateText } from 'lang'
    import './main.sass' 
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { charGender } from 'store/chars'
    import { menu, getClothesDictionary, clothesEmpty } from 'json/clothes.js'
    import { clothesName } from '@/views/player/menu/elements/inventory/functions.js';

    export let viewData;

    if (!viewData)
        viewData = "{}";

    viewData = JSON.parse (viewData);
    
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }
        
    const gender = Bool($charGender) ? "Male" : "Female";
 
    let menuData = [];

    let selectMenu = {};

    const getDictionary = (dictionary) => {
        const clothesData = JSON.parse (getClothesDictionary (gender, dictionary))
        if (viewData && clothesData && viewData [dictionary]) {
            
        }
    }

    const onSelectMenu = data => {
        if (selectMenu == data)
            return;

        selectMenu = data;

        selectTexture = 0;
        executeClient ('client.clothes.getDictionary', getClothesDictionary (selectMenu.dictionary));
        executeClient ('client.clothes.updateCameraToBone', selectMenu.camera);
    }

    const isCount = (dictionary, isDonate) => {
        let count = false;
        let data = [];

        data = JSON.parse (getClothesDictionary (gender, dictionary));
        data = Object.values (data);

        let item;
        for(let i = 0; i < data.length; i++) {
            item = data [i];
            if (item && ((!isDonate && item.Price > 0) || (isDonate && item.Donate > 0))) {
                count = true;
                break;
            }
        }

        return count;
    }

    const OnOpen = (type, json, isDonate) => {
        if (json)
            json = JSON.parse (json);
        else 
            json = false;

        let newMenu = [];
        menu.forEach(data => {
            if (data.type == type && (!json || json.includes (data.dictionary)) && isCount (data.dictionary, isDonate) && (!data.gender || data.gender === gender)) {
                newMenu.push (data)
            }
        });

        menuData = newMenu;
        onSelectMenu (menuData [0]);
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
        OnOpen (viewData.json, viewData.isDonate);
    });

    //

    let selectDictionary = false;
    let selectTexture = 0;
    let dictionaryData = [];  
    
    let torso = clothesEmpty[gender][3];  
    let torsos = {};
    let torsosTexture = 0;

    const UpdateDictionary = (json) => {
        if (refCategory)
            refCategory.scrollTop = 0;
        dictionaryData = JSON.parse (json);
        OnSelectDictionary (dictionaryData[0]);
        OnSettingConditions ();
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

        if (selectDictionary.Textures) {
            
            textureSort = selectDictionary.Textures.slice (0, length);
            OnSelectClothes (selectDictionary.Textures [0]);
        }   
    }

    const OnSelectClothes = (index) => {
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
            } else 
                executeClient ('client.clothes.' + func[0].event, 
                                                    func[0].componentId, 
                                                    selectDictionary.Variation, 
                                                    selectTexture);
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
    }

    const length = 8;


    const handleKeyDown = (event) => {
        const { keyCode } = event;
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
        executeClient (`client.clothes.buy`, selectMenu.dictionary, selectDictionary.Id, selectTexture);
    }
</script>

<svelte:window on:keyup={handleKeyUp} on:keydown={handleKeyDown} />

<div id="newbarbershop">
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
            {translateText('fractions', 'Выбор категории')}
        </div>
    </div>
    <div class="newbarbershop__center">
        <div class="newbarbershop__menu">
            <div class="box-flex">
                <div class="newbarbershop__button">
                    <span class="newbarbershopicons-updown"></span>
                </div>
                <div>{translateText('fractions', 'Выбор')} {selectMenu.title}</div>
            </div>
            <div class="newbarbershop__menu_info">{translateText('fractions', 'В ассортименте')} {dictionaryData.length} {translateText('fractions', 'шт')}.</div>
            <div class="newbarbershop__menu_list" bind:this={refCategory} on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
                {#each dictionaryData as data, index}
                    <div class="newbarbershop__menu_element" id="menu_{index}" class:active={data === selectDictionary} on:click={() => OnSelectDictionary (data)}>
                        <div class="newbarbershop__menu_element-absolute"></div>
                        <div class="newbarbershop__menu_name">{@html getName(data.descName)}</div>
                        {#if data.Donate > 0}
                        <div class="newbarbershop__menu_price">{format("money", data.Donate)} RB</div>
                        {:else}
                        <div class="newbarbershop__menu_price">$ {format("money", data.Price)}</div>
                        {/if}
                    </div>
                {/each}
            </div>
        </div>
        <!--
            Нет функционал дальше
        -->
        <div class="newbarbershop__help">
            <div class="newbarbershop__help_img"></div>
            <div class="newbarbershop__help_main">
                <div class="newbarbershop__help_text">{translateText('fractions', 'Используйте горячие клавиши для быстрого перемещения по интерфейсу')}:</div>
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
            <div on:click={OnBuy}>{translateText('fractions', 'Купить')}</div>
        </div>
        <div class="box-flex">
            <div>{translateText('fractions', 'Выйти')}</div>
            <div class="newbarbershop__button" on:click={OnExit}>ESC</div>
        </div>
    </div>
</div>