const
    clientName = "client.phone.",
    rpcName = "rpc.phone.",
    serverName = "server.phone.";

phoneData.weatherList = [];

gm.events.add(clientName + "initWeather", (json) => {
    phoneData.weatherList = [];

    json = JSON.parse(json);

    let weatherList = [];

    json.forEach((item) => {
        weatherList.push({
            weatherId: item[0],
            hour: item[1],
            minute: item[2],
            temp: item[3]
        });
    });

    phoneData.weatherList = weatherList;
});

rpc.register(rpcName + "getWeather", () => {
    return JSON.stringify(phoneData.weatherList.slice(0, 6));
});

rpc.register(rpcName + "getCurrentWeather", () => {
    return JSON.stringify(phoneData.weatherList [0]);
});

gm.events.add(clientName + "addWeather", (weatherId, hour, minute, temp) => {
    phoneData.weatherList.splice(0, 1);

    phoneData.weatherList.push({
        weatherId: weatherId,
        hour: hour,
        minute: minute,
        temp: temp
    });
});

gm.events.add(clientName + "updWeather", (weatherId, temp) => {
    phoneData.weatherList [0].weatherId = weatherId;
    phoneData.weatherList [0].temp = temp;
});