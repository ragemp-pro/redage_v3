<script>
    import { translateText } from 'lang'
</script>
<div class="policecomputer__title">{translateText('fractions', 'Список разыскиваемых')}</div>
<div class="policecomputer__comments big">
    {#each new Array(20) as item}
        <div class="policecomputer__comments_element">
            <div class="box-between">
                <div class="mr-10">{translateText('fractions', 'Дело')} №2281337</div>
                <div class="policecomputer__button">{translateText('fractions', 'Закрыть дело')}</div>
            </div>
            <div class="box-flex mt-4">
                <div class="box-column mr-20">
                    <div class="policecomputer__info_title">{translateText('fractions', 'Имя и фамилия')}:</div>
                    <div class="policecomputer__info_subtitle">Vitaliy Zdobich</div>
                </div>
                <div class="box-column">
                    <div class="policecomputer__info_title">{translateText('fractions', 'Статьи')}:</div>
                    <div class="policecomputer__info_subtitle">
                        <span class="gray">УК:</span> 13,6
                    </div>
                </div>
            </div>
            <div class="policecomputer__info_title mt-24">{translateText('fractions', 'Приметы')}:</div>
            <div class="policecomputer__info_subtitle f-regular">
                Темнокожий блондин, на руке татуировка.
            </div>
            <div class="policecomputer__info_title mt-24">{translateText('fractions', 'Описание')}:</div>
            <div class="policecomputer__info_subtitle f-regular">
                Гражданин напал на сотрудников с огнестрельным оружием. Гражданин напал на сотрудников с огнестрельным оружием. Гражданин напал на сотрудников с огнестрельным оружием.
            </div>
        </div>
    {/each}
</div>