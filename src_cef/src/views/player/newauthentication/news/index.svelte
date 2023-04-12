<script>
    import { newsData } from 'store/news.js'
</script>
<div class="news">
    {#each $newsData as data, index}
        <div class="news-box">
            <div class="news-box__header-img" style="background-image: url({data.image_s3})">
            </div>
            <div class="news-box__text_background">
                {data.description}
            </div>
        </div>
    {/each}
</div>