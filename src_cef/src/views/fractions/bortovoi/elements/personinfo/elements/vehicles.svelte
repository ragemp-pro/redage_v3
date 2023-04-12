<script>
    import { translateText } from 'lang'
</script>
<div class="bortovoie__tablet_element title">
    <div class="number">{translateText('fractions', 'Статус')}:</div>
    <div class="name">{translateText('fractions', 'Модель')}:</div>
    <div class="name">{translateText('fractions', 'Номер машины')}:</div>
    <div class="info">{translateText('fractions', 'Местоположение')}:</div>
</div>
<div class="policecomputer__tablet">
    {#each new Array(1) as item}
        <div class="bortovoie__tablet_element">
            <div class="number wanted">{translateText('fractions', 'В розыске')}</div>
            <div class="name">Bugatti Chiron</div>
            <div class="name">LV2281337SA</div>
            <div class="info hover"><span class="underline">{translateText('fractions', 'Указать в')} GPS</span></div>
        </div>
    {/each}
</div>