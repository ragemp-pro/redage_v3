<script>
    export let onChangePage;
    import { translateText } from 'lang'

</script>
<div class="policecomputer__back" on:click={() => onChangePage("DataBase")}>
    &#x2190; {translateText('fractions', 'Вернуться в')} DataBase
</div>
<div class="box-between mt-20">
    <div class="policecomputer__title">DataBase №43332</div>
    <div>{translateText('fractions', 'Арестован')} 12 марта 2020 в 15:36</div>
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
            <div class="bortovoiicon-star fz-20"></div>
        </div>
    </div>
</div>
<div class="policecomputer__info_title mt-24">{translateText('fractions', 'Статьи')}:</div>
<div class="policecomputer__info_subtitle">
    <span class="gray">УК:</span> 14,5
</div>
<div class="policecomputer__info_title mt-24">{translateText('fractions', 'Комментарий сотрудника')}:</div>
<div class="policecomputer__info_subtitle">
    {translateText('fractions', 'Гражданин напал на сотрудников с огнестрельным оружием. Гражданин напал на сотрудников с огнестрельным оружием. Гражданин напал на сотрудников с огнестрельным оружием.')}
</div>
{#if false}
    <div class="box-flex mt-45">
        <div class="policecomputer__button mr-20">{translateText('fractions', 'Перейти в досье')}</div>
        <div class="policecomputer__button delete">{translateText('fractions', 'Удалить')} DataBase</div>
    </div>
{/if}
{#if true}
    <div class="box-flex mt-45">
        <div class="policecomputer__info_title m-0 mr-20">{translateText('fractions', 'Вы уверены, что хотите удалить DataBase')}?</div>
        <div class="policecomputer__button delete mr-20">{translateText('fractions', 'Да, удалить')}</div>
        <div class="policecomputer__button">{translateText('fractions', 'Нет, отмена')}</div>
    </div>
{/if}