<script>
    import { translateText } from 'lang'
    import { accountUnique } from 'store/account'
    import { format } from 'api/formatter'
    export let SetView;
    export let SetPopup;


    let ItemList = {
        packages: false,
        cases: false
    };


    executeClientAsync("donate.getPack").then((result) => {
        if (result && typeof result === "string") {
            ItemList.packages = JSON.parse(result);
            getUnique ();
        }
    });

    executeClientAsync("donate.roulette.getList").then((result) => {
        if (result && typeof result === "string") {
            ItemList.cases = JSON.parse(result).caseData;
            getUnique ();
        }
    });

    let selectListId = {};
    let isBuy = false;

    const onSelectMenu = () => {
        getUnique ();
        if (isBuy)
            return window.notificationAdd(4, 9, `Вы уже использовали данное предложение`, 3000);
        let getList = $accountUnique.split("_")[0];
        if (getList === "cases") SetView("Cases")
        else SetPopup ("PopupDpPopup", selectListId);
    }

    import {executeClientAsync} from "api/rage";

    const getUnique = () => {
        if ($accountUnique) {
            let getData = $accountUnique.split("_");
            if (ItemList[getData[0]][getData[1]]) {
                selectListId = ItemList[getData[0]][getData[1]];
                isBuy = Number (getData[2]);
            } else
                isBuy = true;
        } else
            isBuy = true; 
    }

</script>

{#if ItemList.cases && ItemList.packages}
<div class="main-menu__header-element" class:isBuy={isBuy}>
    <div class="main-menu__star-block">
        {#if $accountUnique.split("_")[0] === "cases"}
        <div class="star-img" style="background-image: url({document.cloud + `img/roulette/${selectListId.image}.png`})"/>
        {:else}
        <div class="star-img" style="background-image: url({document.cloud + `donate/personal/${selectListId.id + 1}.png`})"/>
        {/if}
    </div>
    <div class="main-menu__element-info">
        <div class="main-menu__timer-box">
            <div class="main-menu__timer box-center">{translateText('donate', 'Предложение дня')} (-30%)</div>
        </div>
        <div class="main-menu__title">{selectListId.name}</div>
        {#if selectListId.list}
        <div class="main-menu__paragraph">
        {#each selectListId.list as text, index}
            <b>- </b>{text}<br />
        {/each} 
        </div>
        {:else if selectListId.desc}
        <div class="main-menu__paragraph">{selectListId.desc}</div>
        {/if}
        <div class="newdonate__button_small" on:click={onSelectMenu}>
            <div class="newdonate__button-text">{translateText('donate', 'Купить за')} {format("money", Math.round (selectListId.price * 0.7))} RB</div>
        </div>
    </div>
</div>
{/if}