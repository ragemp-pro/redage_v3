<script>
    import { translateText } from 'lang'
</script>
<div class="bortovoie__tablet_element title">
    <div class="number">{translateText('fractions', 'Номер дома')}:</div>
    <div class="passport">{translateText('fractions', 'Гараж')}:</div>
    <div class="name">{translateText('fractions', 'Владелец')}:</div>
    <div class="status">{translateText('fractions', 'Класс')}:</div>
    <div class="info">{translateText('fractions', 'Местоположение')}:</div>
</div>
<div class="policecomputer__tablet">
    {#each new Array(1) as item}
        <div class="bortovoie__tablet_element">
            <div class="number">№ 228</div>
            <div class="passport">24 {translateText('fractions', 'места')}</div>
            <div class="name">Vitaliy Zdobich</div>
            <div class="status">Люкс +</div>
            <div class="info hover"><span class="underline">{translateText('fractions', 'Указать в GPS')}</span></div>
        </div>
    {/each}
</div>