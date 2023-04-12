<script>
    import { translateText } from 'lang'
    import { categorieName } from './../data'
    export let onSelected;
    export let onBack;
    import { fade } from 'svelte/transition'

</script>

<div in:fade class="box-100">
    <div class="newphone__maps_header m-top newphone__project_padding20">{translateText('player2', 'Выберите категорию')}:</div>
    <div class="newphone__ads_list">
        {#each Object.values(categorieName) as item, index}
            <div class="newphone__project_categorie" on:click={()=> onSelected (index)}>
                <div>{item}</div>
                <div class="phoneicons-Button newphone__chevron-back"></div>
            </div>
        {/each}
    </div>
    <div class="red box-center mt-20" on:click={onBack}>{translateText('player2', 'Назад')}</div>
</div>