<script>
    import { executeClient } from 'api/rage'
    import { onDestroy } from 'svelte'

    import './css/main.css'
    export let viewData;

    let firstname = viewData[0],
        lastname = viewData[1],
        date = viewData[2],
        gender = viewData[3],
        lics = viewData[4],
        timer = setTimeout(() => HideDocs(), 10000);

    window.license = {
        hide: () => HideDocs()
    }

    const HideDocs = () => {
        if (timer !== null) {
            clearTimeout(timer);
            timer = null;
            executeClient ('dochide');
            window.router.setHud();
        }
    }

    onDestroy(() => {
        if (timer !== null) clearTimeout(timer);
    });

</script>

<div class="docs">
    <div class="license">
        <div class="icon-exit exit" on:click={HideDocs}></div>
        <p id="firstname">{firstname}</p>
        <p id="lastname">{lastname}</p>
        <p id="date">{date}</p>
        <p id="gender">{gender}</p>
        <p id="lics">{lics}</p>
    </div>
</div>