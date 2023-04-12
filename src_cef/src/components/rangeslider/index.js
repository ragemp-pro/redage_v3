!function(t, e) {
    "object" == typeof exports && "object" == typeof module ? module.exports = e() : "function" == typeof define && define.amd ? define([], e) : "object" == typeof exports ? exports.rangesliderJs = e() : t.rangesliderJs = e()
}(window, function() {
    return function(t) {
        var e = {};
        function n(i) {
            if (e[i])
                return e[i].exports;
            var s = e[i] = {
                i: i,
                l: !1,
                exports: {}
            };
            return t[i].call(s.exports, s, s.exports, n),
            s.l = !0,
            s.exports
        }
        return n.m = t,
        n.c = e,
        n.d = function(t, e, i) {
            n.o(t, e) || Object.defineProperty(t, e, {
                enumerable: !0,
                get: i
            })
        }
        ,
        n.r = function(t) {
            "undefined" != typeof Symbol && Symbol.toStringTag && Object.defineProperty(t, Symbol.toStringTag, {
                value: "Module"
            }),
            Object.defineProperty(t, "__esModule", {
                value: !0
            })
        }
        ,
        n.t = function(t, e) {
            if (1 & e && (t = n(t)),
            8 & e)
                return t;
            if (4 & e && "object" == typeof t && t && t.__esModule)
                return t;
            var i = Object.create(null);
            if (n.r(i),
            Object.defineProperty(i, "default", {
                enumerable: !0,
                value: t
            }),
            2 & e && "string" != typeof t)
                for (var s in t)
                    n.d(i, s, function(e) {
                        return t[e]
                    }
                    .bind(null, s));
            return i
        }
        ,
        n.n = function(t) {
            var e = t && t.__esModule ? function() {
                return t.default
            }
            : function() {
                return t
            }
            ;
            return n.d(e, "a", e),
            e
        }
        ,
        n.o = function(t, e) {
            return Object.prototype.hasOwnProperty.call(t, e)
        }
        ,
        n.p = "",
        n(n.s = 3)
    }([function(t, e, n) {
        "use strict";
        var i = function(t) {
            return "number" == typeof t && !isNaN(t)
        };
        t.exports = function(t, e) {
            var n = (e = e || e.currentTarget).getBoundingClientRect()
              , s = t.originalEvent || t
              , r = 0
              , o = 0;
            return t.touches && t.touches.length ? i(t.touches[0].pageX) && i(t.touches[0].pageY) ? (r = t.touches[0].pageX,
            o = t.touches[0].pageY) : i(t.touches[0].clientX) && i(t.touches[0].clientY) && (r = s.touches[0].clientX,
            o = s.touches[0].clientY) : i(t.pageX) && i(t.pageY) ? (r = t.pageX,
            o = t.pageY) : t.currentPoint && i(t.currentPoint.x) && i(t.currentPoint.y) && (r = t.currentPoint.x,
            o = t.currentPoint.y),
            {
                x: r - n.left,
                y: o - n.top
            }
        }
    }
    , function(t, e, n) {
        (function(e) {
            var n = e.CustomEvent;
            t.exports = function() {
                try {
                    var t = new n("cat",{
                        detail: {
                            foo: "bar"
                        }
                    });
                    return "cat" === t.type && "bar" === t.detail.foo
                } catch (t) {}
                return !1
            }() ? n : "undefined" != typeof document && "function" == typeof document.createEvent ? function(t, e) {
                var n = document.createEvent("CustomEvent");
                return e ? n.initCustomEvent(t, e.bubbles, e.cancelable, e.detail) : n.initCustomEvent(t, !1, !1, void 0),
                n
            }
            : function(t, e) {
                var n = document.createEventObject();
                return n.type = t,
                e ? (n.bubbles = Boolean(e.bubbles),
                n.cancelable = Boolean(e.cancelable),
                n.detail = e.detail) : (n.bubbles = !1,
                n.cancelable = !1,
                n.detail = void 0),
                n
            }
        }
        ).call(this, n(2))
    }
    , function(t, e) {
        var n;
        n = function() {
            return this
        }();
        try {
            n = n || new Function("return this")()
        } catch (t) {
            "object" == typeof window && (n = window)
        }
        t.exports = n
    }
    , function(t, e, n) {
        "use strict";
        n.r(e);
        var i = n(1)
          , s = n.n(i);
        function r(t) {
            return 0 === t.offsetWidth || 0 === t.offsetHeight || !1 === t.open
        }
        window.requestAnimationFrame = window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame;
        var o = Number.isNaN || function(t) {
            return t != t
        }
          , a = Number.isFinite || function(t) {
            return !("number" != typeof t || o(t) || t === 1 / 0 || t === -1 / 0)
        }
        ;
        var u = function() {
            var t = []
              , e = !1;
            function n() {
                e || (e = !0,
                window.requestAnimationFrame ? window.requestAnimationFrame(i) : setTimeout(i, 66))
            }
            function i() {
                t.forEach(function(t) {
                    t()
                }),
                e = !1
            }
            return {
                add: function(e) {
                    !t.length && window.addEventListener("resize", n),
                    function(e) {
                        e && t.push(e)
                    }(e)
                }
            }
        }()
          , l = {
            emit: function(t, e, n) {
                t.dispatchEvent(new s.a(e,n))
            },
            isFiniteNumber: a,
            getFirstNumberLike: function() {
                if (!arguments.length)
                    return null;
                for (var t = 0, e = arguments.length; t < e; t++)
                    if (n = arguments[t],
                    a(parseFloat(n)) || a(n))
                        return arguments[t];
                var n
            },
            getDimension: function(t, e) {
                var n, i = function(t) {
                    for (var e = [], n = t.parentNode; n && r(n); )
                        e.push(n),
                        n = n.parentNode;
                    return e
                }(t), s = i.length, o = [], a = t[e], u = 0;
                function l(t) {
                    void 0 !== t.open && (t.open = !t.open)
                }
                if (s) {
                    for (u = 0; u < s; u++)
                        n = i[u].style,
                        o[u] = n.display,
                        n.display = "block",
                        n.height = "0",
                        n.overflow = "hidden",
                        n.visibility = "hidden",
                        l(i[u]);
                    for (a = t[e],
                    u = 0; u < s; u++)
                        n = i[u].style,
                        l(i[u]),
                        n.display = o[u],
                        n.height = "",
                        n.overflow = "",
                        n.visibility = ""
                }
                return a
            },
            insertAfter: function(t, e) {
                t.parentNode.insertBefore(e, t.nextSibling)
            },
            forEachAncestorsAndSelf: function(t, e) {
                for (e(t); t.parentNode && !e(t); )
                    t = t.parentNode;
                return t
            },
            clamp: function(t, e, n) {
                return e < n ? t < e ? e : t > n ? n : t : t < n ? n : t > e ? e : t
            },
            optimizedResize: u
        }
          , h = n(0)
          , c = n.n(h)
          , d = {
            MIN_DEFAULT: 0,
            MAX_DEFAULT: 100,
            RANGE_CLASS: "rangeslider",
            FILL_CLASS: "rangeslider__fill",
            FILL_BG_CLASS: "rangeslider__fill__bg",
            HANDLE_CLASS: "rangeslider__handle",
            DISABLED_CLASS: "rangeslider--disabled",
            STEP_DEFAULT: 1,
            START_EVENTS: ["mousedown", "touchstart", "pointerdown"],
            MOVE_EVENTS: ["mousemove", "touchmove", "pointermove"],
            END_EVENTS: ["mouseup", "touchend", "pointerup"],
            PLUGIN_NAME: "rangeslider-js"
        };
        function f(t, e) {
            for (var n = 0; n < e.length; n++) {
                var i = e[n];
                i.enumerable = i.enumerable || !1,
                i.configurable = !0,
                "value"in i && (i.writable = !0),
                Object.defineProperty(t, i.key, i)
            }
        }
        var p = function(t) {
            var e = document && document.createElement("div");
            return e.classList.add(t),
            e
        }
          , m = function(t) {
            return "".concat(t).replace(".", "").length - 1
        }
          , v = function() {
            function t(e, n) {
                var i = this;
                !function(t, e) {
                    if (!(t instanceof e))
                        throw new TypeError("Cannot call a class as a function")
                }(this, t),
                n = n || {},
                this.element = e,
                this.options = n,
                this.onSlideEventsCount = -1,
                this.isInteracting = !1,
                this.needTriggerEvents = !1,
                this.constructor.count = this.constructor.count || 0,
                this.identifier = "js-".concat(d.PLUGIN_NAME, "-").concat(this.constructor.count++),
                this.min = l.getFirstNumberLike(n.min, parseFloat(e.getAttribute("min")), d.MIN_DEFAULT),
                this.max = l.getFirstNumberLike(n.max, parseFloat(e.getAttribute("max")), d.MAX_DEFAULT),
                this.value = l.getFirstNumberLike(n.value, parseFloat(e.getAttribute("value")), this.min + (this.max - this.min) / 2),
                this.step = l.getFirstNumberLike(n.step, parseFloat(e.getAttribute("step")), d.STEP_DEFAULT),
                this.percent = null,
                this._updatePercentFromValue(),
                this.toFixed = m(this.step),
                this.range = p(d.RANGE_CLASS),
                this.range.id = this.identifier,
                this.fillBg = p(d.FILL_BG_CLASS),
                this.fill = p(d.FILL_CLASS),
                this.handle = p(d.HANDLE_CLASS),
                ["fillBg", "fill", "handle"].forEach(function(t) {
                    return i.range.appendChild(i[t])
                }),
                ["min", "max", "step"].forEach(function(t) {
                    return e.setAttribute(t, "".concat(i[t]))
                }),
                this._setValue(this.value),
                l.insertAfter(e, this.range),
                e.style.position = "absolute",
                e.style.width = "1px",
                e.style.height = "1px",
                e.style.overflow = "hidden",
                e.style.opacity = "0",
                ["_update", "_handleDown", "_handleMove", "_handleEnd", "_startEventListener", "_changeEventListener"].forEach(function(t) {
                    i[t] = i[t].bind(i)
                }),
                this._init(),
                l.optimizedResize.add(this._update),
                d.START_EVENTS.forEach(function(t) {
                    return i.range.addEventListener(t, i._startEventListener)
                }),
                e.addEventListener("change", this._changeEventListener)
            }
            var e, n, i;
            return e = t,
            (n = [{
                key: "_init",
                value: function() {
                    this._update(),
                    this.options.onInit && this.options.onInit.call(this, this.value, this.percent, this.position)
                }
            }, {
                key: "_updatePercentFromValue",
                value: function() {
                    this.percent = (this.value - this.min) / (this.max - this.min)
                }
            }, {
                key: "_startEventListener",
                value: function(t) {
                    var e = t.target
                      , n = this.identifier
                      , i = !1;
                    l.forEachAncestorsAndSelf(e, function(t) {
                        return i = t.id === n && !t.classList.contains(d.DISABLED_CLASS)
                    }),
                    i && this._handleDown(t)
                }
            }, {
                key: "_changeEventListener",
                value: function(t, e) {
                    (e && e.origin) !== this.identifier && this._setPosition(this._getPositionFromValue(t.target.value))
                }
            }, {
                key: "_update",
                value: function() {
                    this.handleWidth = l.getDimension(this.handle, "offsetWidth"),
                    this.rangeWidth = l.getDimension(this.range, "offsetWidth"),
                    this.maxHandleX = this.rangeWidth - this.handleWidth,
                    this.grabX = this.handleWidth / 2,
                    this.position = this._getPositionFromValue(this.value),
                    this.range.classList[this.element.disabled ? "add" : "remove"](d.DISABLED_CLASS),
                    this._setPosition(this.position),
                    this._updatePercentFromValue(),
                    this._emit("change")
                }
            }, {
                key: "_listen",
                value: function(t) {
                    var e = this
                      , n = "".concat(t ? "add" : "remove", "EventListener");
                    d.MOVE_EVENTS.forEach(function(t) {
                        return document && document[n](t, e._handleMove)
                    }),
                    d.END_EVENTS.forEach(function(t) {
                        document && document[n](t, e._handleEnd),
                        e.range[n](t, e._handleEnd)
                    })
                }
            }, {
                key: "_handleDown",
                value: function(t) {
                    if (t.preventDefault(),
                    this.isInteracting = !0,
                    this._listen(!0),
                    !t.target.classList.contains(d.HANDLE_CLASS)) {
                        var e = c()(t, this.range).x
                          , n = this.range.getBoundingClientRect().left
                          , i = this.handle.getBoundingClientRect().left - n;
                        this._setPosition(e - this.grabX),
                        e >= i && e < i + this.handleWidth && (this.grabX = e - i),
                        this._updatePercentFromValue()
                    }
                }
            }, {
                key: "_handleMove",
                value: function(t) {
                    this.isInteracting = !0,
                    t.preventDefault();
                    var e = c()(t, this.range).x;
                    this._setPosition(e - this.grabX)
                }
            }, {
                key: "_handleEnd",
                value: function(t) {
                    t.preventDefault(),
                    this._listen(!1),
                    this._emit("change"),
                    (this.isInteracting || this.needTriggerEvents) && this.options.onSlideEnd && this.options.onSlideEnd.call(this, this.value, this.percent, this.position),
                    this.onSlideEventsCount = 0,
                    this.isInteracting = !1
                }
            }, {
                key: "_setPosition",
                value: function(t) {
                    var e = this.isInteracting ? this._getValueFromPosition(l.clamp(t, 0, this.maxHandleX)) : this.value
                      , n = this._getPositionFromValue(e);
                    this.fill.style.width = n + this.grabX + "px",
                    this.handle.style.webkitTransform = this.handle.style.transform = "translate(".concat(n, "px, -50%)"),
                    this._setValue(e),
                    this.position = n,
                    this.value = e,
                    this._updatePercentFromValue(),
                    (this.isInteracting || this.needTriggerEvents) && (this.options.onSlideStart && 0 === this.onSlideEventsCount && this.options.onSlideStart.call(this, this.value, this.percent, this.position),
                    this.options.onSlide && this.options.onSlide.call(this, this.value, this.percent, this.position)),
                    this.onSlideEventsCount++
                }
            }, {
                key: "_getPositionFromValue",
                value: function(t) {
                    return (t - this.min) / (this.max - this.min) * this.maxHandleX
                }
            }, {
                key: "_getValueFromPosition",
                value: function(t) {
                    var e = t / (this.maxHandleX || 1)
                      , n = this.step * Math.round(e * (this.max - this.min) / this.step) + this.min;
                    return Number(n.toFixed(this.toFixed))
                }
            }, {
                key: "_setValue",
                value: function(t) {
                    (t = l.clamp(t, this.min, this.max)) === this.value && t === this.element.value || (this.value = this.element.value = t,
                    this._emit("input"))
                }
            }, {
                key: "_emit",
                value: function(t) {
                    l.emit(this.element, t, {
                        origin: this.identifier
                    })
                }
            }, {
                key: "update",
                value: function(t, e) {
                    return t = t || {},
                    Object.keys(t).forEach(function(e) {
                        return "string" == typeof t[e] && (t[e] = parseFloat(t[e]))
                    }),
                    this.needTriggerEvents = e,
                    l.isFiniteNumber(t.min) && (this.element.setAttribute("min", "".concat(t.min)),
                    this.min = t.min),
                    l.isFiniteNumber(t.max) && (this.element.setAttribute("max", "".concat(t.max)),
                    this.max = t.max),
                    l.isFiniteNumber(t.step) && (this.element.setAttribute("step", "".concat(t.step)),
                    this.step = t.step,
                    this.toFixed = m(t.step)),
                    l.isFiniteNumber(t.value) && this._setValue(t.value),
                    this._update(),
                    this.onSlideEventsCount = 0,
                    this.needTriggerEvents = !1,
                    this
                }
            }, {
                key: "destroy",
                value: function() {
                    var t = this;
                    "undefined" != typeof window && window.removeEventListener("resize", this._update, !1),
                    d.START_EVENTS.forEach(function(e) {
                        return t.range.removeEventListener(e, t._startEventListener)
                    }),
                    this.element.removeEventListener("change", this._changeEventListener),
                    this.element.style.cssText = "",
                    delete this.element[d.PLUGIN_NAME],
                    this.range.parentNode.removeChild(this.range)
                }
            }]) && f(e.prototype, n),
            i && f(e, i),
            t
        }();
        e.default = {
            RangeSlider: v,
            utils: l,
            create: function(t, e) {
                function n(t) {
                    t[d.PLUGIN_NAME] = t[d.PLUGIN_NAME] || new v(t,e)
                }
                t && (t.length ? Array.prototype.slice.call(t).forEach(function(t) {
                    return n(t)
                }) : n(t))
            }
        }
    }
    ]).default
});
