<script>
    import { translateText } from 'lang'

    import { TimeFormat } from 'api/moment'
    import { loadImage } from 'api/functions'

    import moment from 'moment';
    import { executeClientAsyncToGroup, executeClientToGroup } from "api/rage";

    let data = []

    executeClientAsyncToGroup("getGallery").then((result) => {
        if (result && typeof result === "string")
            data = JSON.parse(result);
    });

    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'


    let selectedImage = null;
    let isDeletePopup = false;


    const onDell = (link) => {

        executeClientToGroup ("dellGallery", link);

        const index = data.findIndex(a => a[0] === link);
        if (data [index])
            data.splice(index, 1);

        selectedImage = null;
        isDeletePopup = false;
    }
    
    const getImagePlace = (images, selectImage) => {

    }
    import { fade } from 'svelte/transition'
</script>
<div class="newphone__gallery" style={selectedImage ? "background: linear-gradient(180deg, rgba(180,180,180,1) 0%, rgba(247,247,239,1) 10%)" : "background: #FEFEFE"} in:fade>
    <Header />
    {#if selectedImage == null}
        <div class="newphone__gallery_title">{translateText('player2', 'Галерея')}</div>
        <div class="newphone__gallery_grid">
            {#each data as item}
                <div class="newphone__gallery_item" use:loadImage={item[0]} on:click={() => selectedImage = item} />
            {/each}
        </div>
        <div class="newphone__gallery_count">{data.length} {translateText('player2', 'фото')}</div>
    {:else}
        {#if isDeletePopup}
            <div class="newphone__popup">
                <div class="newphone__popup_element red" on:click={() => onDell (selectedImage[0])}>{translateText('player2', 'Удалить фото')}</div>
                <div class="newphone__popup_element" on:click={() => isDeletePopup = false}>{translateText('player2', 'Отмена')}</div>
            </div>
        {/if}
        <div class="newphone__gallery_header">
            <div class="phoneicons-Button newphone__backicon" on:click={() => selectedImage = null}></div>
            <div class="newphone__gallery_date">
                <div class="newphone__gallery_month">{TimeFormat (selectedImage[1], "DD.MM")}</div>
                <div class="newphone__gallery_time">{TimeFormat (selectedImage[1], "HH:mm")}</div>
            </div>
            <div class="phoneicons-Button" style="opacity: 0"></div>
        </div>
        <div class="newphone__gallery_image" use:loadImage={selectedImage[0]}></div>
        <div class="newphone__gallery_footer">
            <div class="newphone__gallery_last">
                {#each data as item}
                    <div class="newphone__galleryfooter_item" use:loadImage={item[0]} on:click={() => selectedImage = item} />
                {/each}
            </div>
            <div class="box-between">
                <div class="phoneicons-upload" style="opacity:0"></div>
                <div class="phoneicons-trash"  on:click={() => isDeletePopup = true}></div>
            </div>
        </div>
    {/if}
    <HomeButton />
</div>