let alcoUI = null;
global.openAlco = false;
const ClubNames = {
    10: "Bahama Mamas West",
    11: "Vanila Unicorn",
    12: "Tequi-la-la",
    13: "Diamond Penthouse",
};

const ClubAlcos = {
    10: ["«Martini Asti»", "«Sambuca»", "«Campari»"],
    11: [translateText("«На корке лимона»"), translateText("«На бруснике»"), translateText("«Русский стандарт»")],
    12: ["«Asahi»", "«Midori»", "«Yamazaki»"],
    13: [translateText("«Дживан»"), translateText("«Арарат»"), "«Noyan Tapan»"],
};

const ClubDrinks = [75, 115, 150];
var selectedAlco = 0;

gm.events.add("openAlco", (club, modief, isOwner, stock) => {
	try
	{
		if (global.openAlco || alcoUI) alcoUI.Close();
		setTimeout(() => {
			global.openAlco = true;
			selectedAlco = 0;
			global.menuOpen();
			mp.gui.cursor.visible = false;
			var res = mp.game.graphics.getScreenActiveResolution(0, 0);
			const UIPositions = {
				RightMiddle: new global.Point(res.x - 180, res.y / 2),
				LeftMiddle: new global.Point(0, res.y / 2 - 200),
			};
			alcoUI = new global.NativeMenu(translateText("Клуб"), ClubNames[club], UIPositions.LeftMiddle);
			gm.discord(translateText("Выбирает алкоголь в клубе"));
		
			var drinks = [` ${ClubAlcos[club][0]} ${(ClubDrinks[0] * modief).toFixed()}$`,
				` ${ClubAlcos[club][1]} ${(ClubDrinks[1] * modief).toFixed()}$`,
				` ${ClubAlcos[club][2]} ${(ClubDrinks[2] * modief).toFixed()}$`];
		
			alcoUI.AddItem(new UIMenuListItem(
				translateText("Напитки"),
				translateText("Вы можете выбрать любой напиток"),
				new ItemsCollection(drinks)
			));
		
			if (isOwner) {

				alcoUI.AddItem(new UIMenuItem(translateText("Инфо"), translateText("Материалы: {0}\n{1} - {2}\n{3} - {4}\n{5} - {6}", stock[0], ClubAlcos[club][0], stock[1], ClubAlcos[club][1], stock[2], ClubAlcos[club][2], stock[3])));
				alcoUI.AddItem(new UIMenuItem(translateText("Взять"), translateText("Взять выбранный напиток со склада")));
				alcoUI.AddItem(new UIMenuItem(translateText("Скрафтить"), translateText("Скрафтить выбранный напиток")));
				alcoUI.AddItem(new UIMenuItem(translateText("Установить цену"), translateText("Установить модификатор цены для всех продуктов (от 50% до 150%)")));
			}
		
			alcoUI.AddItem(new UIMenuItem(translateText("Купить"), translateText("Купить выбранный напиток")));
		
			var uiItem = new UIMenuItem(translateText("Закрыть"), translateText("Закрыть меню"));
			uiItem.BackColor = new Color(255, 0, 0);
			alcoUI.AddItem(uiItem);
		
			alcoUI.ItemSelect.on(item => {
				if(new Date().getTime() - global.lastCheck < 100) return; 
				global.lastCheck = new Date().getTime();
				if (item.Text == translateText("Купить")) {
					mp.events.callRemote('menu_alco', 0, selectedAlco);
				}
				else if (item.Text == translateText("Взять")) {
					mp.events.callRemote('menu_alco', 1, selectedAlco);
				}
				else if (item.Text == translateText("Скрафтить")) {
					mp.events.callRemote('menu_alco', 2, selectedAlco);
				}
				else if (item.Text == translateText("Установить цену")) {
					global.openAlco = false;
					global.menuClose();
					alcoUI.Close();
					alcoUI = null;
					mp.events.callRemote('menu_alco', 3, 0);
				}
				else if (item.Text == translateText("Закрыть")) {
					global.openAlco = false;
					global.menuClose();
					alcoUI.Close();
					alcoUI = null;
				}
			});
		
			alcoUI.ListChange.on((item, index) => {
				selectedAlco = index;
			});
		
			alcoUI.Open();
			
			alcoUI.MenuClose.on(() => {
				if(global.openAlco) alcoUI.Open();
			});

		}, 0);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "player/clubmenu", "openAlco", e.toString());
	}
});

gm.events.add("render", () => {
	try 
	{
        if (!global.loggedin) return;
		if ((global.openAlco || alcoUI) && (mp.gui.cursor.visible || !global.menuCheck ())) {
			global.openAlco = false;
			global.menuClose();
			alcoUI.Close();
			alcoUI = null;
		}
	}
	catch (e) 
	{
		if(new Date().getTime() - global.trycatchtime["casino/roulette"] < 60000) return;
		global.trycatchtime["casino/roulette"] = new Date().getTime();
		mp.events.callRemote("client_trycatch", "player/clubmenu", "render", e.toString());
	}
});

