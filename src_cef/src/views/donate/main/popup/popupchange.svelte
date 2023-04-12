<script>
    import { executeClient } from 'api/rage'
    import { accountRedbucks } from 'store/account'
    import { serverDonateDoubleConvert } from 'store/server'
    export let SetPopup;

    let 
        changeRbBtn = 10,
        changeCashBtn = Math.round(changeRbBtn * Math.round (10 * $serverDonateDoubleConvert));

	const onHandleInputTransfer = (value, isRb = false) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 0) value = 0;
        else if (!isRb && value > Math.round(999999 * Math.round (10 * $serverDonateDoubleConvert))) value = Math.round(999999 * Math.round (10 * $serverDonateDoubleConvert));
        else if (isRb && value > 999999) value = 999999;

        if (!isRb) {
            changeCashBtn = value;
            changeRbBtn = Math.round(value / Math.round (10 * $serverDonateDoubleConvert));
            if (changeRbBtn < 0) changeRbBtn = 0;
        }
        else {            
            changeCashBtn = Math.round(value * Math.round (10 * $serverDonateDoubleConvert));
            changeRbBtn = value;
        }
    }

    const onChange = () => {
        if ($accountRedbucks < changeRbBtn)
            return window.notificationAdd(4, 9, `Недостаточно Redbucks!`, 3000);
        SetPopup ()
        executeClient ("client.donate.change", changeRbBtn);
    }
</script>

<div class="newdonate__change">
    <div class="newdonate__change-block" on:mouseenter on:mouseleave>
        <div class="newdonate__change-top">
            <div class="newdonate__change-element">
                <div class="newdonate__change-img redbucks"/>
                <input class="newdonate__change-input" bind:value={changeRbBtn} on:input={(event) => onHandleInputTransfer (event.target.value, true)} placeholder="10">
            </div>
            <div class="newdonate__change-element">
                <div class="newdonate__change-img progress"/>
                <div class="newdonate__change-curs">1 RB к ${Math.round (10 * $serverDonateDoubleConvert)}</div>
            </div>
            <div class="newdonate__change-element">
                <div class="newdonate__change-img money"/>
                <input class="newdonate__change-input" bind:value={changeCashBtn} on:input={(event) => onHandleInputTransfer (event.target.value)} placeholder="10">
            </div>
        </div>
        <div class="newdonate__change-bottom">
            <div class="newdonate__button_small" on:click={onChange}>
                <div class="newdonate__button-text">Обменять</div>
            </div>
        </div>
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