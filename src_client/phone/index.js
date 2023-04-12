/***
 * Blacklist - [Number - int...]
 *
 */

/***
 * Messages - {}
 *      [Number - int] - {}
 *          Time - DateTime
 *          Type - Tynyint
 *          Text - Text
 *          Data - Text
 */

global.phoneData = { }

require("./gps/index.js");
require("./taxi/index.js");
require("./mechanic/index.js");
require("./message.js");
require("./call.js");
require("./gallery");
require("./weather");
require("./news");
require("./property/index");
require("./recents");
require("./cars");
require("./job/index");
require("./settings");
require("./forbes");
require("./notify");
require("./auction");

require('./camera/index');
require("./phoneAnim");
require("./tinder");

const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";


global.phoneSound = (id, name, volume = 0.25, loop = false) => {
    mp.events.call("sounds.playAmbient", "cloud/sound/iphone/" + name, {
        id: id,
        volume: volume,
        loop: loop
    });
}

global.isPhoneOpen = false;
gm.events.add(clientName + "finger", (direction) => {
    mp.game.mobile.moveFinger(direction);
})


global.binderFunctions.openPlayerMenu = () => {// M key
    mp.events.call(clientName + "open")
};

let inFocus = false;
gm.events.add(clientName + "inputFocus", (toggled) => {
    if (!global.isPhoneOpen)
        return;

    inFocus = toggled;
    global.menuOpen(!inFocus);
});

let phoneOpenAntiFlood = 0;

gm.events.add(clientName + "open", () => {
    if (!global.loggedin || global.chatActive || global.editing || global.cuffed || global.isDeath == true || global.isDemorgan == true || global.attachedtotrunk || inFocus || phoneOpenAntiFlood > Date.now() || global.phoneCameraOpen) return;

    if (!global.isPhoneOpen) {
        if (global.menuCheck()) return;
        mp.gui.emmit(`window.hudStore.isHudNewPhone (true)`);
        mp.events.callRemote(serverName + "open");
        global.menuOpen(true);
        global.isPhoneOpen = true;
        if (global.localplayer.cSen !== "cphone_call") {
            mp.game.mobile.createMobilePhone(0);
            mp.game.mobile.setMobilePhoneScale(0);
        }
        phoneOpenAntiFlood = Date.now() + 500;

    } else if (global.menuCheck())
        mp.events.call(clientName + "close");
});

gm.events.add(clientName + "close", () => {
    if (!global.isPhoneOpen)
        return;

    phoneOpenAntiFlood = Date.now() + 500;

    inFocus = false;
    global.isPhoneOpen = false;
    mp.gui.emmit(`window.hudStore.isHudNewPhone (false)`);
    mp.events.callRemote(serverName + "close");
    global.menuClose();

    if (global.localplayer.cSen === "cphone_base")
        nativeInvoke ("DESTROY_MOBILE_PHONE");
});

/***
 * contacts - {}
 *      [Number - int]: Name - String (50)
 *
 */

const defaultContacts = {
    112: {
        Name: translateText("Полиция"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/police.jpg",
        IsSystem: true
    },
    911: {
        Name: translateText("Больница"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/ems.jpg",
        IsSystem: true
    },
    333: {
        Name: translateText("Механик"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/mech.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },
    228: {
        Name: translateText("Такси"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/taxi.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    101: {
        Name: "RedAge",
        Avatar: "https://cloud.redage.net/cloud/img/iphone/ra.jpg",
        IsSystem: true,
        DefaultMessage: translateText("Привет! :cowboy_hat_face: Сюда ты можешь отправить найденный бонус-код или промо-код и сразу получить свои бонусы. Просто пришли его в ответном сообщении! :gift: :gift: :gift:")
    },

    4386: {
        Name: translateText("Банк"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/bank.jpg",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999999: {
        Name: translateText("Информатор"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/inform.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999998: {
        Name: translateText("Склад"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/sklad.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999997: {
        Name: translateText("Аукцион"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/auc.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999996: {
        Name: "News",
        Avatar: "https://cloud.redage.net/cloud/img/iphone/news.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999995: {
        Name: translateText("Арендатор"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/rent.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999994: {
        Name: translateText("Палатка"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/tent.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999993: {
        Name: translateText("Подсказки"),
        Avatar: "https://cloud.redage.net/cloud/img/iphone/help.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },

    99999992: {
        Name: "Tinder",
        Avatar: "https://cloud.redage.net/cloud/img/iphone/tinder.png",
        IsSystem: true,
        IsNotShow: true,
        NoSend: true
    },
    99999991: {
        Name: "Прямой эфир",
        Avatar: "https://cloud.redage.net/cloud/img/iphone/efir.png",
        IsSystem: true,
        IsNotShow: false,
        NoSend: false
    },
}


phoneData.contacts = defaultContacts;

gm.events.add(clientName + "initContacts", (json) => {
    phoneData.contacts = defaultContacts;

    json = JSON.parse(json);
    for (let key in json) {
        phoneData.contacts [key] = {
            Name: json [key][0],
            Avatar: json [key][1]
        };
    }
});

phoneData.blackList = [];
gm.events.add(clientName + "initBalckList", (json) => {
    phoneData.blackList = JSON.parse(json);
});

//********************* Functions

global.getContact = (number) => {
    let rContactData = {
        Name: number.toString(),
        Number: number,
        IsBlackList: phoneData.blackList.includes (number),
        Avatar: null,
        IsAdded: false
    }

    const contactData = phoneData.contacts [number];
    if (typeof contactData === "object") {
        rContactData = {
            ...rContactData,
            ...contactData,
        }

        rContactData.IsAdded = true;
    }

    return rContactData;
}

const getContacts = () => {
    let contactsData = []
    Object.keys (phoneData.contacts).forEach((number) => {
        contactsData.push({
            ...getContact (number),
            Number: number
        });
    });

    let rContactsData = [];
    contactsData.forEach((data) => {
        const letterName = data.Name[0].toUpperCase();
        const index = rContactsData.findIndex(ld => ld.Name === letterName);

        if (index === -1) {
            const letterData = {
                Name: letterName,
                List: []
            };

            letterData.List.push(data)

            rContactsData.push(letterData);
        } else {

            rContactsData[index].List.push(data);
        }

    });

    return rContactsData;
}
//

gm.events.add(clientName + "addContact", (number, name, avatar) => {
    if (typeof phoneData.contacts [number] !== "object") {
        phoneData.contacts [number] = {
            Name: name,
            Avatar: avatar
        }
        mp.events.callRemote(serverName + "addContact", number, name, avatar);
    }
});

gm.events.add(clientName + "updateContact", (number, name, avatar) => {
    if (typeof phoneData.contacts [number] === "object") {
        phoneData.contacts [number] = {
            Name: name,
            Avatar: avatar
        };
        mp.events.callRemote(serverName + "updateContact", number, name, avatar);
    }
});

gm.events.add(clientName + "dellContact", (number) => {
    if (typeof phoneData.contacts [number] === "object") {
        delete phoneData.contacts [number];
        mp.events.callRemote(serverName + "dellContact", number);
    }
});

rpc.register(rpcName + "getContacts", () => {
    return JSON.stringify (getContacts ());
});

rpc.register(rpcName + "getContact", (number) => {
    return JSON.stringify (getContact (number));
});

//

rpc.register(rpcName + "addBlackList", (number) => {;
    const index = phoneData.blackList.findIndex(n => n === number);
    if (index === -1) {
        phoneData.blackList.push(number);
        mp.events.callRemote(serverName + "addBlackList", number);
        return true;
    }
    return false;
});

rpc.register(rpcName + "dellBlackList", (number) => {
    const index = phoneData.blackList.findIndex(n => n === number);
    if (index !== -1) {
        phoneData.blackList.splice(index, 1);
        mp.events.callRemote(serverName + "dellBlackList", number);
        return true;
    }
    return false;
});

rpc.register(rpcName + "dellContact", (number) => {
    if (typeof phoneData.contacts [number] === "object") {
        delete phoneData.contacts [number];
        mp.events.callRemote(serverName + "dellContact", number);
        return true;
    }
    return false;
});

//




//Messages