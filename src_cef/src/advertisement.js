/*import axios from 'axios';
import { accountLogin, accountData } from 'store/account'

let data = {};
accountData.subscribe(value => {
	data = value;
}); 


let loginUser = "";
accountLogin.subscribe(value => {
	loginUser = value;
});

const code = 'UA-138889592-2';
const ec = 'ra_game';

window.inAdvertisement = (event1, event2) => {
    try {
        console.log("inAdvertisement")
        console.log(loginUser)
        let url = `https://www.google-analytics.com/collect?v=1&tid=${code}&cid=${data.Login}&el=${data.Email}&t=event&ec=${ec}`;
        console.log(url)
        url += `&ea=${event1}`
        if (event2) 
            url += `&ev=${event2}`
        console.log(url)
    
        axios.get(url);
    } catch (err) {
        //return false;
    }
}*/

import axios from 'axios';
import { accountData } from 'store/account';

const code = '';
const ec = '';

let data = {};
accountData.subscribe(value => {
    data = value;
});


window.inAdvertisement = (event1, event2) => {

    try {
        const cid = Math.round (new Date().getTime() * Math.random());
        let url = `https://www.google-analytics.com/collect?v=1&tid=${code}&cid=${cid}&el=${data.Email}&t=event&ec=${ec}`;
        //console.log(url)
        url += `&ea=${event1}`
        if (event2 && event2 !== undefined && event2 !== "undefined")
            url += `&ev=${event2}`

        console.log(url)
        axios.get(url);
    } catch (err) {
        //return false;
    }
}