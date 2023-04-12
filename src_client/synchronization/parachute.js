
let parachuteState = -1;
gm.events.add("render", () => {
    if (!global.loggedin) return;
    const _parachuteState = global.localplayer.getParachuteState ();

    if (parachuteState !== _parachuteState) {
        parachuteState = _parachuteState;
        if (parachuteState >= -1 && parachuteState <= 1) {            
            mp.events.callRemote("server.parachute.state", parachuteState);
        }
    }
});

const createParachute = async (player, state) => {
    if (player && mp.players.exists(player) && player.handle !== 0 && player.handle !== global.localplayer.handle) {
        if (Number (state) === -1 && player.parachuteObject) {
            player.taskParachute (false);
            if (mp.objects.exists(player.parachuteObject)) {
                player.parachuteObject.destroy();
                delete player.parachuteObject;
            }
        } else if (Number (state) === 0)
            player.taskParachute (true);
        else if (Number (state) === 1) {
            if (player.parachuteObject && mp.objects.exists(player.parachuteObject)) {
                player.parachuteObject.destroy();
                delete player.parachuteObject;
            }
            player.parachuteObject = mp.objects.new(mp.game.joaat("p_parachute1_mp_s"), player.position, {
                rotation: new mp.Vector3(0, 0, 0),
                dimension: player.dimension
            });
            await global.IsLoadEntity (player.parachuteObject);
            player.parachuteObject.setCollision(false, false);
            Natives.SET_ENTITY_LOD_DIST (player.parachuteObject, global.getLodDist (global.DistancePlayer));
            player.parachuteObject.attachTo(player.handle, 57717, 0, 0, 3, 0, 0, 0, true, true, true, false, 0, true);
        }
    }
}
gm.events.add('client.parachute.state', (player, state) => {
    createParachute (player, state);
});

gm.events.add("playerStreamIn", (entity) =>{
    if (entity && entity.parachuteObject) {
        if (mp.objects.exists(entity.parachuteObject)) {
            entity.parachuteObject.destroy();
            delete entity.parachuteObject;
        }
    }
});