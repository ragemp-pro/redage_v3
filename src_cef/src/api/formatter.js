function replaceAll(string, search, replace) {
    return string.split(search).join(replace);
}

const newRankPattern = /^[а-яА-Яa-zA-Z0-9_\-.\s]+$/;
const textPattern = /[^0-9a-zA-Zа-яА-Яё_ @*()-=?«»"[\]!#$%:;.,^"'\s\d]/g;

export let format = (name, value) =>
{
    try {
        let text = "";
        switch(name) {
            case "rank":
                if(value.length < 2)
                    text = "Ранг не может быть меньше 2 символов.";
                else if(value.length > 25)
                    text = "Ранг не может быть больше 25 символов.";
                else if(!newRankPattern.test(String(value).toLowerCase()))
                    text = "Ранг может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
            case "tag":
                if(value.length < 2)
                    text = "Тэг не может быть меньше 2 символов.";
                else if(value.length > 5)
                    text = "Тэг не может быть больше 5 символов.";
                else if(!newRankPattern.test(String(value).toLowerCase()))
                    text = "Тэг может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
            case "createOrg":
                if(value.length < 3)
                    text = "Название организации не может быть меньше 3 символов.";
                else if(value.length > 30)
                    text = "Название организации не может быть больше 30 символов.";
                else if(!newRankPattern.test(String(value).toLowerCase()))
                    text = "Название организации может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
            case "name":
                if(value.length < 2)
                    text = "Поле не может быть меньше 2 символов.";
                else if(value.length > 32)
                    text = "Поле не может быть больше 32 символов.";
                else if(!newRankPattern.test(String(value).toLowerCase()))
                    text = "Поле может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
            case "text":
                if(value.length < 2)
                    text = "Текст не может быть меньше 2 символов.";
                else if(value.length > 185)
                    text = "Текст не может быть больше 185 символов.";
                else if(!newRankPattern.test(String(value).toLowerCase()))
                    text = "Текст может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
            case "call":
                if(value.length < 2)
                    text = "Позывной не может быть меньше 2 символов.";
                else if(value.length > 6)
                    text = "Позывной не может быть больше 6 символов.";
                else if(!newRankPattern.test(String(value).toLowerCase()))
                    text = "Позывной может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
            case "money":
                if (!value) return value;
                // Форматирование денег 1.000.000.000
                value = value.toString().replace(/\D/,'');
                return value.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1.');
            case "materials":
                if (!value) return value;
                // Форматирование материалов в гараже фракции 14 000                
                value = value.toString().replace(/\D/,'');
                return value.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1.');
            case "stringify":                
                if (!value) return value;
                const entityServerMap = {
                    '&amp;': '$1$',
                    '&lt;': '$2$',
                    '&gt;': '$3$',
                    '&quot;': '$4$',
                    "&#39;": '$5$',
                    '&#x2F;': '$6$',
                    '&#x60;': '$7$',
                    '&#x3D;': '$8$',
                    '&': '$1$',
                    '<': '$2$',
                    '>': '$3$',
                    '"': '$4$',
                    "'": '$5$',
                    '/': '$6$',
                    '`': '$7$',
                    '=': '$8$',
                };
                for(let key in entityServerMap) {
                    value = replaceAll (value, key, entityServerMap [key])
                }
                return value;
                //return String(value).replace(/[&<>"'`=\/]/g, function (s) {
                //    return entityServerMap[s];
                //});
            case "parse":
                if (!value) return value;
                const entityClientMap = {
                    '$1$': '&amp;',
                    '$2$': '&lt;',
                    '$3$': '&gt;',
                    '$4$': '&quot;',
                    '$5$': '&#39;',
                    '$6$': '&#x2F;',
                    '$7$': '&#x60;',
                    '$8$': '&#x3D;'
                };
                for(let key in entityClientMap) {
                    value = replaceAll (value, key, entityClientMap [key])
                }
                return value;
            case "parseDell":
                if (!value) return value;
                const entityClientMapDell = {
                    '&amp;': '',
                    '&lt;': '',
                    '&gt;': '',
                    '&quot;': '',
                    "&#39;": '',
                    '&#x2F;': '',
                    '&#x60;': '',
                    '&#x3D;': '',
                    '&nbsp;': '',
                    '&': '',
                    '<': '',
                    '>': '',
                    '"': '',
                    "'": '',
                    '/': '',
                    '`': '',
                    '=': '',
                };
                for(let key in entityClientMapDell) {
                    value = replaceAll (value, key, entityClientMapDell [key])
                }
                return value;
            case "textAd":
                if(value.length < 15)
                    text = "Текст не может быть меньше 15 символов.";
                else if(value.length > 150)
                    text = "Текст не может быть больше 150 символов.";
                else if(!textPattern.test(String(value).toLowerCase()))
                    text = "Текст может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
                break;
        }
        return {valid: (text.length > 0 ? false : true), text: text};
    } catch(e) {
        console.log(e);
        return 0;
    }
}