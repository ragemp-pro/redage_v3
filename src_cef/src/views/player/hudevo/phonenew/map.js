/*<div id="raceAppMapContainer" style="position: relative; top: 0px; left: 0px; pointer-events: none;">
    <img src="package://Menu/UI/Images/gta_map_8k.png" draggable="false" style="position: absolute; top: 0; left: 0; height: 8192px; width: 8192px; pointer-events: none;">
</div>


*/

const raceAppMapWrapper = document.getElementById("raceAppMapWrapper"),
raceAppMapContainer = document.getElementById("raceAppMapContainer");
let posXToMap = 0,
    posYToMap = 0,
    d = !1,
    c = 0,
    l = 0,
    m = 0,
    u = 0,
    test = 0,
    h = null,
    f = [],
;
raceAppMapWrapper.addEventListener("click", async e => {//Добовление меток на карту
    if (u + 750 < E() || 7 <= test) return;
    const [t, o] = [e.offsetX + posXToMap, e.offsetY + posYToMap], [r, s] = GetMapPosToCoords(t, o);
    for (const t of f)
        if (75 > n(a(t.x - r, 2) + a(t.y - s, 2))) return;
    const d = f[f.length - 1];
    return 400 < n(a(d.x - r, 2) + a(d.y - s, 2)) ? serverAPI.notifyError("\u041D\u043E\u0432\u0430\u044F \u0442\u043E\u0447\u043A\u0430 \u043D\u0435 \u0434\u043E\u043B\u0436\u043D\u0430 \u0431\u044B\u0442\u044C \u0434\u0430\u043B\u0435\u043A\u043E \u043E\u0442 \u043F\u0440\u0435\u0434\u044B\u0434\u0443\u0449\u0435\u0439") :  exceptionCoords(r, s) ? serverAPI.notifyError("\u0422\u0443\u0442 \u043D\u0435\u043B\u044C\u0437\u044F \u0441\u0442\u0430\u0432\u0438\u0442\u044C \u0442\u043E\u0447\u043A\u0443") : void setMapPoint(r, s)
}),
raceAppMapWrapper.addEventListener("mousedown", e => {
    const [t, o] = [e.offsetX + posXToMap, e.offsetY + posYToMap], [r, s] = GetMapPosToCoords(t, o);
    let c = null;
    for (const t of f)
        if (0 !== t.index && 25 > n(a(t.x - r, 2) + a(t.y - s, 2))) {
            if (2 === e.button) return t.elementPoint.remove(), f = f.filter(e => e !== t), void v();
            c = t;
            break
        }
    d = !0, m = 0, test = 0, u = E(), h = c
});
const y = () => {
    d = !1
};
document.addEventListener("mouseup", y),
raceAppMapWrapper.addEventListener("mousemove", n => {
    if (d) {
        if (0 == ++m % 15 && (c = n.offsetX, l = n.offsetY), 7 > ++test) return;
        if (h) return void h.moveByMapCoord(n.offsetX + posXToMap, n.offsetY + posYToMap);
        posXToMap = Mathmax(0, Mathmin(posXToMap - .15 * (n.offsetX - c), 7592)), posYToMap = Mathmax(0, Mathmin(posYToMap - .15 * (n.offsetY - l), 7592)), SetCoordMap()
    } else c = n.offsetX, l = n.offsetY
});
const b = setInterval(() => {
    smartphoneAppVue.isOpen && smartphoneAppVue.currentApp === raceApp || _()
}, 500),
SetCoordMap = () => {
    raceAppMapContainer.style.left = -posXToMap + "px", raceAppMapContainer.style.top = -posYToMap + "px"
},
_ = () => {
    this.openPointsMenu = !1, document.removeEventListener("mouseup", y), o.remove(), clearInterval(b)
},
setMapPoint = (e, t) => {
    const n = {
        index: f.length,
        x: e,
        y: t,
        elementPoint: document.createElement("div"),
        moveByMapCoord(e, t) {
            const [a, o] = GetMapPosToCoords(e, t);
            n.elementPoint.style.left = e + "px", n.elementPoint.style.top = t + "px", n.x = a, n.y = o
        }
    };
    f.push(n);
    const [a, o] = w(e, t);
    n.elementPoint.style.left = a + "px", n.elementPoint.style.top = o + "px", raceAppMapContainer.insertAdjacentElement("beforeend", n.elementPoint), v()
},
v = () => {
    let e = 0;
    for (const t of f) t.index = e++, t.elementPoint.style.position = "absolute", t.elementPoint.style.width = "23px", t.elementPoint.style.height = "23px", t.elementPoint.style.display = "flex", t.elementPoint.style.justifyContent = "center", t.elementPoint.style.alignItems = "center", t.elementPoint.style.borderRadius = "50%", t.elementPoint.style.transform = "translate(-50%, -50%)", 0 === t.index || t.index === f.length - 1 ? (t.elementPoint.style.background = "#BDFF00", t.elementPoint.style.boxShadow = "0px 4px 4px #494949", t.elementPoint.innerHTML = `<div class="yk-text w-6 s-12" style="position: absolute; top: 110%; left: 50%; transform: translateX(-50%); color: #BDFF00; text-shadow: 0px 0px 4px #000000;">${0===t.index?"START":"FINISH"}</div>`) : (t.elementPoint.style.background = "#FFFFFF", t.elementPoint.style.boxShadow = "0px 4px 4px rgba(0, 0, 0, 0.25)", t.elementPoint.style.color = "#1C2544", t.elementPoint.style.textShadow = "0px 4px 4px rgba(0, 0, 0, 0.25)", t.elementPoint.className = "yk-text w-5 raceAppMapContainer-14", t.elementPoint.innerHTML = t.index)
},
GetCoordsToMap = (posX, posY) => [3756 + posX / 1.51821820693, 5528 - posY / 1.51821820693],
GetMapPosToCoords = (posX, posY) => [1.51821820693 * (posX - 3756), -1.51821820693 * (posY - 5528)],
    exceptionCoords = (e, t) => {
    const o = [
        [-2122.65, 3030.52, 440],
        [1705.495, 2584.215, 180]
    ];
    for (const r of o)
        if (n(a(e - r[0], 2) + a(t - r[1], 2)) < r[2]) return !0;
    return !1
},
E = () => new Date().getTime();
document.getElementById("raceAppBtnEnd").onclick = async () => {
    try {
        for (const e of f)
            if ( exceptionCoords(e.x, e.y)) return serverAPI.notifyError("\u041D\u0435\u043A\u043E\u0442\u043E\u0440\u044B\u0435 \u0442\u043E\u0447\u043A\u0438 \u043D\u0430\u0445\u043E\u0434\u044F\u0442\u0441\u044F \u043D\u0430 \u0437\u0430\u043A\u0440\u044B\u0442\u044B\u0445 \u0442\u0435\u0440\u0440\u0438\u0442\u043E\u0440\u0438\u044F\u0445");
        const e = parseInt(document.getElementById("raceAppInputBet").value),
            t = parseInt(document.getElementById("raceAppInputCount").value),
            n = await serverAPI.getAsync("server_raceApp_create", [t, e, f.filter(e => 0 !== e.index).map(e => [e.x.toFixed(2), e.y.toFixed(2)])]);
        this.openRace((await this.convertFromClient(n))), _()
    } catch (e) {
        serverAPI.notifyError("" + e)
    }
}, document.getElementById("raceAppBtnCancel").onclick = async () => {
    _()
};


const getPosition = await serverAPI.callApiAsync("player.getPosition");//mp.player.player.position

((posX, posY) => {
    const [x, y] = GetCoordsToMap(posX, posY);
    posXToMap = x - 300;
    posYToMap = y - 300;
    SetCoordMap();
})(getPosition.x, getPosition.y);

setMapPoint(getPosition.x, getPosition.y);

