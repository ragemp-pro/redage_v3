<script>
    import { TimeFormat } from 'api/moment'
    import { serverDateTime } from 'store/server'
    import { isInputToggled, isWaterMark, isPlayer, isHelp, isPhone } from 'store/hud'
    import keys from 'store/keys'
    import keysName from 'json/keys.js'
    import { charFractionID, charOrganizationID } from 'store/chars'
    import CustomKey from './Key.svelte'

    export let SafeSone;

    let isWorld = true;
    window.hudStore.isWorld = (value) => isWorld = value;

    let direction = "NE";
    window.hudStore.direction = (value) => direction = value;

    let greenZone = false;
    window.hudStore.greenZone = (value) => greenZone = value;

    let street = "";
    window.hudStore.street = (value) => street = value;

    let area = "";
    window.hudStore.area = (value) => area = value;

    let micIcon = "hud__icon-micoff";

    let microphone = 0;
    window.hudStore.microphone = (value) => {
        microphone = value;
        UpdateMicrophoneIcon ();
    }

    let isMute = false;
    window.hudStore.isMute = (value) => {
        isMute = value;
        UpdateMicrophoneIcon ();
    }

    const UpdateMicrophoneIcon = () => {
        if (isMute) micIcon = "hud__icon-micmute";
        else if (microphone) micIcon = "hud__icon-micon";
        else micIcon = "hud__icon-micoff";
    }

    
    let polygon = 0;
    window.hudStore.polygon = (value) => polygon = value;

    let radio = 0;
    window.hudStore.radio = (value) => radio = value;

    
    let serverPlayerId = 0;
    window.serverStore.serverPlayerId = (value) => serverPlayerId = value;

    let serverOnline = 0;
    window.serverStore.serverOnline = (value) => serverOnline = value;

    const getOnlineName = (online) => {
        if(online <= 99)
            return "Низкий";
        else if(online <= 499)
            return "Средний";

        return "Высокий";
    }
</script>

<div class="hudevo__placetime">
    <div class="hudevo__placetime_image">
        <div class="hudevo__placetime_cirlce"><div class="hudevoicon-person"></div></div>
        <div class="hudevo__placetime_title">Онлайн</div>
        <div class="hudevo__placetime_subtitle">{serverOnline} игроков</div>
    </div>
    {#if isWorld}
    <div class="hudevo__placetime_image">
        <div class="hudevo__placetime_cirlce">{direction}</div>
        <div class="hudevo__placetime_title">{street}</div>
        <div class="hudevo__placetime_subtitle">{area}</div>
    </div>
    {/if}
    {#if $isPlayer}
    <div class="hudevo__placetime_image">
        <div class="hudevo__placetime_cirlce"><div class="hudevoicon-clock"></div></div>
        <div class="hudevo__placetime_title">{TimeFormat ($serverDateTime, "H:mm")}</div>
        <div class="hudevo__placetime_subtitle">{TimeFormat ($serverDateTime, "DD.MM.YYYY")}</div>
    </div>
    {/if}
    <div class="placetime__buttons">
        <div class="playerinfo__box">
            <CustomKey bottom={true} keyCode={$keys[36]} nonactive={isMute || $isInputToggled}>
                <span class="{micIcon} placetime__icon" class:active={microphone} class:polygon={polygon} class:mute={isMute} />
                <div class="playerinfo__box_key">{keysName[$keys[36]]}</div>
            </CustomKey>
        </div>
        {#if $charFractionID > 0 || $charOrganizationID > 0}
            <div class="playerinfo__box">
                <CustomKey bottom={true} keyCode={$keys[50]} nonactive={isMute || $isInputToggled}>
                    <span class="hudevoicon-radio placetime__icon" class:active={radio} class:polygon={polygon} class:mute={isMute} />
                    <div class="playerinfo__box_key">{keysName[$keys[50]]}</div>
                </CustomKey>
            </div>
        {/if}
        {#if $isHelp}
            <div class="playerinfo__box">
                <CustomKey bottom={true} keyCode={$keys[30]} nonactive={isMute || $isInputToggled}>
                    <span class="hud__icon-phone placetime__icon" class:active={$isPhone} />
                    <div class="playerinfo__box_key">{keysName[$keys[30]]}</div>
                </CustomKey>
            </div>
            <div class="playerinfo__box">
                <CustomKey bottom={true} keyCode={$keys[12]} nonactive={isMute || $isInputToggled}>
                    <span class="hud__icon-inventory placetime__icon"></span>
                    <div class="playerinfo__box_key">{keysName[$keys[12]]}</div>
                </CustomKey>
            </div>
            <div class="playerinfo__box">
                <CustomKey bottom={true} keyCode={$keys[21]} nonactive={isMute || $isInputToggled} keyDonate={true}>
                    <span class="hud__icon-money placetime__icon"></span>
                    <div class="playerinfo__box_key">{keysName[$keys[21]]}</div>
                </CustomKey>
            </div>
        {/if}
    </div>
    {#if greenZone}
        <div class="hudevo__greenzone" style="margin-bottom: -{SafeSone.y}px">
            <div class="hudevoicon-shield"></div>
            <div class="hudevo__greenzone_text"><b>Зеленая</b> зона</div>
        </div>
    {/if}
</div>