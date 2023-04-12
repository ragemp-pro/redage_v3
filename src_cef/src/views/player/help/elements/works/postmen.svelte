<script>
    import image from '../../images/post.jpg'
	import image2 from '../../images/post2.jpg'
    export let onWayPoint;
</script>

<div class="text">
  <p>
    <span style="font-weight: 400;">Почтальону необходимо хорошо знать местность, ведь без этого он не сможет развозить посылки быстро и зарабатывать много $$$.</span>
  </p>
  <p>
    <span style="font-weight: 400;">После трудоустройства отправьтесь к отделению почты ( <strong>M -&gt; Работы -&gt; Отделение почты</strong>). В этом поможет служба такси, или, например, можно воспользоваться арендными машинами возле мэрии. </span>
  </p>
  <p>
    <span style="font-weight: 400;">По прибытию к отделению, необходимо пройти к маркеру у двери и нажать "E", после этого откроется телефон, в котором вы сможете начать или закончить работу, а так же взять дополнительные посылки.</span>
  </p>
  <p>
    <span style="font-weight: 400;">После начала работы нужно пройти на маркер " <strong>Взять рабочий транспорт</strong>", он появится, и вы сможете развозить посылки. Так же есть возможность развозить посылки на личном транспорте и даже в компании друзей. </span>
  </p>
  <p>
    <span style="font-weight: 400;">
      <strong>Требования</strong>: права категории &ldquo; <strong>B</strong>&rdquo; <br />
    </span>
    <span style="font-weight: 400;">
      <strong>Зарплата</strong>: примерно 200$ за час работы. <br />
    </span>
    <span style="font-weight: 400;">
      <strong>Бонусы</strong>: Для данной работы, действуют дополнительные бонусы к зарплате при наличии VIP статуса.&nbsp; </span>
  </p>
  <p>
    <span style="font-weight: 400;">
      <strong>Gold VIP</strong> - Увеличение зарплаты на 15%; <br>
      <strong>PLatinum VIP</strong> - Увеличение зарплаты на 25%; <br>
      <strong>Diamond VIP</strong> - Увеличение зарплаты на 35%. </span>
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