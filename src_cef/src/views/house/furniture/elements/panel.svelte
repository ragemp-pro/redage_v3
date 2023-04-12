<script>

    import Residents from './residents.svelte';
    import Garage from './garage.svelte';
    import Upgrades from './upgrades.svelte';
    import Furniture from './furniture.svelte';

    const Views = {
        Residents,
        Garage,
        Upgrades,
        Furniture,
    }

    let ViewsNames = {
        Residents: "Жильцы",
        Garage : "Гараж",
        Upgrades : "Улучшение дома",
        Furniture : "Мебель"
    }

    let SelectViews = "Garage";

    const OnUpdatePage = (page) => {
        SelectViews = page;
    }

</script>
<div class="house__panel">
    <div class="house__header">{ViewsNames[SelectViews]}</div>
    <div class="house__mainmenu">
        <div class="house__mainmenu_block">
            <div class="house__mainmenu_categorie" class:active={SelectViews === "Garage"} on:click={() => OnUpdatePage ("Garage")}>
                <div class="line"></div>
                Гараж
            </div>
            <div class="house__mainmenu_categorie" class:active={SelectViews === "Residents"} on:click={() => OnUpdatePage ("Residents")}>
                <div class="line"></div>
                Жильцы
            </div>
            <div class="house__mainmenu_categorie" class:active={SelectViews === "Upgrades"} on:click={() => OnUpdatePage ("Upgrades")}>
                <div class="line"></div>
                Улучшение дома
            </div>
            <div class="house__mainmenu_categorie" class:active={SelectViews === "Furniture"} on:click={() => OnUpdatePage ("Furniture")}>
                <div class="line"></div>
                Мебель
            </div>
        </div>
        <div class="house__mainmenu_center">
            <svelte:component this={Views[SelectViews]} />
        </div>
        <div class="house__mainmenu_block">
            <div class="house__info small m-0">
                Пятикомнотный особняк в центре Rockford Hills
            </div>
            <div class="house__info_flex">
                <div class="house__menu">Государственная стоимость:</div>
                <div class="house__stat">$1.000.000</div>
            </div>
            <div class="house__info_flex">
                <div class="house__menu">Улучшения</div>
                <div class="box-column">
                    <div class="house__stat">Сигнализация</div>
                    <div class="house__stat">Взломостойкий сейф</div>
                    <div class="house__stat">Шкаф для вещей [x3]</div>
                </div>
            </div>
            <div class="house__info_flex">
                <div class="house__menu">Предложенная стоимость:</div>
                <div class="house__stat">$2.000.000</div>
            </div>
        </div>
    </div>
    <div class="box-between">
        <div class="house_bottom_buttons back">
            <div>Выбрать</div>
            <div class="house_bottom_button">Enter</div>
        </div>
        <div class="house_bottom_buttons esc">
            <div>Выйти</div>
            <div class="house_bottom_button">ESC</div>
        </div>
    </div>
    <div class="houseicon-house house__background"></div>
</div>