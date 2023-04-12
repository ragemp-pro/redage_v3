<script>
    import image from '../../images/247.jpg'
	import image2 from '../../images/2472.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">В любом магазине <strong>24/7</strong> вы сможете купить следующие предметы:</span></p>
    <p><span style="font-weight: 400;"><strong>Гаечный ключ</strong> - используется в целях самообороны, также данный ключ используется для ремонта транспорта.<br /></span><span style="font-weight: 400;"><strong>Монтировка</strong> - используется для самообороны, также с её помощью криминальные организации могут грабить дома.&nbsp;<br /></span><span style="font-weight: 400;"><strong>Фонарик</strong> - поможет осветить темные места.<br /></span><span style="font-weight: 400;"><strong>Молоток</strong> - чаще всего используется в целях самообороны.<br /></span><span style="font-weight: 400;"><strong>Сим-карта</strong> - Необходима для смены номера вашего телефона и использования телефона.<br /></span><span style="font-weight: 400;"><strong>Канистра бензина</strong> - Используется для заправки транспорта.<br /></span><span style="font-weight: 400;"><strong>Связка ключей</strong> - Собирает все ваши ключи в одну большую связку и экономит место в инвентаре.&nbsp;<br /></span><span style="font-weight: 400;"><strong>Пицца</strong> - Пополняет ваше здоровье на 30%.<br /></span><span style="font-weight: 400;"><strong>Чипсы</strong> - Пополняет ваше здоровье на 30%.&nbsp;<br /></span><span style="font-weight: 400;"><strong>Пиво</strong> - Если выпить много можно получить эффект опьянения.&nbsp;<br /></span><span style="font-weight: 400;"><strong>Лотерейный билет</strong> - Розыгрыш призового фонда с покупки билетов происходит каждый час в 00 минут, шанс на победу зависит от количества купленных вами билетов.&nbsp;<br /></span><span style="font-weight: 400;"><strong>Сумка</strong> - Предназначена для хранения предметов, используется как дополнительный инвентарь.&nbsp;</span></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>