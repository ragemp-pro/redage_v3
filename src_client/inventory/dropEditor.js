global.dropEditor = false;

var resDrop = mp.game.graphics.getScreenActiveResolution(0, 0);

let MOVE_SENSITIVTY_DROP = 50;
let ROT_SENSITIVITY_DROP = 800;

let selObjDrop = null;
let oldPosDrop;
let oldRotDrop;
let modeDrop = 'Move';
let curBtnDrop;
let oldcursorPosDrop = [0, 0];

let xboxDrop;
let yboxDrop;
let zboxDrop;

let itemData = null;
const fireworkData = [
	-1611832715,
	-879052345,
	-1118757580,
	-1502580877]
	
gm.events.add('client.inventory.objecteditor', (objid, arrayName, index) => {
	try
	{
		global.binderFunctions.GameMenuClose ();
		global.OnObjectEditor (objid, null, (pos, rot, _) => {
			if (fireworkData.includes(objid) && global.isInSafeZone)
				return mp.events.call('notify', 1, 9, translateText("Фейерверк нельзя ставить в зеленой зоне"), 3000);
				
			mp.events.callRemote('server.dropeditor.finish', arrayName, index, pos.x, pos.y, pos.z, 0, 0, rot.z);
		})
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "inventory/dropEditor", "client.inventory.objecteditor", e.toString());
	}
});
