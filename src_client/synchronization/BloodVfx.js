const VFX_MODE_DEFAULT = 0;
const VFX_MODE_ALIEN = 1;
const VFX_MODE_CLOWN = 2;

Object.defineProperty(mp.game.graphics, "bloodVfxMode", {
    get() {
        return this._vfxMode || VFX_MODE_DEFAULT;
    },

    async set(value) {
        switch (value) {
            case VFX_MODE_ALIEN:
                await global.requestNamedPtfxAsset("scr_rcbarry1");

                mp.game.graphics.enableClownBloodVfx(false);
                mp.game.graphics.enableAlienBloodVfx(true);
                break;

            case VFX_MODE_CLOWN:
                await global.requestNamedPtfxAsset("scr_rcbarry2");

                mp.game.graphics.enableAlienBloodVfx(false);
                mp.game.graphics.enableClownBloodVfx(true);
                break;

            default:
                value = VFX_MODE_DEFAULT;

                mp.game.graphics.enableAlienBloodVfx(false);
                mp.game.graphics.enableClownBloodVfx(false);
                break;
        }

        this._vfxMode = value;
    }
});