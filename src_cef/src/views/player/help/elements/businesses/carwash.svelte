<script>
    import image from '../../images/lawn.jpg'
	import image2 from '../../images/lawn2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Ваша машина совсем утонула в грязи?</strong> Тогда быстрее едьте в одну из ближайших автомоек.</span></p>
    <p><span style="font-weight: 400;">Чтобы воспользоваться услугами автомойки, необходимо подъехать на вашем транспорте к "<strong>Маркеру</strong>", который расположен в автомойке, <strong>заглушить ваш транспорт</strong> и нажать "<strong>E</strong>", после этого подтвердить, что вы хотите воспользоваться данной услугой. После подтверждения транспорт снова будет чист.</span></p>
    <p><span style="font-weight: 400;"><strong>Стоимость</strong> одной мойки транспорта - от 95$ до 175$.</span></p>
    
		<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайшую<span class="icon map"/></div>
    </div>
</div>