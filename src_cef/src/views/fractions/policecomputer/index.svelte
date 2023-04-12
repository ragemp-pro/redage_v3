<script>
    import { translateText } from 'lang'
    import './css/main.css'
    import { executeClient } from 'api/rage';
    let
        vehicleData = {model: "", owner: ""},
        userData = {fname: "", lname: "", pass: "", phonenumber: "", gender: "", lvl: "", lic: "", houseInfo: ""},
        wantedData = [];
    
    window.policecomputer = {
        openCar: (model, owner) => {
            vehicleData = {model: model, owner: owner};
        },
        openPerson: (name, lastname, uuid, fractionname, pnumber, gender, wantedlvl, lic, houseInfo) => {
            userData = {fname: name, lname: lastname, pass: uuid, fraction_name: fractionname, phonenumber: pnumber, gender: gender, lvl: wantedlvl, lic: lic, houseInfo: houseInfo};
        },
        openWanted: (data) => {
            wantedData = JSON.parse(data);
        }
    }

    import ClearWanted from './elements/clearwanted.svelte'
    import OpenNumber from './elements/opennumber.svelte'
    import OpenPersone from './elements/openpersone.svelte'
    import WantedList from './elements/wantedlist.svelte'
    let SelectViews = "";

    const Views = {
        ClearWanted,
        OpenNumber,
        OpenPersone,
        WantedList
    }
</script>

<div class="pc">
    <div class="screen">
        <div class="left">
            <menu>
                <li on:click={() => SelectViews = "ClearWanted"}>{translateText('fractions', 'Очистить розыск')}</li>
                <li on:click={() => SelectViews = "OpenNumber"}>{translateText('fractions', 'Пробить номера')}</li>
                <li on:click={() => SelectViews = "OpenPersone"}>{translateText('fractions', 'Пробить по базе')}</li>
                <li on:click={() => {
                    executeClient ("client:wantedListRequest");
                    SelectViews = "WantedList";
                }}>{translateText('fractions', 'Список разыскиваемых')}</li>
                <li on:click={() => executeClient ('client:pcMenuExit')}>{translateText('fractions', 'Выход')}</li>
            </menu>
        </div>
        <div class="right">
            <svelte:component this={Views[SelectViews]} {...vehicleData} {...userData} {wantedData} />
        </div>
        <div class="bottom"></div>
    </div>
</div>