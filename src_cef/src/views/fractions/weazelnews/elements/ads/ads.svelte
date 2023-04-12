<script>
    import { translateText } from 'lang'
    import { executeClientAsyncToGroup, executeClientToGroup } from 'api/rage'
    import {format} from "api/formatter";

    export let selectedAddId;
    export let onBackAdvert;
    let answerText;

    let advert = false;
    executeClientAsyncToGroup("getAddByID", selectedAddId).then((result) => {
        if (result && typeof result === "string"){
            advert = JSON.parse(result);
            answerText = advert.AD
        }
        else{
            answerText = "";
            onBackAdvert();
        }
    });

    const onSendAnswer = () => {
        if (!answerText) return;
        answerText = String(answerText).replace(/(\<(\/?[^>]+)>)/g, '');
        answerText = format("parseDell", answerText);
        executeClientToGroup ("send", advert.ID, answerText);
        advert = false;
        onBackAdvert();
        answerText = "";
    }

    const onDeleteAdverts = () => {
        if (!answerText || answerText == advert.text) {
            window.notificationAdd(4, 9, translateText('fractions', 'При удалении объявления Вы должны указать причину!'), 3000);
            return;
        }
        answerText = String(answerText).replace(/(\<(\/?[^>]+)>)/g, '');
        answerText = format("parseDell", answerText);
        executeClientToGroup ("delete", advert.ID, answerText);
        advert = false;
        onBackAdvert();
        answerText = "";
    }
    import { categorieName } from "@/views/player/hudevo/phonenew/components/news/data";
</script>
{#if advert}
    <div class="weazelnews__back" on:click={onBackAdvert}>
        &#x2190; {translateText('fractions', 'Вернуться к объявлениям')}
    </div>
    <div class="box-between mt-20">
        <div class="weazelnews__title">{translateText('fractions', 'Объявление')} №{advert.ID}</div>
        <div class="weazelnews__button" on:click={onDeleteAdverts}>{translateText('fractions', 'Отклонить публикацию')}</div>
    </div>
    <div class="weazelnews__info_title mt-45">{translateText('fractions', 'Отправитель')}:</div>
    <div class="weazelnews__info_subtitle">{advert.Author}</div>
    {#if advert.Link && /(?:jpg|jpeg|png)/g.test(advert.Link)}
        <div class="weazelnews__info_title mt-20">{translateText('fractions', 'Изображение')}:</div>
        <div class="weazelnews__person_image map mt-4" style="background-image: url({advert.Link})">
        </div>
    {/if}
    <div class="weazelnews__info_title mt-20">{translateText('fractions', 'Оригинальная публикация')}:</div>
    <div class="weazelnews__info_subtitle">{advert.AD}</div>
    {#if Object.values(categorieName)[advert.Type]}
        <div class="weazelnews__info_title mt-24">{translateText('fractions', 'Категория')}:</div>
        <div class="weazelnews__info_subtitle f-regular">
            {Object.values(categorieName)[advert.Type]}
        </div>
    {/if}
    <div class="weazelnews__info_title mt-20">{translateText('fractions', 'Отредактированное объявление')}:</div>
    <textarea placeholder="Введите ваш текст.." class="weazelnews__textarea w-100 mt-20" bind:value={answerText}></textarea>
    <div class="weazelnews__button mt-12" on:click={onSendAnswer}>{translateText('fractions', 'Опубликовать')}</div>
{/if}