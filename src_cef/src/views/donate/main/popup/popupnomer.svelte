
<script>
    import { format } from "api/formatter";
    import {accountRedbucks} from "store/account";
    import { charWanted, charMoney, charBankMoney } from 'store/chars'
    import {executeClient} from "api/rage";
    import {validate} from "api/validation";

    let number = "";

    const getPrice = (text) => {
        text = text.toLowerCase()
        if (text.match(/^([A-Za-z]{1,1})([0-9]{1,1})([0-9]{1,1})([0-9]{1,1})([A-Za-z]{1,1})$/)) {
            let coincidence = 0

            for (var i = 1; i < 4; i++) {
                for (var x = i + 1; x < 4; x++) {
                    if (text[i] == text[x]) coincidence++;
                }
            }

            if (text[0] != text[4]) {
                if (coincidence == 0)
                    return [500000, "$", "Обычный"];//вирты
                else if (coincidence == 1 && text[1] != text[3])
                    return [3000, "RB", "Редкий"];
                else if (coincidence == 1 && text[1] == text[3])
                    return [5000, "RB", "Уникальный"];
                else if (coincidence == 3)
                    return [7500, "RB", "Уникальный"];
            } else if (text[0] == text[4]) {
                if (coincidence == 0)
                    return [3000, "RB", "Редкий"];
                else if (coincidence == 1 && text[1] != text[3])
                    return [5000, "RB", "Уникальный"];
                else if (coincidence == 1 && text[1] == text[3])
                    return [7500, "RB", "Уникальный"];
                else if (coincidence == 3)
                    return [10000, "RB", "Люкс"];
            }
        }
        return [30000, "RB", "Люкс"];
    }

    let confirm = false;
    const onBuy = () => {
        let check = validate("vehicleNumber", number);
        if(!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }
        const numberData = getPrice (number);

        if (numberData[1] === "RB" && $accountRedbucks < numberData[0])
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        if (numberData[1] === "$" && $charMoney < numberData[0])
            return window.notificationAdd(4, 9, `Недостаточно денег!`, 3000);

        if (!confirm)
            confirm = true;
        else {

            if (!window.loaderData.delay ("donate.onBuy", 1.5))
                return;

            executeClient ("client.donate.buyVehNumber", number);
        }
    }
</script>



<div class="newdonate__info ">
    <div class="newdonate__info-block" on:mouseenter on:mouseleave>
        <div class="newdonate__info-number">
            <input type="text" class="newdonate__number_input" placeholder="REDAGE" bind:value={number} maxLength={8}  on:focus={() => confirm = false}>
        </div>
        <div class="newdonate__info-info">
            <div class="box-flex">
                <div class="newdonate__info-title">Номер для транспорта</div>
            </div>
            <div class="newdonate__info-paragraph">
                Нажмите на картинку слева и введите желаемый номер. Номер не должен начинаться с 0. <br><br><b>{getPrice (number)[2]}</b> номер транспортного средства позволит притянуть завистливые взгляды окружающих к вашему авто! Цена формируется динамически в зависимости от количества символов и формата номера.
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