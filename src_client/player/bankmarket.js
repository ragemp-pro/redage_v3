gm.events.add('mavrshop', (json) => {
	let data = JSON.parse(json);
    global.openSM(2, JSON.stringify(data));
});

gm.events.add('gangmis', () => {
    let data = [
        translateText("Угон автотранспорта"),
        translateText("Перевозка автотранспорта"),
    ];
    global.openSM(8, JSON.stringify(data));
});

gm.events.add('mafiamis', () => {
    let data = [
        translateText("Перевозка оружия"),
        translateText("Перевозка денег"),
        translateText("Перевозка трупов"),
    ];
    global.openSM(9, JSON.stringify(data));
});

gm.events.add('bikermis', () => {
    let data = [
        translateText("Перевозка оружия"),
        translateText("Перевозка денег"),
        translateText("Перевозка трупов"),
    ];
    global.openSM(10, JSON.stringify(data));
});

gm.events.add('shop', (json) => {
    let data = JSON.parse(json);
    global.openSM(1, JSON.stringify(data));
})
