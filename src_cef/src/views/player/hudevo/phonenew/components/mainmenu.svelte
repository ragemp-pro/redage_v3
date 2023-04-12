<script>
    import { charWorkID } from 'store/chars';
    import Header from './header.svelte'
    import { currentPage, currentWeather } from '../stores'
    
    import MapsIcon from '../assets/images/maps.png'
    import GalleryIcon from '../assets/images/gallery.png'
    import TaxiIcon from '../assets/images/taxi.png'
    import CarsIcon from '../assets/images/rent.png'
    import NewsIcon from '../assets/images/news.png'
    import MechIcon from '../assets/images/mech.png'
    import PropertyIcon from '../assets/images/property.png'
    import TinderIcon from '../assets/images/tinder.png'


    import RadioIcon from '../assets/images/radio.png'
    import ForbesIcon from '../assets/images/forbes.png'
    import TruckerIcon from '../assets/images/trucker.png'
    import AucIcon from '../assets/images/auction.png'
    import CallIcon from '../assets/images/call.png'
    import MessagesIcon from '../assets/images/messages.png'
    import CameraIcon from '../assets/images/camera.png'
    import SettingsIcon from '../assets/images/settings.png'
    import GiftIcon from '../assets/images/gift.png'

    let menuArray = [
        {
            name: "GPS",
            icon: MapsIcon,
            link: "maps"
        },
        {
            name: "Галерея",
            icon: GalleryIcon,
            link: "gallery"
        },
        {
            name: "Имущество",
            icon: PropertyIcon,
            link: "property"
        },
        {
            name: "Транспорт",
            icon: CarsIcon,
            link: "cars"
        },
        {
            name: "W.News",
            icon: NewsIcon,
            link: "news"
        },
        {
            name: "Развозчик",
            icon: TruckerIcon,
            link: "trucker",
            jobId: 6
        },
        {
            name: "Такси",
            icon: TaxiIcon,
            link: "taxi"
        },
        {
            name: "Механик",
            icon: MechIcon,
            link: "mech"
        },
/*         {
            name: "RAvito",
            icon: RAvitoIcon,
            link: "avito"
        }, */
        {
            name: "Радио",
            icon: RadioIcon,
            link: "radio"
        },
       {
            name: "Forbes",
            icon: ForbesIcon,
            link: "forbes"
        },
       /*  {
            name: "RA",
            icon: SocialIcon,
            link: "social"
        },
        {
            name: "Darknet",
            icon: DarknetIcon,
            link: "darknet"
        }, */
        {
            name: "Аукцион",
            icon: AucIcon,
            link: "auction"
        },
        {
            name: "Подарок",
            icon: GiftIcon,
            link: 101
        },
        {
            name: "Tinder",
            icon: TinderIcon,
            link: "tinder"
        },
    ]

    import WeatherWidget from './weather/widget.svelte'
    import { fade } from 'svelte/transition'


    import { onMessage } from "@/views/player/hudevo/phonenew/data";
    import {executeClientAsyncToGroup, executeClientToGroup} from "api/rage";

    const onSelectPage = (pageName) => {
        if (typeof pageName === "string") {
            if (pageName === "camera") {
                executeClientAsyncToGroup("getGallery").then((result) => {
                    result = JSON.parse(result);

                    if (result.length >= 25) {
                        currentPage.set("gallery");
                        window.notificationAdd(4, 9, `На телефоне закончилось место, сначала удалите фотографии`, 3000);
                    } else
                        currentPage.set("camera");
                });
            } else
                currentPage.set(pageName);
        } else if (typeof pageName === "number") {
            onMessage(pageName);
            executeClientToGroup("messageDefault", pageName);
        }
    }

    let background = ""
    executeClientAsyncToGroup("settings.wallpaper").then((result) => {
        background = result;
    });
</script>
<div class="newphone__background" in:fade="{{duration: 200}}" style="background-image: url({background})">
    <Header />
    <div class="newphone__mainmenu_grid">
        <WeatherWidget />

        {#each menuArray as item}
            {#if item.jobId === undefined || item.jobId === $charWorkID}
                <div class="newphone__mainmenu_element" on:click={() => onSelectPage(item.link)}>
                    <div class="newphone__mainmenu_icon" style="background-image: url({item.icon})"></div>
                    <div class="newphone__mainmenu_name">{item.name}</div>
                </div>
            {/if}
        {/each}
    </div>
    <div class="newphone__mainmenu_bottom">
        <div class="newphone__mainmenu_element" on:click={() => onSelectPage("call")}>
            <div class="newphone__mainmenu_icon" style="background-image: url({CallIcon})"></div>
        </div>
        <div class="newphone__mainmenu_element" on:click={() => onSelectPage("messages")}>
            <div class="newphone__mainmenu_icon" style="background-image: url({MessagesIcon})"></div>
        </div>
        <div class="newphone__mainmenu_element" on:click={() => onSelectPage("camera")}>
            <div class="newphone__mainmenu_icon" style="background-image: url({CameraIcon})"></div>
        </div>
        <div class="newphone__mainmenu_element" on:click={() => onSelectPage("settings")}>
            <div class="newphone__mainmenu_icon" style="background-image: url({SettingsIcon})"></div>
        </div>
    </div>
</div>