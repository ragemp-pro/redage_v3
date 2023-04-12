import moment from 'moment-timezone';
import { readable, derived } from 'svelte/store';

/*export default (time, format) => {
    if (!time) return moment().tz("Europe/Moscow").format(format);
    else return moment(time).tz("Europe/Moscow").format(format);
}*/


export let TimeFormat = (time = undefined, format = undefined) => {    
    return moment(time).tz("Europe/Moscow").format(format);
}

export let TimeFormatStartOf = (time = undefined, unitOfTime = undefined) => {
	return moment(time).tz("Europe/Moscow").startOf(unitOfTime).fromNow();
}

export let TimeFormatEndOf = (time = undefined, unitOfTime = undefined, format = 'YYYY-MM-DD HH:mm:ss') => {
	return moment(time).tz("Europe/Moscow").endOf(unitOfTime).format(format);
}

export const TimeFormatStartOfReadable = (time = undefined, unitOfTime = undefined) => readable(null, function start(set) {
	const interval = setInterval(() => {
		set(moment(time).tz("Europe/Moscow").startOf(unitOfTime).fromNow());
	}, 1000);

	return function stop() {
		clearInterval(interval);
	};
});



//moment(message.Date).startOf('hour').fromNow()

export let GetTime = (time = undefined) => {    
    return moment(!time ? window.serverStore.getDateTime() : time).tz("Europe/Moscow");
}

let updateTime = false;

export const setTime = (dateTime) => {
    updateTime = GetTime (dateTime);
};

export const time = readable(new Date(), function start(set) {
	const interval = setInterval(() => {
		set(GetTime ());
	}, 50);

	return function stop() {
		clearInterval(interval);
	};
});

export const elapsed = derived(
	time,
	$time => {
        if (updateTime === "-")
            return;

        return GetTime (updateTime).diff($time);
    }
);

export const elapsedUp = derived(
	time,
	$time => {
		if (updateTime === "-")
			return;

		return GetTime ($time).diff(updateTime);
	}
);