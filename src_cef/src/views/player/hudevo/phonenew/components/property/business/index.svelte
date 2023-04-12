<script>
    export let onSelectedView;
    export let selectedId;

    import LoaderSmall from './../../loadersmall.svelte'

    let isLoad = false;//false;
    const updateLoad = () => isLoad = true;

    import { executeClientToGroup } from "api/rage";
    executeClientToGroup ("business.load", selectedId);

    import { addListernEvent } from 'api/functions';
    addListernEvent ("phoneBusinessInit", updateLoad);


    import Menu from './menu.svelte'
    import Stocks from './stock/index.svelte'
    import Orders from './orders/index.svelte'
    import Stats from './stats.svelte'

    import TopClients from './topclients.svelte'
    import TopOrders from './toporders.svelte'

    const viewsBusiness = {
        Menu,
        Stocks,
        Orders,
        Stats,
        TopClients,
        TopOrders
    }

    let selectedViewBusiness = "Menu"

    const onSelectedViewBusiness = (view) => selectedViewBusiness = view;
</script>

{#if !isLoad}
    <LoaderSmall />
{:else}
    <svelte:component this={viewsBusiness[selectedViewBusiness]} {onSelectedView} {selectedId} {onSelectedViewBusiness} />
{/if}
