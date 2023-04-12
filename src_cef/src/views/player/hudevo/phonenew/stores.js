import { writable } from 'svelte/store';
import {executeClientToGroup} from "api/rage";

export const isMapLoad = writable(false);

export const isSim = writable(false);

export const selectedImage = writable(false);
export const selectedImageFunc = writable(false);
export const radioState = writable(false);
export const radioStation = writable(0);


let pageArray = [];

export const currentPage = writable("mainmenu");

currentPage.subscribe(page => {
    if (page === "mainmenu") {
        pageArray = [];
        executeClientToGroup ("finger", 1)
    } else if (page !== "callView") {
        pageArray.push(page)
        executeClientToGroup ("finger", 5)
    }
});


export const pageBack = () => {

    let page = "mainmenu";
    const lastIndex = pageArray.length - 1;

    if (typeof pageArray [lastIndex] === "string") {
        page = pageArray [lastIndex];
        pageArray.splice(lastIndex, 1);
    }

    if (page !== "call")
        selectNumber.set(null);

    currentPage.set (page);
}


export const selectNumber = writable(null);




export const currentWeather = writable("thunder");


export const categoriesList = writable([
    {
        name: "Гос. структуры",
        icon: "gos",
        content: [
            "City Hall",
            "LSPD",
            "EMS",
            "FIB",
            "NEWS",
            "Центр управления",
            "SHERIFF 1",
            "SHERIFF 2",
        ]
    },
    {
        name: "Банды",
        icon: "weapons",
        content: [
            "Marabunta Grande",
            "Vagos",
            "Ballas",
            "The Families",
            "Bloods Street",
        ]
    },
    {
        name: "Мафии",
        icon: "mafia",
        content: [
            "La Cosa Nostra",
            "Русская мафия",
            "Yakuza",
            "Армянская мафия",
        ]
    },
    {
        name: "Работы",
        icon: "licenses",
        content: [
            "Электростанция",
            "Отделение почты",
            "Таксопарк",
            "Автобусный парк",
            "Стоянка газонокосилок",
            "Стоянка дальнобойщиков",
            "Стоянка инкассаторов",
            "Стоянка автомехаников",
        ]
    },
    {
        name: "Подработка",
        icon: "jobs",
        content: [
            "Гражданская шахта 1",
            "Гражданская шахта 2",
            "Гражданская шахта 3",
            "Гражданская шахта 4",
            "Государственная шахта",
            "Лесоруб 1",
            "Лесоруб 2",
            "Лесоруб 3",
            "Лесоруб 4",
            "Лесоруб 5",
        ]
    },
    {
        name: "Ближайшие места",
        icon: "recent",
        content: [

            "Ближайшая аренда мотоциклов",
            "Ближайшая аренда велосипеда",
            "Ближайшая аренда лодки",
           /* "Ближайшая аренда самолета",
            "Ближайшая аренда вертолета",*/
        ]
    },
    {
        name: "Прочее",
        icon: "clubs",
        content: [
            "Автошкола",
            "Казино",
            "Семьи",
            "Арена",
            "Амфитеатр",
            "Humane Labs",
            "Маяк",
            "Охотничий магазин",
            "Главный рынок",
            "Черный рынок",
            "Церковь",
            "Продавец питомцев",
            "Суд",
        ]
    }
])