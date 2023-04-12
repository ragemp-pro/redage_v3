<script>
    import { translateText } from 'lang'
    import { executeClientToGroup, executeClientAsyncToGroup } from 'api/rage'
    executeClientToGroup('clothesLoad')


    let clothes = []
    const getClothes = (_clothes) => {
        clothes = JSON.parse(_clothes);
    }

    import { addListernEvent } from "api/functions";

    addListernEvent ("table.clothes", getClothes)

    import { setPopup, onUpdateRank } from "../../data";


    let selectForm = {}
    let newFormName = ""

    const updateRank = (rank) => {
        executeClientToGroup("clothesUpdate", selectForm.name, newFormName, rank, selectForm.gender);
    }

    const onEditFormCallback = (value) => {
        newFormName = value
        onUpdateRank("Список рангов", "fractionsicon-form", "Выберите ранг, с которого будет доступна форма.", null, null, updateRank);
    }

    const onEditForm = (item) => {
        selectForm = item
        setPopup ("Input", {
            headerTitle: "1",
            headerIcon: "fractionsicon-form",
            input: {
                title: "Название формы",
                placeholder: "Введите название формы",
                maxLength: 36,
                value: selectForm.name
            },
            button: translateText('popups', 'Подтвердить'),
            callback: onEditFormCallback
        })
    }
    import Logo from '../other/logo.svelte'
</script>
<div class="fractions__header">
    <div class="box-flex">
        <Logo />
        <div class="box-column">
            <div class="box-flex">
                <span class="fractionsicon-form"></span>
                <div class="fractions__header_title">{translateText('player1', 'Форма')}</div>
            </div>
            <div class="fractions__header_subtitle">{translateText('player1', 'Меню создания и детальной настройки формы')}</div>
        </div>
    </div>
</div>
<div class="fractions__content backgr h-fit">
    <div class="fractions__main_head box-between mb-0">
        <div class="box-flex">
            <span class="fractionsicon-form"></span>
            <div class="fractions__main_title">{translateText('player1', 'Управление комплектами одежды')}</div>
        </div>
        <!--<div class="fractions__main_button extra">
            {translateText('player1', 'Добавить форму')}
        </div>-->
    </div>
    <div class="fractions__main_scroll extrabig h-fit fractions__main_grid mt-40">
        {#each clothes as item, index}
           <div class="box-column">
                <div class="fractions__element_black newparams">
                    <div class="box-column">
                        <div class="fractions__black_title">{item.name} ({item.gender ? "М" : "Ж"})</div>
                        <div class="fractions__black_title gray mb-18">{item.rankName}</div>
                        <div class="fractions__black_subtitle silver hover box-flex mt-8" on:click={() => onEditForm(item)}>
                            <span class="fractionsicon-settings"></span>
                            {translateText('player1', 'Редактировать')}
                        </div>
                    </div>
                    <div class="fractions__black_img form"></div>
                </div>
           </div>
        {/each}
    </div>
</div>