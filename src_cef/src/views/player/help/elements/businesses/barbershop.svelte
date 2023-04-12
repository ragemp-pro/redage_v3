<script>
    import image from '../../images/barb1.png'
	import image2 from '../../images/barb2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Внешность вашего персонажа</strong> один из самых важных спектров. Хотите сменить свою прическу или макияж? Первым делом необходимо зайти в один из барбершопов штата.</span></p>
    <p><span style="font-weight: 400;">В барбершопе <strong>доступны следующие услуги</strong>:</span></p>
    <p><strong>Прически<br /></strong><strong>Борода<br /></strong><strong>Брови<br /></strong><strong>Грудь<br /></strong><strong>Линзы<br /></strong><strong>Помада<br /></strong><strong>Румянец<br /></strong><strong>Тени</strong></p>
    <p><span style="font-weight: 400;"><strong>Все услуги</strong> позволят сделать вашего персонажа уникальным, но не стоит забывать, что <strong>в некоторых фракциях запрещено</strong> иметь некоторые виды причесок и т.д.</span></p>

<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>

    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>