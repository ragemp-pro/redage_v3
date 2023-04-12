<script>
    import './main.sass';


    let selectedSortKey = "";
    export let columns = [];
    export let elements = [];

    export let searchKey = "";
    let searchText = "";

    const sortByProperty = (a, b, property) => {
        if (a[property] > b[property]) return 1;
        if (a[property] < b[property]) return -1;
        return 0;
    }

    const selectSort = (column) => {
        if (column.sortable) {
            if (selectedSortKey === column.key) {
                elements = elements.reverse();
                return true;
            } else {
                selectedSortKey = column.key;
                elements = elements.sort((a, b) => sortByProperty(a, b, column.key));
                return selectSort(column);
            }
        }
    }

    const filter = (member) => {
        let memberName = member.name.toLowerCase();
        return memberName.includes(searchText.replace(" ", "_").toLowerCase());
    }

    const filterCheck = (el, text) => {
        text = text.toLowerCase().trim();
        if (searchKey !== "") {
            let success = false;
            if (!el[searchKey]) {
                Object.values(el).forEach((item) => {
                    if (!Array.isArray(item)) {
                        if (String (item).toLowerCase().trim().includes(text.replace(" ", "_")))
                            success = true;
                    } else {
                        item.forEach((i) => {
                                
                            if (String (i).toLowerCase().trim().includes(text.replace(" ", "_")))
                                success = true;
                        })
                    }
                })
                return success;
            } else {

                const element = el[searchKey].toLowerCase().trim();
                return element.includes(text.replace(" ", "_"));
            }
        }
        return true;
    }

</script>

<div class="box-sortlist">
    <div class="box-input">
    {#if searchKey !== ""}
        <input bind:value={searchText} placeholder="Поиск человека" type="text" maxLength="32" class="sort-input" />
    {/if}
    </div>
    <div class="box-header">
        {#each columns as column, index}
            <div class="desc" style="width: {column.width}%" on:click={() => { if (selectSort(column)) { column.reverse = !column.reverse; }}} class:active={selectedSortKey == column.key}>
                {column.name}
                {#if column.sortable === true}
                    <div class="triangle" class:up={!column.reverse} class:down={column.reverse}/>
                {/if}
            </div>
        {/each}
    </div>
    <div class="listScrollBar">
        {#each elements.filter(el => { return filterCheck(el, searchText)}) as element, index}
            <slot index={index} element={element}></slot>
        {/each}
    </div>
</div>