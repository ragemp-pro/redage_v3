<script>
    import { itemsInfo } from 'json/itemsInfo.js'
    import { serverDonatMultiplier } from 'store/server'
    import { charWanted, charMoney, charBankMoney } from 'store/chars'
    import { isWaterMark, isPlayer } from 'store/hud'
    import { fly } from 'svelte/transition';
    import { format } from 'api/formatter'
    import CountUp from 'api/countup';
    export let SafeSone;
    
    let userData = {
        targetMoney: 0,
        changeMoney: 0,
        timerIdMoney: 0,
        Money: 0,
        targetBank: 0,
        changeBank: 0,
        timerIdBank: 0,
        Bank: 0,
    };

    import { onMount } from 'svelte';
    onMount(async () => {
        //bar.animate(1.0);

        charMoney.subscribe(value => {
            if (userData.Money !== value) {
                CounterUpdate ("Money", value);
            }
        });
        charBankMoney.subscribe(value => {
            if (userData.Bank !== value) {
                CounterUpdate ("Bank", value);
            }
        });
    });

    const CounterUpdate = (args, value) => {
        if (userData["timerId" + args])
            clearTimeout (userData["timerId" + args]);
        userData["change" + args] = userData[args] > value ? (0 - (userData[args] - value)) : (value - userData[args]);
        userData[args] = value;
        userData["timerId" + args] = setTimeout (() => {
            userData["timerId" + args] = 0;
            userData["change" + args] = 0;
            if (!userData["target" + args]) {
                userData["target" + args] = new CountUp("target" + args, value);
                //userData["target" + args].start();
                //userData["target" + args].update(value);
            }
            else
                userData["target" + args].update(value);
        }, !userData["target" + args] ? 0 : 5000)
    }

    let serverName = "";
    window.setServerName = (name) => serverName = name;

    let isRotate = false;
    
    const secretFunction = () => {
        isRotate = !isRotate;

    }

    let serverPlayerId = 0;
    window.serverStore.serverPlayerId = (value) => serverPlayerId = value;

    let weaponItemId = 0;
    window.hudStore.weaponItemId = (value) => weaponItemId = value;

    let clipSize = 0;
    window.hudStore.clipSize = (value) => clipSize = value;

    let ammo = 0;
    window.hudStore.ammo = (value) => ammo = value;

    let isShow = false;

    serverDonatMultiplier.subscribe(value => {
        if (value > 1) {
            isShow = true;

            setTimeout(() => {
                isShow = false;
            }, 1000 * 30);
        }
    });

</script>
<div class="hudevo__playerinfo">
    <div class="box-flex mb-5">
        <div class="box-column align-end">
            <div class="hudevo__playerinfo_link">RedAge.net</div>
            <div class="box-flex">
                <div class="hudevo__playerinfo_online hudevo__elementparams paramsright">
                    <span class="hudevoicon-person"></span>#{serverPlayerId}
                </div>
                <div class="hudevo__playerinfo_name">{serverName}</div>
            </div>
        </div>
        <div class="hudevo__playerinfo_logo {isRotate ? "transform" : ""}" on:click={()=> secretFunction()}>
            <div class="hudevo__playerinfo_icon"></div>
        </div>
    </div>
    <div class="box-flex mb-5">
        <div class="hudevo__playerinfo_money hudevo__elementparams paramsright">
            <div class="box-flex" class:hudevo__playerinfo_hide={userData["changeMoney"] !== 0}>$ <div id="targetMoney">0</div></div>

            {#if userData["changeMoney"] !== 0}
                <div in:fly={{ x: (userData["changeMoney"] > 0 ? -5 : 5), duration: 250 }}
                    style={`color:${userData["changeMoney"] > 0 ? "#c1ff3d" : "#fe5b3b"}`}>{userData["changeMoney"] > 0 ? "+" : "-"}{format("money", userData["changeMoney"])}</div>
            {/if}
        </div>

        <div class="hudevo__playerinfo_icon"><div class="hudevoicon-wallet"></div></div>
    </div>
    <div class="box-flex mb-5">

        <div class="hudevo__playerinfo_money hudevo__elementparams paramsright">
            <div class="box-flex" class:hudevo__playerinfo_hide={userData["changeBank"] !== 0}>$ <div id="targetBank">0</div></div>

            {#if userData["changeBank"] !== 0}
                <div in:fly={{ x: 5, duration: 250 }}
                    style={`color:${userData["changeBank"] > 0 ? "#c1ff3d" : "#fe5b3b"}`}>{userData["changeBank"] > 0 ? "+" : "-"}{format("money", userData["changeBank"])}</div>
            {/if}
        </div>

        <div class="hudevo__playerinfo_icon"><div class="hudevoicon-bank"></div></div>
    </div>
    <div class="hudevo__weaponbox">
        <div class="hudevo__playerinfo_stars mb-5" class:newhud__hide={!$isPlayer}>
            {#if $charWanted > 0}
                {#each new Array(6) as e, i}
                    <div class="hudevoicon-star" in:fly={{ y: 10, duration: 50 * i }} class:active={i < $charWanted}></div>
                {/each}
            {/if}
        </div>
        {#if ammo > 0}
            <div class="hudevo__playerinfo_weapon hudevo__elementparams paramsright mb-5">
                <div class="hudevo__weapon_image" style="background-image: url('{document.cloud}inventoryItems/items/{weaponItemId}.png')"></div>
            </div>
            <div class="box-flex mb-5" class:newhud__hide={!$isPlayer}>
                {#if ammo > 0}
                    <div class="hudevo__playerinfo_white"><div class="hudevoicon-ammo"></div></div>
                    <div class="hudevo__playerinfo_ammo">{ammo}{#if clipSize > 0 && clipSize < 1000}/{clipSize}{/if}</div>
                {/if}
                <div class="hudevo__playerinfo_red">{itemsInfo[weaponItemId].Name}</div>
            </div>
        {/if}
    </div>
    {#if isShow}
        <div class="hudevo__playerinfo_donate" style="margin-right: -{SafeSone.x}px" in:fly={{ x: -50, duration: 500 }} out:fly={{ x: 50, duration: 250 }}></div>
    {/if}
</div>