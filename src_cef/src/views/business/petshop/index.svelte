<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'

    import '../../assets/sass/store.sass';

    let dunamicData = {
        indexM: 0,
        indexC: 0,
        models: [],
        hashes: [],
        prices: [],
        activeItem: 0,
        header: "Магазин питомцев"
    }


    window.petshop = {
        setVariable: (type, jsonstr) => {
            dunamicData[type] = JSON.parse(jsonstr);
        }       
    }

    const left = (type) => {
        dunamicData.indexM--;
        if(dunamicData.indexM < 0) dunamicData.indexM = dunamicData.models.length - 1;
            executeClient ('petshop','model', dunamicData.indexM);
    }

    const right = (type) => {
        dunamicData.indexM++
        if(dunamicData.indexM == dunamicData.models.length) dunamicData.indexM = 0;
        executeClient ('petshop','model',dunamicData.indexM);
    }


    const buy = () => {
        executeClient ('buyPet');
    }

    const exit = () => {
        executeClient ('closePetshop');
    }

    const setItem = (id, event) => {
        dunamicData.activeItem = id;
        executeClient ('petshop','model', id)
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        executeClient ('closePetshop');
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div id="store">
    <div class="box-content">
        <div class="box-ch">
            <div class="box-info">
            <span class="ic-st-paw"/>
                <div class="l">
                    <div class="title">{translateText('business', 'Магазин питомцев')}</div>
                </div>
                
            </div>
            <div class="box-max" on:mouseenter={() => executeClient ("client.camera.toggled", false)} on:mouseleave={() => executeClient ("client.camera.toggled", true)}>
                <div class="box-list">
                    <div class="label"><span class="ic-st-sliders-4"/>{translateText('business', 'Выберите питомца')}:</div>

                        <ul class="list">

                            {#each dunamicData.models as value, index}
                            <li
                            key={index}
                            on:click={() => setItem(index)}
                            class={`${dunamicData.activeItem !== index || "active"}`}>
                                <div class="t">{@html value}</div>
                                <div class='price'>{format("money", dunamicData.prices[index])}$</div>
                             </li>
                            {/each}
                        </ul>
                </div>
            </div>
            <div class="bottom-btn" on:mouseenter={() => executeClient ("client.camera.toggled", false)} on:mouseleave={() => executeClient ("client.camera.toggled", true)}> 
                <div class="btn green" on:click={() => buy()} >{translateText('business', 'Купить')}</div>
                <div class="btn red" on:click={() => exit()}>{translateText('business', 'Выйти')}</div>
            </div>
        </div>

        <div class="box-info-button">
            <div class="bottom-btn">
                <div class="btn-group">
                    <div class="redcircle"><span class='autoshop-mouse'></span></div>
                </div>
                <div class="text">{translateText('business', 'ЛКМ - осмотреть')}</div>
            </div>

        </div>
    
    </div>
</div>