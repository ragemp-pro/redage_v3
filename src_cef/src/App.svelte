<script>
	let isMultiplayer = window.mp && window.mp.events;

	document.api = "https://api.redage.net/";

	import 'lang/index'
	//import '@/advertisement';
	import router from 'router';
	import axios from 'axios';
	import '@/views/assets/css/main.js';
	import '@/views/assets/fonts/main.js';
	import 'components/rangeslider/main.css'
	import 'api/events'
	import 'api/imgSave'
	import 'store/customization';

	import 'store/account';
    import { charIsPet } from 'store/chars'
	import 'store/loader';
	import 'store/keys';
	import { inVehicle, isInputToggled } from 'store/hud';
	import 'store/quest';
	import 'store/settings';
    import { storeAnimBind } from 'store/animation'
    import { newsData } from 'store/news'

    import { executeClient } from 'api/rage'
	import ViewContainer from 'components/viewcontainer/index.svelte';
	import PopusContainer from 'components/popuscontainer/index.svelte';
	import FadeContainer from 'components/fadecontainer/index.svelte';
	import FilterBlur from './components/filterblur/index.svelte';

	//Игрока'
	import PlayerBattlePass from '@/views/player/battlepass/index.svelte';
	import PlayerRentCar from '@/views/player/rentcar/index.svelte';
	import PlayerAuthentication from '@/views/player/newauthentication/index.svelte';
	import PlayerCustomization from '@/views/player/customization/index.svelte';
	import PlayerAtm from '@/views/player/atm/index.svelte';
	import PlayerBinder from '@/views/player/binder/index.svelte';
	import PlayerDocumets from '@/views/player/documets/index.svelte';
	import PlayerClothesEditor from '@/views/player/clothesEditor/index.svelte';
	import PlayerCarMarket from '@/views/player/carmarket/index.svelte';
	import PlayerHelp from '@/views/player/help/index.svelte';
	import PlayerGameMenu from '@/views/player/menu/index.svelte';
	import PlayerLicense from '@/views/player/license/index.svelte';
	import PlayerPassport from '@/views/player/passport/index.svelte';
	import PlayerDropinfo from '@/views/player/dropinfo/index.svelte';
	import PlayerHud from '@/views/player/hudevo/index.svelte';
	import PlayerLift from '@/views/player/lift/index.svelte';
	import PlayerGasStation from '@/views/player/gasStation/index.svelte';
	import PlayerBreakingLock from '@/views/player/breakingLock/index.svelte';
	import PlayerTransfer from '@/views/player/transfer/index.svelte';
	import PlayerJobSelector from '@/views/player/jobselector/index.svelte';
	import PlayerReports from '@/views/player/reports/index.svelte';

	//import PlayerNewYearStats from '@/views/player/newyearstats/index.svelte';
	import PlayerAnimations from '@/views/player/animations/index.svelte';
	import PlayerOresSale from '@/views/player/oressale/index.svelte';
	import PlayerMetro from '@/views/player/metro/index.svelte';
	import PlayerWedding from '@/views/player/wedding/index.svelte';
	import PlayerTickets from '@/views/player/tickets/index.svelte';
	import PlayerRestart from '@/views/player/restart/index.svelte';

	//Администрации

	//import AdminPlayersView from '@/views/admin/playersView/index.svelte';
	//import AdminReport from '@/views/admin/report/index.svelte';

	//Бизнес
	import BusinessAutoShop from '@/views/business/autoshop/index.svelte';
	import BusinessWeaponShop from '@/views/business/weaponshop/index.svelte';
	import BusinessPetShop from '@/views/business/petshop/index.svelte';
	import BusinessMenu from '@/views/business/menu/index.svelte';
	import BusinessNewPetShop from '@/views/business/newpetshop/index.svelte';
	import BusinessClothes from '@/views/business/clothes/index.svelte';

	//Фракции
	// /import FractionsAdverts from '@/views/fractions/adverts/index.svelte';
	import FractionsBSearch from '@/views/fractions/bsearch/index.svelte';
	import FractionsMats from '@/views/fractions/mats/index.svelte';
	import FractionsCraft from '@/views/fractions/craft/index.svelte';
	import FractionsCreate from '@/views/fractions/create/index.svelte';
	import FractionsStock from '@/views/fractions/stock/index.svelte';
	import FractionsPolicecomputer from '@/views/fractions/policecomputer/index.svelte';
	import FractionsTicket from '@/views/fractions/ticket/index.svelte';
	import FractionsBortovoi from '@/views/fractions/bortovoi/index.svelte';
	import FractionsWeazelNews from '@/views/fractions/weazelnews/index.svelte';
	import FractionsWar from '@/views/fractions/war/index.svelte';

	//Казино
	import CasinoBlackjack from '@/views/casino/blackjack/index.svelte';
	import CasinoHorse from '@/views/casino/horse/index.svelte';
	import CasinoJacpot from '@/views/casino/jacpot/index.svelte';
	import CasinoRoullete from '@/views/casino/roullete/index.svelte';

	import VehicleAir from '@/views/vehicle/air/index.svelte';
	import VehicleLsCustom from '@/views/vehicle/lscustom/index.svelte';

	//Games
	import GamesOtherMain from '@/views/games/other/index.svelte';
	import GamesOtherMafia from '@/views/games/other/mafia/index.svelte';
	import GamesOtherLobby from '@/views/games/other/lobby/index.svelte';

	//
	import DonateMain from '@/views/donate/main/index.svelte';
	//import DonateCards from '@/views/donate/cards/index.svelte';
	import DonateSapper from '@/views/donate/sapper/index.svelte';


	import QuestsDialog from '@/views/quests/dialog/index.svelte';

	//import QuestsDialog1 from '@/views/quests/questsnew/questsnewdialog/index.svelte';
	//import QuestsDialog2 from '@/views/quests/questsnew/questsnewlist/index.svelte';
	//import QuestsDialog3 from '@/views/quests/questsnew/questsprise/index.svelte';

	import EventsValentine from '@/views/events/valentine/index.svelte';

	//House
	import HouseMenu from '@/views/house/menu/index.svelte';
	import HouseRielt from '@/views/house/rieltagency/index.svelte';
	import HouseBuy from '@/views/house/buymenu/index.svelte';
	import HouseFurniture from '@/views/house/furniture/index.svelte';


	const Views = {
		PlayerBattlePass,
		//PlayerNewAuthentication,
		PlayerAuthentication,//Авторизация, выбор спавна, выбор чара
		PlayerCustomization,//кастомизация
		PlayerAtm,//Банкоматы
		PlayerBinder,//биндер
		PlayerDocumets,//Документы фракционные
		PlayerClothesEditor,//Эдитор одежды (только для тестов)
		PlayerCarMarket,// авторынок НУЖНО СДЕЛАТЬ СЕЙЧАС НЕ РАБОТАЕТ
		PlayerHelp,//Хелп
		PlayerLicense,//Лицензии
		PlayerPassport,//паспорт
		PlayerTransfer,// Перенос
		PlayerJobSelector,//Выбор работы
		PlayerReports,//Репорты
		PlayerDropinfo,//Подсказки для кнопки 'поставить'
		PlayerLift,//Лифт
		PlayerGasStation,//Заправка
		PlayerBreakingLock,//Взлом сейфа
		PlayerRentCar,//Аренда авто
		//PlayerNewYearStats,
		PlayerAnimations,
		PlayerOresSale,//Продажа руд
		PlayerMetro,
		PlayerWedding,
		PlayerTickets,
		PlayerRestart,

		BusinessAutoShop,
		BusinessWeaponShop,
		BusinessPetShop,
		BusinessMenu,//24/7, черный рынок
		BusinessNewPetShop, //новый магазин животных
		BusinessClothes,

		//FractionsAdverts,//лс невс
		FractionsBSearch, //Протокол обыска
		FractionsMats,//Материал
		FractionsCraft,//Крафт
		FractionsStock,//Склад
		FractionsPolicecomputer,
		FractionsTicket,
		FractionsBortovoi,
		FractionsWeazelNews,
		FractionsWar,
		FractionsCreate,

		CasinoBlackjack,
		CasinoHorse,
		CasinoJacpot,
		CasinoRoullete,

		//AdminPlayersView,//Не работает
		//AdminReport,

		VehicleAir,
		VehicleLsCustom,//Тюнинг

		GamesOtherMain,//Меню основных мероприятий
		GamesOtherMafia,//Игра Мафия
		GamesOtherLobby,//Меню лобби

		DonateMain,
		//DonateCards,
		DonateSapper,

		QuestsDialog,
		//QuestsDialog1,
		//QuestsDialog2,
		//QuestsDialog3,
		EventsValentine,

		HouseBuy,
		HouseMenu,
		HouseRielt,
		HouseFurniture
	}
	
	//window.router.setView("PlayerAuthentication")
	window.router.setHud()
	//window.router.setView("FractionsCreate")
	//window.router.close()
	//window.router.updateStatic("PlayerGameMenu");

	//Popups
	import PopupInput from '@/popups/input/index.svelte';
	import PopupConfirm from '@/popups/confirm/index.svelte';
	import HospitalPopupConfirm from '@/popups/confirm/hospital_index.svelte';
	import PopupDonate from '@/popups/donate/index.svelte';
	import PopupDeath from '@/popups/death/index.svelte';
	import CircleMenu from '@/popups/circle/index.svelte';
	//import PopupAuth from '@/popups/auth/index.svelte';
	import PopupMain from '@/popups/main/index.svelte';
	import PopupSelect from '@/popups/select/index.svelte';
	import PopupRoulette from '@/popups/roulette/index.svelte';
	import PopupUpgrade from '@/popups/upgrade/index.svelte';
	import PopupCamera from '@/popups/camera/index.svelte';
	import PopupWar from '@/popups/war/index.svelte';

	const Popus = {
		PopupConfirm,
		HospitalPopupConfirm,
		PopupInput,
		PopupDeath,
		PopupDonate,
		CircleMenu,
		PopupSelect,
		//PopupAuth,
		PopupMain,
		PopupUpgrade,
		PopupRoulette,
		PopupCamera,
		PopupWar
	}
	//window.router.setPopUp("PopupWar")

	/*axios.get(`https://redage.net/?getnews=1`).then(res => {
		if (res && res.data && res.data.result) {
			newsData.set(res.data.result);
		}
	});*/

	import { onMount } from 'svelte';

	onMount(() => {
		executeClient ("client:OnBrowserInit");
		if (!isMultiplayer) {
			const body = document.querySelector('body');
            body.style.background = "black";
			window.FadeScreen (false, 0);
			window.initCustomizations ();
			window.events.callEvent("cef.inventory.InitData", '{"accessories":[{"SqlId":5976,"ItemId":-4,"Count":1,"Data":"83_1_True","Index":9,"Price":0},{"SqlId":5972,"ItemId":-6,"Count":1,"Data":"79_0_True","Index":13,"Price":0}],"inventory":[{"SqlId":5971,"ItemId":100,"Count":1,"Data":"15_3_True","Index":0,"Price":0},{"SqlId":5970,"ItemId":-12,"Count":1,"Data":"13_6_True","Index":1,"Price":0},{"SqlId":5978,"ItemId":-11,"Count":1,"Data":"107_2_True","Index":8,"Price":0},{"SqlId":5995,"ItemId":229,"Count":1,"Data":"419","Index":23,"Price":0}],"fastSlots":[{"SqlId":5997,"ItemId":230,"Count":1,"Data":"","Index":1,"Price":0},{"SqlId":6005,"ItemId":263,"Count":1,"Data":"","Index":2,"Price":0},{"SqlId":6006,"ItemId":263,"Count":1,"Data":"","Index":3,"Price":0}]}', true)
			window.events.callEvent("cef.inventory.InitOtherData", 10, 1, '[{"SqlId":29,"ItemId":220,"Count":1,"Data":"Camry","Index":0,"Price":0},{"SqlId":30,"ItemId":220,"Count":1,"Data":"Quad1","Index":1,"Price":0},{"SqlId":206,"ItemId":120,"Count":1,"Data":"DRoulette","Index":2,"Price":0},{"SqlId":207,"ItemId":220,"Count":1,"Data":"BmwM5","Index":3,"Price":0},{"SqlId":208,"ItemId":111,"Count":1,"Data":"DRoulette","Index":4,"Price":0},{"SqlId":209,"ItemId":145,"Count":1,"Data":"DRoulette","Index":5,"Price":0},{"SqlId":210,"ItemId":220,"Count":1,"Data":"Schlagen","Index":6,"Price":0},{"SqlId":211,"ItemId":220,"Count":1,"Data":"Bugatti","Index":7,"Price":0},{"SqlId":49,"ItemId":223,"Count":1,"Data":"","Index":8,"Price":0},{"SqlId":67,"ItemId":19,"Count":1,"Data":"DB11_E472I","Index":10,"Price":0},{"SqlId":75,"ItemId":19,"Count":1,"Data":"COMET2_U037X","Index":11,"Price":0},{"SqlId":76,"ItemId":234,"Count":1,"Data":"1","Index":12,"Price":0},{"SqlId":79,"ItemId":226,"Count":1,"Data":"1","Index":13,"Price":0},{"SqlId":138,"ItemId":19,"Count":1,"Data":"JESKO_A128G","Index":14,"Price":0},{"SqlId":139,"ItemId":226,"Count":1,"Data":"1","Index":15,"Price":0},{"SqlId":141,"ItemId":227,"Count":1,"Data":"1","Index":16,"Price":0},{"SqlId":142,"ItemId":228,"Count":1,"Data":"1","Index":17,"Price":0},{"SqlId":221,"ItemId":220,"Count":1,"Data":"BmwM5","Index":18,"Price":0},{"SqlId":222,"ItemId":220,"Count":1,"Data":"Schlagen","Index":19,"Price":0},{"SqlId":223,"ItemId":145,"Count":1,"Data":"DRoulette","Index":20,"Price":0},{"SqlId":224,"ItemId":-1,"Count":1,"Data":"183_0_True","Index":21,"Price":0},{"SqlId":225,"ItemId":-1,"Count":1,"Data":"183_0_True","Index":22,"Price":0},{"SqlId":226,"ItemId":-13,"Count":1,"Data":"25_0_True","Index":23,"Price":0},{"SqlId":432,"ItemId":-11,"Count":1,"Data":"405_0_True","Index":26,"Price":0},{"SqlId":433,"ItemId":-5,"Count":1,"Data":"95_0_True","Index":27,"Price":0},{"SqlId":434,"ItemId":-1,"Count":1,"Data":"182_0_True","Index":28,"Price":0},{"SqlId":435,"ItemId":-4,"Count":1,"Data":"120_0_True","Index":29,"Price":0},{"SqlId":1046,"ItemId":220,"Count":1,"Data":"Veto","Index":30,"Price":0},{"SqlId":1047,"ItemId":220,"Count":1,"Data":"Openwheel2","Index":31,"Price":0},{"SqlId":1048,"ItemId":220,"Count":1,"Data":"Speedo2","Index":32,"Price":0},{"SqlId":1049,"ItemId":220,"Count":1,"Data":"Speedo2","Index":33,"Price":0},{"SqlId":1050,"ItemId":220,"Count":1,"Data":"Tezeract","Index":34,"Price":0},{"SqlId":2387,"ItemId":-11,"Count":1,"Data":"430_0_False","Index":35,"Price":0},{"SqlId":2388,"ItemId":-1,"Count":1,"Data":"182_0_False","Index":36,"Price":0},{"SqlId":2392,"ItemId":-5,"Count":1,"Data":"95_0_False","Index":37,"Price":0},{"SqlId":2391,"ItemId":-12,"Count":1,"Data":"100_0_False","Index":38,"Price":0},{"SqlId":2389,"ItemId":-11,"Count":1,"Data":"430_0_False","Index":39,"Price":0},{"SqlId":2390,"ItemId":-12,"Count":1,"Data":"100_0_False","Index":40,"Price":0},{"SqlId":2397,"ItemId":-11,"Count":1,"Data":"372_0_False","Index":41,"Price":0},{"SqlId":2396,"ItemId":-12,"Count":1,"Data":"100_0_False","Index":42,"Price":0},{"SqlId":2395,"ItemId":-11,"Count":1,"Data":"372_0_False","Index":43,"Price":0},{"SqlId":2393,"ItemId":-13,"Count":1,"Data":"26_0_False","Index":44,"Price":0},{"SqlId":2394,"ItemId":-11,"Count":1,"Data":"372_0_False","Index":45,"Price":0},{"SqlId":2398,"ItemId":220,"Count":1,"Data":"MBG63","Index":46,"Price":0},{"SqlId":2399,"ItemId":220,"Count":1,"Data":"LX570","Index":47,"Price":0},{"SqlId":2401,"ItemId":220,"Count":1,"Data":"Nero2","Index":48,"Price":0},{"SqlId":2400,"ItemId":220,"Count":1,"Data":"Prototipo","Index":49,"Price":0},{"SqlId":2403,"ItemId":220,"Count":1,"Data":"Camper","Index":50,"Price":0},{"SqlId":2404,"ItemId":220,"Count":1,"Data":"Openwheel2","Index":51,"Price":0},{"SqlId":2402,"ItemId":220,"Count":1,"Data":"Nero2","Index":52,"Price":0},{"SqlId":2695,"ItemId":248,"Count":18,"Data":"0","Index":53,"Price":0},{"SqlId":2696,"ItemId":248,"Count":6,"Data":"1","Index":54,"Price":0},{"SqlId":2697,"ItemId":248,"Count":1,"Data":"2","Index":55,"Price":0},{"SqlId":2698,"ItemId":250,"Count":100,"Data":"0","Index":56,"Price":0},{"SqlId":2699,"ItemId":250,"Count":15,"Data":"1","Index":57,"Price":0},{"SqlId":2700,"ItemId":250,"Count":1,"Data":"2","Index":58,"Price":0},{"SqlId":2702,"ItemId":250,"Count":18,"Data":"0","Index":59,"Price":0},{"SqlId":2703,"ItemId":250,"Count":6,"Data":"1","Index":60,"Price":0},{"SqlId":2704,"ItemId":250,"Count":1,"Data":"2","Index":61,"Price":0},{"SqlId":2709,"ItemId":220,"Count":1,"Data":"Cheburek","Index":63,"Price":0},{"SqlId":2711,"ItemId":250,"Count":1,"Data":"0","Index":64,"Price":0},{"SqlId":2713,"ItemId":111,"Count":1,"Data":"DRoulette","Index":65,"Price":0},{"SqlId":2712,"ItemId":250,"Count":1,"Data":"0","Index":66,"Price":0},{"SqlId":2716,"ItemId":220,"Count":1,"Data":"Toros","Index":67,"Price":0},{"SqlId":2714,"ItemId":250,"Count":1,"Data":"0","Index":68,"Price":0},{"SqlId":2715,"ItemId":250,"Count":1,"Data":"0","Index":69,"Price":0},{"SqlId":2717,"ItemId":250,"Count":1,"Data":"0","Index":70,"Price":0},{"SqlId":2726,"ItemId":220,"Count":1,"Data":"BmwM5","Index":71,"Price":0},{"SqlId":2727,"ItemId":250,"Count":1,"Data":"0","Index":72,"Price":0},{"SqlId":2781,"ItemId":250,"Count":1,"Data":"","Index":73,"Price":0},{"SqlId":2796,"ItemId":250,"Count":1,"Data":"","Index":74,"Price":0},{"SqlId":2797,"ItemId":220,"Count":1,"Data":"Schlagen","Index":75,"Price":0},{"SqlId":2798,"ItemId":250,"Count":1,"Data":"","Index":76,"Price":0},{"SqlId":2799,"ItemId":250,"Count":1,"Data":"","Index":77,"Price":0},{"SqlId":2800,"ItemId":1,"Count":1,"Data":"DRoulette","Index":78,"Price":0},{"SqlId":2801,"ItemId":-9,"Count":1,"Data":"100","Index":79,"Price":0},{"SqlId":2802,"ItemId":1,"Count":1,"Data":"DRoulette","Index":80,"Price":0},{"SqlId":2805,"ItemId":260,"Count":25,"Data":"","Index":81,"Price":0},{"SqlId":2810,"ItemId":250,"Count":1,"Data":"","Index":82,"Price":0},{"SqlId":200,"ItemId":-8,"Count":1,"Data":"0_2_False","Index":83,"Price":0},{"SqlId":187,"ItemId":19,"Count":1,"Data":"IMPREZA08_C744V","Index":84,"Price":0},{"SqlId":2406,"ItemId":237,"Count":1,"Data":"","Index":85,"Price":0},{"SqlId":190,"ItemId":-8,"Count":1,"Data":"0_3_True","Index":86,"Price":0},{"SqlId":191,"ItemId":-4,"Count":1,"Data":"103_4_True","Index":87,"Price":0},{"SqlId":1626,"ItemId":-1,"Count":1,"Data":"191_0_True","Index":88,"Price":0},{"SqlId":146,"ItemId":232,"Count":1,"Data":"1","Index":89,"Price":0},{"SqlId":37,"ItemId":225,"Count":1,"Data":"100","Index":90,"Price":0},{"SqlId":3019,"ItemId":200,"Count":4,"Data":"","Index":91,"Price":0},{"SqlId":1586,"ItemId":-1,"Count":1,"Data":"191_0_True","Index":92,"Price":0},{"SqlId":203,"ItemId":-8,"Count":1,"Data":"0_3_True","Index":93,"Price":0},{"SqlId":1604,"ItemId":222,"Count":1,"Data":"123123123","Index":94,"Price":0},{"SqlId":1715,"ItemId":228,"Count":1,"Data":"","Index":95,"Price":0},{"SqlId":3042,"ItemId":257,"Count":3,"Data":"","Index":96,"Price":0},{"SqlId":3048,"ItemId":256,"Count":3,"Data":"0","Index":97,"Price":0},{"SqlId":144,"ItemId":230,"Count":1,"Data":"1","Index":98,"Price":0},{"SqlId":2792,"ItemId":14,"Count":1,"Data":"123123123","Index":99,"Price":0},{"SqlId":3501,"ItemId":250,"Count":1,"Data":"","Index":100,"Price":0},{"SqlId":3502,"ItemId":251,"Count":1,"Data":"","Index":101,"Price":0},{"SqlId":202,"ItemId":-6,"Count":1,"Data":"13_2_False","Index":102,"Price":0},{"SqlId":3463,"ItemId":249,"Count":1,"Data":"100","Index":103,"Price":0},{"SqlId":1312,"ItemId":13,"Count":250,"Data":"1","Index":104,"Price":0},{"SqlId":3673,"ItemId":-11,"Count":1,"Data":"40_0_True","Index":105,"Price":0},{"SqlId":3671,"ItemId":-12,"Count":1,"Data":"40_0_True","Index":106,"Price":0},{"SqlId":3522,"ItemId":-12,"Count":1,"Data":"7_0_True","Index":107,"Price":0},{"SqlId":3672,"ItemId":-11,"Count":1,"Data":"6_0_True","Index":108,"Price":0},{"SqlId":3523,"ItemId":-8,"Count":1,"Data":"0_0_True","Index":109,"Price":0},{"SqlId":3676,"ItemId":-12,"Count":1,"Data":"130_0_True","Index":110,"Price":0},{"SqlId":3678,"ItemId":-8,"Count":1,"Data":"44_0_True","Index":111,"Price":0},{"SqlId":3680,"ItemId":-13,"Count":1,"Data":"22_0_True","Index":112,"Price":0},{"SqlId":3677,"ItemId":-8,"Count":1,"Data":"71_0_True","Index":113,"Price":0},{"SqlId":3686,"ItemId":-4,"Count":1,"Data":"6_0_True","Index":114,"Price":0},{"SqlId":3682,"ItemId":-11,"Count":1,"Data":"83_0_True","Index":115,"Price":0},{"SqlId":3688,"ItemId":-12,"Count":1,"Data":"13_0_True","Index":116,"Price":0},{"SqlId":3687,"ItemId":-12,"Count":1,"Data":"7_0_True","Index":117,"Price":0},{"SqlId":3690,"ItemId":-13,"Count":1,"Data":"17_0_True","Index":118,"Price":0},{"SqlId":3689,"ItemId":-13,"Count":1,"Data":"5_0_True","Index":119,"Price":0},{"SqlId":3684,"ItemId":-8,"Count":1,"Data":"44_0_True","Index":120,"Price":0},{"SqlId":3728,"ItemId":250,"Count":1,"Data":"","Index":121,"Price":0},{"SqlId":3880,"ItemId":220,"Count":1,"Data":"Trophytruck","Index":123,"Price":0},{"SqlId":3881,"ItemId":220,"Count":1,"Data":"Trophytruck","Index":124,"Price":0},{"SqlId":3882,"ItemId":220,"Count":1,"Data":"Trophytruck","Index":125,"Price":0},{"SqlId":3883,"ItemId":220,"Count":1,"Data":"Trophytruck","Index":126,"Price":0},{"SqlId":3884,"ItemId":220,"Count":1,"Data":"Trophytruck","Index":127,"Price":0},{"SqlId":4548,"ItemId":250,"Count":1,"Data":"","Index":128,"Price":0},{"SqlId":4582,"ItemId":250,"Count":1,"Data":"","Index":129,"Price":0},{"SqlId":4589,"ItemId":250,"Count":1,"Data":"","Index":130,"Price":0},{"SqlId":4590,"ItemId":250,"Count":1,"Data":"","Index":131,"Price":0},{"SqlId":4591,"ItemId":250,"Count":1,"Data":"","Index":132,"Price":0},{"SqlId":6077,"ItemId":250,"Count":1,"Data":"","Index":133,"Price":0},{"SqlId":6311,"ItemId":220,"Count":1,"Data":"BMWE38","Index":134,"Price":0},{"SqlId":6313,"ItemId":250,"Count":1,"Data":"","Index":135,"Price":0},{"SqlId":6312,"ItemId":114,"Count":1,"Data":"DRoulette","Index":136,"Price":0},{"SqlId":6310,"ItemId":250,"Count":1,"Data":"","Index":137,"Price":0},{"SqlId":6307,"ItemId":189,"Count":1,"Data":"DRoulette","Index":138,"Price":0},{"SqlId":6317,"ItemId":189,"Count":1,"Data":"DRoulette","Index":139,"Price":0},{"SqlId":6309,"ItemId":121,"Count":1,"Data":"DRoulette","Index":140,"Price":0},{"SqlId":6316,"ItemId":250,"Count":1,"Data":"","Index":141,"Price":0},{"SqlId":6314,"ItemId":114,"Count":1,"Data":"DRoulette","Index":142,"Price":0},{"SqlId":6306,"ItemId":250,"Count":1,"Data":"","Index":143,"Price":0},{"SqlId":6308,"ItemId":250,"Count":1,"Data":"","Index":144,"Price":0},{"SqlId":6315,"ItemId":189,"Count":1,"Data":"DRoulette","Index":145,"Price":0},{"SqlId":6338,"ItemId":121,"Count":1,"Data":"DRoulette","Index":146,"Price":0},{"SqlId":6339,"ItemId":121,"Count":1,"Data":"DRoulette","Index":147,"Price":0},{"SqlId":6340,"ItemId":250,"Count":1,"Data":"","Index":148,"Price":0},{"SqlId":6452,"ItemId":250,"Count":1,"Data":"","Index":149,"Price":0},{"SqlId":6453,"ItemId":250,"Count":1,"Data":"","Index":150,"Price":0},{"SqlId":6454,"ItemId":250,"Count":1,"Data":"","Index":151,"Price":0},{"SqlId":6835,"ItemId":250,"Count":1,"Data":"","Index":152,"Price":0},{"SqlId":8177,"ItemId":250,"Count":1,"Data":"","Index":153,"Price":0},{"SqlId":8184,"ItemId":250,"Count":1,"Data":"","Index":154,"Price":0},{"SqlId":8196,"ItemId":250,"Count":1,"Data":"","Index":155,"Price":0},{"SqlId":8197,"ItemId":250,"Count":1,"Data":"","Index":156,"Price":0},{"SqlId":8198,"ItemId":250,"Count":1,"Data":"","Index":157,"Price":0},{"SqlId":8276,"ItemId":252,"Count":1,"Data":"","Index":158,"Price":0},{"SqlId":8387,"ItemId":252,"Count":1,"Data":"","Index":159,"Price":0},{"SqlId":8388,"ItemId":252,"Count":1,"Data":"","Index":160,"Price":0},{"SqlId":8389,"ItemId":252,"Count":1,"Data":"","Index":161,"Price":0},{"SqlId":8649,"ItemId":250,"Count":1,"Data":"","Index":162,"Price":0}]', 163, false, false, '[]');

			//window.hudStore.isHudNewPhone(true)

			for(let i = 0; i < 9; i++)
				window.chat.addMessage(i + " test тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс тектс")

			isInputToggled.set(true)
		} else {
			//window.router.close ();
			window.router.setView('PlayerAuthentication');
		}
	});
	//window.router.updateStatic("PlayerGameMenu", [{ActorName:"", Line:""}])

    import keys from 'store/keys'

	let isKeyBind = -1;
	let isKeyBindUse = false;
	const keyToBind = {
		48: 9,
		49: 0,
		50: 1,
		51: 2,
		52: 3,
		53: 4,
		54: 5,
		55: 6,
		56: 7,
		57: 8,
	}

	let isKeyDown = false;

	window.SetBindToKey = (key) => {
		if (key === -1)
			isKeyDown = false;
		else
			isKeyDown = true;

		isKeyBind = key;

		executeClient ("setBindToKey", key);
	}

	let fastClickData = {};

	const FastClick = (key, awaitFunc, Func) => {
		if (fastClickData [key]) {
			clearTimeout(fastClickData [key]);
			delete fastClickData [key];
			Func()
		} else {
			fastClickData [key] = setTimeout(() => {
				delete fastClickData [key];
				awaitFunc ();
			}, 250)
		}
	}

	const handleKeydown = (event) => {
		if (!$router.PlayerHud)
			return;
		else if ($inVehicle || $isInputToggled)
			return;
		const { keyCode } = event;

		if (isKeyBind !== -1) {
			if (isKeyBind === $keys[8]) {
				const anim = $storeAnimBind;
				if (anim) {
					for (let i = 0; i < 11; i++) {
						if (keyCode === (48 + i) && anim[keyToBind [48 + i]] && anim[keyToBind [48 + i]].split('_')) {
							executeClient ("client.animation.play", anim[keyToBind [48 + i]]);
							isKeyBindUse = true;
							window.SetBindToKey (-1);
							return;
						}
					}
				}
			} else if (isKeyBind === $keys[55] && keyCode === $keys[55]) {
				window.hudStore.isAnimal (false);
				executeClient ("client.pet.isUse", false);
				window.SetBindToKey (-1);
			}
		} else {
			if (isKeyDown)
				return;

			if (keyCode === $keys[8]) {
				isKeyBindUse = false;
				window.SetBindToKey ($keys[8]);
			}
			else if (keyCode === 32) {
				executeClient ("client.animation.stop");
			}
			else if($charIsPet && keyCode === $keys[55]) {
				window.hudStore.isAnimal (true);
				executeClient ("client.pet.isUse", true);
				window.SetBindToKey ($keys[55]);
			}
		}
	}

	const handleKeyup = (event) => {
		if (!isKeyDown)
			return;

		if (!$router.PlayerHud)
			return;
		else if ($inVehicle || $isInputToggled)
			return;

		const { keyCode } = event;

		if (keyCode === $keys[8]) {
			window.SetBindToKey (-1);
			if (!isKeyBindUse) {
				executeClient ("client.animation.open");
			}
		}
	}



</script>

<svelte:window on:keydown={handleKeydown} on:keyup={handleKeyup} />

<FadeContainer />
<ViewContainer visible={Popus[$router.popup] ? true : false} opacity={$router.opacity && $router.popup !== "PopupCamera" ? 1 : 0}>
	{#if Views[$router.view]}
	<svelte:component this={Views[$router.view]} viewData={$router.viewData} />
	{/if}
	<!--<UiFilter/>-->
	<PlayerGameMenu visible={$router.PlayerGameMenu} />
	<PlayerHud visible={$router.PlayerHud} />
	<!--<svelte:component this={Views.PlayerGameMenu} visible={$router.PlayerGameMenu} />
	<svelte:component this={Views.PlayerHud} visible={$router.PlayerHud} />
	<svelte:component this={Views.QuestsDialog} />-->
</ViewContainer>

<PopusContainer visible={Popus[$router.popup] ? true : false} opacity={$router.opacity}>
	<svelte:component this={Popus[$router.popup]} popupData={$router.popupData} popupFunc={$router.popupFunc} />
</PopusContainer>