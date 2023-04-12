<script>

    export let onSelectedViewBusiness;

    import List from './list.svelte'
    import Item from './item.svelte'
    import ChangePrice from './changeprice.svelte'
    import OrderProduct from './orderproduct.svelte'

    const viewsProduct = {
        List,
        Item,
        ChangePrice,
        OrderProduct
    }

    let selectedViewProduct = "List";
    const onSelectedViewProduct = (view) => selectedViewProduct = view;

    let selectedProductName = "";
    const onSelectedProductName = (productName) => selectedProductName = productName;



</script>

<svelte:component this={viewsProduct[selectedViewProduct]} {onSelectedViewBusiness} {onSelectedViewProduct} {onSelectedProductName} {selectedProductName} />