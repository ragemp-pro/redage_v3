import { writable } from 'svelte/store';
import { executeClient } from 'api/rage'

window.animationStore = {};

//Анимации
export const storeAnimFavorites = writable([]);
let animFavorites = [];
storeAnimFavorites.subscribe(value => {
	animFavorites = value;
}); 

window.animationStore.initAnimFavorites = (json) => {
    storeAnimFavorites.set (JSON.parse (json));
}

window.animationStore.addAnimFavorite = (anim) => {   
    if (!animFavorites) animFavorites = [];
    const listIndex = animFavorites.findIndex(a => a == anim);
    if (!animFavorites [listIndex]) {
        animFavorites = [
            ...animFavorites,
            anim
        ];
        executeClient ("client.animation.favorite", JSON.stringify (animFavorites));
        storeAnimFavorites.set(animFavorites);
    }            
}

window.animationStore.dellAnimFavorite = (anim) => {
    if (!animFavorites) animFavorites = [];
    const listIndex = animFavorites.findIndex(a => a == anim);
    if (animFavorites [listIndex]) {
        animFavorites.splice(listIndex, 1);
        executeClient ("client.animation.favorite", JSON.stringify (animFavorites));
        storeAnimFavorites.set(animFavorites);
    }           
}

export const storeAnimBind = writable([0,0,0,0,0,0,0,0,0,0]);
let animBind = [0,0,0,0,0,0,0,0,0,0];
storeAnimBind.subscribe(value => {
	animBind = value;
}); 


window.animationStore.initAnimBind = (json) => {
    storeAnimBind.set (JSON.parse (json));
}

window.animationStore.addAnimBind = (index, anim) => {
    if (!animBind) animBind = [];                
    const listIndex = animBind.findIndex(a => a == anim);
    if (animBind [listIndex]) animBind [listIndex] = 0;            
    animBind [index] = anim;
    executeClient ("client.animation.bind", JSON.stringify (animBind));
    storeAnimBind.set(animBind);         
}

window.animationStore.dellAnimBind = (anim) => {
    if (!animBind) animBind = [];                
    const listIndex = animBind.findIndex(a => a == anim);
    if (animBind [listIndex]) animBind [listIndex] = 0;
    executeClient ("client.animation.bind", JSON.stringify (animBind));
    storeAnimBind.set(animBind);         
}