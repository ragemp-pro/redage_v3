const debugRenderMap = new Map();
class DebugRender {
    constructor(data) {
        this.name = data;
        debugRenderMap.set(data, this);
    }

    render () {}

    crush (e) {
        this.crushFlood = Date.now() + 1000;
        mp.console.logError(`[RedAge] Crush in 'render' key: ${this.name}`, true);
        crushLog("debugRender", this.name, e);
    }

    delete () {
        if (debugRenderMap.has(this.name))
            debugRenderMap.delete(this.name);
    }
}

global.debugRender = DebugRender;

let isRenderBlock = false;

const getRenderBlock = (name) => {
    if (!isRenderBlock)
        return false;

    if (isRenderBlock === name)
        return true;

    return isRenderBlock.includes(name);
}

const onCall = () => {
    const time = Date.now();

    for (let item of debugRenderMap.values()) {
        try {
            if (item.crushFlood > time)
                return;
            if (getRenderBlock (item.name))
                return;

            debugLog (`[Render-Start] ${item.name}`);

            item.render();

            debugLog (`[Render-End] ${item.name}`);

        } catch (e) {
            item.crush(e);
        }
    }
}

mp.events.add("render", onCall);

mp.events.add("debugRender", (text) => {
    if (global.IsJsonString (text))
        isRenderBlock = JSON.parse(text);
    else
        isRenderBlock = text;
});