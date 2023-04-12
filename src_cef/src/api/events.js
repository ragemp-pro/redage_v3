let eventsPool = {};
window.events = {}

window.events.callEvent = (eventName, ...args) => {
    if (!eventsPool [eventName]) {
        //console.log(`[events] Error, event ${eventName} not found in pool.`);
        return false;
    }

    eventsPool [eventName](...args);
};

window.events.addEvent = (eventName, callback) => {
    window.events.removeEvent(eventName);
    eventsPool [eventName] = callback;
};

window.events.removeEvent = (eventName) => {

    if (eventsPool [eventName]) {
        eventsPool [eventName] = null;
        delete eventsPool [eventName];
    }
};