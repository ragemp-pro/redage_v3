<script>
    import { data } from 'store/customization';
    import ListButton from './listbutton.svelte';
    import Selector from './selector/index.svelte';
    import { gender, customization, updateCharacteristic } from 'store/customization';

    import characteristics from './characteristics.js';

    let isUse = false;
    let refCallback = null;
    let refCallbackPoint = null;
    let activeItem = 0;

    const componentsData = [
        {
            name: 'Лоб',
            dataname: 'brow',
            preset: 0,
            gridType: false,

            xgrid: ["Плоский", "Выпуклый"],
            ygrid: ["Высокий", "Низкий"],
        },
        {
            name: 'Глаза',
            dataname: 'eyes',
            preset: 0,
            gridType: true,

            xgrid: ["Узкие", "Широкие"],
            ygrid: [],
        },
        {
            name: 'Нос',
            dataname: 'nose',
            preset: 0,
            gridType: false,

            xgrid: ["Узкий", "Широкий"],
            ygrid: ["Кверху", "Низкий"],
        },
        {
            name: 'Проф. носа',
            dataname: 'noseprofile',
            preset: 0,
            gridType: false,

            xgrid: ["Малый", "Большой"],
            ygrid: ["С горбинкой", "Выгнутый"],
        },
        {
            name: 'Кончик носа',
            dataname: 'nosetip',
            preset: 0,
            gridType: false,

            xgrid: ["Скос влево", "Скос вправо"],
            ygrid: ["Кончик вверх", "Кончик вниз"],
        },
        {
            name: 'Скулы',
            dataname: 'cbones',
            preset: 0,
            gridType: false,

            xgrid: ["Худые", "Надутые"],
            ygrid: ["Высокие", "Низкие"],
        },
        {
            name: 'Щёки',
            dataname: 'cheeks',
            preset: 0,
            gridType: true,

            xgrid: ["Худые", "Надутые"],
            ygrid: [],
        },
        {
            name: 'Губы',
            dataname: 'lips',
            preset: 0,
            gridType: true,

            xgrid: ["Тонкие", "Толстые"],
            ygrid: [],
        },
        {
            name: 'Челюсть',
            dataname: 'jaw',
            preset: 0,
            gridType: false,

            xgrid: ["Узкая", "Широкая"],
            ygrid: ["Круглая", "Квадратная"],
        },
        {
            name: 'Профиль подбородка',
            dataname: 'chinprofile',
            preset: 0,
            gridType: false,

            xgrid: ["Короткий", "Длинный"],
            ygrid: ["Высокий", "Низкий"],
        },
        {
            name: 'Форма подбородка',
            dataname: 'chinform',
            preset: 0,
            gridType: false,

            xgrid:["Квадратный", "Заострённый"],
            ygrid: ["Округлый", "С ямочкой"],
        }
    ];

    const OnPresetChanged = (index, change) => {
        activeItem = index;

        const key = componentsData[activeItem].dataname;
        let preset = $customization[key].preset;
        preset += change;

        if (preset >= 3) preset = 0;
        else if (preset < 0) preset = 2;

        let presetSettings = characteristics[key][preset];
        updateCharacteristic($gender, key, preset, presetSettings.x, presetSettings.y);
    }
    
    const OnCustomPresetChanged = (key, x, y) => {
        updateCharacteristic($gender, key, 3, x, y);
    }
    
    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch (keyCode) {
            case 38: // up
                if(--activeItem < 0)
                    activeItem = componentsData.length - 1;
                break;
            case 40: // down
                if(++activeItem >= componentsData.length)
                    activeItem = 0;
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div class="auth__customization_elements">
    {#each componentsData as item, index}
    {#if $customization[item.dataname]}
    <div class="auth__customization_element" class:active={activeItem === index} on:click={() => activeItem = index}>
        <div class="auth__customization_leftside">{item.name}</div>

        <ListButton
            on:click={e => activeItem = index}
            key={item.dataname}
            active={activeItem === index}
            preset={$customization[item.dataname].preset}
            onChange={(change) => OnPresetChanged (index, change)} />
    </div>
    {/if}
    {/each}
</div>
<div class="auth__scroll" />
<div class="title margin-top-20">{componentsData[activeItem].name}:</div>
{#if $customization[componentsData[activeItem].dataname]}
<Selector
    isLine={componentsData[activeItem].gridType}
    key={componentsData[activeItem].dataname}
    x={$customization[componentsData[activeItem].dataname].x}
    y={$customization[componentsData[activeItem].dataname].y}
    xLeftName={componentsData[activeItem].xgrid[0]}
    xRightName={componentsData[activeItem].xgrid[1]}
    yTopName={componentsData[activeItem].ygrid[0]}
    yBottomName={componentsData[activeItem].ygrid[1]}
    onChange={OnCustomPresetChanged} />

{/if}