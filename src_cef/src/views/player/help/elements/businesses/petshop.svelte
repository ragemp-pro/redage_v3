<script>
    import image from '../../images/zoo.jpg'
	import image2 from '../../images/zoo2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">В зоомагазине можно купить себе <strong>домашнего питомца</strong>. Пушистый любимец будет ждать вас дома каждый день и радовать глаз своим присутствием :з</span></p>
    <p><span style="font-weight: 400;">Питомца можно купить только <strong>при наличии дома класса "Comfort+"</strong> или выше.<br /></span><span style="font-weight: 400;"><strong>Питомцы используются только в доме</strong>, после покупки питомца он будет находится на диване в доме.</span></p>
    <p><span style="font-weight: 400;">В списке <strong>доступных к покупке животных</strong> можно найти таких питомцев, как:</span></p>
    <p><span style="font-weight: 400;">Хаски, </span><span style="font-weight: 400;">Мопс, </span><span style="font-weight: 400;">Пудель, </span>Ретривер, <span style="font-weight: 400;">Ротвейлер, </span><span style="font-weight: 400;">Sherherd, </span>Westy, <span style="font-weight: 400;">Кот, </span>Кролик</p>
    <p><span style="font-weight: 400;">&nbsp;</span></p>

<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>