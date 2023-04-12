<script>
    import { translateText } from 'lang'
    import './main.sass'
    import { format } from "api/formatter";
    import { executeClient } from 'api/rage'
    import { executeClientToGroup, executeClientAsyncToGroup } from "api/rage";

    executeClientToGroup('getUpgrate')

    let upgrate = [];
    const getUpgrate = (result) => {
        if (result && typeof result === "string")
            upgrate = JSON.parse(result);

        onClose ();
    }

    import { addListernEvent } from "api/functions";
    addListernEvent ("table.upgrate", getUpgrate)

    let selectItem = false;

    const onSelectItem = (item) => {

        if (item.type === "newleader") 
            return setPopup ("NewLeaderPopup")

        selectItem = item;
    }

    const onBuy = () => {
        executeClientToGroup('buyUpgrate', selectItem.type)
        onClose ();
    }

    const onClose = () => selectItem = false;
    import { setPopup } from "../../data";

</script>


{#if selectItem}
    <div class="popup__newhud">
        <div class="popup__newhud_box">
            <div class="popup__newhud_title">
                <span class="popup__newhud_icon hud__icon-inventory"/> {translateText('popups', 'Улучшения')}
            </div>
            <div class="popup__newhud_text">Вы действительно хотите купить '{selectItem.name}' за {!selectItem.isRb ? `$${format("money", selectItem.price)}` : `${format("money", selectItem.price)} RB`}?</div>

            <div class="popup__newhud__buttons">
                <div class="popup__newhud_button" on:click={onBuy}>{translateText('popups', 'Купить')}</div>
                <div class="popup__newhud_button" on:click={onClose}>{translateText('popups', 'Отмена')}</div>
            </div>
        </div>
    </div>
{:else}

    <div id="popup__newhud_upgrade">
        <div class="popup__newhud_box">
            <div class="popup__newhud_title">
                <span class="hud__icon-inventory popup__newhud_icon"/> {translateText('popups', 'Улучшения')}
            </div>
            <div class="popup__newhud_grid">
                {#each upgrate as item, index}
                    <div class="popup__grid_element">
                        <div class="popup__grid_image">
                            {#if item.icon}
                                <div class="{item.icon} gridimageicon"></div>
                            {/if}
                            <div>{item.name}</div>
                        </div>
                        <div class="box-between w-100">
                            {#if !item.isRb}
                                <div class="gray">${format("money", item.price)}</div>
                            {:else}
                                <div class="gray">{format("money", item.price)} RB</div>
                            {/if}
                            <div class="popup__grid_button" on:click={() => onSelectItem (item)}>{translateText('popups', 'Купить')}</div>
                        </div>
                    </div>
                {/each}
            </div>
            <div class="popup__newhud__buttons">
                <div class="popup__newhud_button" on:click={() => setPopup ()}>{translateText('popups', 'Закрыть')}</div>
            </div>
        </div>
    </div>
{/if}