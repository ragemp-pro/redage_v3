<script>
    import { translateText } from 'lang'
    export let onSelected;
    export let onBack;
    import { fade } from 'svelte/transition'
</script>

<div in:fade class="box-100">
    <div class="newphone__maps_header m-top newphone__project_padding20">{translateText('player2', 'Подать объявление')}:</div>
    <div class="newphone__ads_list">
        <div class="newphone__project_categorie" on:click={()=> onSelected (false)}>
            <div>{translateText('player2', 'Обычное объявление')}</div>
            <div class="phoneicons-Button newphone__chevron-back"></div>
        </div>
        <div class="newphone__project_categorie" on:click={()=> onSelected (true)}>
            <div>{translateText('player2', 'VIP объявление')}</div>
            <div class="phoneicons-Button newphone__chevron-back"></div>
        </div>
    </div>
    <div class="red box-center mt-20" on:click={onBack}>{translateText('player2', 'Назад')}</div>
</div>