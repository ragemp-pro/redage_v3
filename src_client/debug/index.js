
require('./events')
//require('./render')
require('./natives')
require('./stream')
require('./timer')
require('./discord')


global.isDebug = false;

global.debugLog = (text, _isDebug = false) => {
    if (global.isDebug || _isDebug) {
        mp.console.logError(`[RedAge] DebugLog: ${text}`, true);
    }
}

let crushLogList = [];

global.crushLog = (type, name, e) => {
    const errorText = e && e.stack ? e.stack : JSON.stringify(e);
    const addListText = `${type}_${name}_${errorText}`;

    if (crushLogList.includes(addListText))
        return;

    crushLogList.push(addListText);
    mp.events.callRemote("client_trycatch", type, name, errorText);
}

