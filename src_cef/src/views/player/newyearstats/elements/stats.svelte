<script>
    import { fade } from 'svelte/transition';
    import { charData } from 'store/chars'
    import moment from 'moment'
    import { format } from 'api/formatter'
</script>

<div class="newyearstats__stats" in:fade={{  duration: 1000 }}>
    <div class="newyearstats__logo"/>
    <div class="box-flex">
        <div class="newyearstats-img"/>
        <div class="newyearstats__time">
            <div class="newyearstats__subtitle">Вы отыграли более:</div>
            <div class="newyearstats__title">{moment.duration($charData.Time.TotalTime, "minutes").format("y[г.] M[мес.] w[нед.] d[д.] h[ч.] m[м.]")}</div>
        </div>
    </div>
    <div class="newyearstats__info">
        <div class="newyearstats__info-element first settings">{$charData.Kills} человек</div>
        <div class="newyearstats__info-element second settings">{$charData.Deaths} раза</div>
        <div class="newyearstats__info-element third settings">${format("money", $charData.EarnedMoney)}</div>
        <div class="newyearstats__info-element fourth settings">{$charData.EatTimes} раза</div>
        <div class="newyearstats__info-element five settings">{$charData.Revived} раз</div>
        <div class="newyearstats__info-element six settings">{$charData.Handshaked} рук</div>
    </div>
</div>