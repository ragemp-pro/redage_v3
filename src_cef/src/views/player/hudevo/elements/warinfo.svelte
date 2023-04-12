<script>
    import { fly } from 'svelte/transition';

    import { addListernEvent } from "api/functions";

    import { typeBattle, composition, weaponsCategory } from '@/popups/war/data'

    let visible = false;

    let isAttack = false;
    let mapName = "";
    let selectTypeBattle = 0;
    let time = "";
    let selectComposition = 0;
    let selectWeaponsCategory = 0;
    addListernEvent ("hud.war.info", (_visible, _isAttack, _mapName, _selectTypeBattle, _time, _selectComposition, _selectWeaponsCategory) => {
        if (_visible) {
            isAttack = _isAttack;
            mapName = _mapName;
            selectTypeBattle = _selectTypeBattle;
            time = _time;
            selectComposition = _selectComposition;
            selectWeaponsCategory = _selectWeaponsCategory;
        }
        visible = _visible;
    });
</script>

{#if visible}
<div class="hudevo__event" in:fly={{ x: 50, duration: 500 }} out:fly={{ x: 50, duration: 250 }}>
    <div class="hudevo__notification_subtitle">Война за влияние</div>
    <div class="hudevo__notification_title">
        {isAttack ? 'Вы забили войну' : 'Вам забили войну'}
    </div>
    <div class="hudevo__notification_text medium">
        {#if mapName && mapName.length > 1}
            <div>Зона: <b>{mapName}</b></div>
        {/if}
        <div>Тип битвы: <b>{typeBattle[selectTypeBattle]}</b></div>
        <div>Время: <b>{time}</b></div>
        <div>Кол-во: <b>{selectComposition == -1 ? composition [0] : composition [selectComposition] }</b></div>
        <div>Оружие: <b>{weaponsCategory [selectWeaponsCategory] }</b></div>
    </div>
</div>
{/if}