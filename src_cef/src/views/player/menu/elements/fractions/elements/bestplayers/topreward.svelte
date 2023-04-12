<script>
    import { translateText } from 'lang'
</script>
<div class="fractions__content backgr h-fit mt-20">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-best"></span>
            <div class="fractions__main_title">{translateText('player1', 'Топ участников')}</div>
        </div>
    </div>
    <div class="fractions__main_grid mt-40">
        {#each new Array(6) as item, index}
            <div class="fractions__element_black">
                <div class="fractions__black_img"></div>
                <div class="box-column">
                    <div class="fractions__black_title">{translateText('player1', 'Вертолётный кейс')}</div>
                    <div class="fractions__black_title gray">{index+1} {translateText('player1', 'место')}</div>
                    <div class="fractions__black_subtitle">{translateText('player1', 'Уникальный вертолётный кейс, лучшая награда для лучшего работника')}!</div>
                </div>
            </div>
        {/each}
    </div>
</div>