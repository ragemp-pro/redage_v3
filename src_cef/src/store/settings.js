import { writable } from 'svelte/store';

window.settingsStore = {};

export const storeSettings = writable({
    Timestamp: false, // 0
    ChatShadow: true,
    Fontsize: 16, // 16
    Chatalpha: 100, // 0
    Pagesize: 10, // 10
    Widthsize: 50, // 50
    Transition: 0, // 0
    WalkStyle: 0, // 0
    FacialEmotion: 0, // 0
    Deaf: false, // 0
    TagsHead: true, // 0
    HudToggled: true, // 0
    HudStats: true, // 0
    HudSpeed: true, // 0
    HudOnline: true, // 0
    HudLocation: true, // 0
    HudKey: true, // 0
    HudMap: true, // 0
    HudCompass: true,
    VolumeInterface: 100, // 100
    VolumeQuest: 50, // 100
    VolumeAmbient: 80, // 100
    VolumePhoneRadio: 50, // 100
    VolumeVoice: 100, // 100
    VolumeRadio: 70,
    VolumeBoombox: 70,
    FirstMute: false, // 0
    DistancePlayer: 50, // 100
    DistanceVehicle: 50, // 100

    //Прицел
    cPToggled: false,
    cPWidth: 2,
    cPGap: 2,
    cPDot: true,
    cPThickness: 0,
    cPColorR: 255,
    cPColorG: 255,
    cPColorB: 255,
    cPOpacity: 9,
    cPCheck: true,
    
    APunishments: false,

    CircleVehicle: false,
    
    //Эффекты
    cEfValue: 0,

    notifCount: 2,

    hitPoint: false,
});

window.settingsStore.init = (json) => {
    storeSettings.set (JSON.parse (json));
}