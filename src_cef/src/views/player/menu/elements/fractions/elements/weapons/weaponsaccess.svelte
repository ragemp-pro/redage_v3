<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
</script>
<div class="fractions__stats_subtitle gray hover mt-20 large">&#x2190; {translateText('player1', 'Вернуться к списку наборов')}</div>
<div class="box-between mt-20 not-center">
    <div class="box-column">
        <div class="fractions__main_box h-568">
            <div class="fractions__main_head box-between">
                <div class="box-flex">
                    <span class="fractionsicon-rank"></span>
                    <div class="fractions__main_title">1 {translateText('player1', 'ранг')} - General</div>
                </div>
            </div>
            <div class="box-flex">
                <div class="fractions__selector active">
                    {translateText('player1', 'Доступы')}
                </div>
                <div class="fractions__selector">
                    {translateText('player1', 'Крафт')}
                </div>
            </div>
            <div class="fractions__main_scroll big">
                {#each new Array(12) as item}
                    <div class="fractions__scroll_element">
                        <div>Принимать людей</div>
                        <div class="fractions__access_box">
                            <div class="fractions__access_element decline active"><div class="fractionsicon-decline"></div></div>
                            <div class="fractions__access_element neutral"><div class="fractionsicon-neutral"></div></div>
                            <div class="fractions__access_element accept"><div class="fractionsicon-check"></div></div>
                        </div>
                    </div>
                {/each}
            </div>
        </div>
    </div>
    <div class="box-column">
        <div class="fractions__main_box w-317">
            <div class="fractions__main_head">
                <span class="fractionsicon-rank"></span>
                <div class="fractions__main_title">{translateText('player1', 'Сменить название')}</div>
            </div>
            <div class="fractions__input w-269">
                <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder={translateText('player1', 'Введите название..')}>
            </div>
            <div class="fractions__main_button h-40">{translateText('player1', 'Изменить')}</div>
        </div>
        <div class="fractions__main_box w-317h272 mt-18">
            <div class="fractions__main_head">
                <span class="fractionsicon-rank"></span>
                <div class="fractions__main_title">{translateText('player1', 'Оружие в наборе')}</div>
            </div>
            <div class="fractions__main_grid large">
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
                <div class="fractions__weapon_box">+</div>
            </div>
            <div class="fractions__main_button h-40">{translateText('player1', 'Изменить')}</div>
        </div>
    </div>
</div>