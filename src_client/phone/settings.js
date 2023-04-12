const
    clientName = "client.phone.settings.",
    rpcName = "rpc.phone.settings.",
    serverName = "server.phone.settings.";

let settingData = {}

gm.events.add(clientName + "init", (json) => {
    settingData = JSON.parse(json);
});

rpc.register(rpcName + "isAir", () => {
    return settingData.IsAir;
});

gm.events.add(clientName + "air", () => {
    settingData.IsAir = !settingData.IsAir;
    mp.events.callRemote(serverName + "air");
});

//

rpc.register(rpcName + "forbesVisible", () => {
    return settingData.ForbesVisible;
});

gm.events.add(clientName + "forbesVisible", () => {
    settingData.ForbesVisible = !settingData.ForbesVisible;
    mp.events.callRemote(serverName + "forbesVisible");
});
//


gm.events.add(clientName + "removeSim", () => {
    mp.events.callRemote(serverName + "removeSim");
});

//

rpc.register(rpcName + "bellId", () => {
    return settingData.BellId;
});

gm.events.add(clientName + "bellId", (id) => {
    settingData.BellId = id;
    mp.events.callRemote(serverName + "bellId", id);
});

//

rpc.register(rpcName + "smsId", () => {
    return settingData.SmsId;
});

gm.events.add(clientName + "smsId", (id) => {
    settingData.SmsId = id;
    mp.events.callRemote(serverName + "smsId", id);
});

//

//

rpc.register(rpcName + "wallpaper", () => {
    return settingData.Wallpaper;
});

gm.events.add(clientName + "wallpaper", (url) => {
    settingData.Wallpaper = url;
    mp.events.callRemote(serverName + "wallpaper", url);
});

//

gm.events.add(clientName + "play", (url) => {
    mp.events.call("sounds.stop", "phoneSound");

    mp.events.call("sounds.playAmbient", url, {
        id: "phoneSound",
        volume: 0.05,
    });
});

