<script>
    export let onSetLoad;
    import { executeClientToGroup } from 'api/rage'
    executeClientToGroup('tasksLoad')

    import Tasks from './tasks.svelte'
</script>

<Tasks {onSetLoad} />