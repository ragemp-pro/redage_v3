<script>
    import image from '../../images/gazon.jpg'
	import image2 from '../../images/gazon2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Отличная возможность для какого-то первоначального заработка, будучи приезжим. Работа не из грязных, да и к тому же не требует лицензий или опыта.</span></p>
    <p><span style="font-weight: 400;">После трудоустройства, необходимо отправиться на стоянку газонокосилок (<strong>M -&gt; Работы -&gt; Стоянка газонокосилок</strong>). Чтобы добраться, можно попробовать попросить подвезти вас до работы людей, стоящих у мэрии, либо воспользоваться арендой транспорта неподалеку от здания.</span></p>
    <p><span style="font-weight: 400;">После прибытия на стоянку газонокосилок, вам <strong>необходимо пройти к свободной газонокосилке и сесть в неё</strong>, после этого появится окно, в котором необходимо подтвердить аренду газонокосилки.</span></p>
    <p><span style="font-weight: 400;"><strong>Зарплата</strong>: 3$ за одну пройденную метку.<br /></span><span style="font-weight: 400;"><strong>Бонусы</strong>: Для данной работы, действуют дополнительные бонусы к зарплате при наличии VIP статуса.</span><span style="font-weight: 400;"><br /></span><span style="font-weight: 400;"><br /></span><span style="font-weight: 400;"><strong>Gold VIP</strong> - Увеличение зарплаты на 15%; <br><strong>Platinum VIP</strong> - Увеличение зарплаты на 25%; <br><strong>Diamond VIP</strong> - Увеличение зарплаты на 35%.</span></p>

    <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>