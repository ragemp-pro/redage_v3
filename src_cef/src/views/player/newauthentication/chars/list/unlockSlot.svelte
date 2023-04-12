<script>
    import { translateText } from 'lang'
    import { selectIndex, selectType, settings } from './../store.js';
    export let charid;

    const onSelectSlot = () => {
        selectIndex.set (charid);
        selectType.set (settings.buy);
    }

</script>

<div class="auth__characters_block" class:active={$selectIndex === charid} on:click={onSelectSlot}>
    <div class="auth__characters_character">
        <div class="box-column">
            <b>{translateText('player2', 'Слот')}</b>
            <div>{translateText('player2', 'заблокирован')}</div>
        </div>
        <div class="auth__characters_circle red">
            <div class="auth__characters_circle_text">
                <b class="auth-dollar"></b>
            </div>
        </div>
    </div>
    <div class="auth__characters_hover">
        <div>{translateText('player2', 'Разблокировать')}</div>
        <div class="main__button_square large box-center">
            <span class="auth-arrow"/>
        </div>
    </div>
</div>