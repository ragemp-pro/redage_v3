// WEAPON SHOP //
let weaponShopOpened = false;

gm.events.add('wshop', async (act, value, sub) => {
    try
    {
        if(new Date().getTime() - global.lastCheck < 50) return;
        global.lastCheck = new Date().getTime();
        
        switch (act) {
            case "cat":
                if (value == 3 || value == 4) return;
                wshop.tab = value;
                weaponShopOpened = true;
                mp.gui.emmit(`window.router.setView("BusinessWeaponShop")`);
                await global.wait(50);
                mp.gui.emmit(`window.weaponshop.setData(${value},'${JSON.stringify(wshop.data[value])}')`);
                break;
            case "buy":
                mp.events.callRemote('wshop', wshop.tab, value);
                break;
            case "rangebuy":
                mp.events.callRemote('wshopammo', value, sub);
                break;
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weaponshop", "wshop", e.toString());
    }
});

gm.events.add('client.weaponshop.buy', (category, activeItemID) => {  
    try
    {
        if(new Date().getTime() - global.lastCheck < 50) return;
        global.lastCheck = new Date().getTime();  
        mp.events.callRemote('server.weaponshop.buy', category, activeItemID);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weaponshop", "client.weaponshop.buy", e.toString());
    }
});

gm.events.add('client.weaponshop.buyAmmo', (category, value) => {
    try
    {
        if(new Date().getTime() - global.lastCheck < 50) return;
        global.lastCheck = new Date().getTime();    
        mp.events.callRemote('server.weaponshop.buyAmmo', category, value);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "player/weaponshop", "client.weaponshop.buyAmmo", e.toString());
    }
});

global.weaponComponentPrice = 0;
gm.events.add('client.gunshop.open', async (weaponJson, ammoJson, price) => {
    if (global.menuCheck()) return;
    global.menuOpen();

    global.weaponComponentPrice = price;
    weaponShopOpened = true;
    mp.gui.emmit(`window.router.setView("BusinessWeaponShop")`);
    await global.wait(50); 
    mp.gui.emmit(`window.weaponshop('${weaponJson}','${ammoJson}')`);
});

gm.events.add('client.weaponshop.close', () => {
    global.binderFunctions.c_weaponshop ();
});

global.binderFunctions.c_weaponshop = () => {
    if(weaponShopOpened) {
        global.menuClose();
        mp.gui.emmit(`window.router.setHud()`);
        weaponShopOpened = false;
    }
};