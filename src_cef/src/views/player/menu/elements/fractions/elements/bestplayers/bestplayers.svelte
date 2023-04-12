<script>
    import { translateText } from 'lang'
    import TopPlayers from './topplayers.svelte'
    import TopReward from './topreward.svelte'
    import Logo from '../other/logo.svelte'

</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-best"></span>
                <div class="fractions__header_title">{translateText('player1', 'Лучшие игроки')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Здесь отображается информация о лучших игроках, статистика обновляется в начале месяца')}</div>
        </div>
    </div>
</div>
<div class="box-flex mt-20">
    <div class="fractions__selector active">
        {translateText('player1', 'Лучшие игроки')}
    </div>
    <div class="fractions__selector">
        {translateText('player1', 'Награды лучшим')}
    </div>
</div>
<TopPlayers/>