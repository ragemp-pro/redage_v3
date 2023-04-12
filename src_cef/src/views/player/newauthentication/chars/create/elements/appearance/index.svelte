<script>
    import { gender, customization, updateAppearance } from 'store/customization';

    import ListButton from './listbutton.svelte';
    import InputBlock from './../input_item.svelte';
    import appearances from './appearances.js';

    let activeItem = 0;
    
    const hairColors = [
        {id: 0, hexcolor: "#201c19"},
        {id: 1, hexcolor: "#362f29"},
        {id: 2, hexcolor: "#4c392e"},
        {id: 3, hexcolor: "#462319"},
        {id: 4, hexcolor: "#773923"},
        {id: 5, hexcolor: "#6c2516"},
        {id: 6, hexcolor: "#a95e37"},
        {id: 7, hexcolor: "#a86b45"},
        {id: 8, hexcolor: "#a56f48"},
        {id: 9, hexcolor: "#a8754f"},
        {id: 11, hexcolor: "#bd925f"},
        {id: 12, hexcolor: "#d2ad78"},
        {id: 14, hexcolor: "#ddb26a"},
        {id: 15, hexcolor: "#d6aa6a"},
        {id: 16, hexcolor: "#b98557"},
        {id: 17, hexcolor: "#8e4b2f"},
        {id: 18, hexcolor: "#8d3323"}, 
        {id: 19, hexcolor: "#7d1714"},
        {id: 20, hexcolor: "#8e1a14"},
        {id: 23, hexcolor: "#e14e28"},
        {id: 24, hexcolor: "#c25732"},
        {id: 25, hexcolor: "#c74e2d"},
        {id: 26, hexcolor: "#ad917b"},
        {id: 27, hexcolor: "#c7ac92"},
    ];

    const componentsData = [
        {
            name: 'Причёска',
            dataname: 'hair',
            onlysex: "all",
            colorSelector: hairColors
        },
        {
            name: 'Брови',
            dataname: 'eyebrows',
            onlysex: "all",
            opacitySelector: true,
            colorSelector: hairColors
        },
        {
            name: 'Борода',
            dataname: 'facialhair',
            onlysex: true,
            lockSelectorZero: true,
            opacitySelector: true,
            colorSelector: hairColors
        },
        {
            name: 'Дефекты кожи',
            dataname: 'blemishes',
            onlysex: "all",
            lockSelectorZero: true,
            opacitySelector: true
        },
        {
            name: 'Старение кожи',
            dataname: 'ageing',
            onlysex: "all",
            lockSelectorZero: true,
            opacitySelector: true
        },
        {
            name: 'Тип кожи',
            dataname: 'complexion',
            onlysex: "all",
            lockSelectorZero: true,
            opacitySelector: true
        },
        {
            name: 'Родинки и веснушки',
            dataname: 'molesfreckles',
            onlysex: "all",
            lockSelectorZero: true,
            opacitySelector: true
        },
        {
            name: 'Повреждение кожи',
            dataname: 'sundamage',
            onlysex: "all",
            lockSelectorZero: true,
            opacitySelector: true
        },
        {
            name: 'Цвет глаз',
            dataname: 'eyescolor',
            onlysex: "all"
        },
        {
            name: 'Помада',
            dataname: 'lipstick',
            onlysex: false,
            lockSelectorZero: true,
            opacitySelector: true,
            colorSelector: [
                {id: 0, hexcolor: "#992532"},
                {id: 1, hexcolor: "#c9395d"},
                {id: 2, hexcolor: "#bd516b"},
                {id: 3, hexcolor: "#b8637a"},
                {id: 4, hexcolor: "#a6526a"},
                {id: 5, hexcolor: "#b1434c"},
                {id: 6, hexcolor: "#803136"},
                {id: 7, hexcolor: "#a6625f"},
                {id: 8, hexcolor: "#c18779"},
                {id: 9, hexcolor: "#cc9f99"},
                {id: 10, hexcolor: "#c7918f"},
                {id: 11, hexcolor: "#ab6f64"},
                {id: 12, hexcolor: "#af6051"},
                {id: 13, hexcolor: "#a84c33"},
                {id: 14, hexcolor: "#b67078"},
                {id: 15, hexcolor: "#ca7f93"},
                {id: 16, hexcolor: "#ec9cbf"},
                {id: 17, hexcolor: "#e476a5"},
                {id: 18, hexcolor: "#de3d81"},
                {id: 19, hexcolor: "#b44c6f"},
                {id: 20, hexcolor: "#71263a"},
                {id: 21, hexcolor: "#71263a"},
                {id: 22, hexcolor: "#aa2230"},
                {id: 23, hexcolor: "#dd2034"}
            ]
        },
    ];

    let componentsDataToGender = [];
    gender.subscribe((value) => {
        let _componentsDataToGender = [];
        componentsData.forEach((component, index) => {
            if (component.onlysex === "all" || component.onlysex === value) {
                _componentsDataToGender.push (component);
            }
        });
        componentsDataToGender = _componentsDataToGender;
    });


    const OnChangeAppearance = (index, change) => {
        activeItem = index;

        let key = componentsDataToGender[activeItem].dataname;
        let id = $customization[key].id;
        id += change;
        
        let appearancesList = Array.isArray(appearances[key]) ? appearances[key] : appearances[key][$gender];
        
        if (id >= appearancesList.length - 1) id = 0;
        else if (id < 0) id = appearancesList.length - 1;

        updateAppearance($gender, key, id, $customization[key].color, $customization[key].opacity)
    }
    
    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch(keyCode) {
            case 38: // up
                if(--activeItem < 0)
                    activeItem = componentsDataToGender.length - 1;
                break;
            case 40: // down
                if(++activeItem >= componentsDataToGender.length)
                    activeItem = 0;
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div class="auth__customization_elements">
    {#each componentsDataToGender as component, index}
        {#if component.onlysex === "all" || component.onlysex === $gender}
            <div class="auth__customization_element" class:active={activeItem === index} on:click={() => activeItem = index}>
                <div class="auth__customization_leftside">{component.name}</div>

                <ListButton
                    id={$customization[component.dataname].id}
                    on:click={e => activeItem = index}
                    key={component.dataname}
                    active={activeItem === index}
                    onChange={(change) => OnChangeAppearance (index, change)} />
            </div>
        {/if}
    {/each}
</div>
<div class="auth__scroll" />

<div class="auth__customization_elements">
    {#if componentsDataToGender[activeItem].opacitySelector}
    <div class="title margin-top-20">{componentsDataToGender[activeItem].name}:</div>
    <div class="auth__customization_element">
        <div class="auth__customization_leftside">Насыщенность:</div>
        <InputBlock
            id="appearance"
            leftText="arrow-down"
            rightText="arrow-up"
            step={0.1}
            min={0}
            max={1}
            value={$customization[componentsDataToGender[activeItem].dataname].opacity}
            callback={newvalue => updateAppearance($gender, componentsDataToGender[activeItem].dataname, $customization[componentsDataToGender[activeItem].dataname].id, $customization[componentsDataToGender[activeItem].dataname].color, newvalue)} />
    </div>
    {/if}

    {#if componentsDataToGender[activeItem].colorSelector}
    <div class="auth__customization_element column">
        <div class="auth__customization_leftside">Цвет</div>
        
        <div class="auth__customization_colors" style={`grid-template-rows: repeat(${Math.ceil(componentsDataToGender[activeItem].colorSelector.length / 9)}, 1fr)`}>
            {#each componentsDataToGender[activeItem].colorSelector as color, index}
            <div
                key={index}
                class="auth__customization_color" class:active={$customization[componentsDataToGender[activeItem].dataname].color === color.id}
                on:click={() => updateAppearance($gender, componentsDataToGender[activeItem].dataname, $customization[componentsDataToGender[activeItem].dataname].id, color.id, $customization[componentsDataToGender[activeItem].dataname].opacity)}
                style="background: {color.hexcolor}" />
        {/each}
        </div>
    </div>
    {/if}
</div>