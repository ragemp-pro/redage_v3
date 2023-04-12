<script>
    import { translateText } from 'lang'
    import SquadsList from './squadslist.svelte'
    import SquadSettings from './squadsettings.svelte'
    import SquadRanks from './squadranks.svelte'
    import SquadAccesses from './squadaccesses.svelte'
    import Logo from '../other/logo.svelte'
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo/>
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-squads"></span>
                <div class="fractions__header_title">{translateText ("player1", 'Отряды')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText ("player1", 'Меню просмотра, настройки и создания отрядов')}</div>
        </div>
    </div>
</div>
<SquadAccesses/>