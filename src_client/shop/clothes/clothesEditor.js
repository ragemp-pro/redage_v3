let gender = 1;

let selectComponent = "";
let selectDrawable = 0;
let selectTexture = 0;
let openClothesEditor = false;

let ClothesData = {
    1: {},
    0: {}
}
gm.events.add('client.clothesEditor.data', function (_gender, json) {
    ClothesData[_gender] = JSON.parse (json);
})

gm.events.add('client.clothesEditor.open', function () {
    try
    {
        global.localplayer.freezePosition(true);

        global.createCamera ("char", global.localplayer);
        
        //global.localplayer.taskPlayAnim("amb@world_human_guard_patrol@male@base", "base", 8.0, 1, -1, 1, 0.0, false, false, false);
        global.localplayer.model = mp.game.joaat('mp_m_freemode_01');

        clearClothesCreator ();
        mp.gui.emmit(`window.router.setView('PlayerClothesEditor')`);

        global.menuOpen();
        selectComponent = "";
        openClothesEditor = true;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/clothesEditor", "client.clothesEditor.open", e.toString());
    }
});

gm.events.add('client.clothesEditor.close', () => {
    try
    {
        global.cameraManager.stopCamera(true, 1000);

        mp.game.ui.displayRadar(true);

        //global.localplayer.stopAnimTask("amb@world_human_guard_patrol@male@base", "base", 3.0)
        global.localplayer.freezePosition(false);

        mp.gui.emmit(`window.router.setHud();`);
        global.menuClose();
        mp.events.callRemote("server.clothesEditor.close");
        openClothesEditor = false;
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/clothesEditor", "client.clothesEditor.close", e.toString());
    }
});

let drawable = 1;

gm.events.add('client.clothesEditor.update', (_id, _props, _drawable, _texture) => {
    try {
        if (!_props) {

            selectDrawable = Number (_drawable);
            selectTexture = Number (_texture);
            switch (Number (_id)) {
                /*case 11:
                case 8:
                    selectComponent = 1;
                    if (ClothesData[gender][selectComponent][Number (_drawable)]) {
                        const texture = ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] ? ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] : Number (_texture);
                        global.localplayer.setComponentVariation(11, ClothesData[gender][selectComponent][Number (_drawable)][1], texture, 0);
                        global.localplayer.setComponentVariation(3, ClothesData[gender][selectComponent][Number (_drawable)][2], 0, 0);
                    } else if (ClothesData[gender][2][Number (_drawable)]) {
                        selectComponent = 2;
                        const texture = ClothesData[gender][2][Number (_drawable)][4][Number (_texture)] ? ClothesData[gender][2][Number (_drawable)][4][Number (_texture)] : Number (_texture);
                        global.localplayer.setComponentVariation(11, ClothesData[gender][2][Number (_drawable)][1], texture, 0);
                        global.localplayer.setComponentVariation(3, ClothesData[gender][2][Number (_drawable)][2], 0, 0);
                    } else                     
                        global.localplayer.setComponentVariation(Number (_id), Number (_drawable), Number (_texture), 0);                    
                    break;
                case 4:
                    selectComponent = 3;                    
                    if (ClothesData[gender][selectComponent][Number (_drawable)]) {
                        const texture = ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] ? ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] : Number (_texture);
                        global.localplayer.setComponentVariation(Number (_id), ClothesData[gender][selectComponent][Number (_drawable)][1], texture, 0);
                    }else                     
                        global.localplayer.setComponentVariation(Number (_id), Number (_drawable), Number (_texture), 0); 
                    break;
                case 6:
                    selectComponent = 4;
                    if (ClothesData[gender][selectComponent][Number (_drawable)]) {
                        const texture = ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] ? ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] : Number (_texture);
                        global.localplayer.setComponentVariation(Number (_id), ClothesData[gender][selectComponent][Number (_drawable)][1], texture, 0);
                    }
                    else                     
                        global.localplayer.setComponentVariation(Number (_id), Number (_drawable), Number (_texture), 0); 
                    break;
                case 7:
                    selectComponent = 7;
                    
                    if (ClothesData[gender][selectComponent][Number (_drawable)]) {
                        const texture = ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] ? ClothesData[gender][selectComponent][Number (_drawable)][4][Number (_texture)] : Number (_texture);
                        global.localplayer.setComponentVariation(Number (_id), ClothesData[gender][selectComponent][Number (_drawable)][1], texture, 0);
                    }else                     
                        global.localplayer.setComponentVariation(Number (_id), Number (_drawable), Number (_texture), 0); 
                    break;*/
                default:
                    selectComponent = "";
                    global.localplayer.setComponentVariation(Number (_id), Number (_drawable), Number (_texture), 0);
                    break;
            }
        }
        else global.localplayer.setPropIndex(Number (_id), Number (_drawable), Number (_texture), true);
        if (drawable == _id) return;
        drawable = _id;
        if (_props || (!_props && _id !== 3 && _id !== 8)) clearClothesCreator ();
        if (!_props && (_id == 1 || _id == 2)) global.updateCameraToBone ("hat");
        else if (_props && (_id == 0 || _id == 1 || _id == 2 || _id == 1 || _id == 1)) global.updateCameraToBone ("hat");
        else if (!_props && (_id == 7 || _id == 11 || _id == 5)) global.updateCameraToBone ("top");
        else if (_props && (_id == 7 || _id == 6)) global.updateCameraToBone ("top");
        else if (!_props && (_id == 4)) global.updateCameraToBone (3);
        else if (!_props && (_id == 6)) global.updateCameraToBone (4);
    } catch(e) {

        mp.events.call('notify', 4, 9, 'clothesEditor ' + e.name + ":" + e.message, 3000);
  
    }
});

let prefixTops = ""
gm.events.add("render", () => {
    if (!global.loggedin) return;
    if (openClothesEditor && ClothesData[gender] && ClothesData[gender][selectComponent] && ClothesData[gender][selectComponent][selectDrawable]) {
        const cloth = ClothesData[gender][selectComponent][selectDrawable];
        const texture = cloth[4][selectTexture] ? cloth[4][selectTexture] : selectTexture;
        if (selectComponent == 1)
            prefixTops = `\n/additem -11 1 ${selectDrawable}_${texture}_${gender ? 'True' : 'False'}`
        else if (selectComponent == 2)
            prefixTops = `\n/additem -8 1 ${selectDrawable}_${texture}_${gender ? 'True' : 'False'}`
        else if (selectComponent == 3)
            prefixTops = `\n/additem -4 1 ${selectDrawable}_${texture}_${gender ? 'True' : 'False'}`
        else if (selectComponent == 4)
            prefixTops = `\n/additem -6 1 ${selectDrawable}_${texture}_${gender ? 'True' : 'False'}`
        else
            prefixTops = ``

        if (cloth[5] > 0) {
            mp.game.graphics.drawText(`ID #${cloth[0]}\nВ магазине ${cloth[5] ? "Есть" : translateText("Нет")} - Цена: ${cloth[5]}$` + prefixTops, [0.5, 0.8], {
                font: 0,
                color: [255, 255, 255, 200],
                scale: [0.35, 0.35],
                outline: true
            });
        } else if (cloth[6] > 0) {
            mp.game.graphics.drawText(`ID #${cloth[0]}\nВ магазине ${cloth[6] ? "Есть" : translateText("Нет")} - Цена: ${cloth[6]}$` + prefixTops, [0.5, 0.8], {
                font: 0,
                color: [255, 255, 255, 200],
                scale: [0.35, 0.35],
                outline: true
            });
        }
    }
});

gm.events.add('client.clothesEditor.gender', (_gender) => {
    try
    {
        if (_gender === "men") {
            gender = 1;
            global.localplayer.model = mp.game.joaat('mp_m_freemode_01');
        } else {
            gender = 0;
            global.localplayer.model = mp.game.joaat('mp_f_freemode_01');
        }
        clearClothesCreator ();
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "shop/clothes/clothesEditor", "client.clothesEditor.gender", e.toString());
    }
});

function clearClothesCreator() {
    global.localplayer.clearProp(0);
    global.localplayer.clearProp(1);
    global.localplayer.clearProp(2);
    global.localplayer.clearProp(6);
    global.localplayer.clearProp(7);

    global.localplayer.setComponentVariation(1, clothesEmpty[gender][1], 0, 0);
    global.localplayer.setComponentVariation(3, clothesEmpty[gender][3], 0, 0);
    global.localplayer.setComponentVariation(4, clothesEmpty[gender][4], 0, 0);
    global.localplayer.setComponentVariation(5, clothesEmpty[gender][5], 0, 0);
    global.localplayer.setComponentVariation(6, clothesEmpty[gender][6], 0, 0);
    global.localplayer.setComponentVariation(7, clothesEmpty[gender][7], 0, 0);
    global.localplayer.setComponentVariation(8, clothesEmpty[gender][8], 0, 0);
    global.localplayer.setComponentVariation(9, clothesEmpty[gender][9], 0, 0);
    global.localplayer.setComponentVariation(10, clothesEmpty[gender][10], 0, 0);
    global.localplayer.setComponentVariation(11, clothesEmpty[gender][11], 0, 0);
}