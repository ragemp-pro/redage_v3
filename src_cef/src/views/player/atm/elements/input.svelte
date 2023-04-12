<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { format } from 'api/formatter'
    export let activeMain;
    export let subdata;
    export let type;
    export let placeholder;
    export let menuItem;

    let value = "";
    let icon = 'dollar';
    $: {
        if (placeholder === translateText('player', 'Счет зачисления')) {
            icon = 'card';
        } else {
            icon = 'dollar';
        }
    }
    const onNext = () => {
        executeClient ('atmVal', value);
        value = ""
    }

    const onPrev = () => {
        executeClient ('atmCB', type, 0);
        value = ""
    }
	const onHandleInput = (value, num) => {
        value = Math.round(value.replace(/\D+/g, ""));
        if (value < 1) value = 1;
        else if (num === 6 && value > 9999999) value = 9999999;
        else if (num === 10 && value > 99999999) value = 99999999;
    }
</script>


<div class="atm_step">
    <div class="bf_img">
        <span class="m1" />
    </div>
    <div class="mdatm">
        {#if activeMain === 2 || activeMain === 3}
            <div class="head_inp head_vertical">
                <span class="inp_ic {menuItem [activeMain].icon}" />
                <div>
                    <span>{menuItem [activeMain].title}</span>
                    {#if subdata.length}
                        <div class="small">{translateText('player', 'Баланс')}: <span class="yellow">{format("money", subdata.split('/')[0])}$</span></div>
                        <div class="small">{translateText('player', 'Максимум')}: <span class="yellow">{format("money", subdata.split('/')[1])}$</span></div>
                    {/if}
                </div>
            </div>
        {:else}
            <div class="head_inp head_column">
                <span class="inp_ic {menuItem [activeMain].icon}" />
                <div>
                    <span>{menuItem [activeMain].title}</span>
                </div>
            </div>  
        {/if}
        <div class="inp_atm dollar">
            <div class="after {icon}" />
            {#if icon == "card"}
            <input bind:value={value} type="text" on:input={(event) => onHandleInput (event.target.value, 10)} placeholder={placeholder} maxLength={10}/>
            {:else}
            <input bind:value={value} type="text" on:input={(event) => onHandleInput (event.target.value, 6)} placeholder={placeholder} maxLength={8}/>
            {/if}
        </div>

        <ul class="info_atm_button">
            <li on:click={onNext}>
                <span class="info_head">{translateText('player', 'Далее')}</span>
            </li>
            <li on:click={onPrev}>
                <span class="info_head">{translateText('player', 'Назад')}</span>
            </li>
        </ul>

    </div>
    <div class="bf_img">
        <span class="m2" />
    </div>
</div>