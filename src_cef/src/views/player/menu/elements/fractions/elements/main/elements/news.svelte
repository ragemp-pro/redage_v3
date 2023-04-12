<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";
    import { charUUID } from 'store/chars';
    import { TimeFormat } from 'api/moment'

    export let settings;
    let board = { };
    let selectPage = 1;

    const getBoard = () => {
        executeClientAsyncToGroup("getBoard").then((result) => {
            if (result && typeof result === "string") {
                board = JSON.parse(result);

                selectPage = board.page;
            }
        });
    }
    getBoard();

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.board", getBoard)

    import Pagination from './newPagination.svelte'
    import { setPopup } from "../../../data";

    const onAddBoard = (title, text) => {
        if (!title.length || title.length >= 20)
            return;
        else if (!text.length || text.length >= 100)
            return;

        executeClientToGroup("addBoard", title, text);
    }

    const addBoard = () => {
        setPopup ("Input", {
            headerTitle: "Написать новость",
            headerIcon: "fractionsicon-news",
            inputs: [
                {
                    title: "Введите название новости",
                    placeholder: "Введите название новости",
                    maxLength: 20
                },
                {
                    title: "Введите текст новости",
                    placeholder: "Введите текст новости",
                    maxLength: 100,
                    textarea: true
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onAddBoard
        })
    }

    const onUpdateBoard = (title, text) => {
        if (!title.length || title.length >= 20)
            return;
        else if (!text.length || text.length >= 100)
            return;

        executeClientToGroup("updateBoard", board.page, title, text);
    }

    const updateBoard = () => {
        setPopup ("Input", {
            headerTitle: "Редактировать новость",
            headerIcon: "fractionsicon-news",
            inputs: [
                {
                    title: "Новое название новости",
                    placeholder: "Введите новое название",
                    maxLength: 20,
                    value: board.title
                },
                {
                    title: "Новый текст новости",
                    placeholder: "Введите новый текст",
                    maxLength: 100,
                    textarea: true,
                    value: board.text
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onUpdateBoard
        })
    }

    const deleteBoard = () => {
        executeClientToGroup("deleteBoard", board.page);

    }
</script>
<div class="fractions__main_box w-520 h-190">
    {#if board.count > 0}
        <div class="fractions__main_head">
            <span class="fractionsicon-news"></span>
            <div class="fractions__main_title">{translateText('player1', 'Важные новости')}</div>
        </div>
        <div class="box-between">
            <div class="fractions__stats_subtitle fit mr-12">{board.title}</div>
            <div class="fractions__stats_subtitle gray fit">{TimeFormat (board.time, "HH:mm DD.MM.YY")}</div>
        </div>
        <div class="fractions__news_text">{board.text}</div>
        <div class="box-flex">
            <div class="fractions_stats_title large flex">
                <div class="fractions__circle_mini">
                    <div class="fractions__circle_smallperson"></div>
                </div> {translateText('player1', 'Автор')}: <span class="white">{board.name}</span> {board.rankName}
            </div>
        </div>
    {:else}
        <div class="fractions__stats_subtitle box-center gray wh-100">
            {translateText('player1', 'Новостей пока нет.')} <br> {translateText('player1', 'Напишите первую новость!')}
        </div>
    {/if}
</div>
<div class="box-column">

    <Pagination count={board.count} {selectPage} />

    <div class="box-flex mt-8">
        {#if board.uuid === $charUUID || (typeof board.uuid !== "undefined" && settings.editAllTabletWall)}
            <div class="fractions__main_button news" on:click={deleteBoard}>{translateText('player1', 'Удалить')}</div>
        {/if}
        {#if board.uuid === $charUUID || (typeof board.uuid !== "undefined" && settings.editAllTabletWall)}
            <div class="fractions__main_button news" on:click={updateBoard}>{translateText('player1', 'Редактировать')}</div>
        {/if}
        {#if settings.tableWall}
            <div class="fractions__main_button news" on:click={addBoard}>{translateText('player1', 'Написать')}</div>
        {/if}
    </div>
</div>

<!---
<button
        class="text"
        class:disabled={setPage === 1} on:click={() => setPage($pageNumber - 1)}
>
    {@html $options.labels.previous}
</button>
<button class:active={$pageNumber === 1} on:click={() => setPage(1)}>
    1
</button>
{#if pageCount.length > 6 && $pageNumber >= 5}
    <button class="ellipse">...</button>
{/if}

{#each buttons as n}
    {#if n > 0 && n < pageCount.length - 1}
        <button
                class:active={$pageNumber === n + 1}
                on:click={() => setPage(n + 1)}
        >
            {n + 1}
        </button>
    {/if}
{/each}

{#if pageCount.length > 6 && $pageNumber <= pageCount.length - 3}
    <button class="ellipse">...</button>
{/if}

{#if pageCount.length > 1}
    <button
            class:active={$pageNumber === pageCount.length}
            on:click={() => setPage(pageCount.length)}
    >
        {pageCount.length}
    </button>
{/if}

<button
        class="text"
        class:disabled={$pageNumber === pageCount.length} on:click={() => setPage($pageNumber + 1)}
>
    {@html $options.labels.next}
</button>-->