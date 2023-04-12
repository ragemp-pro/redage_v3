setInterval(() => {
    try {
        if (!global.loggedin) return;
        const { position, dimension } = global.localplayer;

        mp.polygons.pool.map((polygon) => {
            if (polygon.colliding) {
                if (!mp.polygons.isPositionWithinPolygon(position, polygon, dimension)) {
                    polygon.colliding = false;
                    mp.events.call("playerLeavePolygon", polygon);
                }
            } else {
                if (mp.polygons.isPositionWithinPolygon(position, polygon, dimension)) {
                    polygon.colliding = true;
                    mp.events.call("playerEnterPolygon", polygon);
                }
            }
        });
    } catch (e) {
        if (new Date().getTime() - global.trycatchtime["polygons/events"] < 5000) return;
        global.trycatchtime["polygons/events"] = new Date().getTime();
        mp.events.callRemote("client_trycatch", "polygons/events", "setInterval", e.toString());
    }
}, 500);