<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage';
    export let model;
    export let owner;

    let value = "";
</script>

<h1>{translateText('fractions', 'База номеров')}</h1>
<div class="elements">
    <input bind:value={value} maxLength="8" placeholder={translateText('fractions', 'Номер')}/>
    <div class="button" on:click={() => executeClient ('client:pcMenuInput', "checkNumber", value)}>{translateText('fractions', 'Пробить')}</div>
    
    <p>{translateText('fractions', 'МАРКА')}: <span>{model}</span></p>
    <p>{translateText('fractions', 'ВЛАДЕЛЕЦ')}: <span>{owner}</span></p>
</div>