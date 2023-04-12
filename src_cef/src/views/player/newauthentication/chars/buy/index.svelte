<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import { selectIndex } from './../store.js';

    const onBuy = () => {
        executeClient('buyNewSlot', $selectIndex);
    }

    const MouseUse = (toggled) => {
        executeClient ("client.camera.toggled", toggled);
    }
    const onKeyUp = (event) => {
		const { keyCode } = event;

        switch (keyCode) {
            case 13:
                onBuy ();
                break;
        }
    }
</script>

<svelte:window on:keyup={onKeyUp} />

<div class="auth__center">
    <div class="auth__buttons" style="justify-content: center;" on:mouseenter={() => MouseUse (false)} on:mouseleave={() => MouseUse (true)}>
        <div class="main__button main_button_size_large" style="margin-right: 0" on:click={onBuy}>
            <div class="main__button_left box-center">{translateText('player2', 'Разблокировать')}</div>
            <div class="main__button_right box-center">
                <div class="main__button_square box-center">
                    <span class="auth-arrow"/>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="auth__stats"></div>