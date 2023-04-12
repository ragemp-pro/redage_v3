<script>
    import { translateText } from 'lang'
</script>
<div class="policecomputer__title mt-20">{translateText('fractions', 'Розыск преступников')}</div>
<div class="policecomputer__input_descr">
    ID {translateText('fractions', 'человека')}:
</div>
<div class="policecomputer__input small">
    <input placeholder="Введите ID">
</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Статьи')}:
</div>
<div class="policecomputer__input small">
    <input placeholder="Введите статьи">
</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Описание произошедшего')}:
</div>
<textarea class="policecomputer__textarea" placeholder="Описание.."></textarea>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Приметы преступника')}:
</div>
<textarea class="policecomputer__textarea" placeholder="Приметы.."></textarea>
<div class="policecomputer__button mt-45">{translateText('fractions', 'Объявить в розыск')}</div>