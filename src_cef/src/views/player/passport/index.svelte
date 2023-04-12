<script>
    import { executeClient } from 'api/rage'
    import { onDestroy } from 'svelte'

    import './css/main.css'
    export let viewData;

    let number = viewData [0],
        firstname = viewData [1],
        lastname = viewData [2],
        date = viewData [3],
        gender = viewData [4],
        member = viewData [5],
        work = viewData [6],
        timer = setTimeout(() => HideDocs(), 10000);
    
    window.passport = {
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
    <div class="passport">
        <p id="number">{number}</p>
        <p id="firstname">{firstname}</p>
        <p id="lastname">{lastname}</p>
        <p id="date">{date}</p>
        <p id="gender">{gender}</p>
        <p id="member">{member}</p>
        <p id="work">{work}</p>
        <div class="icon-exit exit" on:click={HideDocs}></div>
    </div>
</div>