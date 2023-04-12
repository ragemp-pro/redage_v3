<script>
    import Header from '../header.svelte'
    import HomeButton from '../homebutton.svelte'
    import { TimeFormat } from 'api/moment'
    import { serverDateTime } from 'store/server'

    import { executeClientAsyncToGroup } from "api/rage";


    import { weatherName, formatTime, getWeatherIdToName } from './data'

    let weatherInfo = []
    let currentWeather = {}

    executeClientAsyncToGroup("getWeather").then((result) => {
        if (result && typeof result === "string") {
            weatherInfo = JSON.parse(result);

            currentWeather = weatherInfo[0];
        }
    });

    import { fade } from 'svelte/transition'
</script>
<div class="newphone__weather newphone__weather_{getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)}" in:fade>
    <Header />
    <div class="newphone__weather_content" style={["night", "nightcloud", "nightfog", "nightrain", "nightthunder", "nightsnow"].includes(getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)) ? "color:white" : "color:black"}>
        <div class="newphone__weather_header">
            <div class="newphone__weather_headicon"></div>
            Los Santos <span class="blue">Weather</span>
        </div>
        <div class="box-column">
            <div class="newphone__weather_day">
                {TimeFormat ($serverDateTime, "dddd, DD MMMM")}
            </div>
            <div class="newphone__weather_city">
                Los Santos
            </div>
            <div class="newphone__weather_image {getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)}"></div>
        </div>
        <div class="newphone__weather_temp">
            <div class="newphone__weather_number">-{currentWeather.temp}</div>
            <div class="box-column">
                <div class="newphone__weather_gr">&#176;</div>
                <div class="newphone__weather_type">{weatherName[getWeatherIdToName(currentWeather.weatherId, currentWeather.hour)]}</div>
            </div>
        </div>
        <div class="newphone__weather_list">
            {#each weatherInfo as weather}
                <div class="newphone__weather_element">
                    <div class="newphone__weather_time">{formatTime(weather.hour)}:{formatTime(weather.minute)}</div>
                    <div class="newphone__weather_icon {getWeatherIdToName (weather.weatherId, weather.hour)}"></div>
                    <div class="newphone__weather_temper">{weather.temp}</div>
                </div>
            {/each}
        </div>
    </div>
    <HomeButton />
</div>