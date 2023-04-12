<script>
    import { executeClient } from 'api/rage';
    import { translateText } from 'lang'
    export let wantedData;
</script>

<h1>{translateText('fractions', 'Сейчас в розыске')}</h1>
<div class="elements">
    <ol>
        {#each wantedData as value, index}
            <li>{value}</li>
        {/each}
    </ol>
</div>