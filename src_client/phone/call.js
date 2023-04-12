const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";

phoneData.coming = {};

const maxCallTime = 45;

let callTime = null;

const stopSound = () => mp.events.call("sounds.stop", "call");

const closeCallView = () => mp.gui.emmit(`window.listernEvent ('downPhone');`);
const isPhoneCall = () => mp.gui.emmit(`window.listernEvent ('isPhoneCall');`);

const clearTimeOut = (isEndTimeOut = false) => {
    if (!isEndTimeOut && callTime !== null)
        clearTimeout (callTime);

    if (isEndTimeOut)
        mp.events.call(clientName + "put");

    callTime = null;
}

const clearData = () => {
    phoneData.coming = {};
}

gm.events.add(clientName + "take", () => {//Поднять телефон
    mp.events.callRemote(serverName + "take");
});

gm.events.add(clientName + "put", () => {//Отклонить

    mp.events.callRemote(serverName + "put");

    clearTimeOut ();
    clearData ();
    closeCallView ();
    stopSound ();
});

gm.events.add(clientName + "callError", () => {
    //if (phoneData.coming.callStart)
    //    mp.events.callRemote(serverName + "take");

    clearTimeOut ();
    clearData ();
    closeCallView ();
    stopSound ();
    global.phoneSound ("msgReceived", "abonentdaun.ogg");
});


gm.events.add(clientName + "call", (number) => {
    const contact = global.getContact(number);

    if (contact && contact.IsSystem) {
        mp.gui.emmit(`window.listernEvent ('phone.call.onMessage', ${number});`);
        return;
    }

    mp.events.callRemote(serverName + "call", number);

    //

    phoneData.coming = {
        Number: number,
        isCall: true,
        isComing: false,
    };

    //

    callTime = setTimeout(() => {
        clearTimeOut (true);
        clearData ();
        closeCallView ();
        stopSound ();
    }, maxCallTime * 1000);
});

gm.events.add(clientName + "callStart", () => {
    phoneData.coming.callStart = true; // Сбросить телефон

    //mp.events.call("sounds.stop", "call");

    global.phoneSound("call", "call.ogg", 0.25, true)
});

gm.events.add(clientName + "callAccept", () => {//Начали говорить
    phoneData.coming.isComing = true;

    if (phoneData.coming.callStart)
        delete phoneData.coming.callStart;

    clearTimeOut ();
    stopSound ();

    mp.gui.emmit(`window.listernEvent ('callAccept');`);
});

gm.events.add(clientName + "bell", (number) => {
    //
    phoneData.coming = {
        Number: number,
        isCall: false,
        isComing: false,
    };

    isPhoneCall ();

    mp.events.call('phone.notify', number, translateText("Звонит вам.."), 5);
});

rpc.register(rpcName + "getComingPhone", () => {
    if (phoneData.coming && phoneData.coming.Number) {
        const comingData = {
            ...phoneData.coming,
            ...getContact(phoneData.coming.Number),

        }
        return JSON.stringify(comingData);
    }

    return false;
});

rpc.register(rpcName + "isCall", () => {
    if (phoneData.coming && typeof phoneData.coming.Number !== "undefined")
        return true;
    else
        return false;
});

//

global.phoneMute = false;
gm.events.add(clientName + "mute", (toggled) => {
    global.phoneMute = toggled;
});
