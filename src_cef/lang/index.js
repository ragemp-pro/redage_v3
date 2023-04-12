const url = "https://cloud.redage.net/lang";

/*
import { writable } from "svelte/store";
export const langText = writable({});

const getData = async (lang) => {
    const response = await fetch(`${url}/${lang}.json`);
    const ajax = await response.json();
    langText.set(ajax);
}

export const currentLang = writable('ru');
currentLang.subscribe(value => {
    getData (value);
});


export const translateText = (_langText, key) => {
    if (_langText && _langText[key])
        return _langText[key];

    return {};
}*/

//import { get, isString } from "lodash";
import { writable } from "svelte/store";

import test from './ru.json'

let lang = test;


const getData = async (value) => {
    const response = await fetch(`${url}/${value}.json`);
    const ajax = await response.json();
    //lang = ajax;
}

export const currentLang = writable('ru');
currentLang.subscribe(value => {
    //getData (value);
});

const isString = (text) => typeof text === "string";

const get = (args) => {
    let formatArgs = false;
    let text = lang;
    for (let i = 0; i < args.length; i++) {
        text = text[args[i]];

        if (typeof text === "undefined") {
            text = false;
            break;
        }

        if (isString (text)) {
            formatArgs = args;
            formatArgs.splice(0, i + 1);
            break;
        }
    }

    if (formatArgs && typeof formatArgs[0] !== "undefined") {
        return {
            text: text,
            formatArgs: formatArgs
        }
    }

    return text;
}

const format = (text, ...args) => {
    return text.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] !== "undefined" ? args[number] : match;
    });
};

export const translateText = (...keys) => {
    let result = get(keys);

    if (!isString(result) && result.formatArgs) {
        result = format(result.text, ...result.formatArgs);
    }

    return isString(result) ? result : `Неизвестный ключ ${keys.join(".")}`;
};