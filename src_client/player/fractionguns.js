gm.events.add('policeg', () => {
    let data = [
        translateText("Дубинка"),
        translateText("Пистолет"),
        "SMG",
        translateText("Дробовик"),
        "Tazer",
        translateText("Бронежилет"),
        translateText("Аптечка"),
        translateText("Пистолетный калибр x12"),
        translateText("Малый калибр x30"),
        translateText("Дробь x6"),
        translateText("Сдать бронежилет")
    ];
    global.openSM(4, JSON.stringify(data));
});

gm.events.add('fbiguns', () => {
    let data = [
        "Tazer",
        translateText("Пистолет"),
        translateText("ПОС"),
        translateText("Карабин"),
        translateText("Снайперская винтовка"),
        translateText("Бронежилет"),
        translateText("Аптечка"),
        translateText("Пистолетный калибр x12"),
        translateText("Малый калибр x30"),
        translateText("Автоматный калибр x30"),
        translateText("Снайперский калибр x5"),
        "Бейдж",
        translateText("Сдать бронежилет")
    ];
    global.openSM(3, JSON.stringify(data));
});

gm.events.add('govguns', () => {
    let data = [
        "Tazer",
        translateText("Пистолет"),
        "Advanced Rifle",
        "Gusenberg Sweeper",
        translateText("Бронежилет"),
        translateText("Аптечка"),
        translateText("Пистолетный калибр x12"),
        translateText("Малый калибр x30"),
        translateText("Автоматный калибр x30"),
        translateText("Сдать бронежилет")
    ];
    global.openSM(6, JSON.stringify(data));
});

gm.events.add('armyguns', () => {
    let data = [
        translateText("Пистолет"),
        translateText("Карабин"),
		translateText("Боевой пулемет"),
        translateText("Бронежилет"),
        translateText("Аптечка"),
        translateText("Пистолетный калибр x12"),
        translateText("Автоматный калибр x30"),
		translateText("Малый калибр x100"),
        translateText("Сдать бронежилет")
    ];
    global.openSM(7, JSON.stringify(data));
});