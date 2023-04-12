<script>
    import image from '../../images/waxta.jpg'
	import image2 from '../../images/lesorub.jpg'
    export let onWayPoint;
</script>

<div class="text">
    <p>
      <span style="font-weight: 400;">Невероятная возможность не упустить ни копейки и получить от жизни максимум - <strong>подработки!</strong>
      </span>
    </p>
    <p>
      <span style="font-weight: 400;">На данный момент можно копать руду и рубить деревья. <br><strong>GPS &gt; Подработка &gt; Шахта/Лесоруб</strong>. <br><br><span>Чтобы работать на любой из подработок, достаточно лишь иметь необходимый инструмент, который можно преобрести в специальном магазине. <br><strong>GPS &gt; Прочее &gt; Охотничий магазин.</strong>
        </span>
      </span>
    </p>
    <p>
      <span style="font-weight: 400;">Главная "фишка" данных подработок в том, что персонаж делает всю работу за вас! <br>
        <strong>Достаточно лишь подойти к дереву или шахте с инструментом и нажать "Е"</strong> и можно смело отправляться по своим делам, а по возвращению получить ресурсы, продать их и заработать деньги, либо же скрафтить себе мебель в дом. </span>
    </p>
    <div style="display: flex; flex-direction: column;">
      <img src={image} alt="News" class="contentImg" />
      <img src={image2} alt="News" class="contentImg" />
    </div>
    <div class="button aligncenter">
      <div class="btn" on:click={onWayPoint}>Показать на карте <span class="icon map" />
      </div>
    </div>
  </div>