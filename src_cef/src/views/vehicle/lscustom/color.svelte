<script>
    import './color.css'
    import { executeClient } from 'api/rage'
    import tinycolor from 'api/tinycolor.min.js'

    const colorLists = [
        [
            '#FFFFFF',
            '#FFFB0D',
            '#0532FF',
            '#FF9300',
            '#00F91A',
            '#FF2700',
            '#000000',
            '#686868',
            '#EE5464',
            '#D27AEE',
            '#5BA8C4',
            '#E64AA9'
        ],
        [
            "#0d1116",
            "#1c1d21",
            "#32383d",
            "#454b4f",
            "#999da0",
            "#c2c4c6",
            "#979a97",
            "#637380",
            "#63625c",
            "#3c3f47",
            "#444e54",
            "#1d2129",
            "#13181f",
            "#26282a",
            "#515554",
            "#151921",
            "#1e2429",
            "#333a3c",
            "#8c9095",
            "#39434d",
            "#506272",
            "#1e232f",
            "#363a3f",
            "#a0a199",
            "#d3d3d3",
            "#b7bfca",
            "#778794",
            "#c00e1a",
            "#da1918",
            "#b6111b",
            "#a51e23",
            "#7b1a22",
            "#8e1b1f",
            "#6f1818",
            "#49111d",
            "#b60f25",
            "#d44a17",
            "#c2944f",
            "#f78616",
            "#cf1f21",
            "#732021",
            "#f27d20",
            "#ffc91f",
            "#9c1016",
            "#de0f18",
            "#8f1e17",
            "#a94744",
            "#b16c51",
            "#371c25",
            "#132428",
            "#122e2b",
            "#12383c",
            "#31423f",
            "#155c2d",
            "#1b6770",
            "#66b81f",
            "#22383e",
            "#1d5a3f",
            "#2d423f",
            "#45594b",
            "#65867f",
            "#222e46",
            "#233155",
            "#304c7e",
            "#47578f",
            "#637ba7",
            "#394762",
            "#d6e7f1",
            "#76afbe",
            "#345e72",
            "#0b9cf1",
            "#2f2d52",
            "#282c4d",
            "#2354a1",
            "#6ea3c6",
            "#112552",
            "#1b203e",
            "#275190",
            "#608592",
            "#2446a8",
            "#4271e1",
            "#3b39e0",
            "#1f2852",
            "#253aa7",
            "#1c3551",
            "#4c5f81",
            "#58688e",
            "#74b5d8",
            "#ffcf20",
            "#fbe212",
            "#916532",
            "#e0e13d",
            "#98d223",
            "#9b8c78",
            "#503218",
            "#473f2b",
            "#221b19",
            "#653f23",
            "#775c3e",
            "#ac9975",
            "#6c6b4b",
            "#402e2b",
            "#a4965f",
            "#46231a",
            "#752b19",
            "#bfae7b",
            "#dfd5b2",
            "#f7edd5",
            "#3a2a1b",
            "#785f33",
            "#b5a079",
            "#fffff6",
            "#eaeaea",
            "#b0ab94",
            "#453831",
            "#2a282b",
            "#726c57",
            "#6a747c",
            "#354158",
            "#9ba0a8",
            "#5870a1",
            "#eae6de",
            "#dfddd0",
            "#f2ad2e",
            "#f9a458",
            "#83c566",
            "#f1cc40",
            "#4cc3da",
            "#4e6443",
            "#bcac8f",
            "#f8b658",
            "#fcf9f1",
            "#fffffb",
            "#81844c",
            "#ffffff",
            "#f21f99",
            "#fdd6cd",
            "#df5891",
            "#f6ae20",
            "#b0ee6e",
            "#08e9fa",
            "#0a0c17",
            "#0c0d18",
            "#0e0d14",
            "#9f9e8a",
            "#621276",
            "#0b1421",
            "#11141a",
            "#6b1f7b",
            "#1e1d22",
            "#bc1917",
            "#2d362a",
            "#696748",
            "#7a6c55",
            "#c3b492",
            "#5a6352",
            "#81827f",
            "#afd6e4",
            "#7a6440",
            "#7f6a48",
        ],
        [
            "#ffffff00",
            "#ffffff",
            "#0000ff",//1
            "#7DF9FF",//2
            "#98FF98",//3
            "#32cd32",//4
            "#ffff00",//Yellow 
            "#ffd700",
            "#ffa500",
            "#ff0000",
            "#EE3580",
            "#FF69B4",
            "#A020F0",
            "#5945E5",
        ]
    ]
    import { onMount } from 'svelte'

    onMount(() => {
        let canvas;
        let ctx;

        let _ = document.querySelector.bind(document);
        let spectrumCanvas = document.getElementById('spectrum-canvas');
        let spectrumCtx = spectrumCanvas ? spectrumCanvas.getContext('2d') : undefined;
        let spectrumCursor = document.getElementById('spectrum-cursor');
        let spectrumRect = spectrumCanvas ? spectrumCanvas.getBoundingClientRect() : undefined;


        //https://github.com/bgrins/TinyColor

        //let addSwatch = document.getElementById('add-swatch');
        let swatches = document.getElementsByClassName('default-swatches')[0];
        let colorIndicator = document.getElementById('color-indicator');
        //let userSwatches = document.getElementById('user-swatches');

        let currentColor;
        let hueCanvas = document.getElementById('hue-canvas');
        let hueCtx = hueCanvas ? hueCanvas.getContext('2d') : undefined;
        let hueCursor = document.getElementById('hue-cursor');
        let hueRect = hueCanvas ? hueCanvas.getBoundingClientRect() : undefined;

        let hue = 0;
        let saturation = 1;
        let lightness = .5;


        let red = document.getElementById('red');
        let blue = document.getElementById('blue');
        let green = document.getElementById('green');

        function ColorPicker() {
            this.addDefaultSwatches();
            createShadeSpectrum();
            createHueSpectrum();
        };
        
        ColorPicker.prototype.defaultSwatches = colorLists [lists];

        function createSwatch(target, color, index) {
            let swatch = document.createElement('button');
            swatch.classList.add('swatch');
            swatch.setAttribute('title', color);
            swatch.style.backgroundColor = color;
            swatch.addEventListener('click', function () {
                let color = tinycolor(this.style.backgroundColor);
                if (red) {
                    colorToPos(color);
                    setColorValues(color);
                } else {
                    executeClient ('client.custom.coloritem', index);
                }                    
            });
            target.appendChild(swatch);
            refreshElementRects();
            setColorValues(color);
        };

        ColorPicker.prototype.addDefaultSwatches = function () {
            for (let i = 0; i < this.defaultSwatches.length; ++i) {
                createSwatch(swatches, this.defaultSwatches[i], i);
            }
        }

        function refreshElementRects() {
            if (spectrumCanvas == undefined) return;
            spectrumRect = spectrumCanvas.getBoundingClientRect();
            hueRect = hueCanvas.getBoundingClientRect();
        }

        function createShadeSpectrum(color) {
            if (spectrumCanvas == undefined) return;
            canvas = spectrumCanvas;
            ctx = spectrumCtx;
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            if (!color) color = '#f00';
            ctx.fillStyle = color;
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            let whiteGradient = ctx.createLinearGradient(0, 0, canvas.width, 0);
            whiteGradient.addColorStop(0, "#fff");
            whiteGradient.addColorStop(1, "transparent");
            ctx.fillStyle = whiteGradient;
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            let blackGradient = ctx.createLinearGradient(0, 0, 0, canvas.height);
            blackGradient.addColorStop(0, "transparent");
            blackGradient.addColorStop(1, "#000");
            ctx.fillStyle = blackGradient;
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            canvas.addEventListener('mousedown', function (e) {
                startGetSpectrumColor(e);
            });
        };

        function createHueSpectrum() {
            if (hueCanvas == undefined) return;
            let canvas = hueCanvas;
            let ctx = hueCtx;
            let hueGradient = ctx.createLinearGradient(0, 0, 0, canvas.height);
            hueGradient.addColorStop(0.00, "hsl(0,100%,50%)");
            hueGradient.addColorStop(0.17, "hsl(298.8, 100%, 50%)");
            hueGradient.addColorStop(0.33, "hsl(241.2, 100%, 50%)");
            hueGradient.addColorStop(0.50, "hsl(180, 100%, 50%)");
            hueGradient.addColorStop(0.67, "hsl(118.8, 100%, 50%)");
            hueGradient.addColorStop(0.83, "hsl(61.2,100%,50%)");
            hueGradient.addColorStop(1.00, "hsl(360,100%,50%)");
            ctx.fillStyle = hueGradient;
            ctx.fillRect(0, 0, canvas.width, canvas.height);
            canvas.addEventListener('mousedown', function (e) {
                startGetHueColor(e);
            });
        };

        function colorToHue(color) {
            if (spectrumRect == undefined) return;
            color = tinycolor(color);
            let hueString = tinycolor('hsl ' + color.toHsl().h + ' 1 .5').toHslString();
            return hueString;
        };

        function colorToPos(color) {
            if (spectrumRect == undefined) return;
            color = tinycolor(color);
            let hsl = color.toHsl();
            hue = hsl.h;
            let hsv = color.toHsv();
            let x = spectrumRect.width * hsv.s;
            let y = spectrumRect.height * (1 - hsv.v);
            let hueY = hueRect.height - ((hue / 360) * hueRect.height);
            updateSpectrumCursor(x, y);
            updateHueCursor(hueY);
            setCurrentColor(color);
            createShadeSpectrum(colorToHue(color));
        };

        function setColorValues(color) {
            //convert to tinycolor object
            color = tinycolor(color);
            let rgbValues = color.toRgb();
            //set inputs
            if (red) red.value = rgbValues.r;
            if (green) green.value = rgbValues.g;
            if (blue) blue.value = rgbValues.b;
        };

        function setCurrentColor(color) {
            //if (spectrumCursor == undefined) return;
            color = tinycolor(color);
            currentColor = color;
            if (colorIndicator) colorIndicator.style.backgroundColor = color;
            //document.body.style.backgroundColor = color; 
            if (spectrumCursor) spectrumCursor.style.backgroundColor = color;
            if (hueCursor) hueCursor.style.backgroundColor = 'hsl(' + color.toHsl().h + ', 100%, 50%)';
            
            color = color.toRgb();
            executeClient ('client.custom.color', color.r, color.g, color.b);
        };

        function updateHueCursor(y) {
            hueCursor.style.top = y + 'px';
        }

        function updateSpectrumCursor(x, y) {
            if (spectrumCursor == undefined) return;
            //assign position
            spectrumCursor.style.left = x + 'px';
            spectrumCursor.style.top = y + 'px';
        };

        let startGetSpectrumColor = function (e) {
            if (spectrumCursor == undefined) return;
            getSpectrumColor(e);
            spectrumCursor.classList.add('dragging');
            window.addEventListener('mousemove', getSpectrumColor);
            window.addEventListener('mouseup', endGetSpectrumColor);
        };

        function getSpectrumColor(e) {
            if (spectrumRect == undefined) return;
            // got some help here - http://stackoverflow.com/questions/23520909/get-hsl-value-given-x-y-and-hue
            e.preventDefault();
            //get x/y coordinates
            let x = e.pageX - spectrumRect.left;
            let y = e.pageY - spectrumRect.top;
            //constrain x max
            if (x > spectrumRect.width) { x = spectrumRect.width }
            if (x < 0) { x = 0 }
            if (y > spectrumRect.height) { y = spectrumRect.height }
            if (y < 0) { y = .1 }
            //convert between hsv and hsl
            let xRatio = x / spectrumRect.width * 100;
            let yRatio = y / spectrumRect.height * 100;
            let hsvValue = 1 - (yRatio / 100);
            let hsvSaturation = xRatio / 100;
            lightness = (hsvValue / 2) * (2 - hsvSaturation);
            saturation = (hsvValue * hsvSaturation) / (1 - Math.abs(2 * lightness - 1));
            let color = tinycolor('hsl ' + hue + ' ' + saturation + ' ' + lightness);
            setCurrentColor(color);
            setColorValues(color);
            updateSpectrumCursor(x, y);
        };

        function endGetSpectrumColor(e) {
            if (spectrumCursor == undefined) return;
            spectrumCursor.classList.remove('dragging');
            window.removeEventListener('mousemove', getSpectrumColor);
        };

        function startGetHueColor(e) {
            if (hueCursor == undefined) return;
            getHueColor(e);
            hueCursor.classList.add('dragging');
            window.addEventListener('mousemove', getHueColor);
            window.addEventListener('mouseup', endGetHueColor);
        };

        function getHueColor(e) {
            if (hueRect == undefined) return;
            e.preventDefault();
            let y = e.pageY - hueRect.top;
            if (y > hueRect.height) { y = hueRect.height };
            if (y < 0) { y = 0 };
            let percent = y / hueRect.height;
            hue = 360 - (360 * percent);
            let hueColor = tinycolor('hsl ' + hue + ' 1 .5').toHslString();
            let color = tinycolor('hsl ' + hue + ' ' + saturation + ' ' + lightness).toHslString();
            createShadeSpectrum(hueColor);
            updateHueCursor(y, hueColor)
            setCurrentColor(color);
            setColorValues(color);
        };

        function endGetHueColor(e) {
            if (hueCursor == undefined) return;
            hueCursor.classList.remove('dragging');
            window.removeEventListener('mousemove', getHueColor);
        };

        // Add event listeners
        if (red) {
            red.addEventListener('change', function () {
                let color = tinycolor('rgb ' + red.value + ' ' + green.value + ' ' + blue.value);
                colorToPos(color);
            });

        }

        if (green) {
            green.addEventListener('change', function () {
                let color = tinycolor('rgb ' + red.value + ' ' + green.value + ' ' + blue.value);
                colorToPos(color);
            });            
        }

        if (blue) {            
            blue.addEventListener('change', function () {
                let color = tinycolor('rgb ' + red.value + ' ' + green.value + ' ' + blue.value);
                colorToPos(color);
            });
        }

        /*addSwatch.addEventListener('click', function(){  
        createSwatch(userSwatches, currentColor);
        });*/



        new ColorPicker();
    });

    export let title;
    export let lists
</script>
<div class="color-picker-panel" class:big={lists} on:mouseenter={() => executeClient("client.camera.toggled", false)} on:mouseleave={() => executeClient("client.camera.toggled", true)}>
    <h2 class="panel-header top">{title}</h2>
    <div class="panel-row">
        <div class={"swatches default-swatches " + (lists === 0 || "big"+lists)}></div>
        {#if lists === 0}<div id="color-indicator" class="color-button preview"></div>{/if}
    </div>
    {#if lists === 0}
    <div class="panel-row mrt-18">
        <div class="spectrum-map">
            <div id="spectrum-cursor" class="color-cursor"></div>
            <canvas id="spectrum-canvas"></canvas>
        </div>
        <div class="hue-map">
            <div id="hue-cursor" class="color-cursor"></div>
            <canvas id="hue-canvas"></canvas>
        </div>
    </div>
    {/if}
    {#if lists === 0}
    <div class="panel-row mrt-18">
        <div id="rgb-fields" class="field-group value-fields rgb-fields active">
            <div class="field-group">
                <label for="_" class="field-label">R:</label>
                <input type="number" max="255" min="0" id="red" class="field-input rgb-input" />
            </div>
            <div class="field-group">
                <label for="_" class="field-label">G:</label>
                <input type="number" max="255" min="0" id="green" class="field-input rgb-input" />
            </div>
            <div class="field-group">
                <label for="_" class="field-label">B:</label>
                <input type="number" max="255" min="0" id="blue" class="field-input rgb-input" />
            </div>
        </div>
    </div>
    {/if}
</div>