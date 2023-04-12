// DIAL //
var vall, off;

gm.events.add('dial', (act, val, reset) => {
    try
    {
        switch (act) {
            case "open":
                if (reset == true) 
                {
                    mp.gui.emmit('window.router.setHud()');
                    global.menuClose();
                }
                var off = Math.random(2, 5);

                val = val;
                off = off;
                mp.gui.emmit(
                    `window.router.setView("PlayerBreakingLock", {value: ${val}, off: ${off}})`
                );
                global.menuOpen();
                break;
            case "close":
                mp.gui.emmit('window.router.setHud()');
                global.menuClose();
                break;
            case "call":
                mp.events.callRemote('dialPress', val);
                global.menuClose();
                break;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/dial", "dial", e.toString());
    }
});