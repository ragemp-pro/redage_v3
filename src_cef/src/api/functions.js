import { onDestroy } from 'svelte';

window.listernEvent = (eventName, ...args) => {
    try {
        if (typeof window.functionList [eventName] === "function")
            window.functionList [eventName] (...args);
    } catch (err) {
        //console.log(`[listernEvent] ${eventName}`)
    }
}

export const addListernEvent = (eventName, func) => {

    if (typeof window.functionList !== "object")
        window.functionList = {};

    window.functionList [eventName] = func;

    onDestroy(() => {
        delete window.functionList [eventName];
    });
}

export const hasJsonStructure = (str) => {
    if (typeof str !== 'string') return false;
    try {
        const result = JSON.parse(str);
        const type = Object.prototype.toString.call(result);
        return type === '[object Object]'
            || type === '[object Array]';
    } catch (err) {
        return false;
    }
}

export const loadImage = (node, url) => {
    let isDestroy = false;
    const updateNode = (node, url) => {
        if (url && /(?:jpg|jpeg|png)/g.test(url)) {
            if (isDestroy)
                return;

            const image = url;
            const img = new Image();
            img.src = image;
            img.onload = () => {
                if (isDestroy)
                    return;

                return node.nodeName === 'IMG' ?
                    node.src = image :
                    node.style.backgroundImage = `url(${image})`;
            };
        }
    }

    updateNode (node, url)

    return {
        update (url) {
            updateNode (node, url)
        },

        destroy() {
            isDestroy = false;
        }
    }
}



export const loadAwaitImage = (src) => {
    return new Promise(function(resolve) {
        let img = new Image()
        img.onload = resolve
        img.src = src;
    })
}