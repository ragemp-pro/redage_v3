global.objectsItemsStreamed = new Set();

mp.objects.forEachInStreamRangeItems = call => {
    const objects = [...global.objectsItemsStreamed.values()];
    for (let i = 0; i < objects.length; i++) {
        const object = objects[i];
        if (mp.objects.exists(object)) {
            try {
                call(object);
            } catch (e) {
            }
        } else
            global.objectsItemsStreamed.delete(object);
    }
}

gm.events.add("playerReady", () => {
    try {
        let value;
        mp.objects.forEach(object => {
            if (object.hasVariable('furniture')) {
                object.notifyStreaming = true;
                
                addObjectFurnitureData (object);
            } else if (value = object.getVariable('DropData')) {
                object.notifyStreaming = true;
        
                addObjectDropData (object, value);
            }/* else if (object.hasVariable('objectInvisible')) {
                object.notifyStreaming = true;
                object.objectInvisible = true;

                setInvisible (object);
            }*/
        });
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/stream", "playerReady", e.toString());
    }
});

//******************************** */

mp.events.addDataHandler("DropData", (object, value) => {
    if (object && mp.objects.exists(object)) {
        object.notifyStreaming = true;
        
        addObjectDropData (object, value);
    }
});

const addObjectDropData = (object, value) => {
    try {
        if (object) {
            object.dropData = JSON.parse (value);

            if (object.handle) {
                object.setCollision(false, false);

                addObjectData (object);
            }
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/stream", "addObjectDropData", e.toString());
    }
}

//******************************** */

mp.events.addDataHandler("furniture", object => {
    if (object && mp.objects.exists(object)) {
        object.notifyStreaming = true;
        
        addObjectFurnitureData (object);
    }
});

const addObjectFurnitureData = (object) => {
    try {
        if (object) {
            object.furniture = true;

            addObjectData (object);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/stream", "addObjectFurnitureData", e.toString());
    }
}

//******************************** */
/*mp.events.addDataHandler("objectInvisible", (object, value) => {
    if (object && mp.objects.exists(object)) {
        if (value) {
            object.notifyStreaming = true;
            object.objectInvisible = true;
			setInvisible (object);
        } else {
            delete object.notifyStreaming;
            delete object.objectInvisible;
			delInvisible (object);
        }
    }
});

const setInvisible = (object) => {
    if (object && object.handle) {
        object.setVisible(false, false);
        object.setAlpha(0);
    }
}

const delInvisible = (object) => {
    if (object && object.handle) {
        object.setVisible(true, false);
        object.setAlpha(255);
    }
}*/

//******************************** */

const addObjectData = (object) => {
    try {

        if (object && object.handle && !global.objectsItemsStreamed.has (object)) {
            global.objectsItemsStreamed.add (object);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/stream", "addObjectData", e.toString());
    }
}

const delObjectData = (object) => {
    try {

        if (object && mp.objects.exists(object) && global.objectsItemsStreamed.has (object)) {
            global.objectsItemsStreamed.delete (object);
        }
    }
    catch (e) 
    {
        mp.events.callRemote("client_trycatch", "world/stream", "addObjectData", e.toString());
    }
}

//******************************** */


gm.events.add("objectStreamIn", (entity) => {
    if (entity.furniture)
        addObjectData(entity);
    else if (entity.dropData) {
        entity.setCollision(true, false);
        addObjectData(entity);
    }
});

gm.events.add("objectStreamOut", (entity) => {
    delObjectData (entity);
});