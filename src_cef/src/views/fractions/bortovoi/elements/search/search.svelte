<script>
    import { translateText } from 'lang'
    import SearchPerson from './elements/searchperson.svelte'
    import SearchVehicle from './elements/searchvehicle.svelte'
    import SuPerson from './elements/superson.svelte'
    import SuVehicle from './elements/suvehicle.svelte'

    const Views = {
        SearchPerson,
        SearchVehicle,
        SuPerson,
        SuVehicle,
    }

    let selectView = "SearchPerson"
</script>
{#if selectView == "SearchPerson" || selectView == "SearchVehicle"}
    <div class="box-between">
        <div class="policecomputer__title">{translateText('fractions', 'Бортовой компьютер')}</div>
        {#if selectView == "SearchPerson"}
            <div class="policecomputer__button" on:click={() => selectView = "SuPerson"}>{translateText('fractions', 'Объявить в розыск')}</div>
        {/if}
        {#if selectView == "SearchVehicle"}
            <div class="policecomputer__button" on:click={() => selectView = "SuVehicle"}>{translateText('fractions', 'Объявить в розыск')}</div>
        {/if}
    </div>
    <div class="policecomputer__selectors">
        <div class="policecomputer__selector" class:active={selectView == "SearchPerson"} on:click={() => selectView = "SearchPerson"}>{translateText('fractions', 'Гражданин')}</div>
        <div class="policecomputer__selector" class:active={selectView == "SearchVehicle"} on:click={() => selectView = "SearchVehicle"}>{translateText('fractions', 'Транспорт')}</div>
    </div>
{/if}
{#if selectView == "SuPerson"}
    <div class="policecomputer__back" on:click={() => selectView = "SearchPerson"}>
        &#x2190; {translateText('fractions', 'Вернуться в поиск')}
    </div>
{/if}
<svelte:component this={Views[selectView]} />
