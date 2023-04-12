<script>

    import { translateText } from 'lang'
    import { addListernEvent } from "api/functions";

    let onlineData = {
        online: 0,
        offline: 0,
        all: 0
    };
    const onOnline = (json) => {
        onlineData = JSON.parse(json);
    }
    addListernEvent ("table.online", onOnline)
</script>

<div class="fractions__stats">
    <div class="fractions__menu_element white">
        <span class="fractionsicon-wifi"></span>
        <div class="fractions__menu_text">{translateText('player1', 'Состояние')}:</div>
    </div>
    <div class="fractions__stats_element">
        <div class="fractions__stats_circle online">
            <div class="fractions__stats_smallcircle"></div>
        </div>
        <div>{onlineData.online} {translateText('player1', 'человек онлайн')}</div>
    </div>
    <div class="fractions__stats_element">
        <div class="fractions__stats_circle offline">
            <div class="fractions__stats_smallcircle"></div>
        </div>
        <div>{onlineData.offline} {translateText('player1', 'человек оффлайн')}</div>
    </div>
    <div class="fractions__stats_element">
        <div class="fractions__stats_circle neutral">
            <div class="fractions__stats_smallcircle"></div>
        </div>
        <div>{onlineData.all} {translateText('player1', 'человек всего')}</div>
    </div>
</div>