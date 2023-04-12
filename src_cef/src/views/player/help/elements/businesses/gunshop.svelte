<script>
    import image from '../../images/gun.png'
	import image2 from '../../images/gun2.png'
    export let onWayPoint;
</script>

<div class="text">
    <p><span style="font-weight: 400;"><strong>Оружейный магазин</strong> представляет огромный ассортимент разнообразного вооружения от револьверов до снайперских винтовок, которое может служить своему хозяину в самых различных целях. Если чувствуете нужду - приезжайте в местный GunShop и покупайте себе ствол, благо есть из чего выбрать. </span></p>
    <p><span style="font-weight: 400;">Для покупки оружия в магазине, вы должны <strong>иметь лицензию на ношение оружия, которая получается в полицейском департаменте</strong>.</span></p><br>
    <p><span style="font-weight: 400;"><strong>В магазине продаются</strong>:&nbsp;</span></p>
    <p>Пистолеты<br />Дробовики<br />Пистолеты-пулиметы<br />Штурмовые винтовки<br />Снайперские винтовки</p>
    <p><span style="font-weight: 400;">А если вы любитель тихого, яркого и красочного оружия, можете <strong>купить модификации</strong> на него. Установите фонарик, глушитель, пламегаситель или обклейте своё орудие какой-нибудь яркой раскраской, чтоб враги боялись!
    <p><span style="font-weight: 400;"><strong>Все модификации</strong> можно приобрести в магазине оружия.</span></p>

<div style="display: flex; flex-direction: column;"><img src={image} alt="Государственные фракции" class="contentImg"/><img src={image2} alt="Государственные фракции" class="contentImg"/></div>
<br>
    <div class="button aligncenter">
        <div class="btn" on:click={onWayPoint}>Показать ближайший<span class="icon map"/></div>
    </div>
</div>