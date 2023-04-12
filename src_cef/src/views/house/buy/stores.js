import { writable } from 'svelte/store';

export const currentHouseData = writable([
    {
        number: 219,
        class: "Премиум +",
        price: 5000,
    },
    {
        number: 220,
        class: "Премиум",
        price: 4500,
    },
    {
        number: 218,
        class: "Эконом",
        price: 500,
    },
    {
        number: 217,
        class: "Средний",
        price: 2500,
    },
]);

export const currentBusinessData = writable([
    {
        number: 219,
        class: "Премиум +",
        price: 5000,
    },
    {
        number: 220,
        class: "Премиум",
        price: 4500,
    },
    {
        number: 218,
        class: "Эконом",
        price: 500,
    },
    {
        number: 217,
        class: "Средний",
        price: 2500,
    },
    
    {
        number: 217,
        class: "Средний",
        price: 2500,
    },
]);