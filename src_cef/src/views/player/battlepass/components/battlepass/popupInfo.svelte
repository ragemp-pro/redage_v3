<script>
    import { translateText } from 'lang'
    import { isPopupInfoOpened} from '../../stores.js'

</script>

<div class="battlepass__popup_info">
    <div class="newproject__buttonblock" on:click={() => isPopupInfoOpened.set(false)}>
        <div class="newproject__button">ТЫК</div>
        <div>{translateText('player', 'Закрыть')}</div>
    </div>
    <div class="battlepass__popup_block">
        <div class="box-between">
            <div class="battlepass__info_button">{translateText('player', 'Информация о мероприятии')}</div>
            <div class="battlepass__image_info"></div>
        </div>
        <div class="battlepass__popup_text">
            <b>{translateText('player', 'Боевой пропуск')}</b> - {@html translateText('player', 'это уровневая система вознаграждений за выполнение заданий в течение определенного времени (“сезона”). Повышается уровень засчет XP. <br> <br> <b>XP</b> - это опыт, который вы получаете за каждое задание. Чем больше XP, тем выше уровень, а с каждым новым уровнем можно забрать новые крутые награды!<br> <br> Существует два вида пропуска - <b>обычный и премиум</b>. Премиум опция приобретается за RedBucks и позволяет получать дополнительные награды за выполнение уровней, обычная - абсолютно бесплатна для каждого. <br> <br> <b>Сеть заданий</b> - это дополнительная ветка заданий, за выполненение которых вы так же сможете получить дополнительные XP и награды. Обратите внимание, что выполнение засчитывается, если вы выбрали задание, перед тем как выполнить его.')}
        </div>
    </div>
</div>