<script>
    import image from '../../images/bus.jpg'
	import image2 from '../../images/bus2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Это профессия, которая требует максимальной концентрации и хороших навыков вождения. Водителю придется развозить пассажиров от одной остановки до другой.</span></p>
    <p><span style="font-weight: 400;">После трудоустройства вам необходимо выйти из мэрии и использовать навигатор в телефоне, выбрать &laquo;<strong>GPS &gt; Работы &gt; Автобусный парк</strong>&raquo;. По приезду в парк, вам необходимо выбрать нужный вам автобус, после этого сесть и арендовать его.</span></p>
    <p><span style="font-weight: 400;">После начала работы, вам будет выдан определенный маршрут, по которому необходимо следовать.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Требования</strong>: права категории &ldquo;<strong>С</strong>&rdquo;<br /></span><span style="font-weight: 400;"><strong>Зарплата:</strong> от 2$ до 6$ за метку, сумма зависит от маршрута.<br /></span><span style="font-weight: 400;"><strong>Бонусы</strong>: Для данной работы, действуют дополнительные бонусы к зарплате при наличии VIP статуса.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Gold VIP</strong> - Увеличение зарплаты на 15%;<br><strong>Platinum VIP</strong> - Увеличение зарплаты на 25%;<br><strong>Diamond VIP</strong> - Увеличение зарплаты на 35%.</span></p>
    
    <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>