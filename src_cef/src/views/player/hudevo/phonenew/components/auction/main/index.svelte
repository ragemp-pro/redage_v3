<script>
    import { translateText } from 'lang'
    import { categorieName } from "@/views/player/hudevo/phonenew/components/auction/data";

    export let onSelectedView;

    import MyLots from './myLots.svelte'

    import { executeClientToGroup } from "api/rage";
    const onSelect = (index) => {
        executeClientToGroup ("auction.selectCategory", index);
        onSelectedView("List");
    }
</script>

<div class="newphone__auction_content">

    <MyLots {onSelectedView} />

    <div class="newphone__maps_header m-top newphone__project_padding20 w-1 mb-10">{translateText('player2', 'Категории')}:</div>
    {#each categorieName as name, index}
        <div class="newphone__project_categorie newphone__margin_left16" on:click={()=> onSelect(index)}>
            <div>{name}</div>
            <div class="phoneicons-Button newphone__chevron-back green"></div>
        </div>
    {/each}
    <div class="newphone__project_button newphone__margin_left16 auction mb-0" on:click={()=> onSelectedView ("Create")}>{translateText('player2', 'Выставить лот')}</div>
</div>