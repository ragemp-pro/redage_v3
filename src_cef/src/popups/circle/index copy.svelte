<script>
    import  './assets/css/iconscircle.css';
    import  './assets/css/circle.sass';
    import { charFractionID, charOrganizationID } from 'store/chars'
    import { executeClient } from 'api/rage'
    export let popupData;
    const    
        prefix = "circle-",
        categoryData = {
            "Игрок":
            [
                ["sell", "offer", "fraction", "documents", "heal", "house", "paired_animations", "family"],
            ],
            "Документы":
            [
                ["passport", "licenses", "idcard", "badge", "lspdbadge", "fibbadge"],
            ],
			"Взаимодействия":
            [
                [ "handshake", "tinter", "givemoney", "tradehouse", "tradebiz", "tradecar", "vmuted"/*, "whisper"*/ ],
            ],
			"Парные анимации":
            [
                [ "embrace", "kiss", "paired_five", "paired_slap", "carry_0", "carry_1", "carry_2", "carry_3" ],
            ],
            "Машина":
            [
                ["hood", "trunk", "doors", "carinv", "trunkAction", "ticketveh", "breaking_trunk", "veh_fix"],
            ],
            "Взаимодействие с багажником":
            [
                [ "intrunk", "fromtrunk"],
            ],
            "В машине":
            [
                ["belt", "hood", "trunk", "doors", "streetrace"],
            ],
            "Недвижимость":
            [
                ["sellcar", "sellhouse", "roommate", "invitehouse"],
            ],
            "Фракция":
            [
                [],
                ["leadaway", "handsup", "rob", "robguns", "pocket"],
                ["leadaway", "handsup", "rob", "robguns", "pocket"],
                ["leadaway", "handsup", "rob", "robguns", "pocket"],
                ["leadaway", "handsup", "rob", "robguns", "pocket"],
                ["leadaway", "handsup", "rob", "robguns", "pocket"],
                ["leadaway", "search"],
                ["leadaway", "search", "takegun", "takeillegal", "takemask", "ticket"],
                ["sellkit", "offerheal"],
                ["leadaway", "search", "takegun", "takeillegal", "takemask"],
                ["leadaway", "pocket", "handsup", "rob", "robguns"],
                ["leadaway", "pocket", "handsup", "rob", "robguns"],
                ["leadaway", "pocket", "handsup", "rob", "robguns"],
                ["leadaway", "pocket", "handsup", "rob", "robguns"],
                ["leadaway", "search", "takegun"],
                [],
                ["leadaway", "rob", "robguns", "pocket"],
		        ["leadaway", "search", "pocket", "takemask"],
                ["leadaway", "search", "takegun", "takeillegal", "takemask", "ticket"],
            ],
			"Семья":
            [
                ["handsup", "rob", "robguns", "pocket", "leadaway"],
            ],
            "Кальян":
            [
                ["use_hookah", "take_hookah"],
            ],
            "Лифт 1":
            [
                ["f_lift_0", "f_lift_1", "f_lift_2", "f_lift_3"],
            ],
            "Лифт 2":
            [
                ["s_lift_0", "s_lift_1", "s_lift_2", "s_lift_3", "s_lift_4"],
            ],
            "Лифт":
            [
                ["c_lift_0", "c_lift_1"],
            ],
            "Лифт правительства":
            [
                ["gov_lift_1", "gov_lift_3", "gov_lift_4"],
            ],
            "Открыть планшет":
            [
                ["fraction_table", "org_table"]
            ],
            "Покинуть фракцию/семью":
            [
                ["leave_fraction", "leave_org"]
            ],
            "Test":
            [
                ["leave_fraction", "leave_org", "leave_org", "leave_org", "leave_org", "leave_org", "leave_org"]
            ]
        },
        categoryDesc = {
            "veh_fix": "Починить транспорт",
            "breaking_trunk": "Взломать транспорт",
            "belt": "Ремень безопасности",
            "sell": "Взаимодействия",
            "paired_animations": "Парные анимации",
            "whisper": "Шептаться",
            "intrunk": "Залезть в багажник",
            "trunkAction": "Багажник",
            "fromtrunk": "Выкинуть из багажника",
            "tradehouse": "Обмен недвижимостью",
            "tradebiz": "Обмен бизнесами",
            "tradecar": "Обмен машинами",
            "streetrace": "Уличная гонка",

            "handshake": "Пожать руку",
            "licenses": "Показать лицензии",
            "documents": "Документы",
            "idcard": "Показать ID-карту",
            "badge": "Показать удостоверение",
            "lspdbadge": "Посмотреть значок",
            "fibbadge": "Посмотреть бейджик",
            "carinv":"Инвентарь",
            "doors":"Открыть/Закрыть двери",
            "fraction":"Фракция",
            "family":"Семья",
            "offer":"Предложить обмен",
			"givemoney":"Передать деньги",
            "heal":"Вылечить",
            "hood":"Открыть/Закрыть капот",
            "leadaway":"Вести за собой",
            "offerheal":"Предложить лечение",
            "passport":"Показать паспорт",
            "search":"Обыскать",
            "sellkit":"Продать аптечку",
            "takegun":"Изъять оружие",
            "takeillegal":"Изъять нелегал",
            "trunk":"Открыть/Закрыть багажник",
            "pocket": "Надеть/снять мешок",
            "takemask": "Сорвать маску/мешок",
            "handsup": "Заставить поднять руки",
            "rob": "Ограбить",
            "robguns": "Украсть оружие",
            "house": "Недвижимость",
            "ticket": "Выписать штраф",
            "ticketveh": "Выписать штраф",

            "sellcar": "Продать машину",
            "sellhouse": "Продать недвижимость",
            "roommate": "Заселить в дом",
            "invitehouse": "Пригласить в дом",

            "embrace" : "Обнять",
            "kiss" : "Поцеловать",
            "paired_five" : "Дать пять",
            "paired_slap" : "Дать пощечину",
            "carry_0" : "Взять на руки",
            "carry_1" : "Закинуть на шею",
            "carry_2" : "Закинуть на плечо",
            "carry_3" : "Взять в заложники",

			"tinter": "Повторить анимацию",

            "use_hookah": "Использовать кальян",
            "take_hookah": "Убрать кальян",

            "f_lift_0": "0 этаж",
            "f_lift_1": "1 этаж",
            "f_lift_2": "2 этаж",
            "f_lift_3": "3 этаж",

            "s_lift_0": "0 этаж",
            "s_lift_1": "1 этаж",
            "s_lift_2": "2 этаж",
            "s_lift_3": "3 этаж",
            "s_lift_4": "4 этаж",
            
            "c_lift_0": "1 этаж",
            "c_lift_1": "2 этаж",

            "gov_lift_1": "1 этаж",
            "gov_lift_3": "3 этаж",
            "gov_lift_4": "4 этаж",

            "fraction_table": "Планшет фракции",
            "org_table": "Планшет семьи",

            "leave_fraction": "Покинуть фракцию",
            "leave_org": "Покинуть семью"
        };

    let categoryName = popupData.title,
        drawname = popupData.title,
        muted = false,      
        selectAction = categoryData[popupData.title][popupData.id];


    const onMouseOut = () => {
        drawname = categoryName;
    }

    const OnHovered = (index) => {
        //const index = event.target.id;
        //window.chatAPI.push (`OnHovered - `+index);
        if(index == 8) {
            drawname = "Закрыть"
        } else if(selectAction[index] === 'vmuted') {
            drawname = muted ? "Выключить микрофон" : 'Включить микрофон'
        } else {
            const action = selectAction[index];
            if (action == null) drawname = categoryName;
            else drawname = categoryDesc[action];
        }
    }

    const onCircleClick = (index) => {
        if (index == 8) {
            executeClient ("client.circle.events", null, -1);
            window.router.setHud();
            return;
        } else if (selectAction[index] === 'vmuted') {
            drawname = muted ? "Выключить микрофон" : 'Включить микрофон';
        }

        switch (selectAction[index]) {
            case 'trunkAction':
                OpenCategory ("Багажник", 0);
                break;
            case 'sell':
                executeClient ("client.circle.events", categoryName, Number(index));
                OpenCategory ("Взаимодействия", 0)
                break;
            case 'paired_animations':
                OpenCategory ("Парные анимации", 0);
                break;
            case 'fraction':
                if ($charFractionID == 0 || $charFractionID == 15) return;
                OpenCategory ("Фракция", $charFractionID);
                break;
            case 'family':
                if ($charOrganizationID == 0) return;
                OpenCategory ("Семья", 0);
                break;
            case 'documents': 
                OpenCategory ("Документы", 0);
                break;
            case 'house': 
                OpenCategory ("Недвижимость", 0);
                break;
            case 'documents': 
                OpenCategory ("Документы", 0);
                break;
            case 'documents': 
                OpenCategory ("Документы", 0);
                break;
            default:
                if (selectAction[index] === undefined) return;

                if (categoryName === "Категории" || categoryName === "Анимации") {
                    executeClient ("client.circle.animation", categoryName, Number(index));
                } else {
                    executeClient ("client.circle.events", categoryName, Number(index));
                    window.router.setHud();
                }
                break;

        }
    }
    
    const OpenCategory = (category, id) => {
        if (category == 'vmuted') {
            muted = id;
        } else {
            categoryName = category;
            selectAction = categoryData[category][id];
        }
    }
    window.events.addEvent("cef.circle.category", OpenCategory);

    import { onDestroy } from 'svelte'

    onDestroy(() => {
        window.events.removeEvent("cef.circle.category", OpenCategory);
    });



    const ontest = (index, max) => {
        switch (max) {
            case 1:
                return 1;
            case 2:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 5;
                }
                return;
            case 3:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 3;
                    case 2:
                        return 5;
                }
                return;
            case 4:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 3;
                    case 2:
                        return 5;
                    case 3:
                        return 7;
                }
                return;
            case 5:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 4;
                    case 3:
                        return 6;
                    case 4:
                        return 8;
                }
                return;
            case 6:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 4;
                    case 3:
                        return 5;
                    case 4:
                        return 6;
                    case 5:
                        return 8;
                }
                return;
        
        }
        return index + 1;
    }


    const initCircle = () => {

    }


</script>

<div class="circle" on:mouseleave={onMouseOut}>
    <div class="circle__close" this:initCircle on:mouseenter={() => OnHovered (8)}  on:mouseleave={onMouseOut}>
        
    </div>
    <div class="center" on:click={() => onCircleClick (8)} on:mouseenter={() => OnHovered (8)}>
        {#each selectAction as name, index}
        <li on:click={() => onCircleClick (index)} class="contents child{ontest (index, selectAction.length)}">
            <span class="icons-circle {prefix}{name}{name === 'vmuted' ? `_${muted}` : ""}" />
            <div>{categoryDesc[name]}</div>
            <div class="contents__index">{index +1}</div>
        </li>
    {/each}
    </div>
</div>