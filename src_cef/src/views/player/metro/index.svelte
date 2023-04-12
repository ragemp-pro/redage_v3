<script>
    import { translateText } from 'lang'
    import './main.sass'
    import './fonts/style.css'
    import KeyAnimation from '@/components/keyAnimation/index.svelte';
    import { executeClient } from 'api/rage'
    export let viewData;


    const StationsData = [
        {
            station: "LSIA Terminal",
            id: 1,
            class: "first"
        },        
        {
            station: "LSIA Parking",
            id: 2,
            class: "second"
        },    
        {
            station: "Puerto Del Sol",
            id: 3,
            class: "third"
        },
        {
            station: "Strawberry",
            id: 4,
            class: "fourth"
        },        
        {
            station: "Pillbox North",
            id: 6,
            class: "fifth",
            isClose: true
        },
        {
            station: "Burton",
            id: 6,
            class: "sixth"
        },
        {
            station: "Portola Drive",
            id: 7,
            class: "seventh"
        },
        {
            station: "Del Perro",
            id: 8,
            class: "eight"
        },
        {
            station: "Little Seoul",
            id: 9,
            class: "nineth"
        },
        {
            station: "Pillbox South",
            id: 10,
            class: "tenth"
        },
        {
            station: "Davis",
            id: 11,
            class: "eleventh"
        },
    ]
    const priceTicket = 50;
    let indexMyPoint = StationsData.findIndex(sd => sd.station == viewData);
    let SelectStation = 0;
    let Increase = 0;

    const OnClose = () => {
        executeClient ("client.metro.close", true);
    }

    const OnBuy = () => {
        executeClient ("client.metro.buyTicket", StationsData [SelectStation].station, Increase);
    }

    const OnLeft = () => {
        if (--SelectStation < 0) SelectStation = StationsData.length - 1;
        if (StationsData [SelectStation].isClose) OnLeft ();
        else if (viewData === StationsData [SelectStation].station) OnLeft ();
        else UpdateIncrease ();
    }

    const OnRight = () => {
        if (++SelectStation >= StationsData.length) SelectStation = 0;
        if (StationsData [SelectStation].isClose) OnRight ();
        else if (viewData === StationsData [SelectStation].station) OnRight ();
        else UpdateIncrease ();
    }    
    
    const handleArrowKeys = (events) => {
        const { keyCode } = events;
        switch (keyCode) {
            case 37:
                OnLeft();
                break;
            case 39:
                OnRight();
                break;
            case 13: 
                OnBuy ();
                break;
            case 27: 
                OnClose ();
                break;
        }
    }
    const OnSelectStation = (index) => {       
        if (viewData === StationsData [index].station)
            return;
        else if (StationsData [index].isClose)
            return;
        
        SelectStation = index
        UpdateIncrease ()
    }

    const UpdateIncrease = () => {        
        indexMyPoint = StationsData.findIndex(sd => sd.station == viewData);
        const indexNewPoint = StationsData.findIndex(sd => sd.station == StationsData [SelectStation].station);

        Increase = indexMyPoint > indexNewPoint ? indexMyPoint - indexNewPoint : indexNewPoint - indexMyPoint;
    }	
    
	import { onMount } from 'svelte';

    onMount(() => {
        SelectStation = -1;
        OnRight ();
	});
</script>
<svelte:window on:keyup={handleArrowKeys} />
<div id="metro">
    <div class="metro__header">
        <span class="metroicons-station"></span>
        {translateText('player2', 'Карта метро')}
    </div>
    <div class="metro__main">
        <div class="metro__img">
            {#each StationsData as station, index}
            <div class="{station.class} metro__img_element" class:active={SelectStation == index} on:click={() => OnSelectStation (index)}>
                <div class="text">{station.id}. {station.station}</div>
                {#if viewData === station.station}<div class="red">{translateText('player2', 'Вы тут!')}</div>{/if}
                {#if station.isClose}<div class="gray">Closed</div>{/if}
            </div>
            {/each}
        </div>
        <div class="metro__info">
            <div class="metro__title">
                <span class="metroicons-znak"></span>
                {translateText('player2', 'Выбор станции')}
            </div>
            <div class="metro__stations">
                {#each StationsData as station, index}
                {#if !station.isClose}
                <div class="metro__station" class:active={SelectStation == index} class:noActive={viewData === station.station} on:click={() => OnSelectStation (index)}>
                    <div class="metro__station__header">
                        <div class="metroicons-diamond"></div>
                        {station.id}. {station.station}
                    </div>
                    <div class="metro__station_price">
                        {translateText('player2', 'Прайс:')} <span class="price">${priceTicket * (indexMyPoint > index ? indexMyPoint - index : index - indexMyPoint)}</span>
                    </div>
                </div>
                {/if}
                {/each}
            </div>
            <div class="metro__title">
                <span class="metroicons-ticket"></span>
                {translateText('player2', 'Покупка билета')}
            </div>
            <div class="metro__small">{translateText('player2', 'Станция')}</div>
            <div class="metro__subtitle">{StationsData [SelectStation].id}. {StationsData [SelectStation].station}</div>
            <div class="metro__small">{translateText('player2', 'Оплата')}</div>
            <div class="metro__subtitle price">${priceTicket * Increase}</div>
            <div class="metro__button" on:click={OnBuy}>{translateText('player2', 'Купить билет')}</div>
        </div>
    </div>
    <div class="metro__bottom">
        <div class="box-KeyAnimation" on:click={OnBuy}>
            <div>{translateText('player2', 'Купить билет')}</div>
            <KeyAnimation keyCode={13}>Enter</KeyAnimation>
        </div>
        <KeyAnimation keyCode={37} classData="m-l-26" on:click={OnLeft}><span class="metroicons-left"></span></KeyAnimation>
        <KeyAnimation keyCode={39} classData="m-l-26" on:click={OnRight}><span class="metroicons-right"></span></KeyAnimation>
        <div class="box-KeyAnimation m-l-26" on:click={OnClose}>
            <KeyAnimation keyCode={27}>ESC</KeyAnimation>
            <div>{translateText('player2', 'Выйти')}</div>
        </div>
    </div>
</div>