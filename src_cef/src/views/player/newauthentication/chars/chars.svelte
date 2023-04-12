<script>
    import { selectType, settings } from './store.js';
    import List from './list/index.svelte';

    import Create from './create/index.svelte';
    import Spawn from './spawn/index.svelte';
    import Buy from './buy/index.svelte';
    import PopupSelectPers from '../select/index.svelte';

    const Views = {
        [settings.create]: Create,
        [settings.char]: Spawn,
        [settings.buy]: Buy,
    }
</script>

<!-- <PopupSelectPers /> -->
<List />

<svelte:component this={Views[$selectType]} />