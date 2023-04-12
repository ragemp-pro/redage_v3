<script>
    import { translateText } from 'lang'
    import { format } from "api/formatter";

    import { executeClientAsync} from 'api/rage'

    let selectTaxi = {}

    const getCounter = () => {
        executeClientAsync("phone.taxi.getCounter").then((result) => {
            if (result && typeof result === "string") {
                selectTaxi = JSON.parse(result);
            }
        });
    }

    getCounter();
    import { addListernEvent } from 'api/functions';
    addListernEvent ("hud.taxi.updateCounter", getCounter);


</script>
{#if selectTaxi}
    <div class="hudevo__taxicount">
        <div class="hudevo__taxicount_title hudevo__elementparams">{translateText('player2', 'Счётчик включен')}</div>
        <div class="box-flex">
            <div class="hudevo__taxicount_thin">{translateText('player2', 'Стоимость')}:</div>
            <div class="hudevo__taxicount_cost">${format("money", selectTaxi.price)}</div>
        </div>
    </div>
{/if}