<script>
    import { translateText } from 'lang'
    import { accountData, accountIsSession } from 'store/account'
    import './main.sass';
    import { storeSettings } from 'store/settings'
    import { executeClient } from 'api/rage'
    import InputBlock from './input_item.svelte';
    import ListButton from './listbutton.svelte';
    import InputCustom from 'components/input/index.svelte'

    let SettingsList = {
        WalkStyle: {
            type: "WalkStyle",
            title: translateText('settings', 'Основные'),
            categorie: "Main",
            selectId: $storeSettings.WalkStyle,
            name: translateText('settings', 'Стиль походки'),
            desc: translateText('settings', 'Настройка стиля походки, который будет использовать при ходьбе ваш персонаж.'),
            list: [
                translateText('settings', 'Стандартная'),
                translateText('settings', 'Стремительный'),
                translateText('settings', 'Уверенный'),
                translateText('settings', 'Подшофе'),
                translateText('settings', 'Вразвалку'),
                translateText('settings', 'Грустный'),
                translateText('settings', 'Женственный'),
                translateText('settings', 'Громила'),
                translateText('settings', 'Непринужденная'),
                translateText('settings', 'Ковбой'),
                translateText('settings', 'Гэнгстерская'),
                translateText('settings', 'Пританцовывая'),
                translateText('settings', 'Бегунья'),
                translateText('settings', 'Важная'),
                translateText('settings', 'Подвыпивший'),
                translateText('settings', 'Дворник'),
                translateText('settings', 'Женская'),
                translateText('settings', 'Широкий'),
                translateText('settings', 'Женская 2'),
                translateText('settings', 'Широкий 2'),
                translateText('settings', 'Подвыпивший 2'),
                translateText('settings', 'Опасный'),
                translateText('settings', 'Важная'),
                translateText('settings', 'Напуганная'),
                translateText('settings', 'Дерзкая'),
            ]
        },
        FacialEmotion: {
            type: "FacialEmotion",
            categorie: "Main",
            selectId: $storeSettings.FacialEmotion,
            name: translateText('settings', 'Лицевая эмоция'),
            desc: translateText('settings', 'Настройка эмоции на лице, с которой будет ходить ваш персонаж.'),
            list: [
                translateText('settings', 'Стандартная'),
                translateText('settings', 'Презрение'),
                translateText('settings', 'Хмурость'),
                translateText('settings', 'Подшофе'),
                translateText('settings', 'Веселье'),
                translateText('settings', 'Удивление'),
                translateText('settings', 'Злость'),
                translateText('settings', 'Спать'),
                translateText('settings', 'Без сознания'),
                translateText('settings', 'Электрический'),
                translateText('settings', 'Есть'),
                translateText('settings', 'Пить'),
                translateText('settings', 'Дуться'),
                translateText('settings', 'Кашель'),
                translateText('settings', 'Шок 1'),
                translateText('settings', 'Шок 2'),
                translateText('settings', 'Напряженный 1'),
                translateText('settings', 'Напряженный 2'),
                translateText('settings', 'Боль 1'),
                translateText('settings', 'Боль 2'),
                translateText('settings', 'Боль 3'),
                translateText('settings', 'Курить 1'),
                translateText('settings', 'Курить 2'),
                translateText('settings', 'Курить 3'),
            ]
        },
        Deaf: {
            type: "Deaf",
            categorie: "Main",
            toggled: $storeSettings.Deaf,
            name: translateText('settings', 'Я не слышу'),
            desc: translateText('settings', 'Настройка, которая выключает у вас звуки других игроков. Другие игроки видят у вас над головой статус Я не слышу'),
        },
        TagsHead: {
            type: "TagsHead",
            categorie: "Main",
            toggled: $storeSettings.TagsHead,
            name: translateText('settings', 'Теги над головой'),
            desc: translateText('settings', 'Настройка, которая отключает всю информацию над головой у других персонажей.'),
        },
        //hud
        HudToggled: {
            type: "HudToggled",
            title: translateText('settings', 'Настройка худа'),
            categorie: "Hud",
            toggled: $storeSettings.HudToggled,
            name: translateText('settings', 'Худ'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение худа.'),
        },
        HudStats: {
            type: "HudStats",
            categorie: "Hud",
            toggled: $storeSettings.HudStats,
            name: translateText('settings', 'Статистика'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение статистики в худе.'),
        },
        HudSpeed: {
            type: "HudSpeed",
            categorie: "Hud",
            toggled: $storeSettings.HudSpeed,
            name: translateText('settings', 'Спидометр'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение спидометра в худе.'),
        },
        HudLocation: {
            type: "HudLocation",
            categorie: "Hud",
            toggled: $storeSettings.HudLocation,
            name: translateText('settings', 'Локация'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение текущей локации в худе.'),
        },
        HudKey: {
            type: "HudKey",
            categorie: "Hud",
            toggled: $storeSettings.HudKey,
            name: translateText('settings', 'Горячие клавиши'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение горячих клавишей в худе.'),
        },
        HudMap: {
            type: "HudMap",
            categorie: "Hud",
            toggled: $storeSettings.HudMap,
            name: translateText('settings', 'Карта'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение карты в худе.'),
        },
        HudCompass: {
            type: "HudCompass",
            categorie: "Hud",
            toggled: $storeSettings.HudCompass,
            name: translateText('settings', 'Компас'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение компаса в худе.'),
        },
        /*CircleVehicle: {
            type: "CircleVehicle",
            toggled: $storeSettings.CircleVehicle,
            name: "Тип спидометра",
            desc: "",
        },*/
        //Чат
        Widthsize: {
            type: "Widthsize",
            title: "Чат",
            categorie: "Chat",
            name: translateText('settings', 'Длина чата'),
            desc: translateText('settings', 'Настройка, которая отвечает за длину чата в худе.'),
            value: $storeSettings.Widthsize,
            min: 0,
            max: 100,
        },
        ChatOpacity: {
            type: "ChatOpacity",
            categorie: "Chat",
            name: translateText('settings', 'Прозрачность чата'),
            desc: translateText('settings', 'Настройка, которая отвечает за прозрачность чата в худе.'),
            value: $storeSettings.ChatOpacity,
            min: 0,
            max: 100,
        },
        Transition: {
            type: "Transition",
            categorie: "Chat",
            name: translateText('settings', 'Плавность анимации'),
            desc: translateText('settings', 'Настройка, которая отвечает за плавность анимации чата в худе.'),
            value: $storeSettings.Transition,
            min: 0,
            max: 100,
        },
        Pagesize: {
            type: "Pagesize",
            categorie: "Chat",
            name: translateText('settings', 'Кол-во строк'),
            desc: translateText('settings', 'Настройка, которая отвечает за максимальное количество строк чата в худе.'),
            value: $storeSettings.Pagesize,
            min: 5,
            max: 20,
        },
        Fontsize: {
            type: "Fontsize",
            categorie: "Chat",
            name: translateText('settings', 'Размер шрифта'),
            desc: translateText('settings', 'Настройка, которая отвечает за размер шрифта чата в худе.'),
            value: $storeSettings.Fontsize,
            min: 10,
            max: 20,
        },
        Timestamp: {
            type: "Timestamp",
            categorie: "Chat",
            toggled: $storeSettings.Timestamp,
            name: translateText('settings', 'Время'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение времени в чате.'),
        },
        ChatShadow: {
            type: "ChatShadow",
            categorie: "Chat",
            toggled: $storeSettings.ChatShadow,
            name: translateText('settings', 'Обводка чата'),
            desc: translateText('settings', 'Включает обводку в чате.'),
        },
		APunishments: {
            type: "APunishments",
            categorie: "Chat",
            toggled: $storeSettings.APunishments,
            name: translateText('settings', 'Наказания'),
            desc: translateText('settings', 'Настройка, которая отвечает за отображение наказаний в чате.'),
        },
        //Звук
        //
        VolumeInterface: {
            type: "VolumeInterface",
            title: translateText('settings', 'Звук'),
            categorie: "Sound",
            name: translateText('settings', 'Громкость звуков интерфейса'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость звуков интерфейса, например: перетаскивание вещей в инвентаре, открытие инвентаря.'),
            value: $storeSettings.VolumeInterface,
            min: 0,
            max: 100,
        },
        VolumeQuest: {
            type: "VolumeQuest",
            categorie: "Sound",
            name: translateText('settings', 'Громкость квестовых персонажей'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость голоса квестовых персонажей.'),
            value: $storeSettings.VolumeQuest,
            min: 0,
            max: 100,
        },
        VolumeAmbient: {
            type: "VolumeAmbient",
            categorie: "Sound",
            name: translateText('settings', 'Громкость окружения'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость звуков окружения, например: звуки переодевания, звуки еды.'),
            value: $storeSettings.VolumeAmbient,
            min: 0,
            max: 100,
        },
        VolumePhoneRadio: {
            type: "VolumePhoneRadio",
            categorie: "Sound",
            name: translateText('settings', 'Громкость Redage FM'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость звуков радио Redage FM, которое можно включить в телефоне..'),
            value: $storeSettings.VolumePhoneRadio,
            min: 0,
            max: 100,
        },
        //
        VolumeVoice: {
            type: "VolumeVoice",
            categorie: "Sound",
            name: translateText('settings', 'Громкость голосового чата'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость голосового чата (то, как вы слышите других игроков).'),
            value: $storeSettings.VolumeVoice,
            min: 0,
            max: 100,
        },
        VolumeRadio: {
            type: "VolumeRadio",
            categorie: "Sound",
            name: translateText('settings', 'Громкость рации'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость рации (то, как вы слышите других игроков из рации).'),
            value: $storeSettings.VolumeRadio,
            min: 0,
            max: 100,
        },
        VolumeBoombox: {
            type: "VolumeBoombox",
            categorie: "Sound",
            name: translateText('settings', 'Громкость бумбокса'),
            desc: translateText('settings', 'Настройка, которая отвечает за громкость бумбокса.'),
            value: $storeSettings.VolumeBoombox,
            min: 0,
            max: 100,
        },
		FirstMute: {
            type: "FirstMute",
            categorie: "Sound",
            toggled: $storeSettings.FirstMute,
            name: translateText('settings', 'Новички в мьюте'),
            desc: translateText('settings', 'Настройка, которая делает так, чтобы вы не слышали звуки от игроков ниже второго уровня.'),
        },
        //Дополнительно
        DistancePlayer: {
            type: "DistancePlayer",
            title: translateText('settings', 'Дополнительно'),
            categorie: "Sound",
            name: translateText('settings', 'Дистанция прогрузки игроков'),
            desc: translateText('settings', 'Настройка, которая отвечает за дистанцию прогрузки моделей игроков. Может помочь оптимизировать игру на слабых ПК.'),
            value: $storeSettings.DistancePlayer,
            min: 0,
            max: 50,
        },
        DistanceVehicle: {
            type: "DistanceVehicle",
            categorie: "Sound",
            name: translateText('settings', 'Дистанция прогрузки авто'),
            desc: translateText('settings', 'Настройка, которая отвечает за дистанцию прогрузки моделей автомобилей. Может помочь оптимизировать игру на слабых ПК.'),
            value: $storeSettings.DistanceVehicle,
            min: 0,
            max: 50,
        },
        //
        hitPoint: {
            type: "hitPoint",
            categorie: "Pricel",
            title: translateText('settings', 'Отображение урона'),
            name: translateText('settings', 'Отображение исходящего урона'),
            desc: translateText('settings', 'Позволяет видеть циферки в момент нанесения урона по другому персонажу.'),
            toggled: $storeSettings.hitPoint,
        },
        //Прицел
        cPToggled: {
            type: "cPToggled",
            categorie: "Pricel",
            title: translateText('settings', 'Прицел'),
            name: translateText('settings', 'Персональный прицел'),
            desc: translateText('settings', 'Настройка, которая отвечает за включение кастомного прицела.'),
            toggled: $storeSettings.cPToggled,
        },
        cPWidth: {
            type: "cPWidth",
            categorie: "Pricel",
            name: translateText('settings', 'Ширина'),
            desc: translateText('settings', 'Настройка, которая отвечает за ширину кастомного прицела.'),
            value: $storeSettings.cPWidth,
            min: 0,
            max: 36,
        },
        cPGap: {
            type: "cPGap",
            categorie: "Pricel",
            name: translateText('settings', 'Зазор'),
            desc: translateText('settings', 'Настройка, которая отвечает за зазор кастомного прицела.'),
            value: $storeSettings.cPGap,
            min: 0,
            max: 20,
        },
        cPDot: {
            type: "cPDot",
            categorie: "Pricel",
            name: translateText('settings', 'Точка в центре'),
            desc: translateText('settings', 'Настройка, которая включает точку в центре кастомного прицела.'),
            toggled: $storeSettings.cPDot,
        },
        cPThickness: {
            type: "cPThickness",
            categorie: "Pricel",
            name: translateText('settings', 'Толщина'),
            desc: translateText('settings', 'Настройка, отвечает за толщину кастомного прицела.'),
            value: $storeSettings.cPThickness,
            min: 0,
            max: 9,
        },
        cPColorR: {
            type: "cPColorR",
            categorie: "Pricel",
            name: translateText('settings', 'Цвет R'),
            desc: translateText('settings', 'Настройка, отвечает за цвет кастомного прицела.'),
            value: $storeSettings.cPColorR,
            min: 0,
            max: 255,
        },
        cPColorG: {
            type: "cPColorG",
            categorie: "Pricel",
            name: translateText('settings', 'Цвет G'),
            desc: translateText('settings', 'Настройка, отвечает за цвет кастомного прицела.'),
            value: $storeSettings.cPColorG,
            min: 0,
            max: 255,
        },
        cPColorB: {
            type: "cPColorB",
            categorie: "Pricel",
            name: translateText('settings', 'Цвет B'),
            desc: translateText('settings', 'Настройка, отвечает за цвет кастомного прицела.'),
            value: $storeSettings.cPColorB,
            min: 0,
            max: 255,
        },
        cPOpacity: {
            type: "cPOpacity",
            categorie: "Pricel",
            name: translateText('settings', 'Прозрачность'),
            desc: translateText('settings', 'Настройка, отвечает за прозрачность кастомного прицела.'),
            value: $storeSettings.cPOpacity,
            min: 0,
            max: 9,
        },
		cPCheck: {
            type: "cPCheck",
            categorie: "Pricel",
            name: translateText('settings', 'Индикатор при наведении'),
            desc: translateText('settings', 'Настройка, отвечает за индикацию при наведении прицела на игрока.'),
            toggled: $storeSettings.cPCheck,
        },
		//Эффекты
		cEfValue: {
            type: "cEfValue",
            categorie: "Pricel",
            selectId: $storeSettings.cEfValue,
            name: translateText('settings', 'Эффект при стрельбе'),
            desc: translateText('settings', 'Настройка, которая включает кастомный эффект при попадании по другому игроку.'),
            list: [
                translateText('settings', 'Нет'),
                translateText('settings', 'Молнии'),
                translateText('settings', 'Знаки'),
            ]
        },
        notifCount: {
            type: "notifCount",
            categorie: "Hud",
            name: translateText('settings', 'Количество уведомлений'),
            desc: translateText('settings', 'Настройка, которая отвечает за максимальное одновременное количество уведомлений.'),
            value: $storeSettings.notifCount,
            min: 1,
            max: 5,
        },
        
    }

    let SelectCategorie = "Main";

    let UseListButton = -1;
    let isUpdate = false;
    const OnChangeList = (type, change) => {
        SettingsList [type].selectId += change;

        let appearancesList = SettingsList [type].list;
        
        if (SettingsList [type].selectId >= appearancesList.length) SettingsList [type].selectId = 0;
        else if (SettingsList [type].selectId < 0) SettingsList [type].selectId = appearancesList.length - 1;
        isUpdate = true;
    }

    const OnSwitch = (type, change) => {
        SettingsList [type].toggled = !SettingsList [type].toggled;
        isUpdate = true;
        if (type === "HudToggled") {
            SettingsList ["ChatOpacity"].value = 50;
            SettingsList ["HudStats"].toggled = 
            SettingsList ["HudSpeed"].toggled = 
            SettingsList ["HudOnline"].toggled = 
            SettingsList ["HudLocation"].toggled = 
            SettingsList ["HudKey"].toggled = 
            SettingsList ["HudMap"].toggled = 
            SettingsList ["HudCompass"].toggled = SettingsList ["HudToggled"].toggled;

            
        } else if (type === "HudStats" ||
                    type === "HudSpeed" ||
                    type === "HudOnline" ||
                    type === "HudLocation" ||
                    type === "HudKey" ||
                    type === "HudMap" ||
                    type === "HudCompass"||
                    type === "ChatOpacity") {
            SettingsList ["HudToggled"].toggled = true;            
        }
    }

    const OnProgress = (type, value) => {
        SettingsList [type].value = value;
        isUpdate = true;
    }

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        SaveData ();
    });
    export let visible;
    $: {
        if (!visible && isUpdate) {
            SaveData ();
        }
    }

    let saveDataDefault = {};
    storeSettings.subscribe((value) => {
        if (saveDataDefault !== value) {
            saveDataDefault = value;
            
            for (let key in saveDataDefault) {
                if (SettingsList [key]) {
                    if (SettingsList [key].list !== undefined && SettingsList [key].selectId !== undefined) SettingsList [key].selectId = saveDataDefault [key];
                    else if (SettingsList [key].toggled !== undefined) SettingsList [key].toggled = saveDataDefault [key];
                    else if (SettingsList [key].min !== undefined && SettingsList [key].max !== undefined) SettingsList [key].value = saveDataDefault [key];
                }
            }
        }
    });

    const SaveData = () => {
        let saveData = {};
        Object.values (SettingsList).forEach((item) => {
            if (item.list !== undefined && item.selectId !== undefined) saveData[item.type] = item.selectId;
            else if (item.toggled !== undefined) saveData[item.type] = item.toggled;
            else if (item.min !== undefined && item.max !== undefined) saveData[item.type] = item.value;
        })
        executeClient("chatconfig", JSON.stringify (saveData));
    }

    const onDefaultButton = () => {
        const DefaultAllAccess = {
            Timestamp: false, // 0
            ChatShadow: true,
			APunishments: false, // 0
            Fontsize: 16, // 16
            ChatOpacity: 50, // 100
            Chatalpha: 1, // 100            
            Pagesize: 10, // 10
            Widthsize: 100, // 50
            Transition: 0, // 0
            WalkStyle: 0, // 0
            FacialEmotion: 0, // 0
            Deaf: false, // 0
            TagsHead: true, // 0
            HudToggled: true, // 0
            HudStats: true, // 0
            HudSpeed: true, // 0
            HudOnline: true, // 0
            HudLocation: true, // 0
            HudKey: true, // 0
            HudMap: true, // 0
            VolumeInterface: 100, // 100
            VolumeQuest: 50, // 100
            VolumeAmbient: 50, // 100
            VolumePhoneRadio: 50, // 100
            VolumeVoice: 100, // 100
            VolumeRadio: 70,
            VolumeBoombox: 70,
			FirstMute: false, // 0
            DistancePlayer: 50, // 100
            DistanceVehicle: 50, // 100
            
            cPToggled: false,
            cPWidth: 2,
            cPGap: 2,
            cPDot: true,
            cPThickness: 0,
            cPGtaCross: true,
            cPColorR: 255,
            cPColorG: 255,
            cPColorB: 255,
            cPOpacity: 9,
            cPCheck: true,
            CircleVehicle: false,
			
			cEfValue: 0,
            
            notifCount: 2,

            hitPoint: false
        }

        for (let key in DefaultAllAccess) {
            if (SettingsList [key]) {
                if (SettingsList [key].list !== undefined && SettingsList [key].selectId !== undefined) SettingsList [key].selectId = DefaultAllAccess [key];
                else if (SettingsList [key].toggled !== undefined) SettingsList [key].toggled = DefaultAllAccess [key];
                else if (SettingsList [key].min !== undefined && SettingsList [key].max !== undefined) SettingsList [key].value = DefaultAllAccess [key];
            }
        }
    }
    
    const handleKeyDown = (event) => {
        const { keyCode } = event;
        if (keyCode !== 9) return;
        event.preventDefault();
    }

    const OnSession = () => {
        if (!window.loaderData.delay ("OnSession", 5))
            return;

        executeClient("client.session.update");
    }

    let changePass = "";


    import { validate } from 'api/validation';

    let confirmEmain = $accountData.Email;

    const onConfirmEmain = () => {
        let check;

        check = validate("email", confirmEmain);

        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        if (!window.loaderData.delay ("onConfirmEmain", 1.5))
            return;

        executeClient("client.email.confirm", confirmEmain);

    }

</script>

<svelte:window on:keydown={handleKeyDown}/>

<div id ="sound__settings">
    <div class="settings__nav">
        <div class="settings__title">{translateText('settings', 'Основные настройки')}</div>
        <div class="settings__subtitle" on:click={() => SelectCategorie = "Main"} class:active={SelectCategorie == "Main"}>— {translateText('settings', 'Настройки персонажа')}</div>
        <div class="settings__subtitle" on:click={() => SelectCategorie = "Hud"} class:active={SelectCategorie == "Hud"}>— {translateText('settings', 'Настройки худа')}</div>
        <div class="settings__subtitle" on:click={() => SelectCategorie = "Chat"} class:active={SelectCategorie == "Chat"}>— {translateText('settings', 'Настройки чата')}</div>
        <div class="settings__subtitle" on:click={() => SelectCategorie = "Sound"} class:active={SelectCategorie == "Sound"}>— {translateText('settings', 'Настройки игры и звуков')}</div>
        <div class="settings__title">{translateText('settings', 'Дополнительные настройки')}</div>
        <div class="settings__subtitle" on:click={() => SelectCategorie = "Pricel"} class:active={SelectCategorie == "Pricel"}>— {translateText('settings', 'Настройки прицела')}</div>
        <div class="settings__subtitle" on:click={() => SelectCategorie = "Acc"} class:active={SelectCategorie == "Acc"}>— {translateText('settings', 'Настройки аккаунта')}</div>
    </div>
    <div class="settings__background">
        <div class="settings__header">
            <span class="settings__icon"></span>
            <div class="box-column">
                <div class="settings__header_title">{translateText('settings', 'Настройки игры')}</div>
                <div class="settings__header_subtitle" on:click={onDefaultButton}>{translateText('settings', 'Сбросить по умолчанию')}</div>
            </div>
        </div>
        <div class="sound__scroll">
            {#if SelectCategorie == "Acc"}
                <div class="sound__element">
                    <div class="sound__description">
                        <div class="sound__title">{translateText('settings', 'Сохранение сессии')}</div>
                        <div class="sound__text_small">{translateText('settings', 'Позволяет входить в игру автоматически, без ввода пароля.')}</div>
                    </div>
                    <div class="sound__input-block switch-box">
                        <label class="switch" on:click={OnSession}>
                            <input type="checkbox" checked={$accountIsSession} disabled >
                            <span class="slider round"></span>
                        </label>
                    </div>
                </div>
                {#if !/[0-9]{8,11}[.][0-9]{8,11}/i.test($accountData.Ga)}
                <div class="sound__element">
                    <div class="sound__description">
                        <div class="sound__title">{translateText('settings', 'Подтверждение почты')}</div>
                        <div class="sound__text_small">{translateText('settings', 'Подтвердите почту, чтобы всегда иметь возможность восстановить пароль от аккаунта.')}</div>
                    </div>
                    <div class="sound__input-block box-center">
                        <div class="box-flex box-center">
                            <InputCustom placeholder="Введите почту.." type="text" settingsClass={true} settingsMargin={true} setValue={(value) => confirmEmain = value} value={confirmEmain}/>
                            <div class="main__button_square box-center" on:click={onConfirmEmain}>
                                <span class="auth-arrow"/>
                            </div>
                        </div>
                    </div>
                </div>
                {/if}
            {:else}
                {#each Object.values (SettingsList) as item, index}
                    {#if item.categorie == SelectCategorie}
                        {#if item.title}<div class="sound__header">{item.title}</div>{/if}
                        <div class="sound__element">
                            <div class="sound__description">
                                {#if item.type === "cPColorR" || item.type === "cPColorG" || item.type === "cPColorB"}
                                <div class="sound__title">{item.name} ({item.value})</div>
                                {:else}
                                <div class="sound__title">{item.name}</div>
                                {/if}
                                <div class="sound__text_small">{item.desc}</div>
                            </div>
                            {#if item.list !== undefined && item.selectId !== undefined}
                                <div class="sound__input-block">
                                    <ListButton
                                        name={item.list [item.selectId]}
                                        on:click={e => UseListButton = index}
                                        active={UseListButton === index}
                                        onChange={(change) => OnChangeList (item.type, change)} />
                                </div>
                            {:else if item.toggled !== undefined}
                                <div class="sound__input-block switch-box">
                                    <label class="switch" on:click={() => OnSwitch (item.type)}>
                                        <input type="checkbox" checked={item.toggled} disabled >
                                        <span class="slider round"></span>
                                    </label>
                                </div>
                            {:else if item.min !== undefined && item.max !== undefined}
                                <InputBlock
                                    id={"index_" + index}
                                    leftText="0"
                                    rightText="100"
                                    step={1}
                                    min={item.min}
                                    max={item.max}
                                    value={item.value}
                                    callback={newvalue => OnProgress (item.type, newvalue)} />

                            {/if}
                        </div>
                    {/if}
                {/each}    
            {/if}        
        </div>
    </div>
    <div class="settings__nav"></div>
</div>