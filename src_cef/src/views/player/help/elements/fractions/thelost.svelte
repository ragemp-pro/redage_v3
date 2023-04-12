<script>
    import image from '../../images/lost.jpg'
	import image2 from '../../images/lost2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><strong>Место</strong>, где обитают настоящие байкеры. Организация, тайны которой объяты мраком. На первый взгляд это обычная группировка мотоциклистов, которая мирно катается по штату в своих интересах. Кто знает, что у них на уме....</p>
    <p><span style="font-weight: 400;">Байкеры занимаются <strong>грабежами</strong>, <strong>продажей оружия</strong>, <strong> продажей нелегальных веществ</strong> и <strong>помощью бандам и мафиям в перекрытии поставок или ограблениях</strong>.</span></p>
    <p><span style="font-weight: 400;"><strong>Набор в The Lost</strong> проходит через подачу объявлений. Если есть желание попасть к ним - нужно неплохо разбираться в мототранспорте и иметь волевые нервы.</span></p>

    <div style="display: flex; flex-direction: column;"><img src={image} alt="Lost" class="contentImg"/><img src={image2} alt="Lost" class="contentImg"/></div>

    <div class="button aligncenter">
	    <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>