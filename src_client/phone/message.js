const messageType = {
    text: 0,
    map: 1,
    img: 2,
    sticker: 3
}

const messageStatus = {
    sent: 0,
    received: 1,
    error: 2
}

const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";

global.messagesData = {};

messagesData.list = [];
messagesData.messages = {};
messagesData.draft = {};

let selectNumber = null;

const onSelectNumber = (number) => {
    if (selectNumber === number)
        return;

    selectNumber = number;

    if (!number)
        mp.events.callRemote(serverName + "sNumber", -1);
    else {
        updateStatus (number);
        mp.events.callRemote(serverName + "sNumber", number);
    }
}

gm.events.add(clientName + "closeMessage", () => {
    onSelectNumber (null);
});

const getMessageData = (number, date, text, type, isMe, status) => {
    const data = {
        Number: number,
        Avatar: null,
        Name: number,
        Date: date,
        Text: text,
        Type: type,
        IsMe: isMe,
        Status: isMe ? true : status
    }
    return data;
}

gm.events.add(clientName + "initMessages", (json) => {
    messagesData.list = [];
    messagesData.messages = {};
    messagesData.draft = {};

    json = JSON.parse(json);

    json.forEach((item, index) => {
        messagesData.list.push(getMessageData (item[1], item[4], item[2], item[3], item[5]));
    });
});

rpc.register(rpcName + "getMessages", () => {

    selectNumber = null;

    let returnData = [];

    messagesData.list.forEach(message => {
        const newMessage = {
            ...message
        };

        const draftText = getDraftText (newMessage.Number);
        if (typeof draftText === "string" && draftText.length > 0) {
            newMessage.DraftText = draftText;
        }

        const contactData = phoneData.contacts [newMessage.Number];
        if (typeof contactData === "object") {
            newMessage.Name = contactData.Name;
            newMessage.Avatar = contactData.Avatar;
        }

        const chatStatus = messagesData.ChatStatus [newMessage.Number];
        if (chatStatus && chatStatus.IsWrite) {
            newMessage.IsWrite = true;
        }

        returnData.push(newMessage)
    })

    return JSON.stringify (returnData);
});

const updateListData = (number, date, text, type, isMe, status) => {
    const index = messagesData.list.findIndex(a => a.Number == number);

    if (typeof messagesData.list [index] === "object")
        messagesData.list.splice(index, 1);

    messagesData.list.unshift(getMessageData (number, date, text, type, isMe, status));
}

const updateStatus = (number) => {
    const index = messagesData.list.findIndex(a => a.Number == number);

    if (typeof messagesData.list [index] === "object" && !messagesData.list [index].Status) {
        messagesData.list [index].Status = true;
        mp.events.callRemote(serverName + "updStatus", number);
    }
}

//

const getDraftText = (number) => {
    const draftText = messagesData.draft [number];
    if (typeof draftText === "string" && draftText.length > 0) {
        return draftText;
    }
    return "";
}

gm.events.add(clientName + "draftMessages", (text) => {
    if (typeof selectNumber === "number") {
        if (text && text.length)
            messagesData.draft [selectNumber] = text;
        else
            delete messagesData.draft [selectNumber];
    }
});

rpc.register(rpcName + "getDraftMessages", (number) => {
    return getDraftText(number);
});

//

const maxCountMessages = 15;

const getMessage = (messageList, id = -1) => {
    let messages = [];
    const index = id === -1 ? messageList.length : messageList.findIndex(m => m.Id === id);

    for(let i = index - maxCountMessages; i < index; i++) {
        if (messageList [i])
            messages.push(messageList [i]);
    }
    return JSON.stringify (messages);
}

rpc.register(rpcName + "getMessage", (number) => {

    onSelectNumber (Number (number));

    const messageData = messagesData.messages [number];

    if (typeof messageData === "object") {
        const messages = getMessage (messageData);
        onSendMessageDefault (number);
        return messages;
    }

    if (messageData !== -1) {
        messagesData.messages [number] = -1;//Антифлуд что бы не выщзывать запрос на сервер еще раз после того как запросили сообщения

        mp.events.callRemote(serverName + "getMsg", number);
    }

    return JSON.stringify([])
});

rpc.register(rpcName + "requestMessages", (id) => {
    if (typeof selectNumber === "number") {
        const messageData = messagesData.messages [selectNumber];

        if (typeof messageData === "object") {
            return getMessage (messageData, id);
        }
    }
    return null;
});

gm.events.add(clientName + "setMsg", (number, json) => {
    json = JSON.parse(json);

    let messages = [];

    json.forEach((item, index) => {
        messages.push({
            Text: item [1],
            Date: item [2],
            Me: item [3],
            Type: item [4],
            Status: messageStatus.received,
            Id: item [0]
        })
    });

    if (selectNumber === number) {
        mp.gui.emmit(`window.listernEvent ('messageInit', '${getMessage(messages)}');`);
        onSendMessageDefault (number);
    }

    messagesData.messages [number] = messages;
});

gm.events.add(clientName + "sendMsg", (key, text, type) => {
    if (typeof selectNumber === "number") {
        //

        if (typeof messagesData.messages [selectNumber] !== "object")
            messagesData.messages [selectNumber] = [];

        //

        const messageData = {
            Key: key,
            Text: text,
            Date: -1,
            Me: true,
            Type: type,
            Status: messageStatus.sent,
            Id: key
        };

        messagesData.messages [selectNumber].push (messageData);

        const contact = global.getContact(selectNumber);

        if (contact && contact.NoSend) {
            mp.events.call(clientName + "updMsgStatus", selectNumber, key, JSON.stringify(global.DateTime), messageStatus.error);
            return;
        }

        if (contact && contact.IsSystem)
            mp.events.callRemote(serverName + "sendSystemMsg", selectNumber, key, text, type);
        else
            mp.events.callRemote(serverName + "sendMsg", selectNumber, key, text, type);


        //
    }
});

gm.events.add(clientName + "updMsgStatus", (number, key, date, status) => {
    if (typeof messagesData.messages [number] !== "object")
        messagesData.messages [number] = [];

    const index = messagesData.messages [number].findIndex(m => m.Key === key);

    const message = messagesData.messages [number] [index];

    if (typeof message === "object") {
        messagesData.messages [number] [index].Date = JSON.parse(date);
        messagesData.messages [number] [index].Status = status;

        if (selectNumber === number)
            mp.gui.emmit(`window.listernEvent ('updMsgStatus', ${key}, ${date}, ${status});`);

        // Оьновляем список сообщений

        if (status === messageStatus.received)
            global.phoneSound ("msgReceived", "messagesent.ogg");

        updateListData (number, JSON.parse(date), message.Text, message.Type, true, true);
    }
});

gm.events.add(clientName + "msgAdd", (number, text, date, type) => {
    if (typeof messagesData.messages [number] !== "object")
        messagesData.messages [number] = [];

    //

    const messageData = {
        Text: text,
        Date: JSON.parse(date),
        Me: false,
        Type: type,
        Status: messageStatus.received
    };

    messagesData.messages [number].push (messageData);

    const isStatus = selectNumber === number;
    if (isStatus)
        mp.gui.emmit(`window.listernEvent ('msgAdd', '${text}', ${date}, ${type});`);
    else {
        let notifyText = text;

        if (type === messageType.map)
            notifyText = translateText("Вложение: геопозиция");
        else if (type === messageType.img)
            notifyText = translateText("Вложение: фотография");

        mp.events.call('phone.notify', number, notifyText, 5);
    }

    updateListData (number, JSON.parse(date), text, type, false, isStatus);


    //global.phoneSound ("msgReceived", "newmessage.ogg");
});

//

/***
 *
 * export const chatStatusName = [
 "",
 translateText("Онлайн"),
 translateText("Был в сети недавно"),
 translateText("Печатает")
 ]
 * @type {*[]}
 */
messagesData.ChatStatus = {};

const setChatStatus = (number) => {
    if (messagesData.ChatStatus [number]) {
        const chatStatus = messagesData.ChatStatus [number];
        if (chatStatus.IsWrite)
            mp.gui.emmit(`window.listernEvent ('phoneChatUpdStatus', 3);`);
        else
            mp.gui.emmit(`window.listernEvent ('phoneChatUpdStatus', ${chatStatus.status});`);
    } else
        mp.gui.emmit(`window.listernEvent ('phoneChatUpdStatus', 0);`);
}


let statusAntiFlood = 0;

gm.events.add(clientName + "getPhoneChatStatus", (number) => {
    const contact = global.getContact(number);

    if (contact && contact.IsSystem) {
        mp.gui.emmit(`window.listernEvent ('phoneChatUpdStatus', 1);`);
        return;
    }

    let status = true;
    const index = messagesData.list.findIndex(a => a.Number == number);

    if (typeof messagesData.list [index] === "object" && !messagesData.list [index].Status) {
        status = false;
        messagesData.list [index].Status = true;
    }

    if (statusAntiFlood > Date.now())
        return;

    statusAntiFlood = Date.now() + 250;
    mp.events.callRemoteUnreliable(serverName + "getChatStatus", number, status);
});

gm.events.add(clientName + "setPhoneChatStatus", (number, status) => {
    if (typeof messagesData.ChatStatus [number] !== "object")
        messagesData.ChatStatus [number] = {};

    messagesData.ChatStatus [number].status = status;

    if (selectNumber === number)
        setChatStatus (number);
});


gm.events.add(clientName + "setPhoneChatWrite", (number, toggled) => {
    if (typeof messagesData.ChatStatus [number] !== "object")
        messagesData.ChatStatus [number] = {};

    messagesData.ChatStatus [number].IsWrite = toggled;
    if (selectNumber === number)
        setChatStatus (number);
});

gm.events.add(clientName + "startWrite", () => {
    if (typeof selectNumber === "number") {
        if (phoneData.blackList.includes (selectNumber))
            return;

        mp.events.callRemoteUnreliable(serverName + "write", selectNumber, true);
    }
});

gm.events.add(clientName + "endWrite", () => {

    if (typeof selectNumber === "number") {
        if (phoneData.blackList.includes (selectNumber))
            return;

        mp.events.callRemoteUnreliable(serverName + "write", selectNumber, false);
    }
});

//

gm.events.add(clientName + "sendPopupMsg", (number, key, text, type) => {

    if (typeof messagesData.messages [number] !== "object")
        messagesData.messages [number] = [];

    //

    const messageData = {
        Key: key,
        Text: text,
        Date: -1,
        Me: true,
        Type: type,
        Status: messageStatus.sent,
        Id: key,
    };

    messagesData.messages [number].push (messageData);

    const contact = global.getContact(number);

    if (contact && contact.NoSend) {
        mp.events.call(clientName + "updMsgStatus", number, key, JSON.stringify(global.DateTime), messageStatus.error);
        return;
    }

    if (contact && contact.IsSystem)
        mp.events.callRemote(serverName + "sendSystemMsg", number, key, text, type);
    else
        mp.events.callRemote(serverName + "sendMsg", number, key, text, type);
});

let messageDefault = {}

gm.events.add(clientName + "messageDefault", (number) => {
    const contact = global.getContact(number);

    if (contact && contact.DefaultMessage)
        messageDefault [number] = true;
});

const onSendMessageDefault = (number) => {
    if (messageDefault [number]) {

        delete messageDefault [number];

        const contact = global.getContact(number);

        if (contact && contact.DefaultMessage) {
            setTimeout(() => {
                mp.gui.emmit(`window.listernEvent ('msgAdd', '${contact.DefaultMessage}', '${global.DateTime}', ${0});`);
            }, 150)
        }
    }
}