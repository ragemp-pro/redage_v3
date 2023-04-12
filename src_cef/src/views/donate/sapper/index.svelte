<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    import { accountRedbucks } from 'store/account'
    import './main.scss'
    import logo from './images/logo-sapper.png'

    const betMin = 10;
    const betMax = 5000;
    let inputCash = "10";


    let SlotListToggled = [
        [false, false, false, false, false], //0,
        [false, false, false, false, false], //1,
        [false, false, false, false, false], //2,
        [false, false, false, false, false], //3,
        [false, false, false, false, false], //4,
        [false, false, false, false, false], //5,
        [false, false, false, false, false], //6,
    ];

    let SlotToggled = [
        -1, //0,
        -1, //0,
        -1, //0,
        -1, //0,
        -1, //0,
        -1, //0,
        -1, //0,
    ];

    let SelectSlot = -1;
    
    const SlotListCoef = [
        1.1,
        1.2,
        1.4,
        1.6,
        2,
        2.5,
        3,
    ]

    let IsGame = false;
    function randomInteger(min, max) {
        const rand = min + Math.random() * (max - min);
        return Math.round(rand);
    }

    const onSlotUse = (sSlot, index) => {
        if (IsGame !== true) return;
        else if (sSlot !== SelectSlot)
            return;

        const rand = randomInteger (0, 100);
        SlotListToggled[sSlot][index] = rand < 80 ? true : 9;

        if (SlotListToggled[sSlot][index] === true) {
            let arrayIndex = [];
            for (let i = 0; i < 5; i++) {
                if (i !== index)
                    arrayIndex.push (i);
            }

            let mineIndex = arrayIndex [randomInteger (0, 3)];
            
            let listData = SlotListToggled[sSlot];
            for (let i = 0; i < 5; i++) {
                if (listData[i] === false) {
                    listData[i] = mineIndex !== i ? true : 9;
                }
            }
            SlotListToggled[sSlot] = listData;
            SlotToggled[sSlot] = true;
            if (++SelectSlot >= SlotListToggled.length) onGetWin (SelectSlot);
        } else {
            let listData = SlotListToggled[sSlot];
            for (let i = 0; i < 5; i++) {
                if (listData[i] === false) {
                    listData[i] = true;
                }
            }
            SlotListToggled[sSlot] = listData;
            //
            listData = SlotListToggled;
            for (let s = SelectSlot + 1; s < SlotListToggled.length; s++) {
                const mineIndex = randomInteger (0, 4);
                
                for (let i = 0; i < 5; i++) {
                    if (listData[s][i] === false) {
                        listData[s][i] = mineIndex !== i ? true : 9;
                    }
                }
            }
            SlotListToggled = listData;
            //
            SlotToggled[sSlot] = false;
            onGetWin (-2);
            window.notificationAdd(4, 9, `К сожалению, вы проиграли.`, 3000);
        }

    }
	const onHandleInput = (value, num) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 0) value = 0;
        else if (value > betMax) value = betMax;
        inputCash = value;
    }

    const onStartGame = () => {
        if (IsGame !== false) return;
        else if (Number (inputCash) > Number ($accountRedbucks)) {            
            inputCash = $accountRedbucks;
            window.notificationAdd(4, 9, `У Вас нет столько RB!`, 3000);
            return;
        } else if (Number (inputCash) < betMin) {            
            inputCash = betMin;
            window.notificationAdd(4, 9, `Минимальная ставка составляет ${format("money", betMin)} RB`, 3000);
            return;
        } else if (Number (inputCash) > Number (betMax)) {            
            inputCash = betMax;
            window.notificationAdd(4, 9, `Максимальная ставка составляет ${format("money", betMax)} RB`, 3000);
            return;
        }
        //IsGame = true;//
        //SelectSlot = 0;//
        executeClient ("client.sapper.bet", Number (inputCash));
    }

    const startGame = (bet) => {
        inputCash = bet;
        IsGame = true;//
        SelectSlot = 0;//
        window.notificationAdd(4, 9, `Игра началась`, 3000);
    }

    window.events.addEvent("cef.sapper.game", startGame);
    import { onDestroy } from 'svelte'
    onDestroy(() => {
        if (IsGame === -1) executeClient ("client.sapper.end", Number (SelectSlot));
        else if (IsGame === true) executeClient ("client.sapper.end", -1);
        window.events.removeEvent("cef.sapper.game", startGame);
    });

    const onExit = () => {
        window.router.setView('DonateMain');
    }

    const onGetWin = (type) => {
        if (type > 0) window.notificationAdd(4, 9, `Игра закончилась, вы выиграли ${format("money", type < 1 ? 0 : Math.round (inputCash * SlotListCoef [type - 1]))} RB`, 3000);
        executeClient ("client.sapper.end", type);
        IsGame = false;
        SelectSlot = -1;//
        SlotToggled = [
            -1, //0,
            -1, //0,
            -1, //0,
            -1, //0,
            -1, //0,
            -1, //0,
            -1, //0,
        ];
        let listData = SlotListToggled;
        for (let s = 0; s < SlotListToggled.length; s++) {
            for (let i = 0; i < 5; i++) {
                if (listData[s][i] !== false) {
                    listData[s][i] = false;
                }
            }
        }
        SlotListToggled = listData;
    }
    
            
</script>

<div id="sapper">
    <div class="game">
        <div class="box">

            <div class="content">
                <img src={logo} alt="" class="logoSapper" />

                <div class="title">{translateText('donate', 'Игра')} -<span>{translateText('donate', 'Сапёр')}</span></div>
                <p class="desc">{translateText('donate', 'Игра "Сапёр" - проверьте свою удачу, с каждым шагом двигаясь ко всё более крупному призу')}!</p>
                <div class="title rb">{translateText('donate', 'Ваш баланс')}: <span>{format("money", $accountRedbucks)} RB</span></div>
                

                <div class="win">
                    <div class="n">{format("money", SelectSlot < 1 ? 0 : Math.round (inputCash * SlotListCoef [SelectSlot - 1]))} RB</div>
                    <p>{translateText('donate', 'твой выигрыш')}</p>
                </div>

                <ul class="playZone">

                    {#each SlotListToggled as items, index}
                        <li class="group">
                            <div class="info" class:success={SlotToggled[index] === true} class:fail={SlotToggled[index] === false} class:half={SelectSlot === index}>
                                {SlotListCoef [index]}X
                            </div>
                            <ul class="items">
                                {#each items as slot, indexSlot}
                                    <li class:checked={slot === false}
                                        class:success={slot === true}
                                        class:mine={slot === 9} 
                                        on:click={() => onSlotUse (index, indexSlot)} />
                                {/each}
                            </ul>
                        </li>
                    {/each}             
                </ul>
                <div class="input">
                    <input disabled={IsGame !== false} class="rate" bind:value={inputCash} type="text" on:input={(event) => onHandleInput (event.target.value, 10)}>

                    {#if IsGame == false}
                    <div class="btn" on:click={onStartGame}>{translateText('donate', 'Играть')}</div>
                    {:else}                    
                    <div class="btn" on:click={() => onGetWin (SelectSlot)}>{translateText('donate', 'Забрать')}</div>
                    {/if}
                    <div class="exit" on:click={onExit}>{translateText('donate', 'Выйти')}</div>
                </div>
            </div>

        </div>
    </div>
</div>