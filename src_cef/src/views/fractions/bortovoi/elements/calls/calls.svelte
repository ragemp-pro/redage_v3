<script>
    import { translateText } from 'lang'
</script>
{#if false}
    <div class="box-between">
        <div class="policecomputer__title">{translateText('fractions', 'Бортовой компьютер')}</div>
        <div>3 {translateText('fractions', 'активных вызова')}</div>
    </div>
    <div class="bortovoie__tablet_element title mt-20">
        <div class="number">{translateText('fractions', 'Номер')}:</div>
        <div class="number">{translateText('fractions', 'Расстояние')}:</div>
        <div class="number">{translateText('fractions', 'Статус')}:</div>
        <div class="name">{translateText('fractions', 'Вызвал')}:</div>
        <div class="descr">{translateText('fractions', 'Описание')}:</div>
        <div class="button"></div>
    </div>
    <div class="policecomputer__tablet">
        {#each new Array(20) as item}
            <div class="bortovoie__tablet_element noborder">
                <div class="number">№22</div>
                <div class="number">1234 м.</div>
                <div class="number">CODE 0</div>
                <div class="name">Vitaliy Zdobich</div>
                <div class="descr">Стрельба в районе города</div>
                <div class="button">
                    <div class="policecomputer__button small">{translateText('fractions', 'Подробнее')}</div>
                </div>
            </div>
        {/each}
    </div>
{/if}
{#if true}
    <div class="policecomputer__back">
        &#x2190; {translateText('fractions', 'Вернуться к вызовам')}
    </div>
    <div class="box-between mt-20">
        <div class="box-flex">
            <div class="policecomputer__title mr-20">{translateText('fractions', 'Вызов')} №22</div>
            <div class="policecomputer__info_title">
                <span class="white">{translateText('fractions', 'Вы')}</span>
                {translateText('fractions', 'и')} 2 {translateText('fractions', 'патруля уже приняли вызов')}
            </div>
        </div>
        <div class="policecomputer__button">{translateText('fractions', 'Принять вызов')}</div>
    </div>
    <div class="policecomputer__info_title mt-45">{translateText('fractions', 'Вызвал')}:</div>
    <div class="policecomputer__info_subtitle">Vitaliy Zdobich</div>
    <div class="policecomputer__info_title mt-20">{translateText('fractions', 'Адрес')}:</div>
    <div class="policecomputer__info_subtitle">Рокфорд Хиллс, 12</div>
    <div class="policecomputer__info_title mt-20">{translateText('fractions', 'Сообщение')}:</div>
    <div class="policecomputer__info_subtitle">Стрельба в районе города, двое неизвестных. Судя по выстрелам имеют автоматическое оружие.</div>
    <div class="policecomputer__info_title mt-20">{translateText('fractions', 'Местоположение')}:</div>
    <div class="policecomputer__person_image map mt-4">

    </div>
    <div class="policecomputer__button large mt-12">{translateText('fractions', 'Отметить в GPS')}</div>
{/if}
