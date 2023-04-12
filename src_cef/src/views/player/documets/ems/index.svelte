<script>
	import { translateText } from 'lang'
	import Male from './../images/male.png'
	import Female from './../images/female.png'
	import Serial from './images/serial.png'

	export let gender;
	export let name;
	export let cardNO;

	const pad = (str, max = 6) => {
		str = str.toString();
		return str.length < max ? pad("0" + str, max) : str;
	}
</script>

<div class="card ems rounding5" id="card">
	<div class="content">
		<div class="title">
			<div class="star"></div>
				EMERGENCY MEDICAL SERVICE
			<div class="star"></div>
		</div>
		
		<div class="left">
			<div class="ava" style="background-image: url({gender === "Мужской" ? Male : Female})"/>
			<div class="box-column">
				<div class="label">
					<span>{translateText('player', 'Имя')}:</span>
					<span>{name}</span>
				</div>
				<div class="label">
					<span>{translateText('player', 'Проживание')}:</span>
					<span>Los Santos</span>
				</div>
			</div>
		</div>
		<div class="right emsblock">
			<div class="title-r"><span class="ic_ems"></span>EMS</div>
			<div class="h2r">ONE MISSION ONE TEAM</div>
			
			<div class="serial">
				<img src={Serial} alt="" />
				<div class="id">RA{pad(cardNO)}</div>
			</div>
		</div>
	</div>
</div>