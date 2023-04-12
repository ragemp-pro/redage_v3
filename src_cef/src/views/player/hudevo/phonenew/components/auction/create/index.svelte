<script>
    import { translateText } from 'lang'
    import { categorieName } from "@/views/player/hudevo/phonenew/components/auction/data";

    export let onSelectedView;

    import { executeClientToGroup } from "api/rage";
    const onSelect = (index) => {
        executeClientToGroup ("auction.selectCategory", index);
        executeClientToGroup ("auction.getItem", index);
        onSelectedView("CreateList", false);
    }
</script>
<div class="newphone__maps_header m-top newphone__project_padding20">{translateText('player2', 'Выставить лот')}:</div>
<div class="newphone__ads_list">
    {#each categorieName as name, index}
        <div class="newphone__project_categorie" on:click={()=> onSelect(index)}>
            <div>{name}</div>
            <div class="phoneicons-Button newphone__chevron-back green"></div>
        </div>
    {/each}
</div>
<div class="green box-center mt-5" on:click={() => onSelectedView("Main")}>{translateText('player2', 'Назад')}</div>