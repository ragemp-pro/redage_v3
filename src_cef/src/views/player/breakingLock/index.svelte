<script>
    import { executeClient } from 'api/rage'
    export let viewData;

    import './assets/css/index.css'

    let rot = 0,
        value = viewData.value,
        off = viewData.off,
        shakeClass = "";

    const tick = () => {
        if(rot % 2) return;
        executeClient ("sounds.playInterface", "breakingLock/tick",  0.6)
    }

    const set = (value) => {

        switch(value){
            case 'minus':
                if(rot <= 0) {
                    return; // Надо с пашей решить, добавляем ли полный круг или нет
                    rot = 360;
                }
                tick(); 
            
                rot = rot - 1
                break;
            case 'plus':
                if(rot >= 360) {
                    return;
                    rot = 0
                }
                tick(); 
                rot = rot + 1
                break;
        }
    }

    const shake = () => {
        var random = Math.floor(Math.random() * (4 - 2)) + 2;
        if(rot == value)
        {
            shakeClass = 'shake3';
        }
        else if ((value - off) <= rot && rot <= (value + off))
        {
            shakeClass = 'shake2';
        }
        else if ((value - off - random) <= rot && rot <= (value + off + random))
        {
            shakeClass ='shake1';
        }
        else 
        {
            shakeClass = '';
        }
    }

    const send = () => {

        tick();

        if(rot == value) {
            executeClient ("dial", 'call', true);
            hide();
        } else {
            executeClient ("dial", 'call', false);
        }
    }

    const handleKeyDown = (event) => {
        switch(event.which) {
            case 37: case 38:
                set('minus')
                break;
            case 39: case 40:
                set('plus')
                break;
            case 32: case 13:
                send();
                break;
            default: 
                break;
        }
        shake();
    }

</script>

<svelte:window on:keydown={handleKeyDown} />
<div class="dial {shakeClass}">
    <div class="img base"></div>
    <div class="img num" style="transform: rotate({-rot}deg)"></div>
    <div class="img center"></div>
</div>