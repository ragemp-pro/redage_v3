<script>
    import { translateText } from 'lang'
    import { daysName, getPngUrl, getTypeToTitle, statusType, getTimeFromMins } from './data'
    import { format } from 'api/formatter'
    export let item;

</script>

<div class="reward__element">
    <div class="box-column">
        <div class="reward__element_title">{item.desc}</div>
        <div class="reward__element_subtitle">{translateText('player1', 'Пополнение на')} {format("money", item.maxDonate)} RB</div>
    </div>
    <div class="reward__element_box">
        <div class="reward__element_reward">
            <span class="tooltiptext">{getTypeToTitle (item.type, item.itemId)}</span>
            <div class="reward__element_img" style="background-image: url({getPngUrl (item)})"></div>
        </div>
    </div>

    {#if item.status === statusType.received}
        <div class="reward__element_button taked">
            {translateText('player1', 'Получено')}
        </div>
    {:else if item.status === statusType.receive}
        <div class="reward__element_button" on:click>
            {translateText('player1', 'Забрать награду')}
        </div>
    {:else if item.status === statusType.time}
        <div class="reward__element_button time">
            {format("money", item.donate)} RB
        </div>
    {/if}
</div>