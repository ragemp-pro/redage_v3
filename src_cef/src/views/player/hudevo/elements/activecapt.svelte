<script>
    import { fly } from 'svelte/transition';

    import { addListernEvent } from "api/functions";
    
    let 
        visible = false,
        warType,
        warGripType,
        weaponType,
        title, 
        time, 
        attackName, 
        protectingName, 
        attackingCount, 
        protectingCount,
        attackingPlayersInZone, 
        protectingPlayersInZone;

    addListernEvent ("hud.war.open", (_warType, _warGripType, _weaponType, _title, _attackName, _protectingName) => {
        weaponType = _weaponType;
        warType = _warType;
        warGripType = _warGripType;
        title = _title;
        attackName = _attackName;
        protectingName = _protectingName;
        visible = true;
    });

    addListernEvent ("hud.war.update", (type, value1, value2) => {
        if (type === "count") {
            attackingCount = value1;
            protectingCount = value2;
        }

        if (type === "playersCount") {
            attackingPlayersInZone = value1;
            protectingPlayersInZone = value2;
        }

        if (type === "time") {
            time = value1;
        }
    });

    addListernEvent ("hud.war.close", () => {
        visible = false;
        attackingPlayersInZone = 0;
        protectingPlayersInZone = 0;
        attackingCount = 0;
        protectingCount = 0;
    });

    import { typeBattle, weaponsCategory } from '@/popups/war/data'

    const getProgress = (value, max) => {
        if (!max)
            return 50;
            
        return 100 / max * value;
    }

</script>

{#if visible}
<div class="hudevo__activecapt" in:fly={{ y: -50, duration: 500 }} out:fly={{ y: -50, duration: 250 }}>
    <div class="hudevo__activecapt_image">{title}</div>
    <div class="box-between">
        <div class="box-flex">
            {#if warGripType === 0 || warGripType === 3}
                <div class="hudevoicon-skull mr"></div>
            {:else}
                <div class="hudevoicon-star mr"></div>
            {/if}
            <div>{protectingCount}</div>
        </div>
        <div class="box-flex">
            {time}
        </div>
        <div class="box-flex">
            <div>{attackingCount}</div>
            {#if warGripType === 0 || warGripType === 3}
                <div class="hudevoicon-skull ml"></div>
            {:else}
                <div class="hudevoicon-star ml"></div>
            {/if}
        </div>
    </div>
    <div class="box-between mt-14">
        <div class="hudevo__activecapt_square green"></div>
        <div class="hudevo__activecapt_line">
            <div class="hudevo__activecapt_left" style="width: {getProgress (protectingCount, (attackingCount + protectingCount))}%"></div>
            <!--<div class="hudevo__activecapt_center"></div>-->
            <div class="hudevo__activecapt_right" style="width: {getProgress (attackingCount, (attackingCount + protectingCount))}%"></div>
        </div>
        <div class="hudevo__activecapt_square red"></div>
    </div>
    <div class="box-between mt-14">
        <div class="activewidth l">{protectingName}</div>
        <div class="fs-10">{typeBattle[warGripType]}, {weaponsCategory[weaponType]}</div>
        <div class="activewidth r">{attackName}</div>
    </div>
    <div class="box-between mt-5">
        <div class="box-flex">
            <div class="hudevoicon-war-person mr"></div>
            <div class="activewidth l">{protectingPlayersInZone}</div>
        </div>
        <!--<div class="box-flex">
            <div class="hudevoicon-war-person mr"></div>
            <div>23</div>
        </div>-->
        <div class="box-flex">
            <div class="activewidth r">{attackingPlayersInZone}</div>
            <div class="hudevoicon-war-person ml"></div>
        </div>
    </div>
</div>
{/if}