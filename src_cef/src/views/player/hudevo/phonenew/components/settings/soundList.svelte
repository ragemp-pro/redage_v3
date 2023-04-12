<script>
    import { translateText } from 'lang'
    import { fade } from 'svelte/transition'
    import {executeClient, executeClientAsyncToGroup, executeClientToGroup} from "api/rage";
    export let onSelectedView;

    const soundList = [
        {
            name: translateText('player2', 'Jingle Bells'),
            url: "cloud/sound/iphone/calls/jingle.ogg"
        },
        {
            name: translateText('player2', 'Кабы не было зимы'),
            url: "cloud/sound/iphone/calls/kaby.ogg"
        },
        {
            name: translateText('player2', 'Last Christmas'),
            url: "cloud/sound/iphone/calls/lastc.ogg"
        },
        {
            name: translateText('player2', 'Новогодняя 1'),
            url: "cloud/sound/iphone/calls/mczali.ogg"
        },
        {
            name: translateText('player2', 'Новогодняя 2'),
            url: "cloud/sound/iphone/calls/ngbaby.ogg"
        },
        
        {
            name: translateText('player2', 'Стандартный 1'),
            url: "cloud/sound/iphone/calls/call1.ogg"
        },
        {
            name: translateText('player2', 'Стандартный 2'),
            url: "cloud/sound/iphone/calls/call2.ogg"
        },
        {
            name: translateText('player2', 'Маяк'),
            url: "cloud/sound/iphone/calls/beacon.ogg"
        },
        {
            name: translateText('player2', 'Перезвон'),
            url: "cloud/sound/iphone/calls/chimes.ogg"
        },
        {
            name: translateText('player2', 'Электросхема'),
            url: "cloud/sound/iphone/calls/circuit.ogg"
        },
        {
            name: translateText('player2', 'Волны'),
            url: "cloud/sound/iphone/calls/constellation.ogg"
        },
        {
            name: translateText('player2', 'Космос'),
            url: "cloud/sound/iphone/calls/cosmic.ogg"
        },
        {
            name: translateText('player2', 'Кристаллы'),
            url: "cloud/sound/iphone/calls/crystals.ogg"
        },
        {
            name: translateText('player2', 'Сова'),
            url: "cloud/sound/iphone/calls/night-owl.ogg"
        },
        {
            name: translateText('player2', 'Час потехи'),
            url: "cloud/sound/iphone/calls/playtime.ogg"
        },
        {
            name: translateText('player2', 'Скорей, скорей'),
            url: "cloud/sound/iphone/calls/presto.ogg"
        },
        {
            name: translateText('player2', 'Радар'),
            url: "cloud/sound/iphone/calls/radar.ogg"
        },
        {
            name: translateText('player2', 'Свечение'),
            url: "cloud/sound/iphone/calls/radiate.ogg"
        },
        {
            name: translateText('player2', 'Зыбь'),
            url: "cloud/sound/iphone/calls/ripples.ogg"
        },
        {
            name: translateText('player2', 'Сентя'),
            url: "cloud/sound/iphone/calls/sencha.ogg"
        },
        {
            name: translateText('player2', 'Сигнал'),
            url: "cloud/sound/iphone/calls/signal.ogg"
        },
        {
            name: translateText('player2', 'Шёлк'),
            url: "cloud/sound/iphone/calls/silk.ogg"
        },
        {
            name: translateText('player2', 'Медленно в гору'),
            url: "cloud/sound/iphone/calls/slow-rise.ogg"
        },
        {
            name: translateText('player2', 'Мерцание'),
            url: "cloud/sound/iphone/calls/stargaze.ogg"
        },
        {
            name: translateText('player2', 'Вступление'),
            url: "cloud/sound/iphone/calls/summit.ogg"
        },
        {
            name: translateText('player2', 'Свечение'),
            url: "cloud/sound/iphone/calls/twinkle.ogg"
        },
        {
            name: translateText('player2', 'Подъем'),
            url: "cloud/sound/iphone/calls/uplift.ogg"
        },
        {
            name: translateText('player2', 'Волны'),
            url: "cloud/sound/iphone/calls/waves.ogg"
        },
        {
            name: "Lida",
            url: "cloud/sound/iphone/calls/lida.ogg"
        },
        {
            name: translateText('player2', 'Снова я напиваюсь'),
            url: "cloud/sound/iphone/calls/snova.ogg"
        },
        {
            name: translateText('player2', 'Белла, чао!'),
            url: "cloud/sound/iphone/calls/bella.ogg"
        },
        {
            name: translateText('player2', 'Миллион дорог'),
            url: "cloud/sound/iphone/calls/million.ogg"
        },
        {
            name: "Astral Step",
            url: "cloud/sound/iphone/calls/astral.ogg"
        },
        {
            name: translateText('player2', 'Ауф'),
            url: "cloud/sound/iphone/calls/auf.ogg"
        },
        {
            name: translateText('player2', 'Пёсик'),
            url: "cloud/sound/iphone/calls/pes.ogg"
        },
        {
            name: translateText('player2', 'Братва на связи'),
            url: "cloud/sound/iphone/calls/bratva.ogg"
        },
        {
            name: translateText('player2', 'Человечек'),
            url: "cloud/sound/iphone/calls/chelovek.ogg"
        },
        {
            name: "Astronaut In The Ocean",
            url: "cloud/sound/iphone/calls/astronaut.ogg"
        },
        {
            name: translateText('player2', 'Ты горишь как огонь'),
            url: "cloud/sound/iphone/calls/gorish.ogg"
        },
        {
            name: "My Enemy",
            url: "cloud/sound/iphone/calls/dragons.ogg"
        },
        {
            name: "Suicidal Thoughts",
            url: "cloud/sound/iphone/calls/suicid.ogg"
        },
        {
            name: "Toxic",
            url: "cloud/sound/iphone/calls/toxic.ogg"
        },
        {
            name: "Money",
            url: "cloud/sound/iphone/calls/money.ogg"
        },
        {
            name: "Everyday",
            url: "cloud/sound/iphone/calls/everyday.ogg"
        },
        {
            name: translateText('player2', 'Я как Федерико Феллини'),
            url: "cloud/sound/iphone/calls/federico.ogg"
        },
        {
            name: translateText('player2', 'Солнце, монако'),
            url: "cloud/sound/iphone/calls/solnce.ogg"
        },
        {
            name: "All i need",
            url: "cloud/sound/iphone/calls/allineed.ogg"
        },
        {
            name: "Baby mama",
            url: "cloud/sound/iphone/calls/mama.ogg"
        },
        {
            name: "Rampampam",
            url: "cloud/sound/iphone/calls/rampampam.ogg"
        },
        {
            name: "Squid Game",
            url: "cloud/sound/iphone/calls/squidgame.ogg"
        },
        {
            name: translateText('player2', 'Куриный рэп'),
            url: "cloud/sound/iphone/calls/chick.ogg"
        },
        {
            name: translateText('player2', 'Ржач Лошади'),
            url: "cloud/sound/iphone/calls/loshad.ogg"
        },
        {
            name: translateText('player2', 'Большие башмаки'),
            url: "cloud/sound/iphone/calls/bashmak.ogg"
        },
        {
            name: translateText('player2', 'Мама звонит...'),
            url: "cloud/sound/iphone/calls/mamaz.ogg"
        },
        {
            name: translateText('player2', 'Что такое доброта?'),
            url: "cloud/sound/iphone/calls/dobrota.ogg"
        },
        {
            name: translateText('player2', 'Губка Боб'),
            url: "cloud/sound/iphone/calls/sponge.ogg"
        },
        {
            name: "Xanax",
            url: "cloud/sound/iphone/calls/xanax.ogg"
        },
        {
            name: "Ukraine",
            url: "cloud/sound/iphone/calls/ukraine.ogg"
        },
        {
            name: "Antihype",
            url: "cloud/sound/iphone/calls/antihype.ogg"
        },
        {
            name: "Deadinside",
            url: "cloud/sound/iphone/calls/dedinsid.ogg"
        },
        {
            name: translateText('player2', 'Куртец'),
            url: "cloud/sound/iphone/calls/kurtec.ogg"
        },
        {
            name: "Minecraft",
            url: "cloud/sound/iphone/calls/minecraft.ogg"
        },
        {
            name: "Fortnite",
            url: "cloud/sound/iphone/calls/fortnite.ogg"
        },
        {
            name: "Bones",
            url: "cloud/sound/iphone/calls/bones.ogg"
        },
        {
            name: "Lipsi Ha Slow",
            url: "cloud/sound/iphone/calls/lipsislow.ogg"
        },
        {
            name: translateText('player2', 'Губка Боб 2'),
            url: "cloud/sound/iphone/calls/sponge2.ogg"
        },
        {
            name: "Internal",
            url: "cloud/sound/iphone/calls/internal.ogg"
        },
        {
            name: translateText('player2', 'Реальные Пацаны'),
            url: "cloud/sound/iphone/calls/pacani.ogg"
        },
        {
            name: translateText('player2', 'Втюрилась'),
            url: "cloud/sound/iphone/calls/dora.ogg"
        },
        {
            name: translateText('player2', 'Барбисайз'),
            url: "cloud/sound/iphone/calls/barbiesize.ogg"
        },
        {
            name: "Back In Black",
            url: "cloud/sound/iphone/calls/acdc.ogg"
        },
        {
            name: translateText('player2', 'Стикер'),
            url: "cloud/sound/iphone/calls/stiker.ogg"
        },
        {
            name: "Can You Feel My Heart",
            url: "cloud/sound/iphone/calls/bmth.ogg"
        },
        {
            name: translateText('player2', 'Малиновый закат'),
            url: "cloud/sound/iphone/calls/lada.ogg"
        },
        {
            name: translateText('player2', 'Фотографирую закат'),
            url: "cloud/sound/iphone/calls/zakat.ogg"
        },
        {
            name: translateText('player2', 'Просто друг'),
            url: "cloud/sound/iphone/calls/drug.ogg"
        },
        {
            name: translateText('player2', 'Я не знаю'),
            url: "cloud/sound/iphone/calls/yaneznayu.ogg"
        },
        {
            name: translateText('player2', 'Батарейка'),
            url: "cloud/sound/iphone/calls/batarejka.ogg"
        },
        {
            name: translateText('player2', 'На Луне'),
            url: "cloud/sound/iphone/calls/nalune.ogg"
        },
        {
            name: "Run",
            url: "cloud/sound/iphone/calls/run.ogg"
        },
        {
            name: "Rock Queen",
            url: "cloud/sound/iphone/calls/queen.ogg"
        },
        {
            name: "Bitches Come And Go",
            url: "cloud/sound/iphone/calls/yunglean.ogg"
        },
        {
            name: "Aomine Daiki",
            url: "cloud/sound/iphone/calls/aomine.ogg"
        },
        {
            name: translateText('player2', 'Дурак и молния'),
            url: "cloud/sound/iphone/calls/kish.ogg"
        },
        {
            name: translateText('player2', 'Шипучка'),
            url: "cloud/sound/iphone/calls/shipu4ka.ogg"
        },
        {
            name: translateText('player2', 'Мятой'),
            url: "cloud/sound/iphone/calls/myatoy.ogg"
        },
        {
            name: translateText('player2', 'Ромашки'),
            url: "cloud/sound/iphone/calls/romashki.ogg"
        },
        {
            name: translateText('player2', 'Звездное Лето'),
            url: "cloud/sound/iphone/calls/leto.ogg"
        },
        {
            name: translateText('player2', 'Если я спал с тобой'),
            url: "cloud/sound/iphone/calls/spal.ogg"
        },
        {
            name: translateText('player2', 'Мальчик на девятке'),
            url: "cloud/sound/iphone/calls/malchik.ogg"
        },
        {
            name: translateText('player2', 'Три на три'),
            url: "cloud/sound/iphone/calls/3x3.ogg"
        },
        {
            name: translateText('player2', 'Занни ща во мне'),
            url: "cloud/sound/iphone/calls/milliontape.ogg"
        },
        {
            name: translateText('player2', 'Город под подошвой'),
            url: "cloud/sound/iphone/calls/gorodpod.ogg"
        },
        {
            name: translateText('player2', 'Звезда упала'),
            url: "cloud/sound/iphone/calls/zvezda.ogg"
        },
        {
            name: translateText('player2', 'Все хотят от меня шоу'),
            url: "cloud/sound/iphone/calls/show.ogg"
        },
        {
            name: translateText('player2', 'Знаю ты далеко'),
            url: "cloud/sound/iphone/calls/daleko.ogg"
        },
        {
            name: translateText('player2', 'Всё будет хорошо'),
            url: "cloud/sound/iphone/calls/horosho.ogg"
        },
        {
            name: translateText('player2', 'Чувства'),
            url: "cloud/sound/iphone/calls/chuvst.ogg"
        },
        {
            name: "Industry Baby",
            url: "cloud/sound/iphone/calls/industr.ogg"
        },
        {
            name: "Abcdefu",
            url: "cloud/sound/iphone/calls/abcdefu.ogg"
        },
        {
            name: translateText('player2', 'Я всё решу'),
            url: "cloud/sound/iphone/calls/reshu.ogg"
        },
        {
            name: translateText('player2', 'Аризона Бошки'),
            url: "cloud/sound/iphone/calls/arizona.ogg"
        },
        {
            name: "So Icy Nihao",
            url: "cloud/sound/iphone/calls/nihao.ogg"
        },
        {
            name: "Errbody Sleeping",
            url: "cloud/sound/iphone/calls/err.ogg"
        },
        {
            name: "99 Problems (Marimba)",
            url: "cloud/sound/iphone/calls/99mar.ogg"
        },
        {
            name: "99 Problems",
            url: "cloud/sound/iphone/calls/99pr.ogg"
        },
        {
            name: "Baby Walker",
            url: "cloud/sound/iphone/calls/babyw.ogg"
        },
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

    executeClientAsyncToGroup("settings.bellId").then((result) => {
        selectIndex = result;
        defaultIndex = selectIndex;
    });

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        executeClient ("sounds.stop", "phoneSound")
        if (defaultIndex !== selectIndex)
            executeClientToGroup("settings.bellId", selectIndex)
    });
</script>
<div class="newphone__settings_flex newphone__project_padding16" in:fade>
    <div class="box-flex" on:click={()=> onSelectedView(null)}>
        <div class="phoneicons-Vector-Stroke"></div>
        <div>{translateText('player2', 'Назад')}</div>
    </div>
    <div style="margin-left: 16px">{translateText('player2', 'Рингтоны')}</div>
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