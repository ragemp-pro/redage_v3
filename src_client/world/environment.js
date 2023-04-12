let updateGameTime = true;
let updateGameWeather = true;

let unixtime = 0;

let nowDate = {
	hours: 0,
	minutes: 0
};

global.DateTime = null;

gm.events.add('DateTime', (dateTime) => {
	global.DateTime = JSON.parse(dateTime);
	mp.gui.emmit(`window.serverStore.serverDateTime (${dateTime})`);
})

gm.events.add('SetTime', (hours, minutes, unix) => {
	nowDate.hours = hours;
	mp.gui.emmit(`window.hudStore.setHour (${hours})`);
	nowDate.minutes = minutes;

	if (updateGameTime)
		mp.game.time.setClockTime(nowDate.hours, nowDate.minutes, 0);

	unixtime = unix;
	updateWeather ();
})

/*
gm.events.add(global.renderName ["1s"], () => {
	unixtime++;
});
*/

const weatherArray = [ "EXTRASUNNY", "CLEAR", "CLOUDS", "SMOG", "FOGGY", "OVERCAST", "RAIN", "THUNDER", "CLEARING", "NEUTRAL", "SNOW", "BLIZZARD", "SNOWLIGHT", "XMAS", "HALLOWEEN" ];


let weather1 = null;
let weather2 = null;

let weatherNextTime = null;

gm.events.add('SetWeather', (w1, w2, wTime) => {
	weather1 = mp.game.gameplay.getHashKey(weatherArray[w1]);
	weather2 = mp.game.gameplay.getHashKey(weatherArray[w2]);

	weatherNextTime = wTime;

	updateWeather ();
})

let customWeather = false;

gm.events.add('SetWeatherCMD', (number) => {
	if (weatherArray[number])
		customWeather = weatherArray[number];
	else
		customWeather = false;
});

gm.events.add('weatherinfo', () => {
	let wId1 = 0;
	let wId2 = 0;
	for(let i = 0; i < weatherArray.length; i++) {
		if (mp.game.gameplay.getHashKey(weatherArray [i]) === weather1)
			wId1 = i;

		if (mp.game.gameplay.getHashKey(weatherArray [i]) === weather2)
			wId2 = i;
	}
	const time = weatherNextTime - unixtime;
	let progress = 1.0 - time / 1800;

	if (progress > 1.0)
		progress = 1.0;
	else if (progress < 0.0)
		progress = 0.0;

	mp.gui.chat.push(`${weatherArray [wId1]} (${wId1}) | ${weatherArray [wId2]}  (${wId2}) | ${progress} (${1.0 - time / 1800}) | ${weatherNextTime} - ${unixtime} = ${time}`);
})

gm.events.add(global.renderName ["1s"], () => {

	unixtime -= 1;

});


const updateWeather = () => {
	//if (!global.loggedin) return;

	if (!updateGameWeather) {
		mp.game.gameplay.setWeatherTypeNow(weatherArray[0]);
		return;
	}
	if (customWeather) {
		mp.game.gameplay.setWeatherTypeNow(customWeather);
		return;
	}

	const time = weatherNextTime - unixtime;
	let progress = 1.0 - time / 1800;

	if (progress > 1.0)
		progress = 1.0;
	else if (progress < 0.0)
		progress = 0.0;

	mp.game.gameplay.setWeatherTypeTransition(weather1, weather2, progress);
}

gm.events.add("render", () => {
	if (!global.loggedin) return;

	updateWeather ();
})

gm.events.add('setTimeCmd', (hour, minute, second) => {
	if(hour == -1 && minute == -1 && second == -1) {
		updateGameTime = true;
		mp.gui.emmit(`window.hudStore.setHour (${nowDate.hours})`);
		mp.game.time.setClockTime(nowDate.hours, nowDate.minutes, 0);
	} else {
		updateGameTime = false;
		mp.gui.emmit(`window.hudStore.setHour (${hour})`);
		mp.game.time.setClockTime(hour, minute, second);
	}
})

gm.events.add('stopTime', () => {
	updateGameTime = false;
	updateGameWeather = false;

	mp.game.time.setClockTime(0, 0, 0);
	mp.gui.emmit(`window.hudStore.setHour (${0})`);
});

gm.events.add('resumeTime', () => {
	updateGameTime = true;
	updateGameWeather = true;

	mp.game.time.setClockTime(nowDate.hours, nowDate.minutes, 0);
	mp.gui.emmit(`window.hudStore.setHour (${nowDate.hours})`);
})


const rendersData = [
	{
		calls: [
			global.renderName ["render"]
		],
		maxTime: 0
	},
	{
		calls: [
			global.renderName ["1s"], global.renderName ["sound"]
		],
		maxTime: 1000
	},
	{
		calls: [
			global.renderName ["2s"]
		],
		maxTime: 2000
	},
	{
		calls: [
			global.renderName ["2.5ms"]
		],
		maxTime: 2500
	},
	{
		calls: [
			global.renderName ["5s"]
		],
		maxTime: 5000
	},
	{
		calls: [
			global.renderName ["10s"]
		],
		maxTime: 10000
	},
	{
		calls: [
			global.renderName ["50ms"]
		],
		maxTime: 50
	},
	{
		calls: [
			global.renderName ["100ms"], global.renderName ["soundRot"]
		],
		maxTime: 100
	},
	{
		calls: [
			global.renderName ["125ms"]
		],
		maxTime: 125
	},
	{
		calls: [
			global.renderName ["150ms"]
		],
		maxTime: 150
	},
	{
		calls: [
			global.renderName ["250ms"]
		],
		maxTime: 250
	},
	{
		calls: [
			global.renderName ["200ms"]
		],
		maxTime: 200
	},
	{
		calls: [
			global.renderName ["350ms"]
		],
		maxTime: 350
	},
	{
		calls: [
			global.renderName ["500ms"]
		],
		maxTime: 500
	},
]

rendersData.forEach((renderData) => {
	renderData.calls.forEach((callName) => {
		setInterval(() => {
			mp.events.call(callName);
		}, renderData.maxTime);
	});
});

