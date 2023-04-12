const nameProp = "mj_xm_box2"

let eventsData = {};

gm.events.add('client.events.open', (json) => {
	try
	{
		mp.gui.emmit(`window.serverStore.isEvent (true)`);
        json = JSON.parse (json);
        json.forEach((item, index) => {
            if (!eventsData [index]) eventsData [index] = {};
            eventsData [index].Position = item.Position;
            
            eventsData [index].object = mp.objects.new(mp.game.joaat(nameProp), new mp.Vector3(item.Position.x, item.Position.y, item.Position.z), {
                rotation: new mp.Vector3(0, 0, item.Rotation.z),
                dimension: 0
            });

            eventsData [index].shape = mp.colshapes.newSphere(
                item.Position.x,
                item.Position.y,
                item.Position.z,
                1.5
            );

            eventsData [index].shape.festive = true;
            eventsData [index].shape.fId = item.Id;
            eventsData [index].shape.fIndex = index;
            
            eventsData [index].shapeMusic = mp.colshapes.newSphere(
                item.Position.x,
                item.Position.y,
                item.Position.z,
                10.0
            );
            
            eventsData [index].shapeMusic.eventMusicID = index;
            eventsData [index].shapeMusic.eventMusic = true;
        })
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/festive", "client.events.open", e.toString());
    }
})

gm.events.add("pedStreamIn", (entity) => {
    if (!global.loggedin)
        return;

    if (entity.eIndex) {
        const ped = PedsData [entity.eIndex - 1];
        global.requestAnimDict(ped.animDictionary).then(() => {
            entity.taskPlayAnim (ped.animDictionary, ped.animationName, 8, 1, -1, Number(ped.type), 0, false, false, false);
        });
    }

});

global.selectFestive = -1;
let selectMusic = -1;

global.getRandomInt = (min, max) => {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

const MPWaypointsInfo = {
    blips: {},
    colshapes: {},
    countToDelete: {}
}

gm.events.add('createMPWaypoint', (adminValue, posX, posY, adminDimension) => {
    try
	{
        if (MPWaypointsInfo.blips[adminValue] !== undefined) {
            MPWaypointsInfo.blips[adminValue].setRoute(false);
			MPWaypointsInfo.blips[adminValue].destroy();
			MPWaypointsInfo.blips[adminValue] = undefined;
        }

        if (MPWaypointsInfo.colshapes[adminValue] !== undefined) {
			MPWaypointsInfo.colshapes[adminValue].destroy();
			MPWaypointsInfo.colshapes[adminValue] = undefined;
		}

		MPWaypointsInfo.blips[adminValue] = mp.blips.new(545, new mp.Vector3(posX, posY), { alpha: 255, color: 1, name: translateText("Метка МП"), dimension: adminDimension });
        MPWaypointsInfo.blips[adminValue].setRoute(true);
		MPWaypointsInfo.blips[adminValue].setRouteColour(1);

		MPWaypointsInfo.colshapes[adminValue] = mp.colshapes.newCircle(posX, posY, 10, adminDimension);
        MPWaypointsInfo.colshapes[adminValue].indexMP = adminValue;

        if (global.localplayer.remoteId == adminValue) {
            MPWaypointsInfo.countToDelete[adminValue] = 1;
        } else {
            MPWaypointsInfo.countToDelete[adminValue] = 0;
        }
	}
	catch (e)
    {
        mp.events.callRemote("client_trycatch", "events/festive", "createMPWaypoint", e.toString());
    }
});

gm.events.add('playerEnterColshape', (shape) => {
	try
	{
        if (shape && shape.indexMP !== undefined) {
            var valueToRemove = shape.indexMP;
            if (valueToRemove >= 0 && (!MPWaypointsInfo.countToDelete[valueToRemove] || (MPWaypointsInfo.countToDelete[valueToRemove] - 1) <= 0)) {
                MPWaypointsInfo.countToDelete[valueToRemove] = 0;

                if (MPWaypointsInfo.blips[valueToRemove] !== undefined) {
                    MPWaypointsInfo.blips[valueToRemove].setRoute(false);
                    MPWaypointsInfo.blips[valueToRemove].destroy();
                    MPWaypointsInfo.blips[valueToRemove] = undefined;
                }
        
                if (MPWaypointsInfo.colshapes[valueToRemove] !== undefined) {
                    MPWaypointsInfo.colshapes[valueToRemove].destroy();
                    MPWaypointsInfo.colshapes[valueToRemove] = undefined;
                }
            }
        }

		if (shape && shape.festive !== undefined && shape.fId !== undefined && global.selectFestive == -1) {
			mp.events.call('hud.oEnter', 1);
            global.selectFestive = {
                fId: shape.fId,
                fIndex: shape.fIndex
            }
            
		} else if (shape && shape.eventMusic && selectMusic == -1) {
			if (getRandomInt(1, 10) > 8) mp.events.call("client.particleEffect", "scr_indep_fireworks", "scr_indep_firework_shotburst", eventsData [shape.eventMusicID].Position, 5000);
			mp.events.call("sounds.playInterface", "cloud/sound/festive/put2.ogg", 50);
			selectMusic = shape.eventMusicID;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/festive", "playerEnterColshape", e.toString());
    }
});

gm.events.add('playerExitColshape', (shape) => {
	try
	{
		if (shape && shape.festive !== undefined && shape.fId !== undefined && global.selectFestive !== -1 && global.selectFestive.fId === shape.fId) {
			mp.events.call('hud.cEnter');
			global.selectFestive = -1;
		} else if (shape && shape.eventMusic && selectMusic == shape.eventMusicID) {
			global.StopSound ("cloud/sound/festive/put2.ogg");
			selectMusic = -1;
		}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/festive", "playerExitColshape", e.toString());
    }
});

global.lastCheckKeyToEvents = 0;

gm.events.add('client.events.confirming', async (count, countItems, countMaxFestive) => {
	try
	{
		global.lastCheckKeyToEvents = new Date().getTime();
        let prefix = "";
        if (count === 2 || count === 3 || count === 4) prefix = translateText("а");
        else if (count > 4) prefix = translateText("ов");
        mp.gui.chat.push(translateText("!{#FFA500}Вы собрали {0} подарков из {1} возможных. Вы на верном пути!", countItems, countMaxFestive));
        mp.events.call('hud.event.cool', "Мероприятие", "Ого!", translateText("Вы собрали секретный подарок и получили {0} коин{1}!", count, prefix), "png", 10000);
        if (countItems !== countMaxFestive) mp.events.call('hud.event.cool', "Мероприятие", "Ого!", translateText("Отлично! Вам осталось собрать {0} подарков. Продолжайте поиски!", countMaxFestive - countItems), "png", 10000);
        else mp.events.call('hud.event.cool', "Мероприятие", "Ого!", translateText("Поздравляем, вы собрали абсолютно все существующие подарков на карте. Вы действительно чемпион с сильными нервами!"), "png", 10000);
        global.StopSound ("cloud/sound/festive/put.ogg");
        mp.events.call("sounds.playInterface", "cloud/sound/festive/bring2.ogg", 50);
        if (global.selectFestive !== -1) {
            const eData = eventsData [global.selectFestive.fIndex];
            if (eData) {
                mp.events.call("client.particleEffect", "scr_indep_fireworks", "scr_indep_firework_fountain", eData.Position, 7500);
                if (eData.shape)
                    eData.shape.destroy();
                
                if (eData.object && mp.objects.exists(eData.object))
                    eData.object.destroy();
                        
                delete eventsData [global.selectFestive.fIndex];
            }
        }
        mp.events.call('hud.cEnter');
        global.selectFestive = -1;
        selectMusic = -1;
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/festive", "client.events.confirming", e.toString());
    }
})

let eventsRefresh = new Date().getTime();
gm.events.add('client.events.buyItem', (index) => {
	try
	{
		if (new Date().getTime() - eventsRefresh < 1000) {
			mp.events.call('notify', 4, 9, translateText("Слишком быстро"), 3000);
			return;
		}
		eventsRefresh = new Date().getTime();
		mp.events.callRemote('sever.events.buyItem', index)
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/festive", "client.events.buyItem", e.toString());
    }
})

gm.events.add('EndEvent', () => {
    try
	{
		mp.gui.emmit(`window.serverStore.isEvent (false)`);
        for(let key in eventsData) {
            const eData = eventsData [key];
            if (eData) {
                if (eData.shape)
                    eData.shape.destroy();
                
                if (eData.object && mp.objects.exists(eData.object))
                    eData.object.destroy();
            }
        }
        eventsData = {}
	}
	catch (e) 
    {
        mp.events.callRemote("client_trycatch", "events/festive", "client.events.buyItem", e.toString());
    }
})