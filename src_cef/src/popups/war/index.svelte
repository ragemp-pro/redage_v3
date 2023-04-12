<script>
    import { executeClient } from "api/rage";
    import './main.sass'
    import { fly } from 'svelte/transition';
    import { translateText } from 'lang'

    import { typeBattle, composition, weaponsCategory } from './data'

    let selectTypeBattle = 0;
    
    const onSelectTypeBattle = (count) => {
        let value = selectTypeBattle;
        value += count;

        if (0 > value)
            value = typeBattle.length - 1;
        else if (value >= typeBattle.length)
            value = 0;

        selectTypeBattle = value;
    }

    //
    let selectComposition = 0;

    const onSelectComposition = (count) => {
        let value = selectComposition;
        value += count;

        if (0 > value)
            value = composition.length - 1;
        else if (value >= composition.length)
            value = 0;

        selectComposition = value;
    }

    //
    let selectWeaponsCategory = 0;

    
    const onSelectWeaponsCategory = (count) => {
        let value = selectWeaponsCategory;
        value += count;

        if (0 > value)
            value = weaponsCategory.length - 1;
        else if (value >= weaponsCategory.length)
            value = 0;

        selectWeaponsCategory = value;
    }

    let selectMap = 0;

    export let popupData;


    $: if (popupData && typeof popupData === "string") 
        popupData = JSON.parse (popupData)
    

    if (!popupData) 
        popupData = {
            maps: [
                "Домбас"
            ],
            title: "",
            owner: "",
            minHour: 14
        }
    
    const onSelectMap = (count) => {
        let value = selectMap;
        value += count;

        if (0 > value)
            value = popupData.maps.length - 1;
        else if (value >= popupData.maps.length)
            value = 0;

        selectMap = value;
    }

    //
    let selectDay = 0;
    const days = [
        "Сегодня",
        "Завтра"
    ]
    const onSelectDay = (count) => {
        let value = selectDay;
        value += count;

        if (0 > value)
            value = days.length - 1;
        else if (value >= days.length)
            value = 0;

        selectDay = value;
    }

    //
    let selectMin = 0;
    const mins = [
        0,
        15,
        30,
        45
    ]
    const onSelectMin = (count) => {
        let value = selectMin;
        value += count;

        if (0 > value)
            value = mins.length - 1;
        else if (value >= mins.length)
            value = 0;

        selectMin = value;
    }
    //
    const minHour = 14;
    const maxHour = 23;
    let selectHour = minHour;
    const onSelectHour = (count) => {
        let value = selectHour;
        value += count;

        if (selectDay === 0) {
            if (minHour > value)
                value = maxHour;
            else if (value > maxHour)
                value = popupData.minHour;
        } else {
            if (minHour > value)
                value = maxHour;
            else if (value > maxHour)
                value = minHour;
        }

        selectHour = value;
    }

    //

    const onClose = () => {
        executeClient ('client.closeWar')
    }

    const onWar = () => {
        executeClient ('client.war', selectTypeBattle, selectComposition, selectWeaponsCategory, selectDay, selectHour, mins[selectMin])
    }
</script>

<div class="popup__war">
    <div class="box-column">
        <div class="box-flex">
            <div class="fractionsicon-squads popup__war_icon"></div>
            <div class="box-column">
                <div class="popup__war_title">Объявление войны</div>
                <div class="popup__war_subtitle">Выберите параметры и начните захват</div>
            </div>
        </div>
        <div class="popup__war_smalltitle mt-40">{popupData.title}</div>
        <div class="popup__war_subtitle">Название объекта</div>
        <div class="popup__war_smalltitle">{popupData.owner}</div>
        <div class="popup__war_subtitle">Владелец</div>
    </div>
    <div class="box-column">
        <div class="box-between mb-23">
            <div class="box-flex">
                <div class="fractionsicon-settings popup__war_iconsmall"></div>
                <div class="box-column">
                    <div class="popup__war_headtitle">Настройка войны за бизнес</div>
                    <div class="popup__war_headsubtitle">Здесь вы можете настроить все параметры войны за бизнес</div>
                </div>
            </div>
            <div class="popup__war_smallbutton" on:click={onClose}>Назад</div>
       </div>
       <div class="popup__war_element">
            <div class="box-column">
                <div class="popup__element_title">Тип битвы</div>
                <div class="popup__element_subtitle">Выберите нужный тип битвы</div>
            </div>
            <div class="popup__war_selector">
                <div class="popup__element_button" on:click={() => onSelectTypeBattle (-1)}>&lt;</div>
                <div>{typeBattle[selectTypeBattle]}</div>
                <div class="popup__element_button" on:click={() => onSelectTypeBattle (1)}>&gt;</div>
            </div>
        </div>
       <div class="popup__war_element">
            <div class="box-column">
               <div class="popup__element_title">Состав участников битвы</div>
               <div class="popup__element_subtitle">Выберите подходящее количество</div>
            </div>
            <div class="popup__war_selector">
                <div class="popup__element_button" on:click={() => onSelectComposition (-1)}>&lt;</div>
                <div>{composition[selectComposition]}</div>
                <div class="popup__element_button" on:click={() => onSelectComposition (1)}>&gt;</div>
            </div>
       </div>
       <div class="popup__war_element">
            <div class="box-column">
                <div class="popup__element_title">Тип оружия</div>
                <div class="popup__element_subtitle">Выберите нужный тип оружия</div>
            </div>
            <div class="popup__war_selector">
                <div class="popup__element_button" on:click={() => onSelectWeaponsCategory (-1)}>&lt;</div>
                <div>{weaponsCategory[selectWeaponsCategory]}</div>
                <div class="popup__element_button" on:click={() => onSelectWeaponsCategory (1)}>&gt;</div>
            </div>
        </div>
        <!--<div class="popup__war_element">
            <div class="box-column">
                <div class="popup__element_title">Место проведения битвы</div>
                <div class="popup__element_subtitle">Выберите место для войны за объект</div>
            </div>
            <div class="popup__war_selector">
                <div class="popup__element_button" on:click={() => onSelectMap (-1)}>&lt;</div>
                <div>{popupData.maps[selectMap]}</div>
                <div class="popup__element_button" on:click={() => onSelectMap (1)}>&gt;</div>
            </div>
        </div>-->
        <div class="popup__war_element">
            <div class="box-column">
                <div class="popup__element_title">Дата проведения битвы</div>
                <div class="popup__element_subtitle">Выберите дату проведения войны за территорию</div>
            </div>
            <div class="popup__war_selector w-340">
                <!--<div class="popup__war_small">дд/мм/чч</div>-->
                <div class="box-flex">
                    <div class="popup__war_left" on:click={() => onSelectDay (-1)}>&lt;</div>
                    <div class="popup__war_date">{days [selectDay]}</div>
                    <div class="popup__war_right mr-10" on:click={() => onSelectDay (1)}>&gt;</div>
                    <div class="popup__war_left" on:click={() => onSelectHour (-1)}>&lt;</div>
                    <div class="popup__war_date">{selectHour}</div>
                    <div class="popup__war_right mr-10" on:click={() => onSelectHour (1)}>&gt;</div>
                    <div class="popup__war_left" on:click={() => onSelectMin (-1)}>&lt;</div>
                    <div class="popup__war_date">{mins [selectMin]}</div>
                    <div class="popup__war_right" on:click={() => onSelectMin (1)}>&gt;</div>
                </div>
            </div>
        </div>
        <div class="popup__war_button" on:click={onWar}>Забить войну</div>
    </div>
    <div class="popup__war_image"></div>
</div>