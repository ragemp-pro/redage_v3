<script>
    import { translateText } from 'lang'
    import { fade } from 'svelte/transition'
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    export let onSelectedView;

    const soundList = [
        {
            name: translateText('player2', 'Аврора'),
            url: "cloud/sound/iphone/notify/aurora.ogg"
        },
        {
            name: translateText('player2', 'Аккорд'),
            url: "cloud/sound/iphone/notify/chord.ogg"
        },
        {
            name: translateText('player2', 'Бамбук'),
            url: "cloud/sound/iphone/notify/bamboo.ogg"
        },
        {
            name: translateText('player2', 'Кружочки'),
            url: "cloud/sound/iphone/notify/circles.ogg"
        },
        {
            name: translateText('player2', 'Успешно'),
            url: "cloud/sound/iphone/notify/complete.ogg"
        },
        {
            name: translateText('player2', 'Приветствие'),
            url: "cloud/sound/iphone/notify/hello.ogg"
        },
        {
            name: translateText('player2', 'Ввод'),
            url: "cloud/sound/iphone/notify/input.ogg"
        },
        {
            name: translateText('player2', 'Ключи'),
            url: "cloud/sound/iphone/notify/keys.ogg"
        },
        {
            name: translateText('player2', 'Нота'),
            url: "cloud/sound/iphone/notify/note.ogg"
        },
        {
            name: translateText('player2', 'Попкорн'),
            url: "cloud/sound/iphone/notify/popcorn.ogg"
        },
        {
            name: translateText('player2', 'Синтезатор'),
            url: "cloud/sound/iphone/notify/synth.ogg"
        },
        {
            name: translateText('player2', 'На цыпочках'),
            url: "cloud/sound/iphone/notify/t1.ogg"
        },
        {
            name: translateText('player2', 'Колокольчик'),
            url: "cloud/sound/iphone/notify/t2.ogg"
        }
    ]
    let data = []

    soundList.forEach((ute) => {
        data.push(ute.url)
    })

    let selectIndex = 0;
    let defaultIndex = 0;
    const onSelectItem = (url, index) => {
        selectIndex = index;

        executeClientToGroup("settings.play", url)
    }

    executeClientAsyncToGroup("settings.smsId").then((result) => {
        selectIndex = result;
        defaultIndex = selectIndex;
    });

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        executeClient ("sounds.stop", "phoneSound")
        if (defaultIndex !== selectIndex)
            executeClientToGroup("settings.smsId", selectIndex)
    });
</script>
<div class="newphone__settings_flex newphone__project_padding16" in:fade>
    <div class="box-flex" on:click={()=> onSelectedView(null)}>
        <div class="phoneicons-Vector-Stroke"></div>
        <div>{translateText('player2', 'Назад')}</div>
    </div>
    <div style="margin-left: 16px">{translateText('player2', 'Уведомления')}</div>
    <div class="box-flex"></div>
</div>
<div class="newphone__contacts_list n-p big">
    {#each soundList as item, index}
    <div class="newphone__settings_element" on:click={() => onSelectItem (item.url, index)}>
        <div class="newphone__settings_icon">
            {#if selectIndex === index}
            <div class="phoneicons-asdasd"></div>
            {/if}
        </div>
        <div class="box-between w-1">
            <div>{item.name}</div>
            <div></div>
        </div>
    </div>
    {/each}
</div>