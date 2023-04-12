<script>
	import Male from './../images/male.png'
	import Female from './../images/female.png'
	import Fib_logo from './images/fib_logo.png'

	export let gender;
	export let name;
</script>
<div class="card_leather" id="card">
	<div class="b1">
		<div class="card fib rounding5">
			<div class="content">
				<div class="title">
				FEDERAL INVESTIGATION BUREAU
				</div>						
				<div class="pl1">
					<img src={Fib_logo} class="img_lg" alt="fib" />							
					<div class="txt">
						<div>FIB</div>
						<div>FEDERAL INVESTIGATION BUREAU</div>
					</div>							
					<div class="ava" style="background-image: url({gender === "Мужской" ? Male : Female})"/>							
				</div>				
				<div class="pl2">
					<div class="t1">SPECIAL  AGENT</div>
					<div class="t2">{name}</div>
				</div>						
				<div class="pl3">
					<div class="t1">FEDERAL BUREAU OF INVESTIGATION UNITED STATES DEPARTMENT OF JUSTICE</div>
					<div class="t2"></div>
				</div>
			</div>
		</div>
	</div>
	<div class="b2"></div>
	<div class="b3">
		<div class="pl1 rounding5">
			<p>SUCH IS CHARGED WITH THE DUTY OF INVESTIGATING VIOLATIONS OF THE LAWS OF THE UNITED STATES. COLLECTING EVIDENCE IN CASE IN WHICH THE UNITED STATES IS OR MAY BE PARTY IN INTEREST AN PERFORMING OTHER DUTIES IMPOSED URON HIM BY LAW.</p>
			<div class="t2">{name}</div>
			<p>SPECIAL  AGENT</p>
			<p>THIS IS THE WARRANT AND AUTHORITY FOR EXECUTING THE DUTIES OF THEIR OFFICE.</p>
		</div>
		<div class="pl2"></div>
	</div>
</div>