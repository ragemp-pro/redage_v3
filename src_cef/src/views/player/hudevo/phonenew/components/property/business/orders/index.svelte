<script>

    export let onSelectedViewBusiness;

    import List from './list.svelte'
    import Item from './item.svelte'

    const viewsOrder = {
        List,
        Item,
    }

    let selectedViewOrder = "List";
    const onSelectedViewOrder = (view) => selectedViewOrder = view;

    let selectedOrderName = "";
    const onSelectedOrderName = (orderName) => selectedOrderName = orderName;



</script>

<svelte:component this={viewsOrder[selectedViewOrder]} {onSelectedViewBusiness} {onSelectedViewOrder} {onSelectedOrderName} {selectedOrderName} />