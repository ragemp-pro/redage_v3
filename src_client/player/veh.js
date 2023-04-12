mp.game.controls.useDefaultVehicleEntering = false;

const MAX_DIST_ENTER = 10.0;
global.VehicleSeatFix = 0;

const boneArray = [
    "seat_dside_f",//Передняя левая
    "seat_pside_f",//Передняя правая
    "seat_dside_r",//Передняя правая
    "seat_pside_r",//Задняя правая
    "seat_dside_r1",
    "seat_pside_r1",
    "seat_pside_r2",
    "seat_dside_r2",
    "seat_dside_r3",
    "seat_pside_r3",
    "seat_dside_r4",
    "seat_pside_r4",
    "seat_dside_r5",
    "seat_pside_r5",
    "seat_dside_r6",
    "seat_pside_r6",
    "seat_dside_r7",
    "seat_pside_r7"
];

const PutToPlayerInVehicleStatus = {
    none: -1,
    get: 0,
    drive: 1,
    driveUp: 2,
    passenger: 3,
    passengerUp: 4,
};

class PutToPlayerInVehicle {

    constructor(dist) {
      this.maxDistEnter = dist;
      this.inVehicle = null;
      this.doorsId = -1;
      this.dist = dist;
      this.status = PutToPlayerInVehicleStatus.none;
      this.intervalId = null;
      this.timerId = null;
      this.antiSpam = 0;
      this.time = 0;
      this.delay = 0;
      this.SitDist = 0;
      this.SitTime = 0;
      this.SitToggle = 0;
    }

    clear (seat = false) {
		try
		{
			if (this.status === PutToPlayerInVehicleStatus.none) return;
			if (seat) global.VehicleSeatFix = new Date().getTime() + 250;
			else global.VehicleSeatFix = 0;
			this.delInterval ();
			this.delTimer ();

			this.status = PutToPlayerInVehicleStatus.none;
			this.dist = this.maxDistEnter;
			//this.inVehicle = null;
			this.SitToggle = 0;
			this.delay = 0;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "clear", e.toString());
		}
    }

    delInterval () {
		try
		{
			if (this.intervalId !== null) {
				clearInterval(this.intervalId);
				this.intervalId = null
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "delInterval", e.toString());
		}
    }

    delTimer () {
		try
		{
			if (this.timerId !== null) {
				clearTimeout(this.timerId);
				this.timerId = null
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "delTimer", e.toString());
		}
    }

    stop (seat = false) {
		try
		{
			if (this.status === PutToPlayerInVehicleStatus.none) return;
			else if (this.delay > new Date().getTime()) return;
			
			this.clear(seat);

			global.localplayer.taskEnterVehicle(-1, 0, -1, 3.0, 3, 1);
			global.localplayer.clearTasks();
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "stop", e.toString());
		}
    }

    distance(var1,var2) {
        var dx = var1.x - var2.x;
        var dy = var1.y - var2.y;
        var dz = var1.z - var2.z;
        return Math.sqrt( dx * dx + dy * dy + dz * dz );
    }

    getVehicle () {
		try
		{
			const playerPos = global.localplayer.position;	
			let endDist;
			mp.vehicles.forEachInStreamRange(vehicle => {
				endDist = this.getVehicleDoor(vehicle, playerPos);
				if(endDist < this.maxDistEnter && vehicle.isDriveable(false) && mp.vehicles.exists(vehicle) && 2 !== vehicle.getDoorLockStatus()) {
					if(endDist < this.dist) {
						this.inVehicle = vehicle;
						this.dist = endDist;
						endDist = null;
					}
				}
			});
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "getVehicle", e.toString());
		}
    }

    getVehicleDoor (vehicle, playerPos) {
		try
		{
			const doorsBone = [
				"door_dside_f",
				"door_dside_r",
				"door_pside_f",
				"door_pside_r"
			];
			let returnDist = this.distance(playerPos, vehicle.position);
			let endDist = 0;

			doorsBone.forEach(item => {
				endDist = this.distance(playerPos, vehicle.getWorldPositionOfBone(vehicle.getBoneIndexByName(item)));
				if(endDist < returnDist) returnDist = endDist;
			});
			return returnDist;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "getVehicleDoor", e.toString());
			return 0;
		}
    }

    getVehicleDoorPos (vehicle, playerPos) {
		try
		{
			const doorsBone = [
				"door_dside_f",
				"door_dside_r",
				"door_pside_f",
				"door_pside_r"
			];
			let returnPos = vehicle.position;
			let endPos = 0;
			let dist = this.distance(playerPos, returnPos);
			let endDist = 0;

			doorsBone.forEach(item => {
				endPos = vehicle.getWorldPositionOfBone(vehicle.getBoneIndexByName(item));
				endDist = this.distance(playerPos, endPos);
				if(endDist < dist) {
					dist = endDist;
					returnPos = endPos;
				}
			});
			return returnPos;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "getVehicleDoorPos", e.toString());
			return global.localplayer.position;
		}
    }

    getUnBind() {
		try
		{
			this.status = PutToPlayerInVehicleStatus.get;
			this.timerId = setTimeout(() => {
				this.timerId = null;
				this.status = PutToPlayerInVehicleStatus.passenger;
				this.enter (false);
			}, 1000);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "getUnBind", e.toString());
		}
    }

    up () {
		try
		{
			if(global.localplayer.getIsTaskActive(160) && this.inVehicle != null && mp.vehicles.exists(this.inVehicle) && this.distance(this.getVehicleDoorPos(this.inVehicle, global.localplayer.position), global.localplayer.position) > 2.0)
			{
				if(global.localplayer.isInWater()) this.stop();
				else {
					if (this.inVehicle.getPedInSeat(this.doorsId) !== 0 && this.inVehicle.getPedInSeat(this.doorsId) !== global.localplayer.handle) this.stop();
					else {
						this.status = this.status === PutToPlayerInVehicleStatus.passenger ? PutToPlayerInVehicleStatus.passengerUp : PutToPlayerInVehicleStatus.driveUp;
						if(this.doorsId < 4) global.localplayer.taskEnterVehicle(this.inVehicle.handle, -1, this.doorsId, 3.0, 1, 0);
						else global.localplayer.taskEnterVehicle(this.inVehicle.handle, -1, -2, 3.0, 1, 0);
					}
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "up", e.toString());
		}
    }

    enter (inDrive) {
		try
		{
			if (this.status === PutToPlayerInVehicleStatus.passenger && inDrive) return;
			else if (this.status === PutToPlayerInVehicleStatus.drive && !inDrive) return;
			else if(this.inVehicle != null && mp.vehicles.exists(this.inVehicle) && this.dist < this.maxDistEnter && this.inVehicle.getSpeed() < 6.0) 
			{
				let myanim = global.localplayer.getVariable('ANIM_USE');
				if(myanim != undefined && myanim != "null") 
				{
					this.clear ();
					mp.events.call('notify', 1, 9, translateText("Чтобы сесть в транспортное средство Вам нужно отменить анимацию."), 3000);
				}
				else if (2 == this.inVehicle.getDoorLockStatus()) {
					this.clear ();
					mp.events.call('notify', 1, 9, translateText("Двери закрыты"), 1000);
				}
				else if (inDrive && (this.inVehicle.isSeatFree(-1) || this.inVehicle.getPedInSeat(-1) == global.localplayer.handle || 0 == this.inVehicle.getPedInSeat(-1))) {
					this.SitToggle = false;
					this.status = PutToPlayerInVehicleStatus.drive;
					this.doorsId = -1;
					this.delay = new Date().getTime() + 200;
					global.VehicleSeatFix = new Date().getTime() + 1000 * 30;
					global.localplayer.taskEnterVehicle(this.inVehicle.handle, -1, this.doorsId, (global.localplayer.isInWater())?(2.0):(1.0), 1, 0);
				} else {
					const playerPos = global.localplayer.position;
					this.doorsId = -1;
					let doorDist = this.maxDistEnter;
					for(var i = 0; i < this.inVehicle.getMaxNumberOfPassengers(); i++) {
						if(this.inVehicle.isSeatFree(i)) {
							if (boneArray[i + 1]) {
								let endDist = this.distance(this.inVehicle.getWorldPositionOfBone(this.inVehicle.getBoneIndexByName(boneArray[i + 1])), playerPos);
								if (endDist < doorDist) {                            
									doorDist = endDist;
									this.doorsId = i;
								} else if (i === (this.inVehicle.getMaxNumberOfPassengers() - 1) && this.doorsId === -1) this.doorsId = i;
							} else if (this.doorsId === -1) {
								doorDist = this.distance(this.inVehicle.position, playerPos);
								this.doorsId = i;
								break;
							} else if (this.doorsId !== -1) break;
						}
					}
					if(this.doorsId === -1) {
						this.stop ();
						mp.events.call('notify', 1, 9, translateText("Мест нет"), 1000);
					} else if(this.doorsId !== -1) {
						global.localplayer.clearTasks();
						this.SitToggle = 0;
						this.status = PutToPlayerInVehicleStatus.passenger;
						this.delay = this.SitTime = new Date().getTime() + 100;    
						this.SitDist = global.localplayer.position;
						global.localplayer.taskEnterVehicle(this.inVehicle.handle, -1, this.doorsId, (global.localplayer.isInWater())?(2.0):(1.0), 1, 0);
					}
				}
			} else
				this.clear();
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "enter", e.toString());
		}
    }

    Interval () {
		try
		{
			if (global.localplayer.isSittingInAnyVehicle()) this.antiSpam = new Date().getTime() + 250;
			if (this.status === PutToPlayerInVehicleStatus.none && this.status === PutToPlayerInVehicleStatus.get && global.localplayer.getIsTaskActive(160)) {
				global.localplayer.taskEnterVehicle(-1, 0, -1, 3.0, 3, 1);
				global.localplayer.clearTasks();
				global.VehicleSeatFix = 0;
			}
			else if (this.status === PutToPlayerInVehicleStatus.none || this.status === PutToPlayerInVehicleStatus.get) return;
			else if (this.inVehicle === null || !mp.vehicles.exists(this.inVehicle)) this.stop();
			else if (global.localplayer.vehicle && global.localplayer.isSittingInAnyVehicle() && this.doorsId === -1 && this.inVehicle.getPedInSeat(this.doorsId) !== 0 && this.inVehicle.getPedInSeat(this.doorsId) === global.localplayer.handle) this.stop(true);
			else if (this.inVehicle.getPedInSeat(this.doorsId) !== 0 && this.inVehicle.getPedInSeat(this.doorsId) !== global.localplayer.handle) this.stop();
			else if (this.inVehicle.getDoorLockStatus() === 2) this.stop();
			else if (this.inVehicle !== null && this.distance(this.getVehicleDoorPos(this.inVehicle, global.localplayer.position), global.localplayer.position) > this.maxDistEnter) this.stop();
			else if ((this.status === PutToPlayerInVehicleStatus.passenger || this.status === PutToPlayerInVehicleStatus.passengerUp) && global.localplayer.getIsTaskActive(160) && this.SitToggle === 1 && this.distance(this.getVehicleDoorPos(this.inVehicle, global.localplayer.position), global.localplayer.position) < 1) {
				this.stop();
				const handle = this.inVehicle.handle;
				const doorsId = this.doorsId;
				global.localplayer.taskWarpIntoVehicle(handle, doorsId);  
			} else if ((this.status === PutToPlayerInVehicleStatus.passenger || this.status === PutToPlayerInVehicleStatus.passengerUp) && 
			  this.SitToggle === 0 &&
			  global.localplayer.getIsTaskActive(160) &&
			  (global.localplayer.getIsTaskActive(35) || global.localplayer.getIsTaskActive(195))) {
				this.SitToggle = -1;
			} else if ((this.status === PutToPlayerInVehicleStatus.passenger || this.status === PutToPlayerInVehicleStatus.passengerUp) && 
			this.SitTime < new Date().getTime() &&
			this.SitToggle === 0 &&
			global.localplayer.getIsTaskActive(160) &&
			(!global.localplayer.getIsTaskActive(35) && !global.localplayer.getIsTaskActive(195))) {
			  this.SitToggle = 1;
			  if (this.distance(this.getVehicleDoorPos(this.inVehicle, global.localplayer.position), global.localplayer.position) > 2.0) global.localplayer.taskEnterVehicle(this.inVehicle.handle, -1, -2, (global.localplayer.isInWater())?(2.0):(1.0), 1, 0);
			  else {
				  this.stop();
				  const handle = this.inVehicle.handle;
				  const doorsId = this.doorsId;
				  global.localplayer.taskWarpIntoVehicle(handle, doorsId);  
			  }
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "Interval", e.toString());
		}
    }

    bind (toggled) {
		try
		{
			if (!global.localplayer.isSittingInAnyVehicle() && this.antiSpam < new Date().getTime()) {
				if (toggled) {
					if (this.status === PutToPlayerInVehicleStatus.get) this.clear();
					if (this.status === PutToPlayerInVehicleStatus.none) {
						this.clear();
						this.getVehicle ();
						this.getUnBind ();
					}
					else if (this.status === PutToPlayerInVehicleStatus.passengerUp || this.status === PutToPlayerInVehicleStatus.driveUp)
						this.stop ();
					else if (this.status === PutToPlayerInVehicleStatus.drive || this.status === PutToPlayerInVehicleStatus.passenger)
						this.up ();
				} else if (this.status === PutToPlayerInVehicleStatus.get) {
					this.delTimer ();
					this.status = PutToPlayerInVehicleStatus.drive;
					this.enter (true);
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "player/veh", "bind", e.toString());
		}
    }
}

let putToPlayerInVehicle = new PutToPlayerInVehicle(MAX_DIST_ENTER);

let AttachToVehicleTime = 0;
var vehenterallow = true;
mp.keys.bind(70, true, () => {
    if(global.loggedin === true && !global.freeze && !global.menuCheck () && global.chatActive === false/* && global.showhud*/ && global.ANTIANIM != true && !global.attachedtotrunk && !global.localplayer.carryngData && vehenterallow === true) {
        putToPlayerInVehicle.bind (true);
    }
});

mp.keys.bind(70, false, () => {
    if(global.loggedin === true && !global.menuCheck () && global.chatActive === false/* && global.showhud*/ && global.ANTIANIM != true && !global.attachedtotrunk && !global.localplayer.carryngData) {
        putToPlayerInVehicle.bind (false);
    } else if (global.loggedin === true && !global.menuCheck () && global.chatActive === false/* && global.showhud*/ && AttachToVehicleTime < new Date().getTime() && global.attachedtotrunk) {        
		AttachToVehicleTime = new Date().getTime() + 1500;
        mp.events.callRemote('server.vehicle.detachPlayer');
    }
});

gm.events.add("VehicleEnterToggle", (allow) => {
    vehenterallow = allow;
});

gm.events.add("render", () => {
	try 
	{
		if (!global.loggedin) return;
		mp.game.controls.disableControlAction(0, global.Inputs.THROW_GRENADE, true);	
		mp.game.controls.disableControlAction(0, global.Inputs.ENTER, true);
		if(global.cuffed) mp.game.controls.disableControlAction(0, global.Inputs.VEH_EXIT, true);

		putToPlayerInVehicle.Interval ();

		if(vehenterallow === false || 
			mp.game.controls.isControlPressed(0, global.Inputs.MOVE_UP_ONLY) || 
			mp.game.controls.isControlPressed(0, global.Inputs.MOVE_DOWN_ONLY) || 
			mp.game.controls.isControlPressed(0, global.Inputs.MOVE_LEFT_ONLY) || 
			mp.game.controls.isControlPressed(0, global.Inputs.MOVE_RIGHT_ONLY) ||
			mp.game.controls.isControlPressed(0, global.Inputs.JUMP)) {
				if (putToPlayerInVehicle.status !== PutToPlayerInVehicleStatus.none && putToPlayerInVehicle.status !== PutToPlayerInVehicleStatus.get) putToPlayerInVehicle.stop ();
			}
		else if ((putToPlayerInVehicle.status === PutToPlayerInVehicleStatus.drive || putToPlayerInVehicle.status === PutToPlayerInVehicleStatus.passenger) && mp.game.controls.isControlPressed(0, global.Inputs.SPRINT)) putToPlayerInVehicle.up ();
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["player/veh"] < 60000) return;
		global.trycatchtime["player/veh"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/veh", "render", e.toString());
	}
});