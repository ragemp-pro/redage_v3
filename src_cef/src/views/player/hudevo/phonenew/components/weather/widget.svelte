<script>
    import { executeClientAsyncToGroup } from "api/rage";
    import { currentPage } from './../../stores'

    import { weatherName, formatTime, getWeatherIdToName } from './data'

    let currentWeather = {}

    executeClientAsyncToGroup("getCurrentWeather").then((result) => {
        if (result && typeof result === "string") {
            currentWeather = JSON.parse(result);
        }
    });
</script>

<div class="newphone__mainmenu_weather" on:click={() => currentPage.set("weather")} style={getWeatherIdToName(currentWeather.weatherId, currentWeather.hour) == ("night" || "nightcloud" || "nightfog" || "nightrain" || "nightthunder" || "nightsnow") ? "color:black" : ""}>
    <div class="newphone__mainmenu_currentweather newphone__weather_image{getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)}">
        <div class="box-flex">
            Los Santos <span class="phoneicons-location"></span>
        </div>
        <div class="newphone__weather_gradus">
            {#if currentWeather.temp}
                -{currentWeather.temp}&#176;C
            {:else}
                5&#176;C
            {/if}
        </div>
        <div class="box-flex">
            <div class="newphone__weather_icon {getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)}"></div>
            <div class="newphone__weather_text">{weatherName[getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)]}</div>
        </div>
    </div>
    <div class="newphone__mainmenu_name">Погода</div>
</div>