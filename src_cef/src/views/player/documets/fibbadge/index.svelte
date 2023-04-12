<script>
	import Male from './images/ava.png'
	import Female from './../images/female.png'
	import Fib_logo from './images/fib_logo.png'

	export let gender;
	export let name;
</script>
<div class="card_leather" id="card">
	<div class="b1 badge">
		<div class="card badge fib rounding5">
			<div class="content">
				<div class="title">
				FEDERAL INVESTIGATION BUREAU
				</div>						
				<div class="pl1">
								
					<div class="txt">
						<div>FIB</div>
					</div>							
								
				</div>		
				<div class="ava" style="background-image: url({gender === "Мужской" ? Male : Female})"/>			
				<div class="pl2">
					<div class="t1">SPECIAL  AGENT</div>
					<div class="t2">{name}</div>
				</div>						
				<img src={Fib_logo} class="img_lg" alt="fib" />		
			</div>
		</div>
	</div>

</div>