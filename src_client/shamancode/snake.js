let crawlplayer = mp.players.local;
crawlplayer.crawl = false;
let timesatm = new Date().getTime();
let SATM = false;


//global.binderFunctions.snake = () => {
global.binderFunctions.Snake = () => {  
    
    if(crawlplayer.vehicle || crawlplayer.isTypingInTextChat) return;
    if(crawlplayer.crawl){
        if (new Date().getTime() - timesatm > 2000) {
            timesatm = new Date().getTime();
            SATM = !SATM; 
        crawlplayer.crawl = false;
        clearInterval(crawlInterval);
        crawlplayer.clearTasks();
        crawlplayer.clearSecondaryTask()
        }}
        else{
            if (!global.loggedin || global.chatActive || global.menuCheck() || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || global.localplayer.isInAnyVehicle(false) || global.localplayer.isFalling() || global.localplayer.isCuffed() || global.localplayer.isFatallyInjured() || global.localplayer.isShooting() || global.localplayer.isSwimming() || global.localplayer.isClimbing()) return;
            else{
                if (new Date().getTime() - timesatm > 3000) {
                    timesatm = new Date().getTime();
                    SATM = !SATM;          
                    crawlInterval = setInterval(handleControls, 0);
                    crawlplayer.crawl = true;
                    mp.game.streaming.requestAnimDict('move_crawlprone2crawlfront');
                    mp.events.callRemote('Snake', SATM);
                    crawlplayer.taskPlayAnim(
                            'move_crawlprone2crawlfront',
                            'front',
                            8.0,
                            1000,
                            -1,
                            2,
                            0,
                            false,
                            false,
                            false
            );
            }}}
    };

let anim;
let timeoutAnim;

function handleControls() {
    if (!crawlplayer.crawl) return;
    let dict = 'move_crawl';
    let rotation = crawlplayer.getRotation(2);
    mp.game.controls.disableControlAction(0, 32, true);
    mp.game.controls.disableControlAction(0, 33, true);
    mp.game.controls.disableControlAction(0, 34, true);
    mp.game.controls.disableControlAction(0, 35, true);
    if(mp.game.controls.isDisabledControlPressed(0, 34))
    {
        crawlplayer.setRotation(rotation.x, rotation.y, rotation.z + 0.2, 2, true);
    }
    if(mp.game.controls.isDisabledControlPressed(0, 35))
    {
    crawlplayer.setRotation(rotation.x, rotation.y, rotation.z - 0.2, 2, true);
    }
    if(mp.game.controls.isDisabledControlPressed(0, 32))
    {
        if (anim === ('onfront_fwd' || 'onfront_bwd') || timeoutAnim) return;
        anim = 'onfront_fwd';
        let timer = mp.game.entity.getEntityAnimDuration('move_crawl', anim);
        mp.game.streaming.requestAnimDict(dict);
        crawlplayer.taskPlayAnim(dict, anim, 8.0, 1000, -1, 2, 0, false, false, false);
        timeoutAnim = setTimeout(() => {
            anim = undefined;
            timeoutAnim = undefined;
        }, (timer - 0.1) * 1000);
    }
    if(mp.game.controls.isDisabledControlPressed(0, 33))
    {
        if (anim === ('onfront_fwd' || 'onfront_bwd') || timeoutAnim) return;
        anim = 'onfront_bwd';
        let timer = mp.game.entity.getEntityAnimDuration('move_crawl', anim);
        mp.game.streaming.requestAnimDict(dict);
        crawlplayer.taskPlayAnim(dict, anim, 8.0, 1000, -1, 2, 0, false, false, false);
        timeoutAnim = setTimeout(() => {
            anim = undefined;
            timeoutAnim = undefined;
        }, (timer - 0.1) * 1000);
    }


}