<script>

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
            <div class="newphone__maps_title">Выберите один из режимов</div>
            <div class="newphone__maps_subtitle">В этом приложении вы можете заказать такси либо начать работать таксистом.</div>
            <div class="newphone__maps_selected" on:click={()=> onSelectView ("Client")}>
                <div class="newphone__maps_square">
                    <div class="phoneicons-npc newphone__maps_elementicon"></div>
                </div>
                <div class="box-column">
                    <div class="newphone__maps_headertitle">Клиент</div>
                    <div class="newphone__maps_headersubtitle">Вы хотите заказать такси</div>
                </div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
            <div class="newphone__maps_selected" on:click={()=> onSelectView ("Driver")}>
                <div class="newphone__maps_square">
                    <div class="phoneicons-cars newphone__maps_elementicon"></div>
                </div>
                <div class="box-column">
                    <div class="newphone__maps_headertitle">Водитель</div>
                    <div class="newphone__maps_headersubtitle">Вы хотите работать в такси</div>
                </div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
        </div>
        <HomeButton />
    </div>
</div>