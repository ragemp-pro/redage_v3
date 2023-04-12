<script>
    import image from '../../images/lsc1.jpg'
	import image2 from '../../images/lsc.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Мастерская для внешнего и внутреннего тюнинга.</span></p>
    <p><span style="font-weight: 400;"><strong>Для улучшения своего транспорта</strong> следует наехать на маркер около "<strong>LSC</strong>" и нажать клавишу "<strong>E</strong>", после чего вы попадете в интерфейс покупки тюнинга.</span></p>
    <p><span style="font-weight: 400;"><strong>Осмотритесь с помощью удобной камеры</strong> и сделайте свою машину по-настоящему уникальной красоткой!<br /></span><span style="font-weight: 400;"><strong>Для каждого автомобиля своя цена на улучшения</strong>, она зависит от стоимости автомобиля (Чем дешевле автомобиль, тем дешевле тюнинг и наоборот).</span></p>
    <p><span style="font-weight: 400;"><strong>Среди доступных модификаций</strong>, можно найти более двадцати различных вариантов улучшений: бампера, колёса, капоты, двигатели и другие улучшения технических характеристик автомобиля, а так же неоновые трубки, фары, градиент и еще много-много всего!&nbsp;</span></p>
    
    <p><span style="font-weight: 400;"><strong>Некоторые владельцы</strong> мастерских делают CashBack своим посетителям, подробнее вы сможете узнать непосредственно у владельца бизнеса, не стесняйтесь обращаться!</span></p>
    
	
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>