<script>
    import { translateText } from 'lang'
    import './assets/sass/main.sass'
    import { charGender } from 'store/chars';
    import { executeClient } from 'api/rage'
    import { ItemId } from 'json/itemsInfo.js'
    export let visible;

    const eventItem = ItemId.Giftcoin;

    //gender - true - мужик / false - женский
    let items = [ 
        {title: translateText('player2', 'Обмен на уникальные штаны'), descr: translateText('player2', 'Получить уникальные рождественские штаны!'), price: 99999, gender: true},
        {title: translateText('player2', 'Обмен на уникальный топ'), descr: translateText('player2', 'Получить рандомный рожденственский топ из верхней одежды'), price: 650, gender: true},
        {title: translateText('player2', 'Обмен на уникальный аксессуар'), descr: translateText('player2', 'Получить рандомные леденцы на спину'), price: 1000, gender: true},

        {title: translateText('player2', 'Обмен на уникальные штаны'), descr: translateText('player2', 'Получить уникальные рождественские штаны'), price: 650, gender: false},
        {title: translateText('player2', 'Обмен на уникальный топ'), descr: translateText('player2', 'Получить рандомный рожденственский топ из верхней одежды'), price: 650, gender: false},
        {title: translateText('player2', 'Обмен на уникальный аксессуар'), descr: translateText('player2', 'Получить рандомные леденцы на спину'), price: 1000, gender: false},
    
        {title: 'Обмен на Снегоход!', descr: 'Можно получить уникальный транспорт - Снегоход!', price: 1500, gender: "all"},

    ];

    let valueEC = window.getItemToCount (eventItem);

    let useVisible = visible;
    $: {
        if (visible !== useVisible) {
            useVisible = visible;
            valueEC = window.getItemToCount (eventItem);
        }
    }
    const Bool = (text) => {
        return String(text).toLowerCase() === "true";
    }

    const onBuyItem = (index) => {
        if (items [index].price > window.getItemToCount (eventItem)) {
            valueEC = window.getItemToCount (eventItem);
            window.notificationAdd(4, 9, translateText('player2', 'У Вас нет столько Coins!'), 3000);
            return;
        }
        executeClient ("client.events.buyItem", index);
    }
    
    const UpdateSlot = (json) => {
        json = JSON.parse(json)
        if (json.ItemId == eventItem) {
            valueEC = window.getItemToCount (eventItem);
        }
    }

    window.events.addEvent("cef.events.UpdateSlot", UpdateSlot);

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        // Инициализация инвентаря игрока
        window.events.removeEvent("cef.events.UpdateSlot", UpdateSlot);
    });
</script>


<div class="event">
    <!-- <div class="event__bottom"></div> -->
    <div class="m">
        <div class="event-logo"></div>
         <div class="event-snow"/> 
    </div>
    <div class="event-main">
        <div class="event-header">
        <div class="event-icon-sun"/>
        <div><span>{translateText('player2', 'Баланс')} :</span><span class="yellow">{valueEC} Coin's </span></div>
        <div class="event-icon-sun"/>
        </div>
        <ul class="event-list">
            <div class="event__main_title">{translateText('player2', 'Скучно? Самое время собрать 160 коробок от Санты! Отправляйся на поиски по всему штату и обменивай полученные подарки на крутые призы в этом меню, либо на RedBucks у NPC Санта.')}</div>
            {#each items as item, index}
                {#if item.gender === "all" || Bool($charGender) === item.gender}
                <li>
                    <div class="event-list-box">
                        <span class="title">
                            {item.title}
                        </span>
                        <span class="descr">
                            {item.descr}
                        </span>
                    </div>
                    <div class="event-icon-gift"/>
                    <div class="btn green" on:click={() => onBuyItem (index)}>{item.price} Coin's</div>
                </li>
                {/if}
            {/each}
        </ul>
    </div>
</div>
