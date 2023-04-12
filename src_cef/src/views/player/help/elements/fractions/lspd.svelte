<script>
    import image from '../../images/pd.jpg'
	import image2 from '../../images/pd2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Государственная служба, которая исполняет роль контролирующего органа. Они следят за исполнением законов штата, за массовыми мероприятиями, да даже за ситуацией на дорогах.&nbsp;</span></p>
    <p><span style="font-weight: 400;">Вступив в ряды полиции, вы сможете следить за порядками на улицах города, выполнять особо опасные задания, расследовать загадочные и странные преступления.</span></p>
    <p><span style="font-weight: 400;">Сотрудники полицейского департамента <strong>помогают</strong> другим государственным фракциям <strong>в поставках материалов и медикаментов</strong> на их базы и очень часто <strong>проводят рейды</strong> на криминальные районы города.</span></p>
    <p><span style="font-weight: 400;"><strong>Для вступления в ряды</strong> блюстителей порядка необходимо прийти на собеседование в главный офис Полицейского Департамента. При себе необходимо иметь медицинскую карту с печатью о том, что Вы здоровы.</span></p>
    <p><strong>Кнопки</strong>
    <br><span style="font-weight: 400;">&ldquo;<strong>U</strong>&rdquo;- Бортовой компьютер&nbsp;<br /></span>&ldquo;<strong>X</strong>&rdquo; - Надеть наручники на человека
        <br />&ldquo;<strong>Z</strong>&rdquo; - Вести человека за собой<br></p>
        <p><strong>Команды</strong>
        <br /><strong>/m</strong> - использование мегафона
        <br /><strong>/su</strong> - подать человека в розыск
        <br /><strong>/rfp</strong> - освободить человека из тюрьмы
        <br /><strong>/incar</strong> - затащить человека в машину
        <br /><strong>/pull</strong> - вытащить человека из машины
        <br /><strong>/opengs</strong> - открыть склад №2
        <br /><strong>/closegs</strong> - закрыть склад №2
        <br /><strong>/pd</strong> - принять вызов игрока
        <br /><strong>/to</strong> - принять CODE 0
        <br /><strong>/arrest</strong> - посадить игрока в камеру
        <br /><strong>/givegunlic</strong> - выдать лицензию на оружие
        <br /><strong>/takegunlic</strong> - изъять лицензию на оружие
        <br /><strong>/ticket</strong> - выписать штраф
        <br /><strong>/warg</strong> - включить режим ЧП
        <br /><strong>/carsu</strong> - объявить автомобиль в розыск</p>

    <div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div style="display: flex; justify-content: between;"><div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div></div>
    </div>
</div>