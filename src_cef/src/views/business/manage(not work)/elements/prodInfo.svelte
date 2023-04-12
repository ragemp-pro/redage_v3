<script>
    export let selectProd;

</script>

<div class="bizmanage__information">
    <div class="box-flex">
        <div class="bizmanage__information_image"></div>
        <div class="box-column">
            <div class="bizmanage__information_title">{selectProd.Name}</div>
            <div class="bizmanage__information_subtitle">-</div>
            <div class="box-flex">
                <div class="box-column">
                    <div>Наценка</div>
                    <div class="small green">{selectProd.Price * 100 / selectProd.DefaultPrice}%</div>
                </div>
                <div class="box-column">
                    <div>На складе</div>
                    <div class="small">{selectProd.Count} / {selectProd.MaxCount} шт.</div>
                </div>
            </div>
        </div>
    </div>
    <div class="bizmanage__grid">
        <!--<div class="box-flex">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle green">125</div>
                <div class="bizmanage__text">
                    <span class="gray">Продаж сегодня</span>
                </div>
            </div>
        </div>
        <div class="box-flex">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle green">125</div>
                <div class="bizmanage__text">
                    <span class="gray">Продаж за месяц</span>
                </div>
            </div>
        </div>-->
        <div class="box-flex button">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle">Изменить цену</div>
            </div>
        </div>
        <!--<div class="box-flex">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle green">125$</div>
                <div class="bizmanage__text">
                    <span class="gray">Доход сегодня</span>
                </div>
            </div>
        </div>
        <div class="box-flex">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle green">125$</div>
                <div class="bizmanage__text">
                    <span class="gray">Доход за месяц</span>
                </div>
            </div>
        </div>-->
        <div class="box-flex button">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle">Заказать товар</div>
            </div>
        </div>
    </div>
</div>