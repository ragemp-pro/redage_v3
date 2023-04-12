<script>
    import { gender, customization, updateClothes} from 'store/customization';
    import ClothButton from './clothbutton.svelte';
    import clothes from './clothes.js';
    import { executeClient } from 'api/rage'

    const componentsData = [
        {key: "head", cam: "hat", title: "Головной убор"},
        {key: "tops", cam: "top", title: "Верхняя одежда"},
        {key: "legs", cam: "legs", title: "Нижняя одежда"},
        {key: "shoes", cam: "shoes", title: "Обувь"}
    ]

    let activeItem = 0;

    $: if (activeItem) {
        executeClient ("client.characters.customization.updateCam", componentsData[activeItem].cam);
    }

    const OnChangeCloth = (change) => {
        const key = componentsData[activeItem].key;
        let id = $customization[key];
        id += change;

        if (id > clothes[$gender][key].length - 1) id = 0;
        else if (id < 0) id = clothes[$gender][key].length - 1;

        updateClothes($gender, key, id)
    }
    
    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch(keyCode) {
            case 38: // up
                if(--activeItem < 0)
                    activeItem = componentsData.length - 1;
                break;
            case 40: // down
                if(++activeItem >= componentsData.length)
                    activeItem = 0;
                break;
        }
    }
</script>

<svelte:window on:keyup={handleKeyUp} />


<div class="auth__customization_elements">
    {#each componentsData as item, index}
    <div class="auth__customization_element" class:active={activeItem === index} on:click={() => activeItem = index}>
        <div class="auth__customization_leftside">{item.title}</div>
        <ClothButton
            id={$customization[item.key]}
            on:click={() => activeItem = index}
            key={item.key}
            active={activeItem === index}
            onChange={OnChangeCloth} />
    </div>
    {/each}
</div>