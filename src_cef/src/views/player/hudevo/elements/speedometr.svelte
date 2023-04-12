<script>
    import { translateText } from 'lang'

    import { fly } from 'svelte/transition';
    import { inVehicle, isInputToggled } from 'store/hud'
    import keys from 'store/keys'
    import keysName from 'json/keys.js'

    let speed = 10;
    let maxSpeed = 100;
    let fuel = 0;

    let speedDeg = 0;
    let fuelDeg = 0;

    let speedometerTest;
    let fuelTest;

    const GetCircle = (element, r, maxWidth = 160) => {
        if (!element) return r;
        element = element.getBoundingClientRect();
        return r / 100 * (element.width * 100 / maxWidth);
    }

    let FuelProcent = 0;
    const maxFuel = 500;



    window.vehicleState = {}

    window.vehicleState.speed = (value) => {
        speed = value;
        //speedDeg = !(speed > maxSpeed) ? (speed * 185 / maxSpeed) : (maxSpeed * 185 / maxSpeed);
    }

    window.vehicleState.maxSpeed = (value) => {
        maxSpeed = value * 1.15;
        //speedDeg = !(speed > maxSpeed) ? (speed * 185 / maxSpeed) : (maxSpeed * 185 / maxSpeed);
    }

    window.vehicleState.fuel = (value) => {
        fuel = value;
        //fuelDeg = !(fuel > maxFuel) ? (fuel * 62 / maxFuel) : (maxFuel * 62 / maxFuel);
        FuelProcent = value;
    }

    // Simple color interpolator for Speed.
    const InterpolateColor = (start, end, steps, count) => {
        var
            s = start,
            e = end,
            final = s + (((e - s) / steps) * count);
        return Math.floor(final);
    }

    const Color = (_r, _g, _b) => {
        return {
            r: _r,
            g: _g,
            b: _b
        };
    }

    const getSpeedColor = (value, max) => {
        let speed_percent = Math.floor(value / max * 100);
        if(speed_percent > 100) speed_percent = 100;

        let
            start = Color(255, 255, 255),
            end = Color(225, 228, 66);
    
        if (speed_percent > 50) {
            speed_percent = speed_percent % 51;
            start = Color(225, 228, 66);
            end = Color(228, 66, 66);
        }

        const start_colors = start;
        const end_colors = end;
        
        const 
            r = InterpolateColor(start_colors.r, end_colors.r, 50, speed_percent),
            g = InterpolateColor(start_colors.g, end_colors.g, 50, speed_percent),
            b = InterpolateColor(start_colors.b, end_colors.b, 50, speed_percent);

        return `rgb(${r}, ${g}, ${b})`;          
    }

    let autoPilot = false;
    window.vehicleState.autoPilot = (value) => autoPilot = value;

    let belt = false;
    window.vehicleState.belt = (value) => belt = value;

    let cruiseControl = false;
    window.vehicleState.cruiseControl = (value) => cruiseControl = value;

    let rightIL = false;
    window.vehicleState.rightIL = (value) => rightIL = value;

    let leftIL = false;
    window.vehicleState.leftIL = (value) => leftIL = value;

    let doors = false;
    window.vehicleState.doors = (value) => doors = value;

    let engine = false;
    window.vehicleState.engine = (value) => engine = value;

    let isToggledVehicleHud = true;
    window.vehicleState.isToggledVehicleHud = (value) => isToggledVehicleHud = value;


</script>

{#if $inVehicle && isToggledVehicleHud}
    <div class="hudevo__speedometr"  in:fly={{ x: 50, duration: 500 }} out:fly={{ x: 50, duration: 250 }}>
        <div class="hudevo__speedometr_bottom">
            <div class="box-flex">
                <span class="hudevoicon-fuel"></span>
                {FuelProcent}
            </div>
        </div>
        <div class="box-column">
            <div class="box-between">
                <div class="hudevo__speedometr_title" style="color: {engine ? getSpeedColor (speed, maxSpeed) : 'rgba(255, 255, 255, 0.3)'};">{speed}</div>
                <div class="box-column">
                    <div class="box-flex">
                        <div class="hudevoicon-left" class:active={leftIL}></div>
                        <div class="hudevoicon-right" class:active={rightIL}></div>
                    </div>
                    <div class="hudevo__speedometr_gray">{translateText('player2', 'км/ч')}</div>
                </div>
                <div class="box-column">
                    <div class="hudevoicon-engine hudevo__speedometr_icon" class:active={engine}></div>
                    <div class="hudevo__speedometr_text">{keysName[$keys[38]]}</div>
                </div>
                <div class="box-column">
                    <div class="hud__icon-cruisecontrol hudevo__speedometr_icon"></div>
                    <div class="hudevo__speedometr_text">{keysName[$keys[28]]}</div>
                </div>
                <div class="box-column">
                    <div class="hudevoicon-key hudevo__speedometr_icon" class:active={!doors}></div>
                    <div class="hudevo__speedometr_text">{keysName[$keys[39]]}</div>
                </div>
                <div class="box-column">
                    <div class="hudevoicon-bel hudevo__speedometr_icon" class:active={belt}></div>
                    <div class="hudevo__speedometr_text">G</div>
                </div>
            </div>
            <div class="hudevo__speedometr_fuel">
                <div class="filled" style="width: {FuelProcent}%"></div>
            </div>
        </div>
    </div>
{/if}