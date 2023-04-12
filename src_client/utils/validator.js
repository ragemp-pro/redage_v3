global.isInvalidLogin = (str) =>
{
	try
	{
		if(str.length > 32) return true;
		else if(str.length < 3) return true;
		else return !(/^[a-zA-Z1-9]*$/g.test(str));
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/validator", "isInvalidLogin", e.toString());
		return true;
	}
}

global.isInvalidEmail = (str) =>
{
	try
	{
		if(str.length > 32) return true;
		else if(str.length < 3) return true;
		else return !(/^[0-9a-z-_\.]+\@[0-9a-z-]{2,}\.[a-z]{2,}$/i.test(str));
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/validator", "isInvalidEmail", e.toString());
		return true;
	}
}

global.checkName = (str) => 
{
    return !(/^[a-zA-Z]*$/g.test(str));
}

global.checkName2 = (str) =>
{
	try
	{
		let ascii = str.charCodeAt(0);
		if (ascii < 65 || ascii > 90) return false; // Если первый символ не заглавный, сразу отказ
		let bsymbs = 0; // Кол-во заглавных символов
		for (let i = 0; i != str.length; i++) {
			ascii = str.charCodeAt(i);
			if (ascii >= 65 && ascii <= 90) bsymbs++;
		}
		if (bsymbs > 2) return false; // Если больше 2х заглавных символов, то отказ. (На сервере по правилам разрешено иметь Фамилию, например McCry, то есть с приставками).
		return true; // string (имя или фамилия) соответствует
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/validator", "checkName2", e.toString());
		return false;
	}
}