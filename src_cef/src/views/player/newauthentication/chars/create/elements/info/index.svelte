<script>
    import { gender, customization, updateParents, updateShapeMix, updateSkinTone } from 'store/customization';
    import parents from './parents.js';
    import ParentsButton from './parentsbutton.svelte';
    import InputBlock from './../input_item.svelte';

    let activeItem = 0;
    const handleKeyUp = (event) => {
        const { keyCode } = event;
        switch(keyCode) {
            case 38: // up
                if(--activeItem < 0)
                    activeItem = 1;
                break;
            case 40: // down
                if(++activeItem >= 2)
                    activeItem = 0;
                break;
        }
    }

    let customization_image_use = 1;
    $: if (activeItem === 0 || activeItem === 1) {
        customization_image_use = activeItem;
    }
</script>

<svelte:window on:keyup={handleKeyUp} />

<div class="auth__customization_elements">
    <div class="auth__customization_image">
        <div class="auth__customization_mother" style="z-index: {customization_image_use === 1 ? 2 : 1};background-image: url({document.cloud + `img/parents/${$customization.motherId}.png`})" />
        <div class="auth__customization_father" style="z-index: {customization_image_use === 0 ? 2 : 1};background-image: url({document.cloud + `img/parents/${$customization.fatherId}.png`})" />
    </div>
    <div class="auth__customization_element" class:active={activeItem === 0} on:click={() => activeItem = 0}>
        <div class="auth__customization_leftside">Отец</div>

        <ParentsButton
            gender={true}
            value={$customization.fatherId}
            active={activeItem === 0}
            onChange={newparent => updateParents($gender, $customization.motherId, newparent)} />
    </div>
    <div class="auth__customization_element" class:active={activeItem === 1} on:click={() => activeItem = 1}>
        <div class="auth__customization_leftside">Мать</div>

        <ParentsButton
            gender={false}
            value={$customization.motherId}
            active={activeItem === 1}
            onChange={newparent => updateParents($gender, newparent, $customization.fatherId)} />
    </div>
    <div class="auth__customization_element">
        <div class="auth__customization_leftside">Схожесть</div>
        <InputBlock
            id="shapemix"
            leftIcon="auth-mother"
            rightIcon="auth-father"
            step={0.1}
            min={0}
            max={1}
            value={$customization.shapeMix}
            callback={newvalue => updateShapeMix($gender, newvalue)} />
    </div>
    <div class="auth__customization_element">
        <div class="auth__customization_leftside">Тон кожи</div>
        <InputBlock
            id="skintone"
            leftIcon="auth-mother"
            rightIcon="auth-father"
            step={0.1}
            min={0}
            max={1}
            value={$customization.skinTone}
            callback={newvalue => updateSkinTone($gender, newvalue)} />
    </div>
</div>