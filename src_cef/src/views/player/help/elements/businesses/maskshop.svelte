<script>
    import image from '../../images/mask.jpg'
	import image2 from '../../images/mask2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">По всему штату разбросаны <strong>магазины масок</strong>. Можно купить парочку, собрать друзей и устроить весёлый карнавал. Да?</span></p>
    <p><span style="font-weight: 400;"></span><span style="font-weight: 400;">Маска скроет имя человека, надевшего её, от всех, кроме членов союзной фракции. В ассортименте представлены сотни красочных вариантов.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Криминальные организации</strong> используют маски для ограблений и грабежей. Если при ограблении дома человек был в маске, то, в случае, если в доме сработала сигнализация, ему не будет выдан розыск.<br /><br></span><span style="font-weight: 400;"><strong>В некоторых случаях</strong> маски используются как обычный аксессуар, помогающий дополнить стиль.<br /></span><span style="font-weight: 400;"><strong>Сотрудники FIB используют маски,</strong> чтобы скрыть свою личность, ведь их работа полна секретов.<br /></span><span style="font-weight: 400;"><strong><br>Не забывайте, что сотрудники</strong> полиции могут сорвать с вас маску и арестовать, если заподозрят в чём-либо нехорошем.</span></p>

<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>