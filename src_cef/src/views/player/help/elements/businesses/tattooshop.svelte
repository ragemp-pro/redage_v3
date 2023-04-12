<script>
    import image from '../../images/tatoo.jpg'
	import image2 from '../../images/tatoo2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;">Если хочется выделяться из толпы, то есть возможность украсть своё тело множеством интересных рисунков и узоров.&nbsp;</span><span style="font-weight: 400;"></span><span style="font-weight: 400;">На выбор доступно <strong>более 500 различных татуировок,</strong>&nbsp;которые можно нанести на тело с ног до головы.</span></p>
    <p><span style="font-weight: 400;"><strong>Для того чтобы набить тату</strong>, стоит зайти в тату-салон и встать на специальный "<strong>Маркер</strong>", после чего нажать "<strong>E</strong>" и у вас откроется меню с выбором места и татуировки.</span></p>
    <p><span style="font-weight: 400;">Если вы решили <strong>свести татуировку</strong>, советуем направиться в центральный госпиталь города Los Santos и найти в одной из комнат NPC, с его помощью есть возможность <strong>свести татуировки</strong> с определенной зоны на теле, стоимость одного сведения - 300$</span></p>
    <p><br /><br /></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>