let renderTarget = null;

export const SCREEN_DIAMONDS = "CASINO_DIA_PL";
export const SCREEN_SKULLS = "CASINO_HLW_PL";
export const SCREEN_SNOW = "CASINO_SNWFLK_PL";
export const SCREEN_WIN = "CASINO_WIN_PL";

const targetName = "casinoscreen_01";
const targetModel = mp.game.joaat('vw_vwint01_video_overlay');
const textureDict = "Prop_Screen_Vinewood";
const textureName = "BG_Wall_Colour_4x4";

let isLoad = false;

const SetScreenType = (screen, chanel = 0, volume = -100) => {
    Natives.SET_TV_CHANNEL_PLAYLIST(0, screen, true);
    mp.game.graphics.setTvAudioFrontend(true);
    mp.game.graphics.setTvVolume(volume);
    mp.game.graphics.setTvChannel(chanel);
}

const LoadTexture = () => {
    try {        
        global.loadTextureDict (textureDict).then(
            response => {
                if (response && mp.game.graphics.hasStreamedTextureDictLoaded(textureDict)) {
                    mp.game.ui.registerNamedRendertarget(targetName, false);
                    mp.game.ui.linkNamedRendertarget(targetModel);
            
                    SetScreenType (SCREEN_SNOW)
            
                    renderTarget = mp.game.ui.getNamedRendertargetRenderId(targetName);
                } else {                    
                    LoadTexture ();
                }
            },
            error => {
                LoadTexture ();
            }
        );
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "casino/casinoscreen", "setInterval", e.toString());
    }
}

gm.events.add("render", () => {
    if (!global.loggedin) return;
    if (!isLoad && Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) == 275201) {
        isLoad = true;
        LoadTexture ();
    } else if (isLoad && Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) == 275201 && mp.game.graphics.hasStreamedTextureDictLoaded(textureDict) && renderTarget !== null) {
        mp.game.ui.setTextRenderId(renderTarget);
        mp.game.graphics.set2dLayer (4);
        Natives.SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU (true);
        Natives._DRAW_INTERACTIVE_SPRITE (textureDict, textureName, 0.25, 0.5, 0.5, 1.0000001, 0.0000001, 255, 255, 255, 255);
        mp.game.graphics.drawTvChannel(0.5, 0.5, 1.0000001, 1.0000001, 0.0000001, 255, 255, 255, 255);
        mp.game.ui.setTextRenderId(1);
    }
})