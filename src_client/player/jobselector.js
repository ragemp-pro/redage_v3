var jobselectorOpened = false;

gm.events.add("showJobMenu", (lvlJson) => {
    try
    {
        if(!global.menuCheck() || jobselectorOpened == true)
        {
            global.menuOpen();
            mp.gui.cursor.visible = true;
            jobselectorOpened = true;
            gm.discord(translateText("Выбирает работу по душе"));

            mp.gui.emmit(
                `window.router.setView("PlayerJobSelector", '${lvlJson}')`
            );
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/jobselector", "showJobMenu", e.toString());
    }
});

gm.events.add("closeJobMenu", () => 
{
    global.menuClose();
    mp.gui.cursor.visible = false;
    jobselectorOpened = false;
    mp.gui.emmit(`window.router.setHud()`);
});

global.binderFunctions.jobselectorOpened = () => {
    if(jobselectorOpened)
    {
        mp.gui.emmit(`window.router.setHud()`);
        jobselectorOpened = false;
        global.menuClose();
    }
};

gm.events.add("selectJob", (jobid) => {
    try
    {
        if (new Date().getTime() - global.lastCheck < 1000) return;
        global.lastCheck = new Date().getTime();
        mp.events.callRemote("jobjoin", jobid);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/jobselector", "selectJob", e.toString());
    }
});