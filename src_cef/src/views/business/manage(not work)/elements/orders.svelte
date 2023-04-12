<script>

    import { format } from 'api/formatter'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'

    let orders = []
    executeClientAsyncToGroup("getOrders").then((result) => {
        if (result && typeof result === "string")
            orders = JSON.parse(result);
    });

    const onCancel = (id) => {
        if (!window.loaderData.delay ("battlePass.onTake", 1))
            return;
        
        executeClientToGroup("cancelOrder", id)
    }
</script>


<div class="bizmanage__grid">
    {#each orders as item}
        <div class="bizmanage__grid_element">
            <div class="bizmanage__grid_image"></div>
            <div class="bizmanage__grid_title">{item.Name}</div>
            <div class="bizmanage__grid_subtitle">Заказано {item.Count} шт.</div>
            <div class="bizmanage__grid_button box-center">Отменить заказ</div>
        </div>
    {/each}
</div>