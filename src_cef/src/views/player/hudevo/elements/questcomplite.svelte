<script>
    import { fade } from 'svelte/transition';

    let
        isOpened = false,        
        title = "",
        desc = "",
        header = "",
        image = "";

    let timerId = 0;

    window.missionComplite = (_title, _desc, _header, _image) => {
        title = _title;
        desc = _desc;
        header = _header;
        image = _image;

        isOpened = true;

        if (timerId != 0)
            clearTimeout (timerId);

        timerId = setTimeout(() => {
            isOpened = false;
            timerId = 0;
        }, 1000 * 5)
    }
</script>
{#if isOpened}
    <div class="hudevo__winner noback" transition:fade={{ duration: 500 }}>
        <div class="hudevo__notification_subtitle">
            {desc}
        </div>
        <div class="hudevo__notification_title fs-14 large">
            {title}
        </div>
    </div>
{/if}