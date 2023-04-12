<script>
    import image from '../../images/army.jpg'
	import image2 from '../../images/army2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Одна из главных задач</strong> данной организации - обеспечение военными ресурсами других силовых государственных структур.&nbsp;</span><span style="font-weight: 400;">Необходимо защищать военные ресурсы и не давать возможности для криминальных элементов вывезти их с территории базы. В обязанности военных также входит реагирование на чрезвычайные ситуации в штате.</span></p>
<p><span style="font-weight: 400;"><strong>Для вступления</strong> в Национальную Гвардию Вам необходимо прибыть на место, которые было озвучено заранее через государственные новости и при себе иметь паспорт, медицинскую справку и водительское удостоверение категории <strong>B</strong> и <strong>C</strong>.</span></p>
<p><span style="font-weight: 400;"><strong>После вступления</strong> в ряды Национальной гвардии, Вы станете Кадетом, чтобы продвигаться по карьерной лестнице выше вам будет необходимо выполнять разные задачи от начальства.</span></p>
<p><strong>Кнопки</strong>
    <br><span style="font-weight: 400;"><strong>X</strong> - Снять/одеть наручники;<br /></span><strong>Z</strong> - Вести за собой<br>
    <strong> "G" > Фракция > Обыскать</strong> - Провести обыск вещей у игрока <br>
        <p><strong>Команды</strong><br><strong>/m</strong> - использование мегафона<br /><strong>/openstock</strong> - открыть склад<br /><strong>/closestock</strong> - закрыть склад<br /><strong>/incar</strong> - затащить человека в машину<br /><strong>/pull</strong> - вытащить человека из машины<br /><strong>/opengs</strong> - открыть склад №2<br /><strong>/closegs</strong> - закрыть склад №2</p>


    <div style="display: flex; flex-direction: column;"><img src={image} alt="mws" class="contentImg"/><img src={image2} alt="mws" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>