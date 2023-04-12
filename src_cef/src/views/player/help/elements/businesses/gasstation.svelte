<script>
    import image from '../../images/azs.jpg'
	import image2 from '../../images/azs2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Не хватает бензина чтобы проехать вокруг штата? А канистру купить забыли? Скорее езжайте на ближайшую АЗС!</span></p>
    <p><span style="font-weight: 400;"><strong>При подъезде</strong> на заправочную станцию необходимо <strong>заглушить транспортное средство</strong> и нажать клавишу "<strong>E</strong>", после чего нужно <strong>ввести необходимое</strong> количество топлива.</span></p>
    <p><span style="font-weight: 400;">Также вы <strong>можете использовать следующие клавиши</strong>:&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>FULL</strong> - Заправит ваш автомобиль до полного бака;&nbsp;<br /></span><strong>GOV</strong> - Бесплатно заправит ваш автомобиль, данная клавиша доступна только сотрудникам государственных организаций.</p>
    <p></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайшую<span class="icon map"/></div>
    </div>
</div>