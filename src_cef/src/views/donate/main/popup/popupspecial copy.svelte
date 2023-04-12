<script>
    export let popupData;
    let popupNabor = false;
</script>
<div class="newdonate__special">
    {#if popupNabor == false}
        <div class="newdonate__stars"/>
        <div class="newdonate__special-title">Специальное предложение</div>
    {/if}
    <div class="newdonate__special-block">
        <div class="newdonate__special-mainimg">
            <div class="newdonate__special-sunduk"/>
        </div>
        <div class="newdonate__special-info">
            {#if popupNabor == false}
            <div class="newdonate__special-title">Золотой сундук <br> Хм, что там?</div>
            <div class="newdonate__special-time">
                <div>Предложение ограничено</div>
                <div class="special-time-img">
                    <div>01:20:21</div>
                </div>
            </div>
            <div class="box-flex">
                <div class="redbucks-img"/>
                <div class="newdonate__special-price">
                    <div class="newdonate__special-current-price">2999 RB</div>
                    <div class="newdonate__special-old-price">4999</div>
                </div>
            </div>
            <div class="newdonate__button_small">
                <div class="newdonate__button-text">Забрать сейчас</div>
            </div>
            <div class="newdonate__special-description">Отложить</div>
            {/if}
            {#if popupNabor == true}
            <div class="newdonate__special-title">Набор новичка</div>
            <div class="newdonate__special-gray">Что входит в набор?</div>
            <div class="newdonate__special-nabor">
                <div class="box-flex">
                    <span class="icon-money"/>
                    <div class="newdonate__special-nabor-item">5000 RB</div>
                </div>
                <div class="box-flex">
                    <span class="icon-money"/>
                    <div class="newdonate__special-nabor-item">3 LVL</div>
                </div>
                <div class="box-flex">
                    <span class="icon-money"/>
                    <div class="newdonate__special-nabor-item">5000 RB</div>
                </div>
            </div>
            <div class="newdonate__special-text">
                Lorem ipsum dolor sit, amet consectetur adipisicing elit. Quam sequi eveniet modi eum molestiae exercitationem!
            </div>
            <div class="newdonate__button_small">
                <div class="newdonate__button-text">Купить за 500</div>
            </div>
            {/if}
        </div>
    </div>
    <div class="newdonate__escape">
        <div class="box-flex">
            <span class="donateicons-esc"/>
            <div class="newdonate__escape-title">ESC</div>
        </div>
        <div class="newdonate__escape-text">
            Нажми, чтобы закрыть предложение <br>
            Учтите, что предложение <b>ограниченное</b>
        </div>
    </div>
</div>