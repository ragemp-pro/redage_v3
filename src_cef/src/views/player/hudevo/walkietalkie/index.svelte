<script>
    import './main.sass';
    import './fonts/style.css';
    import { executeClient } from 'api/rage';
    import { storeSettings } from 'store/settings'

    let SettingsList = {};
    storeSettings.subscribe((value) => {
        if (SettingsList !== value) {
            SettingsList = value;
        }
    });

    let channel_name = 'OFF';
    
    let faction_channel_value = -1;
    let channel_main_number = 0;
    let channel_secendory_number = 0;
    let frequency_selection_index = 0;

    let voiceToggleButton = 'FD';
    let isGovFactionPlayer = true;
    let previousVolume = 7;

    if (isGovFactionPlayer === true) {
        channel_name = '1CH';
    }

    let muteStatus = false;
    let playersData = [];

    let eventsRefresh = new Date().getTime();

    const changeWalkieTalkieVolume = (value) => {
        if (new Date().getTime() - eventsRefresh < 500) {
            window.notificationAdd(4, 9, 'Слишком быстро', 1000);
            return false;
        }
        
        eventsRefresh = new Date().getTime();
        let volume = Math.round(SettingsList.VolumeRadio / 10);

        volume += value;

        if (volume < 0) volume = 0;
        else if (volume > 10) volume = 10;
        
        SettingsList.VolumeRadio = volume * 10;
        executeClient("chatconfig", JSON.stringify (SettingsList));
        if (volume <= 0) {
            muteStatus = true;
        } else {
            muteStatus = false;
        }
        return true;
    };
    
    const muteWalkieTalkieVoice = () => {
        let volume = Math.round(SettingsList.VolumeRadio / 10);
        if (!previousVolume && !volume)
            return;

        if (volume && changeWalkieTalkieVolume (-10)) {
            previousVolume = volume;
        } else if (!volume && changeWalkieTalkieVolume (previousVolume)) {
            previousVolume = 0;
        }
    };

    const changeChannel = (value) => {
        switch (frequency_selection_index)
        {
            case 0:
                channel_secendory_number = value;
                break;
            case 1:
                channel_secendory_number = (channel_secendory_number * 10) + value;
                break;
            case 2:
                channel_secendory_number = (channel_secendory_number * 10) + value;
                break;
            case 3:
                channel_main_number = value;
                break;
            case 4:
                channel_main_number = (channel_main_number * 10) + value;
                break;
            case 5:
                if (((channel_main_number * 10) + value) > 999) {
                    return;
                }

                channel_main_number = (channel_main_number * 10) + value;
                break;
            default:
                return;
        }

        if (frequency_selection_index < 5) {
            frequency_selection_index += 1;
        }

        channel_name = `
            ${channel_main_number < 10 ? `00${channel_main_number}` : (channel_main_number < 100 ? `0${channel_main_number}` : `${channel_main_number}`)}.${channel_secendory_number < 10 ? `00${channel_secendory_number}` : (channel_secendory_number < 100 ? `0${channel_secendory_number}` : `${channel_secendory_number}`)}
        `;
    };

    let lastChanel = 0;
    const changeFrequency = () => {
        if (lastChanel === ((channel_main_number * 1000) + channel_secendory_number))
            return;
        if (new Date().getTime() - eventsRefresh < 1000) {
            window.notificationAdd(4, 9, 'Слишком быстро', 1500);
            return;
        }
        
        eventsRefresh = new Date().getTime();
        lastChanel = (channel_main_number * 1000) + channel_secendory_number;
        executeClient ('UpdateWalkieTalkieFrequency_client', lastChanel);
    };

    const dangerButton = () => {
        executeClient ('PressedDangerButton_client');
    };

    const swapChannel = (state) => {
        if (isGovFactionPlayer === false) {
			window.notificationAdd(4, 9, 'Общая волна доступна только для гос. фракций', 1500);
            return;
        }

        if (new Date().getTime() - eventsRefresh < 100) {
			window.notificationAdd(4, 9, 'Слишком быстро', 1500);
			return;
		}
		
		eventsRefresh = new Date().getTime();

        if (state == 1 && faction_channel_value > -5) {
            faction_channel_value -= 1;
        }
        else if (state == 2 && faction_channel_value < -1) {
            faction_channel_value += 1;
        }
        
        channel_name = `${Math.abs(faction_channel_value)}CH`;
        executeClient ('UpdateWalkieTalkieFrequency_client', faction_channel_value);
    };

    const cancelSettings = (isGos = false) => {
        if (isGos && isGovFactionPlayer === false) {
			window.notificationAdd(4, 9, 'Общая волна доступна только для гос. фракций', 1500);
            return;
        }
        if (new Date().getTime() - eventsRefresh < 1000) {
			window.notificationAdd(4, 9, 'Слишком быстро', 1500);
			return;
		}
		
		eventsRefresh = new Date().getTime();

        if (isGovFactionPlayer === true) {
            channel_name = '1CH';
        } else {
            channel_name = 'OFF';
        }

        faction_channel_value = -1;

        channel_main_number = 0;
        channel_secendory_number = 0;
        frequency_selection_index = 0;

        executeClient ('UpdateWalkieTalkieFrequency_client', -1);
    };

    const closeWalkieTalkie = () => {
        executeClient ('closeWalkieTalkieMenu');
    };

    const addPlayerInfo = (name, id) => {
        if (playersData.length >= 3) {
            playersData.splice(0, 1);
        }

        playersData.push({ id: id, name: name });
        playersData = playersData;
    };

    window.events.addEvent("cef.walkietalkie.addPlayerInfo", addPlayerInfo);

    const removePlayerInfo = (id) => {
        const index = playersData.findIndex(r => r.id == id);
        if (playersData[index]) {
            playersData.splice(index, 1);
            playersData = playersData;
        }
    };

    window.events.addEvent("cef.walkietalkie.removePlayerInfo", removePlayerInfo);

    const updateMainInfo = (ButtonName = "F", isGov = false, walkieTalkieFrequency = -1) => {
        voiceToggleButton = ButtonName;
        isGovFactionPlayer = isGov;
        //playersData = [];
        if (walkieTalkieFrequency === -1) {
            if (isGovFactionPlayer === true) {
                channel_name = '1CH';
            } else {
                channel_name = 'OFF';
            }
        }
    };

    window.events.addEvent("cef.walkietalkie.updateMainInfo", updateMainInfo);

    const updatePhoneRadioVolume = (volume) => {
        SettingsList.RadioVolume = volume;
        executeClient("chatconfig", JSON.stringify (SettingsList));
    };

    window.events.addEvent("cef.walkietalkie.updatePhoneRadioVolume", updatePhoneRadioVolume);
</script>

<div id="walkietalkie">
    <div class="walkietalkie__block">
        <div class="walkietalkie__danger" on:click={() => dangerButton()}></div>
        <div class="walkietalkie__volume">
            <div class="walkietalkie__volume_box" on:click={() => changeWalkieTalkieVolume(-1)}>-</div>
            <div class="walkietalkie__volume_count">{Math.round(SettingsList.VolumeRadio / 10)}</div>
            <div class="walkietalkie__volume_box" on:click={() => changeWalkieTalkieVolume(1)}>+</div>
        </div>
        <div class="walkietalkie__display">
            <div class="walkietalkie__display_top">
                <div class="walkietalkie__channel_name">{channel_name}</div>
                <div class="box-flex">
                    <span class="walkie-persons"/>
                    {#if muteStatus === true}
                        <span class="walkie-nosound"/>
                    {/if}
                </div>
                <span class="walkie-battery"/>
            </div>
            <div class="walkietalkie__display_bottom">
                {#each playersData as item, index}
                    <div>{item.name}</div>
                {/each}
            </div>
        </div>
        <div class="walkietalkie__center">
            <div class="walkietalkie__button red">
                <div class="walkitetalkie__button_small">
                    {voiceToggleButton}
                </div>
            </div>
            <div class="walkietalkie__button blue">
                <div class="walkitetalkie__button_small" on:click={() => muteWalkieTalkieVoice()}>
                    <span class="walkie-nosound"/>
                </div>
            </div>
        </div>
        <div class="walkietalkie__buttons_list">
            <div class="walkietalkie__list_element" on:click={() => cancelSettings(true)}>1CH</div>
            <div class="walkietalkie__list_element" on:click={() => swapChannel(1)}>↑</div>
            <div class="walkietalkie__list_element" on:click={() => swapChannel(2)}>↓</div>
            <div class="walkietalkie__list_element" on:click={() => closeWalkieTalkie()}>EXIT</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(1)}>1</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(2)}>2</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(3)}>3</div>
            <div class="walkietalkie__list_element" on:click={() => cancelSettings()}>CLR</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(4)}>4</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(5)}>5</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(6)}>6</div>
            <div class="walkietalkie__list_element" on:click={() => changeFrequency()}>ENT</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(7)}>7</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(8)}>8</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(9)}>9</div>
            <div class="walkietalkie__list_element" on:click={() => changeChannel(0)}>0</div>
        </div>
    </div>
</div>