// ATM //
var atmIndex = 0;
var atmTcheck = 0;
let ATMTemp = "";

gm.events.add('openatm', () => {
    if (global.menuCheck()) return;
    global.menuOpen();
	gm.discord(translateText("Взаимодействует с банкоматом"));
});

gm.events.add('closeatm', () => {
    global.menuClose();
    mp.gui.emmit(
        `window.router.setHud();`
    );
});

gm.events.add('setatm', (num, name, bal, sub) => {
    mp.gui.emmit(
        `window.router.setView("PlayerAtm", {number: '${num}', holder: '${name}'})`
    );
});

gm.events.add('atmCB', (type, data) => {
    mp.events.callRemote('atmCB', type, data);
});

var LTPressed = 0;
gm.events.add('atmVal', (data) => {
	try
	{
		if (new Date().getTime() - atmTcheck < 1000) {
			LTPressed++;
			if(LTPressed >= 10) {
				LTPressed = 0;
				mp.events.callRemote('atmDP');
			}
		} 
		else if(data === 0) {
			LTPressed = 0;
			mp.events.call('notify', 1, 9, translateText("Введите корректное значение."), 3000);
		}
		else {
			LTPressed = 0;
			mp.events.callRemote('atmVal', data);
			atmTcheck = new Date().getTime();
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/atm", "atmVal", e.toString());
	}
});

gm.events.add('atmOpen', (data) => {
    mp.gui.emmit(`window.atm.open(${data})`);
});

gm.events.add('atmOpenBiz', (data1, data2) => {
    mp.gui.emmit(`window.atm.open([3, ${data1}, ${data2}])`);
});

gm.events.add('atm', (index, data) => 
{
	try
	{
		if (index == 4) {
			ATMTemp = data;
			//mp.gui.emmit('window.atm.change(44)');
		}
		else if (index == 44) {
			mp.events.callRemote('atm', 4, data, ATMTemp);
			mp.gui.emmit('window.atm.reset()');
			return;
		}
		else if (index == 33) {
			mp.events.callRemote('atm', 3, data, ATMTemp);
		}
		else {
			mp.events.callRemote('atm', index, data);
			mp.gui.emmit('window.atm.reset()');
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/atm", "atm", e.toString());
	}
});