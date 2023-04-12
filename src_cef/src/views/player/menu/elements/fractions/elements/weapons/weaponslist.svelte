<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
</script>
<div class="fractions__content backgr extrabig">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-squads"></span>
            <div class="fractions__main_title">{translateText('player1', 'Список отрядов')}</div>
        </div>
        <div class="fractions__input large">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder={translateText('player1', 'Поиск..')}>
        </div>
        <div class="fractions__main_button extra">
            <span class="fractionsicon-plus mr-5"></span>
            {translateText('player1', 'Создать отряд')}
        </div>
    </div>
    <div class="box-between pr-22 pl-8 pr-210">
        <div class="fractions_stats_title fs-14 name mr-8">{translateText('player1', 'Название')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Ранг')}:</div>
        <div class="fractions_stats_title fs-14 descr">{translateText('player1', 'Снаряжение')}:</div>
        <div class="fractions_stats_title fs-14 date">{translateText('player1', 'Участники')}:</div>
    </div>
    <div class="fractions__main_scroll big extrabig">
        {#each new Array(45)  as item}
            <div class="fractions__scroll_element mb-0 hover p-20">
                <div class="fractions_stats_title fs-14 name whitecolor m-0">Набор педика</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">12</div>
                <div class="fractions_stats_title fs-14 descr whitecolor m-0 box-flex">
                    <div class="fractions__weapon_box"></div>
                    <div class="fractions__weapon_box"></div>
                    <div class="fractions__weapon_box"></div>
                    <div class="fractions__weapon_box"></div>
                    <div class="fractions__weapon_box"></div>
                </div>
                <div class="fractions_stats_title fs-14 date whitecolor m-0">37 чел.</div>
                <div class="box-flex hidden">
                    <div class="fractions__main_button black">{translateText('player1', 'Редактировать')}</div>
                    <div class="fractionsicon-fired"></div>
                </div>
            </div>
        {/each}
    </div>    
</div>