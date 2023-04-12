<script>
    import image from '../../images/taxi.jpg'
	import image2 from '../../images/taxi3.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Отличная возможность помочь жителям и приезжим гражданам, найти множество новых знакомств, узнать побольше о том что происходит в штате и не только..</span></p>
    <p><span style="font-weight: 400;">После трудоустройства необходимо выйти из мэрии и использовать навигатор в телефоне, выбрав &laquo;<strong>GPS &gt; Работы &gt; Таксопарк</strong>&raquo;. По приезду в таксопарк, вам необходимо выбрать нужный автомобиль (<strong>один из 4 классов</strong>), после этого сесть и арендовать его.</span></p>
    <p><span style="font-weight: 400;">Стоимость аренды такси и стоимость проезда:&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Econom</strong> - 184$ аренда, 5 - 50$ оплата стоимость проезда;<br /></span><strong>Comfort</strong> - 720$, 20$ - 100$ стоимость проезда;<br /><strong>Business</strong> - 2880$, 80$ - 400$ стоимость проезда;<br /><strong>Ultima</strong> - 5760$, 160$ - 800$ стоимость проезда.</p>
    <p>В меню такси, которое находится в телефоне, можно найти исчерпывающую информацию о заказе, например, расстояние до пассажира или его имя.</p>
    <p><span style="font-weight: 400;"><strong>Требования</strong>: права категории &ldquo;<strong>B</strong>&rdquo;</span></p>
    <p><span style="font-weight: 400;"><strong>Команды</strong>:</span></p>
    <p><span style="font-weight: 400;"><strong>/ta [ID]</strong> - п<em>ринять вызов от человека<br /></em></span><strong>/tprice [ID] [Сумма]</strong> - <em>выписать чек для оплаты клиенту</em></p>
    <p></p>

    <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>