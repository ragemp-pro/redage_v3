
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
    uploadToImgur(dataURL)
    .then(() => {
        console.log("Image uploaded to Imgur successfully");
    })
    .catch((error) => {
        console.error("Failed to upload image to Imgur:", error);
        window.notificationAdd(4, 9, error.message, 3000);
     });
};

const uploadToImgur = async (base64Image) => {
    try
    {
        const response = await axios.post(
            "https://api.imgur.com/3/image",
            {
                image: base64Image,
            },
            {
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Client-ID ${document.imgurClientId}`,
                },
            }
        );

        if (response.status === 200)
        {
            const link = response.data.data.link;
            window.listernEvent("cameraLink", link);
        }

    }
    catch (error)
    {
        console.error("Failed to upload image to Imgur:", error);
    }
};