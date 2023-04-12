
import npc_tracyJson from './ValentineDay/npc_tracy.json';
import npc_doctorJson from './ValentineDay/npc_doctor.json';
import npc_grannyJson from './ValentineDay/npc_granny.json';


import npc_fd_dadaJson from './defenderFatherlandDay/npc_dada.json';
import npc_fd_edwardJson from './defenderFatherlandDay/npc_pavel.json';
import npc_fd_zakJson from './defenderFatherlandDay/npc_zak.json';

import npc_airdrop from './npc_airdrop.json';
import npc_oressale from './npc_oressale.json';
import npc_fracpolic from './fraction/npc_fracpolic.json';
import npc_fracsheriff from './fraction/npc_fracsheriff.json';
import npc_fracnews from './fraction/npc_fracnews.json';
import npc_fracems from './fraction/npc_fracems.json';
import npc_premium from './biz/npc_premium.json';
import npc_huntingshop from './npc_huntingshop.json';
import npc_treessell from './npc_treessell.json';

import npc_stock from './npc_stock.json';

import npc_donateautoroom from './npc_donateautoroom.json';
import npc_cityhall from './npc_cityhall.json';
import npc_wedding from './npc_wedding.json';

import npc_pet from './biz/npc_pet.json';
import npc_petshop from './biz/npc_petshop.json';
import npc_rieltor from './biz/npc_rieltor.json';
import npc_furniture from './biz/npc_furniture.json';
import npc_carevac from './npc_carevac.json';
import npc_airshop from './npc_airshop.json';
import npc_eliteroom from './npc_eliteroom.json';

import npc_zdobich from './npc_zdobich.json';

import npc_automechanic from './work/npc_automechanic.json';
import npc_bus from './work/npc_bus.json';
import npc_collector from './work/npc_collector.json';
import npc_electrician from './work/npc_electrician.json';
import npc_gopostal from './work/npc_gopostal.json';
import npc_lawnmower from './work/npc_lawnmower.json';
import npc_taxi from './work/npc_taxi.json';
import npc_truckers from './work/npc_truckers.json';
import npc_org from './npc_org.json';
import npc_birthday from './npc_birthday.json';

/* type
    quest - Обычный квест который только в меню
    lists - Простой список с квестами не помню для чего
    talk - разговорный квест
*/
const list = {
    npc_tracy: npc_tracyJson,
    npc_doctor: npc_doctorJson,
    npc_granny: npc_grannyJson,

    npc_fd_dada: npc_fd_dadaJson,
    npc_fd_edward: npc_fd_edwardJson,
    npc_fd_zak: npc_fd_zakJson,

    npc_airdrop: npc_airdrop,
    npc_oressale: npc_oressale,
    npc_fracpolic: npc_fracpolic,
    npc_fracsheriff: npc_fracsheriff,
    npc_fracnews: npc_fracnews,
    npc_fracems: npc_fracems,
    
    npc_premium: npc_premium,

    npc_stock: npc_stock,
    npc_huntingshop: npc_huntingshop,
    npc_treessell: npc_treessell,

    npc_donateautoroom: npc_donateautoroom,
    npc_cityhall: npc_cityhall,
    npc_wedding: npc_wedding,

    npc_pet: npc_pet,
    npc_petshop: npc_petshop,
    npc_furniture: npc_furniture,
    npc_rieltor: npc_rieltor,
    npc_carevac: npc_carevac,
    npc_airshop: npc_airshop,
    npc_eliteroom: npc_eliteroom,

    npc_zdobich: npc_zdobich,

    npc_automechanic: npc_automechanic,
    npc_bus: npc_bus,
    npc_collector: npc_collector,
    npc_electrician: npc_electrician,
    npc_gopostal: npc_gopostal,
    npc_lawnmower: npc_lawnmower,
    npc_taxi: npc_taxi,
    npc_truckers: npc_truckers,
    npc_org: npc_org,
    npc_birthday: npc_birthday,
}

const actorData = {
    npc_tracy: {
        name: "Трейси"
    },
    npc_doctor: {
        name: "Доктор Шульц"
    },
    npc_granny: {
        name: "Бабушка Грэнни"
    },

    npc_fd_dada: {
        name: "Дядюшка"
    },
    npc_fd_edward: {
        name: "Эдвард"
    },
    npc_fd_zak: {
        name: "Зак Цукерберг"
    },
    npc_airdrop: {
        name: "Juan de cartel"
    },
    npc_oressale: {
        name: "Марк"
    },
    npc_fracpolic: {
        name: "Работник полиции"
    },
    npc_fracsheriff: {
        name: "Шериф"
    },
    npc_fracnews: {
        name: "Дженнифер"
    },
    npc_fracems: {
        name: "Эммануэль"
    },
    npc_premium: {
        name: "Вовчик"
    },
    npc_stock: {
        name: "Александр"
    },
    npc_huntingshop: {
        name: "Беар Гриллс"
    },
    npc_treessell: {
        name: "Дмитрий"
    },
    npc_donateautoroom: {
        name: "Доната Редбаксовна"
    },
    npc_cityhall: {
        name: "Эльнара Каримова"
    },
    npc_wedding: {
        name: "Отец Михаил"
    },
    npc_pet: {
        name: "Ветеринар Михаил"
    },
    npc_petshop: {
        name: "Продавец питомцев"
    },
    npc_zdobich: {
        name: "Виталий Дебич"
    },
    npc_rieltor: {
        name: "Илон Таск"
    },
    npc_furniture: {
        name: "Иван"
    },
    npc_carevac: {
        name: "Роберт"
    },
    npc_airshop: {
        name: "Продавец воздушного транспорта"
    },
    npc_eliteroom: {
        name: "Продавец элитного транспорта"
    },
    npc_automechanic: {
        name: "Главный механик"
    },
    npc_bus: {
        name: "Начальник автоколонны"
    },
    npc_collector: {
        name: "Банковский HR"
    },
    npc_electrician: {
        name: "Старший электрик"
    },
    npc_gopostal: {
        name: "Старший почтальон"
    },
    npc_lawnmower: {
        name: "Главный газонокосильщик"
    },
    npc_taxi: {
        name: "Директор таксопарка"
    },
    npc_truckers: {
        name: "Водитель-дальнобойщик"
    },
    npc_org: {
        name: "Директор Полли"
    },
    npc_birthday: {
        name: "Праздничная обезьянка"
    },
}

export const getQuests = () => {
    return questsnpc_tailerJsonJson;
}

export const getQuest = (name, questId) => {
    return list[name][questId];
}

export const getActors = (name) => {
    return actorData[name];
}