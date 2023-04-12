<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup} from "api/rage";
    import {addListernEvent, hasJsonStructure} from "api/functions";
    import { getPrefix } from './../data'

    export let selectedProductName;
    export let onSelectedViewProduct;
    export let onSelectedProductName;

    let product = {};

    const updateData = () => {
        executeClientAsyncToGroup("business.getProduct", selectedProductName).then((result) => {
            if (hasJsonStructure(result))
                product = JSON.parse(result);
        });
    }
    updateData ();
    addListernEvent ("phoneBusinessUpdate", updateData);

    let bizType = 0;
    executeClientAsyncToGroup("business.getType").then((result) => {
        bizType = result;
    });

    let price;

    const onSelect = () => {
        if (!window.loaderData.delay ("business.extraCharge", 1.5))
            return;

        const value = Number (price);

        if (product.minPrice > value || value > product.maxPrice ) {
            window.notificationAdd(4, 9, `${translateText('player2', 'Наценка не может быть меньше')} ${product.minPrice}${getPrefix (selectedProductName, bizType)} и больше чем ${product.maxPrice}${getPrefix (selectedProductName, bizType)}`, 3000);
            return;
        }

        executeClientToGroup("business.extraCharge", selectedProductName, value);
    }

    const onSelectBack = () => {
        onSelectedViewProduct ("Item");
    }
    import { fade } from 'svelte/transition'
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>
<div class="newphone__rent_list" in:fade>
    <div class="newphone__project_title w-100 fs-18">{translateText('player2', 'Введите наценку от')} {product.minPrice}{getPrefix (selectedProductName, bizType)} до {product.maxPrice}{getPrefix (selectedProductName, bizType)}:</div>
    <input type="text" class="newphone__ads_input mb-0" placeholder="Введите наценку" bind:value={price} on:focus={onInputFocus} on:blur={onInputBlur} />
    <div class="newphone__project_button auction" on:click={onSelect}>{translateText('player2', 'Обновить')}</div>
    <div class="violet box-center m-top10" on:click={onSelectBack}>{translateText('player2', 'Назад')}</div>
</div>