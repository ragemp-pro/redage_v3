<script>
    import {hasJsonStructure} from "api/functions";
    import { translateText } from 'lang'

    export let viewData;

    if (hasJsonStructure (viewData))
        viewData = JSON.parse(viewData);

    import { executeClient } from 'api/rage'
    import './main.sass'   
    const Pets = [
        {
            name: "Кот",
            price: "200.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Не может защищать",
                    isPlus: false
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "cat"
        },
        {
            name: "Ротвеллер",
            price: "42.000 RB",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Может атаковать врага",
                    isPlus: true
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "rottweiler"
        },
        {
            name: "Хаски",
            price: "50.000 RB",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Может атаковать врага",
                    isPlus: true
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "husky"
        },
        {
            name: "Пудель",
            price: "500.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Не может защищать",
                    isPlus: false
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "poodle"
        },
        {
            name: "Мопс",
            price: "420.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Не может защищать",
                    isPlus: false
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "pug"
        },
        {
            name: "Ретвивер",
            price: "800.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "retriever"
        },
        {
            name: "Овчарка",
            price: "1.500.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Может атаковать врага",
                    isPlus: true
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "shepherd"
        },
        {
            name: "Болонка",
            price: "15.000 RB",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Не может защищать",
                    isPlus: false
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "westy"
        },
        {
            name: "Свинья",
            price: "600.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Не может защищать",
                    isPlus: false
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "pig"
        },
        /*
        {
            name: "Кабан",
            price: "1.000.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Не может атаковать врага",
                    isPlus: false
                },
                {
                    text: "Не может защищать",
                    isPlus: false
                },
                {
                    text: "Может обнюхивать",
                    isPlus: true
                },
            ],
            image: "boar"
        },
        {
            name: "Пантера",
            price: "3.000.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Может атаковать врага",
                    isPlus: true
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
            ],
            image: "panther"
        },
        {
            name: "Черная пантера",
            price: "100.000 RB",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Может атаковать врага",
                    isPlus: true
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
            ],
            image: "bpanther"
        },
        {
            name: "Койот",
            price: "750.000 $",
            description: [
                {
                    text: "Может следовать за тобой",
                    isPlus: true
                },
                {
                    text: "Может следовать за человеком",
                    isPlus: true
                },
                {
                    text: "Может сидеть на месте",
                    isPlus: true
                },
                {
                    text: "Может играть с тобой",
                    isPlus: true
                },
                {
                    text: "Может атаковать врага",
                    isPlus: true
                },
                {
                    text: "Может защищать",
                    isPlus: true
                },
            ],
            image: "coyote"
        },*/
    ]
    
    let currentPed = 0; 
    let Select = Pets[currentPed];

    let isBtnActive = 0;
    
    let isTimeOut = false;
    function buttonActiveSwap(status = 0) {
        isBtnActive = status;
        isTimeOut = false;
    }

    let timeId = -1;
    function onClickDown(){
        if (timeId != -1)
            clearTimeout (timeId)
        buttonActiveSwap(2)
        if(currentPed+1 < Pets.length){
            currentPed++;
            Select = Pets[currentPed];
            OnFocus (currentPed);
        }
        if(isTimeOut){
            timeId = setTimeout(buttonActiveSwap,2000)
        }
    }

    function onClickUp(){
        if (timeId != -1)
            clearTimeout (timeId)
        buttonActiveSwap(1)
        if(currentPed > 0){
            currentPed--;
            Select = Pets[currentPed];
            OnFocus (currentPed);
        }
        if(isTimeOut){
            timeId = setTimeout(buttonActiveSwap,2000)
        }
    }

    import { format } from 'api/formatter'
    const OnSelectPet = (pet, index) => {
        if (pet === Select)
            return;

        Select = pet;
        currentPed = index; 
    }

    const OnFocus = (index) => {
        document.querySelector(`#scroll${index}`).scrollIntoView();
    }

    const handleKeydown = (event) => {
        const { keyCode } = event;
        if(keyCode == 38) {
            onClickUp()
        } else if(keyCode == 40) {
            onClickDown()
        } else if (keyCode == 13) {
            onBuy ();
        }
    }

    let time = 0;
    const onBuy = () => {
        if (time > new Date().getTime()) return;
        time = new Date().getTime() + 1000;
        executeClient ('client.petshop.buy', currentPed);        
    }

</script>
<svelte:window on:keyup={handleKeydown}/>
<div id="newpetshop">
    <div class="newpetshop__controls">
        <div class="newpetshop__button_control button-top" class:active={isBtnActive == 1} on:click={onClickUp}>&#8593;</div>
        <div class="newpetshop__button_control button-down" class:active={isBtnActive == 2} on:click={onClickDown}>&#8595;</div>
    </div>
    <div class="newpetshop__animals">
        <div class="newpetshop__animals_height">
            {#each Pets as pet, index}
                <div class="newpetshop__animal" class:active={pet === Select} on:click={() => OnSelectPet (pet, index)} id="scroll{index}">
                    <div class="newpetshop__animal_img" style="background-image: url({document.cloud}img/pets/{pet.image}.png)"/>
                </div>
            {/each}
        </div>
    </div>
    <div class="newpetshop__main">
        <div class="newpetshop__features">
            <div class="newpetshop__title">{Select.name}</div>
            <div class="newpetshop__subtitle">{format("money", viewData [currentPed].Price)} {viewData [currentPed].isDonate ? "RB" : "$"}</div>
            {#each Select.description as descr, index}
                <div class="newpetshop__feature">
                    <div class="newpetshop__circle {descr.isPlus ? "" : "none"}"><div class="newpetshop__circle_circle"></div></div>
                    <div>{descr.text}</div>
                </div>
            {/each}
        </div>
        <div class="newpetshop__info">
            <div class="newpetshop__background"></div>
            <div class="newpetshop__header">
                <div class="newpetshop__icon"></div>
                {translateText('business', 'Выбор напарника')}
            </div>
            <div class="newpetshop__text">
                {translateText('business', 'Хотите забыть что такое одиночество? Тогда вы попали в нужный магазин! Тут представлен большой выбор различных животных, кто-нибудь точно понравится. Некоторые из них могут нападать на других питомцев и людей, а большинство нужно лишь для вашего эстетического наслаждения: они могут приносить мячик, гулять, следовать за кем-либо или обнюхивать. Питомец нужен каждому! Не забывайте своевременно кормить своего питомца, со временем он начинает хотеть кушать. Подойдет любая еда из Burger Shot.')}
            </div>
        </div>
    </div>
    <div class="newpetshop__buttons">
        <div class="box-flex">
            {translateText('business', 'Назад')}
            <div class="newpetshop__button_control">ESC</div>
        </div>
        <div class="box-flex" on:click={onBuy}>
            {translateText('business', 'Купить')}
            <div class="newpetshop__button_control">Enter</div>
        </div>
    </div>
</div>

