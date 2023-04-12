<script>
    import  './assets/css/iconscircle.css';
    import  './assets/css/circle.sass';
    import { charFractionID, charOrganizationID } from 'store/chars'
    import { executeClient } from 'api/rage'
    export let popupData;


    $: if (popupData && typeof popupData === "string") 
        popupData = JSON.parse (popupData)
    

    const updateCategory = (json) => {
        popupData = JSON.parse (json)
    }
    window.events.addEvent("cef.circle.updateCategory", updateCategory);

    import { onDestroy } from 'svelte'

    onDestroy(() => {
        window.events.removeEvent("cef.circle.updateCategory", updateCategory);
    });


    const    
        prefix = "circle-";


    let drawname = ""
    const OnHovered = (name, isBack = false) => {
        drawname = name;
        executeClient ("client.circle.isBack", isBack);
    }

    const onCircleClick = (func, index = 0) => {
        executeClient ("client.circle.select", func, index);
    }
    

    const ontest = (index, max) => {
        switch (max) {
            case 1:
                return 1;
            case 2:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 5;
                }
                return;
            case 3:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 3;
                    case 2:
                        return 5;
                }
                return;
            case 4:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 3;
                    case 2:
                        return 5;
                    case 3:
                        return 7;
                }
                return;
            case 5:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 4;
                    case 3:
                        return 6;
                    case 4:
                        return 8;
                }
                return;
            case 6:
                switch (index) {
                    case 0:
                        return 1;
                    case 1:
                        return 2;
                    case 2:
                        return 4;
                    case 3:
                        return 5;
                    case 4:
                        return 6;
                    case 5:
                        return 8;
                }
                return;
        
        }
        return index + 1;
    }

    const defaultCircle__closeWidth = 280;
    const defaultCircle__closeHeight = 280;

    const initCircle = (node) => {
        node = node.getBoundingClientRect();
        if (node) {
            const percentWidth = (node.width * 100 / defaultCircle__closeWidth) / 100;
            const percentHeight = (node.height * 100 / defaultCircle__closeHeight) / 100;
            executeClient ("client.circle.initCircle", percentWidth, percentHeight);
        }
    }



    const handleKeyUp = (event) => {
        const { keyCode } = event;

        for(let i = 0; i < 8; i++) {
            if (49 + i == keyCode) {
                onCircleClick (popupData [i].func, i)
                return;
            }
        }
    }
    const handleMouseUp = (event) => {
        const { which } = event;

        if (which === 3)
            onCircleClick ("back");
    }
</script>

<svelte:window on:keyup={handleKeyUp} on:mouseup={handleMouseUp} />

<div class="circle">
    <div class="circle__close" use:initCircle on:mouseenter={() => OnHovered ('Назад', true)} on:mouseleave={() => OnHovered ('Назад')} on:click={() => onCircleClick ("back")}>
        {drawname}
    </div>
    <div class="center">
        {#each popupData as data, index}
        <li on:click={() => onCircleClick (data.func, index)} on:mouseenter={() => OnHovered (data.name)} on:mouseleave={() => OnHovered ("Назад")} class="contents child{ontest (index, popupData.length)}">
            <span class="icons-circle {prefix}" />
            <div>{data.name}</div>
            <div class="contents__index">{index +1}</div>
        </li>
    {/each}
    </div>
</div>