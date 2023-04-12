<script>
    import { translateText } from 'lang'
    import { executeClientAsyncToGroup, executeClientToGroup, executeClient } from "api/rage";
    export let updateHeightMap;

    export let selectedCategory;
    export let selectedList;
    export let setPosition;

    export let closeMenu;

    let data = {}

    executeClientAsyncToGroup("gps.getItem", {
        index: selectedCategory,
        id: selectedList
    }).then((result) => {
        if (result && typeof result === "string") {
            data = JSON.parse(result);

            setPosition(data.pos);

            updateHeightMap ();
        }
    });

    const onStartPoint = () => {
        let point = {}

        point.name = data.name;
        point.x = data.pos.x;
        point.y = data.pos.y;

        //executeClient ("gps.setPoint", JSON.stringify(point));

        executeClient ("createWaypoint", data.pos.x, data.pos.y);
        executeClientToGroup ("close");

    }
    import { fade } from 'svelte/transition'
</script>

<div class="box-between newphone__project_padding20" in:fade>
    <div class="newphone__maps_header">{translateText('player2', 'Категории')}</div>
    <div class="phoneicons-add1" on:click={closeMenu}></div>
</div>

<div class="newphone__maps_list" in:fade>
    <div class="newphone__maps_selected">
        <div class="newphone__maps_square">
            <div class="phoneicons-{data.icon} newphone__maps_elementicon"></div>
        </div>
        <div class="box-column" on:click={onStartPoint}>
            <div class="newphone__maps_headertitle">{data.name}</div>
            <div class="newphone__maps_headersubtitle">{data.dist} м.</div>
        </div>
        <div class="phoneicons-Button newphone__chevron-back"></div>
    </div>
    <div class="newphone__maps_title">{translateText('player2', 'Маршрут построен')}</div>
    <div class="newphone__maps_subtitle">{translateText('player2', 'Точка назначения выставлена на мини-карте сверху. Мы проложили самый короткий маршут до неё, чтобы передать маршрут в навигатор - нажмите Вперёд!. Приятной дороги!')}</div>
    <div class="newphone__project_button" on:click={onStartPoint}>{translateText('player2', 'Вперёд')}!</div>
</div>