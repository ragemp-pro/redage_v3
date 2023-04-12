<script>
    import image from '../../images/inkas.jpg'
	import image2 from '../../images/inkas2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Доставляя деньги по точкам, Вас могут преследовать бандиты и прочие криминальные личности которым за радость будет отнять у Вас деньги.</span></p>
    <p><span style="font-weight: 400;">После трудоустройства вам необходимо выйти из мэрии и использовать навигатор в телефоне, выбрать &laquo;<strong>GPS &gt; Работы &gt; Стоянка инкассаторов</strong>&raquo;.&nbsp;</span><span style="font-weight: 400;">После прибытия на стоянку, вам необходимо пройти к свободному автомобилю и сесть в него, после этого вам предложат арендовать его.</span></p>
    <p><span style="font-weight: 400;"><strong>Требования</strong>: права категории &ldquo;<strong>C</strong>&rdquo;<br /></span><span style="font-weight: 400;"><strong>Бонусы</strong>: Для данной работы, действуют дополнительные бонусы к зарплате при наличии VIP статуса.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Gold VIP</strong> - Увеличение зарплаты на <em>15%</em>; <strong>Platinum VIP</strong> - Увеличение зарплаты на <em>25%</em>; <strong>Diamond VIP</strong> - Увеличение зарплаты на <em>35%</em>.</span></p>
    <p><span style="font-weight: 400;"><strong>Команды</strong>:&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>/mypoint</strong> - с помощью данной команды, вы сможете узнать куда вам необходимо доставить деньги.</span></p>
    
    <div style="display: flex; flex-direction: column;"><img src={image} alt="News" class="contentImg"/><img src={image2} alt="News" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>