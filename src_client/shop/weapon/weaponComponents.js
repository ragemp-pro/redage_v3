require('./weaponComponentsData.js');
require('./weaponNameToHash.js');

gm.events.add('client.weaponshop.components', (weaponHash) => {
    try{
        weaponHash = global.WeaponNameToHash [weaponHash];
        if (!global.ComponentsData [weaponHash] || !weaponHash) {
            mp.events.call('notify', 4, 9, translateText("На данное оружие нет модификаций!"), 3000);
            return;
        }
        else if (!global.ComponentsData [weaponHash].Components) {
            mp.events.call('notify', 4, 9, translateText("На данное оружие нет модификаций!"), 3000);
            return;
        }

        let componentsData = [];
        let componentsType = [];
        for (let key in global.ComponentsData [weaponHash].Components) {
            const cData = global.ComponentsData [weaponHash].Components[key];
            if (!componentsType.includes(cData.Type)) componentsType.push(cData.Type);
            componentsData.push({
                Name: global.escapeHtml (mp.game.ui.getLabelText(cData.Name)),
                Desc: global.escapeHtml (mp.game.ui.getLabelText(cData.Desc)),
                Mats: Math.round(cData.Price / 100 * global.weaponComponentPrice),
                type: cData.Type,
                hash: key
            });        
        }

        mp.gui.emmit(`window.weaponshopcomponents('${JSON.stringify(componentsData)}','${JSON.stringify(componentsType)}')`);
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/weapon/weaponComponents", "client.weaponshop.components", e.toString());
    }
});

gm.events.add('client.weaponshop.buyComponent', (category, activeItemID, componentId) => {
    try
    {
        if(new Date().getTime() - global.lastCheck < 50) return;
        global.lastCheck = new Date().getTime();
        mp.events.callRemote('server.weaponshop.buyComponent', Number (category), Number (activeItemID), componentId.toString());
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/weapon/weaponComponents", "client.weaponshop.buyComponent", e.toString());
    }
});

