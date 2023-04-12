<script>
    import { translateText } from 'lang'
    import moment from 'moment';
    import {executeClientAsyncToGroup, executeClientToGroup} from 'api/rage'
    import { addListernEvent } from 'api/functions'

    executeClientToGroup ("isList", true);

    import { onDestroy } from 'svelte'
    onDestroy(() => {
        executeClientToGroup ("isList", false);
    });

    let adsList;
    export let getCount;

    const getList = () => {

        getCount ();


        executeClientAsyncToGroup("getAdsList").then((result) => {
            if (result && typeof result === "string")
                adsList = JSON.parse(result);
        });
    }
    getList();

    addListernEvent ("updateListAdverts", getList)

    export let onSelectAdvert;

    import { categorieName } from "@/views/player/hudevo/phonenew/components/news/data";
</script>
<div class="weazelnews__title">{translateText('fractions', 'Список объявлений')}</div>
{#if typeof adsList == 'object' && adsList.length}
    <div class="weazelnews__comments big">
        {#each adsList as advert}
            <div class="weazelnews__comments_element">
                <div class="box-between">
                    <div class="mr-10">{translateText('fractions', 'Объявление')} №{advert.ID}</div>
                    <div class="weazelnews__button"  on:click={() => onSelectAdvert (advert.ID)}>{translateText('fractions', 'Редактировать')}</div>
                </div>
                <div class="box-flex mt-4">
                    <div class="box-column mr-20">
                        <div class="weazelnews__info_title">{translateText('fractions', 'Отправитель')}:</div>
                        <div class="weazelnews__info_subtitle">{advert.Author}</div>
                    </div>
                    <div class="box-column">
                        <div class="weazelnews__info_title">{translateText('fractions', 'Дата и время')}:</div>
                        <div class="weazelnews__info_subtitle">
                            {moment(advert.Opened).format('HH:mm от DD.MM.YYYY')}
                        </div>
                    </div>
                </div>
                {#if advert.Link && /(?:jpg|jpeg|png)/g.test(advert.Link)}
                    <div class="weazelnews__info_title mt-20">{translateText('fractions', 'Изображение')}:</div>
                    <div class="weazelnews__person_image map mt-4" style="background-image: url({advert.Link})"></div>
                {/if}
                <div class="weazelnews__info_title mt-24">Описание:</div>
                <div class="weazelnews__info_subtitle f-regular">
                    {advert.AD}
                </div>
                {#if Object.values(categorieName)[advert.Type]}
                    <div class="weazelnews__info_title mt-24">{translateText('fractions', 'Категория')}:</div>
                    <div class="weazelnews__info_subtitle f-regular">
                        {Object.values(categorieName)[advert.Type]}
                    </div>
                {/if}
                {#if advert.Editor && advert.Editor.length}
                    <div class="box-column mt-20">
                        <div class="weazelnews__info_title">{translateText('fractions', 'Редактирует')}:</div>
                        <div class="weazelnews__info_subtitle">{advert.Editor}</div>
                    </div>
                {/if}
            </div>
        {/each}
    </div>
{:else}
    <div class="weazelnews__info_title mt-20">{translateText('fractions', 'Сейчас объявлений нет, но они скоро появятся')}..</div>
{/if}