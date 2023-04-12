<script>
    import { selectNumber, pageBack } from './../../stores'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'

    import List from './list.svelte'
    import Message from './message.svelte'

    let selectedNumber = $selectNumber;

    let isBack = selectedNumber > 0;

    const onSelectNumber = (number) => {
        if (number === -1 && isBack) {
            pageBack ();
            //selectedNumber = null;
            //selectNumber.set(null);
        } else if (number === -1) {
            selectedNumber = null;
            selectNumber.set(null);
        } else {
            selectedNumber = number;
            selectNumber.set(number);
        }
    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__call" in:fade>
    <Header />
    <div class="newphone__messages">
        {#if selectedNumber === null}
            <List {onSelectNumber} />
        {:else}
            <Message {onSelectNumber} {selectedNumber} />
        {/if}
    </div>
    <HomeButton />
</div>