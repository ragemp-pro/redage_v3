<script>

    import Add from './add.svelte'
    import Filter from './filter.svelte'
    import CreateList from './createList.svelte'
    import {addListernEvent} from "api/functions";
    import {pageBack} from "@/views/player/hudevo/phonenew/stores";
    import {executeClientToGroup} from "api/rage";

    export let onSelectedView;

    let views = {
        Filter,
        CreateList,
        Add,
    }

    let selectedViews = "Filter";

    let type = 0;
    let isPremium = false;

    const onSelected = (index) => {
        if (selectedViews === "Filter") {
            type = index;
            selectedViews = "CreateList";
        } else if (selectedViews === "CreateList") {
            isPremium = index;
            selectedViews = "Add";
        }
    }

    const onBack = () => {
        if (selectedViews === "Filter") {
            onSelectedView ("List")
        } else if (selectedViews === "CreateList") {
            type = 0;
            selectedViews = "Filter";
        } else if (selectedViews === "Add") {
            isPremium = false;
            selectedViews = "CreateList";
        }
    }

    const onAdd = (text, link) => {
        executeClientToGroup ('addNews', text, link, type, isPremium);
    }

    addListernEvent ("adSuccess", () => {
        onSelectedView ("List")
    })
</script>

<svelte:component this={views[selectedViews]} {onSelected} {onBack} {onAdd} {isPremium} />