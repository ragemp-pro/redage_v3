<script>
    import image from '../../images/mech.jpg'
	import image2 from '../../images/mech2.png'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Если вы любите копаться в двигателях машин, разбираетесь в этом и делаете это быстро - то это работа определенно для вас.</span></p>
    <p><span style="font-weight: 400;">После трудоустройства вам необходимо выйти из мэрии и использовать навигатор в телефоне, выбрать &laquo;<strong>GPS &gt; Работы &gt; Стоянка автомехаников</strong>&raquo;. <span>После прибытия на стоянку автомехаников, вам необходимо пройти к свободному автомобилю и сесть в него, после этого вам предложат арендовать его.</span></span></p>
    <p><span style="font-weight: 400;">Если вы хотите <strong>не только чинить транспорт, но и заправлять его</strong>, то вам необходимо купить топливо на любой из доступных заправочных станций.</span></p>
    <p><span style="font-weight: 400;"><strong>Зарплата</strong>: от 5$ до 30$</span></p>
    <p><span style="font-weight: 400;"><strong>Команды</strong>:</span></p>
    <p><span style="font-weight: 400;"><strong>/repair [ID] [Цена]</strong> - предложить игроку починить его транспорт.&nbsp;<br /></span><strong>/sellfuel [ID] [Кол-Во литров] [Цена за 1 литр]</strong> - предложить игроку заправить его транспорт.<br /><strong>/buyfuel [Кол-во]</strong> - купить топливо, данная команда используется только на заправках<br /><strong>/ma [ID]</strong> - принять вызов.</p>

    <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>