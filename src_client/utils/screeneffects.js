var lastScreenEffect = "";
gm.events.add('startScreenEffect', function (effectName, duration, looped) {
	try {
		lastScreenEffect = effectName;
		mp.game.graphics.startScreenEffect(effectName, duration, looped);
	} catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/screeneffects", "startScreenEffect", e.toString());
	}
});

gm.events.add('stopScreenEffect', function (effectName) {
	try {
		var effect = (effectName == undefined) ? lastScreenEffect : effectName;
		mp.game.graphics.stopScreenEffect(effect);
	} catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/screeneffects", "stopScreenEffect", e.toString());
	}
});

gm.events.add('stopAndStartScreenEffect', function (stopEffect, startEffect, duration, looped) {
	try {
		mp.game.graphics.stopScreenEffect(stopEffect);
		mp.game.graphics.startScreenEffect(startEffect, duration, looped);
	} catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/screeneffects", "stopAndStartScreenEffect", e.toString());
	}
});

gm.events.add('screenFadeOut', function (duration) {
	try
	{
		global.FadeScreen (true, duration);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/screeneffects", "screenFadeOut", e.toString());
	}
});

gm.events.add('screenFadeIn', function (duration) {
	try
	{
		global.FadeScreen (false, duration);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/screeneffects", "screenFadeIn", e.toString());
	}
});