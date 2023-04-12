<script>
    import image from '../../images/mws.jpg'
	import image2 from '../../images/mws2.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Частная компания</strong>, которая чаще всего работает по контракту с Goverment, оказывая им поддержку своим высококачественным оборудованием. <strong>Основной целью</strong> организации является обеспечение охраны мероприятий/персон и т.д, с которыми организация имеет контракт.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Организация занимается</strong> патрулем трассы, вокруг города Лос-Сантос, или же помогает сотрудникам LSPD / FIB по их запросам в различных заданиях.&nbsp;</span></p>
    <p><span style="font-weight: 400;"><strong>Сотрудники MWS</strong> так же имеют при себе <strong>наручники</strong> и <strong>различные специальные средства</strong>, которые служат для задержания лиц, совершивших правонарушение. Работая в данной фракции вы сможете оказать огромную поддержку нашему штату, в котором господствует закон и порядок.</span></p>
    <p><span style="font-weight: 400;">Команды/Клавиши</span></p>
    <p><span style="font-weight: 400;">&ldquo;<strong>Z</strong>&rdquo; - вести за собой<br /></span><span style="font-weight: 400;">&ldquo;<strong>G</strong>&rdquo; -&gt; Фракция<br /></span><span style="font-weight: 400;"><strong>/incar</strong> - затащить человека в машину<br /></span><span style="font-weight: 400;"><strong>/pull</strong> - вытащить человека из машины<br /></span><span style="font-weight: 400;"><strong>/eject [ID]</strong> - Выкинуть человека из машины.</span></p>
    
	<div style="display: flex; flex-direction: column;"><img src={image} alt="mws" class="contentImg"/><img src={image2} alt="mws" class="contentImg"/></div>
	
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map"/></div>
    </div>
</div>