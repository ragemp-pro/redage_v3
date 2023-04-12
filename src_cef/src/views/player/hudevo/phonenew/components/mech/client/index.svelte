<script>
    import HomeButton from './../../homebutton.svelte'
    import { translateText } from 'lang'

    export let getPosition;
    export let setPosition;
    export let setOtherElement;
    export let closeMenu;


    import { executeClientAsync, executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'


    let streetName = "";
    let areaName = "";
    const getStreetAndArea = (pos) => {

        executeClientAsync("getStreetName", pos).then((result) => {
            streetName = result;
        })

        executeClientAsync("getAreaName", pos).then((result) => {
            areaName = result;
        });
    }



    //

    const onOrder = () => {
        if (!window.loaderData.delay ("mech.onOrder", 1.5))
            return;

        executeClientToGroup ("mech.order");
    }

    const onCancelOrder = () => {
        if (!window.loaderData.delay ("mech.onOrder", 1.5))
            return;

        executeClientToGroup ("mech.cancel");
    }


    let selectMech = {}

    export let position;

    const getData = () => {
        executeClientAsyncToGroup("mech.getOrder").then((result) => {
            if (result && typeof result === "string") {
                selectMech = JSON.parse(result);

                if (selectMech.pos) {
                    setPosition (selectMech.pos);
                    getStreetAndArea (selectMech.pos);
                } else
                    getStreetAndArea(position);

                setOtherElement (otherElement);
            }
        });
    }

    getData();
    import { addListernEvent } from 'api/functions'
    addListernEvent ("phone.mech.load", getData);
    import { fade } from 'svelte/transition'

    let otherElement;
</script>

<div class="newphone__maps_categories" style="background: linear-gradient(90deg, #FF8A00 0%, #D14E04 94.41%)" in:fade>
    {#if selectMech && selectMech.driver}
        <div class="newphone__maps_price">
            <div class="box-column">
                <div class="newphone__maps_pricetitle">{translateText('player2', 'Водитель')}:</div>
                <div class="newphone__maps_pricesubtitle">{selectMech.driver}</div>
                <div class="newphone__maps_pricetitle">{selectMech.number}</div>
            </div>
            <div class="newphone__maps_car mech"></div>
        </div>
    {/if}

    <div class="newphone__maps_subcategories" bind:this={otherElement} use:setOtherElement>
        <div class="box-between newphone__project_padding20">
            <div class="newphone__maps_header">{translateText('player2', 'Вызов механика')}</div>
            <div class="phoneicons-add1" on:click={closeMenu}></div>
        </div>

        <div class="box-flex newphone__project_padding20 mb-0">
            <div class="newmphone__maps_circle"><div class="newmphone__maps_circle2"></div></div>
            <div class="newphone__maps_column">
                <div class="newphone__column_title">{translateText('player2', 'Место прибытия')}</div>
                <div class="newphone__column_subtitle">{streetName} - {areaName}</div>
            </div>
        </div>
        <div class="newphone__maps_list" style="height: auto; min-height: auto; max-height: auto;">
            {#if selectMech && selectMech.isOrder}
                <div class="newphone__maps_title">{translateText('player2', 'Заказ сделан')}</div>
                <div class="newphone__maps_subtitle">{translateText('player2', 'Ожидайте водителя не уходя от точки вызова. Если вы отойдёте, то заказ будет отменён, а на вас будет наложен штраф.')}
                </div>
                <!--<div class="orange">Стоимость: 2500$</div>-->
                <div class="newphone__project_button" on:click={onCancelOrder}>{translateText('player2', 'Отменить')}!</div>
            {:else}
                <div class="newphone__project_button" on:click={onOrder}>{translateText('player2', 'Заказать')}</div>
            {/if}
            <div class="mechcolor box-center" on:click={closeMenu}>{translateText('player2', 'Закрыть')}</div>
        </div>
        <HomeButton />
    </div>
</div>