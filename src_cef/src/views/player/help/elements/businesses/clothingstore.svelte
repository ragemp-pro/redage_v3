<script>
    import image from '../../images/clot.png'
	import image2 from '../../images/clot2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">В <strong> магазине одежды </strong> представлены сотни различных вариаций одежды, начиная "родными" коллекциями от лучших кутюрье Лос Сантоса и заканчивая нашими собственными разработками, которые обязательно придутся тебе по душе!</span></p>
    <p><span style="font-weight: 400;"><strong>После входа</strong> в магазин одежды следует пройти к кассе и встать на "<strong>Маркер</strong>", после чего нажать "<strong>E</strong>".</span></p>
    <p><span style="font-weight: 400;"><strong>Магазины одежды</strong> разбросаны по всему штату, в каждом из них владелец лично устанавливает цену на товар. <strong> Присмотрись к ассортименту</strong>, выбери <strong>лучшие</strong> вещи и будь <strong>самым модным человечком нашего прекрасного штата!</strong>.</span></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>