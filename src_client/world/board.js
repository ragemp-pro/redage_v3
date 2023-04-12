const _3235010 = class {
    constructor({id: _3144159}) {
        this.id = _3144159;
        this.header = '';
        this.title = '';
        this.description = '';
        this.footer = '';
        this.numericId = this.id.toString().padStart(2, '0');
        this.currentType = 0;
        this.currentTypeNumeric = '00';
        this.model = mp.game.joaat("mj_billboard_" + this.currentTypeNumeric + '_' + this.numericId);
        this.screenName = "billboard_" + this.numericId;
        this.objectsHandle = {};
        this.currentObject = null;
        this.textureLoaded = !1;
        this.backgroundRgb = [
            0,
            0,
            0
        ];
        this.backgroundColorActive = !0;
        this.backgroundDict = "mugshot_board_02";
        this.backgroundTexture = "propboardbg";
        this.backgroundImageActive = !1;
    }
    setMeta(_1588090) {
        this.header = "string" == typeof _1588090.header ? _1588090.header : this.header;
        this.title = "string" == typeof _1588090.title ? _1588090.title : this.title;
        this.description = "string" == typeof _1588090.description ? _1588090.description : this.description;
        this.footer = "string" == typeof _1588090.footer ? _1588090.footer : this.footer;
    }
    setBackgroundTexture(_3969962, _4482416) {
        this.backgroundDict = _3969962;
        this.backgroundTexture = _4482416;
        this.textureLoaded = !1;
    }
    toggleBackgroundTexture(_9000223) {
        this.backgroundImageActive = _9000223;
    }
    setBackgroundColor(_5570280, _1410778, _2741163) {
        this.backgroundRgb = [
            _5570280,
            _1410778,
            _2741163
        ];
    }
    toggleBackgroundColor(_13317228) {
        this.backgroundColorActive = _13317228;
    }
    setCurrentType(_1813156) {
        this.currentTypeNumeric = _1813156.toString().padStart(2, '0');
        this.model = mp.game.joaat("mj_billboard_" + this.currentTypeNumeric + '_' + this.numericId);
        this.currentType = _1813156;
    }
    deleteObject() {
        this.currentObject && mp.objects.exists(this.currentObject) && (this.currentObject.destroy(), this.currentObject = null, this.skipFrame = !0);
    }
    draw(_2491007 = this.position, _2402350 = this.rotation, _3537582) {
        if (this.ready && (!this.backgroundImageActive || (this.textureLoaded || (mp.game.graphics.requestStreamedTextureDict(this.backgroundDict, !0), mp.game.graphics.hasStreamedTextureDictLoaded(this.backgroundDict) && (this.textureLoaded = !0)), this.textureLoaded))) {
            if (mp.game.streaming.hasModelLoaded(this.model)) {
                if (this.skipFrame)
                    this.skipFrame = !1;
                else {
                    if (!this.currentObject)
                        return this.currentObject = mp.objects.new(this.model, _2491007, {
                            'rotation': _2402350,
                            'dimension': -1
                        }), this.currentObject.setLodDist(1000), void (this.skipFrame = !0);
                    this.screenHandle || (this.screenHandle = _2369672.api.createNamedRenderTargetForModel(this.screenName, this.model)), _3537582 && (_3537582.callFunction("SET_BOARD", this.header, this.description, this.footer, this.title, 2), _3537582.callFunction("CHANGE_BACKGROUND_COLOR", this.backgroundRgb[0], this.backgroundRgb[1], this.backgroundRgb[2]), mp.game.ui.setTextRenderId(this.screenHandle), mp.game.graphics.set2dLayer(4), _2369672.api.invokeNative("SET_SCRIPT_GFX_DRAW_BEHIND_PAUSEMENU", !0), _2369672.api.invokeNative("_SET_SCALEFORM_FIT_RENDERTARGET", _3537582.handle, !0), _3537582.render2D(0.5, 0.5, 1, 1), mp.game.ui.setTextRenderId(_2369672.api.invokeNative("GET_DEFAULT_SCRIPT_RENDERTARGET_RENDER_ID")));
                }
            } else
                mp.game.streaming.requestModel(this.model);
        }
    }
};