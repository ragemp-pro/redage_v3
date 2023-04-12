<script>
    import { translateText } from 'lang'
    import Houses from './elements/houses.svelte'
    import Vehicles from './elements/vehicles.svelte'
    import Licenses from './elements/licenses.svelte'
    import History from './elements/history.svelte'
    import Comments from './elements/comments.svelte'

    const Views = {
        Houses,
        Vehicles,
        Licenses,
        History,
        Comments
    }

    let selectView = "Houses"
</script>
<div class="policecomputer__back">
    &#x2190; {translateText('fractions', 'Вернуться в поиск')}
</div>
<div class="box-between mt-20">
    <div class="policecomputer__title">Vitaliy Zdobich</div>
    <div class="box-flex">
        <div class="policecomputer__button mr-16">{translateText('fractions', 'Объявить в розыск')}</div>
        <div class="policecomputer__button">{translateText('fractions', 'Оформить')} DataBase</div>
    </div>
</div>
<div class="box-between mtmb-40">
    <div class="policecomputer__person_image"></div>
    <div class="policecomputer__info_grid">
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
                <div class="policecomputer__button_search">
                    <span class="bortovoiicon-Vector"></span>
                    <div>{translateText('fractions', 'Отследить')}</div>
                </div>
            </div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Статус')}:</div>
            <div class="policecomputer__info_subtitle wanted">{translateText('fractions', 'В розыске')}</div>
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
        <div class="box-column">
            <div class="policecomputer__info_title"> </div>
            <div class="box-flex">
                <div class="policecomputer__button_circle hover mr-10">-</div>
                <div class="policecomputer__button_circle hover">+</div>
            </div>
        </div>
        <div class="box-column">
            <div class="policecomputer__info_title">{translateText('fractions', 'Статьи')}:</div>
            <div class="policecomputer__info_subtitle">
                <span class="gray">{translateText('fractions', 'УК')}:</span> 23,1 <span class="gray">{translateText('fractions', 'ТК')}:</span> 23,1
            </div>
        </div>
    </div>
</div>
{#if true}
    <div class="policecomputer__selectors">
        <div class="policecomputer__selector" class:active={selectView == "Houses"} on:click={() => selectView = "Houses"}>{translateText('fractions', 'Недвижимость')}</div>
        <div class="policecomputer__selector" class:active={selectView == "Vehicles"} on:click={() => selectView = "Vehicles"}>{translateText('fractions', 'Транспорт')}</div>
        <div class="policecomputer__selector" class:active={selectView == "Licenses"} on:click={() => selectView = "Licenses"}>{translateText('fractions', 'Лицензии')}</div>
        <div class="policecomputer__selector" class:active={selectView == "History"} on:click={() => selectView = "History"}>{translateText('fractions', 'История нарушений')}</div>
        <div class="policecomputer__selector" class:active={selectView == "Comments"} on:click={() => selectView = "Comments"}>{translateText('fractions', 'Комментарии')}</div>
    </div>

    <svelte:component this={Views[selectView]} />
{/if}
{#if false}
    <div class="policecomputer__center">{translateText('fractions', 'Нет информации')}</div>
{/if}