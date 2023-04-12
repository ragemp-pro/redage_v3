import { correctionPosCharacterForSeatInTableToBlackjack, chipValues, chipModels, chipThickness, pileOffsets, pileRotationOffsets, chipSplitOffsets, chipSplitRotationOffsets, chipOffsets, chipRotationOffsets, chipHeights, blackjackTables, croupiersModels, cardOffsetsDealer, cardSplitRotationOffsets, cardSplitOffsets, cardRotationOffsets, cardOffsets } from './cfg/main.js';
import { requestAnim } from './cfg/utils';


class Blackjack {
    constructor() {
        this.g_blackjackData = [];
        this.dimensionDiamondInterior = 0;
        this.nearestSeat = null;
        this.nearestTable = null;
        this.selectTable = null;
        this.selectSeat = null;
        this.hand = [];
        this.splitHand = [];
        this.btnDouble = 0;
        this.btnSplit = 0;
        this.game = false;
        this.bet = false;
        this.candouble = true;
		this.epress = 0;
		
        this.invokeData = {
            NETWORK_CREATE_SYNCHRONISED_SCENE: "0x7CD6BC4C2BBDD526",
            NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE: "0x742A637471BCECD9",
            NETWORK_START_SYNCHRONISED_SCENE: "0x9A1B3FCDB36C8697",
            NETWORK_STOP_SYNCHRONISED_SCENE: "0xC254481A4574CB2F",
            GET_ANIM_INITIAL_OFFSET_POSITION: "0xBE22B26DD764C040",
            GET_ANIM_INITIAL_OFFSET_ROTATION: "0x4B805E6046EE9E47",
            TASK_GO_STRAIGHT_TO_COORD: "0xD76B57B44F1E6F8B",
        
            _SET_SYNCHRONIZED_SCENE_OCCLUSION_PORTAL: "0x394B9CD12435C981",
            _PLAY_AMBIENT_SPEECH1: "0x8E04FEDD28D42462"
        }
        this.init();
        
        mp.keys.bind(0x45, true, () => {
            this.pressKeyE();
        });

        gm.events.add('playerEnterColshape', (shape) => {
			this.onPlayerEnterColshape(shape);
        });
        
        gm.events.add('playerExitColshape', (shape) => {
			this.onPlayerExitColshape(shape);
        });

		gm.events.add("render", () => {
			if (!global.loggedin) return;

			this.onRender();
		});

		gm.events.add("pedStreamIn", (entity) => {
			this.onEntityStreamIn(entity);
		});
    }

    DealerClothes (randomNumber, dealerPed) {
		try
		{
			if (randomNumber == 0) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 3, 0, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 3, 0, 0)
				dealerPed.setComponentVariation(3, 1, 0, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 3, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 1) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 2, 2, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 4, 0, 0)
				dealerPed.setComponentVariation(3, 0, 3, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 1, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 2) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 2, 1, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 2, 0, 0)
				dealerPed.setComponentVariation(3, 0, 3, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 1, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 3) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 2, 0, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 3, 0, 0)
				dealerPed.setComponentVariation(3, 1, 3, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 3, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 4) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 4, 2, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 3, 0, 0)
				dealerPed.setComponentVariation(3, 0, 0, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 1, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 5) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 4, 0, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 0, 0, 0)
				dealerPed.setComponentVariation(3, 0, 0, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 1, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 6) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 4, 1, 0)
				dealerPed.setComponentVariation(1, 1, 0, 0)
				dealerPed.setComponentVariation(2, 4, 0, 0)
				dealerPed.setComponentVariation(3, 1, 0, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 3, 0, 0)
				dealerPed.setComponentVariation(10, 1, 0, 0)
				dealerPed.setComponentVariation(11, 1, 0, 0)
			} else if (randomNumber == 7) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 1, 1, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 1, 0, 0)
				dealerPed.setComponentVariation(3, 0, 3, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 0, 0, 0)
				dealerPed.setComponentVariation(7, 0, 0, 0)
				dealerPed.setComponentVariation(8, 0, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "DealerClothes", e.toString());
		}
    }

    DealerPedVoiceGroup (randomNumber, dealerPed) {
		try
		{
			if (randomNumber == 0)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_WHITE_01"))
			else if (randomNumber == 1)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_ASIAN_01"))
			else if (randomNumber == 2)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_ASIAN_02"))
			else if (randomNumber == 3)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_ASIAN_01"))
			else if (randomNumber == 4)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_WHITE_01"))
			else if (randomNumber == 5)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_WHITE_02"))
			else if (randomNumber == 6)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_M_Y_Casino_01_WHITE_01"))	
			else if (randomNumber == 7)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_ASIAN_01"))	
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "DealerPedVoiceGroup", e.toString());
		}
    }

    randomInteger(min, max) {
        return Math.round(min + Math.random() * (max - min));
    }

    init () {
		try
		{
			// Удаляем старые столы
			mp.game.entity.createModelHide(1149.38, 269.19, -52.02, 1, mp.game.joaat("vw_prop_casino_blckjack_01"), true);
			mp.game.entity.createModelHide(1151.28, 267.33, -51.84, 1, mp.game.joaat("vw_prop_casino_blckjack_01"), true);
			mp.game.entity.createModelHide(1128.862, 261.795, -51.0357, 1, mp.game.joaat("vw_prop_casino_blckjack_01b"), true);
			mp.game.entity.createModelHide(1143.859, 246.783, -51.035, 1, mp.game.joaat("vw_prop_casino_blckjack_01b"), true);
			mp.game.entity.createModelHide(1146.329, 261.2543, -52.84094, 1, mp.game.joaat("vw_prop_casino_3cardpoker_01"), true);
			mp.game.entity.createModelHide(1143.338, 264.2453, -52.84094, 1, mp.game.joaat("vw_prop_casino_3cardpoker_01"), true);
			mp.game.entity.createModelHide(1133.74, 266.6947, -52.04094, 1, mp.game.joaat("vw_prop_casino_3cardpoker_01b"), true);
			mp.game.entity.createModelHide(1148.74, 251.6947, -52.04094, 1, mp.game.joaat("vw_prop_casino_3cardpoker_01b"), true);
			
			let changeCroupierModel = false; // Для создания поочередно мужской и женской модели крупье
			let randomBlackShit;

			for(let i = 0; i < this.blackjackTables(); i++) {
				let blackjackData = {};

				randomBlackShit = i;

				// Создаем объект стола
				blackjackData.table = mp.objects.new(
					mp.game.joaat(blackjackTables[i][0]),
					new mp.Vector3(
						blackjackTables[i][1],
						blackjackTables[i][2],
						blackjackTables[i][3]
					),                
					{
						rotation: new mp.Vector3(
							0,
							0,
							blackjackTables[i][4])
					}
				);
				
				mp.game.invoke("0x971DA0055324D033", blackjackData.table.handle, 3);

				// Создаем персонажа (крупье)
				const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
					blackjackTables[i][1],
					blackjackTables[i][2],
					blackjackTables[i][3],
					blackjackTables[i][4],
					0,
					0.775,
					1);
				

				blackjackData.croupier = mp.peds.new(
					mp.game.joaat(randomBlackShit < 7 ? croupiersModels[0] : croupiersModels[1]),
					new mp.Vector3(
						x,
						y,
						z
					), 
					blackjackTables[i][4] - 180,
					this.dimensionDiamondInterior
				);
				blackjackData.croupier.setLodDist(100);
				this.DealerClothes (randomBlackShit, blackjackData.croupier);
				this.DealerPedVoiceGroup (randomBlackShit, blackjackData.croupier);
				blackjackData.croupier.blackjack = true;
				blackjackData.croupier.randomBlackShit = randomBlackShit;        
				// Создаём шейпы для посадки персонажа на 1 из 4 стульев    
				for (let l = 1; l < 5; l++) {
					
					const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
						blackjackTables[i][1],
						blackjackTables[i][2],
						blackjackTables[i][3],
						blackjackTables[i][4],
						correctionPosCharacterForSeatInTableToBlackjack[l - 1][0],
						correctionPosCharacterForSeatInTableToBlackjack[l - 1][1],
						correctionPosCharacterForSeatInTableToBlackjack[l - 1][2]);
					
					const shape = mp.colshapes.newSphere(
						x,
						y,
						z,
						0.5
					);

					shape.tableID = i;
					shape.seatID = l;
					shape.blackjack = true;
				}
				// Присваиваем ID стола, к которому привязан крупье
				blackjackData.croupier.tableID = i;
				
				// Создаем массивы для работы
				blackjackData.dealerHand = [];
				blackjackData.dealerHandObjs = [];
				blackjackData.handObjs = {};
				blackjackData.chips = {};
				

				this.g_blackjackData.push(blackjackData);
				// Меняем модель персонажа
				changeCroupierModel = !changeCroupierModel;
			}
			
			
			setTimeout(() => {
				requestAnim('anim_casino_b@amb@casino@games@blackjack@dealer');
				requestAnim('anim_casino_b@amb@casino@games@blackjack@dealer_female');
				requestAnim('anim_casino_b@amb@casino@games@shared@dealer@');

				this.g_blackjackData.forEach(element => {
					const animation = element.croupier.model == mp.game.joaat('S_M_Y_Casino_01') ? "anim_casino_b@amb@casino@games@roulette@dealer" : "anim_casino_b@amb@casino@games@roulette@dealer_female"
					element.croupier.taskPlayAnim(animation, "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
				});
			}, 2000);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "init", e.toString());
		}
    }
    /**
     * Получаем общее количество столов для игры в рулетку
     * @return number
     */
    blackjackTables() {
        return blackjackTables.length;
    }

    /**
     * Событие входа в стрим игрока
     * @param entity - объект
     */
    onEntityStreamIn(entity) {
		try
		{
			if (entity.blackjack) {
				requestAnim('anim_casino_b@amb@casino@games@roulette@dealer');
				requestAnim('anim_casino_b@amb@casino@games@roulette@dealer_female');
				requestAnim('anim_casino_b@amb@casino@games@roulette@table');
				if(entity.model == mp.game.joaat('S_M_Y_Casino_01')) {
					entity.taskPlayAnim("anim_casino_b@amb@casino@games@roulette@dealer", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
					//mp.game.invoke("0x7CDC8C3B89F661B3", entity.handle, mp.game.joaat("S_M_Y_Casino_01_WHITE_01"));
				} else {
					entity.taskPlayAnim("anim_casino_b@amb@casino@games@roulette@dealer_female", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
					//mp.game.invoke("0x7CDC8C3B89F661B3", entity.handle, mp.game.joaat("S_F_Y_Casino_01_ASIAN_01"));
				}
				
				this.DealerClothes (entity.randomBlackShit, entity);
				this.DealerPedVoiceGroup (entity.randomBlackShit, entity);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onEntityStreamIn", e.toString());
		}
    }

    /**
     * Событие, когда игрок вошел в колшэйп
     * @param shape - объект
     */
    onPlayerEnterColshape(shape) {
		try
		{
			if (shape && shape.blackjack && shape.tableID != null && shape.seatID != null) {
				this.nearestSeat = shape.seatID;
				this.nearestTable = shape.tableID;
				mp.game.audio.playSound(-1, "BACK", "HUD_AMMO_SHOP_SOUNDSET", true, 0, true);
				mp.game.graphics.notify(translateText("~g~E~s~ сесть за стол"));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onPlayerEnterColshape", e.toString());
		}
    }

    /**
     * Событие, когда игрок вышел с колшэйпа
     * @param shape - объект
     */
    onPlayerExitColshape(shape) {
		try
		{
			if (shape && shape.blackjack && shape.tableID != null) {
				this.nearestSeat = null;
				this.nearestTable = null;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onPlayerExitColshape", e.toString());
		}
    }
    
    /**
     * Метод при нажатии на клавишу Е
     */
    
    onRender () {
		try
		{
			if (this.selectTable != null && this.selectSeat != null) 
			{
				mp.game.graphics.drawText(translateText("У Вас [{0}]{1}, у дилера [{2}]", this.handValue (this.hand), (this.splitHand.length ? ' [' + this.handValue (this.splitHand) + ']' : ''), this.handValue (this.g_blackjackData[this.selectTable].dealerHand)), [0.5, 0.8], {
					font: 0,
					color: [255, 255, 255, 200],
					scale: [0.35, 0.35],
					outline: true
				});
			}
		}
		catch (e) 
		{
			if(new Date().getTime() - global.trycatchtime["casino/blackjack"] < 60000) return;
			global.trycatchtime["casino/blackjack"] = new Date().getTime();
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onRender", e.toString());
		}
    }
    
    CardValue (card) {
		try
		{
			if(!card) return 0;
			let rank = 10;
			for (let i = 2; i <= 11; i++)
			{
				if (card.indexOf(`${i}`) != -1)
				{
					rank = i;
					break;
				}
			}
			if (card.indexOf("ace") != -1) rank = 11;
			return rank;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "CardValue", e.toString());
			return 0;
		}
    }
    
    handValue (list) {
		try
		{
			if (!list.length) return 0;
			let tmpValue = 0;
			let numAces = 0;
			list.forEach (v => {
				tmpValue += this.CardValue(v);
			})
			list.forEach (v => {
				if (String(v).indexOf("ace") != -1) numAces++;
			})
			if (tmpValue > 21) {
				for (let i = 0; i < numAces; i++) tmpValue = tmpValue - 10;
			}
			return tmpValue;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "handValue", e.toString());
			return 0;
		}
    }
    /**
     * Метод при нажатии на клавишу Е
     */
    pressKeyE() {
		try
		{
			if (global.localplayer.isDead() || mp.gui.cursor.visible) return false;
			if (this.nearestSeat != null && this.selectTable == null) { // Сесть за стол
				if (new Date().getTime() - this.epress < 5000) return;
				this.epress = new Date().getTime();
				mp.game.audio.startAudioScene("DLC_VW_Casino_Table_Games");
				const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
					blackjackTables[this.nearestTable][1],
					blackjackTables[this.nearestTable][2],
					blackjackTables[this.nearestTable][3],
					blackjackTables[this.nearestTable][4],
					correctionPosCharacterForSeatInTableToBlackjack[this.nearestSeat - 1][0],
					correctionPosCharacterForSeatInTableToBlackjack[this.nearestSeat - 1][1],
					correctionPosCharacterForSeatInTableToBlackjack[this.nearestSeat - 1][2]);
				mp.events.callRemote('server.blackjack.character_occupy_place', this.nearestTable, this.nearestSeat, x, y, z, correctionPosCharacterForSeatInTableToBlackjack[this.nearestSeat - 1][3] + blackjackTables[this.nearestTable][4]);
				return;
			} else if (this.selectTable != null) { // Условие, если игрок уже сидит за столом
				if (new Date().getTime() - this.epress < 5000) return;
				this.epress = new Date().getTime();
				this.onLeave();
				return;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "pressKeyE", e.toString());
		}
    } 

    async onSelectTable (i, seat, isBet, isBtn) {
		try
		{
			if (global.menuCheck() || global.pvpStatus) return;
			this.g_blackjackData[i].table.setCollision(false, false);
			this.bet = true;        
			await global.wait(3500);
			this.selectTable = i;
			this.selectSeat = seat;
			this.hand = [];
			this.splitHand = [];
			this.candouble = true;
			this.bet = false;
			gm.discord(translateText("Играет в BlackJack"));
			mp.gui.emmit(`window.router.setView("CasinoBlackjack", {betMax: ${blackjackTables[i][0] == "vw_prop_casino_blckjack_01b" ? 100000 : 25000}, isBet: ${isBet}, isBtn: ${isBtn}, btnDouble: false, btnSplit: false});`);
			global.menuOpen();
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onSelectTable", e.toString());
		}
    }

    onLeave () {
		try
		{
			if (!this.game && this.selectTable != null) { // Условие, если игрок уже сидит за столом
				mp.events.callRemote('server.blackjack.character_leave_place');
				return;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onLeave", e.toString());
		}
    }
	
	onSuccessLeave() {
		try
		{
			if (this.selectTable != null) {
				mp.gui.emmit(`window.router.setHud();`);
				this.g_blackjackData[this.selectTable].table.setCollision(true, false);
				global.menuClose();
				this.bet = true;
				this.game = false;
				setTimeout(() => {            
					this.selectTable = null;
					this.selectSeat = null;
					this.bet = false;
					mp.gui.cursor.visible = false;
				}, 3500);
				return;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "onSuccessLeave", e.toString());
		}
	}

    async GiveCard (i, seat, handSize, card, flipped = false, split = false) {
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			await global.wait(500);
			mp.game.invoke("0x469F2ECDEC046337", true);

			mp.game.audio.startAudioScene("DLC_VW_Casino_Cards_Focus_Hand");
		
			let modelName = `vw_prop_cas_card_${card}`;
			
			if (flipped) {
				this.g_blackjackData[i].realFlippedCard = card;
				modelName = `vw_prop_casino_cards_single`;
			}
			const pedInfo = this.g_blackjackData[i].croupier;
			
			await global.loadModel(modelName);
			const model = mp.objects.new(mp.game.joaat(modelName), new mp.Vector3(blackjackTables[i][1], blackjackTables[i][2], blackjackTables[i][3]));
			await global.IsLoadEntity (model);
			model.setCollision(false, false);
			model.attachTo(pedInfo.handle, pedInfo.getBoneIndex(28422), 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, false, false, false, 2, true);
			
			if (seat > 0) {
				if (!this.g_blackjackData[i].handObjs[seat]) this.g_blackjackData[i].handObjs[seat] = [];
				this.g_blackjackData[i].handObjs[seat].push(model);
			} else {
				if (!this.g_blackjackData[i].dealerHandObjs) this.g_blackjackData[i].dealerHandObjs = [];
				this.g_blackjackData[i].dealerHandObjs.push(model);
			}
			await global.wait(!seat ? 900 : 800);
			if (!model || !mp.objects.exists(model))
				return;
			model.detach(false, true);
			if (!seat) {
				const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
					blackjackTables[i][1],
					blackjackTables[i][2],
					blackjackTables[i][3],
					blackjackTables[i][4],
					cardOffsetsDealer[handSize].x,
					cardOffsetsDealer[handSize].y,
					cardOffsetsDealer[handSize].z);
				model.setCoordsNoOffset(x, y, z, false, false, false);
		
				if (flipped) model.setRotation(180, 0, blackjackTables[i][4] + cardOffsetsDealer[handSize].z, 2, true)
				else model.setRotation(0, 0, blackjackTables[i][4] + cardOffsetsDealer[handSize].z, 2, true)
			} else if (split) {
				const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
					blackjackTables[i][1],
					blackjackTables[i][2],
					blackjackTables[i][3],
					blackjackTables[i][4],
					cardSplitOffsets[seat][handSize].x,
					cardSplitOffsets[seat][handSize].y,
					cardSplitOffsets[seat][handSize].z);
				model.setCoordsNoOffset(x, y, z, false, false, false);
				model.setRotation(0, 0, blackjackTables[i][4] + cardSplitRotationOffsets[seat][handSize], 2, true);
			} else {
				const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
					blackjackTables[i][1],
					blackjackTables[i][2],
					blackjackTables[i][3],
					blackjackTables[i][4],
					cardOffsets[seat][handSize].x,
					cardOffsets[seat][handSize].y,
					cardOffsets[seat][handSize].z);
					
				model.setCoordsNoOffset(x, y, z, false, false, false);
				model.setRotation(0, 0, blackjackTables[i][4] + cardRotationOffsets[seat][handSize], 2, true);
			}
			if (!seat && !flipped) this.g_blackjackData[i].dealerHand.push(card);
			else if (this.selectSeat == seat && this.selectTable == i) {
				if (split) this.splitHand.push(card)
				else this.hand.push(card);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "GiveCard", e.toString());
		}
    }


    async DealerTurnOverCard (i, flipover) {
		try
		{	
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			await global.wait(500);
			const HandObjs = this.g_blackjackData[i].dealerHandObjs[0];
			if (mp.objects.exists(HandObjs)) HandObjs.destroy();
			const flipped = this.g_blackjackData[i].realFlippedCard,
				PedInfo = this.g_blackjackData[i].croupier;
		
			const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
				blackjackTables[i][1],
				blackjackTables[i][2],
				blackjackTables[i][3],
				blackjackTables[i][4],
				cardOffsetsDealer[0].x,
				cardOffsetsDealer[0].y,
				cardOffsetsDealer[0].z);
		
			await global.loadModel(flipover ? `vw_prop_cas_card_${flipped}` : `vw_prop_casino_cards_single`);
			this.g_blackjackData[i].dealerHandObjs[0] = mp.objects.new(mp.game.joaat(flipover ? `vw_prop_cas_card_${flipped}` : `vw_prop_casino_cards_single`), new mp.Vector3(x, y, z));

			await global.IsLoadEntity (this.g_blackjackData[i].dealerHandObjs[0]);
			if (mp.objects.exists(this.g_blackjackData[i].dealerHandObjs[0]))
				this.g_blackjackData[i].dealerHandObjs[0].attachTo(PedInfo.handle, PedInfo.getBoneIndex(28422), 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, false, false, false, false, 2, true);
			await global.wait(1500);		
			
			if (mp.objects.exists(this.g_blackjackData[i].dealerHandObjs[0])) {
				this.g_blackjackData[i].dealerHandObjs[0].detach(false, true);
				this.g_blackjackData[i].dealerHandObjs[0].setCoordsNoOffset(x, y, z, false, false, false);
				this.g_blackjackData[i].dealerHandObjs[0].setRotation(flipover ? 0 : 180, 0, blackjackTables[i][4] + cardOffsetsDealer[0].z, 2, true);
			} else 
				await global.wait(1500);
			if (flipover) this.g_blackjackData[i].dealerHand.push(flipped);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "DealerTurnOverCard", e.toString());
		}
    } 
    
    SplitHand (i, seat, handSize) {
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			const { x, y, z } = mp.game.object.getObjectOffsetFromCoords(
				blackjackTables[i][1],
				blackjackTables[i][2],
				blackjackTables[i][3],
				blackjackTables[i][4],
				cardSplitOffsets[seat][handSize].x,
				cardSplitOffsets[seat][handSize].y,
				cardSplitOffsets[seat][handSize].z);
			
			const obj = this.g_blackjackData[i].handObjs;
		
			if (obj && obj[seat] && obj[seat][obj[seat].length - 1]) 
			{
				obj[seat][obj[seat].length - 1].setCoordsNoOffset(x, y, z, false, false, false);
				obj[seat][obj[seat].length - 1].setRotation(0, 0, blackjackTables[i][4] + cardSplitRotationOffsets[seat][handSize], 2, true);
			}
			if (this.selectSeat == seat && this.selectTable == i) {
				this.splitHand.push(this.hand [this.hand.length - 1]);
				this.hand.splice(this.hand.length - 1, this.hand.length);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "SplitHand", e.toString());
		}
    }

    RetrieveCards (i, seat) {
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			if (!seat) {
				this.g_blackjackData[i].dealerHandObjs.forEach(model => {
					if (mp.objects.exists(model)) model.destroy();
				})
				this.g_blackjackData[i].dealerHandObjs = [];
				this.g_blackjackData[i].dealerHand = [];
			} else {
				if (this.g_blackjackData[i].handObjs && this.g_blackjackData[i].handObjs[seat] && this.g_blackjackData[i].handObjs[seat].length) {
					this.g_blackjackData[i].handObjs[seat].forEach(model => {
						if (mp.objects.exists(model)) model.destroy();
					})
					this.g_blackjackData[i].handObjs[seat] = [];
				}
				if (this.g_blackjackData[i].chips && this.g_blackjackData[i].chips[seat] && this.g_blackjackData[i].chips[seat].length) {
					this.g_blackjackData[i].chips[seat].forEach(model => {
						if (mp.objects.exists(model)) model.destroy();
					})
					this.g_blackjackData[i].chips[seat] = [];
				}
				if ((this.selectSeat == seat && this.selectTable == i) || (this.selectTable == null && this.selectSeat == null)) {
					this.hand = [];
					this.splitHand = [];
					this.candouble = true;
					//if (this.selectSeat == seat && this.selectTable == i) {
					//    mp.gui.emmit(`window.events.callEvent("cef.blackjack.btnExit", 1, "RetrieveCards")`);
					//    mp.gui.cursor.visible = true;
					//}
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "RetrieveCards", e.toString());
		}
    }

    getChips (amount) {
		try
		{
			if (amount <= 2500)
				return { sound: "DLC_VW_CHIP_BET_SML_SINGLE", prop: "vw_prop_chip_50dollar_st" };
			else if (amount <= 10000)
				return { sound: "DLC_VW_CHIP_BET_SML_SMALL", prop: "vw_prop_chip_100dollar_st" };
			else if (amount <= 25000)
				return { sound: "DLC_VW_CHIP_BET_SML_SMALL", prop: "vw_prop_chip_500dollar_st" };
			else if (amount <= 50000)
				return { sound: "DLC_VW_CHIP_BET_SML_MEDIUM", prop: "vw_prop_chip_1kdollar_st" };
			else if (amount <= 100000)
				return { sound: "DLC_VW_CHIP_BET_SML_MEDIUM", prop: "vw_prop_chip_5kdollar_st" };
			else if (amount <= 150000)
				return { sound: "DLC_VW_CHIP_BET_SML_LARGE", prop: "vw_prop_chip_10kdollar_st" };
			else if (amount <= 200000)
				return { sound: "DLC_VW_CHIP_BET_SML_LARGE", prop: "vw_prop_plaq_5kdollar_st" };
			else
				return { sound: "DLC_VW_CHIP_BET_SML_LARGE", prop: "vw_prop_plaq_10kdollar_st" };
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "getChips", e.toString());
			return { sound: "DLC_VW_CHIP_BET_SML_LARGE", prop: "vw_prop_plaq_10kdollar_st" };
		}
        
    }
    async PlaceBetChip (i, seat, bet, double, split) {
		try
		{
			if (Natives.GET_INTERIOR_FROM_ENTITY (global.localplayer.handle) !== 275201)
				return;
			if (this.selectSeat == seat && this.selectTable == i) 
			{  
				mp.gui.emmit(`window.events.callEvent("cef.blackjack.bet", ${bet})`);
				this.bet = true;
				if(double == true || split == true) this.candouble = false;
			}
			let chip = this.getChips (bet);
			if (this.g_blackjackData[i].chips && this.g_blackjackData[i].chips[seat] && this.g_blackjackData[i].chips[seat].length) 
			{
				this.g_blackjackData[i].chips[seat].forEach(model => {
					if (mp.objects.exists(model)) model.destroy();
				})
				this.g_blackjackData[i].chips[seat] = [];
			}
			let location = 0;
			if (double && !split) location = 1;
			else if (!double && split) location = 2;
			else if (double && split) location = 3;

			const model = mp.objects.new(mp.game.joaat(chip.prop), new mp.Vector3(blackjackTables[i][1], blackjackTables[i][2], blackjackTables[i][3]));
			await global.IsLoadEntity (model);
			model.setCollision(false, false);
			
			mp.game.audio.playSoundFromEntity(-1, chip.sound, model.handle, "dlc_vw_table_games_sounds", false, 0);

			const {x, y, z} = mp.game.object.getObjectOffsetFromCoords(blackjackTables[i][1], blackjackTables[i][2], blackjackTables[i][3], blackjackTables[i][4], chipOffsets[seat][location].x, chipOffsets[seat][location].y, 0.896);
			model.setCoordsNoOffset(x, y, z, false, false, false);
			if (!split) model.setRotation(0, 0, blackjackTables[i][4] + chipRotationOffsets[seat], 2, true);
			else model.setRotation(0, 0, blackjackTables[i][4] + chipSplitRotationOffsets[seat], 2, true);

			if (!this.g_blackjackData[i].chips[seat]) this.g_blackjackData[i].chips[seat] = [];
			this.g_blackjackData[i].chips[seat].push(model);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "PlaceBetChip", e.toString());
		}
    }

    PlayDealerAnim (i, animDict, anim) {
		try
		{
			mp.game.streaming.requestAnimDict(animDict);
			const PedInfo = this.g_blackjackData[i].croupier;
			if (PedInfo.model === mp.game.joaat("S_M_Y_Casino_01")) {
				PedInfo.taskPlayAnim(animDict, anim, 4, -2, -1, 2, 0, false, false, false),
				PedInfo.playFacialAnim(anim + "_facial", animDict)
			} else {
				PedInfo.taskPlayAnim(animDict, "female_" + anim, 4, -2, -1, 2, 0, false, false, false),
				PedInfo.playFacialAnim("female_" + anim + "_facial", animDict)
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "PlayDealerAnim", e.toString());
		}
    }

    PlayDealerSpeech (i, speech) {
		try
		{
			const PedInfo = this.g_blackjackData[i].croupier;
			mp.game.invoke(this.invokeData._PLAY_AMBIENT_SPEECH1, PedInfo.handle, speech, "SPEECH_PARAMS_FORCE_NORMAL_CLEAR");
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "PlayDealerSpeech", e.toString());
		}
    }

    CanSplitHand() {
		try
		{
			if (this.hand && this.hand.length == 2 && this.hand[0] && this.hand[1]) 
			{
				if (this.CardValue (this.hand[0]) == this.CardValue (this.hand[1])) return true;
			}
			return false;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "CanSplitHand", e.toString());
			return false;
		}
    }

    openBtn (isBet, isBtn) {      
		try
		{
			this.btnDouble = 0;
			this.btnSplit = 0;

			if (this.hand && this.hand.length == 2 && !this.splitHand.length && this.candouble) this.btnDouble = 1;
			if (!this.splitHand.length && this.CanSplitHand() && this.candouble) this.btnSplit = 1;
			
			mp.gui.emmit(`window.events.callEvent("cef.blackjack.btn", ${isBet}, ${isBtn}, ${this.btnDouble}, ${this.btnSplit})`);
			if (isBet || isBtn) mp.gui.cursor.visible = true;
			else mp.gui.cursor.visible = false;
			mp.gui.emmit(`window.events.callEvent("cef.blackjack.time", 0)`);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/blackjack", "openBtn", e.toString());
		}	
    }
}

global.blackjack = new Blackjack();

gm.events.add('client.blackjack.GiveCard', async function (i, seat, handSize, card, flipped = false, split = false) {
    blackjack.GiveCard (i, seat, handSize, card, flipped, split);
})

gm.events.add('client.blackjack.DealerTurnOverCard', async function (i, flipover = true) {
    blackjack.DealerTurnOverCard (i, flipover);
})

gm.events.add('client.blackjack.character_occupy_place', (i, seat, isBet, isBtn) => {
    blackjack.onSelectTable (i, seat, isBet, isBtn);
});

gm.events.add('client.blackjack.SyncTimer', function (time) {
    mp.gui.emmit(`window.events.callEvent("cef.blackjack.time",  ${time})`);
});

gm.events.add('client.blackjack.ExitBtn', function (btn) {
    mp.gui.emmit(`window.events.callEvent("cef.blackjack.btnExit", ${btn}, "server")`);

    if (!btn) blackjack.game = true;
    else {
        blackjack.bet = false;
        blackjack.game = false;
    }
});

gm.events.add('client.blackjack.successLeave', function () {
    blackjack.onSuccessLeave();
});

gm.events.add('client.blackjack.SplitHand', function (i, seat, handSize) {
    blackjack.SplitHand (i, seat, handSize)
});

gm.events.add('client.blackjack.RetrieveCards', function (i, seat) {
    blackjack.RetrieveCards (i, seat)
});

gm.events.add('client.blackjack.PlayDealerSpeech', function (i, speech) {
    blackjack.PlayDealerSpeech (i, speech);
});

gm.events.add('client.blackjack.PlayDealerAnim', function (i, animDict, anim) {
    blackjack.PlayDealerAnim (i, animDict, anim);
});

gm.events.add('client.blackjack.PlaceBetChip', function (i, seat, bet, double = false, split = false) {
    blackjack.PlaceBetChip (i, seat, bet, double, split);
});

gm.events.add('client.blackjack.setBet', function (value) {
    mp.events.callRemote('server.blackjack.setBet', global.pInt (value));
});


global.binderFunctions.blackjackExit = () => {
    blackjack.onLeave ();
}

gm.events.add('client.blackjack.exit', function (value) {
    blackjack.onLeave ();
});

gm.events.add('client.blackjack.btn', function (value) {
    mp.events.callRemote('server.blackjack.move', value);
});

gm.events.add('client.blackjack.isBtn', function (isBet, isBtn) {
    blackjack.openBtn (isBet, isBtn);
});

gm.events.add('client.blackjack.betWin', function (value) {
    mp.gui.emmit(`window.events.callEvent("cef.blackjack.betWin", ${value})`);
});
