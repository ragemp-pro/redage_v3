const getKey = number => number ? (number ^ 16 * Math.random() >> number / 4).toString(16) : ([10000000] + -1000 + -4000 + -8000 + -100000000000).replace(/[018]/g, getKey);


let timers = new Map();


const createTimeout = (callback, delay = 0) => {
    const id = getKey() + '-' + Date.now().toString(36),
        data = {};

    data.id = id;
    data.delay = delay;
    data.start = Date.now();
    data.callback = callback;

    data.timerId = setTimeout(() => {
        completeTimer (id);
    }, delay);

    timers.set(id, data);

    return data;
}


const removeTimeout = (data) => {
    const timerData = timers.get(data.id);
    if (timerData) {
        clearTimeout(timerData.timerId);
        timers.delete(data.id);
    }
}

const completeTimer = (id) => {
    const timerData = timers.get(id);
    if (timerData) {
        try {
            timerData.callback();
        } catch (e) {

        }
        timers.delete(id);
    }
}


gm.createTimeout = (func, delay, isInitFunc) => {
    let isInitCallBack;
    return (...callback) => {

        if (isInitFunc) {
            isInitCallBack = null;

            func.apply(this, callback);
        } else {

            if (isInitCallBack) {
                (0, removeTimeout)(isInitCallBack);

                isInitCallBack = null;
            }

            isInitCallBack = (0, createTimeout)(() => {
                isInitCallBack = null;

                func.apply(this, callback);
            }, delay);
        }

    };
}


gm.timers = new class {
    setTimeout(func, delay, ...arg) {
        return setTimeout(
            (...arg) => {
                try {
                    func(...arg);
                } catch (e) {
                    crushLog("setTimeout", delay.toString(), e.stack);
                }
            },
            delay,
            ...arg
        );
    }
    setInterval(func, delay, ...arg) {
        return setInterval(
            (...arg) => {
                try {
                    func(...arg);
                } catch (e) {
                    crushLog("setInterval", delay.toString(), e.stack);
                }
            },
            delay,
            ...arg
        );
    }
    setImmediate(func, ...arg) {
        return setTimeout(
            (...arg) => {
                try {
                    func(...arg);
                } catch (e) {
                    crushLog("setImmediate", delay.toString(), e.stack);
                }
            },
            0,
            ...arg
        );
    }
}