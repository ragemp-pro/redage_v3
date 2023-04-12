mp.game.audio.setAudioFlag("DisableFlightMusic", true); // Отключаем звук при полёте.

global.getSafeZCoords = (x, y, forz, found) =>
{
	try
	{
		var zcoord = 0.0;
		var intervalcount = 0;

		var interval = setInterval(function() {
			try 
			{
				intervalcount++;
				mp.game.streaming.setFocusArea(x, y, 1000.0, 0.0, 0.0, 0.0);

				if(forz == 0)
				{
					for(var i = 800; i >= 0; i -= 20)
					{
						var checkz = i + 0.1;
			
						mp.game.streaming.requestCollisionAtCoord(x, y, checkz);
						if(intervalcount >= 50) global.localplayer.setCoordsNoOffset(x, y, checkz, false, false, false);
			
						zcoord = mp.game.gameplay.getGroundZFor3dCoord(x, y, checkz, 0.0, false);
						if(zcoord !== 0.0)
						{
							mp.game.invoke("0x198F77705FA0931D", global.localplayer.handle);
			
							found(zcoord + 0.1);
							clearInterval(interval);
							return;
						}
					}
				}
				else
				{
					zcoord = mp.game.gameplay.getGroundZFor3dCoord(x, y, forz, 0.0, false);
					mp.game.invoke("0x198F77705FA0931D", global.localplayer.handle);

					found(zcoord + 0.1);
					clearInterval(interval);
					return;
				}

				if(intervalcount >= 100)
				{
					if(intervalcount >= 50) global.localplayer.setCoordsNoOffset(x, y, 10.0, false, false, false);
					mp.game.invoke("0x198F77705FA0931D", global.localplayer.handle);

					clearInterval(interval);
					return;
				}
			}
			catch (e) 
			{
				mp.events.callRemote("client_trycatch", "utils/other", "setInterval", e.toString());
			}
			
		}, 1);
	}
	catch (e) 
	{
		mp.events.callRemote("client_trycatch", "utils/other", "getSafeZCoords", e.toString());
	}
}