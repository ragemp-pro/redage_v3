<script>
    import { executeClient } from 'api/rage'

    import './assets/css/petrol.css'

    let input = "";

    window.petrol = {
        reset: () => {
            input = ""
        }        
    }  

    const gov = () => {
        executeClient ('petrol.gov');
    }

    const full = () => {
        executeClient ('petrol.full');
    }

    const yes = () => {
        executeClient ('petrol', parseInt(input));
    }

    const no = () => {
        executeClient ('closePetrol');
    }

</script>

<div class="petrol-menu">
    <input maxLength="3" bind:value={input} />
    <div class="btns">
        <div class="btn gov" on:click={gov}></div>
        <div class="btn full" on:click={full}></div>
        <div class="btn yes" on:click={yes}></div>
        <div class="btn no" on:click={no}></div>
    </div>
</div>