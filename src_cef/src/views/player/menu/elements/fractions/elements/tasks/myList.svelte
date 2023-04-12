<script>
    export let onSetLoad;
    import { executeClientToGroup } from 'api/rage'
    executeClientToGroup('tasksMyLoad')

    import Tasks from './tasks.svelte'
</script>

<Tasks {onSetLoad} />