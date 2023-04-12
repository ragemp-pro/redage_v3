<script>
    import { translateText } from 'lang'
    import HomeButton from '../homebutton.svelte'

    import { fade } from 'svelte/transition'

    export let setOtherElement;
    export let onSelectView;
    export let closeMenu;

</script>

<div class="newphone__maps_categories" in:fade use:setOtherElement>
    <div class="newphone__maps_subcategories">
        <div class="box-between newphone__project_padding20">
            <div class="newphone__maps_header"></div>
            <div class="phoneicons-add1" on:click={closeMenu}></div>
        </div>
        <div class="newphone__maps_list">
            <div class="newphone__maps_title">{translateText('player2', 'Выберите один из режимов')}</div>
            <div class="newphone__maps_subtitle">{translateText('player2', 'В этом приложении вы можете вызвать механика либо начать работать механиком.')}</div>
            <div class="newphone__maps_selected" on:click={()=> onSelectView ("Client")}>
                <div class="newphone__maps_square">
                    <div class="phoneicons-npc newphone__maps_elementicon"></div>
                </div>
                <div class="box-column">
                    <div class="newphone__maps_headertitle">{translateText('player2', 'Клиент')}</div>
                    <div class="newphone__maps_headersubtitle">{translateText('player2', 'Вы хотите вызвать механика')}</div>
                </div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
            <div class="newphone__maps_selected" on:click={()=> onSelectView ("Driver")}>
                <div class="newphone__maps_square">
                    <div class="phoneicons-cars newphone__maps_elementicon"></div>
                </div>
                <div class="box-column">
                    <div class="newphone__maps_headertitle">{translateText('player2', 'Механик')}</div>
                    <div class="newphone__maps_headersubtitle">{translateText('player2', 'Вы хотите работать механиком')}</div>
                </div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
        </div>
        <HomeButton />
    </div>
</div>