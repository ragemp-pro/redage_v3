<script>
    import image from '../../images/daln.jpg'
	import image2 from '../../images/daln2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Если ваш организм привык к долгому отсутствию сна и вы также можете сохранять концентрацию и следить за дорогой, то данная работа для вас.</span></p>
    <p><span style="font-weight: 400;">После трудоустройства необходимо выйти из мэрии и использовать навигатор в телефоне, выбрать &laquo;<strong>GPS &gt; Работы &gt; Стоянка дальнобойщиков</strong>&raquo;.</span></p>
    <p><span style="font-weight: 400;">После прибытия на стоянку, следует пройти к свободному автомобилю и сесть в него, после этого будет предложено арендовать его. Чтобы открыть меню с заказами, нужно прописать команду &ldquo;<strong>/orders</strong>&rdquo; и выбрать понравившийся заказ, который нужно будет забрать со склада и отвезти заказчику.</span></p>
    <p><span style="font-weight: 400;"><strong>Требования</strong>: права категории &ldquo;<strong>C</strong>&rdquo;<br /></span><span style="font-weight: 400;"><strong>Зарплата</strong>: от 800$ до 3.500$</span></p>
    <p><span style="font-weight: 400;"><strong>Команды</strong>:</span></p>
    <p><span style="font-weight: 400;"><strong>/orders</strong> - открыть меню с выбором заказа (работает только на точке загрузки товаров)<br />На точках загрузки товаров были добавлены метки взаимодействия, которые так же открывают меню заказов.<br /></span><strong>/t</strong> - рация дальнобойщиков.<br /><strong>/findtrailer</strong> - если вы потеряли трейлер, данная команда поможет вам его найти.<br /><strong>/mypoint</strong> - с помощью данной команды, вы сможете узнать куда вам необходимо доставить груз.</p>
    <p><br /><br /></p>
    
    <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>