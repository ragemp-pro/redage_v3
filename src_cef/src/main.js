import App from './App.svelte';

import moment from 'moment';
import momentDurationFormatSetup from 'moment-duration-format';
momentDurationFormatSetup(moment)

window.wait = time => new Promise(resolve => setTimeout(resolve, time))

moment.locale('ru');

//console.log(TimeFormat ("2021-08-17T00:44:10.8644836+03:00", "H:M DD.MM.YYYY"))
//console.log(TimeFormat ("8/18/2021 12:34:38 AM", "H:M DD.MM.YYYY"))
const app = new App({
	target: document.body,
});

export default app;