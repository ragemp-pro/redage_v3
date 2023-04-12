<script>
    import image from '../../images/ar.jpg'
    import image2 from '../../images/ar2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Автосалон эконом класса</strong> - в ассортименте <strong>самые дешевые автомобили</strong>, отлично подходят для покупки человеком, недавно приехавшим в штат.<br /><strong>Стоимость автомобилей</strong> в данном автосалоне: от 700$ до 25.000$</span></p>
    <p><span style="font-weight: 400;"><strong>Автосалон премиум класса</strong> - в данном салоне представлены автомобили среднего ценового сегмента, очень много интересных вариантов.<br /><strong>Стоимость автомобилей</strong> в данном автосалоне: от 6.000$ до 160.000$.</span></p>
    <p><span style="font-weight: 400;"><strong>Автосалон люкс класса</strong> - однажды Люкс-салон был самым желанным и престижным в нашем штате, пока не появился следующий... В наличии самые престижные автомобили отечественного производства.<br /></span><span style="font-weight: 400;"><strong>Стоимость автомобилей</strong> в данном автосалоне от 38.500$ до 1.000.000$.</span></p>
    <p><span style="font-weight: 400;"><strong>Автосалон элитного класса</strong> - как-то раз в штат прибыл контейнер с иностранными автомобилями, цены которых кусаются, а внешний вид восторгает.<br /><strong>Стоимость автомобилей</strong> в данном автосалоне от 250.000$ до 3.000.000$.</span></p>
    <p><span style="font-weight: 400;"><strong>Мотосалон</strong> - если есть желание обуздать своего железного коня - стоит заглянуть сюда. Байкерам ку!<br /><strong>Стоимость транспорта</strong> в данном салоне от 250$ до 500.000$.</span><span style="font-weight: 400;"><br /></span></p>
    <p><span style="font-weight: 400;"><strong>При покупке транспорта</strong>&nbsp; есть возможность <strong>выбрать цвет</strong> и <strong>посмотреть характеристики</strong> в специальной панели. Важно отметить, что КАЖДОЕ транспортное средство может быть протестировано с помощью <strong>тест-драйва</strong>. Попробуйте!</span></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>