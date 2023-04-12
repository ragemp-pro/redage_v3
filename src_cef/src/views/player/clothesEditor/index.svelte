<script>
    import { translateText } from 'lang'
    import './main.sass';
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { clothesData, clothesDataToName, MaxClothesComponent, UndershortData, UndershortName, TorsosData } from './data.js'
    let state = {
        componentId: 0,
        gender: 'men',
        drawable: 0,
        texture: 0,
        drawable: 0,
        texture: 0,
        torsos: 0,
        undershirts: 0,
        export: false
    };

    const OnLeft = (array, componentId) => {

        state[array]--;
        if (state[array] < 0) {
            if (state == "undershirts") state[array] = UndershortData [state["gender"]].length - 1;
            else if (state == "torsos") state[array] = TorsosData [state["gender"]].length - 1;
            else state[array] = 0;
        }
            

        if (array === "componentId" && !state["componentId"]) {
            state["componentId"] = 0;
            state["drawable"] = 0;
            state["texture"] = 0;
        }
        else if (array === "componentId") {
            state["drawable"] = 0;
            state["texture"] = 0;            
        }
        else if (array === "drawable") state["texture"] = 0;
        
        if (componentId == undefined) executeClient ('client.clothesEditor.update',
            clothesData[state["componentId"]].id,
            clothesData[state["componentId"]].props,
            state["drawable"], state["texture"]);
        else if (array == "undershirts") executeClient ('client.clothesEditor.update',
            clothesDataToName["undershirts"].id,
            clothesDataToName["undershirts"].props,
            UndershortData [state["gender"]][state["undershirts"]], 0);
        else if (array == "torsos") executeClient ('client.clothesEditor.update',
            clothesDataToName["torsos"].id,
            clothesDataToName["torsos"].props,
            TorsosData [state["gender"]][state["torsos"]], 0);
    }

    const OnRight = (array, componentId) => {

        state[array]++;
        if (state == "undershirts" && UndershortData [state["gender"]][state[componentId]] == undefined) state[array] = 0;
        else if (state == "torsos" && TorsosData [state["gender"]][state[componentId]] == undefined) state[array] = 0;

        

        if (array === "componentId" && state["componentId"] == clothesData.length) {
            state["componentId"] = 0;
            state["drawable"] = 0;
            state["texture"] = 0;
        }
        else if (array === "componentId") {
            state["drawable"] = 0;
            state["texture"] = 0;            
        }
        else if (array === "drawable") state["texture"] = 0;
        if (componentId == undefined) executeClient ('client.clothesEditor.update',
            clothesData[state["componentId"]].id,
            clothesData[state["componentId"]].props,
            state["drawable"], state["texture"]);
        else if (array == "undershirts") executeClient ('client.clothesEditor.update',
            clothesDataToName["undershirts"].id,
            clothesDataToName["undershirts"].props,
            UndershortData [state["gender"]][state["undershirts"]], 0);
        else if (array == "torsos") executeClient ('client.clothesEditor.update',
            clothesDataToName["torsos"].id,
            clothesDataToName["torsos"].props,
            TorsosData [state["gender"]][state["torsos"]], 0);
    }

    const onExit = () => {
        if (state.export !== false) return state.export = false
        executeClient ('client.clothesEditor.close')
    }

    const onExport = () => {
        let textures = [];
        for (let i = 0; i <= state.texture; i++) {
            textures.push([
                String(i),
                ""
            ])
        }
        
        if (clothesData [state.componentId].type !== "tops") state.export = `INSERT INTO clothes_${state.gender === 'men' ? 'male' : 'female'}_${clothesData [state.componentId].type}(id, cvariation, textures, category, can_buy, price) VALUES ('${state["drawable"]}', '${state["drawable"] - MaxClothesComponent [state["gender"]][clothesData [state.componentId].type]}', '${JSON.stringify(textures)}', '', '0', '0');`;
        else state.export = `INSERT INTO clothes_${state.gender === 'men' ? 'male' : 'female'}_${clothesData [state.componentId].type}(id, cvariation, torso, textures, type, category, can_buy, price) VALUES ('${state["drawable"]}', '${state["drawable"] - MaxClothesComponent [state["gender"]][clothesData [state.componentId].type]}', '${TorsosData [state["gender"]][state.torsos]}', '${JSON.stringify(textures)}', '${state.undershirts - 1}', '', '0', '0');`;
    
        /*if (MaxClothesComponent [state["gender"]][clothesData [state.componentId].type] >=  state["drawable"]) {
            return;
        }
        if (clothesData [state.componentId].type == "tops")
            state.export += state.undershirts - 1 === -1 ? `/additem -11 1 ${state["drawable"]}_${selectTexture}_${state.gender === 'men' ? 'True' : 'False'}` : `\n/additem -8 1 ${state["drawable"]}_${state.texture}_${state.gender === 'men' ? 'True' : 'False'}`
        else if (clothesData [state.componentId].type == "legs") 
            state.export += `/additem -4 1 ${state["drawable"]}_${state.texture}_${state.gender === 'men' ? 'True' : 'False'}`
        else if (clothesData [state.componentId].type == "shoes") 
            state.export += `/additem -6 1 ${state["drawable"]}_${state.texture}_${state.gender === 'men' ? 'True' : 'False'}`*/
    }
    
    const setGender = (gender) => {
        state.gender = gender;
        executeClient ('client.clothesEditor.gender', gender)
    }
</script>


<div id="clothesEditor">
    <div class="box-content clothesEditor">
        <div class="box-ch">
            <div class="box-max" on:mouseenter={() => executeClient ("client.camera.toggled", false)} on:mouseleave={() => executeClient ("client.camera.toggled", true)}>
                {#if state.export == false}
                <div class="box-list" style="max-height: 100%">
            
                    <div class="switch">
                        <div id="gendermale" class="btn" on:click={() => setGender ('men')}><span class={state.gender === 'men' ? "active" : ""}/>{translateText('player', 'Мужской')}</div>
                        <div id="genderfemale" class="btn" on:click={() => setGender ('women')}><span class={state.gender === 'women' ? "active" : ""}/>{translateText('player', 'Женский')}</div>
                    </div>
                    <div class="label">{translateText('player', 'Компонент')}:</div>

                    <div class="module-list">
                        <div class="l" on:click={() => OnLeft("componentId")}><i class="left-arrow icon-arrow"></i></div>
                        <div>{clothesData [state.componentId].name}</div>
                        <div class="r" on:click={() => OnRight("componentId")}><i class="right-arrow icon-arrow"></i></div>
                    </div>
                    <div class="label" style="margin-top: 20px">Drawable:</div>

                    <div class="module-list">
                        <div class="l" on:click={() => OnLeft("drawable")}><i class="left-arrow icon-arrow"></i></div>
                        <input bind:value={state.drawable} on:input={event => {
                            if (event.target.value.replace(/\D/,'')) executeClient ('client.clothesEditor.update', clothesData[state["componentId"]].id, clothesData[state["componentId"]].props, event.target.value.replace(/\D/,''), state["texture"]);
                        }} />
                        
                        <div class="r" on:click={() => OnRight("drawable")}><i class="right-arrow icon-arrow"></i></div>
                    </div>
                    <div class="label" style="margin-top: 20px">Texture:</div>

                    <div class="module-list">
                        <div class="l" on:click={() => OnLeft("texture")}><i class="left-arrow icon-arrow"></i></div>
                        <input bind:value={state.texture} on:input={event => {                                      
                            if (event.target.value.replace(/\D/,'')) executeClient ('client.clothesEditor.update', clothesData[state["componentId"]].id, clothesData[state["componentId"]].props, state["drawable"], event.target.value.replace(/\D/,''));
                        }} />
                        <div class="r" on:click={() => OnRight("texture")}><i class="right-arrow icon-arrow"></i></div>
                    </div>

                    {#if clothesData[state["componentId"]].type === "tops"}
                        <div class="label" style="margin-top: 20px">Undershirts:</div>

                        <div class="module-list">
                            <div class="l" on:click={() => OnLeft("undershirts", "undershirts")}><i class="left-arrow icon-arrow"></i></div>
                            <div>{UndershortName [state.undershirts]}</div>
                            <div class="r" on:click={() => OnRight("undershirts", "undershirts")}><i class="right-arrow icon-arrow"></i></div>
                        </div>
                        <div class="label" style="margin-top: 20px">Torsos:</div>

                        <div class="module-list">
                            <div class="l" on:click={() => OnLeft("torsos", "torsos")}><i class="left-arrow icon-arrow"></i></div>
                            <input value={TorsosData [state["gender"]][state.torsos]} on:input={event => {
                                const valueTorsos = event.target.value.replace(/\D/,'');                                    
                                if (valueTorsos && TorsosData [state["gender"]][valueTorsos] !== undefined) {
                                    
                                    state.torsos = valueTorsos;
                                    executeClient ('client.clothesEditor.update', clothesDataToName["torsos"].id, clothesDataToName["torsos"].props, TorsosData [state["gender"]][valueTorsos], 0);
                                }
                            }} />
                            <div class="r" on:click={() => OnRight("torsos", "torsos")}><i class="right-arrow icon-arrow"></i></div>
                        </div>
                    {/if}


                </div>
                {:else}
                <div class="box-list">
                    <textarea type="text" class="exportChar" value={state.export} />
                </div>
                {/if}
            </div>
            <div class="bottom-btn"> 
                <div class="btn green" on:click={onExport}>{translateText('player', 'Экспортировать')}</div>
                <div class="btn red" on:click={onExit}>{state.export == false ? translateText('player', 'Выйти') : translateText('player', 'Назад')}</div>
            </div>
        </div>

        <div class="box-info-button">

            <div class="bottom-btn">
                <div class="btn-group">
                    <div class="redcircle"><span class='autoshop-mouse'></span></div>
                </div>
                <div class="text">{translateText('player', 'ЛКМ - осмотреть')}</div>
            </div>

        </div>
    
    </div>
</div>