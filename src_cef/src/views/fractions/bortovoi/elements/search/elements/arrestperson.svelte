<script>
    import { translateText } from 'lang'
</script>
<div class="policecomputer__back">
    &#x2190; {translateText('fractions', 'Вернуться в поиск')}
</div>
<div class="box-between mt-20">
    <div class="policecomputer__title">{translateText('fractions', 'Оформление в')} DataBase</div>
</div>
<div class="box-flex mtmb-40">
    <div class="policecomputer__person_image"></div>
    <div class="policecomputer__info_grid small">
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Имя')}:</div>
            <div class="policecomputer__info_subtitle">Vitaliy</div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Фамилия')}:</div>
            <div class="policecomputer__info_subtitle">Zdobich</div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Паспорт')}:</div>
            <div class="policecomputer__info_subtitle">2281337</div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Номер телефона')}:</div>
            <div class="box-flex">
                <div class="policecomputer__info_subtitle mr-16">1233213</div>
            </div>
        </div>
    </div>
</div>
<div class="box-flex">
    <div class="box-column mr-20">
        <div class="policecomputer__info_title">{translateText('fractions', 'Время')}:</div>
        <div class="policecomputer__info_subtitle">23.12.20 15:56</div>
    </div>
    <div class="box-column">
        <div class="policecomputer__info_title">{translateText('fractions', 'Уровень розыска')}:</div>
        <div class="box-flex">
            <div class="bortovoiicon-star active mr-4 fz-20"></div>
            <div class="bortovoiicon-star active mr-4 fz-20"></div>
            <div class="bortovoiicon-star mr-4 fz-20"></div>
            <div class="bortovoiicon-star mr-4 fz-20"></div>
            <div class="bortovoiicon-star mr-4 fz-20"></div>
            <div class="bortovoiicon-star fz-20 mr-20"></div>
            <div class="box-flex">
                <div class="policecomputer__button_circle hover mr-10">-</div>
                <div class="policecomputer__button_circle hover">+</div>
            </div>
        </div>
    </div>
</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Статьи')}:
</div>
<div class="policecomputer__input small">
    <input placeholder="Введите статьи">
</div>
<div class="policecomputer__input_descr">
    {translateText('fractions', 'Оставьте свой комментарий')}:
</div>
<textarea class="policecomputer__textarea" placeholder="Описание.."></textarea>
<div class="policecomputer__button mt-45">{translateText('fractions', 'Сделать фото')}</div>