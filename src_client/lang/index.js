import ruLanguage from './ru'

const language = {
    'ru': ruLanguage
}

let currentLang = 'ru';

global.translateText = function (text) {
    const languageText = language[currentLang][text];

    if (languageText)
        text = languageText;

    const args = Array.prototype.slice.call(arguments, 1);
    return text.replace(/{(\d+)}/g, function (match, number) {
        return typeof args[number] != 'undefined' ? args[number] : match;
    });
};