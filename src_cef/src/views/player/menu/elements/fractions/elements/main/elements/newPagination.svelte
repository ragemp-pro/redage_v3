<script>

    import { executeClientToGroup } from "api/rage";
    export let count;
    export let selectPage;

    const maxPage = 5;


    $: pageCount = Array.from(
        Array(count).keys()
    )

    const slice = (arr, page) => {
        if (page < maxPage) {
            return arr.slice(0, maxPage)
        } else if (page > arr.length - (maxPage - 1)) {
            return arr.slice(arr.length - maxPage, arr.length)
        }
        return arr.slice(page - 2, page + 1)
    }

    $: buttons = slice(pageCount, selectPage)

    let pageType = "up"
    const setPage = (number) => {
        if (number <= 0)
            return;
        if (number > count)
            return;
        pageType = selectPage <= number ? "up" : "down";

        if (number === 1)
            pageType = "up";
        else if (number === count)
            pageType = "down";

        executeClientToGroup("getBoard", number);
    }
</script>

<div class="box-flex">
    <div class="fractions__button_small" class:disabled={selectPage === 1} on:click={() => setPage(selectPage - 1)}>
        &lt;
    </div>
    {#if pageCount.length > 0}
        <div class="fractions__button_small" class:active={selectPage === 1} on:click={() => setPage(1)}>
            1
        </div>
    {/if}
    {#if /*pageType === "down" && */pageCount.length > (maxPage + 1) && selectPage >= maxPage}
        <div class="fractions__button_small">
            ..
        </div>
    {/if}

    {#each buttons as n}
        {#if n > 0 && n < pageCount.length - 1}
            <div class="fractions__button_small" class:active={selectPage === n + 1} on:click={() => setPage(n + 1)}>{n + 1}</div>
        {/if}
    {/each}

    {#if /*pageType === "up" && */pageCount.length > (maxPage + 1) && selectPage <= pageCount.length - 3}
        <div class="fractions__button_small">
            ..
        </div>
    {/if}
    
    {#if pageCount.length > 1}
        <div class="fractions__button_small" class:active={selectPage === pageCount.length} on:click={() => setPage(pageCount.length)}>
            {pageCount.length}
        </div>
    {/if}
    <div class="fractions__button_small" class:disabled={selectPage === pageCount.length} on:click={() => setPage(selectPage + 1)}>
        &gt;
    </div>
</div>