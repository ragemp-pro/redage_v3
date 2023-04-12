<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage';
    let value = "";
</script>

<h1>{translateText('fractions', 'Снять с розыска')}</h1>
<div class="elements">
    <input bind:value={value} maxLength="30" placeholder={translateText('fractions', 'Паспорт/Имя_Фамилия')}/>
    <div class="button" on:click={() => executeClient ('client:pcMenuInput', "clearWantedLvl", value)}>{translateText('fractions', 'Очистить розыск')}</div>
</div>