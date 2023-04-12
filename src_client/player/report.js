global.reportactive = false;

gm.events.add('addreport', (id_, author_, quest_) => {
    mp.gui.emmit(`window.reportsStore.addReport(${id_},'${author_}','${quest_}')`);
	if(global.adminLVL <= 8) 
        mp.events.call('notify', 0, 2, translateText("Пришел новый репорт!"), 3000);
})

gm.events.add('setreport', (id, name) => {
    mp.gui.emmit(`window.reportsStore.setStatus(${id}, '${name}')`);
})

gm.events.add('delreport', (id) => {
    mp.gui.emmit(`window.reportsStore.deleteReport(${id})`);
})

gm.events.add('takereport', (id) => {
    mp.events.callRemote('takereport', id);
})

gm.events.add('sendreport', (id, a) => {
    mp.events.callRemote('sendreport', id, a);
})

gm.events.add('funcreport', (id, a) => {
    global.binderFunctions.c_reports ();
    mp.events.callRemote('funcreport', id, a);
})

gm.events.add('exitreport', () => {
    global.binderFunctions.c_reports ();
})

global.binderFunctions.c_reports = () => {
    mp.gui.emmit(`window.router.setHud();`)
    global.menuClose();
    global.reportactive = false;
    mp.gui.cursor.visible = false;
}

global.binderFunctions.o_reports = () => {// F6 key report menu
    if (!global.loggedin || global.chatActive || global.editing || global.advertsactive || new Date().getTime() - global.lastCheck < 1000) return;
    if (global.isAdmin != true) return;
    //global.lastCheck = new Date().getTime();
    if (!global.menuCheck ()) 
    {
        global.menuOpen();
        mp.gui.cursor.visible = true;
        global.reportactive = true;
        mp.gui.emmit(`window.router.setView('PlayerReports');`);
    } 
    else if(global.reportactive) global.binderFunctions.c_reports ();
};

var f10rep = new Date().getTime();

gm.events.add('f10report', (report) => {
	if (!global.loggedin || new Date().getTime() - f10rep < 3000) return;
    f10rep = new Date().getTime();

    mp.events.callRemote('f10helpreport', report);
    global.closeHelpMenu();
});