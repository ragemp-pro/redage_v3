<script>
    import { translateText } from 'lang'
    import { slide, fade } from 'svelte/transition';
    import { executeClient } from 'api/rage'
    import './css/main.sass';
    
    import SelectMain from './elements/index.svelte'

    //Works
    import Podrabotki from './elements/works/podrabotki.svelte';
    import Electrician from './elements/works/electrician.svelte';
    import Lawnmower from './elements/works/lawnmower.svelte';
    import Postmen from './elements/works/postmen.svelte';
    import Taxi from './elements/works/taxi.svelte';
    import Bus from './elements/works/bus.svelte';
    import Mechanic from './elements/works/mechanic.svelte';
    import Trucker from './elements/works/trucker.svelte';
    import Coater from './elements/works/coater.svelte';

    //Fractions
    import Gang from './elements/fractions/gang.svelte';
    import Mafia from './elements/fractions/mafia.svelte';
    import TheLost from './elements/fractions/thelost.svelte';

    //Gov Fractions
    import Lspd from './elements/fractions/lspd.svelte';
    import NationalGuard from './elements/fractions/nationalguard.svelte';
    import Goverment from './elements/fractions/goverment.svelte';
    import Ems from './elements/fractions/ems.svelte';
    import Fib from './elements/fractions/fib.svelte';
    import Merryweather from './elements/fractions/merryweather.svelte';
    import News from './elements/fractions/news.svelte';
    import CreateFraction from './elements/fractions/createfraction.svelte';

    //Businesses
    import Shop from './elements/businesses/shop.svelte';
    import Barbershop from './elements/businesses/barbershop.svelte';
    import Restaurants from './elements/businesses/restaurants.svelte';
    import CarWash from './elements/businesses/carwash.svelte';
    import ClothingStore from './elements/businesses/clothingstore.svelte';
    import GunShop from './elements/businesses/gunshop.svelte';
    import Lsc from './elements/businesses/lsc.svelte';
    import MaskShop from './elements/businesses/maskshop.svelte';
    import GasStation from './elements/businesses/gasstation.svelte';
    import AutoMoto from './elements/businesses/automoto.svelte';
    import PetShop from './elements/businesses/petshop.svelte';
    import TattooShop from './elements/businesses/tattooshop.svelte';

    //Events
    import AirDrop from './elements/events/AirDrop.svelte';
    import Azart from './elements/events/Azart.svelte';
    import BizCapt from './elements/events/BizCapt.svelte';
    import Everyday from './elements/events/Everyday.svelte';
    import GunGame from './elements/events/GunGame.svelte';
    import StreetCar from './elements/events/StreetCar.svelte';
    import HeliCrash from './elements/events/HeliCrash.svelte';

    //Immovables
    import MainImmovables from './elements/immovables/main.svelte';
    import Advance from './elements/immovables/advance.svelte';
    const Views = {
        SelectMain
    }
    
    let menu = [
        {
            open: false,
            component: 'Works',
            name: translateText('player', 'Работы'),
            items: [
                {name: translateText('player', 'Подработки'), component: Podrabotki, waypoint: "Охотничий магазин"},
                {name: translateText('player', 'Электрик'), component: Electrician, waypoint: "Электростанция"},
                {name: translateText('player', 'Газонокосильщик'), component: Lawnmower, waypoint: "Стоянка газонокосилок"},
                {name: translateText('player', 'Почтальон'), component: Postmen, waypoint: "Отделение почты"},
                {name: translateText('player', 'Водитель такси'), component: Taxi, waypoint: "Таксопарк"},
                {name: translateText('player', 'Водитель автобуса'), component: Bus, waypoint: "Автобусный парк"},
                {name: translateText('player', 'Механик'), component: Mechanic, waypoint: "Стоянка автомехаников"},
                {name: translateText('player', 'Дальнобойщик'), component: Trucker, waypoint: "Стоянка дальнобойщиков"},
                {name: translateText('player', 'Инкассатор'), component: Coater, waypoint: "Стоянка инкассаторов"},
            ]
        },
        {
            open: false,
            name: translateText('player', 'Фракции'),
            component: 'Fractions',
            items: [
                {name: translateText('player', 'Банды'), component: Gang},
                {name: translateText('player', 'Мафии'), component: Mafia},
                {name: 'LSPD', component: Lspd, waypoint: "LSPD"},
                {name: 'National Guard', component: NationalGuard, waypoint: "NationalGuard"}, //waypoint not work
                {name: 'Goverment', component: Goverment, waypoint: "City Hall"},
                {name: 'EMS', component: Ems, waypoint: "EMS"},
                {name: 'FIB', component: Fib, waypoint: "FIB"},
                //{name: 'Merryweather Security', component: Merryweather, waypoint: "Merryweather"},
                {name: 'News', component: News, waypoint: "NEWS"},
                {name: translateText('player', 'Создание семьи'), component: CreateFraction, waypoint: "Семьи"}, //waypoint not work
                
            ]
        },
        {
            open: false,
            name: translateText('player', 'Бизнесы'),
            component: 'businesses', //waypoint not work
            items: [
                {name: translateText('player', 'Магазин 24/7'), component: Shop, waypoint: "Ближайший '24/7'"},
                {name: translateText('player', 'Барбершоп'), component: Barbershop, waypoint: "Ближайший 'Barber-Shop'"},
                {name: translateText('player', 'Рестораны'), component: Restaurants, waypoint: "Ближайший 'Burger-Shot'"},
                {name: translateText('player', 'Автомойка'), component: CarWash, waypoint: "Ближайший 'CarWash'"},
                {name: translateText('player', 'Магазин одежды'), component: ClothingStore, waypoint: "Ближайший 'Clothes Shop'"},
                {name: translateText('player', 'Оружейный магазин'), component: GunShop, waypoint: "Ближайший 'Gun Shop'"},
                {name: 'LSC', component: Lsc, waypoint: "Ближайший 'LS Customs'"},
                {name: translateText('player', 'Магазин масок'), component: MaskShop, waypoint: "Ближайший 'Masks Shop'"},
                {name: translateText('player', 'АЗС'), component: GasStation, waypoint: "Ближайший 'Petrol Station'"},
                {name: translateText('player', 'Автосалоны и мотосалоны'), component: AutoMoto, waypoint: "Ближайший 'Premium Autoroom'"},
                {name: translateText('player', 'Зоомагазин'), component: PetShop, waypoint: "Ближайший 'PetShop'"},
                {name: translateText('player', 'Тату-салон'), component: TattooShop, waypoint: "Ближайший 'Tattoo-salon'"},
            ]
        },
        {
            open: false,
            component: 'immovables',
            name: translateText('player', 'Недвижимость'),
            items: [
                {name: translateText('player', 'Общее'), component: MainImmovables},
                {name: translateText('player', 'Улучшения домов'), component: Advance},
            ]
        },
        {
            open: true,
            name: 'Мероприятия', 
            component: 'events',
            items: [
                {name: translateText('player', 'Ежедневные награды'), component: Everyday},
                {name: 'GunGame', component: GunGame},
                {name: 'AirDrop', component: AirDrop, waypoint: "AirDrop"},
                {name: 'Уличные гонки', component: StreetCar},
                {name: 'Бизвары/Капты', component: BizCapt},
                {name: 'Азартные игры', component: Azart},
                {name: 'Крушение вертолёта', component: HeliCrash},
            ]
        },
    ],
    active = {
        dropID: 0,
        mainID: 0,
    };

    const selectMain = (id) => {
        menu[id].open = !menu[id].open;
        active.mainID = id;
        active.dropID = 0;
    }

    const selectDropMain = (id, i) => {
        active.dropID = id;
        active.mainID = i;
        setTimeout(() => {
            document.querySelector("#help .help-card .scroll .content .accordionContent li .list.active").scrollIntoView();
        }, 150)
    }   

    const onExit = () => {
        executeClient ("client:OnCloseHelpMenu")
    }

    const onWayPoint = () => {
        if (!menu [active.mainID].items[active.dropID].waypoint)
            return;
        executeClient ("gps.name", menu [active.mainID].items[active.dropID].waypoint)
        //executeClient ("createWaypoint", menu [active.mainID].items[active.dropID].position[0], menu [active.mainID].items[active.dropID].position[1]);
        onExit ()
    }

    const onVoice = () => {
        executeClient ("client.help.voice")
    }
</script>

<div id="help">
    <div class="help-card">
        <div class="nav">
            <div class="header">
                {translateText('player', 'Помощь')} <div class="close" on:click={onExit}/>
            </div>
            <ul class="navigation">
                {#each menu as content, index}
                    <li class="list">
                        <div class="title" on:click={() => selectMain(index)}>
                            <div class="flex">
                                <span class="icon shieldUser"/>
                                <span>{content.name}</span>
                            </div>
                            {#if content.items}
                                <span class="icon arrow" class:active={content.open === true}/>
                            {/if}
                        </div>
                        {#if content.items && content.open == true}
                            <ul transition:slide|local class="items" >
                                {#each content.items as t, i}
                                    <li on:click={() => selectDropMain(i, index)}>
                                        {t.name}
                                    </li>
                                {/each}
                            </ul> 
                        {/if}
                    </li>
                {/each}
            </ul>

            <!--<div class="button">
                <div class="btn" on:click={onVoice}>Голосовой помощник</div>
            </div>-->

        </div>
        <div class="scroll">
            <div class="content" in:fade="{{ duration: 500 }}" out:fade="{{ duration: 0 }}">
                <ul class="accordionContent">
                {#each menu[active.mainID].items as item, index}
                    <li>
                        <div class="list" class:active={index === active.dropID} on:click={() => selectDropMain(index, active.mainID)}>{item.name} <span class="icon arrow" class:active={index === active.dropID}/></div>
                        {#if active.dropID== index}
                            <div in:slide|local out:slide|local="{{ duration: 150 }}" class="info">
                                <svelte:component this={item.component} {onWayPoint} />
                            </div>
                        {/if}
                    </li>
                {/each}
                </ul>
            </div>
        </div>
    </div>
</div>