
<script>
    import { executeClient } from 'api/rage'
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import './css/main.sass';
    import './fonts/style.css';
    import authInfo from './authInfo';
    const authColors =[
        "#000",
        "#fff",
        "#e60000",
        "#ff7300",
        "#f0f000",
        "#00e600",
        "#00cdff",
        "#0000e6",
        "#be3ca5",
    ];
    let list = [];
    let select = 0;
    let selectTime = -1;
    let sordId = -1;
    let colorId = 0;

    let isDonateAutoroom = false;

    window.authShop = {
        data: (value, isDonate = false) => {
            let returnList = [];
            let modelInfo;
            JSON.parse(value).forEach(value => {
                modelInfo = authInfo [value.modelName] || false;
                
                returnList = [
                    ...returnList, {
                        ...value,
                        speed: (!modelInfo || (modelInfo && !modelInfo.maxSpeed)) ? value.speed : modelInfo.maxSpeed,
                        boost: (!modelInfo || (modelInfo && !modelInfo.acceleration)) ? value.boost : modelInfo.acceleration,
                        seat: (!modelInfo || (modelInfo && !modelInfo.seats)) ? value.seat : modelInfo.seats,
                        invslots: (value.invslots !== undefined) ? value.invslots : 25,
                        desc: !modelInfo ? false : modelInfo.desc,
                    }
                ];
            });
            list = returnList;
            isDonateAutoroom = isDonate;
            return;
        }
    }


    const sort = (value) => {
        if (sordId === value) return;
        let sortList = list;
        let sortSelect = -1;
        if (select !== -1 && list[select]) sortSelect = list[select].index;
        sordId = value;
        sortList.sort(( a, b ) =>  b[value] - a[value]);
        if (sortSelect !== -1) {
            sortList.forEach((value, index) => {
                if (sortSelect === value.index) {
                    select = index;
                }
            });
        }
        list = sortList;
    }


    const setItem = (index) => {
        if (!list[index]) return;
        else if (index === select) return;
        //else if (selectTime > new Date().getTime()) return;
        //selectTime = new Date().getTime() + 1000;
        select = index;
        executeClient ('auto', 'model', list[index].index);
    }

    const setColor = (index) => {
        if (index === colorId) return;
        colorId = index;
        executeClient ('auto', 'color', index);
    }

    const startTestDrive = (type) => {
        if (select === -1 || !list[select]) return;
        executeClient ('testDrive', type);
    }

    const HandleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 27) return;

        executeClient ('closeAuto');
    }
</script>

<svelte:window on:keyup={HandleKeyDown} />
<div id="autoshop">
    <div class="box-KeyInfo relative">
        <div class="KeyInfo"><span class="autoshop-mouse" /></div>
        {translateText('business', 'ЛКМ - осмотреть')}
    </div>

    <div class="box-content">
        <div class="box-auto-list" on:mouseenter={() => executeClient ("client.camera.toggled", false)} on:mouseleave={() => executeClient ("client.camera.toggled", true)}>
            <span class="autoshop-racing logo" />
            <div class="title">{translateText('business', 'Автосалон')}</div>
            <div class="box-sort">
                <button class="btn-sort" class:active={sordId === "price"}>
                    <span class="autoshop-money" on:click={() => sort ("price")} />
                </button>
                <button class="btn-sort" class:active={sordId === "speed"}>
                    <span class="autoshop-speed" on:click={() => sort ("speed")} />
                </button>
                <button class="btn-sort" class:active={sordId === "seat"}>
                    <span class="autoshop-seat" on:click={() => sort ("seat")} />
                </button>
                <button class="btn-sort" class:active={sordId === "boost"}>
                    <span class="autoshop-boost" on:click={() => sort ("boost")} />
                </button>
            </div>
            <div class="box-list">
            {#each list as value, index}
                <div class="box-item" class:active={select === index} on:click={() => setItem (index)}>
                    {@html value.modelName}
                    <span>{format("money", value.price)}{isDonateAutoroom === true ? 'RB' : '$'}</span>
                </div>
            {/each}
            </div>
        </div>
        {#if (select !== -1 && list [select])}
        <div class="box-auto-info" on:mouseenter={() => executeClient ("client.camera.toggled", false)} on:mouseleave={() => executeClient ("client.camera.toggled", true)}>
            <div class="box-title">
                <div class="title">{list [select].modelName}</div>
                <span class="autoshop-racing logo" />
            </div>
            {#if list [select].desc}
            <div class="desc">
                <span>{list [select].modelName} - </span>{list [select].desc}
            </div>
            {/if}
            <div class="info">{translateText('business', 'Характеристики')}:</div>
            <div class="box-info">
                <div class="box-item">
                    <div class="box-icon">
                        <span class="autoshop-money" />
                        {translateText('business', 'Гос.стоимость')}:
                    </div>
                    {format("money", list [select].gosPrice)}
                </div>
                <div class="box-item">
                    <div class="box-icon">
                        <span class="autoshop-speed" />
                        {translateText('business', 'Макс.скорость')}:
                    </div>
                    {list [select].speed}
                </div>
                <div class="box-item">
                    <div class="box-icon">
                        <span class="autoshop-boost" />
                        {translateText('business', 'Разгон')}:
                    </div>
                    {list [select].boost}
                </div>
                <div class="box-item">
                    <div class="box-icon">
                        <span class="autoshop-seat" />
                        {translateText('business', 'Кол-во мест')}:
                    </div>
                    {list [select].seat}
                </div>
                <div class="box-item">
                    <div class="box-icon">
                        <span class="autoshop-invslots" />
                        {translateText('business', 'Багажных мест')}:
                    </div>
                    {list [select].invslots}
                </div>
            </div>
            <div class="info">{translateText('business', 'Цвет')}:</div>
            <div class="box-colors">
            {#each authColors as value, index}
                <i key={index} class={`color ${colorId !== index || "active"}`} on:click={() => setColor (index)} style="background: {value}" />
            {/each}
            </div>
            <button class="btn-test-drive" on:click={() => startTestDrive (1)}>{translateText('business', 'Тест-драйв')} (100$)</button>
            <button class="btn-test-drive" on:click={() => startTestDrive (2)}>{translateText('business', 'Тест-драйв FT')} (300$)</button>
        </div>
        {/if}
    </div>
    <div class="box-KeyInfo relative" on:mouseenter={() => executeClient ("client.camera.toggled", false)} on:mouseleave={() => executeClient ("client.camera.toggled", true)}>
        {#if (select !== -1 && list [select])}
            <button class="btn-footer" on:click={() => executeClient ('buyAuto', 1)}>{translateText('business', 'Купить за')} {isDonateAutoroom === true ? `${format("money", list [select].price)}RB` : `$${format("money", list [select].price)}`}</button>
            <button class="btn-footer" on:click={() => executeClient ('buyAuto', 2)}>{translateText('business', 'Купить для семьи за')} {isDonateAutoroom === true ? `${format("money", list [select].price)}RB` : `$${format("money", list [select].gosPrice)}`}</button>
        {/if}
        <button class="btn-footer" on:click={() => executeClient ('closeAuto')}>{translateText('business', 'Выйти')}</button>
    </div>
</div>