<script>
	import { translateText } from 'lang'
	import Male from './../images/male.png'
	import Female from './../images/female.png'

	export let gender;
	export let name;
	export let surname;
	export let cardNO;

	const pad = (str, max = 6) => {
		str = str.toString();
		return str.length < max ? pad("0" + str, max) : str;
	}
</script>
<div class="card lsnews rounding5" id="card">
	<div class="content">
		<div class="lg">
			<div class="logo"></div><div class="logo2"></div>
		</div>
		<div class="lc">
			<div class="ava" style="background-image: url({gender === translateText('player', 'Мужской') ? Male : Female})"/>
			<div class="desc-bl">
				<div class="desc">
					<span>{translateText('player', 'Имя')}:</span>
					<span>{name}</span>
				</div>
				<div class="desc">
					<span>Identification Card NO.</span>
					<span>RA{pad(cardNO)}</span>
				</div>	
				<div class="desc">
					<span>{translateText('player', 'Фамилия')}:</span>
					<span>{surname}</span>
				</div>
							
				<div class="desc">
					<span>{translateText('player', 'Место жительства')}:</span>
					<span>LOS SANTOS</span>
				</div>
			</div>
	
		</div>
		<div class="right">
			<span class="serial">2516318</span>
			<span class="img"></span>
		</div>
		<div class="bottom">
			<div class="leftx">LOS SANTOS NEWS</div>
			<div class="rightx">Valid 1.January.20**</div>
		</div>
	</div>
</div>