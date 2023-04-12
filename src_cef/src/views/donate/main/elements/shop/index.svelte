<script>
    import { translateText } from 'lang'
    import { format } from 'api/formatter'
    import { serverDonatMultiplier } from 'store/server'
    import { validate } from 'api/validation';
    let selectIndex = 0;

    export let SetPopup;
    const getDonate = (text) => {
        if (text < 0) text = 0;
        else if (text > 999999) text = 999999;
        let tallage = 0;
        if ($serverDonatMultiplier > 1) {
            text = text * $serverDonatMultiplier;
        } else {
            if (text >= 20000) {
                tallage = 50;
            } else if (text >= 10000) {
                tallage = 30;
            } else if (text >= 3000) {
                tallage = 20;
            } else if (text >= 1000) {
                tallage = 10;
            }
        }

        return `${Math.round(text) + Math.round(text / 100 * tallage)}`;
    }

    import ImgMenuTop_up from './img/top_up.png';
    import ImgMenuChar from './img/char.png';

    import ImgBuy from './img/buy.svg';
    import ImgConversion from './img/conversion.svg';
    import ImgSapper from './img/sapper.png';

    import ImgP_1 from './img/p_1.svg';
    import ImgP_2 from './img/p_2.svg';
    import ImgP_3 from './img/p_3.svg';
    import SimPhoto from './img/sim.png';
    import NumberPhoto from './img/number.png';
    
    /*import ImgClM_1 from './img/clothes/male/1.png';
    import ImgClM_2 from './img/clothes/male/2.png';
    import ImgClM_3 from './img/clothes/male/3.png';
    import ImgClM_4 from './img/clothes/male/4.png';
    import ImgClM_5 from './img/clothes/male/5.png';
    import ImgClM_6 from './img/clothes/male/6.png';
    import ImgClM_7 from './img/clothes/male/7.png';
    import ImgClM_8 from './img/clothes/male/8.png';
    import ImgClM_9 from './img/clothes/male/9.png';
    import ImgClM_10 from './img/clothes/male/10.png';
    import ImgClM_11 from './img/clothes/male/11.png';
    import ImgClM_12 from './img/clothes/male/12.png';
    import ImgClM_13 from './img/clothes/male/13.png';
    import ImgClM_14 from './img/clothes/male/14.png';
    import ImgClM_15 from './img/clothes/male/15.png';
    import ImgClM_16 from './img/clothes/male/16.png';
    import ImgClM_17 from './img/clothes/male/17.png';
    import ImgClM_18 from './img/clothes/male/18.png';
    import ImgClM_19 from './img/clothes/male/19.png';
    import ImgClM_20 from './img/clothes/male/20.png';
    import ImgClM_21 from './img/clothes/male/21.png';
    import ImgClM_22 from './img/clothes/male/22.png';
    import ImgClM_23 from './img/clothes/male/23.png';

    import ImgClF_1 from './img/clothes/female/1.png';
    import ImgClF_2 from './img/clothes/female/2.png';
    import ImgClF_3 from './img/clothes/female/3.png';
    import ImgClF_4 from './img/clothes/female/4.png';
    import ImgClF_5 from './img/clothes/female/5.png';
    import ImgClF_6 from './img/clothes/female/6.png';
    import ImgClF_7 from './img/clothes/female/7.png';
    import ImgClF_8 from './img/clothes/female/8.png';
    import ImgClF_9 from './img/clothes/female/9.png';
    import ImgClF_10 from './img/clothes/female/10.png';
    import ImgClF_17 from './img/clothes/female/17.png';
    import ImgClF_18 from './img/clothes/female/18.png';
    import ImgClF_19 from './img/clothes/female/19.png';
    import ImgClF_20 from './img/clothes/female/20.png';
    import ImgClF_21 from './img/clothes/female/21.png';
    import ImgClF_22 from './img/clothes/female/22.png';
    import ImgClF_23 from './img/clothes/female/23.png';*/

    const shopList = [
        {
            title: "Основное",
            desc: "",
            function: "onSelectPrice",
            img: ImgBuy,
            list: [
                /*{
                    name: "Пополнить",
                    desc: "",
                    btnName: "Купить",
                    img: ImgBuy,
                },*/
                {
                    name: "Конвертация",
                    btnName: "Конвертировать",
                    desc: "",
                    img: ImgConversion,
                },
                {
                    name: "Игра сапер",
                    btnName: "Играть",
                    desc: "",
                    img: ImgSapper,
                },
            ]
        },
        {
            title: "Персонаж",
            desc: "",
            function: "onSelectP",
            img: ImgMenuChar,
            list: [
                {
                    id: 0,
                    isName: true,
                    name: "Сменить имя",
                    desc: "",
                    text: `Позволяет один раз сменить имя одного
                            Вашего персонажа.
                            После смены имени персонаж забудет о совершенных ранее рукопожатиях,
                            при этом инвентарь и статистика персонажа никак не изменятся. Только никнейм.`,
                    img: ImgP_1,
                    price: 800,
                },
                {
                    id: 1,
                    name: "Сменить внешность",
                    desc: "",
                    text: `После оплаты этой функции,
                            Ваш персонаж будет отправлен в редактор внешности (как при создании персонажа),
                            где Вы сможете заново настроить его внешность.
                            Содержимое инвентаря и татуировки останутся.`,
                    img: ImgP_2,
                    price: 1000,
                },
                {
                    id: 2,
                    name: "Снятие варна",
                    desc: "",
                    text: `Warn - предупреждение от Администрации. Эта донат-функция снимает только 1 warn.
                        В случае, если у вас 2 warn'a, то для полного снятия потребуется дважды оплатить "Снятие WARN".
                        Если у вас накопится 3 warn'a одновременно - персонаж будет автоматически заблокирован на 30 дней.`,
                    img: ImgP_3,
                    price: 1000,
                },
                {
                    id: 3,
                    isNumber: true,
                    name: "Покупка номера",
                    desc: "",
                    text: `Долгожданная система покупки номера для автомобиля!
                        Соберите свой уникальный номер из букв и/или цифр и пусть все вокруг завидуют...
                        Номер на авто выдаётся как предмет в инвентарь.`,
                    img: NumberPhoto,
                    btnName: "Купить"
                },
                {
                    id: 4,
                    isSim: true,
                    name: "Покупка SIM",
                    desc: "",
                    text: `Долгожданная система покупки номера для телефона!
                        Соберите свой уникальный номер из любых цифр и пусть все вокруг завидуют...
                        Номер на авто выдаётся как предмет в инвентарь.`,
                    img: SimPhoto,
                    btnName: "Купить"
                },
            ]
        },
        /*{
            title: "Мужская одежда",
            desc: "",
            function: "onSelectC",
            img: ImgClM_11,
            list: [
                {
                    id: 0,
                    name: "Борода",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_2,
                    price: 30000,
                },
                {
                    id: 1,
                    name: "Маска крысы",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_17,
                    price: 1000,
                },
                {
                    id: 2,
                    name: "Неоновая маска",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_18,
                    price: 2000,
                },
                {
                    id: 3,
                    name: "Маска Marshmallow",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_5,
                    price: 35000,
                },
                {
                    id: 4,
                    name: "Неоновые очки",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_19,
                    price: 10000,
                },
                {
                    id: 5,
                    name: "Неоновый шлем",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_20,
                    price: 30000,
                },
                {
                    id: 6,
                    name: "Фуражка адмирала",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_21,
                    price: 1500,
                },
                {
                    id: 7,
                    name: "Модная повязка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_6,
                    price: 4200,
                },
                {
                    id: 8,
                    name: "Неоновые штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_23,
                    price: 25000,
                },
                {
                    id: 9,
                    name: "Яркие штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_13,
                    price: 20000,
                },
                {
                    id: 10,
                    name: "Штаны с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_16,
                    price: 20000,
                },
                {
                    id: 11,
                    name: "Шорты Supreme",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_1,
                    price: 5000,
                },
                {
                    id: 12,
                    name: "Штаны GUCCI",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_12,
                    price: 30000,
                },
                {
                    id: 13,
                    name: "Неоновая обувь",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_22,
                    price: 25000,
                },
                {
                    id: 14,
                    name: "Ласты",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_4,
                    price: 3000,
                },
                {
                    id: 15,
                    name: "Неоновый верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_10,
                    price: 15000,
                },
                {
                    id: 16,
                    name: "Яркий верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_14,
                    price: 10000,
                },
                {
                    id: 17,
                    name: "Верх с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_15,
                    price: 10000,
                },
                {
                    id: 18,
                    name: "Толстовка GUCCI",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_11,
                    price: 30000,
                },
                {
                    id: 19,
                    name: "Модная футболка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClM_7,
                    price: 5000,
                },
            ]
        },
        {
            title: "Женская одежда",
            desc: "",
            function: "onSelectC",
            img: ImgClF_1,
            list: [
                {
                    id: 20,
                    name: "Маска крысы",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_17,
                    price: 1000,
                },
                {
                    id: 21,
                    name: "Неоновая маска",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_18,
                    price: 2000,
                },
                {
                    id: 22,
                    name: "Маска Marshmallow",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_7,
                    price: 35000,
                },
                {
                    id: 23,
                    name: "Неоновые очки",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_19,
                    price: 10000,
                },
                {
                    id: 24,
                    name: "Неоновый шлем",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_20,
                    price: 30000,
                },
                {
                    id: 25,
                    name: "Фуражка адмирала",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_21,
                    price: 1500,
                },
                {
                    id: 26,
                    name: "Неоновые штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_2,
                    price: 25000,
                },
                {
                    id: 27,
                    name: "Яркие штаны",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_5,
                    price: 20000,
                },
                {
                    id: 28,
                    name: "Штаны с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_10,
                    price: 20000,
                },
                {
                    id: 29,
                    name: "Неоновая обувь",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_22,
                    price: 10000,
                },
                {
                    id: 30,
                    name: "Ласты",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_8,
                    price: 3000,
                },
                {
                    id: 31,
                    name: "Неоновый верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_3,
                    price: 15000,
                },
                {
                    id: 32,
                    name: "Яркий верх",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_6,
                    price: 10000,
                },
                {
                    id: 33,
                    name: "Верх с подсветкой",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_9,
                    price: 10000,
                },
                {
                    id: 34,
                    name: "Модная футболка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_1,
                    price: 2000,
                },
                {
                    id: 35,
                    name: "Пошлая футболка",
                    desc: "",
                    text: `Одежда`,
                    img: ImgClF_4,
                    price: 4000,
                },
            ]
        }*/
    ];

    let
        FirstName = "",
        LastName = "";
    const onToServer = (item) => {
        switch (shopList[selectIndex].function) {
            case "onSelectPrice":
                if (item.btnName === "Купить") SetPopup ("PopupPayment", 0);
                else if (item.btnName === "Конвертировать") SetPopup ("PopupChange");
                else window.router.setView('DonateSapper');
                break;
            case "onSelectP":
                if (item.isNumber) {
                    SetPopup ("PopupNomer");
                    return;
                }
                if (item.isSim) {
                    SetPopup ("PopupSim");
                    return;
                }

                if (item.isName) {
                    let check;

                    check = validate("name", FirstName);
                    if(!check.valid) {
                        window.notificationAdd(4, 9, check.text, 3000);
                        return;
                    }

                    check = validate("surname", LastName);
                    if(!check.valid) {
                        window.notificationAdd(4, 9, check.text, 3000);
                        return;
                    }
                    item.isName = `${FirstName}_${LastName}`
                    item.text = `Вы действительно хотите сменить ник на ${item.isName}`
                }
                SetPopup ("PopupPPopup", item);
                break;
            case "onSelectC":
                SetPopup ("PopupCPopup", item);
                break;
        }
    }
    const onSelectPrice = (item) => {
        if (!item.price) SetPopup ("PopupPayment", 0);
        else SetPopup ("PopupPayment", item.priceReal);
    }
</script>


<div id="newdonate__shop">
    <div class="shop-elements">

        {#each shopList[selectIndex].list as item, index}
        <div class="shop-element">
            {#if item.icon}
            <div class="shop-element__icon">
                <span class="{item.icon} element__icon" />
            </div>
            {:else}
            <div class="star-img" style="background-image: url({item.img})" />
            {/if}
            <div class="shop-element__info">
                <!--<div class="shop-element__condition">До 3 уровня</div>-->
                <div class="shop-element__title">{item.name}</div>
                {#if item.isName}
                <input class="shop-element__input" placeholder="Имя" type="text" bind:value={FirstName} >
                <input class="shop-element__input" placeholder="Фамилия" type="text" bind:value={LastName}>
                {:else}
                <div class="shop-element__paragraph">{item.desc}</div>
                {/if}
                <div class="shop-element__button-box">
                    <div class="newdonate__button_small shop-element__button" on:click={() => onToServer (item)}>
                        {#if item.btnName}
                        <div class="newdonate__button-text">{item.btnName}</div>
                        {:else}
                        <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", item.price)} RB</div>
                        {/if}
                    </div>
                    
                </div>
            </div>
        </div>
        {/each}
    </div>
    <div class="shop-categorie">
        {#each shopList as item, index}        
        <div class="shop-categorie__element" class:active={selectIndex === index} on:click={() => selectIndex = index}>
            <div class="shop-categorie__info">
                <div class="shop-categorie__checkbox">
                    <div class="shop-categorie__checkbox_active"/>
                </div>
                <div class="shop-element__title">{item.title}</div>
                <div class="shop-element__paragraph">{item.desc}</div>
            </div>
            <div class="star-img" style="background-image: url({item.img})"/>
        </div>
        {/each}
    </div>
</div>