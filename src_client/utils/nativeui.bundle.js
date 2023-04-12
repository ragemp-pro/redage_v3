/******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = 0);
/******/ })
/************************************************************************/
/******/ ([
/* 0 */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

    "use strict";
    __webpack_require__.r(__webpack_exports__);
    
    // CONCATENATED MODULE: ./enums/BadgeStyle.ts
    var BadgeStyle;
    (function (BadgeStyle) {
        BadgeStyle[BadgeStyle["None"] = 0] = "None";
        BadgeStyle[BadgeStyle["BronzeMedal"] = 1] = "BronzeMedal";
        BadgeStyle[BadgeStyle["GoldMedal"] = 2] = "GoldMedal";
        BadgeStyle[BadgeStyle["SilverMedal"] = 3] = "SilverMedal";
        BadgeStyle[BadgeStyle["Alert"] = 4] = "Alert";
        BadgeStyle[BadgeStyle["Crown"] = 5] = "Crown";
        BadgeStyle[BadgeStyle["Ammo"] = 6] = "Ammo";
        BadgeStyle[BadgeStyle["Armour"] = 7] = "Armour";
        BadgeStyle[BadgeStyle["Barber"] = 8] = "Barber";
        BadgeStyle[BadgeStyle["Clothes"] = 9] = "Clothes";
        BadgeStyle[BadgeStyle["Franklin"] = 10] = "Franklin";
        BadgeStyle[BadgeStyle["Bike"] = 11] = "Bike";
        BadgeStyle[BadgeStyle["Car"] = 12] = "Car";
        BadgeStyle[BadgeStyle["Gun"] = 13] = "Gun";
        BadgeStyle[BadgeStyle["Heart"] = 14] = "Heart";
        BadgeStyle[BadgeStyle["Makeup"] = 15] = "Makeup";
        BadgeStyle[BadgeStyle["Mask"] = 16] = "Mask";
        BadgeStyle[BadgeStyle["Michael"] = 17] = "Michael";
        BadgeStyle[BadgeStyle["Star"] = 18] = "Star";
        BadgeStyle[BadgeStyle["Tatoo"] = 19] = "Tatoo";
        BadgeStyle[BadgeStyle["Trevor"] = 20] = "Trevor";
        BadgeStyle[BadgeStyle["Lock"] = 21] = "Lock";
        BadgeStyle[BadgeStyle["Tick"] = 22] = "Tick";
    })(BadgeStyle || (BadgeStyle = {}));
    /* harmony default export */ var enums_BadgeStyle = (BadgeStyle);
    
    // CONCATENATED MODULE: ./enums/Font.ts
    var Font;
    (function (Font) {
        Font[Font["ChaletLondon"] = 0] = "ChaletLondon";
        Font[Font["HouseScript"] = 1] = "HouseScript";
        Font[Font["Monospace"] = 2] = "Monospace";
        Font[Font["CharletComprimeColonge"] = 4] = "CharletComprimeColonge";
        Font[Font["Pricedown"] = 7] = "Pricedown";
    })(Font || (Font = {}));
    /* harmony default export */ var enums_Font = (Font);
    
    // CONCATENATED MODULE: ./utils/Color.ts
    class Color {
        constructor(r, g, b, a = 255) {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }
    }
    Color.Empty = new Color(0, 0, 0, 0);
    Color.Transparent = new Color(0, 0, 0, 0);
    Color.Black = new Color(0, 0, 0, 255);
    Color.White = new Color(255, 255, 255, 255);
    Color.WhiteSmoke = new Color(245, 245, 245, 255);
    
    // CONCATENATED MODULE: ./utils/Screen.ts
    const gameScreen = mp.game.graphics.getScreenActiveResolution(0, 0);
    const Screen = {
        width: gameScreen.x,
        height: gameScreen.y
    };
    
    // CONCATENATED MODULE: ./modules/Sprite.ts
    
    
    class Sprite_Sprite {
        constructor(textureDict, textureName, pos, size, heading = 0, color = new Color(255, 255, 255)) {
            this.TextureDict = textureDict;
            this.TextureName = textureName;
            this.pos = pos;
            this.size = size;
            this.heading = heading;
            this.color = color;
            this.visible = true;
        }
        LoadTextureDictionary() {
            mp.game.graphics.requestStreamedTextureDict(this._textureDict, true);
            while (!this.IsTextureDictionaryLoaded) {
                mp.game.wait(0);
            }
        }
        set TextureDict(v) {
            this._textureDict = v;
            if (!this.IsTextureDictionaryLoaded)
                this.LoadTextureDictionary();
        }
        get TextureDict() {
            return this._textureDict;
        }
        get IsTextureDictionaryLoaded() {
            return mp.game.graphics.hasStreamedTextureDictLoaded(this._textureDict);
        }
        Draw(textureDictionary, textureName, pos, size, heading, color, loadTexture) {
            textureDictionary = textureDictionary || this.TextureDict;
            textureName = textureName || this.TextureName;
            pos = pos || this.pos;
            size = size || this.size;
            heading = heading || this.heading;
            color = color || this.color;
            loadTexture = loadTexture || true;
            if (loadTexture) {
                if (!mp.game.graphics.hasStreamedTextureDictLoaded(textureDictionary))
                    mp.game.graphics.requestStreamedTextureDict(textureDictionary, true);
            }
            const screenw = Screen.width;
            const screenh = Screen.height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            const width = height * ratio;
            const w = this.size.Width / width;
            const h = this.size.Height / height;
            const x = this.pos.X / width + w * 0.5;
            const y = this.pos.Y / height + h * 0.5;
            mp.game.graphics.drawSprite(textureDictionary, textureName, x, y, w, h, heading, color.R, color.G, color.B, color.A);
        }
    }
    
    // CONCATENATED MODULE: ./utils/LiteEvent.ts
    class LiteEvent {
        constructor() {
            this.handlers = [];
        }
        on(handler) {
            this.handlers.push(handler);
        }
        off(handler) {
            this.handlers = this.handlers.filter(h => h !== handler);
        }
        emit(...args) {
            this.handlers.slice(0).forEach(h => h(...args));
        }
        expose() {
            return this;
        }
    }
    
    // CONCATENATED MODULE: ./utils/Point.ts
    class Point {
        constructor(x, y) {
            this.X = 0;
            this.Y = 0;
            this.X = x;
            this.Y = y;
        }
        static Parse(arg) {
            if (typeof arg === "object") {
                if (arg.length) {
                    return new Point(arg[0], arg[1]);
                }
                else if (arg.X && arg.Y) {
                    return new Point(arg.X, arg.Y);
                }
            }
            else if (typeof arg === "string") {
                if (arg.indexOf(",") !== -1) {
                    const arr = arg.split(",");
                    return new Point(parseFloat(arr[0]), parseFloat(arr[1]));
                }
            }
            return new Point(0, 0);
        }
    }
    
    // CONCATENATED MODULE: ./utils/Size.ts
    class Size {
        constructor(w = 0, h = 0) {
            this.Width = w;
            this.Height = h;
        }
    }
    
    // CONCATENATED MODULE: ./modules/IElement.ts
    class IElement {
        constructor() {
            this.enabled = true;
        }
    }
    
    // CONCATENATED MODULE: ./modules/Rectangle.ts
    
    
    
    class Rectangle_Rectangle extends IElement {
        constructor(pos, size, color) {
            super();
            this.enabled = true;
            this.pos = pos;
            this.size = size;
            this.color = color;
        }
        Draw(pos, size, color) {
            if (!pos)
                pos = new Size(0, 0);
            if (!size && !color) {
                pos = new Point(this.pos.X + pos.Width, this.pos.Y + pos.Height);
                size = this.size;
                color = this.color;
            }
            const w = size.Width / 1280.0;
            const h = size.Height / 720.0;
            const x = pos.X / 1280.0 + w * 0.5;
            const y = pos.Y / 720.0 + h * 0.5;
            mp.game.graphics.drawRect(x, y, w, h, color.R, color.G, color.B, color.A);
        }
    }
    
    // CONCATENATED MODULE: ./modules/ResRectangle.ts
    
    
    
    
    class ResRectangle_ResRectangle extends Rectangle_Rectangle {
        constructor(pos, size, color) {
            super(pos, size, color);
        }
        Draw(pos, size, color) {
            if (!pos)
                pos = new Size();
            if (pos && !size && !color) {
                pos = new Point(this.pos.X + pos.Width, this.pos.Y + pos.Height);
                size = this.size;
                color = this.color;
            }
            const screenw = Screen.width;
            const screenh = Screen.height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            const width = height * ratio;
            const w = size.Width / width;
            const h = size.Height / height;
            const x = pos.X / width + w * 0.5;
            const y = pos.Y / height + h * 0.5;
            mp.game.graphics.drawRect(x, y, w, h, color.R, color.G, color.B, color.A);
        }
    }
    
    // CONCATENATED MODULE: ./modules/Text.ts
    
    
    
    
    class Text_Text extends IElement {
        constructor(caption, pos, scale, color, font, centered) {
            super();
            this.caption = caption;
            this.pos = pos;
            this.scale = scale;
            this.color = color || new Color(255, 255, 255, 255);
            this.font = font || 0;
            this.centered = centered || false;
        }
        Draw(caption, pos, scale, color, font, centered) {
            if (caption && !pos && !scale && !color && !font && !centered) {
                pos = new Point(this.pos.X + caption.Width, this.pos.Y + caption.Height);
                scale = this.scale;
                color = this.color;
                font = this.font;
                centered = this.centered;
            }
            const x = pos.X / 1280.0;
            const y = pos.Y / 720.0;
            mp.game.ui.setTextFont(parseInt(font));
            mp.game.ui.setTextScale(scale, scale);
            mp.game.ui.setTextColour(color.R, color.G, color.B, color.A);
            mp.game.ui.setTextCentre(centered);
            mp.game.ui.setTextEntry("STRING");
            ResText_ResText.AddLongString(caption);
            mp.game.ui.drawText(x, y);
        }
    }
    exports = Text_Text;
    
    // CONCATENATED MODULE: ./modules/ResText.ts
    
    
    
    
    
    var Alignment;
    (function (Alignment) {
        Alignment[Alignment["Left"] = 0] = "Left";
        Alignment[Alignment["Centered"] = 1] = "Centered";
        Alignment[Alignment["Right"] = 2] = "Right";
    })(Alignment || (Alignment = {}));
    class ResText_ResText extends Text_Text {
        constructor(caption, pos, scale, color, font, justify) {
            super(caption, pos, scale, color || new Color(255, 255, 255), font || 0, false);
            this.TextAlignment = Alignment.Left;
            if (justify)
                this.TextAlignment = justify;
        }
        Draw(arg1, pos, scale, color, font, arg2, dropShadow, outline, wordWrap) {
            let caption = arg1;
            let centered = arg2;
            let textAlignment = arg2;
            if (!arg1)
                arg1 = new Size(0, 0);
            if (arg1 && !pos) {
                textAlignment = this.TextAlignment;
                caption = this.caption;
                pos = new Point(this.pos.X + arg1.Width, this.pos.Y + arg1.Height);
                scale = this.scale;
                color = this.color;
                font = this.font;
                if (centered == true || centered == false) {
                    centered = this.centered;
                }
                else {
                    centered = undefined;
                    dropShadow = this.DropShadow;
                    outline = this.Outline;
                    wordWrap = this.WordWrap;
                }
            }
            const screenw = Screen.width;
            const screenh = Screen.height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            const width = height * ratio;
            const x = this.pos.X / width;
            const y = this.pos.Y / height;
            mp.game.ui.setTextFont(parseInt(font));
            mp.game.ui.setTextScale(1.0, scale);
            mp.game.ui.setTextColour(color.R, color.G, color.B, color.A);
            if (centered !== undefined) {
                mp.game.ui.setTextCentre(centered);
            }
            else {
                if (dropShadow)
                    mp.game.ui.setTextDropshadow(2, 0, 0, 0, 0);
                if (outline)
                    console.warn("not working!");
                switch (textAlignment) {
                    case Alignment.Centered:
                        mp.game.ui.setTextCentre(true);
                        break;
                    case Alignment.Right:
                        mp.game.ui.setTextRightJustify(true);
                        mp.game.ui.setTextWrap(0.0, x);
                        break;
                }
                if (wordWrap) {
                    const xsize = (this.pos.X + wordWrap.Width) / width;
                    mp.game.ui.setTextWrap(x, xsize);
                }
            }
            mp.game.ui.setTextEntry("STRING");
            ResText_ResText.AddLongString(caption);
            mp.game.ui.drawText(x, y);
        }
        static AddLongString(str) {
            const strLen = 99;
            for (var i = 0; i < str.length; i += strLen) {
                const substr = str.substr(i, Math.min(strLen, str.length - i));
                mp.game.ui.addTextComponentSubstringPlayerName(substr);
            }
        }
    }
    
    // CONCATENATED MODULE: ./utils/UUIDV4.ts
    function UUIDV4() {
        var uuid = "", ii;
        for (ii = 0; ii < 32; ii += 1) {
            switch (ii) {
                case 8:
                case 20:
                    uuid += "-";
                    uuid += ((Math.random() * 16) | 0).toString(16);
                    break;
                case 12:
                    uuid += "-";
                    uuid += "4";
                    break;
                case 16:
                    uuid += "-";
                    uuid += ((Math.random() * 4) | 8).toString(16);
                    break;
                default:
                    uuid += ((Math.random() * 16) | 0).toString(16);
            }
        }
        return uuid;
    }
    
    // CONCATENATED MODULE: ./items/UIMenuItem.ts
    
    
    
    
    
    
    
    
    
    class UIMenuItem_UIMenuItem {
        constructor(text, description = "") {
            this.Id = UUIDV4();
            this.BackColor = UIMenuItem_UIMenuItem.DefaultBackColor;
            this.HighlightedBackColor = UIMenuItem_UIMenuItem.DefaultHighlightedBackColor;
            this.ForeColor = UIMenuItem_UIMenuItem.DefaultForeColor;
            this.HighlightedForeColor = UIMenuItem_UIMenuItem.DefaultHighlightedForeColor;
            this.RightLabel = "";
            this.LeftBadge = enums_BadgeStyle.None;
            this.RightBadge = enums_BadgeStyle.None;
            this.Enabled = true;
            this._rectangle = new ResRectangle_ResRectangle(new Point(0, 0), new Size(431, 38), new Color(150, 0, 0, 0));
            this._text = new ResText_ResText(text, new Point(8, 0), 0.33, Color.WhiteSmoke, enums_Font.ChaletLondon, Alignment.Left);
            this.Description = description;
            this._selectedSprite = new Sprite_Sprite("commonmenu", "gradient_nav", new Point(0, 0), new Size(431, 38));
            this._badgeLeft = new Sprite_Sprite("commonmenu", "", new Point(0, 0), new Size(40, 40));
            this._badgeRight = new Sprite_Sprite("commonmenu", "", new Point(0, 0), new Size(40, 40));
            this._labelText = new ResText_ResText("", new Point(0, 0), 0.35, Color.White, 0, Alignment.Right);
        }
        get Text() {
            return this._text.caption;
        }
        set Text(v) {
            this._text.caption = v;
        }
        SetVerticalPosition(y) {
            this._rectangle.pos = new Point(this.Offset.X, y + 144 + this.Offset.Y);
            this._selectedSprite.pos = new Point(0 + this.Offset.X, y + 144 + this.Offset.Y);
            this._text.pos = new Point(8 + this.Offset.X, y + 147 + this.Offset.Y);
            this._badgeLeft.pos = new Point(0 + this.Offset.X, y + 142 + this.Offset.Y);
            this._badgeRight.pos = new Point(385 + this.Offset.X, y + 142 + this.Offset.Y);
            this._labelText.pos = new Point(420 + this.Offset.X, y + 148 + this.Offset.Y);
        }
        addEvent(event, ...args) {
            this._event = { event: event, args: args };
        }
        fireEvent() {
            if (this._event) {
                mp.events.call(this._event.event, this, ...this._event.args);
            }
        }
        Draw() {
            this._rectangle.size = new Size(431 + this.Parent.WidthOffset, 38);
            this._selectedSprite.size = new Size(431 + this.Parent.WidthOffset, 38);
            if (this.Hovered && !this.Selected) {
                this._rectangle.color = new Color(255, 255, 255, 20);
                this._rectangle.Draw();
            }
            this._selectedSprite.color = this.Selected
                ? this.HighlightedBackColor
                : this.BackColor;
            this._selectedSprite.Draw();
            this._text.color = this.Enabled
                ? this.Selected
                    ? this.HighlightedForeColor
                    : this.ForeColor
                : new Color(163, 159, 148);
            if (this.LeftBadge != enums_BadgeStyle.None) {
                this._text.pos = new Point(35 + this.Offset.X, this._text.pos.Y);
                this._badgeLeft.TextureDict = this.BadgeToSpriteLib(this.LeftBadge);
                this._badgeLeft.TextureName = this.BadgeToSpriteName(this.LeftBadge, this.Selected);
                this._badgeLeft.color = this.IsBagdeWhiteSprite(this.LeftBadge)
                    ? this.Enabled
                        ? this.Selected
                            ? this.HighlightedForeColor
                            : this.ForeColor
                        : new Color(163, 159, 148)
                    : Color.White;
                this._badgeLeft.Draw();
            }
            else {
                this._text.pos = new Point(8 + this.Offset.X, this._text.pos.Y);
            }
            if (this.RightBadge != enums_BadgeStyle.None) {
                this._badgeRight.pos = new Point(385 + this.Offset.X + this.Parent.WidthOffset, this._badgeRight.pos.Y);
                this._badgeRight.TextureDict = this.BadgeToSpriteLib(this.RightBadge);
                this._badgeRight.TextureName = this.BadgeToSpriteName(this.RightBadge, this.Selected);
                this._badgeRight.color = this.IsBagdeWhiteSprite(this.RightBadge)
                    ? this.Enabled
                        ? this.Selected
                            ? this.HighlightedForeColor
                            : this.ForeColor
                        : new Color(163, 159, 148)
                    : Color.White;
                this._badgeRight.Draw();
            }
            if (this.RightLabel && this.RightLabel !== "") {
                this._labelText.pos = new Point(420 + this.Offset.X + this.Parent.WidthOffset, this._labelText.pos.Y);
                this._labelText.caption = this.RightLabel;
                this._labelText.color = this._text.color = this.Enabled
                    ? this.Selected
                        ? this.HighlightedForeColor
                        : this.ForeColor
                    : new Color(163, 159, 148);
                this._labelText.Draw();
            }
            this._text.Draw();
        }
        SetLeftBadge(badge) {
            this.LeftBadge = badge;
        }
        SetRightBadge(badge) {
            this.RightBadge = badge;
        }
        SetRightLabel(text) {
            this.RightLabel = text;
        }
        BadgeToSpriteLib(badge) {
            return "commonmenu";
        }
        BadgeToSpriteName(badge, selected) {
            switch (badge) {
                case enums_BadgeStyle.None:
                    return "";
                case enums_BadgeStyle.BronzeMedal:
                    return "mp_medal_bronze";
                case enums_BadgeStyle.GoldMedal:
                    return "mp_medal_gold";
                case enums_BadgeStyle.SilverMedal:
                    return "medal_silver";
                case enums_BadgeStyle.Alert:
                    return "mp_alerttriangle";
                case enums_BadgeStyle.Crown:
                    return "mp_hostcrown";
                case enums_BadgeStyle.Ammo:
                    return selected ? "shop_ammo_icon_b" : "shop_ammo_icon_a";
                case enums_BadgeStyle.Armour:
                    return selected ? "shop_armour_icon_b" : "shop_armour_icon_a";
                case enums_BadgeStyle.Barber:
                    return selected ? "shop_barber_icon_b" : "shop_barber_icon_a";
                case enums_BadgeStyle.Clothes:
                    return selected ? "shop_clothing_icon_b" : "shop_clothing_icon_a";
                case enums_BadgeStyle.Franklin:
                    return selected ? "shop_franklin_icon_b" : "shop_franklin_icon_a";
                case enums_BadgeStyle.Bike:
                    return selected ? "shop_garage_bike_icon_b" : "shop_garage_bike_icon_a";
                case enums_BadgeStyle.Car:
                    return selected ? "shop_garage_icon_b" : "shop_garage_icon_a";
                case enums_BadgeStyle.Gun:
                    return selected ? "shop_gunclub_icon_b" : "shop_gunclub_icon_a";
                case enums_BadgeStyle.Heart:
                    return selected ? "shop_health_icon_b" : "shop_health_icon_a";
                case enums_BadgeStyle.Lock:
                    return "shop_lock";
                case enums_BadgeStyle.Makeup:
                    return selected ? "shop_makeup_icon_b" : "shop_makeup_icon_a";
                case enums_BadgeStyle.Mask:
                    return selected ? "shop_mask_icon_b" : "shop_mask_icon_a";
                case enums_BadgeStyle.Michael:
                    return selected ? "shop_michael_icon_b" : "shop_michael_icon_a";
                case enums_BadgeStyle.Star:
                    return "shop_new_star";
                case enums_BadgeStyle.Tatoo:
                    return selected ? "shop_tattoos_icon_b" : "shop_tattoos_icon_";
                case enums_BadgeStyle.Tick:
                    return "shop_tick_icon";
                case enums_BadgeStyle.Trevor:
                    return selected ? "shop_trevor_icon_b" : "shop_trevor_icon_a";
                default:
                    return "";
            }
        }
        IsBagdeWhiteSprite(badge) {
            switch (badge) {
                case enums_BadgeStyle.Lock:
                case enums_BadgeStyle.Tick:
                case enums_BadgeStyle.Crown:
                    return true;
                default:
                    return false;
            }
        }
        BadgeToColor(badge, selected) {
            switch (badge) {
                case enums_BadgeStyle.Lock:
                case enums_BadgeStyle.Tick:
                case enums_BadgeStyle.Crown:
                    return selected
                        ? new Color(255, 0, 0, 0)
                        : new Color(255, 255, 255, 255);
                default:
                    return new Color(255, 255, 255, 255);
            }
        }
    }
    UIMenuItem_UIMenuItem.DefaultBackColor = Color.Empty;
    UIMenuItem_UIMenuItem.DefaultHighlightedBackColor = Color.White;
    UIMenuItem_UIMenuItem.DefaultForeColor = Color.WhiteSmoke;
    UIMenuItem_UIMenuItem.DefaultHighlightedForeColor = Color.Black;
    
    // CONCATENATED MODULE: ./items/UIMenuCheckboxItem.ts
    
    
    
    
    
    
    class UIMenuCheckboxItem_UIMenuCheckboxItem extends UIMenuItem_UIMenuItem {
        constructor(text, check = false, description = "") {
            super(text, description);
            this.OnCheckedChanged = new LiteEvent();
            this.Checked = false;
            const y = 0;
            this._checkedSprite = new Sprite_Sprite("commonmenu", "shop_box_blank", new Point(410, y + 95), new Size(50, 50));
            this.Checked = check;
        }
        get CheckedChanged() {
            return this.OnCheckedChanged.expose();
        }
        SetVerticalPosition(y) {
            super.SetVerticalPosition(y);
            this._checkedSprite.pos = new Point(380 + this.Offset.X + this.Parent.WidthOffset, y + 138 + this.Offset.Y);
        }
        Draw() {
            super.Draw();
            this._checkedSprite.pos = this._checkedSprite.pos = new Point(380 + this.Offset.X + this.Parent.WidthOffset, this._checkedSprite.pos.Y);
            const isDefaultHightlitedForeColor = this.HighlightedForeColor == UIMenuItem_UIMenuItem.DefaultHighlightedForeColor;
            if (this.Selected && isDefaultHightlitedForeColor) {
                this._checkedSprite.TextureName = this.Checked
                    ? "shop_box_tickb"
                    : "shop_box_blankb";
            }
            else {
                this._checkedSprite.TextureName = this.Checked
                    ? "shop_box_tick"
                    : "shop_box_blank";
            }
            this._checkedSprite.color = this.Enabled
                ? this.Selected && !isDefaultHightlitedForeColor
                    ? this.HighlightedForeColor
                    : this.ForeColor
                : new Color(163, 159, 148);
            this._checkedSprite.Draw();
        }
        SetRightBadge(badge) {
            return this;
        }
        SetRightLabel(text) {
            return this;
        }
    }
    
    // CONCATENATED MODULE: ./modules/ListItem.ts
    
    class ListItem_ListItem {
        constructor(text = "", data = null) {
            this.Id = UUIDV4();
            this.DisplayText = text;
            this.Data = data;
        }
    }
    
    // CONCATENATED MODULE: ./modules/ItemsCollection.ts
    
    class ItemsCollection_ItemsCollection {
        constructor(items) {
            if (items.length === 0)
                throw new Error("ItemsCollection cannot be empty");
            this.items = items;
        }
        length() {
            return this.items.length;
        }
        getListItems() {
            const items = [];
            for (const item of this.items) {
                if (item instanceof ListItem_ListItem) {
                    items.push(item);
                }
                else if (typeof item == "string") {
                    items.push(new ListItem_ListItem(item.toString()));
                }
            }
            return items;
        }
    }
    
    // CONCATENATED MODULE: ./modules/StringMeasurer.ts
    
    
    class StringMeasurer_StringMeasurer {
        static MeasureStringWidthNoConvert(input) {
            mp.game.ui.setTextEntryForWidth("STRING");
            ResText_ResText.AddLongString(input);
            mp.game.ui.setTextFont(0);
            mp.game.ui.setTextScale(0.35, 0.35);
            return mp.game.ui.getTextScreenWidth(false);
        }
        static MeasureString(str) {
            const screenw = Screen.width;
            const screenh = Screen.height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            const width = height * ratio;
            return this.MeasureStringWidthNoConvert(str) * width;
        }
    }
    
    // CONCATENATED MODULE: ./items/UIMenuListItem.ts
    
    
    
    
    
    
    
    
    
    
    
    class UIMenuListItem_UIMenuListItem extends UIMenuItem_UIMenuItem {
        constructor(text, description = "", collection = new ItemsCollection_ItemsCollection([]), startIndex = 0) {
            super(text, description);
            this.currOffset = 0;
            this.collection = [];
            this.ScrollingEnabled = true;
            this.HoldTimeBeforeScroll = 200;
            this.OnListChanged = new LiteEvent();
            this._index = 0;
            let y = 0;
            this.Collection = collection.getListItems();
            this.Index = startIndex;
            this._arrowLeft = new Sprite_Sprite("commonmenu", "arrowleft", new Point(110, 105 + y), new Size(30, 30));
            this._arrowRight = new Sprite_Sprite("commonmenu", "arrowright", new Point(280, 105 + y), new Size(30, 30));
            this._itemText = new ResText_ResText("", new Point(290, y + 104), 0.35, Color.White, enums_Font.ChaletLondon, Alignment.Right);
        }
        get Collection() {
            return this.collection;
        }
        set Collection(v) {
            if (!v)
                throw new Error("The collection can't be null");
            this.collection = v;
        }
        set SelectedItem(v) {
            const idx = this.Collection.findIndex(li => li.Id === v.Id);
            if (idx > 0)
                this.Index = idx;
            else
                this.Index = 0;
        }
        get SelectedItem() {
            return this.Collection.length > 0 ? this.Collection[this.Index] : null;
        }
        get SelectedValue() {
            return this.SelectedItem == null
                ? null
                : this.SelectedItem.Data == null
                    ? this.SelectedItem.DisplayText
                    : this.SelectedItem.Data;
        }
        get ListChanged() {
            return this.OnListChanged.expose();
        }
        get Index() {
            if (this.Collection == null)
                return -1;
            if (this.Collection != null && this.Collection.length == 0)
                return -1;
            return this._index % this.Collection.length;
        }
        set Index(value) {
            if (this.Collection == null)
                return;
            if (this.Collection != null && this.Collection.length == 0)
                return;
            this._index = 100000 - (100000 % this.Collection.length) + value;
            const caption = this.Collection.length >= this.Index
                ? this.Collection[this.Index].DisplayText
                : " ";
            this.currOffset = StringMeasurer_StringMeasurer.MeasureString(caption);
        }
        setCollection(collection) {
            this.Collection = collection.getListItems();
        }
        setCollectionItem(index, item, resetSelection = true) {
            if (index > this.Collection.length)
                throw new Error("Index out of bounds");
            if (typeof item === "string")
                item = new ListItem_ListItem(item);
            this.Collection.splice(index, 1, item);
            if (resetSelection)
                this.Index = 0;
        }
        SetVerticalPosition(y) {
            this._arrowLeft.pos = new Point(300 + this.Offset.X + this.Parent.WidthOffset, 147 + y + this.Offset.Y);
            this._arrowRight.pos = new Point(400 + this.Offset.X + this.Parent.WidthOffset, 147 + y + this.Offset.Y);
            this._itemText.pos = new Point(300 + this.Offset.X + this.Parent.WidthOffset, y + 147 + this.Offset.Y);
            super.SetVerticalPosition(y);
        }
        SetRightLabel(text) {
            return this;
        }
        SetRightBadge(badge) {
            return this;
        }
        Draw() {
            super.Draw();
            const caption = this.Collection.length >= this.Index
                ? this.Collection[this.Index].DisplayText
                : " ";
            const offset = this.currOffset;
            this._itemText.color = this.Enabled
                ? this.Selected
                    ? this.HighlightedForeColor
                    : this.ForeColor
                : new Color(163, 159, 148);
            this._itemText.caption = caption;
            this._arrowLeft.color = this.Enabled
                ? this.Selected
                    ? this.HighlightedForeColor
                    : this.ForeColor
                : new Color(163, 159, 148);
            this._arrowRight.color = this.Enabled
                ? this.Selected
                    ? this.HighlightedForeColor
                    : this.ForeColor
                : new Color(163, 159, 148);
            this._arrowLeft.pos = new Point(375 - offset + this.Offset.X + this.Parent.WidthOffset, this._arrowLeft.pos.Y);
            if (this.Selected) {
                this._arrowLeft.Draw();
                this._arrowRight.Draw();
                this._itemText.pos = new Point(405 + this.Offset.X + this.Parent.WidthOffset, this._itemText.pos.Y);
            }
            else {
                this._itemText.pos = new Point(420 + this.Offset.X + this.Parent.WidthOffset, this._itemText.pos.Y);
            }
            this._itemText.Draw();
        }
    }
    
    // CONCATENATED MODULE: ./items/UIMenuSliderItem.ts
    
    
    
    
    
    
    class UIMenuSliderItem_UIMenuSliderItem extends UIMenuItem_UIMenuItem {
        get Index() {
            return this._index % this._items.length;
        }
        set Index(value) {
            this._index = 100000000 - (100000000 % this._items.length) + value;
        }
        constructor(text, items, index, description = "", divider = false) {
            super(text, description);
            const y = 0;
            this._items = items;
            this._arrowLeft = new Sprite_Sprite("commonmenutu", "arrowleft", new Point(0, 105 + y), new Size(15, 15));
            this._arrowRight = new Sprite_Sprite("commonmenutu", "arrowright", new Point(0, 105 + y), new Size(15, 15));
            this._rectangleBackground = new ResRectangle_ResRectangle(new Point(0, 0), new Size(150, 9), new Color(4, 32, 57, 255));
            this._rectangleSlider = new ResRectangle_ResRectangle(new Point(0, 0), new Size(75, 9), new Color(57, 116, 200, 255));
            if (divider) {
                this._rectangleDivider = new ResRectangle_ResRectangle(new Point(0, 0), new Size(2.5, 20), Color.WhiteSmoke);
            }
            else {
                this._rectangleDivider = new ResRectangle_ResRectangle(new Point(0, 0), new Size(2.5, 20), Color.Transparent);
            }
            this.Index = index;
        }
        SetVerticalPosition(y) {
            this._rectangleBackground.pos = new Point(250 + this.Offset.X + this.Parent.WidthOffset, y + 158.5 + this.Offset.Y);
            this._rectangleSlider.pos = new Point(250 + this.Offset.X + this.Parent.WidthOffset, y + 158.5 + this.Offset.Y);
            this._rectangleDivider.pos = new Point(323.5 + this.Offset.X + this.Parent.WidthOffset, y + 153 + this.Offset.Y);
            this._arrowLeft.pos = new Point(235 + this.Offset.X + this.Parent.WidthOffset, 155.5 + y + this.Offset.Y);
            this._arrowRight.pos = new Point(400 + this.Offset.X + this.Parent.WidthOffset, 155.5 + y + this.Offset.Y);
            super.SetVerticalPosition(y);
        }
        IndexToItem(index) {
            return this._items[index];
        }
        Draw() {
            super.Draw();
            this._arrowLeft.color = this.Enabled
                ? this.Selected
                    ? Color.Black
                    : Color.WhiteSmoke
                : new Color(163, 159, 148);
            this._arrowRight.color = this.Enabled
                ? this.Selected
                    ? Color.Black
                    : Color.WhiteSmoke
                : new Color(163, 159, 148);
            let offset = ((this._rectangleBackground.size.Width -
                this._rectangleSlider.size.Width) /
                (this._items.length - 1)) *
                this.Index;
            this._rectangleSlider.pos = new Point(250 + this.Offset.X + offset + +this.Parent.WidthOffset, this._rectangleSlider.pos.Y);
            if (this.Selected) {
                this._arrowLeft.Draw();
                this._arrowRight.Draw();
            }
            else {
            }
            this._rectangleBackground.Draw();
            this._rectangleSlider.Draw();
            this._rectangleDivider.Draw();
        }
        SetRightBadge(badge) { }
        SetRightLabel(text) { }
    }
    
    // CONCATENATED MODULE: ./modules/Container.ts
    
    
    
    class Container_Container extends Rectangle_Rectangle {
        constructor(pos, size, color) {
            super(pos, size, color);
            this.Items = [];
        }
        addItem(item) {
            this.Items.push(item);
        }
        Draw(offset) {
            if (!this.enabled)
                return;
            offset = offset || new Size();
            const screenw = Screen.width;
            const screenh = Screen.height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            const width = height * ratio;
            const w = this.size.Width / width;
            const h = this.size.Height / height;
            const x = (this.pos.X + offset.Width) / width + w * 0.5;
            const y = (this.pos.Y + offset.Height) / height + h * 0.5;
            mp.game.graphics.drawRect(x, y, w, h, this.color.R, this.color.G, this.color.B, this.color.A);
            for (var item of this.Items)
                item.Draw(new Size(this.pos.X + offset.Width, this.pos.Y + offset.Height));
        }
    }
    
    // CONCATENATED MODULE: ./utils/Common.ts
    class Common {
        static PlaySound(audioName, audioRef) {
            mp.game.audio.playSound(-1, audioName, audioRef, false, 0, true);
        }
    }
    
    // CONCATENATED MODULE: ./index.ts
    /* harmony export (binding) */ __webpack_require__.d(__webpack_exports__, "default", function() { return index_NativeUI; });
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    class index_NativeUI {
        constructor(title, subtitle, offset, spriteLibrary, spriteName) {
            this.Id = UUIDV4();
            this.counterPretext = "";
            this.counterOverride = undefined;
            this.lastUpDownNavigation = 0;
            this.lastLeftRightNavigation = 0;
            this._activeItem = 1000;
            this.extraOffset = 0;
            this.WidthOffset = 0;
            this.Visible = true;
            this.MouseControlsEnabled = false;
            this._justOpened = true;
            this.safezoneOffset = new Point(0, 0);
            this.MaxItemsOnScreen = 9;
            this._maxItem = this.MaxItemsOnScreen;
            this.AUDIO_LIBRARY = "HUD_FRONTEND_DEFAULT_SOUNDSET";
            this.AUDIO_UPDOWN = "NAV_UP_DOWN";
            this.AUDIO_LEFTRIGHT = "NAV_LEFT_RIGHT";
            this.AUDIO_SELECT = "SELECT";
            this.AUDIO_BACK = "BACK";
            this.AUDIO_ERROR = "ERROR";
            this.MenuItems = [];
            this.IndexChange = new LiteEvent();
            this.ListChange = new LiteEvent();
            this.SliderChange = new LiteEvent();
            this.SliderSelect = new LiteEvent();
            this.CheckboxChange = new LiteEvent();
            this.ItemSelect = new LiteEvent();
            this.MenuOpen = new LiteEvent();
            this.MenuClose = new LiteEvent();
            this.MenuChange = new LiteEvent();
            this.MouseEdgeEnabled = true;
            if (!(offset instanceof Point))
                offset = Point.Parse(offset);
            this.title = title;
            this.subtitle = subtitle;
            this.spriteLibrary = spriteLibrary || "commonmenu";
            this.spriteName = spriteName || "interaction_bgd";
            this.offset = new Point(offset.X, offset.Y);
            this.Children = new Map();
            this._mainMenu = new Container_Container(new Point(0, 0), new Size(700, 500), new Color(0, 0, 0, 0));
            this._logo = new Sprite_Sprite(this.spriteLibrary, this.spriteName, new Point(0 + this.offset.X, 0 + this.offset.Y), new Size(431, 107));
            this._mainMenu.addItem((this._title = new ResText_ResText(this.title, new Point(215 + this.offset.X, 20 + this.offset.Y), 1.15, new Color(255, 255, 255), 1, Alignment.Centered)));
            if (this.subtitle !== "") {
                this._mainMenu.addItem(new ResRectangle_ResRectangle(new Point(0 + this.offset.X, 107 + this.offset.Y), new Size(431, 37), new Color(0, 0, 0, 255)));
                this._mainMenu.addItem((this._subtitle = new ResText_ResText(this.subtitle, new Point(8 + this.offset.X, 110 + this.offset.Y), 0.35, new Color(255, 255, 255), 0, Alignment.Left)));
                if (this.subtitle.startsWith("~")) {
                    this.counterPretext = this.subtitle.substr(0, 3);
                }
                this._counterText = new ResText_ResText("", new Point(425 + this.offset.X, 110 + this.offset.Y), 0.35, new Color(255, 255, 255), 0, Alignment.Right);
                this.extraOffset += 37;
            }
            this._upAndDownSprite = new Sprite_Sprite("commonmenu", "shop_arrows_upanddown", new Point(190 + this.offset.X, 147 +
                37 * (this.MaxItemsOnScreen + 1) +
                this.offset.Y -
                37 +
                this.extraOffset), new Size(50, 50));
            this._extraRectangleUp = new ResRectangle_ResRectangle(new Point(0 + this.offset.X, 144 +
                38 * (this.MaxItemsOnScreen + 1) +
                this.offset.Y -
                37 +
                this.extraOffset), new Size(431, 18), new Color(0, 0, 0, 200));
            this._extraRectangleDown = new ResRectangle_ResRectangle(new Point(0 + this.offset.X, 144 +
                18 +
                38 * (this.MaxItemsOnScreen + 1) +
                this.offset.Y -
                37 +
                this.extraOffset), new Size(431, 18), new Color(0, 0, 0, 200));
            this._descriptionBar = new ResRectangle_ResRectangle(new Point(this.offset.X, 123), new Size(431, 4), Color.Black);
            this._descriptionRectangle = new Sprite_Sprite("commonmenu", "gradient_bgd", new Point(this.offset.X, 127), new Size(431, 30));
            this._descriptionText = new ResText_ResText("Description", new Point(this.offset.X + 5, 125), 0.35, new Color(255, 255, 255, 255), enums_Font.ChaletLondon, Alignment.Left);
            this._background = new Sprite_Sprite("commonmenu", "gradient_bgd", new Point(this.offset.X, 144 + this.offset.Y - 37 + this.extraOffset), new Size(290, 25));
            gm.events.add("render", this.render.bind(this));
            console.log(`Created Native UI! ${this.title}`);
        }
        get CurrentSelection() {
            return this._activeItem % this.MenuItems.length;
        }
        set CurrentSelection(v) {
            this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
            this._activeItem = 1000 - (1000 % this.MenuItems.length) + v;
            if (this.CurrentSelection > this._maxItem) {
                this._maxItem = this.CurrentSelection;
                this._minItem = this.CurrentSelection - this.MaxItemsOnScreen;
            }
            else if (this.CurrentSelection < this._minItem) {
                this._maxItem = this.MaxItemsOnScreen + this.CurrentSelection;
                this._minItem = this.CurrentSelection;
            }
        }
        RecalculateDescriptionPosition() {
            this._descriptionBar.pos = new Point(this.offset.X, 149 - 37 + this.extraOffset + this.offset.Y);
            this._descriptionRectangle.pos = new Point(this.offset.X, 149 - 37 + this.extraOffset + this.offset.Y);
            this._descriptionText.pos = new Point(this.offset.X + 8, 155 - 37 + this.extraOffset + this.offset.Y);
            this._descriptionBar.size = new Size(431 + this.WidthOffset, 4);
            this._descriptionRectangle.size = new Size(431 + this.WidthOffset, 30);
            let count = this.MenuItems.length;
            if (count > this.MaxItemsOnScreen + 1)
                count = this.MaxItemsOnScreen + 2;
            this._descriptionBar.pos = new Point(this.offset.X, 38 * count + this._descriptionBar.pos.Y);
            this._descriptionRectangle.pos = new Point(this.offset.X, 38 * count + this._descriptionRectangle.pos.Y);
            this._descriptionText.pos = new Point(this.offset.X + 8, 38 * count + this._descriptionText.pos.Y);
        }
        SetMenuWidthOffset(widthOffset) {
            this.WidthOffset = widthOffset;
            if (this._logo != null) {
                this._logo.size = new Size(431 + this.WidthOffset, 107);
            }
            this._mainMenu.Items[0].pos = new Point((this.WidthOffset + this.offset.X + 431) / 2, 20 + this.offset.Y);
            if (this._counterText) {
                this._counterText.pos = new Point(425 + this.offset.X + widthOffset, 110 + this.offset.Y);
            }
            if (this._mainMenu.Items.length >= 2) {
                const tmp = this._mainMenu.Items[1];
                tmp.size = new Size(431 + this.WidthOffset, 37);
            }
        }
        AddItem(item) {
            if (this._justOpened)
                this._justOpened = false;
            item.Offset = this.offset;
            item.Parent = this;
            item.SetVerticalPosition(this.MenuItems.length * 25 - 37 + this.extraOffset);
            this.MenuItems.push(item);
            item.Description = this.FormatDescription(item.Description);
            this.RefreshIndex();
            this.RecalculateDescriptionPosition();
        }
        RefreshIndex() {
            if (this.MenuItems.length == 0) {
                this._activeItem = 1000;
                this._maxItem = this.MaxItemsOnScreen;
                this._minItem = 0;
                return;
            }
            for (let i = 0; i < this.MenuItems.length; i++)
                this.MenuItems[i].Selected = false;
            this._activeItem = 1000 - (1000 % this.MenuItems.length);
            this._maxItem = this.MaxItemsOnScreen;
            this._minItem = 0;
        }
        Clear() {
            this.MenuItems = [];
            this.RecalculateDescriptionPosition();
        }
        Open() {
            Common.PlaySound(this.AUDIO_BACK, this.AUDIO_LIBRARY);
            this.Visible = true;
            this._justOpened = true;
            this.MenuOpen.emit();
        }
        Close() {
            Common.PlaySound(this.AUDIO_BACK, this.AUDIO_LIBRARY);
            this.Visible = false;
            this.RefreshIndex();
            this.MenuClose.emit();
        }
        set Subtitle(text) {
            this.subtitle = text;
            this._subtitle.caption = text;
        }
        GoLeft() {
            if (!(this.MenuItems[this.CurrentSelection] instanceof UIMenuListItem_UIMenuListItem) &&
                !(this.MenuItems[this.CurrentSelection] instanceof UIMenuSliderItem_UIMenuSliderItem))
                return;
            if (this.MenuItems[this.CurrentSelection] instanceof UIMenuListItem_UIMenuListItem) {
                const it = this.MenuItems[this.CurrentSelection];
                if (it.Collection.length == 0)
                    return;
                it.Index--;
                Common.PlaySound(this.AUDIO_LEFTRIGHT, this.AUDIO_LIBRARY);
                this.ListChange.emit(it, it.Index);
            }
            else if (this.MenuItems[this.CurrentSelection] instanceof UIMenuSliderItem_UIMenuSliderItem) {
                const it = this.MenuItems[this.CurrentSelection];
                it.Index = it.Index - 1;
                Common.PlaySound(this.AUDIO_LEFTRIGHT, this.AUDIO_LIBRARY);
                this.SliderChange.emit(it, it.Index, it.IndexToItem(it.Index));
            }
        }
        GoRight() {
            if (!(this.MenuItems[this.CurrentSelection] instanceof UIMenuListItem_UIMenuListItem) &&
                !(this.MenuItems[this.CurrentSelection] instanceof UIMenuSliderItem_UIMenuSliderItem))
                return;
            if (this.MenuItems[this.CurrentSelection] instanceof UIMenuListItem_UIMenuListItem) {
                const it = this.MenuItems[this.CurrentSelection];
                if (it.Collection.length == 0)
                    return;
                it.Index++;
                Common.PlaySound(this.AUDIO_LEFTRIGHT, this.AUDIO_LIBRARY);
                this.ListChange.emit(it, it.Index);
            }
            else if (this.MenuItems[this.CurrentSelection] instanceof UIMenuSliderItem_UIMenuSliderItem) {
                const it = this.MenuItems[this.CurrentSelection];
                it.Index++;
                Common.PlaySound(this.AUDIO_LEFTRIGHT, this.AUDIO_LIBRARY);
                this.SliderChange.emit(it, it.Index, it.IndexToItem(it.Index));
            }
        }
        SelectItem() {
            if (!this.MenuItems[this.CurrentSelection].Enabled) {
                Common.PlaySound(this.AUDIO_ERROR, this.AUDIO_LIBRARY);
                return;
            }
            const it = this.MenuItems[this.CurrentSelection];
            if (this.MenuItems[this.CurrentSelection] instanceof UIMenuCheckboxItem_UIMenuCheckboxItem) {
                it.Checked = !it.Checked;
                Common.PlaySound(this.AUDIO_SELECT, this.AUDIO_LIBRARY);
                this.CheckboxChange.emit(it, it.Checked);
            }
            else {
                Common.PlaySound(this.AUDIO_SELECT, this.AUDIO_LIBRARY);
                this.ItemSelect.emit(it, this.CurrentSelection);
                if (this.Children.has(it.Id)) {
                    const subMenu = this.Children.get(it.Id);
                    this.Visible = false;
                    subMenu.Visible = true;
                    subMenu._justOpened = true;
                    subMenu.MenuOpen.emit();
                    this.MenuChange.emit(subMenu, true);
                }
            }
            it.fireEvent();
        }
        getMousePosition(relative = false) {
            const screenw = Screen.width;
            const screenh = Screen.height;
            const cursor = mp.gui.cursor.position;
            let [mouseX, mouseY] = [cursor[0], cursor[1]];
            if (relative)
                [mouseX, mouseY] = [cursor[0] / screenw, cursor[1] / screenh];
            return [mouseX, mouseY];
        }
        GetScreenResolutionMantainRatio() {
            const screenw = Screen.width;
            const screenh = Screen.height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            var width = height * ratio;
            return new Size(width, height);
        }
        IsMouseInBounds(topLeft, boxSize) {
            const res = this.GetScreenResolutionMantainRatio();
            const [mouseX, mouseY] = this.getMousePosition();
            return (mouseX >= topLeft.X &&
                mouseX <= topLeft.X + boxSize.Width &&
                (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height));
        }
        IsMouseInListItemArrows(item, topLeft, safezone) {
            mp.game.invoke("0x54ce8ac98e120cab".toUpperCase(), "jamyfafi");
            mp.game.ui.addTextComponentSubstringPlayerName(item.Text);
            var res = this.GetScreenResolutionMantainRatio();
            var screenw = res.Width;
            var screenh = res.Height;
            const height = 1080.0;
            const ratio = screenw / screenh;
            var width = height * ratio;
            const labelSize = mp.game.invoke("0x85f061da64ed2f67".toUpperCase(), 0) * width * 0.35;
            const labelSizeX = 5 + labelSize + 10;
            const arrowSizeX = 431 - labelSizeX;
            return this.IsMouseInBounds(topLeft, new Size(labelSizeX, 38))
                ? 1
                : this.IsMouseInBounds(new Point(topLeft.X + labelSizeX, topLeft.Y), new Size(arrowSizeX, 38))
                    ? 2
                    : 0;
        }
        ProcessMouse() {
            if (!this.Visible ||
                this._justOpened ||
                this.MenuItems.length == 0 ||
                !this.MouseControlsEnabled) {
                this.MenuItems.filter(i => i.Hovered).forEach(i => (i.Hovered = false));
                return;
            }
            if (!mp.gui.cursor.visible)
                mp.gui.cursor.visible = true;
            let limit = this.MenuItems.length - 1;
            let counter = 0;
            if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                limit = this._maxItem;
            if (this.IsMouseInBounds(new Point(0, 0), new Size(30, 1080)) &&
                this.MouseEdgeEnabled) {
                mp.game.cam.setGameplayCamRelativeHeading(mp.game.cam.getGameplayCamRelativeHeading() + 5.0);
                mp.game.ui.setCursorSprite(6);
            }
            else if (this.IsMouseInBounds(new Point(this.GetScreenResolutionMantainRatio().Width - 30.0, 0), new Size(30, 1080)) &&
                this.MouseEdgeEnabled) {
                mp.game.cam.setGameplayCamRelativeHeading(mp.game.cam.getGameplayCamRelativeHeading() - 5.0);
                mp.game.ui.setCursorSprite(7);
            }
            else if (this.MouseEdgeEnabled) {
                mp.game.ui.setCursorSprite(1);
            }
            for (let i = this._minItem; i <= limit; i++) {
                let xpos = this.offset.X;
                let ypos = this.offset.Y + 144 - 37 + this.extraOffset + counter * 38;
                let xsize = 431 + this.WidthOffset;
                const ysize = 38;
                const uiMenuItem = this.MenuItems[i];
                if (this.IsMouseInBounds(new Point(xpos, ypos), new Size(xsize, ysize))) {
                    uiMenuItem.Hovered = true;
                    if (mp.game.controls.isControlJustPressed(0, 24) ||
                        mp.game.controls.isDisabledControlJustPressed(0, 24))
                        if (uiMenuItem.Selected && uiMenuItem.Enabled) {
                            if (this.MenuItems[i] instanceof UIMenuListItem_UIMenuListItem &&
                                this.IsMouseInListItemArrows(this.MenuItems[i], new Point(xpos, ypos), 0) > 0) {
                                const res = this.IsMouseInListItemArrows(this.MenuItems[i], new Point(xpos, ypos), 0);
                                switch (res) {
                                    case 1:
                                        Common.PlaySound(this.AUDIO_SELECT, this.AUDIO_LIBRARY);
                                        this.MenuItems[i].fireEvent();
                                        this.ItemSelect.emit(this.MenuItems[i], i);
                                        break;
                                    case 2:
                                        var it = this.MenuItems[i];
                                        if ((it.Collection == null
                                            ? it.Items.Count
                                            : it.Collection.Count) > 0) {
                                            it.Index++;
                                            Common.PlaySound(this.AUDIO_LEFTRIGHT, this.AUDIO_LIBRARY);
                                            this.ListChange.emit(it, it.Index);
                                        }
                                        break;
                                }
                            }
                            else
                                this.SelectItem();
                        }
                        else if (!uiMenuItem.Selected) {
                            this.CurrentSelection = i;
                            Common.PlaySound(this.AUDIO_UPDOWN, this.AUDIO_LIBRARY);
                            this.IndexChange.emit(this.CurrentSelection);
                            this.SelectItem();
                        }
                        else if (!uiMenuItem.Enabled && uiMenuItem.Selected) {
                            Common.PlaySound(this.AUDIO_ERROR, this.AUDIO_LIBRARY);
                        }
                }
                else
                    uiMenuItem.Hovered = false;
                counter++;
            }
            const extraY = 144 +
                38 * (this.MaxItemsOnScreen + 1) +
                this.offset.Y -
                37 +
                this.extraOffset +
                this.safezoneOffset.Y;
            const extraX = this.safezoneOffset.X + this.offset.X;
            if (this.MenuItems.length <= this.MaxItemsOnScreen + 1)
                return;
            if (this.IsMouseInBounds(new Point(extraX, extraY), new Size(431 + this.WidthOffset, 18))) {
                this._extraRectangleUp.color = new Color(30, 30, 30, 255);
                if (mp.game.controls.isControlJustPressed(0, 24) ||
                    mp.game.controls.isDisabledControlJustPressed(0, 24)) {
                    if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                        this.GoUpOverflow();
                    else
                        this.GoUp();
                }
            }
            else
                this._extraRectangleUp.color = new Color(0, 0, 0, 200);
            if (this.IsMouseInBounds(new Point(extraX, extraY + 18), new Size(431 + this.WidthOffset, 18))) {
                this._extraRectangleDown.color = new Color(30, 30, 30, 255);
                if (mp.game.controls.isControlJustPressed(0, 24) ||
                    mp.game.controls.isDisabledControlJustPressed(0, 24)) {
                    if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                        this.GoDownOverflow();
                    else
                        this.GoDown();
                }
            }
            else
                this._extraRectangleDown.color = new Color(0, 0, 0, 200);
        }
        ProcessControl() {
            if (!this.Visible)
                return;
            if (this._justOpened) {
                this._justOpened = false;
                return;
            }
            if (mp.game.controls.isControlJustReleased(0, 177)) {
                this.GoBack();
            }
            if (this.MenuItems.length == 0)
                return;
            if (mp.game.controls.isControlPressed(0, 172) &&
                this.lastUpDownNavigation + 120 < Date.now()) {
                this.lastUpDownNavigation = Date.now();
                if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                    this.GoUpOverflow();
                else
                    this.GoUp();
            }
            else if (mp.game.controls.isControlJustReleased(0, 172)) {
                this.lastUpDownNavigation = 0;
            }
            else if (mp.game.controls.isControlPressed(0, 173) &&
                this.lastUpDownNavigation + 120 < Date.now()) {
                this.lastUpDownNavigation = Date.now();
                if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                    this.GoDownOverflow();
                else
                    this.GoDown();
            }
            else if (mp.game.controls.isControlJustReleased(0, 173)) {
                this.lastUpDownNavigation = 0;
            }
            else if (mp.game.controls.isControlPressed(0, 174) &&
                this.lastLeftRightNavigation + 100 < Date.now()) {
                this.lastLeftRightNavigation = Date.now();
                this.GoLeft();
            }
            else if (mp.game.controls.isControlJustReleased(0, 174)) {
                this.lastLeftRightNavigation = 0;
            }
            else if (mp.game.controls.isControlPressed(0, 175) &&
                this.lastLeftRightNavigation + 100 < Date.now()) {
                this.lastLeftRightNavigation = Date.now();
                this.GoRight();
            }
            else if (mp.game.controls.isControlJustReleased(0, 175)) {
                this.lastLeftRightNavigation = 0;
            }
            else if (mp.game.controls.isControlJustPressed(0, 201)) {
                this.SelectItem();
            }
        }
        FormatDescription(input) {
            if (input.length > 99)
                input = input.slice(0, 99);
            const maxPixelsPerLine = 425 + this.WidthOffset;
            let aggregatePixels = 0;
            let output = "";
            const words = input.split(" ");
            for (const word of words) {
                const offset = StringMeasurer_StringMeasurer.MeasureString(word);
                aggregatePixels += offset;
                if (aggregatePixels > maxPixelsPerLine) {
                    output += "\n" + word + " ";
                    aggregatePixels = offset + StringMeasurer_StringMeasurer.MeasureString(" ");
                }
                else {
                    output += word + " ";
                    aggregatePixels += StringMeasurer_StringMeasurer.MeasureString(" ");
                }
            }
            return output;
        }
        GoUpOverflow() {
            if (this.MenuItems.length <= this.MaxItemsOnScreen + 1)
                return;
            if (this._activeItem % this.MenuItems.length <= this._minItem) {
                if (this._activeItem % this.MenuItems.length == 0) {
                    this._minItem = this.MenuItems.length - this.MaxItemsOnScreen - 1;
                    this._maxItem = this.MenuItems.length - 1;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
                    this._activeItem = 1000 - (1000 % this.MenuItems.length);
                    this._activeItem += this.MenuItems.length - 1;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
                }
                else {
                    this._minItem--;
                    this._maxItem--;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
                    this._activeItem--;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
                }
            }
            else {
                this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
                this._activeItem--;
                this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
            }
            Common.PlaySound(this.AUDIO_UPDOWN, this.AUDIO_LIBRARY);
            this.IndexChange.emit(this.CurrentSelection);
        }
        GoUp() {
            if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                return;
            this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
            this._activeItem--;
            this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
            Common.PlaySound(this.AUDIO_UPDOWN, this.AUDIO_LIBRARY);
            this.IndexChange.emit(this.CurrentSelection);
        }
        GoDownOverflow() {
            if (this.MenuItems.length <= this.MaxItemsOnScreen + 1)
                return;
            if (this._activeItem % this.MenuItems.length >= this._maxItem) {
                if (this._activeItem % this.MenuItems.length ==
                    this.MenuItems.length - 1) {
                    this._minItem = 0;
                    this._maxItem = this.MaxItemsOnScreen;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
                    this._activeItem = 1000 - (1000 % this.MenuItems.length);
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
                }
                else {
                    this._minItem++;
                    this._maxItem++;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
                    this._activeItem++;
                    this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
                }
            }
            else {
                this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
                this._activeItem++;
                this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
            }
            Common.PlaySound(this.AUDIO_UPDOWN, this.AUDIO_LIBRARY);
            this.IndexChange.emit(this.CurrentSelection);
        }
        GoDown() {
            if (this.MenuItems.length > this.MaxItemsOnScreen + 1)
                return;
            this.MenuItems[this._activeItem % this.MenuItems.length].Selected = false;
            this._activeItem++;
            this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
            Common.PlaySound(this.AUDIO_UPDOWN, this.AUDIO_LIBRARY);
            this.IndexChange.emit(this.CurrentSelection);
        }
        GoBack() {
            Common.PlaySound(this.AUDIO_BACK, this.AUDIO_LIBRARY);
            this.Visible = false;
            if (this.ParentMenu != null) {
                this.ParentMenu.Visible = true;
                this.ParentMenu._justOpened = true;
                this.ParentMenu.MenuOpen.emit();
                this.MenuChange.emit(this.ParentMenu, false);
            }
            this.MenuClose.emit();
        }
        BindMenuToItem(menuToBind, itemToBindTo) {
            menuToBind.ParentMenu = this;
            menuToBind.ParentItem = itemToBindTo;
            this.Children.set(itemToBindTo.Id, menuToBind);
        }
        ReleaseMenuFromItem(releaseFrom) {
            if (!this.Children.has(releaseFrom.Id))
                return false;
            const menu = this.Children.get(releaseFrom.Id);
            menu.ParentItem = null;
            menu.ParentMenu = null;
            this.Children.delete(releaseFrom.Id);
            return true;
        }
        render() {
            if (!this.Visible)
                return;
            if (this._justOpened) {
                if (this._logo != null && !this._logo.IsTextureDictionaryLoaded)
                    this._logo.LoadTextureDictionary();
                if (!this._background.IsTextureDictionaryLoaded)
                    this._background.LoadTextureDictionary();
                if (!this._descriptionRectangle.IsTextureDictionaryLoaded)
                    this._descriptionRectangle.LoadTextureDictionary();
                if (!this._upAndDownSprite.IsTextureDictionaryLoaded)
                    this._upAndDownSprite.LoadTextureDictionary();
            }
            this._mainMenu.Draw();
            this.ProcessMouse();
            this.ProcessControl();
            this._background.size =
                this.MenuItems.length > this.MaxItemsOnScreen + 1
                    ? new Size(431 + this.WidthOffset, 38 * (this.MaxItemsOnScreen + 1))
                    : new Size(431 + this.WidthOffset, 38 * this.MenuItems.length);
            this._background.Draw();
            if (this.MenuItems.length > 0) {
                this.MenuItems[this._activeItem % this.MenuItems.length].Selected = true;
                if (this.MenuItems[this._activeItem % this.MenuItems.length].Description.trim() !== "") {
                    this.RecalculateDescriptionPosition();
                    let descCaption = this.MenuItems[this._activeItem % this.MenuItems.length].Description;
                    this._descriptionText.caption = descCaption;
                    const numLines = this._descriptionText.caption.split("\n").length;
                    this._descriptionRectangle.size = new Size(431 + this.WidthOffset, numLines * 25 + 15);
                    this._descriptionBar.Draw();
                    this._descriptionRectangle.Draw();
                    this._descriptionText.Draw();
                }
            }
            if (this.MenuItems.length <= this.MaxItemsOnScreen + 1) {
                let count = 0;
                for (const item of this.MenuItems) {
                    item.SetVerticalPosition(count * 38 - 37 + this.extraOffset);
                    item.Draw();
                    count++;
                }
                if (this._counterText && this.counterOverride) {
                    this._counterText.caption = this.counterPretext + this.counterOverride;
                    this._counterText.Draw();
                }
            }
            else {
                let count = 0;
                for (let index = this._minItem; index <= this._maxItem; index++) {
                    var item = this.MenuItems[index];
                    item.SetVerticalPosition(count * 38 - 37 + this.extraOffset);
                    item.Draw();
                    count++;
                }
                this._extraRectangleUp.size = new Size(431 + this.WidthOffset, 18);
                this._extraRectangleDown.size = new Size(431 + this.WidthOffset, 18);
                this._upAndDownSprite.pos = new Point(190 + this.offset.X + this.WidthOffset / 2, 147 +
                    37 * (this.MaxItemsOnScreen + 1) +
                    this.offset.Y -
                    37 +
                    this.extraOffset);
                this._extraRectangleUp.Draw();
                this._extraRectangleDown.Draw();
                this._upAndDownSprite.Draw();
                if (this._counterText) {
                    if (!this.counterOverride) {
                        const cap = this.CurrentSelection + 1 + " / " + this.MenuItems.length;
                        this._counterText.caption = this.counterPretext + cap;
                    }
                    else {
                        this._counterText.caption =
                            this.counterPretext + this.counterOverride;
                    }
                    this._counterText.Draw();
                }
            }
            this._logo.Draw();
        }
    }
    global.NativeMenu = index_NativeUI;
    global.UIMenuItem = UIMenuItem_UIMenuItem;
    global.UIMenuListItem = UIMenuListItem_UIMenuListItem;
    global.UIMenuCheckboxItem = UIMenuCheckboxItem_UIMenuCheckboxItem;
    global.UIMenuSliderItem = UIMenuSliderItem_UIMenuSliderItem;
    global.BadgeStyle = enums_BadgeStyle;
    global.Point = Point;
    global.Size = Size;
    global.Color = Color;
    global.Font = enums_Font;
    global.ItemsCollection = ItemsCollection_ItemsCollection;
    global.ListItem = ListItem_ListItem;
    
    
    /***/ })
    /******/ ]);