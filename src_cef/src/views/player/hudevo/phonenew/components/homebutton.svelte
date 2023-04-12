<script>
    import { currentPage, selectNumber } from '../stores'

    const onMain = () => {
        currentPage.set("mainmenu")
        selectNumber.set(null)
    }
</script>
<div class="newphone__homebutton" on:click={onMain}>
    <div class="newphone__homebutton_button"></div>
</div>