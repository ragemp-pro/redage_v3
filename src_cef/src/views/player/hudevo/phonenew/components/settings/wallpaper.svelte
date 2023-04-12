<script>
    import { translateText } from 'lang'
    import { fade } from 'svelte/transition'
    import { executeClientAsyncToGroup, executeClientToGroup } from 'api/rage';

    export let onSelectedView;

    const wallpapers = [
        document.cloud + "img/iphone/wallpapers/1.png",
        document.cloud + "img/iphone/wallpapers/2.png",
        document.cloud + "img/iphone/wallpapers/3.png",
        document.cloud + "img/iphone/wallpapers/4.png",
        document.cloud + "img/iphone/wallpapers/5.png",
        document.cloud + "img/iphone/wallpapers/6.png",
        document.cloud + "img/iphone/wallpapers/7.png",
        document.cloud + "img/iphone/wallpapers/8.png",
        document.cloud + "img/iphone/wallpapers/9.png",
        document.cloud + "img/iphone/wallpapers/10.png",
        document.cloud + "img/iphone/wallpapers/11.png",
        document.cloud + "img/iphone/wallpapers/12.png",
        document.cloud + "img/iphone/wallpapers/13.png",
        document.cloud + "img/iphone/wallpapers/14.png",
        document.cloud + "img/iphone/wallpapers/15.png",
        document.cloud + "img/iphone/wallpapers/16.png",
        document.cloud + "img/iphone/wallpapers/17.png",
        document.cloud + "img/iphone/wallpapers/18.png",
        document.cloud + "img/iphone/wallpapers/19.png",
        document.cloud + "img/iphone/wallpapers/20.png",
        document.cloud + "img/iphone/wallpapers/21.png",
        document.cloud + "img/iphone/wallpapers/22.png",
        document.cloud + "img/iphone/wallpapers/23.png",
        document.cloud + "img/iphone/wallpapers/24.png",
        document.cloud + "img/iphone/wallpapers/25.png",
        document.cloud + "img/iphone/wallpapers/26.png",
        document.cloud + "img/iphone/wallpapers/27.png",
        document.cloud + "img/iphone/wallpapers/28.png",
        document.cloud + "img/iphone/wallpapers/29.png",
        document.cloud + "img/iphone/wallpapers/30.png",
        document.cloud + "img/iphone/wallpapers/31.png",
        document.cloud + "img/iphone/wallpapers/32.png",
        document.cloud + "img/iphone/wallpapers/33.png",
        document.cloud + "img/iphone/wallpapers/34.png",
        document.cloud + "img/iphone/wallpapers/35.png",
        document.cloud + "img/iphone/wallpapers/36.png",
        document.cloud + "img/iphone/wallpapers/37.png",
        document.cloud + "img/iphone/wallpapers/38.png",
        document.cloud + "img/iphone/wallpapers/39.png",
        document.cloud + "img/iphone/wallpapers/40.png",
        document.cloud + "img/iphone/wallpapers/41.png",
        document.cloud + "img/iphone/wallpapers/42.png",
        document.cloud + "img/iphone/wallpapers/43.png",
        document.cloud + "img/iphone/wallpapers/44.png",
        document.cloud + "img/iphone/wallpapers/45.png",
        document.cloud + "img/iphone/wallpapers/46.png",
        document.cloud + "img/iphone/wallpapers/47.png",
        document.cloud + "img/iphone/wallpapers/48.png",
        document.cloud + "img/iphone/wallpapers/49.png",
        document.cloud + "img/iphone/wallpapers/50.png",
        document.cloud + "img/iphone/wallpapers/51.png"
        
    ]

    let selectWallpaper = wallpapers[0]
    let defaultWallpaper = wallpapers[0]

    executeClientAsyncToGroup("settings.wallpaper").then((result) => {
        selectWallpaper = result;
        defaultWallpaper = selectWallpaper;
    });

    const onSelectWallpaper = (url) => {
        selectWallpaper = url
    }

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        if (defaultWallpaper !== selectWallpaper)
            executeClientToGroup("settings.wallpaper", selectWallpaper)
    });

</script>
<div class="newphone__settings_flex newphone__project_padding16" in:fade>
    <div class="box-flex" on:click={()=> onSelectedView(null)}>
        <div class="phoneicons-Vector-Stroke"></div>
        <div>{translateText('player2', 'Назад')}</div>
    </div>
    <div>{translateText('player2', 'Обои')}</div>
    <div class="box-flex"></div>
</div>
<div class="newphone__settings_imagesselect n-p">
    {#each wallpapers as url}
    <div class="newphone__settings_imageselector" style="background-image: url({url});" class:active={selectWallpaper === url} on:click={()=>onSelectWallpaper(url)}>
    </div>
    {/each}
</div>