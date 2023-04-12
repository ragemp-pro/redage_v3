<script>
    import { fade } from 'svelte/transition';
    import { translateText } from 'lang'
    let
        isOpened = false,        
        title = "",
        name = 0,
        image = 0,
        desc = "";

    let timerId = 0;

    window.updateWin = (_title, _name, _image, _desc) => {
        title = _title;
        name = _name;
        image = _image;
        desc = _desc;
        isOpened = true;

        if (timerId != 0) clearTimeout (timerId);

        timerId = setTimeout(() => {
            isOpened = false;
            timerId = 0;
        }, 1000 * 5)
    }
</script>
{#if isOpened}
    <div class="hudevo__winner" transition:fade={{ duration: 500 }}>
        <div class="hudevo__notification_subtitle">
            {desc}
        </div>
        <div class="hudevo__notification_title fs-14 large">
            {title}
        </div>
        <div class="hudevo__winner_image" style={`background-image: url(${document.cloud + `img/roulette/${image}.png`}`}></div>
    </div>
{/if}