<script>
    import { translateText } from 'lang'
    import './css/main.css'
    import { executeClient } from 'api/rage'
    export let viewData;
    let
        name = "",
        weapons = [],
        items = [];    
	
    $: {
        if (viewData) {
            if (name !== viewData.Name)
                name = viewData.Name;

            if (weapons !== viewData.Weapons)
                weapons = viewData.Weapons;

            if (items !== viewData.Items)
                items = viewData.Items;
        }
    }

    const onBtn = (id) => {
        executeClient ('bsearch', id);
    }
</script>

<div class="bsearch">
    <div on:click={() => onBtn(0)} class="icon-exit btn-close"></div>
    <div class="list">
        <p>{translateText('fractions', 'Имя и фамилия')}: {name}</p>
        <p>{translateText('fractions', 'Оружие')}:</p>
        <ul>
            {#each weapons as value, index}
                <li>{value}</li>
            {/each}
        </ul>
        <p>{translateText('fractions', 'Предметы инвентаря')}:</p>
        <ul>
            {#each items as value, index}
                <li>{value}</li>
            {/each}
        </ul>
    </div>
    <div class="btns">
        <div on:click={() => onBtn(1)} class="btn">{translateText('fractions', 'Лицензии')}</div>
        <div on:click={() => onBtn(2)} class="btn">{translateText('fractions', 'Паспорт')}</div>
    </div>
</div>