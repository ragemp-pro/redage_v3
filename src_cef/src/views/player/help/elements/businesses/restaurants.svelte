<script>
    import image from '../../images/burger.jpg'
	import image2 from '../../images/burger2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">По всему штату размещены десятки предприятий быстрого питания, в которых каждый желающий может приобрести себе сочный бургер или сосиску в тесте! Ниже представлена энергетическая ценность продуктов в соотношении к вашему здоровью: </span><span style="font-weight: 400;"></span></p>
    <p><span style="font-weight: 400;"><strong>Бургер</strong> - пополняет 30% здоровья.<br /></span><span style="font-weight: 400;"><strong>Хот-Дог</strong> - пополняет 30% здоровья.&nbsp;<br /></span><strong>Сэндвич</strong> - пополняет 30% здоровья.<br /><span style="font-weight: 400;"><strong>eCola</strong> - пополняет 10% здоровья.<br /></span><span style="font-weight: 400;"><strong>Sprunk</strong> -пополняет 10% здоровья.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Стоимость</strong> на напитки начинается с 3$ и ограничивается 5$, стоимость еды начинается с 10$ и ограничивается 24$.</span></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>