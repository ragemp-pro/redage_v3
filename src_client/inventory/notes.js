let NotesVisible = false;
global.OpenNotes = (text) => {
    if (global.menuCheck()) return;
    else if (NotesVisible) return;
    if (!text) mp.gui.emmit(`window.router.setView("EventsValentine");`);
    else mp.gui.emmit(`window.router.setView("EventsValentine", '${text}');`);
    NotesVisible = true;
    global.menuOpen();
}

gm.events.add("client.note.close", () => {
    try
    {
        if (!NotesVisible) return;
        mp.gui.emmit(`window.router.setHud();`);
        NotesVisible = false;
        global.menuClose();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "inventory/notes", "client.note.close", e.toString());
    }
});

gm.events.add("client.note.create", (type, ItemId, nameValue, textValue) => {
    try
    {
        if (!NotesVisible) return;
        mp.events.call('client.note.close');
        mp.events.callRemote('server.note.create', type, ItemId, nameValue, textValue);
        gm.discord(translateText("Пишет записку"));
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "inventory/notes", "client.note.create", e.toString());
    }
});

gm.events.add("client.note.open", (json) => {
    global.OpenNotes (json);
    gm.discord(translateText("Читает записку"));
});