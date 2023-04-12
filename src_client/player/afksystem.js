global.afkStatus = false;
let afkSecondsCount = 0;
const afkMaxSecondsCount = 60 * 10;


setInterval(function () {
    afkSecondsCount++;
    //
    if (!global.loggedin
        || mp.game.controls.isControlPressed(0, 32)
        || mp.game.controls.isControlPressed(0, 33)
        || mp.game.controls.isControlPressed(0, 34)
        || mp.game.controls.isControlPressed(0, 35)
        || mp.game.controls.isControlPressed(0, 24)
        || global.isDeath
        || (global.localplayer.isInAnyVehicle(false) && global.localplayer.vehicle && global.localplayer.vehicle.getSpeed() > 0)) {
        afkSecondsCount = 0;
    }
    //

    if (afkSecondsCount >= afkMaxSecondsCount && afkStatus != true)
        global.updateAfkStatus (true);
    else if (afkStatus == true && afkSecondsCount < afkMaxSecondsCount)
        global.updateAfkStatus (false);

    if (afkStatus == true && afkSecondsCount >= afkMaxSecondsCount)
    {
        const minutes = Math.trunc(afkSecondsCount / 60);
        const seconds = afkSecondsCount % 60;
        gm.discord(translateText("Сладенько спит уже  {0}:{1}", global.formatIntZero(minutes, 2), global.formatIntZero(seconds, 2)));
    }
}, 1000);

global.updateAfkStatus = (status = false) => {

    if (afkStatus === status)
        return;

    mp.events.callRemote('server.updateAfkStatus', status);
    afkStatus = status;

    if (!status)
        global.discordDefault ()
}