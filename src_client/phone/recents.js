const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";

/***
 * Recents - [{}...]
 *      Number - int
 *      Time - DateTime
 *      IsCall - Bolean,
 *      Duration - int
 */

phoneData.recents = [];

gm.events.add(clientName + "initRecents", (json) => {
    json = JSON.parse(json);

    json.forEach((item) => {
        const recentData = {
            number: item[0],
            time: item[1],
            isCall: item[2],
            duration: item[3]
        }

        phoneData.recents.push(recentData);
    })
});

const maxRecent = 35;

const addRecents = (number, isCall, time, duration = -1) => {

    const recentData = {
        number: number,
        time: JSON.parse(time),
        isCall: isCall,
        duration: duration
    }

    const index = phoneData.recents.findIndex(r => r.number === number);

    if (typeof phoneData.recents [index] === "object")
        phoneData.recents.splice(index, 1);

    phoneData.recents.push(recentData);

    if (phoneData.recents.length >= maxRecent)
        phoneData.recents.splice(phoneData.recents.length - 1, 1);
}

gm.events.add(clientName + "addRecent", addRecents);

const updateRecent = (duration) => {
    const index = phoneData.recents.length - 1;

    if (typeof phoneData.recents [index] === "object")
        phoneData.recents [index].duration = duration;
}
gm.events.add(clientName + "updateRecent", updateRecent);

const getRecents = () => {
    let contactsData = [];

    phoneData.recents.forEach((recent) => {
        contactsData.unshift({
            ...recent,
            ...global.getContact (recent.number),
        });
    });

    return contactsData;
}

rpc.register(rpcName + "getRecents", () => {
    return JSON.stringify (getRecents ());
});

rpc.register(rpcName + "recentsClear", () => {
    phoneData.recents = [];
    mp.events.callRemote(serverName + "recentsClear");
    return true;
});