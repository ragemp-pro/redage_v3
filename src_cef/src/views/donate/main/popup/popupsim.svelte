
<script>
    import { format } from "api/formatter";
    import {accountRedbucks} from "store/account";
    import { charWanted, charMoney, charBankMoney } from 'store/chars'
    import {executeClient} from "api/rage";
    import {validate} from "api/validation";
    import {numberValue} from "@/views/player/hudevo/phonenew/components/calls/addContact.svelte";

    let number = "";

    const getPrice = (text) => {
        text = Number(text);

        const length = text.toString().length;
        if (length == 1)
            return [50000, "RB", "Люкс"];
        else if (length == 2)
            return [40000, "RB", "Люкс"];
        else if (length == 3)
            return [30000, "RB", "Люкс"];
        else if (length == 4)
            return [20000, "RB", "Редкий"];
        else if (length == 5)
            return [15000, "RB", "Редкий"];
        else if (length == 6)
            return [10000, "RB", "Уникальный"];
        else if (length == 7)
            return [7500, "RB", "Уникальный"];
        else if (length == 8)
            return [5000, "RB", "Уникальный"];
        else if (length == 9)
            return [2500, "RB", "Обычный"];

        return [0, "RB", "Обычный"];
    }

    let confirm = false;
    const onBuy = () => {
        const sim = Number (number);
        let check = validate("phonenumber", sim);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }
        
        const numberData = getPrice (sim);

        if (numberData[1] === "RB" && $accountRedbucks < numberData[0])
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);

        if (!confirm)
            confirm = true;
        else {

            if (!window.loaderData.delay ("donate.onBuy", 1.5))
                return;


            executeClient ("client.donate.buySim", sim);
        }
    }
</script>



<div class="newdonate__info ">
    <div class="newdonate__info-block" on:mouseenter on:mouseleave>
        <div class="newdonate__info-sim">
            <input type="text" class="newdonate__number_input" placeholder="420" bind:value={number} maxLength={9}>
        </div>
        <div class="newdonate__info-info">
            <div class="box-flex">
                <div class="newdonate__info-title">Номер для телефона</div>
            </div>
            <div class="newdonate__info-paragraph">
                Нажмите на картинку слева и введите желаемый номер. Номер не должен начинаться с 0. <br> <br> Ваш <b>{getPrice (number)[2]}</b> номер телефона подчеркнет индивидуальность и позволит ощутить себя по-настоящему эксклюзивно! Цена формируется динамически в зависимости от количества символов.
            </div>
        </div>
        {#if !confirm}
        <div class="newdonate__button number" on:click={onBuy}>
            <div class="newdonate__button-main">
                <div class="newdonate__button-text">Купить за {format("money", getPrice (number)[0])}{getPrice (number)[1]}</div>
            </div>
        </div>
        {:else}
            <div class="box-flex">
                <div class="newdonate__button number-1" on:click={onBuy}>
                    <div class="newdonate__button-main">
                        <div class="newdonate__button-text">Купить за {format("money", getPrice (number)[0])}{getPrice (number)[1]}?</div>
                    </div>
                </div>
                <div class="newdonate__button number-2" on:click={() => confirm = false}>
                    <div class="newdonate__button-main">
                        <div class="newdonate__button-text">Отмена</div>
                    </div>
                </div>
            </div>
        {/if}
    </div>
    <div class="newdonate__escape">
        <div class="box-flex">
            <span class="donateicons-esc"/>
            <div class="newdonate__escape-title">ESC</div>
        </div>
        <div class="newdonate__escape-text">
            Нажми, чтобы закрыть
        </div>
    </div>
</div>