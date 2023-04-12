<script>
    import { translateText } from 'lang'
</script>
<div class="content s2">
    <div class="title">{translateText('donate', 'Твой приз')}</div>

    <div class="winBox">
        <div class="l">
            <img src={money} alt="" class="prize" />
        </div>
        <div class="r">
            <div class="prizeName">5000$</div>
            <div class="button">{translateText('donate', 'Забрать')}</div>
        </div>
    </div>         
</div>