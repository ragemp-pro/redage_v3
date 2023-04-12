<script>
    import { translateText } from 'lang'
</script>
<div class="policecomputer__back">
    &#x2190; {translateText('fractions', 'Вернуться в поиск')}
</div>
<div class="box-between mt-20">
    <div class="policecomputer__title">Lambo 220</div>
    <div class="box-flex">
        <div class="policecomputer__button mr-16">{translateText('fractions', 'Объявить ТС в розыск')}</div>
    </div>
</div>
<div class="box-between mtmb-40">
    <div class="policecomputer__person_image car"></div>
    <div class="policecomputer__info_grid small">
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Модель')}:</div>
            <div class="policecomputer__info_subtitle">Lambo 770</div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Номер')}:</div>
            <div class="policecomputer__info_subtitle">LV33223SA</div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Статус')}:</div>
            <div class="policecomputer__info_subtitle">Нет нарушений</div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Владелец')}:</div>
            <div class="box-flex w-max">
                <div class="policecomputer__info_subtitle mr-10">Vitaliy Zdobich</div>
                <div class="policecomputer__button_search">
                    {translateText('fractions', 'Изучить досье')} >
                </div>
            </div>
        </div>
    </div>
</div>
<div class="policecomputer__title mtmb-3620">{translateText('fractions', 'Список штрафов')}</div>
<div class="bortovoie__tablet_element title">
    <div class="number">{translateText('fractions', 'Дата')}:</div>
    <div class="name">{translateText('fractions', 'Сотрудник')}:</div>
    <div class="name">{translateText('fractions', 'Статьи')}:</div>
    <div class="info">{translateText('fractions', 'Сумма штрафа')}:</div>
</div>
<div class="policecomputer__tablet">
    {#each new Array(1) as item}
        <div class="bortovoie__tablet_element">
            <div class="number wanted">12.05.20 15:56</div>
            <div class="name">Vitaliy Zdobich</div>
            <div class="name">
                <span class="gray">УК:</span> 23,1 <span class="gray">ТК:</span> 23,1 
            </div>
            <div class="info hover">5000$</div>
        </div>
    {/each}
</div>