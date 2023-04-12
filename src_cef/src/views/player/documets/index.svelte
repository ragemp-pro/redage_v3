<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { onMount } from 'svelte'
    import { onDestroy } from 'svelte'
    import './css/main.sass';
    import './fontlicenses/style.css';
    export let viewData;

/*     let viewData = {
            page: "lsnews",
            data: {
                gender: "Мужской",
                name: "Vitaliy",
                surname: "Zdobich",
                dateReg: "12.05.19",
                cardNO: "2281337",
                lic: [
                    true,
                    false,
                    false,
                    false,
                    true,
                    true,
                    false,
                    true,
                    true
                ]
            }
        }
 */
    let closeTimer = null;

    import army from './army/index.svelte';
    import ems from './ems/index.svelte';
    import fib from './fib/index.svelte';
    import fibbadge from './fibbadge/index.svelte';
    import lspdbadge from './lspdbadge/index.svelte';
    import goverment from './goverment/index.svelte';
    import lsnews from './lsnews/index.svelte';
    import lspd from './lspd/index.svelte';
    import msc from './msc/index.svelte';
    import passport from './passport/index.svelte';
    
    const Views = {
        army,
        ems,
        fib,
        goverment,
        lsnews,
        lspd,
        msc,
        passport,
        fibbadge,
        lspdbadge,
    }

    const HideDocs = () => {
        if (closeTimer !== null) {
            clearTimeout(closeTimer);
            closeTimer = null;
            executeClient ('dochide');
        }
    }

    onMount(() => {
		closeTimer = setTimeout(HideDocs, 10000);
    });

    onDestroy(() => {
        if (closeTimer !== null) clearTimeout(closeTimer);
    });
    function handleKeyUp(){
        const { keyCode } = event;
        switch (keyCode) {
            case 27: // esc
            HideDocs();
                break;
        }
    }
</script>
<svelte:window on:keyup={handleKeyUp} />
<div class="container_cards">
    <div class="box-column">
        <div class="newproject__buttonblock" on:click={()=> HideDocs()}>
            <div class="newproject__button">ESC</div>
            <div>{translateText('player', 'Закрыть')}</div>
        </div>
        <svelte:component this={Views[viewData.page]} {...viewData.data} />
    </div>
</div>