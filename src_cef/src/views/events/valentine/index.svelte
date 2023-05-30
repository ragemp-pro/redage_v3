<script>
    import { translateText } from 'lang'
    import './main.sass';
    import './fonts/style.css';
    import KeyAnimation from '@/components/keyAnimation/index.svelte';
    import { executeClient } from 'api/rage'
    export let viewData;

    import LoveNote from './loveNote.svelte'
    import Note from './note.svelte'

    if (viewData) {
        try {
            viewData = JSON.parse (viewData);
        } catch (e)  {
            viewData = false;
        }
    }

    const handleArrowKeys = (events) => {
        if (viewData)
            return;
        const { keyCode } = events;
        if (keyCode === 27) {
            OnClose ();
        }
    }
    const OnClose = () => {
        executeClient ("client.note.close")
    }
</script>

<svelte:window on:keyup={handleArrowKeys} />

<div id="valentine">
    {#if !viewData || (viewData && viewData.Type == undefined)}
        <div class="valentine__main_img"></div>
        <div class="valentine__text">
            {translateText('events', 'Привет, милый человек! Сейчас на сервере проходит серия мероприятий, посвященных Дню Святого Валентина: в магазинах 24/7 доступны к покупке валентинки и записки, в которых можно что-то написать. Так же советуем ознакомиться с небольшой квестовой линией, где тебе предстоит спасти жизнь человеку! Да здравствует любовь!')}
        </div>
        <div class="box-KeyAnimation" on:click={OnClose}>
            <div>{translateText('events', 'Закрыть')}</div>
            <KeyAnimation keyCode={27}>{translateText('events', 'Закрыть')}</KeyAnimation>
        </div>
    {:else if viewData && viewData.Type}
        <LoveNote {viewData} />
    {:else}
        <Note {viewData} />
    {/if}
</div>