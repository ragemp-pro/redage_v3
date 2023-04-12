<script>
    import image from '../../images/fib.jpg'
	import image2 from '../../images/fib2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Фракция</strong>, которая занимается работой по уголовным статьям.;</span><span style="font-weight: 400;">Для работы в данной фракции нужно иметь большой стаж работы в силовых структурах и иметь отличную физическую подготовку.;</span></p>
    <p><span style="font-weight: 400;"><strong>Среди всех</strong> силовых структур, FIB являются приоритетными на различных секретных операциях и непосредственно занимаются командованием. <strong>Форма</strong>, <strong>автомобили</strong> и <strong>оружие</strong> у данной фракции соответствует их статусу и считается самым лучшим. Работая в данной фракции вы сможете оказывать огромную поддержку для штата.</span></p>
    <p><strong>Кнопки</strong>
        <br><span style="font-weight: 400;">&ldquo;<strong>U</strong>&rdquo;- Бортовой компьютер&nbsp;<br /></span>&ldquo;<strong>X</strong>&rdquo; - Надеть наручники на человека
            <br />&ldquo;<strong>Z</strong>&rdquo; - Вести человека за собой<br></p>
            <p><strong>Команды</strong><span style="font-weight: 400;"><br>
                <strong>/m</strong> - использование мегафона<br /></span>
    <span style="font-weight: 400;"><strong>/su</strong> - подать человека в розыск<br /></span><span style="font-weight: 400;"><strong>/rfp</strong> - освободить человека из тюрьмы<br /></span>
    <span style="font-weight: 400;"><strong>/incar</strong> - затащить человека в машину<br /></span><span style="font-weight: 400;"><strong>/pull</strong> - вытащить человека из машины</span><br />
    <span style="font-weight: 400;"><strong>/opengs</strong> - открыть склад №2</span><br />
    <span style="font-weight: 400;"><strong>/closegs</strong> - закрыть склад №2</span>
    <br /><strong>/pd</strong> - принять вызов игрока
    <br /><strong>/to</strong> - принять CODE 0
    <br /><strong>/arrest</strong> - посадить игрока в камеру
    <br /><strong>/givegunlic</strong> - выдать лицензию на оружие
    <br /><strong>/takegunlic</strong> - изъять лицензию на оружие
    <br /><strong>/warg</strong> - включить режим ЧП
    <br /><strong>/carsu</strong> - объявить автомобиль в розыск</p>
    

    <div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>