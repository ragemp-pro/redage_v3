<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage';
    export let fname;
    export let lname;
    export let pass;
    export let fraction_name;
    export let phonenumber;
    export let gender;
    export let lvl;
    export let lic;
    export let houseInfo;

    let value = "";
</script>


<h1>{translateText('fractions', 'База данных')}</h1>
<div class="elements">
    <input bind:value={value} type="text" maxLength="30" placeholder={translateText('fractions', 'Паспорт/Имя_Фамилия')}/>
    <div class="button" on:click={() => executeClient ('client:pcMenuInput', "checkPerson", value)}>{translateText('fractions', 'Пробить')}</div>
    
    <p>{translateText('fractions', 'Имя')}: <span>{fname}</span></p>
    <p>{translateText('fractions', 'Фамилия')}: <span>{lname}</span></p>
    <p>{translateText('fractions', 'Паспорт')}: <span>{pass}</span></p>
    <p>{translateText('fractions', 'Фракция')}: <span>{fraction_name != undefined ? fraction_name : ""}</span></p>
    <p>{translateText('fractions', 'Номер телефона')}: <span>{phonenumber}</span></p>
    <p>{translateText('fractions', 'Пол')}: <span>{gender}</span></p>
    <p>{translateText('fractions', 'Ур. Розыска')}: <span>{lvl}</span></p>
    <p>{translateText('fractions', 'Список лицензий')}: <span>{lic}</span></p>
    <p>{translateText('fractions', 'Дом')}: <span>{houseInfo}</span></p>
</div>