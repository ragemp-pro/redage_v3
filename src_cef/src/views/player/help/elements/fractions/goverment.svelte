<script>
    import image from '../../images/gov.jpg'
	import image2 from '../../images/gov2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Фракция</strong>, включающая в себя <strong>судебную</strong>, <strong>исполнительную</strong>, и <strong>законодательную</strong> власть. Занимается созданием законов, принятием важных решений касательно всего штата, проведением мероприятий, а также финансированием различных отраслей в штате.</span></p>
    <p><span style="font-weight: 400;">Можете <strong>быть секретарём</strong>, <strong>охраной губернатора</strong>, <strong>судьей</strong>, <strong>прокурором</strong>, <strong>сотрудником секретной службы</strong>, или <strong>даже самим губернатором</strong> -  возможности ограничены лишь вашим потенциалом.</span></p>
    <p><span style="font-weight: 400;">На ней лежит <strong>управление всеми</strong>&nbsp;государственными службами. Форма сотрудников преимущественно состоит из одежды делового стиля, а у сотрудников секретной службы это стандартная форма черного цвета. <strong>Автопарк</strong> состоит из автомобилей для деловых поездок и окрашен в белый цвет.</span></p>
    <p><span style="font-weight: 400;"><strong>Для вступления</strong> в ряды правительства необходим паспорт и медицинская карта. Обычно трудоустройство происходит в здании правительства (Мэрии), об этом заранее сообщается в государственных новостях.</span></p>
    <p><strong>Кнопки</strong>
        <br><span style="font-weight: 400;"><strong>X</strong> - Снять/одеть наручники;<br /></span><strong>Z</strong> - Вести за собой<br>
        <strong> "G" > Фракция > Обыскать</strong> - Провести обыск вещей у игрока <br>
            <p><strong>Команды</strong>
            <br /><strong>/incar</strong> - посадить нарушителя в машину
            <br /><strong>/pull</strong> - выкинуть игрока из машины
            <br /><strong>/takegunlic</strong> - изъять лицензию на оружие
            <br /><strong>/su</strong> - подать человека в розыск
            <br /><strong>/ticket</strong> - выписать штраф
            <br /><strong>/go</strong> - принять вызов игрока</p>
            
    <div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>