<script>
    import { translateText } from 'lang'
    import WeaponsList from './weaponslist.svelte'
    import WeaponsAccess from './weaponsaccess.svelte'
    import Logo from '../other/logo.svelte'
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo/>
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-stock"></span>
                <div class="fractions__header_title">{translateText('player1', 'Наборы оружия')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'В данном меню вы можете создавать и менять наборы оружия')}</div>
        </div>
    </div>
</div>
<WeaponsAccess/>