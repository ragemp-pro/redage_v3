<script>
    import { translateText } from 'lang'
    import './main.sass';
    import './fonts/style.css';
    import Animations from 'json/animations.js'
    import AnimElement from './element.svelte'
    import { storeAnimFavorites, storeAnimBind } from 'store/animation'
    import { spring } from 'svelte/motion';
    import { executeClient } from 'api/rage'
    import KeyAnimation from '@/components/keyAnimation/index.svelte';
    export let viewData;
    import keys from 'store/keys'
    import keysName from 'json/keys.js'
    
    let searchText = "";
    let animMenuList = [
        {
            id: 0,
            title: translateText('player', 'Избранное'),
            count: 0
        },
        {
            id: 1,
            title: translateText('player', 'Сесть/Лечь'),
            count: 0
        },
        {
            id: 2,
            title: translateText('player', 'Социальные'),
            count: 0
        },
        {
            id: 3,
            title: translateText('player', 'Позы'),
            count: 0
        },
        {
            id: 4,
            title: translateText('player', 'Неприличное'),
            count: 0
        },
        {
            id: 5,
            title: translateText('player', 'Физ. упражнения'),
            count: 0
        },
        {
            id: 6,
            title: translateText('player', 'Танцы'),
            count: 0
        },
        {
            id: 7,
            title: translateText('player', 'Прочее'),
            count: 0
        }
    ];
    let selectMenu = animMenuList [0];

    Object.values(Animations).forEach(animation => {
        animMenuList.forEach((item, index) => {
            if (animation[0] === item.title) {
                animMenuList[index].count++;
            }
        });
    });
    
    let favoritesAnim = [];
    storeAnimFavorites.subscribe((value) => {
        favoritesAnim = value;
        animMenuList[0].count = favoritesAnim.length;
    });

    const onSelectMenu = (index) => {
        selectMenu = animMenuList [index];
    }
    let enterAnim = "";

    function handleSlotMouseEnter (index) {
        if (DragonDropData != "")
            return;
        enterAnim = `${selectMenu.id}_${index}`;
    }
	
	// Когда выходим из зоны ячейки
	function handleSlotMouseLeave() {
        enterAnim = "";
    }

    let dubleClickData = ''
    let dubleClickTime = 0;
    const onPlayAnimation = (item) => {
        if (dubleClickData === item && dubleClickTime > new Date().getTime()) {
            executeClient ("client.animation.play", item);
            //window.events.callEvent("hud.enter", 'SPACE', 'Нажмите чтобы отменить анимацию');
        } else {
            dubleClickTime = new Date().getTime() + 1000;
            dubleClickData = item
        }
    }

    let DragonDropData = "";
    let offsetInElementX = 0;
    let offsetInElementY = 0;
    let clientX = 0;
    let clientY = 0;

    /* Functions */
    let coords = spring({ x: 0, y: 0 }, {
        stiffness: 1.0,
        damping: 1.0
    });

    const handleMouseDown = (event, item) => {
        const target = event.target.getBoundingClientRect();

        offsetInElementX = (target.width - (target.right - event.clientX)) * 0.7222;
        offsetInElementY = (target.height - (target.bottom - event.clientY)) * 0.7222;
        DragonDropData = item;
        coords.set({ x: event.clientX, y: event.clientY });
        clientX = event.clientX;
        clientY = event.clientY;
    }
    
    let favoriteIndex = -1;
    function handleFavoriteSlotMouseEnter (index) {
        favoriteIndex = index;
    }
	
	// Когда выходим из зоны ячейки
	function handleFavoriteSlotMouseLeave() {
        favoriteIndex = -1;
    }

    let fastSlotIndex = -1;
    let fastSlotAnim = true;

    function handleFastSlotMouseEnter (index) {
        fastSlotIndex = index;
    }
	
	// Когда выходим из зоны ячейки
	function handleFastSlotMouseLeave() {
        fastSlotIndex = -1;
    }

    const handleGlobalMouseUp = () => {
        if (fastSlotIndex !== -1 && DragonDropData != "" && DragonDropData.split("_") && DragonDropData.split("_").length) {
            window.animationStore.addAnimBind(fastSlotIndex, DragonDropData);
        }
        DragonDropData = "";
        fastSlotAnim = false;
    }


    const onDell = (item) => {
        window.animationStore.dellAnimBind(item);
    }
    // Глобальные эвенты    
    const handleGlobalMouseMove = (event) => {
        if (DragonDropData != "" && DragonDropData.split("_") && DragonDropData.split("_").length) {
            if (clientX !== event.clientX || clientY !== event.clientY) {
                dubleClickData = ''
                coords.set({ x: event.clientX, y: event.clientY });
                fastSlotAnim = true;
            }
        }
    }

    const IsFavorite = (index, AnimListFavorites) => {
        let success = false;
        if (AnimListFavorites) {
            if (AnimListFavorites.findIndex(a => a == `${selectMenu.id}_${index}`) !== -1) success = true;
        }
        return success;
    }

    const AddFavorite = (event, item) => {
        event.stopPropagation();
        window.animationStore.addAnimFavorite(item);
    }

    const DellFavorite = (event, item) => {
        event.stopPropagation();
        window.animationStore.dellAnimFavorite(item);
    }

    const StopAnim = () => {
        viewData = false;
        executeClient ("client.animation.stop");
    }

    const OnClose = () => {
        executeClient ("escape");
    }
</script>

<svelte:window on:mouseup={handleGlobalMouseUp} on:mousemove={handleGlobalMouseMove} />
<div id="animations">
    {#if DragonDropData != "" && ($coords.x !== clientX || $coords.y !== clientY) && DragonDropData.split("_") && DragonDropData.split("_").length}
        <div class="animations__dragondrop" style={`top:${$coords.y - offsetInElementY}px;left:${$coords.x - offsetInElementX}px;`}>  
            <div class="animations__dragondrop_block">
                <div class="animations__element_preview" />
            </div>
        </div>  
    {/if}
    <div class="animations__header">
        <div class="box-flex">
            <div class="animationsicon-mainanim"></div>
            <div class="animations-title">{translateText('player', 'Анимации')}</div>
        </div>
        <div class="quests-newbutton right">
            <div class="box-KeyAnimation m-l-26" on:click={OnClose}>
                <div>{translateText('player', 'Выйти')}</div>
                <KeyAnimation keyCode={27}>ESC</KeyAnimation>
            </div>
            {#if viewData}
            <div class="box-KeyAnimation" on:click={StopAnim}>
                <div>{translateText('player', 'Чтобы сбросить анимацию, нажмите дважды')}.</div>
                <KeyAnimation keyCode={$keys[8]}>{keysName[$keys[8]]}</KeyAnimation>
            </div>
            {/if}
        </div>
    </div>
    <div class="animations__main">
        <div class="animations__categories">
            <div class="animations__subtitle">{translateText('player', 'Категории')}</div>
            <div class="animations__categories_list">
                
                {#each animMenuList as item, index}
                <div class="animations__categories_element" class:active={item.title === selectMenu.title} on:click={() => onSelectMenu (index)}>
                    <div class="box-column">
                        <div class="animations__categories_title">{item.title}</div>
                        <div class="animations__categories_subtitle">
                            <span class="orange">{item.count}</span> {translateText('player', 'анимаций')}
                        </div>
                    </div>
                    <div class="animations__categories_img" />
                </div>
                {/each}
            </div>
        </div>
        <div class="animations__line"></div>
        <div class="animations__block">
            <div class="animations__block_header">
                <div class="animations__subtitle">{selectMenu.title}</div>
                <div class="animations__find_block">
                    <span class="animationsicon-loop2"></span>
                    <input class="animations__input" bind:value={searchText} placeholder="Введите название"/>
                </div>
            </div>
            <div class="animations__grid">
                {#if selectMenu.id === 0}
                    {#each favoritesAnim as animation, index}
                        {#if (!searchText || !searchText.length) || (searchText && Animations [animation][2].toLowerCase().trim().includes(searchText.toLowerCase().trim()))}
                        <AnimElement
                            title={Animations [animation][2]}
                            isEnterAnim={favoriteIndex === index}
                            use={animation}
                            isFavorite={true}
                            dellFavorite={DellFavorite}
                            {onPlayAnimation}
                            on:mousedown={(event) => handleMouseDown (event, animation)}
                            on:mouseenter={() => handleFavoriteSlotMouseEnter (index)}
                            on:mouseleave={handleFavoriteSlotMouseLeave} />
                        {/if}
                    {/each}
                {:else}
                    {#each Object.values(Animations).filter(el => el[0] === selectMenu.title) as animation, index}
                        {#if (!searchText || !searchText.length) || (searchText && animation[2].toLowerCase().trim().includes(searchText.toLowerCase().trim()))}
                        <AnimElement
                            title={animation[2]}
                            isEnterAnim={enterAnim === `${selectMenu.id}_${animation[1]}`}
                            use={`${selectMenu.id}_${animation[1]}`}
                            isFavorite={IsFavorite(animation[1], favoritesAnim)}
                            addFavorite={AddFavorite}
                            dellFavorite={DellFavorite}
                            {onPlayAnimation}
                            on:mousedown={(event) => handleMouseDown (event, `${selectMenu.id}_${animation[1]}`)}
                            on:mouseenter={() => handleSlotMouseEnter (animation[1])}
                            on:mouseleave={handleSlotMouseLeave} />
                        {/if}
                    {/each}
                {/if}
            </div>
        </div>
    </div>
    <div class="animations__fastbuttons">
        {#each $storeAnimBind as item, index}
        <div class="animations__fast_element">
            <div class="animations__fast_button">{index + 1 === 10 ? 0 : index + 1}</div>
            {#if $storeAnimBind && Animations [item]}
            <AnimElement
                isEnterAnim={fastSlotIndex === index}
                use={item}
                isBind={true}
                {onDell}
                {onPlayAnimation}
                on:mouseenter={() => handleFastSlotMouseEnter (index)}
                on:mouseleave={handleFastSlotMouseLeave} />
            <div class="animations__fast_text">{Animations [item][2]}</div>
            {:else}
            <AnimElement
                animations__anim={fastSlotAnim}
                isBind={true}
                use={""}
                on:mouseenter={() => handleFastSlotMouseEnter (index)}
                on:mouseleave={handleFastSlotMouseLeave} />
            <div class="animations__fast_text">{translateText('player', 'Пусто')}</div>
            {/if}
        </div>
        {/each}
    </div>
</div>