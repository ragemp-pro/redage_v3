<script>
    import {executeClient, executeClientToGroup} from "api/rage";
    export let closeMenu;

    export let selectTrucker;

    const onStartPoint = () => {
        executeClient ("createWaypoint", selectTrucker.pos.x, selectTrucker.pos.y);
        executeClientToGroup ("close");
    }

    const onCancelOrder = () => {
        if (!window.loaderData.delay ("truck.onCancelOrder", 2))
            return;

        executeClientToGroup ("truck.cancel");
    }
    import { fade } from 'svelte/transition'
</script>
<div in:fade class="box-100">
    <div class="box-between newphone__project_padding20">
        <div class="newphone__maps_header">Активные заказы</div>
        <div class="phoneicons-add1" on:click={closeMenu}></div>
    </div>

    <div class="newphone__maps_list">
        <div class="box-flex" style="width: 100%">
            <div class="newmphone__maps_circle"><div class="newmphone__maps_circle2"></div></div>
            <div class="newphone__maps_column">
                <div class="newphone__column_title">{selectTrucker.aStreet}</div>
                <div class="newphone__column_subtitle">{selectTrucker.aArea}</div>
            </div>
        </div>
        <div class="newphone__maps_title">Маршрут построен</div>
        <div class="newphone__maps_subtitle">Точка назначения уже отмечена в вашем GPS. Аккуратно отвезите туда груз, чтобы получить оплату.</div>
        <div class="newphone__project_button mt-0" on:click={onStartPoint}>Показать на карте</div>
        <div class="newphone__project_button mt-0" on:click={onCancelOrder}>Отменить заказ</div>
    </div>
</div>