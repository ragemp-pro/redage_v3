<script>
    import { translateText } from 'lang'
</script>
<div class="policecomputer__title mt-20">{translateText('fractions', 'Розыск транспортного средства')}</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Номер машины')}:
</div>
<div class="policecomputer__input small">
    <input placeholder="Введите номер">
</div>
<div class="policecomputer__input_descr mt-12">
    <div class="box-between">
        <div class="box-flex">{translateText('fractions', 'Владелец')}: <span class="white">Vitaliy_Zdobich</span> {translateText('fractions', 'Подробнее о владельце')} ></div>
        <div class="box-flex">{translateText('fractions', 'Модель')}: <span class="white">Lambo112</span> </div>
    </div>
</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Статьи ДК')}:
</div>
<div class="policecomputer__input small">
    <input placeholder="Введите статьи">
</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Описание произошедшего')}:
</div>
<textarea class="policecomputer__textarea" placeholder="Описание.."></textarea>
<div class="box-flex">
    <div class="policecomputer__button mt-45 mr-16">{translateText('fractions', 'Объявить в розыск')}</div>
    <div class="policecomputer__button mt-45">{translateText('fractions', 'Сделать фото')}</div>
</div>