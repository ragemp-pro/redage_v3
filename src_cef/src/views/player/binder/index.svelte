<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import Keyboard from './keyboard.svelte'
    import { bindList } from './state.js'
    import './css/main.css'

    const listPage = [
        {name: translateText('player', 'Основные'), icon: 'icon-keyboard', type: "all"},
        {name: translateText('player', 'Транспорт'), icon: 'icon-keyboard', type: "vehicle"},
        {name: translateText('player', 'Быстрый доступ'), icon: 'icon-keyboard', type: "fast"},
        {name: translateText('player', 'Фракционные'), icon: 'icon-keyboard', type: "fraction"},
        {name: translateText('player', 'Разное'), icon: 'icon-keyboard', type: "other"},
        {name: translateText('player', 'Для администрации'), icon: 'icon-keyboard', type: "admin"}
    ];

    let listId = "all";
    
    let list = [];
    let indexId = -1;

    window.binder = {
        setData: (value) => {
            list = JSON.parse(value);
        },
        setBindData: (value) => {
            bindList.set (JSON.parse(value));
        },
        updateData: (index, name) => {
            for (let i = 0; i < list.length; i++) {
                if (list[i].index === index) {
                    list[i].name = name;
                    break;
                }
            }
        },
        index: () => {
            indexId = -1;
        }
    }

    const onClickPage = (index) => {
        listId = index;
        executeClient ("client:binder", "get", index);
    }

    const onClickIndex = (index) => {
        indexId = index;
        executeClient ("client:binder", "update", index);
    }

    const setTitle = () => {
        let title = "";
        listPage.forEach((value) => {
            if (value.type === listId) {
                title = value.name;
            }
        });
        return title;
    }
</script>
<div class="rd-body-inventory-donate">
    <div class="universal_menu module_binder">
        <div class="wrap" >
            <ul class="d-md">
                <li class="disabled">
                    <div class="space-between">
                        {translateText('player', 'Биндер')}
                        <button class="binder-button" on:click={() => executeClient ("client:binder", "refresh", listId)}>{translateText('player', 'Сбросить')}</button>
                    </div>
                </li>
                {#each listPage as value, index}
                    <li class:active={value.type === listId} on:click={() => onClickPage(value.type)}>
                        <span class={value.icon}></span>
                        {value.name}
                    </li>
                {/each}
            </ul>

            <div class="r">
                <button class="binder-button-close" on:click={() => executeClient ("client:binder", "close")}>X</button>
                <div class="md sett">
                    <div class="title">{setTitle ()}</div>
                    <ul class="settings">
                        {#each list as value, _}
                        <li class:active={value.index === indexId} on:click={() => onClickIndex (value.index)}>
                            <div class="wrapp">
                                <div>{value.title}</div>
                                <div class="st">
                                    <span>{value.name}</span>
                                </div>
                            </div>
                        </li>
                        {/each}
                    </ul>
                </div> 
            </div> 
        </div>
        <Keyboard />
    </div>
</div>