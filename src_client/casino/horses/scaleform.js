class Scalefrom {
    constructor(scaleformStr) {
		this._handle = mp.game.graphics.requestScaleformMovie(scaleformStr);
		this.queueCallFunction = new Map();
        this.renderTargetId = undefined;
    }
 
	get isLoaded() {
		try {
			return mp.game.graphics.hasScaleformMovieLoaded(this._handle);
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "isLoaded", e.toString());
		}
	}
 
	get isValid() {
		return this._handle !== 0;
	}
 
	get handle() {
		return this._handle;
	}

	callbackLoad(){
		return new Promise( (function(resolve, reject){
			let countCheck = 0;
			const timer = setInterval(() => {
				if(countCheck > 100){
					resolve(translateText("Ошибка загрузки"));
					clearInterval(timer);
					return;
				}
				if(this.isValid && this.isLoaded) {
					clearInterval(timer);
					countCheck++;
					resolve();
				}
			}, 10);
		}).bind(this));
	}

	callFunction(strFunction, ...args) {
		try{
			if (this.isLoaded && this.isValid) {
				const graphics = mp.game.graphics;
				graphics.pushScaleformMovieFunction(this._handle, strFunction);
				args.forEach(arg => {
					switch(typeof arg) {
						case 'string': {
							graphics.pushScaleformMovieFunctionParameterString(arg);
							break;
						}
						case 'boolean': {
							graphics.pushScaleformMovieFunctionParameterBool(arg);
							break;
						}
						case 'number': {
							if(Number(arg) === arg && arg % 1 !== 0) {
								graphics.pushScaleformMovieFunctionParameterFloat(arg);
							} else {
								graphics.pushScaleformMovieFunctionParameterInt(arg);
							}
						}
					}
				});
				graphics.popScaleformMovieFunctionVoid();
			} else {
				this.queueCallFunction.set(strFunction, args);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "callFunction", e.toString());
		}
	}
 
	onUpdate() {
		try {
			if (this.isLoaded && this.isValid) {
				this.queueCallFunction.forEach((args, strFunction) => {
					this.callFunction(strFunction, ...args);
					this.queueCallFunction.delete(strFunction);
				});
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "onUpdate", e.toString());
		}
	}

	renderFullscreen() {
		try{
			this.onUpdate();
			if (this.isLoaded && this.isValid) {
				mp.game.graphics.drawScaleformMovieFullscreen(this._handle, 255, 255, 255, 255, false);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "renderFullscreen", e.toString());
		}
	}
 
	render2D(x, y, width, height) {
		try {
			this.onUpdate();
			if (this.isLoaded && this.isValid) {
				if (typeof x !== 'undefined' && typeof y !== 'undefined' && typeof width !== 'undefined' && typeof height !== 'undefined') {
					mp.game.graphics.drawScaleformMovie(this._handle, x, y, width, height, 255, 255, 255, 255, 0);
				} else {
					mp.game.graphics.drawScaleformMovieFullscreen(this._handle, 255, 255, 255, 255, false);
				}
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "render2D", e.toString());
		}
	}
 
	render3D(position, rotation, scale) {
		try {
			this.onUpdate();
			if (this.isLoaded && this.isValid) {
				mp.game.graphics.drawScaleformMovie3dNonAdditive(this._handle, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, 2, 2, 1, scale.x, scale.y, scale.z, 2);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "render3D", e.toString());
		}
	}
 
	render3DAdditive(position, rotation, scale) {
		try {
			this.onUpdate();
			if (this.isLoaded && this.isValid) {
				mp.game.graphics.drawScaleformMovie3d(this._handle, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, 2, 2, 1, scale.x, scale.y, scale.z, 2);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "render3DAdditive", e.toString());
		}
    }
    createRenderTarget(name, model){
		try {
			if(!mp.game.ui.isNamedRendertargetRegistered(name))
				mp.game.ui.registerNamedRendertarget(name, false); //Register render target
			if(!mp.game.ui.isNamedRendertargetLinked(mp.game.joaat(model)))
				mp.game.ui.linkNamedRendertarget(mp.game.joaat(model)); //Link it to all models
			if(mp.game.ui.isNamedRendertargetRegistered(name))
				return mp.game.ui.getNamedRendertargetRenderId(name); //Get the handle
			return -1;
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "createRenderTarget", e.toString());
			return -1;
		}
    }

    renderTarget(name, model, x, y, width, height){
		try {
			this.onUpdate();
			if (this.isLoaded && this.isValid) {
				if(!this.renderTargetId)this.renderTargetId = this.createRenderTarget(name, model);
				if(this.renderTargetId == -1) return mp.gui.chat.push('Could not create render target.')
				mp.game.ui.setTextRenderId(this.renderTargetId);
				mp.game.graphics.set2dLayer(4);
				mp.game.graphics.drawScaleformMovie(this._handle, x, y, width, height, 255, 255 ,255, 255, 0);
				mp.game.ui.setTextRenderId(1);
			}
		}
		catch (e) 
		{
			mp.events.callRemote("client_trycatch", "horses/scaleform", "renderTarget", e.toString());
		}
    }

	dispose() {
		mp.game.graphics.setScaleformMovieAsNoLongerNeeded(this._handle);
	}
}

global.ScaleFormRacing = Scalefrom;


global.requestScaleformMovie = scaleformHandle => new Promise(async (resolve, reject) => {
	try {
		const scaleform = mp.game.graphics.requestScaleformMovie(scaleformHandle);
		if (mp.game.graphics.hasScaleformMovieLoaded(scaleform))
			return resolve(true);
        let d = 0;
		while (!mp.game.graphics.hasScaleformMovieLoaded(scaleform)) {
            if (d > 5000) return resolve(translateText("Ошибка requestScaleformMovie. Texture: ") + scaleformHandle);
            d++;
            await global.wait (0);
        }
        return resolve(true);
    } 
    catch (e) 
	{
		mp.events.callRemote("client_trycatch", "casino/horses", "requestScaleformMovie", e.toString());
		resolve();
	}
});