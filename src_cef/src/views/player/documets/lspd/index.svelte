<script>
	import { translateText } from 'lang'
	import Male from './../images/male.png'
	import Female from './../images/female.png'

	export let gender;
	export let name;
	export let surname;
	export let rank;
	export let cardNO;

	const pad = (str, max = 6) => {
		str = str.toString();
		return str.length < max ? pad("0" + str, max) : str;
	}

</script>
<div class="lspd_leather" id="card">
	<div class="ct">
		<div class="left">
			<div class="a rounding5">
			
				<div class="title">
					<span>LOS SANTOS</span>
					<span>POLICE DEPARTMENT</span>
				</div>
				
				<div class="ava" style="background-image: url({gender === "Мужской" ? Male : Female})"/>
				
				<div class="desc">
					<span class="line">{name} {surname}</span>
					<span>{translateText('player', 'Имя')}:</span>
				</div>
				
				<div class="desc">
					<span class="line">{rank}</span>
					<span>{translateText('player', 'Должность')}:</span>
				</div>
				
				<div class="desc">
					<span class="line">RA{pad(cardNO)}</span>
					<span>{translateText('player', 'Жетон')}:</span>
				</div>
				
				<p>THIS IS THE WARRANT AND AUTHORITY FOR EXECUTING THE DUTIES OF THEIR OFFICE.</p>
				
			</div>
		</div>
		<div class="center"></div>
		<div class="right"></div>
	</div>
</div>