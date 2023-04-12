<script>
    import { translateText } from 'lang'
    import Logo from '../other/logo.svelte'
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo/>
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-gov"></span>
                <div class="fractions__header_title">{translateText('player1', 'Война за территории')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Здесь вы можете управлять владениями вашей организации')}</div>
        </div>
    </div>
    <div class="fractions__main_button extra mr-10">{translateText('player1', 'Режим карты')}</div>
</div>
<div class="fractions__content backgr h-fit">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-gov"></span>
            <div class="fractions__main_title">{translateText('player1', 'Список территорий')}</div>
        </div>
    </div>
    <div class="fractions__notification">
        <div class="box-flex">
            <div class="fractionsicon-warning"></div>
            <div>Внимание!</div>
        </div>
        <div class="fractions__notification_text">
            За бизнес №24 забита война, в 20:00, будь готов!
        </div>
    </div>
    <div class="fractions__war_grid">
        {#each new Array(12) as item}
        <div class="fractions__war_element">
            <div class="fractions__war_smallgrid">
                {#each new Array(18) as item, i}
                    <div class="fractions__war_border">
                        <div class="fractions__war_biz lcn">{i+1}</div>
                    </div>
                {/each}
            </div>  
            <div class="fractions__war_stats">
                <div>АЗС</div>
                <div class="fractions__war_price">$ 300/час</div>
            </div>
        </div>
        {/each}
    </div>
</div>