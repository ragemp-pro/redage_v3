<script>
    import { translateText } from 'lang'
    import './main.sass'
    import './fonts/style.css'
    import InputCustom from 'components/input/oneInput.svelte'
    import { isInput } from '@/views/player/newauthentication/store.js';
    import { executeClient } from 'api/rage'
	import router from 'router';

    export let viewData;

    if (!viewData)
        viewData = {
            number: "228XUI",
            model: "Bugatti",
            holder: "Vitaliy_Zdobich"
        }

    if (viewData && typeof viewData === "string")
        viewData = JSON.parse (viewData)

    let ticketText;
    let ticketPrice;
    let isEvac = false;


    
    const onKeyUp = (event) => {
        if (!$router.opacity)
            return;
        if ($isInput)
            return;
        const { keyCode } = event;

        const isCamera = $router.popup === "PopupCamera";

        if(keyCode == 27 && !isCamera) {
            onExit ()
        }

        if (keyCode == 89 && !isCamera)
            updateCameraToggled ();

        if (keyCode == 13 && !isCamera)
            onEnter ()
    }

    let cameraMode = false
    const onCameraMode = () => {
        cameraMode = !cameraMode
        executeClient ("camera.freemode", cameraMode)
    }

    const onEnter = () => {
        let price = Number (ticketPrice)
        if (typeof price !== "number" || price < 100 || price > 500) {
           return window.notificationAdd(4, 9, translateText('fractions', 'Вы указали неверное значение. Нужна сумма от 100$ до 500$'), 5000);
        }
        if (typeof ticketText !== "string" || ticketText.length < 1 || ticketText.length >= 45) {
           return window.notificationAdd(4, 9, translateText('fractions', 'Вы ошиблись в описании нарушения. Минимум 1 символ, максимум 45 символов.'), 3000);
        }
        if (typeof cameraLink !== "string" || !cameraLink) {
           return window.notificationAdd(4, 9, translateText('fractions', 'Вы не сделали фотографию'), 3000);
        }

        executeClient ("client.ticket.end", ticketText, price, cameraLink, isEvac)
        onExit ();
    }

    const updateCameraToggled = () => {
        if (cameraLink == true) {
           return window.notificationAdd(4, 9, translateText('fractions', 'Фотография загружается'), 3000);
        }
        //return window.notificationAdd(4, 9, "Временно не работает", 3000);
        window.router.setPopUp("PopupCamera", updateCameraLink)
        executeClient ("camera.open", false)
    }

    const onExit = () => {
        executeClient ("client.ticket.close")
    }

    let cameraLink = false;
    const updateCameraLink = (link) => {
        cameraLink = link;
    }

    import { addListernEvent } from 'api/functions';
    addListernEvent ("cameraLink", updateCameraLink)

</script>

<svelte:window on:keyup={onKeyUp} />

{#if $router.opacity}
    {#if !($router.popup === "PopupCamera")}
        <div id="ticket">
            <div class="ticket__title">{translateText('fractions', 'Оформление штрафа')}</div>
            <div class="ticket__number">{viewData.number}</div>
            <div class="ticket__line"></div>
            <div class="ticket__title">{translateText('fractions', 'Информация')}</div>
            <div class="box-between">
                <div class="gray">{translateText('fractions', 'Владелец')}:</div>
                <div class="ticket__line"></div>            
                <div class="ticket__name">{viewData.holder}</div>
            </div>
            <div class="box-between">
                <div class="gray">{translateText('fractions', 'Модель машины')}:</div>
                <div class="ticket__line"></div>
                <div class="ticket__name">{viewData.model}</div>
            </div>
            <div class="ticket__subtitle">{translateText('fractions', 'Нарушение')}</div>
            <InputCustom cl="ticket__input" setValue={(value) => ticketText = value} value={ticketText} placeholder={translateText('fractions', 'Описание нарушения')} type="text" />
            <div class="ticket__subtitle">{translateText('fractions', 'Сумма штрафа')}</div>
            <InputCustom cl="ticket__input" setValue={(value) => ticketPrice = value} value={ticketPrice} placeholder={translateText('fractions', 'Введите сумму от 100 до 500$')} type="number" />
            <div class="sound__input-block switch-box" on:click={() => isEvac = !isEvac}>
                <label class="switch">
                    <input type="checkbox" checked={isEvac} disabled >
                    <span class="slider round"></span>
                </label>
                {translateText('fractions', 'Поместить на эвакуатор')}
            </div>
            <div class="box-flex">
                {#if cameraLink && typeof cameraLink === "string"}
                <div class="ticket__small_image" style="background-image: url({cameraLink})"></div>
                <div class="box-column">
                    <div class="ticket__subtitle">{translateText('fractions', 'Фотография')}</div>
                    <div class="gray">{translateText('fractions', 'Нарушение зафиксировано')}</div>
                </div>
                {:else}
                <div class="ticket__small_image" on:click={updateCameraToggled}/>
                <div class="box-column">
                    <div class="ticket__subtitle">{translateText('fractions', 'Фотография')}</div>
                    <div class="gray">{cameraLink == true ? translateText('fractions', 'Фотография загружается') : translateText('fractions', 'Нет фотографии')}</div>
                </div>
                {/if}
            </div>
            <div class="ticket__buttons">
                <div class="ticket_bottom_buttons center" on:click={updateCameraToggled}>
                    <div class="ticket_bottom_button">Y</div>
                    <div>{translateText('fractions', 'Сделать фотографию нарушения')}</div>
                </div>
                <div class="ticket_bottom_buttons center" on:click={onEnter}>
                    <div class="ticket_bottom_button">ENTER</div>
                    <div>{translateText('fractions', 'Выписать штраф')}</div>
                </div>
                <div class="ticket_bottom_buttons esc" on:click={onExit}>
                    <div>{translateText('fractions', 'Выйти')}</div>
                    <div class="ticket_bottom_button">ESC</div>
                </div>
            </div>
        </div>
    {/if}
{/if}