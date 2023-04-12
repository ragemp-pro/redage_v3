<script>
    import { translateText } from 'lang'
</script>
<div class="bortovoie__tablet_element title">
    <div class="number">{translateText('fractions', 'Дата')}:</div>
    <div class="name">{translateText('fractions', 'Сотрудник')}:</div>
    <div class="name">{translateText('fractions', 'Статьи')}:</div>
    <div class="comment">{translateText('fractions', 'Комментарий')}:</div>
</div>
<div class="policecomputer__tablet">
    {#each new Array(6) as item}
        <div class="bortovoie__tablet_element">
            <div class="number">19.09.21 12:55</div>
            <div class="name">Vitaliy Zdobich</div>
            <div class="name">
                <span class="gray">УК:</span> 23,1 <span class="gray">ТК:</span> 23,1 
            </div>
            <div class="comment">Грубо общался с сотрудниками, оказывал активное сопротивление.</div>
        </div>
    {/each}
</div>