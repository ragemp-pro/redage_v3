<script>
    import image from '../../images/electric.jpg'
	import image2 from '../../images/electric2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Одна из начальных профессий для только прибывших в штат граждан. Электрики занимаются ремонтом оборудования... На электростанции. &nbsp;</span></p>
    <p><span style="font-weight: 400;">После трудоустройства необходимо отправиться на электростанцию <strong>(M -&gt; Работы -&gt; Электростанция)</strong>. Можно арендовать автомобиль на стоянке транспорта, или, например, вызвать такси <strong>(M -&gt; Звонки -&gt; Вызывать такси)</strong>.</span></p>
    <p><span style="font-weight: 400;">После прибытия на электростанцию необходимо подойти к прорабу, который стоит в будке, и начать рабочий день.&nbsp;&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Зарплата</strong>: 5$ за одну пройденную метку.</span><span style="font-weight: 400;"><br /></span><span style="font-weight: 400;"><strong>Бонусы</strong>: Для данной работы, действуют дополнительные бонусы к зарплате при наличии <strong>VIP статуса</strong>.</span><span style="font-weight: 400;"><br /></span><span style="font-weight: 400;"><br /></span><span style="font-weight: 400;"><strong>Gold VIP</strong> - Увеличение зарплаты на 15%; <br> <strong>Platinum VIP</strong> - Увеличение зарплаты на 25%; <br> <strong>Diamond VIP</strong> - Увеличение зарплаты на 35%.</span></p>

    <div style="display: flex; flex-direction: column;">
        <img src={image} alt="News" class="contentImg"/>
        <img src={image2} alt="News" class="contentImg"/>
    </div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>