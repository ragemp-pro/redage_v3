const boomboxInfo = {
    startedBoomboxMusic: false,
    isBoomboxOwner: false,
    boomboxCoords: undefined
};

gm.events.add("setBoomboxInfo", (_isOwner, _coords = undefined) => {
	try
	{
        boomboxInfo.isBoomboxOwner = _isOwner;
        boomboxInfo.boomboxCoords = _coords;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/boombox", "setBoomboxInfo", e.toString());
    }
});

gm.events.add("playBoomboxMusic", (_url) => {
	try
	{
        if (!boomboxInfo.startedBoomboxMusic) {
            boomboxInfo.startedBoomboxMusic = true;
            mp.events.call('client.sounds.play2d', _url, global.MaxVolumeBoombox * 0.01 * (100 * 0.01));
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/boombox", "playBoomboxMusic", e.toString());
    }
});

gm.events.add("stopBoomboxMusic", () => {
	try
	{
        if (boomboxInfo.startedBoomboxMusic === true) {
            mp.events.call('client.sounds.stop2d');
            boomboxInfo.startedBoomboxMusic = false;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/boombox", "stopBoomboxMusic", e.toString());
    }
});

let updateBoomboxTimer = 0;
mp.keys.bind(0x45, true, () => {
    if (global.menuCheck() || !boomboxInfo.isBoomboxOwner || boomboxInfo.boomboxCoords === undefined || mp.game.gameplay.getDistanceBetweenCoords(global.localplayer.position.x, global.localplayer.position.y, global.localplayer.position.z, boomboxInfo.boomboxCoords.x, boomboxInfo.boomboxCoords.y, boomboxInfo.boomboxCoords.z, true) > 3) {
        return;
    }

    if (new Date().getTime() - updateBoomboxTimer >= 1000) {
        mp.events.callRemote("boomboxManageMenu");
        updateBoomboxTimer = new Date().getTime();
    }
});