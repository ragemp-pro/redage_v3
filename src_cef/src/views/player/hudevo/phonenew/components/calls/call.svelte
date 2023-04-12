<script>
    import { translateText } from 'lang'
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'
    import dial from './dial.svelte'
    import contacts from './contacts.svelte'
    import recent from './recent.svelte'
    import { selectNumber } from './../../stores'
    import { currentView } from './stores'

    let Views = {
        dial,
        contacts,
        recent
    }


    const updateView = (view) => {
        if ($currentView === "contacts")
            selectNumber.set(null);

        currentView.set(view);
    }
    import { fade } from 'svelte/transition'    

</script>
<div class="newphone__call" in:fade>
    <Header />

    <svelte:component this={Views[$currentView]} {updateView} />

    <div class="nephone__call_selectors">
        <div class="box-between w-1">
            <div class="newphone__call_selector" class:active={$currentView === "recent"} on:click={() => updateView ("recent")}>
                <div class="phoneicons-recent newphone__selector_icon"></div>
                <div>{translateText('player2', 'Недавние')}</div>
            </div>
            <div class="newphone__call_selector" class:active={$currentView === "contacts"} on:click={() => updateView ("contacts")}>
                <div class="phoneicons-contacts newphone__selector_icon"></div>
                <div>{translateText('player2', 'Контакты')}</div>
            </div>
            <div class="newphone__call_selector" class:active={$currentView === "dial"} on:click={() => updateView ("dial")}>
                <div class="phoneicons-buttons newphone__selector_icon"></div>
                <div>{translateText('player2', 'Клавиши')}</div>
            </div>
        </div>
        <HomeButton />
    </div>
</div>