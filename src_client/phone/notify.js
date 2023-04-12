
///
class Notification {
    constructor(number, text, timeout) {
        this.number = number;
        this.text = text;
        this.endtime = -1;
        this.timeout = timeout * 1000;
    }
}

let notifications = [];

const AddNotification = (number, text, timeout) => {
    const index = notifications.findIndex(notify => notify.number === number);
    if (index !== -1) {
        notifications [index].timeout = timeout * 1000;
        notifications [index].text = text;
        if (index === 0) {
            notifications [index].endtime = new Date().getTime() + timeout;
            UpdateNotification();
        }
    } else 
        notifications.push(new Notification(number, text, timeout));
}

const UpdateNotification = () => {
    if (typeof notifications[0] === "object") {
        const notify = notifications[0];
        const contact = global.getContact(notify.number);

        const notifyData = {
            Name: contact.Name,
            Avatar: contact.Avatar,
            Text: notify.text,
        }

        mp.gui.emmit(`window.listernEvent ('phone.notify', '${JSON.stringify(notifyData)}');`);
    } else
        mp.gui.emmit(`window.listernEvent ('phone.notify', false);`);
}

const Tick = () => {
    if ((!global.menuOpened || global.isPhoneOpen) && notifications.length > 0) {
        if (typeof notifications[0] === "object") {
            const now = new Date().getTime();
            const notify = notifications[0];

            if(notify.endtime === -1) {
                notify.endtime = new Date().getTime() + notify.timeout;
                UpdateNotification ();
            }

            if(notify !== undefined && now > notify.endtime) {
                notifications.splice(0, 1);
                UpdateNotification ();
            }
        }
    }
}

gm.events.add(global.renderName ["150ms"], () => {
    Tick ();
});

gm.events.add('phone.notify', (number, msg, time) => {
    AddNotification (number, msg, time);
});
