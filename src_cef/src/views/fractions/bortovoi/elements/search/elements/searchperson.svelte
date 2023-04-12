<script>
    import { translateText } from 'lang'
    let inputValue = "";
    
    const onKeyUp = (event) => {
        const { keyCode } = event;
        if (keyCode == 13)
            onClose ()
    }
</script>
<svelte:window on:keyup={onKeyUp} />
<div class="policecomputer__input">
    <div class="bortovoiicon-loop"></div>
    <input placeholder="Введите ключевые слова" bind:value={inputValue} maxlength="50">
</div>
{#if inputValue.length == 0}
    <div class="policecomputer__descr">
        {translateText('fractions', 'Введите имя и фамилию гражданина, его номер паспорта или номер телефона для просмотра дополнительной информации о нем.')}
    </div>
    <div class="box-flex">
        <div class="bortovoiicon-info"></div>
        <div class="policecomputer__bold">{translateText('fractions', 'Помощь по работе с Бортовым Компьютером')}</div>
    </div>
    <div class="policecomputer__block_info">
        {translateText('fractions', 'Для оформления Data Base введите в поиск информацию о задержанном. Если хотите объявить гражданина или Т/С в розыск, то воспользуйтесь кнопкой “Объявить в розыск” и заполните информацию.')}
    </div>
    <div class="box-between mt-auto">
        <div class="policecomputer__font_20">
            {translateText('fractions', 'Список последних задержанных')}:
        </div>
        <div class="policecomputer__font_18">
            {translateText('fractions', 'Перейти в Датабейз')} >
        </div>
    </div>
    <div class="box-between mt-20">
        <div class="policecomputer__database_element">
            <div class="policecomputer__database_image"></div>
            <div class="policecomputer__database_number">{translateText('fractions', 'Досье')} №43332</div>
            <div class="policecomputer__database_name">Vitaliy Zdobich</div>
            <div class="policecomputer__database_info">
                УК <span class="white">12.6, 13.4</span> АК <span class="white">12.6, 13.4</span>
            </div>
        </div>
        <div class="policecomputer__database_element">
            <div class="policecomputer__database_image"></div>
            <div class="policecomputer__database_number">{translateText('fractions', 'Досье')} №43332</div>
            <div class="policecomputer__database_name">Vitaliy Zdobich</div>
            <div class="policecomputer__database_info">
                УК <span class="white">12.6, 13.4</span> АК <span class="white">12.6, 13.4</span>
            </div>
        </div>
        <div class="policecomputer__database_element">
            <div class="policecomputer__database_image"></div>
            <div class="policecomputer__database_number">{translateText('fractions', 'Досье')} №43332</div>
            <div class="policecomputer__database_name">Vitaliy Zdobich</div>
            <div class="policecomputer__database_info">
                УК <span class="white">12.6, 13.4</span> АК <span class="white">12.6, 13.4</span>
            </div>
        </div>
        <div class="policecomputer__database_element">
            <div class="policecomputer__database_image"></div>
            <div class="policecomputer__database_number">{translateText('fractions', 'Досье')} №43332</div>
            <div class="policecomputer__database_name">Vitaliy Zdobich</div>
            <div class="policecomputer__database_info">
                УК <span class="white">12.6, 13.4</span> АК <span class="white">12.6, 13.4</span>
            </div>
        </div>
    </div>
{/if}
{#if inputValue.length > 0}
    <div class="policecomputer__search">{translateText('fractions', 'По запросу')} "{inputValue}" {translateText('fractions', 'найдено')}:</div>
    <div class="bortovoie__tablet_element title">
        <div class="name">{translateText('fractions', 'Имя')}:</div>
        <div class="passport">{translateText('fractions', 'Паспорт')}:</div>
        <div class="number">{translateText('fractions', 'Номер телефона')}:</div>
        <div class="status">{translateText('fractions', 'Статус')}:</div>
        <div class="info"></div>
    </div>
    <div class="policecomputer__tablet">
        {#each new Array(20) as item}
            <div class="bortovoie__tablet_element">
                <div class="name">Vitaliy Zdobich</div>
                <div class="passport">1233332</div>
                <div class="number">144232321</div>
                <div class="status wanted">{translateText('fractions', 'В розыске')}</div>
                <div class="info hover">{translateText('fractions', 'Инфо')} ></div>
            </div>
        {/each}
    </div>
{/if}