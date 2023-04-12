const attachToBed = async (flatbed, targetVeh) => {
    if (!flatbed.handle || (targetVeh !== false && !targetVeh.handle)) return
  
    if (targetVeh === false) {
      	flatbed.attachedVehicle.detach(true, false)
      	delete flatbed.attachedVehicle
  
    } else {
        flatbed.freezePosition(true)
        let height = getVehicleHeight (targetVeh);
        targetVeh.freezePosition(false)
          
        targetVeh.attachTo(flatbed.handle, flatbed.getBoneIndexByName('chassis'), 0, -3, height, 0, 0, 180, true, false, true, false, 0, true);
      	flatbed.attachedVehicle = targetVeh;
        
        await global.isAttached(targetVeh);
        //await global.wait (250);

        flatbed.freezePosition(false)
    }
}

mp.events.addDataHandler("fbAttach", (entity, value, oldValue) => {
	try
	{
		if (entity && mp.vehicles.exists(entity) && entity.type === 'vehicle' && entity.model == mp.game.joaat('flatbed')) {
			if (typeof value === "string" && global.IsJsonString (value)) {
                entity.fbAttach = value;
                if (entity.handle !== 0) 
                    createVehicle (entity, value);
            } else {
                if (entity.fbAttach) {
					deleteVehicle (entity);
                    delete entity.fbAttach;
                }
            }
		}
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "synchronization/state", "AGM", e.toString());
	}
});

gm.events.add("vehicleStreamIn", (entity) => {
    if (entity.model == mp.game.joaat('flatbed')) {

        if (entity.fbAttach)
            createVehicle (entity, entity.fbAttach);
    }
});

gm.events.add("vehicleStreamOut", (entity) => {
    if (entity.model == mp.game.joaat('flatbed') && entity.fbAttach) {
        deleteVehicle (entity);
    }
});

const deleteVehicle = async (entity) => {
    if (entity.attachedVehicle) {
        entity.attachedVehicle.destroy()
        delete entity.attachedVehicle;
    }
}


const createVehicle = async (flatbed, data) => {

	try {
        if (global.IsJsonString (data)) {
            data = JSON.parse(data);
            const position = flatbed.position;
            position.z += 5;
			const isLoad = await global.loadModel(data.Hash);
            const vehicle = mp.vehicles.new(isLoad ? data.Hash : "adder", position, { 
                numberPlate: "Evac",
                locked: true,
                engine: false,
                dimension: global.localplayer.dimension
            });
            vehicle.freezePosition(true)
            await global.IsLoadEntity (vehicle);
            vehicle.freezePosition(true)
            if (vehicle && mp.vehicles.exists(vehicle) && vehicle.type === 'vehicle' && vehicle.handle !== 0) {
				
                flatbed.attachedVehicle = vehicle;

                if (Number (data.PrimModColor) === -1 && Number (data.SecModColor) === -1) {
                    if (data.PrimColor != undefined && data.PrimColor.Red != undefined) 
						vehicle.setCustomPrimaryColour(Number (data.PrimColor.Red), Number (data.PrimColor.Green), Number (data.PrimColor.Blue));
                    if (data.SecColor != undefined && data.SecColor.Red != undefined) 
						vehicle.setCustomSecondaryColour(Number (data.SecColor.Red), Number (data.SecColor.Green), Number (data.SecColor.Blue));
                } else {
					vehicle.setColours(Number (data.PrimModColor), Number (data.SecModColor));
                }

                if (data.Spoiler != undefined) vehicle.setMod(0, Number (data.Spoiler));
                if (data.FrontBumper != undefined) vehicle.setMod(1, Number (data.FrontBumper));
                if (data.RearBumper != undefined) vehicle.setMod(2, Number (data.RearBumper));
                if (data.SideSkirt != undefined) vehicle.setMod(3, Number (data.SideSkirt));
                if (data.Muffler != undefined) vehicle.setMod(4, Number (data.Muffler));
                if (data.Frame != undefined) vehicle.setMod(5, Number (data.Frame));
                if (data.Lattice != undefined) vehicle.setMod(6, Number (data.Lattice));
                if (data.Hood != undefined) vehicle.setMod(7, Number (data.Hood));
                if (data.Wings != undefined) vehicle.setMod(8, Number (data.Wings));
                if (data.RWings != undefined) vehicle.setMod(9, Number (data.RWings));
                if (data.Roof != undefined) vehicle.setMod(10, Number (data.Roof));
                if (data.Vinyls != undefined) vehicle.setMod(48, Number (data.Vinyls));

                /*if (data.Engine != undefined) vehicle.setMod(11, Number (data.Engine));
                if (data.Turbo != undefined) vehicle.setMod(18, Number (data.Turbo));
                if (data.Transmission != undefined) vehicle.setMod(13, Number (data.Transmission));
                if (data.Suspension != undefined) vehicle.setMod(15, Number (data.Suspension));
                if (data.Brakes != undefined) vehicle.setMod(12, Number (data.Brakes));
                if (data.Horn != undefined) vehicle.setMod(14, Number (data.Horn));*/

                if (data.WindowTint != undefined) vehicle.setWindowTint(Number (data.WindowTint));
                if (data.NumberPlate != undefined) vehicle.setNumberPlateTextIndex(Number (data.NumberPlate));

                if (data.WheelsType != undefined) vehicle.setWheelType(Number (data.WheelsType));
                if (data.Wheels != undefined) vehicle.setMod(23, Number (data.Wheels));

                if (data.Cover) 
                {
                    vehicle.setModColor1(Number (data.Cover), 1, 0);
                    vehicle.setModColor2(Number (data.Cover), 1);
                }

                vehicle.setExtraColours(data.ColorAdditional === undefined ? 0 : Number (data.ColorAdditional),
                                        data.WheelsColor === undefined ? 0 : Number (data.WheelsColor));

				
				await global.wait (100);

                attachToBed (flatbed, vehicle)
            }
        }
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "vehicle/vehiclesync", "client.SetVehicleCustomization", e.toString());
	}
}
/*

gm.events.add('render', () => {
    mp.vehicles.forEachInStreamRange(vehicle => {
        if (vehicle && vehicle.handle !== 0) {
            const position = vehicle.position;
            
			const dimensions = mp.game.gameplay.getModelDimensions(vehicle.model);

            
            mp.game.graphics.drawText(translateText("Низ - {0}", Math.abs(dimensions.min.z)), [position.x, position.y, position.z + Math.abs(dimensions.min.z)], {
                scale: [0.3, 0.3],
                outline: true,
                color: [255, 255, 255, 150],
                font: 4
            });
            
            mp.game.graphics.drawText(translateText("Вверх - {0}", Math.abs(dimensions.max.z)), [position.x, position.y, position.z + Math.abs(dimensions.max.z)], {
                scale: [0.3, 0.3],
                outline: true,
                color: [255, 255, 255, 150],
                font: 4
            });
            
            let height = getVehicleHeight (vehicle);
            mp.game.graphics.drawText(`height - ${height}`, [position.x, position.y, position.z + height], {
                scale: [0.3, 0.3],
                outline: true,
                color: [255, 255, 255, 150],
                font: 4
            });
        }
    })
});*/

global.getVehicleHeight = (vehicle) => {
	try
	{
		if (vehicle && mp.vehicles.exists(vehicle)) 
		{
			const dimensions = mp.game.gameplay.getModelDimensions(vehicle.model);

            const MinAbs = Math.abs(dimensions.min.z);
            const MaxAbs = Math.abs(dimensions.max.z);

			return (MinAbs / MaxAbs) + (MaxAbs - MinAbs) - ((MaxAbs - MinAbs) * 0.4);
		}
		return 1; // Если вдруг какая-то ошибка, то стоит здесь что-то возвращать, пока хз что
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/circle", "getVehicleHeight", e.toString());
		return 1;
	}
}