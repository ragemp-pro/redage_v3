import { rouletteTables, croupiersModels, correctionPosCharacterForSeatInTable, tableChipsOffsets, tableMarkersOffsets, block_bet_object } from './cfg/main.js';
import { getDistanceBetweenPoints3D, requestAnim } from './cfg/utils';

global.rouletteplay = false;

class Casino {

    constructor() {
        /** Общий массив с данными о каждом столе */
        this.g_rouletteData = [];
    
        /** Объект шарика */
        this.g_rouletteBallObjectID = 87196104;
    
        /** Коррекция X координаты для объекта шарика */
        this.g_rouletteBallCorrectionPosX = 0.734742;
    
        /** Коррекция Y координаты для объекта шарика */
        this.g_rouletteBallCorrectionPosY = 0.16617;
    
        /** Коррекция Y координаты для модели крупье */
        this.g_rouletteCroupierCorrectionPosY = 0.7;
    
        /** Коррекция Z координаты для модели крупье */
        this.g_rouletteCroupierCorrectionPosZ = 1;
    
        /** Виртуальный мир для интерьера казино */
        this.dimensionDiamondInterior = 0;
    
        /** Объект для перемещения камеры */
        this.casinoCamera = null;
    
        /** Выбранный стол для игры в рулетку */
        this.selectTable = null;
    
        /** Выбирает ближайший стул к персонажу */
        this.nearestSeat = null;
    
        /** Выбирает ближайший стол к персонажу */
        this.nearestTable = null;
    
        /** Объект фишки, в момент когда игрок делает ставку */
        this.rouletteBetObject = null;
    
        /** Флаг для translateText("разрешения") ставки */
        this.betState = true;
    
        /** Подсветка доступных ставок */
        this.tableMarkers = [];
		
		/** Подсветка моих ставок */
        this.tableBet1Vector = null;
		this.tableBet2Vector = null;
		this.tableBet3Vector = null;
		this.tableBetsMarkers = [];
    
        /** Текущая ставка */
        this.closestChipSpot = null;
    
        /** Этап игры */
        this.gameStep = 0;

        this.bet = 500;

        this.isSeat = false;

        this.init();

        mp.keys.bind(0x45, true, () => {
            this.pressKeyE();
        });

        mp.keys.bind(0x48, true, () => {
            this.pressKeyH();
        });

        mp.keys.bind(global.Keys.VK_LEFT, true, () => {
			try 
			{
				if (this.selectTable == null) return;
				else if (!this.betState) return;
				this.bet = Math.round (this.bet / 2);
				const minBet = rouletteTables[this.selectTable][0] == "vw_prop_casino_roulette_01" ? 50 : 7500;
				if (minBet > this.bet) this.bet = minBet;
				mp.gui.emmit(`window.events.callEvent("cef.roullete.bet", ${this.bet})`);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "VK_LEFT", e.toString());
			}
        });

        mp.keys.bind(global.Keys.VK_RIGHT, true, () => {
			try 
			{
				if (this.selectTable == null) return;
				else if (!this.betState) return;
				this.bet *= 2;
				const maxBet = rouletteTables[this.selectTable][0] == "vw_prop_casino_roulette_01" ? 25000 : 100000;
				if (maxBet < this.bet) this.bet = maxBet;
				mp.gui.emmit(`window.events.callEvent("cef.roullete.bet", ${this.bet})`);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "VK_RIGHT", e.toString());
			}
        });


        
        gm.events.add('client.roullete.TOGGLE_BET', (value) => {
			try 
			{
				if (this.selectTable == null) return;
				this.betState = true;
				if (value > 0) mp.gui.emmit(`window.events.callEvent("cef.roullete.betWin", ${value})`);
				
				//this.g_rouletteData[this.selectTable].ball.playAnim('idle', 'anim_casino_b@amb@casino@games@roulette@table', 1000.0, false, true, false, 0, 1000.0);
				if (this.g_rouletteData[this.selectTable] &&
						this.g_rouletteData[this.selectTable].ball && mp.objects.exists(this.g_rouletteData[this.selectTable].ball)) this.g_rouletteData[this.selectTable].ball.destroy();  
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "client.roullete.TOGGLE_BET", e.toString());
			}
        });

        gm.events.add('client.roullete.START_GAME', (text) => {
			try 
			{
				if (this.selectTable == null || !this.g_rouletteData[this.selectTable] || !this.g_rouletteData[this.selectTable].croupier) return;
				this.betState = false;
				mp.gui.emmit(`window.events.callEvent("cef.roullete.time", 0)`);
				global.rouletteplay = true;
				
				const animation = this.g_rouletteData[this.selectTable].croupier.model == mp.game.joaat('S_M_Y_Casino_01') ? "anim_casino_b@amb@casino@games@roulette@dealer" : "anim_casino_b@amb@casino@games@roulette@dealer_female"
				this.g_rouletteData[this.selectTable].croupier.taskPlayAnim(animation, "no_more_bets", 8.0, 1, -1, 1, 0.0, false, false, false);

				setTimeout(() => { this.g_rouletteData[this.selectTable].croupier.taskPlayAnim(animation, "idle", 8.0, 1, -1, 1, 0.0, false, false, false); }, 4000);

				setTimeout(() => {
					this.startRouletteWheel(text);
				}, 5000);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "client.roullete.START_GAME", e.toString());
			}
        });

        gm.events.add('client.roullete.CHARACTER_OCCUPY_PLACE', async (table, seat, state) => {
			try 
			{
				this.selectTable = table;
				//this.nearestSeat = seat;
				this.betState = state;
				this.isSeat = false;
				this.clearMyBetsTableMarkers();
				
				this.g_rouletteData[table].table.setCollision(false, false);

				global.localplayer.position = new mp.Vector3(
					rouletteTables[table][1] + correctionPosCharacterForSeatInTable[seat][0], 
					rouletteTables[table][2] + correctionPosCharacterForSeatInTable[seat][1],
					rouletteTables[table][3] + correctionPosCharacterForSeatInTable[seat][2]
				);

				global.localplayer.setHeading(correctionPosCharacterForSeatInTable[seat][3]);
				await global.wait(4000);
				global.rouletteplay = false;
				gm.discord(translateText("Играет в рулетку"));
				mp.gui.emmit(`window.router.setView("CasinoRoullete", ${this.bet});`);
				global.menuOpened = true;
				this.isSeat = true;
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "client.roullete.CHARACTER_OCCUPY_PLACE", e.toString());
			}
        });

        gm.events.add('client.roullete.CHARACTER_LEAVE_PLACE', () => {
			try 
			{
				mp.gui.emmit(`window.router.setHud();`);
				global.menuClose ();
				
				if (this.isCreateCroupierAndTable())
					this.g_rouletteData[this.selectTable].table.setCollision(true, false);
				
				this.clearCamera();
				this.clearBetObject();
				this.clearTableMarkers();
				this.clearMyBetsTableMarkers();
				
				this.selectTable = null;
				this.isSeat = false;
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "client.roullete.CHARACTER_LEAVE_PLACE", e.toString());
			}
        });

        gm.events.add('client.roullete.timer', (value) => {
			try 
			{
				mp.gui.emmit(`window.events.callEvent("cef.roullete.time", ${value})`);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "client.roullete.timer", e.toString());
			}
        });

        gm.events.add('playerEnterColshape', (shape) => {
			try 
			{
				this.onPlayerEnterColshape(shape);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "playerEnterColshape", e.toString());
			}
        });
        
        gm.events.add('playerExitColshape', (shape) => {
			try 
			{
				this.onPlayerExitColshape(shape);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "playerExitColshape", e.toString());
			}
        });

        gm.events.add('playerDeath', (player) => {
			try 
			{
				if (!global.loggedin) return;
				this.onPlayerDeath(player);
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "playerDeath", e.toString());
			}
        });

		gm.events.add("render", () => {

			if (!global.loggedin) return;

			this.renderRoulette();
		});

		gm.events.add("pedStreamIn", (entity) => {
			this.onEntityStreamIn(entity);
		});

        global.binderFunctions.rouletteExit = () => {
			try 
			{
				if (this.selectTable != null && this.isSeat) {
					mp.events.callRemote("server.roullete.CHARACTER_LEAVE_PLACE");
				}
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "casino/roulette", "global.binderFunctions.rouletteExit", e.toString());
			}
        }
    }

    /**
     * Запуск процесса прокрутки колеса
     */
	isCreateCroupierAndTable() {
		if (this.selectTable == null) return false;
		else if (!this.g_rouletteData[this.selectTable]) return false;
		else if (!this.g_rouletteData[this.selectTable].croupier) return false;
		else if (!this.g_rouletteData[this.selectTable].table) return false;
		else if (!mp.objects.exists(this.g_rouletteData[this.selectTable].table)) return false;
		else if (this.g_rouletteData[this.selectTable].table.handle === 0) return false;
		return true;
	}

	isBall() {
		if (this.selectTable == null) return false;
		else if (!this.g_rouletteData[this.selectTable]) return false;
		else if (!this.g_rouletteData[this.selectTable].ball) return false;
		else if (!mp.objects.exists(this.g_rouletteData[this.selectTable].ball)) return false;
		else if (this.g_rouletteData[this.selectTable].ball.handle === 0) return false;
		return true;
	}

    async startRouletteWheel(text) {
		try 
		{
			if (!this.isCreateCroupierAndTable()) return;
			const animation = this.g_rouletteData[this.selectTable].croupier.model == mp.game.joaat('S_M_Y_Casino_01') ? "anim_casino_b@amb@casino@games@roulette@dealer" : "anim_casino_b@amb@casino@games@roulette@dealer_female"
			this.g_rouletteData[this.selectTable].croupier.taskPlayAnim(animation, "spin_wheel", 8.0, 1, -1, 2, 0.0, false, false, false);

			await global.wait(1500);
			if (!this.isCreateCroupierAndTable()) return;
			this.g_rouletteData[this.selectTable].table.playAnim('loop_wheel', 'anim_casino_b@amb@casino@games@roulette@table', 1000.0, false, true, true, 0, 1.0);
			await global.wait(1500);
				
			await global.loadModel(this.g_rouletteBallObjectID);
			this.g_rouletteData[this.selectTable].ball = mp.objects.new(this.g_rouletteBallObjectID, new mp.Vector3(rouletteTables[this.selectTable][1] - 0.56, rouletteTables[this.selectTable][2] + 0.1, rouletteTables[this.selectTable][3] + 1.0715));
			
			//this.g_rouletteData[this.selectTable].ball.position = new mp.Vector3(rouletteTables[this.selectTable][1] - 0.56, rouletteTables[this.selectTable][2] + 0.1, rouletteTables[this.selectTable][3] + 1.0715);
			this.g_rouletteData[this.selectTable].ball.rotation = new mp.Vector3(0.0, 0.0, 0.0);
			this.g_rouletteData[this.selectTable].ball.playAnim('loop_ball', 'anim_casino_b@amb@casino@games@roulette@table', 1000.0, false, true, false, 0, 1000.0);

			const interval = setInterval(async () => {
				if (!this.isBall()) return;
				this.g_rouletteData[this.selectTable].ball.playAnim('loop_ball', 'anim_casino_b@amb@casino@games@roulette@table', 1000.0, false, true, false, 0, 1000.0);
			}, 1000);
			await global.wait(1000 * 10);
			
			clearInterval(interval);
			if (!this.isCreateCroupierAndTable()) return;
			this.g_rouletteData[this.selectTable].table.playAnim(text + "wheel", 'anim_casino_b@amb@casino@games@roulette@table', 1000.0, false, true, false, 0, 100.0);

			if (!this.isBall()) return;
			this.g_rouletteData[this.selectTable].ball.position = new mp.Vector3(rouletteTables[this.selectTable][1] - 0.56, rouletteTables[this.selectTable][2] + 0.1, rouletteTables[this.selectTable][3] + 1.0715);
			this.g_rouletteData[this.selectTable].ball.rotation = new mp.Vector3(0.0, 0.0, 0.0);
			this.g_rouletteData[this.selectTable].ball.playAnim(text + "ball", 'anim_casino_b@amb@casino@games@roulette@table', 1000.0, false, true, false, 0, 1000.0); 
			await global.wait(1000 * 10);
			mp.events.callRemote('server.roullete.CLEAR_TABLE');
			if (!this.isCreateCroupierAndTable()) return;
			this.clearMyBetsTableMarkers();
			this.g_rouletteData[this.selectTable].croupier.taskPlayAnim(animation, "clear_chips_zone2", 8.0, 1, -1, 2, 0.0, false, false, false);
			await global.wait(2000);
			if (!this.isCreateCroupierAndTable()) return;
			this.g_rouletteData[this.selectTable].croupier.taskPlayAnim(animation, "idle", 8.0, 1, -1, 2, 0.0, false, false, false);

			await global.wait(8000);
			if (!this.isCreateCroupierAndTable()) return;
			this.g_rouletteData[this.selectTable].croupier.taskPlayAnim(animation, "idle", 8.0, 1, -1, 2, 0.0, false, false, false);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "startRouletteWheel", e.toString());
		}
    }
    DealerClothes (randomNumber, dealerPed) {
		try 
		{
			if (randomNumber == 0) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 1, 1, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 1, 1, 0)
				dealerPed.setComponentVariation(3, 1, 3, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 0, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 1, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
			} else if (randomNumber == 1) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 2, 0, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 2, 0, 0)
				dealerPed.setComponentVariation(3, 2, 3, 0)
				dealerPed.setComponentVariation(4, 0, 0, 0)
				dealerPed.setComponentVariation(6, 0, 0, 0)
				dealerPed.setComponentVariation(7, 0, 0, 0)
				dealerPed.setComponentVariation(8, 2, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
			} else if (randomNumber == 2) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 2, 1, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 2, 1, 0)
				dealerPed.setComponentVariation(3, 3, 3, 0)
				dealerPed.setComponentVariation(4, 1, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 3, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
			} else if (randomNumber == 3) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 3, 0, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 3, 0, 0)
				dealerPed.setComponentVariation(3, 0, 1, 0)
				dealerPed.setComponentVariation(4, 1, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 1, 0, 0)
				dealerPed.setComponentVariation(8, 0, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
				dealerPed.setPropIndex(1, 0, 0, false)
			} else if (randomNumber == 4) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 3, 1, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 3, 1, 0)
				dealerPed.setComponentVariation(3, 1, 1, 0)
				dealerPed.setComponentVariation(4, 1, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 2, 0, 0)
				dealerPed.setComponentVariation(8, 1, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
			} else if (randomNumber == 5) {
				dealerPed.setDefaultComponentVariation()
				dealerPed.setComponentVariation(0, 4, 0, 0)
				dealerPed.setComponentVariation(1, 0, 0, 0)
				dealerPed.setComponentVariation(2, 4, 0, 0)
				dealerPed.setComponentVariation(3, 2, 1, 0)
				dealerPed.setComponentVariation(4, 1, 0, 0)
				dealerPed.setComponentVariation(6, 1, 0, 0)
				dealerPed.setComponentVariation(7, 1, 0, 0)
				dealerPed.setComponentVariation(8, 2, 0, 0)
				dealerPed.setComponentVariation(10, 0, 0, 0)
				dealerPed.setComponentVariation(11, 0, 0, 0)
				dealerPed.setPropIndex(1, 0, 0, false)
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "DealerClothes", e.toString());
		}
    }

    DealerPedVoiceGroup (randomNumber, dealerPed) {
		try 
		{
			if (randomNumber == 0)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_ASIAN_02"))
			else if (randomNumber == 1)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_ASIAN_01"))
			else if (randomNumber == 2)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_ASIAN_02"))
			else if (randomNumber == 3)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_LATINA_01"))
			else if (randomNumber == 4)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_LATINA_02"))
			else if (randomNumber == 5)
				mp.game.invoke("0x7CDC8C3B89F661B3", dealerPed.handle, mp.game.joaat("S_F_Y_Casino_01_LATINA_01"))
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "DealerPedVoiceGroup", e.toString());
		}

    }
    /**
     * Загрузка казино
     */
    init() {
		try 
		{
			this.betState = true;

			for(let i = 0; i < this.countRouletteTables(); i++) {
				// Создаем объект внутри массива
				this.g_rouletteData[i] = {
					table: null,
					ball: null,
					croupier: {
						tableID: null
					}
				};

				// Создаем объект рулетки
				this.g_rouletteData[i].table = mp.objects.new(
					mp.game.joaat(rouletteTables[i][0]),
					new mp.Vector3(
						rouletteTables[i][1],
						rouletteTables[i][2],
						rouletteTables[i][3]
					)
				);

				// Создаем объект белого шарика (который вбрасывает крупье)
				this.g_rouletteData[i].ball = mp.objects.new(
					this.g_rouletteBallObjectID,
					new mp.Vector3(
						rouletteTables[i][1] - this.g_rouletteBallCorrectionPosX,
						rouletteTables[i][2] - this.g_rouletteBallCorrectionPosY,
						rouletteTables[i][3]
					)
				);

				// Создаем персонажа (крупье)
				this.g_rouletteData[i].croupier = mp.peds.new(
					mp.game.joaat(croupiersModels[1]),
					new mp.Vector3(
						rouletteTables[i][1],
						rouletteTables[i][2] + this.g_rouletteCroupierCorrectionPosY,
						rouletteTables[i][3] + this.g_rouletteCroupierCorrectionPosZ
					), 
					180,
					this.dimensionDiamondInterior
				);
				this.g_rouletteData[i].croupier.setLodDist(100);
				this.DealerClothes (i, this.g_rouletteData[i].croupier);
				this.DealerPedVoiceGroup (i, this.g_rouletteData[i].croupier);
				
				// Создаём шейпы для посадки персонажа на 1 из 4 стульев
				for (let l = 0; l < correctionPosCharacterForSeatInTable.length; l++) {
					const shape = mp.colshapes.newSphere(
						rouletteTables[i][1] + correctionPosCharacterForSeatInTable[l][0],
						rouletteTables[i][2] + correctionPosCharacterForSeatInTable[l][1],
						rouletteTables[i][3] + correctionPosCharacterForSeatInTable[l][2],
						0.5
					);

					shape.tableID = i;
					shape.seatID = l;
					shape.roulette = true;
				}

				// Присваиваем ID стола, к которому привязан крупье
				this.g_rouletteData[i].croupier.tableID = i;
				
				this.g_rouletteData[i].croupier.roulette = true;
			}
			
		
			setTimeout(() => {
				requestAnim('anim_casino_b@amb@casino@games@roulette@dealer');
				requestAnim('anim_casino_b@amb@casino@games@roulette@dealer_female');
				requestAnim('anim_casino_b@amb@casino@games@roulette@table');

				this.g_rouletteData.forEach(element => {
					if (!element.croupier) return;
					const animation = element.croupier.model == mp.game.joaat('S_M_Y_Casino_01') ? "anim_casino_b@amb@casino@games@roulette@dealer" : "anim_casino_b@amb@casino@games@roulette@dealer_female"
					element.croupier.taskPlayAnim(animation, "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
				});
			}, 2000);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "init", e.toString());
		}
    }

    /**
     * Метод при нажатии на клавишу Е
     */
    pressKeyE() {
		try 
		{
			if (global.localplayer.isDead()) return false;
        
			if (this.nearestSeat != null && this.selectTable == null) { // Сесть за стол 
				mp.events.callRemote("server.roullete.CHARACTER_OCCUPY_PLACE", this.nearestTable, this.nearestSeat);
				return;
			}

			if (this.selectTable != null) { // Условие, если игрок уже сидит за столом
				
				global.binderFunctions.rouletteExit ();
				return;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "pressKeyE", e.toString());
		}
    }

    /**
     * Метод при нажатии на клавишу H
     */
    pressKeyH() {
		try 
		{
			if(this.selectTable != null) {
				if(this.casinoCamera == null) {
					this.casinoCamera = mp.cameras.new(
						'default',
						new mp.Vector3(
							rouletteTables[this.selectTable][1],
							rouletteTables[this.selectTable][2] - 0.47,
							rouletteTables[this.selectTable][3] + 2.85
						),
						new mp.Vector3(0,0,0),
						45
					);
			
					this.casinoCamera.setRot(-75.0, 0.0, 0.0, 2);                  
					this.casinoCamera.setActive(true);
					mp.game.cam.renderScriptCams(true, false, 500, true, false);
				}
				else {
					this.clearCamera();
					this.clearBetObject();
					this.clearTableMarkers();
					this.clearMyBetsTableMarkers();
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "pressKeyH", e.toString());
		}
    }

    /**
     * Рендер ставок
     */
    renderRoulette() {
		try 
		{
			if(this.rouletteBetObject == null && this.casinoCamera && this.selectTable != null && this.betState) {
				this.rouletteBetObject = mp.objects.new(mp.game.joaat("vw_prop_chip_100dollar_x1"), new mp.Vector3(rouletteTables[this.selectTable][1], rouletteTables[this.selectTable][2], rouletteTables[this.selectTable][3]));
				this.rouletteBetObject.setCollision(false, false);
			}

			if(!this.betState && this.rouletteBetObject) 
			{
				this.clearBetObject();
				this.getChipsSpots();
				this.clearTableMarkers();
			}

			if(this.betState && this.casinoCamera && this.selectTable != null) {
				if(this.rouletteBetObject != null) {
					const drawObject = this.getCameraHitCoord();
					if(drawObject != null) {
						drawObject.position.z = rouletteTables[this.selectTable][3] + 0.95;

						if(drawObject.position.x > block_bet_object[this.selectTable][0]) drawObject.position.x = block_bet_object[this.selectTable][0];
						if(drawObject.position.x < block_bet_object[this.selectTable][1]) drawObject.position.x = block_bet_object[this.selectTable][1];
						if(drawObject.position.y > block_bet_object[this.selectTable][2]) drawObject.position.y = block_bet_object[this.selectTable][2];
						if(drawObject.position.y < block_bet_object[this.selectTable][3]) drawObject.position.y = block_bet_object[this.selectTable][3];
						
						this.rouletteBetObject.setCoordsNoOffset(drawObject.position.x, drawObject.position.y, drawObject.position.z, false, false, false);
						
						this.getClosestChipSpot(new mp.Vector3(drawObject.position.x, drawObject.position.y, drawObject.position.z));
					}

					if(mp.game.controls.isDisabledControlJustReleased(0, 24) && !mp.gui.cursor.visible) {
						if(this.closestChipSpot != null && this.betState) {
							mp.events.callRemote("server.roullete.CREATE_BET", this.bet, this.closestChipSpot, 
								drawObject.position.x,
								drawObject.position.y,
								rouletteTables[this.selectTable][3] + 0.95);
							if(this.tableBet1Vector == null) this.tableBet1Vector = new mp.Vector3(drawObject.position.x, drawObject.position.y, drawObject.position.z);
							else if(this.tableBet2Vector == null) this.tableBet2Vector = new mp.Vector3(drawObject.position.x, drawObject.position.y, drawObject.position.z);
							else if(this.tableBet3Vector == null) this.tableBet3Vector = new mp.Vector3(drawObject.position.x, drawObject.position.y, drawObject.position.z);
						}
					}
				
					if(mp.game.controls.isDisabledControlJustReleased(0, 25) && !mp.gui.cursor.visible && this.betState)  {
						if (this.closestChipSpot != null) 
						{
							if(this.tableBet3Vector != null) this.tableBet3Vector = null;
							else if(this.tableBet2Vector != null) this.tableBet2Vector = null;
							else if(this.tableBet1Vector != null) this.tableBet1Vector = null;
							mp.events.callRemote("server.roullete.DESTROY_LAST_BET");
						}
					}

					global.clearScript (drawObject);
				}
			}

			if(this.casinoCamera != null && !this.gameStep) {
				let rightAxisX = mp.game.controls.getDisabledControlNormal(0, 220);
				let rightAxisY = mp.game.controls.getDisabledControlNormal(0, 221);
				
				let leftAxisX = 0;
				let leftAxisY = 0;
				
				let pos = this.casinoCamera.getCoord();
				let rr = this.casinoCamera.getDirection();
				let vector = new mp.Vector3(0, 0, 0);
				vector.x = rr.x * leftAxisY;
				vector.y = rr.y * leftAxisY;
				vector.z = rr.z * leftAxisY;
				
				let upVector = new mp.Vector3(0, 0, 1);
				let rightVector = this.getCrossProduct(this.getNormalizedVector(rr), this.getNormalizedVector(upVector));
				rightVector.x *= leftAxisX * 0.5;
				rightVector.y *= leftAxisX * 0.5;
				rightVector.z *= leftAxisX * 0.5;
				
				let rot = this.casinoCamera.getRot(2);
				
				let rotx = rot.x + rightAxisY * -5.0;
				if(rotx > 89) rotx = 89;
				if(rotx < -89) rotx = -89;
				
				this.casinoCamera.setRot(rotx, 0.0, rot.z + rightAxisX * -5.0, 2);
			}
		}
		catch (e) 
		{
			if(new Date().getTime() - global.trycatchtime["casino/roulette"] < 60000) return;
			global.trycatchtime["casino/roulette"] = new Date().getTime();
			mp.events.callRemote("client_trycatch", "casino/roulette", "render", e.toString());
		}
    }

    /**
     * Событие, когда игрок вошел в колшэйп
     * @param shape - объект
     */
    onPlayerEnterColshape(shape) {
		try 
		{
			if (shape && shape.roulette && shape.tableID != null && shape.seatID != null) {
				this.nearestSeat = shape.seatID;
				this.nearestTable = shape.tableID;

				mp.game.audio.playSound(-1, "BACK", "HUD_AMMO_SHOP_SOUNDSET", true, 0, true);
				mp.game.graphics.notify(translateText("~g~E~s~ сесть за стол"));
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "onPlayerEnterColshape", e.toString());
		}
    }

    /**
     * Событие входа в стрим игрока
     * @param entity - объект
     */
    onEntityStreamIn(entity) {
		try 
		{
			if (entity && entity.handle != 0 && entity.roulette) 
			{
				requestAnim('anim_casino_b@amb@casino@games@roulette@dealer');
				requestAnim('anim_casino_b@amb@casino@games@roulette@dealer_female');
				requestAnim('anim_casino_b@amb@casino@games@roulette@table');

				if(entity.model == mp.game.joaat('S_M_Y_Casino_01')) entity.taskPlayAnim("anim_casino_b@amb@casino@games@roulette@dealer", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
				else entity.taskPlayAnim("anim_casino_b@amb@casino@games@roulette@dealer_female", "idle", 8.0, 1, -1, 1, 0.0, false, false, false);
				
				const id = entity.tableID;
				if (this.g_rouletteData && this.g_rouletteData[id] && this.g_rouletteData[id].ball && this.g_rouletteData[id].ball.handle != 0) {
					this.g_rouletteData[id].ball.position = new mp.Vector3(
						rouletteTables[id][1] - this.g_rouletteBallCorrectionPosX,
						rouletteTables[id][2] - this.g_rouletteBallCorrectionPosY,
						rouletteTables[id][3]
					);
				}
				
				this.DealerClothes (id, entity);
				this.DealerPedVoiceGroup (id, entity);
			}
		}
		catch (e) 
		{
			//mp.events.callRemote("client_trycatch", "casino/roulette", "onEntityStreamIn", e.toString());
		}
    }

    /**
     * Событие, когда игрок вышел с колшэйпа
     * @param shape - объект
     */
    onPlayerExitColshape(shape) {
		try 
		{
			if (shape.tableID != null) {
				this.nearestSeat = null;
				this.nearestTable = null;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "onPlayerExitColshape", e.toString());
		}
    }

    /**
     * Событие, когда игрок умирает
     * @param player - объект игрока
     */
    onPlayerDeath(player) {
		try 
		{
			if (player == global.localplayer) {
				if (this.nearestSeat != null) this.nearestSeat = null;
				if (this.selectTable != null) this.selectTable = null;

				this.clearCamera();
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "onPlayerDeath", e.toString());
		}
    }

    /**
     * Удаляет объект фишки
     */
    clearBetObject() {
		try 
		{
			if(this.rouletteBetObject) {
				this.rouletteBetObject.destroy();
				this.rouletteBetObject = null;
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "clearBetObject", e.toString());
		}
    }

    /**
     * Удаляет созданную камеру
     */
    clearCamera() {
		try 
		{
			if (this.casinoCamera != null) {
				this.casinoCamera.destroy(true);
				this.casinoCamera = null;
				mp.game.cam.renderScriptCams(false, false, 0, true, false);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "clearCamera", e.toString());
		}
    }

    /**
     * Получаем общее количество столов для игры в рулетку
     * @return number
     */
    countRouletteTables() {
        return rouletteTables.length;
    }

    /**
     * 
     * @param vector - вектор
     */
    getClosestChipSpot(vector) {
		try 
		{
			let spot = null;
			let prevDistance = 0.05;
			let dist = null;
			
			for(let i = 0; i < tableChipsOffsets.length; i++) {
				dist = getDistanceBetweenPoints3D(vector, new mp.Vector3(rouletteTables[this.selectTable][1] + tableChipsOffsets[i][0], rouletteTables[this.selectTable][2] + tableChipsOffsets[i][1], rouletteTables[this.selectTable][3] + tableChipsOffsets[i][2]));
				if(dist <= prevDistance) {
					spot = i;
					prevDistance = dist;
				}
			}
			
			if(spot != this.closestChipSpot) {
				this.closestChipSpot = spot;
				this.clearTableMarkers();
				
				if(spot != null) {
					let key = null;
					let obj = null;
					for(let i = 0; i < tableChipsOffsets[spot][3].length; i++) {
						key = tableChipsOffsets[spot][3][i];
						if(key == "00" || key == "0") {
							obj = mp.objects.new(269022546, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2]));
							obj.setCollision(false, false);
							this.tableMarkers.push(obj);
						}
						else {
							this.tableMarkers.push(mp.objects.new(3267450776, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2])));
						}
					}
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "getClosestChipSpot", e.toString());
		}
    }
	
	async getChipsSpots() {
		try 
		{
			let spot = null;
			let key = null;
			let obj = null;
			let prevDistance = 0.05;
			let dist = null;
			if(this.tableBet1Vector != null) {
				spot = null;
				dist = null;
				prevDistance = 0.05;
				for(let i = 0; i < tableChipsOffsets.length; i++) {
					dist = getDistanceBetweenPoints3D(this.tableBet1Vector, new mp.Vector3(rouletteTables[this.selectTable][1] + tableChipsOffsets[i][0], rouletteTables[this.selectTable][2] + tableChipsOffsets[i][1], rouletteTables[this.selectTable][3] + tableChipsOffsets[i][2]));
					if(dist <= prevDistance) {
						spot = i;
						prevDistance = dist;
					}
				}
				for(let i = 0; i < tableChipsOffsets[spot][3].length; i++) {
						key = tableChipsOffsets[spot][3][i];
						if(key == "00" || key == "0") {
							
							await global.loadModel(269022546);
							obj = mp.objects.new(269022546, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2]));
							obj.setCollision(false, false);
							this.tableBetsMarkers.push(obj);
						}
						else {
							await global.loadModel(3267450776);
							this.tableBetsMarkers.push(mp.objects.new(3267450776, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2])));
						}
					}
			}
			if(this.tableBet2Vector != null) {
				spot = null;
				dist = null;
				prevDistance = 0.05;
				for(let i = 0; i < tableChipsOffsets.length; i++) {
					dist = getDistanceBetweenPoints3D(this.tableBet2Vector, new mp.Vector3(rouletteTables[this.selectTable][1] + tableChipsOffsets[i][0], rouletteTables[this.selectTable][2] + tableChipsOffsets[i][1], rouletteTables[this.selectTable][3] + tableChipsOffsets[i][2]));
					if(dist <= prevDistance) {
						spot = i;
						prevDistance = dist;
					}
				}
				for(let i = 0; i < tableChipsOffsets[spot][3].length; i++) {
						key = tableChipsOffsets[spot][3][i];
						if(key == "00" || key == "0") {
							await global.loadModel(269022546);
							obj = mp.objects.new(269022546, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2]));
							obj.setCollision(false, false);
							this.tableBetsMarkers.push(obj);
						}
						else {
							await global.loadModel(3267450776);
							this.tableBetsMarkers.push(mp.objects.new(3267450776, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2])));
						}
					}
			}
			if(this.tableBet3Vector != null) {
				spot = null;
				dist = null;
				prevDistance = 0.05;
				for(let i = 0; i < tableChipsOffsets.length; i++) {
					dist = getDistanceBetweenPoints3D(this.tableBet3Vector, new mp.Vector3(rouletteTables[this.selectTable][1] + tableChipsOffsets[i][0], rouletteTables[this.selectTable][2] + tableChipsOffsets[i][1], rouletteTables[this.selectTable][3] + tableChipsOffsets[i][2]));
					if(dist <= prevDistance) {
						spot = i;
						prevDistance = dist;
					}
				}
				for(let i = 0; i < tableChipsOffsets[spot][3].length; i++) {
						key = tableChipsOffsets[spot][3][i];
						if(key == "00" || key == "0") {
							await global.loadModel(269022546);
							obj = mp.objects.new(269022546, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2]));
							obj.setCollision(false, false);
							this.tableBetsMarkers.push(obj);
						}
						else {
							await global.loadModel(3267450776);
							this.tableBetsMarkers.push(mp.objects.new(3267450776, new mp.Vector3(rouletteTables[this.selectTable][1] + tableMarkersOffsets[key][0], rouletteTables[this.selectTable][2] + tableMarkersOffsets[key][1], rouletteTables[this.selectTable][3] + tableMarkersOffsets[key][2])));
						}
					}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "getChipsSpots", e.toString());
		}	
    }

    /**
     * Очищает подсветку ставок
     */
    clearTableMarkers() {
		try 
		{
			this.tableMarkers.forEach((marker) => { marker.destroy(); });
			this.tableMarkers = [];
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "clearTableMarkers", e.toString());
		}
    }
	
	/**
     * Очищает подсветку моих ставок
     */
    clearMyBetsTableMarkers() {
		try 
		{
			this.tableBet1Vector = null;
			this.tableBet2Vector = null;
			this.tableBet3Vector = null;
			this.tableBetsMarkers.forEach((marker) => { marker.destroy(); });
			this.tableBetsMarkers = [];
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "clearMyBetsTableMarkers", e.toString());
		}
    }

    /**
     * Получает данные с райкаста
     */
    getCameraHitCoord() {
		try 
		{
			const position = this.casinoCamera.getCoord();
			const direction = this.casinoCamera.getDirection();
			const farAway = new mp.Vector3((direction.x * 150) + position.x, (direction.y * 150) + position.y, (direction.z * 150) + position.z);
			
			const hitData = mp.raycasting.testPointToPoint(position, farAway, global.localplayer);
			
			if(hitData != undefined) return hitData;
			return null;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "casino/roulette", "getCameraHitCoord", e.toString());
			return null;
		}
    }

    /**
     * Получаем translateText("исправленный") вектор
     * @param vector - вектор
     */
    getNormalizedVector(vector) {
        const mag = Math.sqrt(vector.x * vector.x + vector.y * vector.y + vector.z * vector.z);
        vector.x = vector.x / mag;
        vector.y = vector.y / mag;
        vector.z = vector.z / mag;
        return vector;
    }

    /**
     * Получаем смешанный вектор
     * @param v1 - первый вектор 
     * @param v2 - второй вектор
     */    
    getCrossProduct(v1, v2) {
        let vector = new mp.Vector3(0, 0, 0);
        vector.x = v1.y * v2.z - v1.z * v2.y;
        vector.y = v1.z * v2.x - v1.x * v2.z;
        vector.z = v1.x * v2.y - v1.y * v2.x;
        return vector;
    }
}

const casino = new Casino();