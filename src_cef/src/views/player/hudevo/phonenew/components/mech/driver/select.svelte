<script>
    import { translateText } from 'lang'
    import {executeClient, executeClientToGroup} from "api/rage";
    export let closeMenu;

    export let selectMech;

    const onStartPoint = () => {
        executeClient ("createWaypoint", selectMech.pos.x, selectMech.pos.y);
        executeClientToGroup ("close");
    }

    const onCancelOrder = () => {
        if (!window.loaderData.delay ("mechjob.onCancelOrder", 2))
            return;

        executeClientToGroup ("mechjob.cancel");
    }
    import { fade } from 'svelte/transition'

</script>

<div in:fade class="box-100">
    <div class="box-between newphone__project_padding20">
        <div class="newphone__maps_header">{translateText('player2', 'Активные заказы')}</div>
        <div class="phoneicons-add1" on:click={closeMenu}></div>
    </div>
    
    <div class="newphone__maps_list">
        <div class="box-flex" style="width: 100%">
            <div class="newmphone__maps_circle"><div class="newmphone__maps_circle2"></div></div>
            <div class="newphone__maps_column">
                <div class="newphone__column_title">{selectMech.aStreet}</div>
                <div class="newphone__column_subtitle">{selectMech.aArea}</div>
            </div>
        </div>
        <div class="newphone__maps_title">{translateText('player2', 'Маршрут построен')}</div>
        <div class="newphone__maps_subtitle">{translateText('player2', 'Точка назначения уже отмечена в вашем GPS. Не забывайте про вежливость и учтивость по отношению к клиенту!')}</div>
        <div class="newphone__project_button mt-0" on:click={onStartPoint}>{translateText('player2', 'Показать на карте')}</div>
        <div class="newphone__project_button mt-0" on:click={onCancelOrder}>{translateText('player2', 'Отменить заказ')}</div>
    </div>
</div>