
import axios from 'axios';
import qs from 'querystring';

const config = {
    headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
    }
}

function toDataUrl(url, callback) {
    
    var xhr = new XMLHttpRequest();
    xhr.onload = function() {
        var reader = new FileReader();
        reader.onloadend = function() {
            callback(reader.result);
        }
        reader.readAsDataURL(xhr.response);
    };

    
    xhr.open('GET', url);
    xhr.responseType = 'blob';
    xhr.send();
}

const width = 640;
const height = 360;

const getConvert = (img, callback) => {

    let image = new Image();
    image.src = img;
    image.onload = function () {
        const canvas = document.createElement('canvas'),
            context = canvas.getContext('2d');

        canvas.width = width;
        canvas.height = height;
        context.drawImage(image, 0, 0, width, height);

        callback(canvas.toDataURL());
    };
}

window.screenshot_getbase64 = async (url) => {
    const response = await axios.get(url, {responseType: 'arraybuffer'});
    const image = btoa(new Uint8Array(response.data).reduce((data, byte) => data + String.fromCharCode(byte), ''));

    const base64 = `data:${response.headers['content-type'].toLowerCase()};base64,${image}`;

    getConvert(base64, (resizeBase64) => {
        resizeBase64 = resizeBase64.replace(/^data:image\/[a-z]+;base64,/, "");

        getUrl (resizeBase64)
    });

    /*toDataUrl(url, (dataURL) => {
        dataURL = dataURL.replace(/^data:image\/[a-z]+;base64,/, "");
        
        getUrl (dataURL)
    });*/
}

const getUrl = (dataURL) => {
    axios.post(document.api + 'save', qs.stringify({
        base64image: dataURL
    }), config)
    .then(function (response) {
        if (response && response.data) {                
            response = response.data;

            if (response.link) {
                window.listernEvent ('cameraLink', response.link);
            } else if (response.error) {
                window.notificationAdd(4, 9, response.error, 3000);
            }
        }
    })
    axios.post(document.api + 'test')
    .then(function (response) {
        if (response && response.data) {                
            response = response.data;
        }
    })
}
