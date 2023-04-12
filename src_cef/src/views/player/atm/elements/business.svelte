<script>
    import { executeClient } from 'api/rage'
    export let type;
    export let subdata;

    const emulPress = (index) => {
        executeClient ("atmCB", type, index);
    }

</script>

{#each subdata as value, index}
    {emulPress(index)}
{/each}