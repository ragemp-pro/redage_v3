<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";
</script>
<div class="fractions__content backgr extrabig mt-20">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-best"></span>
            <div class="fractions__main_title">{translateText('player1', 'Топ участников')}</div>
        </div>
        <div class="fractions__input large">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} type="text" placeholder={translateText('player1', 'Поиск..')}>
        </div>
    </div>
    <div class="box-between pr-22 pl-8 pr-226">
        <div class="fractions_stats_title fs-14 name mr-16">{translateText('player1', 'Место')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Имя')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Кол-во очков')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Отряд')}:</div>
    </div>
    <div class="fractions__main_scroll big extrabig">
        {#each new Array(15)  as item, index}
            <div class="fractions__scroll_element hover p-20">
                <div class="fractions_stats_title fs-14 name whitecolor box-flex m-0">
                    <div class="fractionsicon-dots"></div>
                    {#if index+1 == "1"}
                        <div class="gold">{index+1}</div>
                    {:else if index+1 == "2"}
                        <div class="silver">{index+1}</div>
                    {:else if index+1 == "3"}
                        <div class="bronze">{index+1}</div>
                    {:else}
                        {index+1}
                    {/if}
                </div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">Vitaliy Zdobich</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">30 XP</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">PA</div>
                <div class="w-166">
                </div>
            </div>
        {/each}
    </div>    
</div>