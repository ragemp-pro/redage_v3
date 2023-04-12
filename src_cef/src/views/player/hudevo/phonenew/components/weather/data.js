export const weatherName = {
    cloud : "Облачно",
    fog : "Туман",
    rain : "Дождь",
    snow : "Снег",
    sunny : "Солнечно",
    thunder : "Гроза",
    night: "Ночь",
    nightcloud: "Облачно",
    nightfog: "Туман",
    nightrain: "Дождь",
    nightthunder: "Гроза",
    nightsnow: "Снег"
}

export const formatTime = (time) => ("0" + time).slice(0 - 2);

export const getWeatherIdToName = (weatherId, hour) => {
    if(hour > 7 && hour < 21){
        switch (weatherId) {
            case 0:
                return "sunny";
            case 1:
                return "sunny";
            case 2: 
                return "cloud"
            case 3: 
                return "cloud"
            case 4: 
                return "fog"
            case 5: 
                return "cloud"
            case 6: 
                return "rain"
            case 7: 
                return "thunder"
            case 8: 
                return "rain"
            case 9: 
                return "cloud"
            case 10: 
                return "snow"
            case 11: 
                return "snow"
            case 12: 
                return "snow"
            case 13:
                return "snow";
        }
    }
    else{
        switch (weatherId) {
            case 0:
                return "night";
            case 1:
                return "night";
            case 2: 
                return "nightcloud"
            case 3: 
                return "nightcloud"
            case 4: 
                return "nightfog"
            case 5: 
                return "nightcloud"
            case 6: 
                return "nightrain"
            case 7: 
                return "nightthunder"
            case 8: 
                return "nightrain"
            case 9: 
                return "nightcloud"
            case 10: 
                return "nightsnow"
            case 11: 
                return "nightsnow"
            case 12: 
                return "nightsnow"
            case 13:
                return "nightsnow";
        }
    }
}