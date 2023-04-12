import { slots_names, slots_positions, reels_offsets } from './cfg/main'
const topsComponentsToRemove = [ 3, 5, 7, 8, 9, 10, 11 ];

class Spin {

    constructor() {
        /** Общий массив со спинами */
        this.g_slots = [];
    
        /** Выбранный спин */
        this.select_spin = null;

        this.bet = 0;
    
        /** Ближайший автомат */
        this.nearest_spin = null;
    
        /** boolean, отвечающий за возможность крутить спин */
        this.can_bet = false;
    
        /** Отвечает за рендер от 1 лица */
        this.render_cam = false;
    
        /** Заморозка персонажа (фикс бага) */
        this.freeze_player = false;

        this.isSeat = false;

        this.init();
        
        this.removedClothing = [];

        gm.events.add('playerEnterColshape', (shape) => {
            this.on_enter_colshape(shape);
        });
        
        gm.events.add('playerExitColshape', (shape) => {
            this.on_leave_colshape(shape);
        });

        mp.keys.bind(0x45, true, () => {
            this.press_key_e();
        });

		gm.events.add('render', () => {
			if (!global.loggedin) return;
			this.render();
        });


        global.binderFunctions.spinExit = () => {
			try 
			{
				if(this.select_spin == null) return;
				else if(!this.can_bet || !this.isSeat || this.bet) return;
				if (this.removedClothing.length !== 0)  {
					for (const _clothes of this.removedClothing) {
						global.localplayer.setComponentVariation(_clothes._componentId, _clothes.drawable, _clothes.texture, _clothes.palette);
					}
				
					this.removedClothing = [];
				}
				global.cameraManager.stopCamera (true, 3000);
				this.render_cam = false;
				global.menuClose();
				mp.gui.emmit(`window.router.setHud();`);
				mp.events.callRemote('server.spin.LEAVE_SLOT');
				//await global.wait(1500);
				mp.game.cam.setFollowPedCamViewMode(2);            
				this.isSeat = false;
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "global.binderFunctions.spinExit", e.toString());
			}
        }

        gm.events.add('client.spin.OCCUPY_SLOT', async (nearest_spin) => {
			try 
			{
				this.select_spin = nearest_spin;
				if(this.select_spin == null) return;
				this.bet = 0;
				this.g_slots[this.select_spin].slot.setCollision(false, false);
				this.isSeat = false;
				await global.wait(3000);
				gm.discord(translateText("Играет в казино"));
				mp.gui.emmit(`window.router.setView("CasinoJacpot");`);
				global.menuOpen();

				global.createCamera ("spin", slots_positions[this.select_spin]);

				const gender = (global.GetGender (global.localplayer)) ? 1 : 0;
				this.removedClothing = [];
				for (const _componentId of topsComponentsToRemove) {
					const drawable = global.localplayer.getDrawableVariation(_componentId);
					const texture = global.localplayer.getTextureVariation(_componentId);
					const palette = global.localplayer.getPaletteVariation(_componentId);
			
					this.removedClothing.push({ _componentId, drawable, texture, palette });
			
					global.localplayer.setComponentVariation(_componentId, clothesEmpty[gender][_componentId] !== undefined ? clothesEmpty[gender][_componentId] : 0, 0, 0);
				}

				mp.game.audio.playSoundFromCoord(mp.game.invoke("0x430386FE9BF80B45"), "welcome_stinger", slots_positions[this.select_spin].x, slots_positions[this.select_spin].y, slots_positions[this.select_spin].z, "dlc_vw_casino_slot_machine_ds_npc_sounds", false, 20, false);
				
				this.can_bet = true;
				this.render_cam = true;
				mp.game.cam.setFollowPedCamViewMode(4);
				this.isSeat = true;
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "client.spin.OCCUPY_SLOT", e.toString());
			}
        });
        
        gm.events.add('client.spin.exit', () => {
			try 
			{
				global.binderFunctions.spinExit ();
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "client.spin.exit", e.toString());
			}
        });
        
        gm.events.add('client.spin.LEAVE_SLOT', (nearest_spin) => {
			try 
			{
				this.g_slots[nearest_spin].slot.setCollision(true, false);            
            
				this.clear_player_data();            
				this.nearest_spin = nearest_spin;
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "client.spin.LEAVE_SLOT", e.toString());
			}
        });

        gm.events.add('client.spin.SPIN', (r1, r2, r3) => {
			try 
			{
				this.spin(r1, r2, r3);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "client.spin.SPIN", e.toString());
			}
        });

        gm.events.add('client.spin.CLEAR_SPIN', () => {
			try 
			{
				if(this.select_spin == null) return;

				this.can_bet = true;
				this.bet = 0;
				mp.gui.emmit(`window.events.callEvent("cef.spin.btnExit", 1)`);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "client.spin.CLEAR_SPIN", e.toString());
			}
        });

        gm.events.add('client.spin.setBet', (value) => {
			try 
			{
				if(!this.can_bet) return;

				this.bet = value;
				mp.events.callRemote('server.spin.bet', this.bet);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/spin", "client.spin.setBet", e.toString());
			}
        });
    }

    getSounds (type) {
		try 
		{
			switch (type) {
				case 1:
					return "dlc_vw_casino_slot_machine_ak_player_sounds";
				case 2:
					return "dlc_vw_casino_slot_machine_ir_player_sounds";
				case 3:
					return "dlc_vw_casino_slot_machine_rsr_player_sounds";
				case 4:
					return "dlc_vw_casino_slot_machine_fs_player_sounds";
				case 5:
					return "dlc_vw_casino_slot_machine_ds_player_sounds";
				case 6:
					return "dlc_vw_casino_slot_machine_kd_player_sounds";
				case 7:
					return "dlc_vw_casino_slot_machine_td_player_sounds";
				case 8:
					return "dlc_vw_casino_slot_machine_hz_player_sounds";
			}
			return '';
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/spin", "getSounds", e.toString());
			return '';
		}
    }
    async spin(r1, r2, r3) {
		try 
		{
			if(this.can_bet) {
				mp.gui.emmit(`window.events.callEvent("cef.spin.btnExit", 0)`);
				this.can_bet = false;
				mp.game.audio.playSoundFromCoord(mp.game.invoke("0x430386FE9BF80B45"), "attract_loop", slots_positions[this.select_spin].x, slots_positions[this.select_spin].y, slots_positions[this.select_spin].z, this.getSounds (slots_positions[this.select_spin].type), false, 0, false);

				let pos = null;
				for(let i = 0; i < 3; i++) {
					this.g_slots[this.select_spin].reels[i].destroy();
					pos = mp.game.object.getObjectOffsetFromCoords(slots_positions[this.select_spin].x, slots_positions[this.select_spin].y, slots_positions[this.select_spin].z, slots_positions[this.select_spin].rz, reels_offsets[i][0], reels_offsets[i][1], reels_offsets[i][2]);
					this.g_slots[this.select_spin].reels[i] = mp.objects.new(mp.game.joaat("vw_prop_casino_slot_0"+slots_positions[this.select_spin].type+"b_reels"), new mp.Vector3(pos.x, pos.y, pos.z), { rotation: new mp.Vector3(2.3, 0, slots_positions[this.select_spin].rz) });
					this.g_slots[this.select_spin]['spinning'][i] = true;
				}
				
				await global.wait(3000);
				this.g_slots[this.select_spin]['spinning'][0] = null;

				this.g_slots[this.select_spin].reels[0].destroy();
				let position = mp.game.object.getObjectOffsetFromCoords(slots_positions[this.select_spin].x, slots_positions[this.select_spin].y, slots_positions[this.select_spin].z, slots_positions[this.select_spin].rz, reels_offsets[0][0], reels_offsets[0][1], reels_offsets[0][2]);
				this.g_slots[this.select_spin].reels[0] = mp.objects.new(mp.game.joaat("vw_prop_casino_slot_0"+slots_positions[this.select_spin].type+"a_reels"), new mp.Vector3(position.x, position.y, position.z), { rotation: new mp.Vector3(r1, 0, slots_positions[this.select_spin].rz) });
			
				await global.wait(3000);
				this.g_slots[this.select_spin]['spinning'][1] = null;

				this.g_slots[this.select_spin].reels[1].destroy();
				position = mp.game.object.getObjectOffsetFromCoords(slots_positions[this.select_spin].x, slots_positions[this.select_spin].y, slots_positions[this.select_spin].z, slots_positions[this.select_spin].rz, reels_offsets[1][0], reels_offsets[1][1], reels_offsets[1][2]);
				this.g_slots[this.select_spin].reels[1] = mp.objects.new(mp.game.joaat("vw_prop_casino_slot_0"+slots_positions[this.select_spin].type+"a_reels"), new mp.Vector3(position.x, position.y, position.z), { rotation: new mp.Vector3(r2, 0, slots_positions[this.select_spin].rz) });
	 
				await global.wait(3000);
				this.g_slots[this.select_spin]['spinning'][2] = null;

				this.g_slots[this.select_spin].reels[2].destroy();
				position = mp.game.object.getObjectOffsetFromCoords(slots_positions[this.select_spin].x, slots_positions[this.select_spin].y, slots_positions[this.select_spin].z, slots_positions[this.select_spin].rz, reels_offsets[2][0], reels_offsets[2][1], reels_offsets[2][2]);
				this.g_slots[this.select_spin].reels[2] = mp.objects.new(mp.game.joaat("vw_prop_casino_slot_0"+slots_positions[this.select_spin].type+"a_reels"), new mp.Vector3(position.x, position.y, position.z), { rotation: new mp.Vector3(r3 + 0.28, 0, slots_positions[this.select_spin].rz) });
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/spin", "spin", e.toString());
		}
    }

    /**
     * Рендер игры
     */
    render() {
		try 
		{
			if(this.select_spin != null) {
				if(!this.can_bet) {
					let rot = null;

					for(let i = 0; i < 3; i++) {
						if(this.g_slots[this.select_spin]['spinning'][i]) {
							rot = this.g_slots[this.select_spin].reels[i].rotation;
							this.g_slots[this.select_spin].reels[i].rotation = new mp.Vector3(rot.x+7.0, 0.0, rot.z);
						}
					} 
				}
			}

			if(this.freeze_player) {
				for (let i = 6; i <= 16; i++)
					mp.game.controls.disableAllControlActions(i);
			}
		}
		catch (e) 
		{
			if(new Date().getTime() - global.trycatchtime["casino/spin"] < 60000) return;
			global.trycatchtime["casino/spin"] = new Date().getTime();
			mp.events.callRemote("client_trycatch", "casino/spin", "render", e.toString());
		}
    }

    /**
     * Очищает данные о игре
     */
    clear_player_data() {
		this.select_spin = null;
		this.nearest_spin = null;
		this.can_bet = false;
		this.render_cam = false;
		this.freeze_player = false;
    }

    /**
     * Событие, когда игрок нажимает Е
     */
    press_key_e() {
		try 
		{
			if (global.localplayer.isDead() || mp.gui.cursor.visible) return false;
			if (this.select_spin != null) { // Если игрок уже за автоматом
				global.binderFunctions.spinExit ();
				return;
			}

			if(this.select_spin == null && this.nearest_spin != null) { // Если игрок собирается сесть за автомат
				this.freeze_player = true;
				this.select_spin = this.nearest_spin;
				setTimeout(() => { this.freeze_player = false; }, 200);

				const position = mp.game.object.getObjectOffsetFromCoords(slots_positions[this.nearest_spin].x, slots_positions[this.nearest_spin].y, slots_positions[this.nearest_spin].z, slots_positions[this.nearest_spin].rz, -0.48, -1.1, 1);
			   
				mp.events.callRemote('server.spin.OCCUPY_SLOT', this.nearest_spin, position.x, position.y, position.z, slots_positions[this.nearest_spin].rz);
				return;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/spin", "press_key_e", e.toString());
		}
    }

    /**
     * Событие, когда игрок заходит на шейп автомата
     * @param shape - объект шейпа
     */
    on_enter_colshape(shape) {
		try 
		{
			if(shape.casino_slot_id != undefined && this.select_spin == null) {
				this.nearest_spin = shape.casino_slot_id;

				mp.game.audio.playSound(-1, "BACK", "HUD_AMMO_SHOP_SOUNDSET", true, 0, true);
				mp.game.graphics.notify(translateText("~g~E~s~ сыграть в {0}", slots_names[slots_positions[this.nearest_spin].type - 1]));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/spin", "on_enter_colshape", e.toString());
		}
    }

    /**
     * Событие, когда игрок выходит из шейпа автомата
     * @param shape - объект шейпа
     */
    on_leave_colshape(shape) {
		try 
		{
			if(shape.casino_slot_id != undefined) {
				this.nearest_spin = null;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/spin", "on_leave_colshape", e.toString());
		}
    }

    /**
     * Загрузка спинов
     */
    init() {        
		try 
		{
			for (let i = 1; i <= 8; i++) mp.game.entity.createModelHideExcludingScriptObjects(1140.42, 244.32, -51.04, 300, mp.game.joaat("vw_prop_casino_slot_0" + i + "a"), true);
			for(let i = 1; i <= 8; i++) mp.game.entity.createModelHideExcludingScriptObjects(1127.1312255859375, 254.82090759277344, -50.4407958984375, 300.0, mp.game.joaat(`vw_prop_casino_slot_0${i}a`), true);

			for(let i = 0; i < slots_positions.length; i++) {
				this.g_slots[i] = { 
					spinning: []
				};

				this.g_slots[i].slot = mp.objects.new(
					mp.game.joaat(`vw_prop_casino_slot_0${slots_positions[i].type}a`), 
					new mp.Vector3(slots_positions[i].x, slots_positions[i].y, slots_positions[i].z), {
						rotation: new mp.Vector3(0, 0, slots_positions[i].rz)
					}
				);

				this.g_slots[i].reels = [];

				let position = mp.game.object.getObjectOffsetFromCoords(slots_positions[i].x, slots_positions[i].y, slots_positions[i].z, slots_positions[i].rz, 0, -1.5, 1);
				const new_shape = mp.colshapes.newSphere(position.x, position.y, position.z, 0.5);
				new_shape.casino_slot_id = i;
				
				for(var j = 0; j < 3; j++) {
					position = mp.game.object.getObjectOffsetFromCoords(slots_positions[i].x, slots_positions[i].y, slots_positions[i].z, slots_positions[i].rz, reels_offsets[j][0], reels_offsets[j][1], reels_offsets[j][2]);
					this.g_slots[i].reels[j] = mp.objects.new(mp.game.joaat(`vw_prop_casino_slot_0${slots_positions[i].type}a_reels`), new mp.Vector3(position.x, position.y, position.z), { rotation: new mp.Vector3(0, 0, slots_positions[i].rz) });
				}
				
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/spin", "init", e.toString());
		}
    }
}

export default new Spin();
