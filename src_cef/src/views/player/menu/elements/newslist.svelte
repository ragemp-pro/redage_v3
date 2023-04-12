<script>
    import { newsData } from 'store/news.js'
</script>

{#if $newsData.length}
<div class="box-news">
    <div class="news-main"  style="background: linear-gradient(360deg, rgba(27, 30, 40, 0.9) 36.2%, rgba(27, 30, 40, 0) 94.27%), url({$newsData[0].image_s3});background-size: cover, contain, background-position: unset, top;background-repeat: no-repeat, no-repeat;background-position: center;background-size: cover;">
        <div class="bot-desc">
            <div class="title">{$newsData[0].headingName}</div>
            <div class="desc">{$newsData[0].description}</div>
        </div>
    </div>
    <div class="news-list">
        {#each $newsData.slice(1) as post, _}
            <div class="news-box" key={post.id} style="background: linear-gradient(360deg, rgba(27, 30, 40, 0.9) 36.2%, rgba(27, 30, 40, 0) 94.27%), url({post.image_s3});background-size: cover, contain, background-position: unset, top;background-repeat: no-repeat, no-repeat;background-position: center;background-size: cover;"> 
                <div class="bot-desc">
                    <div class="title">{post.headingName}</div>
                    <div class="desc">{post.description}</div>
                </div>
                <div class="data"></div>
            </div>
        {/each}
    </div>
</div>
{/if}