<script>
    import image from '../../images/news.jpg'
	import image2 from '../../images/news2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Организация</strong>, которая занимается распространением и публикацией различной информации, которая будет полезна всем жителям штата.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Постоянная обязанность</strong> сотрудников LSN - своевременная и корректная публикация объявлений от жителей штата, которые хотят продать своё авто/дом и прочее имущество.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Зарплата</strong> работников News довольно хорошая и зависит от того, насколько человек грамотен и креативен. Работая в данной организации есть возможность построить себе хорошую карьеру и быть известной личностью в штате</span></p>
    <p><span style="font-weight: 400;"><strong>Для вступления</strong> в ряды необходимо прийти на собеседование в офис &laquo;<strong>LS NEWS</strong>&raquo;. При себе обязательно нужно иметь медицинскую карту.</span></p>
    <p><strong>Кнопки/Команды</strong></p>
    <p><span style="font-weight: 400;">&ldquo;<strong>F7</strong>&rdquo; - открыть окно редактирования объявлений.</span></p>
    <p><span style="font-weight: 400;"><strong>/delad</strong> - удалить объявление</span></p>
    
   <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>