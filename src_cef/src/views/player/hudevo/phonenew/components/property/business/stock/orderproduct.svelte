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

    let count;

    const countMinOrder = (bType) => (bType == 14 || (bType >= 2 && bType <= 5)) ? 1 : 10;

    const onSelect = () => {
        if (!window.loaderData.delay ("business.addOrder", 1.5))
            return;

        const value = Number (count);

        if (countMinOrder (bizType) > value) {
            window.notificationAdd(4, 9, `${translateText('player2', 'Заказать можно от')} ${countMinOrder (bizType)} ${translateText('player2', 'до')} ${product.maxCount - product.count}`, 3000);
            return;
        }

        if (product.count + value > product.maxCount) {
            window.notificationAdd(4, 9, `${translateText('player2', 'Заказать можно')} ${product.maxCount - product.count}`, 3000);
            return;
        }

        executeClientToGroup("business.addOrder", selectedProductName, value);

        onSelectBack ();
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
    <div class="newphone__project_title w-100 fs-18">{translateText('player2', 'Заказать можно от')} {countMinOrder (bizType)} {translateText('player2', 'до')} {product.maxCount - product.count}:</div>
    <input type="text" class="newphone__ads_input mb-0" placeholder={translateText('player2', 'Введите количество')} bind:value={count} on:focus={onInputFocus} on:blur={onInputBlur} />
    <div class="newphone__project_button auction" on:click={onSelect}>{translateText('player2', 'Заказать')}</div>
    <div class="violet box-center m-top10" on:click={onSelectBack}>{translateText('player2', 'Назад')}</div>
</div>