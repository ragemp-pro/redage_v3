<script>
    import { translateText } from 'lang'
    import ProgressBar from 'progressbar.js';
    import { selectChar, selectType, selectIndex, settings } from './../store.js';
    import { updateIndex } from 'store/customization';
    import moment from 'moment';
    import { setTime, elapsed } from 'api/moment'
    import { executeClient } from 'api/rage'
    export let charid;
    export let char;
    
    const charData = char.Data;
    
    const updateTime = (data) => {
        if ($selectIndex !== charid)
            return;

        selectChar.set (char);

        if (data !== "-") 
            setTime (data);
        else 
            setTime (false);
    }

    $: if (char.Data.DeleteData) {
        updateTime (char.Data.DeleteData)
    }

    selectIndex.subscribe((value) => {
        if (value === charid)
            updateTime (char.Data.DeleteData)
    });

    //export let FractionID;
    //export let Money;
    //export let BankMoney;
    //export let CustomIsCreated;
    
    const GetMaxExp = (lvl) => {
        return 3 + lvl * 3;
    }

    const GetProgress = (exp) => {
       let progress = exp * 100 / GetMaxExp (charData.LVL);
        
        if (progress > 100) progress = 100;

        return progress / 100;
    }

	const CreateProgressBar = () => {
        let ProgressBarId = new ProgressBar.Circle(".auth__characters_circle.UUID-" + charData.UUID, {
            color: '#FF9F1C',
            trailColor: '#eee',
            trailWidth: 1,
            duration: 1400,
            easing: 'bounce',
            strokeWidth: 6,
            from: {color: '#FF9F1C', a:0},
            to: {color: '#E71D36', a:1},
            // Set default step function for all animate calls
            step: function(state, circle) {
                circle.path.setAttribute('stroke', state.color);
            }
        });
        ProgressBarId.animate(GetProgress (charData.EXP));
    }

    const onSelectChar = () => {
        isDell = false;
        updateIndex (charid);
        selectIndex.set (charid);
        selectType.set (settings.char);
        selectChar.set (char);
    }
    let isDell = false;

    const onDell = (e, isCancelDell = false) => {
        e.stopPropagation();

        if (isDell || isCancelDell) {
            if (!window.loaderData.delay("delete", 5))
                return;
            else if (charid > 2)
                return;
            else if ($selectIndex !== charid)
                return;

            isDell = false;
            executeClient("client.char.delete", charid);
        } else {
            isDell = true;
        }
    }
</script>

<div class="auth__characters_block" on:click={onSelectChar} class:active={$selectIndex === charid}>
    <div class="auth__characters_character">
        <div class="box-column">
            <b>{charData.FirstName}</b>
            <div>{charData.LastName}</div>
        </div>
        <div class="auth__characters_circle UUID-{charData.UUID}" use:CreateProgressBar>
            <div class="auth__characters_circle_text">
                <b>{charData.LVL}</b>
                <div>LVL</div>
            </div>
        </div>
    </div>
    {#if charid <= 2}
    <div class="auth__characters_hover">
        {#if char.Data && char.Data.DeleteData === "-"}
            {#if !isDell}
                <div>Удалить персонажа</div>
                <div class="main__button_square large box-center" on:click={(e) => onDell(e)}>
                    Удалить
                </div>
            {:else}
                <div>Вы уверены?</div>
                <div class="main__button_square large box-center" on:click={(e) => onDell(e)}>
                    Удалить
                </div>
            {/if}
        {:else}
        <div>Удалится через {moment.utc($elapsed).format("DD") -1} дня, {moment.utc($elapsed).format("HH")} часов, {moment.utc($elapsed).format("mm")} минут.</div>
        <div class="main__button_square large box-center" on:click={(e) => onDell(e, true)}>
            Отменить
        </div>
        {/if}
    </div>
    {/if}
</div>