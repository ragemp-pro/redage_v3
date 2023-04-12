<script>
    import HomeButton from './../../homebutton.svelte'

    export let setPosition;
    export let setOtherElement;
    export let closeMenu;

    import { format } from "api/formatter";

    import { executeClientAsyncToGroup} from 'api/rage'

    let selectTaxi = {}

    const getCounter = () => {
        executeClientAsyncToGroup("taxi.getCounter").then((result) => {
            if (result && typeof result === "string") {
                selectTaxi = JSON.parse(result);

                setPosition (selectTaxi.pos);

                setOtherElement (otherElement);
            }
        });
    }

    getCounter();
    import { addListernEvent } from 'api/functions';
    addListernEvent ("phone.taxi.updateCounter", getCounter);


    import { fade } from 'svelte/transition'

    let otherElement;
</script>

{#if selectTaxi && selectTaxi.name}
<div class="newphone__maps_categories" style="background: linear-gradient(90deg, #FF8A00 0%, #D14E04 94.41%)" in:fade>
    <div class="newphone__maps_price">
        <div class="box-column">
            <div class="newphone__maps_pricetitle">{selectTaxi.isDriver ? "Водитель" : "Пассажир"}:</div>
            <div class="newphone__maps_pricesubtitle">{selectTaxi.name}</div>
        </div>
        <div class="newphone__maps_car"></div>
    </div>

    <div class="newphone__maps_subcategories" bind:this={otherElement} use:setOtherElement>
        <div class="box-between newphone__project_padding20">
            <div class="newphone__maps_header">Активные заказы</div>
            <div class="phoneicons-add1" on:click={closeMenu}></div>
        </div>

        <div class="newphone__maps_list" style="height: auto; min-height: auto; max-height: auto;">
            <div class="orange">Стоимость: ${format("money", selectTaxi.price)}</div>
            <div class="orange box-center" on:click={closeMenu}>Закрыть</div>
        </div>
        <HomeButton />
    </div>
</div>
{/if}