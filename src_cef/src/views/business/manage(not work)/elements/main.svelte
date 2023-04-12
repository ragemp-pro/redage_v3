<script>
    import { format } from 'api/formatter'
    import { setGroup, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'

    setGroup (".businessmanage.");

    let stats = {}
    executeClientAsyncToGroup("getStats").then((result) => {
        if (result && typeof result === "string")
            stats = JSON.parse(result);
    });

    const getDayName = (day) => {
        let dayName = " дней";

        switch (day) {
            case 1:
                dayName = " день"
                break;
            case 2:
            case 3:
            case 5:
                dayName = " дня"
                break;
        }

        return day + dayName;
    }

    let topProd = []
    executeClientAsyncToGroup("getTopProd").then((result) => {
        if (result && typeof result === "string")
            topProd = JSON.parse(result);
    });

    let topPlayers = []
    executeClientAsyncToGroup("getTopPlayers").then((result) => {
        if (result && typeof result === "string")
            topPlayers = JSON.parse(result);
    });
</script>

<div class="bizmanage__main">
    <div class="bizmanage__title">Основная информация</div>
    <div class="box-between">
        <div class="bizmanage__main_info box-center">
            <span class="businessmanage-balance bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle">{getDayName (stats.taxDay)}</div>
                <div class="bizmanage__text">
                    <span class="gray">Налог оплачен</span>
                </div>
            </div>
        </div>
        <!--<div class="bizmanage__main_info box-center">
            <span class="businessmanage-doors bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle green">Открыты</div>
                <div class="bizmanage__text">
                    <span class="gray">Двери</span>
                </div>
            </div>
        </div>
        <div class="bizmanage__main_info box-center">
            <span class="businessmanage-nalog bizmanage__mr-15"></span>
            <div class="box-column">
                <div class="bizmanage__subtitle green">0$</div>
                <div class="bizmanage__text">
                    <span class="gray">Плата за вход</span>
                </div>
            </div>
        </div>-->
    </div>
    <div class="box-between">
        <div class="box-column">
            <div class="bizmanage__title">Популярные товары</div>
            {#each topProd as item}
                <div class="bizmanage__element">
                    <div class="box-flex">
                        <div class="bizmanage__element_image"></div>
                        <div class="box-column">
                            <div class="bizmanage__subtitle">{item.Name}</div>
                            <div class="bizmanage__text">
                                <span class="gray">Название</span>
                            </div>
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="bizmanage__subtitle">{item.Count} шт.</div>
                        <div class="bizmanage__text">
                            <span class="gray">Продано</span>
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="bizmanage__subtitle green">${item.Cost}</div>
                        <div class="bizmanage__text">
                            <span class="gray">Доход</span>
                        </div>
                    </div>
                </div>
            {/each}
        </div>
        <div class="box-column">
            <div class="bizmanage__title">Лучшие покупатели</div>
            {#each topPlayers as item}
                <div class="bizmanage__element">
                    <div class="box-flex">
                        <div class="bizmanage__element_image"></div>
                        <div class="box-column">
                            <div class="bizmanage__subtitle">Гражданин #{item.UUID}</div>
                            <div class="bizmanage__text">
                                <span class="gray">Имя</span>
                            </div>
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="bizmanage__subtitle">{item.Count} шт.</div>
                        <div class="bizmanage__text">
                            <span class="gray">Куплено</span>
                        </div>
                    </div>
                    <div class="box-column">
                        <div class="bizmanage__subtitle green">${item.Cost}</div>
                        <div class="bizmanage__text">
                            <span class="gray">Прибыль</span>
                        </div>
                    </div>
                </div>
            {/each}
        </div>
    </div>
</div>