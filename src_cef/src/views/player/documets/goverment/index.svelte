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
<div class="card gov rounding5" id="card">
	<div class="content">
	<div class="usa"></div>
	<div class="left_block"></div>
		
		<div class="title">
			<div>LOS SANTOS GOVERNMENT</div>
			<div><span class="black_star"></span>IDENTIFICATION CARD<span class="black_star"></span></div>
		</div>
		
		<div class="desc-bl">
		
			<div class="ava" style="background-image: url({gender === "Мужской" ? Male : Female})"/>
			
			<div class="pl1">				
				<div class="desc">
					<span>{translateText('player', 'Имя')}</span>
					<span>{name}</span>
				</div>

				<div class="desc">
					<span>{translateText('player', 'Фамилия')}</span>
					<span>{surname}</span>
				</div>
							
				<div class="desc">
					<span>{translateText('player', 'Место жительства')}</span>
					<span>LOS SANTOS</span>
				</div>										
			</div>
			
			<div class="pl2">
				<div class="desc">
					<span>Identification Card NO.</span>
					<span>RA{pad(cardNO)}</span>
				</div>						
				<div class="img_b"></div>						
			</div>
			
			<div class="pl3">
				<div class="serialid">254567</div>
				<div class="imgid"></div>
			</div>						
		</div>
	</div>
</div>