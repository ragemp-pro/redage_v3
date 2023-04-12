<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage';
    export let SetPopup;

    let selectListId = {
        id: 0,
        name: "Боевой пропуск Season 4",
        class: "reborn", //ХЗ ЧЕ ЭТО)))
        price: 7777,
        img: document.cloud + 'inventoryItems/items/bp.png',
        list: [
            "Более 300 уникальных заданий",
            "Огромное количество уникальной одежды",
            "Два уникальных автомобиля, которые уже ждут тебя",
        ]
    };

    const onSelectMenu = () => {
        executeClient ("client.donate.close");
        executeClient ('client.battlepass.open');
    }


</script>


<div class="main-menu__header-element">
    <div class="main-menu__star-block">
        <div class="star-img" style="background-image: url({selectListId.img})"/>
    </div>
    <div class="main-menu__element-info">
        <div class="main-menu__timer-box">
            <div class="main-menu__timer box-center">{translateText('donate', 'Новое')}</div>
        </div>
        <div class="main-menu__title">{selectListId.name}</div>
        {#if selectListId.list}
        <div class="main-menu__paragraph">
        {#each selectListId.list as text, index}
            <b>- </b>{text}<br />
        {/each} 
        </div>
        {:else if selectListId.desc}
        <div class="main-menu__paragraph">{selectListId.desc}</div>
        {/if}
        <div class="newdonate__button_small" on:click={onSelectMenu}>
            <div class="newdonate__button-text">{translateText('donate', 'Перейти')}</div>
        </div>
    </div>
</div>