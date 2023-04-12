<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
    import Logo from '../other/logo.svelte'
</script>
<div class="box-column">
    <div class="fractions__header  w-726">
        <div class="box-flex">
            <Logo/>
            <div class="box-column">
                <div class="box-flex">
                    <span class="fractionsicon-warning"></span>
                    <div class="fractions__header_title">{translateText('player1', 'Список жалоб')}</div>
                </div>
                <div class="fractions__header_subtitle">{translateText('player1', 'Здесь отображается информация о жалобах на сотрудников')}</div>
            </div>
        </div>
    </div>
    <div class="fractions__content h-fit">
        <div class="fractions__main_head box-between mb-0  w-726">
            <div class="fractions__input w-522">
                <div class="fractionsicon-loop"></div>
                <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder="Поиск..">
            </div>
            <div class="fractions__main_button complaints">{translateText('player1', 'Удалить все жалобы')}</div>
        </div>
        <div class="fractions__main_scroll extrabig mt-40 w-745">
            {#each new Array(6) as item, index}
                <div class="fractions__complaints_element  w-726">
                    <div class="box-between">
                        <div class="fractions__complaints_text">
                            Жалоба на <span class="whitecolor">Vitaliy Zdobich - 6 ранг, CID</span>
                        </div>
                        <div class="fractions__complaints_text">21:30 12.01.21</div>
                    </div>
                    <div class="fractions__complaints_title">Никита ахуел</div>
                    <div class="fractions__complaints_text whitecolor">Данный офицер нарушил все пункты процессуального кодекса! Не вел запись с бодикамеры и позволял себе много лишнего! Прошу наказать его, иначе дело закончится судом!</div>
                    <div class="fractions__complaints_text mt-16">
                        Автор <span class="whitecolor">Vitaliy Zdobich</span>
                    </div>
                </div>
            {/each}
        </div>
    </div>
</div>