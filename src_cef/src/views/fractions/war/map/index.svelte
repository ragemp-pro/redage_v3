
<script>
    import mapImage from './map.jpg'

    import { loadImage, loadAwaitImage } from 'api/functions'

    export let getPosition = false;

    export let elementWidth = 0;
    export let elementHeight = 0;

    export let onSelectItem;

    let posXToMap,
        posYToMap;

    const GetCoordsToMap = (posX, posY) => [3756 + posX / 1.51821820693, 5528 - posY / 1.51821820693];
    const GetMapPosToCoords = (posX, posY) => [1.51821820693 * (posX - 3756), -1.51821820693 * (posY - 5528)];

    const setMapCoords = (posX, posY) => {
        if (elementWidth && elementHeight) {
            const [x, y] = GetCoordsToMap(posX, posY);
            posXToMap = x - (elementWidth / 2);
            posYToMap = y - (elementHeight / 2);
        }
    }

    $: if (elementHeight)
        setMapCoords (getPosition[0], getPosition[1]);

    $: if (getPosition)
        setMapCoords (getPosition[0], getPosition[1]);

    import { fade } from 'svelte/transition'

    export let zones;

    let isDown;

    let speed,
        awaitCount,
        offsetX,
        offsetY,
        width,
        height,
        maps_wrapper,
        intervalId;

    
    import { onMount } from 'svelte';
	onMount(() => {
		maps_wrapper = document.querySelector('.war__maps_wrapper');
        if (maps_wrapper) {
            maps_wrapper = maps_wrapper.getBoundingClientRect();

            width = 8192 - maps_wrapper.width;
            height = 8192 - maps_wrapper.height;
        }
        
        initZone ();

        intervalId = setInterval(() => {
            zones.forEach(item => {
                if (typeof zonesData[item.id] !== "undefined") {
                    updateZoneColor(zonesData[item.id])
                }
            });
        }, 1000);
    });

    const initZone = () => {
        zones.forEach(item => {
            if (typeof zonesData[item.id] !== "undefined") {
                zonesData[item.id].owner = item.owner;
                zonesData[item.id].color = item.color;
                zonesData[item.id].isWar = item.isWar;
                zonesData[item.id].protectingColor = item.protectingColor;
                zonesData[item.id].attackingColor = item.attackingColor;
                
                setZoneColor(zonesData[item.id])
            }
        });
    }

    $: if (zones) {
        initZone ();
    }

    const setZoneColor = (item) => {
        const element = document.querySelector('.war__maps_absoluteelement.' + item.zone + ' svg path');
        if (!element)
            return;

        if (!item.isWar) {
            
            const element = document.querySelector('.war__maps_absoluteelement.' + item.zone + ' svg path');
            if (!element)
                return;

            let color = `rgb(255, 255, 255)`
            if (item.color && item.color.length)
                color = `rgb(${item.color[0]},${item.color[1]},${item.color[2]})`;


            element.style.fill = color
            element.style.stroke = color
        }
    }

    const updateZoneColor = (item) => {
        if (item.isWar) {
            const element = document.querySelector('.war__maps_absoluteelement.' + item.zone + ' svg path');
            if (!element)
                return;

            let color = `rgb(255, 255, 255)`
            if (item.protectingColor && item.protectingColor.length)
                color = `rgb(${item.protectingColor[0]}, ${item.protectingColor[1]}, ${item.protectingColor[2]})`;

            element.style.fill = color;
            element.style.stroke = color;

            let color2 = `rgb(255, 0, 0)`
            if (item.attackingColor && item.attackingColor.length) 
                color2 = `rgb(${item.attackingColor[0]}, ${item.attackingColor[1]}, ${item.attackingColor[2]})`;


            if (typeof item.isTick === "undefined")
                item.isTick = true;

            if (item.isTick) {
                element.style.fill = color2
                element.style.stroke = color2
                item.isTick = false;
            } else {
                element.style.fill = color
                element.style.stroke = color
                item.isTick = true;
            }
        }
    }
    
    import { onDestroy } from 'svelte';

    onDestroy(() => {
        
        clearInterval(intervalId);
    });

    const handleGlobalMouseMove = event => {
        if (isDown) {
            if (0 == ++speed % 15) {
                offsetX = event.offsetX;
                offsetY = event.offsetY;
            }
            if (4 > ++awaitCount) 
                return;
            awaitCount = 0;

            posXToMap = Math.max(0, Math.min(posXToMap - .15 * (event.offsetX - offsetX), width));
            posYToMap = Math.max(0, Math.min(posYToMap - .15 * (event.offsetY - offsetY), height));
        } else {

            offsetX = event.offsetX;
            offsetY = event.offsetY;
        }
    }

    const handleGlobalMouseUp = event => {
        isDown = false;
    }

    const handleGlobalMouseDown = event => {
        isDown = true;
        onSelectItem ()
    }

    let zonesData = [{
            name: "Аэропорт 1",
            descr: "Позволяет контролировать импорт и экспорт товаров штата",
            zone: "airport1",
            element: airport1,
            owner: "",
            color: null
        },
        {
            name: "Аэропорт 2",
            descr: "Позволяет контролировать импорт и экспорт товаров штата",
            zone: "airport2",
            element: airport2,
            owner: "",
            color: null
        },
        {
            name: "Автосервис",
            descr: "Позволяет получать часть прибыли от работы механиков",
            zone: "mech",
            element: mech,
            owner: "",
            color: null
        },
        {
            name: "Автобусный парк",
            descr: "Позволяет получать часть прибыли водителей автобусов",
            zone: "buspark",
            element: buspark,
            owner: "",
            color: null
        },
        {
            name: "Театр",
            descr: "Позволяет получать часть прибыли от посетителей",
            zone: "theatre",
            element: theatre,
            owner: "",
            color: null
        },
        {
            name: "Аренда велосипедов",
            descr: "Позволяет получать часть прибыли от аренды велосипедов",
            zone: "bikerent",
            element: bikerent,
            owner: "",
            color: null
        },
        {
            name: "Аренда лодок",
            descr: "Позволяет получать часть прибыли от аренды лодок",
            zone: "boatrent",
            element: boatrent,
            owner: "",
            color: null
        },
        {
            name: "Аренда офф-роад машин",
            descr: "Позволяет получать часть прибыли от аренды офф-роад машин",
            zone: "offroadrent",
            element: offroadrent,
            owner: "",
            color: null
        },
        {
            name: "Газонокосилки",
            descr: "Позволяет получать часть прибыли от работников газонокосилок",
            zone: "jobgason",
            element: jobgason,
            owner: "",
            color: null
        },
        {
            name: "Рынок",
            descr: "Позволяет получать часть прибыли от продаж на рынке",
            zone: "rynok",
            element: rynok,
            owner: "",
            color: null
        },
        {
            name: "Авторынок",
            descr: "Позволяет получать часть прибыли от продаж на авторынке",
            zone: "autorynok",
            element: autorynok,
            owner: "",
            color: null
        },
        {
            name: "Гос. шахта",
            descr: "Позволяет получать часть ресурсов от добычи на гос. шахте",
            zone: "gosshahta",
            element: gosshahta,
            owner: "",
            color: null
        },
        {
            name: "Шахта 1",
            descr: "Позволяет получать часть ресурсов от добычи на шахте",
            zone: "shahta1",
            element: shahta1,
            owner: "",
            color: null
        },
        {
            name: "Шахта 2",
            descr: "Позволяет получать часть ресурсов от добычи на шахте",
            zone: "shahta2",
            element: shahta2,
            owner: "",
            color: null
        },
        {
            name: "Шахта 3",
            descr: "Позволяет получать часть ресурсов от добычи на шахте",
            zone: "shahta3",
            element: shahta3,
            owner: "",
            color: null
        },
        {
            name: "Шахта 4",
            descr: "Позволяет получать часть ресурсов от добычи на шахте",
            zone: "shahta4",
            element: shahta4,
            owner: "",
            color: null
        },
        {
            name: "Стоянка дальнобойщиков",
            descr: "Позволяет получать часть прибыли от работы дальнобойщиков",
            zone: "dalnoboi",
            element: dalnoboi,
            owner: "",
            color: null
        },
        {
            name: "Склад 1",
            descr: "Позволяет хранить различные товары",
            zone: "sklad1",
            element: sklad1,
            owner: "",
            color: null
        },
        {
            name: "Склад 2",
            descr: "Позволяет хранить различные товары",
            zone: "sklad2",
            element: sklad2,
            owner: "",
            color: null
        },
        {
            name: "Завод 1",
            descr: "Позволяет получать часть прибыли от работы завода",
            zone: "zavod",
            element: zavod,
            owner: "",
            color: null
        },
        {
            name: "Химическая лаборатория",
            descr: "Позволяет получать часть прибыли от работы химической лаборатории",
            zone: "himlab",
            element: himlab,
            owner: "",
            color: null
        },
        {
            name: "Стоянка инкассаторов",
            descr: "Позволяет получать часть прибыли от работы инкассаторов",
            zone: "inkas",
            element: inkas,
            owner: "",
            color: null
        },
        {
            name: "Казино",
            descr: "Позволяет получать часть прибыли казино",
            zone: "casino",
            element: casino,
            owner: "",
            color: null
        },
        {
            name: "Лесные ресурсы 1",
            descr: "Позволяет получать часть ресурсов от добычи на лесопилке",
            zone: "forest1",
            element: forest1,
            owner: "",
            color: null
        },
        {
            name: "Лесные ресурсы 2",
            descr: "Позволяет получать часть ресурсов от добычи на лесопилке",
            zone: "forest2",
            element: forest2,
            owner: "",
            color: null
        },
        {
            name: "Лесные ресурсы 3",
            descr: "Позволяет получать часть ресурсов от добычи на лесопилке",
            zone: "forest3",
            element: forest3,
            owner: "",
            color: null
        },
        {
            name: "Лесные ресурсы 4",
            descr: "Позволяет получать часть ресурсов от добычи на лесопилке",
            zone: "forest4",
            element: forest4,
            owner: "",
            color: null
        },
        {
            name: "Лесные ресурсы 5",
            descr: "Позволяет получать часть ресурсов от добычи на лесопилке",
            zone: "forest5",
            element: forest5,
            owner: "",
            color: null
        },
        {
            name: "Арена",
            descr: "Позволяет получать часть прибыли от боёв на арене",
            zone: "arena",
            element: arena,
            owner: "",
            color: null
        },
        {
            name: "Охотничий магазин",
            descr: "Позволяет получать часть прибыли от работы охотничьего магазина",
            zone: "huntingshop",
            element: huntingshop,
            owner: "",
            color: null
        },
        {
            name: "Электростанция",
            descr: "Позволяет получать часть прибыли от работы электриков",
            zone: "electric",
            element: electric,
            owner: "",
            color: null
        },
        {
            name: "Риэлторское агенство",
            descr: "Позволяет получать часть прибыли от работы риэлторского агенства",
            zone: "rielt",
            element: rielt,
            owner: "",
            color: null
        },
        {
            name: "Центр Kortz",
            descr: "Позволяет получать часть прибыли от посетителей",
            zone: "observatorya",
            element: observatorya,
            owner: "",
            color: null
        },
        {
            name: "Таксопарк",
            descr: "Позволяет получать часть прибыли от работы таксистов",
            zone: "taxi",
            element: taxi,
            owner: "",
            color: null
        },
        {
            name: "Продажа марихуаны 1",
            descr: "Позволяет получать часть прибыли от продажи марихуаны",
            zone: "drugs",
            element: drugs,
            owner: "",
            color: null
        },
        {
            name: "Продажа марихуаны 2",
            descr: "Позволяет получать часть прибыли от продажи марихуаны",
            zone: "drugs2",
            element: drugs2,
            owner: "",
            color: null
        },
        {
            name: "Чёрный рынок",
            descr: "Позволяет получать часть прибыли от продаж на чёрном рынке",
            zone: "darkshop",
            element: darkshop,
            owner: "",
            color: null
        },
        {
            name: "Порт",
            descr: "Позволяет контролировать импорт и экспорт товаров штата",
            zone: "port",
            element: port,
            owner: "",
            color: null
        },
        {
            name: "Завод 2",
            descr: "Позволяет получать часть прибыли от работы завода",
            zone: "zavod2",
            element: zavod2,
            owner: "",
            color: null
        },
        {
            name: "Склад 3",
            descr: "Позволяет хранить различные товары",
            zone: "sklad3",
            element: sklad3,
            owner: "",
            color: null
        },
        {
            name: "Склад 4",
            descr: "Позволяет хранить различные товары",
            zone: "sklad4",
            element: sklad4,
            owner: "",
            color: null
        },
        {
            name: "Склад 5",
            descr: "Позволяет хранить различные товары",
            zone: "sklad5",
            element: sklad5,
            owner: "",
            color: null
        },
        {
            name: "Склад 6",
            descr: "Позволяет хранить различные товары",
            zone: "sklad6",
            element: sklad6,
            owner: "",
            color: null
        },
        {
            name: "Нефтедобывающее предприятие 1",
            descr: "Позволяет получать часть прибыли от добычи нефти",
            zone: "oil",
            element: oil,
            owner: "",
            color: null
        },
        {
            name: "Нефтедобывающее предприятие 2",
            descr: "Позволяет получать часть прибыли от добычи нефти",
            zone: "oil2",
            element: oil2,
            owner: "",
            color: null
        },
        {
            name: "Нефтедобывающее предприятие 3",
            descr: "Позволяет получать часть прибыли от добычи нефти",
            zone: "oil3",
            element: oil3,
            owner: "",
            color: null
        },
        {
            name: "Нефтедобывающее предприятие 4",
            descr: "Позволяет получать часть прибыли от добычи нефти",
            zone: "oil4",
            element: oil4,
            owner: "",
            color: null
        },
        {
            name: "Бар на острове",
            descr: "Позволяет получать часть прибыли от бара",
            zone: "landbar",
            element: landbar,
            owner: "",
            color: null
        },
        {
            name: "Театр 2",
            descr: "Позволяет получать часть прибыли от посетителей",
            zone: "theatre2",
            element: theatre2,
            owner: "",
            color: null
        },
        {
            name: "Ветряная электростанция 1",
            descr: "Позволяет получать часть прибыли от добычи электричества",
            zone: "vetryanaya1",
            element: vetryanaya1,
            owner: "",
            color: null
        },
        {
            name: "Ветряная электростанция 2",
            descr: "Позволяет получать часть прибыли от добычи электричества",
            zone: "vetryanaya2",
            element: vetryanaya2,
            owner: "",
            color: null
        },
        {
            name: "Ветряная электростанция 3",
            descr: "Позволяет получать часть прибыли от добычи электричества",
            zone: "vetryanaya3",
            element: vetryanaya3,
            owner: "",
            color: null
        },
        {
            name: "Склад 7",
            descr: "Позволяет хранить различные товары",
            zone: "sklad7",
            element: sklad7,
            owner: "",
            color: null
        },
        {
            name: "Ферма 1",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm1",
            element: farm1,
            owner: "",
            color: null
        },
        {
            name: "Ферма 2",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm2",
            element: farm2,
            owner: "",
            color: null
        },
        {
            name: "Ферма 3",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm3",
            element: farm3,
            owner: "",
            color: null
        },
        {
            name: "Ферма 4",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm4",
            element: farm4,
            owner: "",
            color: null
        },
        {
            name: "Ферма 5",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm5",
            element: farm5,
            owner: "",
            color: null
        },
        {
            name: "Ферма 6",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm6",
            element: farm6,
            owner: "",
            color: null
        },
        {
            name: "Ферма 7",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm7",
            element: farm7,
            owner: "",
            color: null
        },
        {
            name: "Ферма 8",
            descr: "Позволяет выращивать ресурсы для дальнейшей продажи",
            zone: "farm8",
            element: farm8,
            owner: "",
            color: null
        },
        {
            name: "Железнодорожная станция",
            descr: "Позволяет контролировать импорт и экспорт товаров штата",
            zone: "vokzal",
            element: vokzal,
            owner: "",
            color: null
        },
        {
            name: "Аренда гоночных машин",
            descr: "Позволяет получать часть прибыли от аренды гоночных машин",
            zone: "speedcarrent",
            element: speedcarrent,
            owner: "",
            color: null
        },
    ]

    import airport1 from './svg/airport1.svelte'
    import airport2 from './svg/airport2.svelte'
    import mech from './svg/mech.svelte'
    import buspark from './svg/buspark.svelte'
    import theatre from './svg/theatre.svelte'
    import bikerent from './svg/bikerent.svelte'
    import boatrent from './svg/boatrent.svelte'
    import offroadrent from './svg/offroadrent.svelte'
    import jobgason from './svg/jobgason.svelte'
    import rynok from './svg/rynok.svelte'
    import autorynok from './svg/autorynok.svelte'
    import gosshahta from './svg/gosshahta.svelte'
    import shahta1 from './svg/shahta1.svelte'
    import shahta2 from './svg/shahta2.svelte'
    import shahta3 from './svg/shahta3.svelte'
    import shahta4 from './svg/shahta4.svelte'
    import dalnoboi from './svg/dalnoboi.svelte'
    import sklad1 from './svg/sklad1.svelte'
    import sklad2 from './svg/sklad2.svelte'
    import zavod from './svg/zavod.svelte'
    import himlab from './svg/himlab.svelte'
    import inkas from './svg/inkas.svelte'
    import casino from './svg/casino.svelte'
    import forest1 from './svg/forest1.svelte'
    import forest2 from './svg/forest2.svelte'
    import forest3 from './svg/forest3.svelte'
    import forest4 from './svg/forest4.svelte'
    import forest5 from './svg/forest5.svelte'
    import arena from './svg/arena.svelte'
    import huntingshop from './svg/huntingshop.svelte'
    import electric from './svg/electric.svelte'
    import rielt from './svg/rielt.svelte'
    import observatorya from './svg/observatorya.svelte'
    import taxi from './svg/taxi.svelte'
    import drugs from './svg/drugs.svelte'
    import drugs2 from './svg/drugs2.svelte'
    import darkshop from './svg/darkshop.svelte'
    import port from './svg/port.svelte'
    import zavod2 from './svg/zavod2.svelte'
    import sklad3 from './svg/sklad3.svelte'
    import sklad4 from './svg/sklad4.svelte'
    import sklad5 from './svg/sklad5.svelte'
    import sklad6 from './svg/sklad6.svelte'
    import oil from './svg/oil.svelte'
    import oil2 from './svg/oil2.svelte'
    import oil3 from './svg/oil3.svelte'
    import oil4 from './svg/oil4.svelte'
    import landbar from './svg/landbar.svelte'
    import theatre2 from './svg/theatre2.svelte'
    import vetryanaya1 from './svg/vetryanaya1.svelte'
    import vetryanaya2 from './svg/vetryanaya2.svelte'
    import vetryanaya3 from './svg/vetryanaya3.svelte'
    import sklad7 from './svg/sklad7.svelte'
    import farm1 from './svg/farm1.svelte'
    import farm2 from './svg/farm2.svelte'
    import farm3 from './svg/farm3.svelte'
    import farm4 from './svg/farm4.svelte'
    import farm5 from './svg/farm5.svelte'
    import farm6 from './svg/farm6.svelte'
    import farm7 from './svg/farm7.svelte'
    import farm8 from './svg/farm8.svelte'
    import vokzal from './svg/vokzal.svelte'
    import speedcarrent from './svg/speedcarrent.svelte'

    export let selectItem;
</script>

{#if getPosition}
    <div class="war__maps_wrapper" on:mousemove={handleGlobalMouseMove}  on:mouseup={handleGlobalMouseUp}><!--  -->

        <div class="war__maps_container" style="top: {-posYToMap}px; left: {-posXToMap}px;">
            {#each zonesData as item, index}
                <div class="war__maps_absoluteelement {item.zone}" class:active={selectItem && selectItem.id === index} class:unEvents={isDown} on:click={() => onSelectItem ({
                    ...item,
                    id: index
                })}>
                    <svelte:component this={item.element} />
                </div>
            {/each}
            {#await loadAwaitImage(mapImage) then _}
                <div class="war__maps_image" on:mousedown={handleGlobalMouseDown} style="background-image: url({mapImage})"/>
            {/await}
        </div>
    </div>
{/if}