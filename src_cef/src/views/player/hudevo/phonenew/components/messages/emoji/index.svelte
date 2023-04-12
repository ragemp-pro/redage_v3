<script>
    import { translateText } from 'lang'
    export let addSmile;

    import { getListData, getEmojiList, getEmojiCategory } from './data';

    const emojiCategory = getEmojiCategory ();
    const emojiList = getEmojiList ();
    const emojiListValues = Object.values(emojiList);



    let selectedColor = 0;

    let selectedEmoji = false;
    let isLeave = true;


    const colorsList = [
        "yellow",
        "white",
        "medium",
        "zagar",
        "nigger",
        "makaka",
    ]

    let isToggledColor = false;

    let updateToggledColor = (toggled) => isToggledColor = toggled;

    const onSelectColor = (index) => {
        selectedColor = index;
        updateToggledColor (false);
        if (isLeave)
            selectedEmoji = defaultList [selectedColor];
    }

    const defaultList = [
        //emojiList [":man_standing_5:"]
    ];

    const setDefaultList = (name) => {
        colorsList.forEach((_, index) => {
            let newName = "";
            if (index > 0)
                newName = `:${name}_${index}:`;
            else
                newName = `:${name}:`;

            const data = getListData (newName);

            data.name = translateText('player2', 'Выбор эмодзи')

            if (data)
                defaultList.push(data);

            selectedEmoji = defaultList [0];
        })
    }

    setDefaultList ('angel');

    const handleSlotMouseEnter = (emoji) => {
        selectedEmoji = emoji;
        isLeave = false;
    }


    const handleSlotMouseLeave = () => {
        selectedEmoji = defaultList [selectedColor];
        updateToggledColor (false);
        isLeave = true;
    }


    const CategoryList = [
        /*{
            icon: "phonesmiles-recent-svgrepo-com",
            name: "Последние",
            index: -1
        },*/
        {
            icon: "phonesmiles-smile-svgrepo-com",
            name: "Smileys & Emotion",
            index: 6
        },
        {
            icon: "phonesmiles-smile-svgrepo-com",
            name: "People & Body",
            index: 5
        },
        {
            icon: "phonesmiles-dog-svgrepo-com",
            name: "Animals & Nature",
            index: 1
        },
        {
            icon: "phonesmiles-apple-svgrepo-com",
            name: "Food & Drink",
            index: 3
        },
        {
            icon: "phonesmiles-soccer-svgrepo-com",
            name: "Activities",
            index: 0
        },
        {
            icon: "phonesmiles-car-svgrepo-com",
            name: "Travel & Places",
            index: 8
        },
        {
            icon: "phonesmiles-select-object-svgrepo-com",
            name: "Objects",
            index: 4
        },
        {
            icon: "phonesmiles-mathematical-symbols-svgrepo-com",
            name: "Symbols",
            index: 7
        },
        {
            icon: "phonesmiles-flag-svgrepo-com",
            name: "Flags",
            index: 2
        },
    ]

    let chatDiv;

    let activeCategory = CategoryList [0].index;

    const OnScroll = e => {
        const scrollTop = e.target.scrollTop;

        let firstTop = -1;
        CategoryList.forEach((_, index) => {
            const elementTop = document.getElementById(`category_${index}`);
            if (elementTop) {
                const react = elementTop.getBoundingClientRect();
                if (react) {
                    if (firstTop === -1)
                        firstTop = react.top;

                    if (scrollTop >= (react.top - firstTop))
                        activeCategory = index;
                }
            }
        })
    }

    const onSelectCategory = (index) => {
        let firstTop = -1;
        for(let i = 0; i < CategoryList.length; i++) {
            const elementTop = document.getElementById(`category_${i}`);
            if (elementTop) {
                const react = elementTop.getBoundingClientRect();
                if (react) {
                    firstTop = react.top;
                    break;
                }
            }
        }


        activeCategory = index;
        const elementTop = document.getElementById(`category_${index}`);
        if (elementTop) {
            const react = elementTop.getBoundingClientRect();
            if (react) {
                chatDiv.scrollTop = react.top - firstTop;
            }
        }
    }

    let searchText = "";

    const filterCheck = (data, text) => {
        if (!text || !text.length)
            return true;

        text = text.toLowerCase();
        let success = false;
        Object.values(data).forEach((item) => {
            if (!success && String (item).toLowerCase().trim().includes(text.replace(" ", "_")))
                success = true;
        })
        return success;
    }
    import { onInputFocus, onInputBlur } from "@/views/player/hudevo/phonenew/data";

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        onInputBlur ();
    });
</script>

<div class="newphone__messages_smiles">
    <div class="newphone__smiles_selector">
        {#each CategoryList as category, index}
            <div class="{category.icon} newphone__smiles_select" class:active={activeCategory === category.index} on:click={() => onSelectCategory (category.index)}></div>
        {/each}
    </div>
    <div class="newphone__smiles_input">
        <div class="phonesmiles-loop"></div>
        <input type="text" placeholder="Поиск по Эмодзи..." bind:value={searchText} on:focus={onInputFocus} on:blur={onInputBlur}>
    </div>
    <div class="newphone__smiles_block" on:scroll={OnScroll} bind:this={chatDiv}>

        {#if !searchText || !searchText.length}
            <div class="newphone__smiles_grid">
                {#each emojiCategory [activeCategory] as emoji}
                    {#if typeof emojiList[emoji] !== "undefined" && typeof emojiList[emoji].colorId === "undefined" || emojiList[emoji].colorId === selectedColor}
                        <div class="newphone__smiles_box"
                             on:click={() => addSmile (emojiList[emoji].shortname)}
                             on:mouseenter={() => handleSlotMouseEnter(emojiList[emoji])}
                             on:mouseleave={handleSlotMouseLeave}>
                            <span class="newphone__smile_img">{@html emojiList[emoji].html}</span>
                        </div>
                    {/if}
                {/each}
            </div>
        {:else}
            <div class="newphone__smiles_grid">
                {#each emojiListValues.filter(el => filterCheck({
                    name: el.name,
                    sText: el.sText,
                    shortname: el.shortname
                }, searchText)) as emoji}
                    {#if typeof emoji.colorId === "undefined" || emoji.colorId === selectedColor}
                        <div class="newphone__smiles_box"
                             on:click={() => addSmile (emoji.shortname)}
                             on:mouseenter={() => handleSlotMouseEnter(emoji)}
                             on:mouseleave={handleSlotMouseLeave}>
                            <span class="newphone__smile_img">{@html emoji.html}</span>
                        </div>
                    {/if}
                {/each}
            </div>
        {/if}

    </div>
    <div class="newphone__smiles_info">
        <span class="newphone__smiles_image">{@html selectedEmoji.html}</span>
        <div class="box-column">
            <div class="newphone__smiles_title">{selectedEmoji.name}</div>
            <div class="newphone__smiles_subtitle">{selectedEmoji.shortname}</div>
        </div>
        {#if isLeave}
            {#if !isToggledColor}
            <div class="newphone__smiles_skins">
                <div class="newphone__smiles_skin {colorsList [selectedColor]}" on:click={() => updateToggledColor (true)}></div>
            </div>
            {:else}
                <div class="newphone__smiles_skins">
                    {#each colorsList as color, index}
                        <div class="newphone__smiles_skin {color} mr-3" class:active={selectedColor === index} on:click={() => onSelectColor (index)}></div>
                    {/each}
                </div>
            {/if}
        {/if}
    </div>
</div>