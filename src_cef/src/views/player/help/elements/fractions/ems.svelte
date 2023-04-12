<script>
    import image from '../../images/ems.jpg'
	import image2 from '../../images/ems2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Фракция, которая занимается лечением пациентов на территории всего штата.&nbsp;</span></p>
    <p><span style="font-weight: 400;">Одной из главных задач, помимо лечения жителей штата, является поддержание государственных фракций аптечками. Работая в EMS вы можете отлично себя зарекомендовать как надежный специалист в области медицины</span></p>
    <p><span style="font-weight: 400;">Для вступления в данную организацию необходимо прийти на собеседование в офис &laquo;<strong>EMS</strong>&raquo;. При себе необходимо иметь права категории &laquo;<strong>B/C</strong>&raquo; и медицинскую карту.</span></p>
    <p><span style="font-weight: 400;">Команды</span></p>
    <p><span style="font-weight: 400;"><strong>/heal [ID] [Цена]</strong> - выписать чек за лечение<br /></span><strong>/ems [ID]</strong> - принять вызов.<br /><strong>/givemedlic [ID]</strong> - выдать медицинскую карту.<br /><strong>/givepmlic [ID] [цена]</strong> - продать лицензию парамедика.<br /><strong>/openstock</strong> - открыть склад<br /><strong>/closestock</strong> - закрыть склад</p>
    
    <div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>