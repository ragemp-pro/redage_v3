<script>
	import { translateText } from 'lang'
	import Male from './../images/male.png'
	import Female from './../images/female.png'

	export let gender;
	export let name;
	export let surname;
	export let dateReg;
	export let cardNO;
	export let lic;

	const pad = (str, max = 6) => {
		str = str.toString();
		return str.length < max ? pad("0" + str, max) : str;
	}
</script>
<div class="card gov passport rounding5" id="card">
	<div class="content">
		<div class="desc-bl">
			<div class="m1">
				<div class="ava" style="background-image: url({gender === translateText('player', 'Мужской') ? Male : Female})"/>
				<div class="passport__idcard">
					<b>ID-CARD</b>
					<span class="number">№ {pad(cardNO)}</span>
				</div>
				<div class="pl1 m-t50">
					<div class="desc">
						<span>{translateText('player', 'Имя')}:</span>
						<span class="passport__name">{name}</span>
					</div>

					<div class="desc m-t20">
						<span>{translateText('player', 'Фамилия')}:</span>
						<span class="passport__name">{surname}</span>
					</div>
				</div>
					
				<div class="pl2 m-t50">
					<div class="desc">
						<span>{translateText('player', 'Дата прибытия')}:</span>
						<span class="passport__name">{dateReg}</span>
					</div>	
					<div class="desc m-t20">
						<span>{translateText('player', 'Пол')}:</span>
						<span class="passport__name">{gender}</span>
					</div>						
				</div>
			</div>
				
			<div class="m2">
				<div class="desc">
					<span class="passport__licenses_title">{translateText('player', 'Лицензии')}:</span>
					<span class="passport__licenses">
						<div class="icon-box" class:active={Boolean(lic[0]) == true}>
							<span class="inv-lic-moto" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[1]) == true}>
							<span class="inv-lic-car" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[2]) == true}>
							<span class="inv-lic-truck" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[3]) == true}>
							<span class="inv-lic-boat" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[4]) == true}>
							<span class="inv-lic-helipad" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[5]) == true}>
							<span class="inv-lic-drone" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[6]) == true}>
							<span class="inv-lic-gun" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[7]) == true}>
							<span class="inv-lic-medical" />
						</div>
						<div class="icon-box" class:active={Boolean(lic[8]) == true}>
							<span class="inv-lic-health" />
						</div>
					</span>
				</div>
			</div>
				
		</div>
	</div>
</div>