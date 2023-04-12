let 
    text = "",
    textPattern = /^[a-zA-Z0-9]+$/,
    onlyTextPattern = /^[a-zA-Z]+$/,
    phoneTextPattern = /^[а-яА-Яa-zA-Z0-9]+$/,
    loginPattern = /^[a-zA-Z0-9]+$/,
    emailPattern = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/,
    textAdPattern = /^[0-9a-zA-Zа-яА-Яё_ @*()-=?«»[\]!#$%:;.,^]+$/;

export let validate = (validator, value) => {
    text = "";
    switch(validator) {
        case "login":
            if(value.length < 3)
                text = "Логин не может быть меньше 5 символов.";
            else if(value.length > 32)
                text = "Логин не может быть больше 32 символов.";
            else if(!textPattern.test(String(value).toLowerCase()))
                text = "Логин может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
            break;
        case "password":
            if(value.length < 5)
                text = "Пароль не может быть меньше 5 символов.";
            else if(value.length > 32)
                text = "Пароль не может быть больше 32 символов.";
            else if(!textPattern.test(String(value).toLowerCase()))
                text = "Пароль может состоять из букв латинского алфавита и цифр.";
            break;
        case "email":
            if(value.length < 5)
                text = "Электронная почта не может быть меньше 5 символов.";
            else if(value.length > 32)
                text = "Электронная почта не может быть больше 32 символов.";
            else if(!emailPattern.test(String(value).toLowerCase()))
                text = "Электронная почта не соответствует формату.";
            break;
        case "promocode":
            if(value.length < 5)
                text = "Промокод не может быть меньше 5 символов.";
            else if(value.length > 16)
                text = "Промокод не может быть больше 16 символов.";
            else if(!textPattern.test(String(value).toLowerCase()))
                text = "Промокод не соответствует формату.";
            break;
        case "name":
            if(!value || value.length < 3)
                text = "Имя не может быть меньше 3 символов.";
            else if(value.length > 25)
                text = "Имя не может быть больше 25 символов.";
            else if(!onlyTextPattern.test(String(value).toLowerCase()))
                text = "Имя персонажа может состоять только из букв латинского алфавита.";
            else {
                let firstSymbol = value[0];
                if(firstSymbol !== firstSymbol.toUpperCase()) {
                    text = "Первый символ должен быть заглавным. Разрешённые форматы: Pavel, Michael";
                    break;
                }

                let upperCaseCount = 0; // Кол-во заглавных символов
                for (let i = 0; i != value.length; i++) {
                    let symbol = value[i];
                    if (symbol === symbol.toUpperCase()) upperCaseCount++;
                }

                if (upperCaseCount > 1) {
                    text = "В имени больше 1 заглавных букв. Разрешённые форматы: Pavel, Michael";
                    break;
                }
            }
            break;
        case "surname":
            if(!value || value.length < 3)
                text = "Фамилия не может быть меньше 3 символов.";
            else if(value.length > 25)
                text = "Фамилия не может быть больше 25 символов.";
            else if(!onlyTextPattern.test(String(value).toLowerCase()))
                text = "Фамилия персонажа может состоять только из букв латинского алфавита.";
            else {
                let firstSymbol = value[0];
                if(firstSymbol !== firstSymbol.toUpperCase()) {
                    text = "Первый символ должен быть заглавным. Разрешённые форматы: Best, Sokolyansky";
                    break;
                }

                let upperCaseCount = 0; // Кол-во заглавных символов
                for (let i = 0; i != value.length; i++) {
                    let symbol = value[i];
                    if (symbol === symbol.toUpperCase()) upperCaseCount++;
                }
                
                if (upperCaseCount > 2) { // Если больше 2х заглавных символов, то отказ. (На сервере по правилам разрешено иметь Фамилию, например McCry, то есть с приставками).
                    text = "В фамилии больше 2 заглавных букв. Разрешённые форматы: Best, Sokolyansky";
                    break;
                }
            }
            break;
        case "phonename":
            if(value.length < 1)
                text = "Имя не может быть меньше 4 символов.";
            else if(value.length > 16)
                text = "Имя не может быть больше 16 символов.";
            else if(!textAdPattern.test(String(value).toLowerCase()))
                text = "Неверный формат.";
            break;
        case "phonenumber":
            value = Number(value);
            if(!value || value < 2)
                text = "Имя не может быть меньше 2 символов.";
            else if(value > 999999999)
                text = "Имя не может быть больше 9 символов.";
            break;
        case "textAd":
            if(value.length < 15)
                text = "Текст не может быть меньше 15 символов.";
            else if(value.length > 150)
                text = "Текст не может быть больше 150 символов.";
            else if(!textAdPattern.test(String(value).toLowerCase()))
                text = "Текст может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
            break;
        case "titleAd":
            if(value.length < 3)
                text = "Заголовок не может быть меньше 3 символов.";
            else if(value.length > 20)
                text = "Заголовок не может быть больше 20 символов.";
            else if(!textAdPattern.test(String(value).toLowerCase()))
                text = "Заголовок может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
            break;
        case "vehicleNumber":
            if(value.length < 2)
                text = "Номер не может быть меньше 2 символов.";
            else if(value.length > 8)
                text = "Номер не может быть больше 8 символов.";
            else if(!textPattern.test(String(value).toLowerCase()))
                text = "Номер не соответствует формату.";
            break;

        case "discord":
            if (!value.includes("https://discord.gg/"))
                text = "Неверный формат ссылки.";
            
            const discord = value.replace("https://discord.gg/", "")
            
            if(discord.length < 2)
                text = "Ссылка не может быть меньше 1 символов.";
            else if(discord.length > 10)
                text = "Ссылка не может быть больше 10 символов.";
            else if(!textPattern.test(String(discord).toLowerCase()))
                text = "Ссылка может состоять из букв латинского алфавита и цифр и спец. символов ( - _ . ).";
            break;
    }
    return {
        valid: (text.length > 0 ? false : true),
        text: text
    };
}