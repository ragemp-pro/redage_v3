<script>
    import { translateText } from 'lang'
    import { onInputFocus, onInputBlur } from "@/views/player/menu/elements/fractions/data.js";

    export let departments;
    export let onSelectDepartment;
    export let onDepartmentDelete;

    let searchText = ""
    const filterCheck = (elText, text) => {
        if (!text || !text.length)
            return true;

        text = text.toUpperCase();

        if (elText.toString().toUpperCase().includes(text))
            return true;

        return false;
    }


    import { setPopup } from "../../data";
    import { executeClientToGroup } from "api/rage";
    import { format } from 'api/formatter'

    const onCreateCallback = (name, tag) => {
        let check = format("rank", name);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        check = format("tag", tag);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClientToGroup('createDepartment', name, tag)
    }

    const onCreate = () => {
        setPopup ("Input", {
            headerTitle: "Создать отряд",
            headerIcon: "fractionsicon-squads",
            inputs: [
                {
                    title: "Название отряда",
                    placeholder: "Введите новое название",
                    maxLength: 36,
                },
                {
                    title: "Тег отряда",
                    placeholder: "Введите тег отряда",
                    maxLength: 5
                }
            ],
            button: translateText('popups', 'Подтвердить'),
            callback: onCreateCallback
        })
    }

    import { executeClientAsyncToGroup } from "api/rage";
    let settings = {};
    const getSettings = () => {
        executeClientAsyncToGroup("getSettings").then((result) => {
            if (result && typeof result === "string")
                settings = JSON.parse(result);
        });
    }
    getSettings();
</script>
<div class="fractions__content backgr extrabig">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-squads"></span>
            <div class="fractions__main_title">{translateText('player1', 'Список отрядов')}</div>
        </div>
        <div class="fractions__input large">
            <div class="fractionsicon-loop"></div>
            <input on:focus={onInputFocus} on:blur={onInputBlur} bind:value={searchText} type="text" placeholder={translateText('player1', 'Поиск..')}>
        </div>
        {#if settings.createDepartment}
        <div class="fractions__main_button extra" on:click={onCreate}>
            <span class="fractionsicon-plus mr-5"></span>
            {translateText('player1', 'Создать отряд')}
        </div>
        {/if}
    </div>
    <div class="box-between pr-22 pl-8 pr-226">
        <div class="fractions_stats_title fs-14 name mr-16">{translateText('player1', 'Название')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Начальник')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Тег отряда')}:</div>
        <div class="fractions_stats_title fs-14 name">{translateText('player1', 'Участники')}:</div>
    </div>
    <div class="fractions__main_scroll big extrabig">
        {#each departments.filter(el => filterCheck(el.name, searchText)) as item}
            <div class="fractions__scroll_element hover p-20">
                <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.name}</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.chiefName}</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">#{item.tag}</div>
                <div class="fractions_stats_title fs-14 name whitecolor m-0">{item.playerCount} чел.</div>
                <div class="box-flex hidden">
                    <div class="fractions__main_button black" on:click={() => onSelectDepartment (item)}>{translateText('player1', 'Информация')}</div>
                    {#if settings.deleteDepartment}
                    <div class="fractionsicon-fired"  on:click={() => onDepartmentDelete (item)}></div>
                    {/if}
                </div>
            </div>
        {/each}
    </div>    
</div>