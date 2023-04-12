<script>
    import { translateText } from 'lang'
    import { TimeFormat, GetTime } from 'api/moment'
    import { serverDateTime } from 'store/server'
    import { charFractionID } from 'store/chars'
    import { fly } from 'svelte/transition';
    import { format } from 'api/formatter'

    let cashValue = 0;
    window.PayDay = async (cash) => {
        //if (window.closeTip ())
        //    await window.wait(250);

        cashValue = cash;
        if (cashValue > 0) {
            window.hudStore.HideHelp (true);            
            await window.wait(5000);
            window.hudStore.HideHelp (false);
            cashValue = 0;
        } else {
            window.hudStore.HideHelp (false);
        }
    }


    const FractionTypes = {
        0: -1,
        1: 1, // The Families
        2: 1, // The Ballas Gang
        3: 1,  // Los Santos Vagos
        4: 1, // Marabunta Grande
        5: 1, // Blood Street
        6: 2, // Cityhall
        7: 2, // LSPD police
        8: 2, // Emergency care
        9: 2, // FBI 
        10: 0, // La Cosa Nostra 
        11: 0, // Russian Mafia
        12: 0, // Yakuza 
        13: 0, // Armenian Mafia 
        14: 2, // Army
        15: 2, // News
        16: 4, // The Lost
        17: 3, // Merryweather
        18: 2, // Sheriff
    };

</script>
{#if cashValue > 0}
    <div class="hudevo__check" in:fly={{ y: -200, duration: 500 }} out:fly={{ y: -200, duration: 250 }}>
        <div class="box-between">
            <div class="hudevo__check_title">
                {translateText('player2', 'ЧЕК')} - #{Math.round (GetTime ().unix() / 1000)}
            </div>
            <div class="hudevo__check_subtitle">{TimeFormat ($serverDateTime, "H:mm")}</div>
        </div>
        <div class="hudevo__check_subtitle">
            {!(FractionTypes [$charFractionID] === 2 || FractionTypes [$charFractionID] === 3) ? translateText('player2', 'Пособие по безработице') : translateText('player2', 'Зарплата')}
        </div>
        <div class="hudevo__check_money">${format("money", cashValue)}</div>
    </div>
{/if}