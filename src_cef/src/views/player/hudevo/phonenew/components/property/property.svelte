<script>
    import { translateText } from 'lang'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'
    import {currentPage} from '../../stores'

    import Furniture from './houses/furniture.svelte'
    import House from './houses/house.svelte'
    import List from './list.svelte'
    import Resident from './houses/resident.svelte'
    import Residents from './houses/residentslist.svelte'
    import Upgrade from './houses/upgrade.svelte'

    import Business from './business/business.svelte'
    import Stock from './business/stock/stock.svelte'
    import Changeprice from './business/stock/changeprice.svelte'
    import Item from './business/stock/item.svelte'
    import Orders from './business/orders/orders.svelte'
    import Order from './business/orders/order.svelte'
    import TopOrders from './business/toporders.svelte'
    import TopClients from './business/topclients.svelte'

    export let selectedItem = null;
    export let selectedPage = null;

    const changePage = (el) => {
        selectedPage = el;
    }

    const changeElement = (el) => {
        selectedItem = el;
    }

    export let propertyList = []
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__rent" in:fade>
    <Header />
    <div class="newphone__rent_content">
        <div class="box-flex newphone__project_padding20 p-top">
            <div class="newphone__maps_headerimage property"></div>
            <div class="newphone__maps_headertitle"><span class="violet">{translateText('player2', 'Управление')} </span>{translateText('player2', 'имуществом')}</div>
        </div>
        {#if propertyList && selectedItem == null && selectedPage == null}
            <List {propertyList} {selectedItem} {changePage} {changeElement}/>
        {/if}
        {#if propertyList == null && selectedPage == null}
            <div class="newphone__rent_none">
                <div class="box-column">
                    <div class="violet">{translateText('player2', 'Имущества нет')}</div>
                    <div class="gray">{translateText('player2', 'Приобрести дом или бизнес можно в риелторском агенстве')}</div>
                </div>
                <div class="newphone__rent_noneimage"></div>
            </div>
            <div class="newphone__project_button violet__background" on:click={()=> currentPage.set("gps")}>{translateText('player2', 'Отметить агенство')}</div>
        {/if}
        {#if selectedItem}
            {#if selectedItem.type == "house" && selectedPage == null}
                <House {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "house" && selectedPage == "residents"}
                <Residents {selectedPage} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "house" && selectedPage == "resident"}
                <Resident {selectedPage} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "house" && selectedPage == "upgrade"}
                <Upgrade {selectedPage} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "house" && selectedPage == "furniture"}
                <Furniture {selectedPage} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == null}
                <Business {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "topclients"}
                <TopClients {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "toporders"}
                <TopOrders {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "stock"}
                <Stock {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "item"}
                <Item {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "changeprice"}
                <Changeprice {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "orders"}
                <Orders {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
            {#if selectedItem.type == "business" && selectedPage == "order"}
                <Order {selectedPage} {selectedItem} {changePage} {changeElement}/>
            {/if}
        {/if}
    </div>
    <HomeButton />
</div>