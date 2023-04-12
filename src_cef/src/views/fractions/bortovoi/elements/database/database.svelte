<script>
    import DataBase from './datalist.svelte'
    import Info from './info.svelte'

    const Views = {
        DataBase,
        Info
    }

    let selectView = "DataBase"

    const onChangePage = (page) => {
        selectView = page;
    }
</script>
<svelte:component this={Views[selectView]} {onChangePage}/>