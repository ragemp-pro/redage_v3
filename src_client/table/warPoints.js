const positions = [
    new mp.Vector3(-986.31604, -185.37245, 37.90033)
]

const drawSprites = [
    ["redage_textures_001", "flag"],
]

const warPoint = new class {
    constructor() {
        gm.events.add('render', () => {
            if (!global.loggedin) return;
            this.render();
        });


        gm.events.add(global.renderName ["2s"], () => {
            if (!global.loggedin) return;
            this.updateTexture();
        });
    }

    getScale (realDist, maxDist) {
        return Math.max(0.25, 1 - realDist / maxDist);
    }

    drawSpritePoly(first, second, third, u1, v1, w1, u2, v2, w2, u3, v3, w3, color1, color2, color3, alpha) {
        mp.game.invoke('0x29280002282F1928', first.x, first.y, first.z, second.x, second.y, second.z, third.x, third.y, third.z, color1, color2, color3, alpha, "redage_textures_001", "flag", u1, v1, w1, u2, v2, w2, u3, v3, w3);
    }

    render() {
        const localPos = global.localplayer.position;

        positions.forEach((position) => {
            mp.game.graphics.drawLine(position.x, position.y, position.z, position.x, position.y, position.z + 50, 255, 255, 255, 255);
            const pos = mp.game.graphics.world3dToScreen2d(position.x, position.y, position.z + 50);

            let dist = mp.game.system.vdist(position.x, position.y, position.z, localPos.x, localPos.y, localPos.z);
            const _getScale = this.getScale (dist, 999);
            let scale = 0.075 * _getScale;

            //mp.gui.chat.push("scale - " + scale + " - " + _getScale);
            const ratio = mp.game.graphics.getScreenAspectRatio(true);
            //mp.game.graphics.drawSprite("Deadline", "Deadline_Trail_01", pos.x, pos.y, scale, scale * ratio, 0, 255, 255, 255, 255);
            this.drawSpritePoly(new mp.Vector3(position.x, position.y, position.z + 50), new mp.Vector3(position.x, position.y, position.z + 50), new mp.Vector3(position.x, position.y, position.z + 50), 0.9999999, 0.9999999, 0.9999999, 0.9999999, 0.9999999, 0.9999999, 0.9999999, 0.0000001, 1.9999999, 255, 255, 255, 255)
            //mp.game.graphics.drawSprite("redage_textures_001", "flag", pos.x, pos.y, scale, scale * ratio, 0, 255, 255, 255, 255);
        })
    }

    updateTexture() {
        const activeResolution = mp.game.graphics.getScreenActiveResolution(0, 0);

        drawSprites.forEach((drawSprite) => {
            const size = defaultSpritesSize [ drawSprite [1] ] ? defaultSpritesSize [ drawSprite [1] ] : defaultSpriteSize;

            const drawSpritesResolution = mp.game.graphics.getTextureResolution(drawSprite [0], drawSprite [1]);
            drawSpritesSize [ drawSprite [1] ] = {
                width: (size * drawSpritesResolution.x) / activeResolution.x,
                height: (size * drawSpritesResolution.y) / activeResolution.y,
                heading: drawSprite [2] ? drawSprite [2] : 0
            }
        });
    }

    //mp.game.gameplay.getGroundZFor3dCoord(position.x, position.y, position.z, 0.0, false);


}