var donateOpened = false;

global.binderFunctions.o_donate = () => {// F9
    if (!global.loggedin || global.chatActive ) return;

    if (global.menuCheck()) {
        if (donateOpened) {
            global.menuClose();
            mp.gui.emmit(`window.router.setHud();`);
            donateOpened = false;
        }
	} else {
        global.menuOpen();
        donateOpened = true;
        mp.gui.emmit(`window.router.setView('DonateMain')`);
        gm.discord(translateText("Изучает донат-меню"));
	}
};

gm.events.add('client.donate.buyVehNumber', (number) => {
    mp.events.callRemote('server.donate.buyVehNumber', number);
});

gm.events.add('client.donate.buySim', (number) => {
    mp.events.callRemote('server.donate.buySim', number);
});

gm.events.add('client.donate.close', () => {
    try
    {
        if(donateOpened) {
            mp.gui.emmit(
                `window.router.setHud();`
            );
            donateOpened = false;
            global.menuClose();
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/donatemenu", "client.donate.close", e.toString());
    }
});

gm.events.add('client.donate.buy.set', (id) => {
    mp.events.callRemote("server.donate.buy.set", id);
});

gm.events.add('client.donate.buy.clothes', (id) => {
    mp.events.callRemote("server.donate.buy.clothes", id);
});


gm.events.add('client.donate.buy.char', (id, name) => {
    mp.events.callRemote("server.donate.buy.char", id, name);
});

gm.events.add('client.donate.reward', () => {
    mp.events.callRemote("server.donate.reward");
});

gm.events.add('client.donate.buy.premium', (id) => {
    mp.events.callRemote("server.donate.buy.premium", id);
});

gm.events.add('client.donate.change', (amount) => {
    mp.events.callRemote("server.donate.change", amount);
});

gm.events.add('donbuy', (id, data, surname) => {
    try
    {
        if(id === 1)
        {
            if (global.checkName(data) || !global.checkName2(data) || data.length > 25 || data.length <= 2) {
                mp.events.call('notify', 1, 9, translateText("Правильный формат имени: 3-25 символов и первая буква имени заглавная"), 3000);
                return;
            }
        
            if (global.checkName(surname) || !global.checkName2(surname) || surname.length > 25 || surname.length <= 2) {
                mp.events.call('notify', 1, 9, translateText("Правильный формат фамилии: 3-25 символов и первая буква фамилии заглавная"), 3000);
                return;
            }

            var newname = data + "_" + surname;
            mp.events.callRemote("donate", id, newname);
        }
        else if(id === 999) {
            if(data.length <= 0 || data.length > 50) {
                mp.events.call('notify', 1, 9, translateText("Неправильный формат логина"), 3000);
                return;
            }
            if(surname < 25 || surname > 9999) {
                mp.events.call('notify', 1, 9, translateText("За раз можно отправить от 25 до 9.999 RedBucks"), 3000);
                return;
            }
            mp.events.callRemote("giftdonate", data, surname);
        } else mp.events.callRemote("donate", id, data);

        //global.menuClose();
        //mp.gui.emmit(`window.router.setHud();`);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/donatemenu", "donbuy", e.toString());
    }
});

gm.events.add('client.roullete.updateCase', (caseid) => {
    mp.gui.emmit(`window.updateCase(${caseid});`);
});

gm.events.add('client.roullete.open', (caseid, count) => {
    if (!global.loggedin || global.cuffed || global.isDeath == true) return;
    mp.events.callRemote('server.roullete.open', caseid, count);
});

gm.events.add('client.roullete.buy', (caseid, count) => {
    if (!global.loggedin || global.cuffed || global.isDeath == true) return;
    mp.events.callRemote('server.roullete.buy', caseid, count);
    gm.discord(translateText("Покупает кейсы"));
});

gm.events.add('client.roullete.confirm', (type, IndexList) => {
    mp.events.callRemote('server.roullete.confirm', type, IndexList);
});

gm.events.add('client.roullete.start', (value) => {
    mp.gui.emmit(`window.events.callEvent("cef.roullete.confirm", ${value})`);
    gm.discord(translateText("Крутит кейсы"));
});

gm.events.add('client.sapper.bet', (value) => {
    mp.events.callRemote('server.sapper.bet', value);
});

gm.events.add('client.sapper.game', (value) => {
    mp.gui.emmit(`window.events.callEvent("cef.sapper.game", ${value})`);
    gm.discord(translateText("Играет в сапёра"));
});

gm.events.add('client.sapper.end', (value) => {
    mp.events.callRemote('server.sapper.end', value);
});

gm.events.add('client.roullete.updateWin', (_title, _name, _image, _desc) => {
    mp.gui.emmit(`window.updateWin('${_title}', '${_name}', '${_image}', '${_desc}');`);
});

let donateBrowsers = null;

gm.events.add('client.opendonatesite', (url) => {
    try
    {
        //if (!donateOpened)
        //    return;

        donateBrowsers = mp.browsers.new(url);
        mp.gui.cursor.show(true, true);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/donatemenu", "client.opendonatesite", e.toString());
    }
});

gm.events.add('client.closedonatesite', () => {
    try
    {
        if (!donateBrowsers)
            return;
        donateBrowsers.destroy();
        donateBrowsers = null;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/donatemenu", "client.closedonatesite", e.toString());
    }
});

//

gm.events.add('client.donatepack.open', (id) => {
    mp.events.callRemote('server.donatepack.open', id);
});

let isInterface = false;
gm.events.add('client.donatepack.show', (id, title, donate, money) => {
    mp.gui.emmit(`window.router.setPopUp("PopupDonate", {id: ${id}, title: "${title}", data: [${donate}, ${money}]});`);
    mp.gui.cursor.visible = true;
    isInterface = false;
    if (!global.menuCheck ()) {
        global.menuOpen(true);
        isInterface = true;
        global.isPopup = true;
    }
});

gm.events.add('client.donatepack.rb', (id) => {
    mp.events.callRemote('server.donatepack.rb', id);
});

gm.events.add('client.donatepack.donate', (id) => {
    mp.events.callRemote('server.donatepack.donate', id);
});

gm.events.add('client.donatepack.close', () => {
    mp.gui.emmit('window.router.setPopUp()');
    if (isInterface) global.menuClose();
    isInterface = false;
    global.isPopup = false;
});








//





gm.events.add('client.donate.roulette.load', () => {
    mp.events.callRemote('server.donate.roulette.load');
});



let rouletteCategoryList = [];
let rouletteCasesData = [];

const colorList = [
    "blue",
    "yellow",
    "pink",
    "red",
]

const getCasesData = (item) => {
    let returnItem = {
        index: item[0],
        price: item[1],
        image: item[2],
        name: item[3],
        desc: item[4],
        items: [],
    }

    let items = [];
    item[5].forEach((itemData) => {
        items.push({
            title: itemData[0],
            image: itemData[1],
            color: colorList[itemData[2]],
        })
    });

    items.sort(function(a, b) {
        return a.color - b.color;
    })

    returnItem.items = items;

    return returnItem;
}

rpc.register("rpc.donate.roulette.getList", () => {
    return JSON.stringify({
        shopList: rouletteCategoryList,
        caseData: rouletteCasesData
    });
});

rpc.register("rpc.donate.roulette.getCase", (caseId) => {
    return JSON.stringify(rouletteCasesData.find(c => c.index === caseId));
});

//


gm.events.add('client.donate.roulette.loadCase', (caseId) => {
    mp.events.callRemote('server.donate.roulette.loadCase', caseId);
});

let selectCase = {}
gm.events.add('client.donate.roulette.initCase', (jsonCase) => {
    selectCase = getCasesData(JSON.parse(jsonCase))
    mp.gui.emmit(`window.listernEvent ('donate.roulette.initCase');`);
});


rpc.register("rpc.donate.roulette.getCaseOne", () => {
    return JSON.stringify(selectCase);
});



//

gm.events.add('client.donate.load', () => {
    mp.events.callRemote("server.donate.load");
});

let packList = [
    {
        id: 0,
        name: translateText("На ход ноги"),
        desc: "",
        price: 5000,
        list: []
    },
    {
        id: 1,
        name: translateText("Стартовый"),
        desc: "",
        price: 8500,
        list: []
    },
    {
        id: 2,
        name: translateText("Продвинутый"),
        desc: "",
        price: 13000,
        list: []
    },
    {
        id: 3,
        name: translateText("Заряженный"),
        desc: "",
        price: 20000,
        list: []
    },
    /*{
        id: 4,
        name: translateText("Хорошее начало"),
        desc: "",
        price: 7999,
        list: []
    },
    {
        id: 5,
        name: translateText("Большие планы"),
        desc: "",
        price: 8999,
        list: []
    },*/
    {
        id: 6,
        name: translateText("Жирный чек"),
        desc: "",
        price: 36000,
        list: []
    },
    /*{
        id: 7,
        name: translateText("Отличное начало"),
        desc: "",
        price: 17999,
        list: []
    },*/
    {
        id: 8,
        name: translateText("ЛЕГЕНДА"),
        desc: "",
        price: 65000,
        list: []
    },
    /*{
        id: 9,
        name: translateText("Прекрасное начало"),
        desc: "",
        price: 29999,
        list: []
    },*/
    {
        id: 10,
        name: translateText("Батя"),
        desc: "",
        price: 100000,
        list: []
    },
]

let vipLists = [
    {
        id: 0,
        name: translateText("Подписка"),
        class: "reborn",
        price: 15000,
        img: "subscripte",
        list: [
            translateText("20000$ каждый день."),
            translateText("150 RB Каждый день."),
            translateText("+ 1 Особенный кейс."),
            translateText("Подписку нужно забирать самостоятельно раз в 24 часа."),
        ]
    },
    {
        id: 1,
        name: "Silver",
        class: "bronze",
        price: 1500,
        img: "silver",
        list: [
            translateText("50$ прибавка к ежечасной зарплате."),
            translateText("Скидка 10% на аренду транспорта."),
        ]
    },
    {
        id: 2,
        name: "Gold",
        class: "silver",
        price: 3500,
        img: "gold",
        list: [
            translateText("70$ прибавка к ежечасной зарплате."),
            translateText("Увеличение зарплаты на начальных работах на 15%."),
            translateText("Удвоенное XP."),
            translateText("Возможность оплачивать налоги за недвижимость и бизнес на 15 дней вперёд."),
            translateText("Скидка 15% на аренду транспорта."),
        ]
    },
    {
        id: 3,
        name: "Platinum",
        class: "gold",
        price: 5000,
        img: "platinum",
        list: [
            translateText("110$ прибавка к ежечасной зарплате."),
            translateText("Увеличение зарплаты на начальных работах на 25%."),
            translateText("Удвоенное XP."),
            translateText("Возможность оплачивать налоги за недвижимость и бизнес на 20 дней вперёд."),
            translateText("Скидка 20% на аренду транспорта."),
            translateText("Каждый PayDay +2 RB на донат-счёт"),
        ]
    },
    {
        id: 4,
        name: "Diamond",
        class: "platinum",
        price: 10000,
        img: "bronze",
        list: [
            translateText("200$ прибавка к ежечасной зарплате."),
            translateText("Увеличение зарплаты на начальных работах на 35%."),
            translateText("Утроенное XP."),
            translateText("Возможность оплачивать налоги за недвижимость и бизнес на 30 дней вперёд."),
            translateText("Скидка 25% на аренду транспорта."),
            translateText("Каждый PayDay +5 RB на донат-счёт"),
            translateText("[NEW] Писать репорты без задержек!"),
        ]
    },
];

gm.events.add('client.donate.init', (premium, priceRb, giveMoney, jsonCategory, jsonKeys) => {

    premium = JSON.parse(premium);

    vipLists.forEach((item) => {
        item.price = premium[item.id].Price;
        if (item.id == 0) {
            item.list[0] = premium[item.id].GiveMoney + translateText("$ каждый день.");
            item.list[1] = premium[item.id].GiveRb + translateText(" RB каждый день.");
        } else {
            item.list[0] = premium[item.id].GiveMoney + translateText("$ прибавка к ежечасной зарплате.");
        }
    })

    //



    priceRb = JSON.parse(priceRb);
    giveMoney = JSON.parse(giveMoney);

    packList.forEach((item) => {
        item.price = priceRb[item.id];
        item.list = giveMoney[item.id];
    })


    //

    jsonCategory = JSON.parse(jsonCategory)
    rouletteCategoryList = [];
    jsonCategory.forEach((item) => {
        rouletteCategoryList.push({
            title: item[0],
            image: item[1],
            cases: item[2],
        })
    })

    jsonKeys = JSON.parse(jsonKeys)
    rouletteCasesData = [];
    jsonKeys.forEach((item) => {
        rouletteCasesData.push(getCasesData (item))
    })

    //

    mp.gui.emmit(`window.listernEvent ('donate.init');`);



});


rpc.register("rpc.donate.getPack", () => {
    return JSON.stringify(packList);
});

rpc.register("rpc.donate.vipLists", () => {
    return JSON.stringify(vipLists);
});