<script>
    import { translateText } from 'lang'
    import { isPopupLvlOpened } from '../../stores.js'
    import { format } from 'api/formatter'
    import { accountRedbucks } from 'store/account'
    import { executeClientToGroup } from 'api/rage'

    const prices = [
        {
            name: translateText('player', '1 УРОВЕНЬ'),
            lvl: 1,
            priceRB: 1600
        },
        {
            name: translateText('player', '5 УРОВНЕЙ'),
            lvl: 5,
            priceRB: 6666
        },
        {
            name: translateText('player', '25 УРОВНЕЙ'),
            lvl: 25,
            priceRB: 29999
        },
    ]

    const onBuyLvl = (index) => {
        if (!window.loaderData.delay ("battlePass.onBuyLvl", 1))
            return;
        else if ($accountRedbucks < prices.priceRB)
            return window.notificationAdd(4, 9, translateText('player', 'Недостаточно Redbucks!'), 3000);

        executeClientToGroup ("buyLvl", index);

        onClose ();
    }

    const onClose = () => isPopupLvlOpened.set(false);
</script>

<div class="battlepass__popup_info popupback">
    <div class="newproject__buttonblock" on:click={onClose}>
        <div class="newproject__button">ESC</div>
        <div>{translateText('player', 'Закрыть')}</div>
    </div>
    <div class="battlepass__popup_title">{translateText('player', 'Покупка уровней')}</div>
    <div class="battlepass__buylevels_box">
        {#each prices as item, index}
        <div class="battlepass__buylevels_element">
            <div class="battlepass__buylevels_title" data-text={item.name}>{item.name}</div>
            <div class="battlepass__buylevels_content">
                <div class="battlepass__buylevels_price">{format("money", item.priceRB)} RB</div>
                <div class="battlepass__button" on:click={() => onBuyLvl (index)}>{translateText('player', 'Приобрести')} {item.lvl} {translateText('player', 'ур')}.</div>
            </div>
        </div>
        {/each}
    </div>
</div>