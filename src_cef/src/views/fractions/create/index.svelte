<script>
    import { translateText } from 'lang'
    import { format } from "api/formatter";
    import { executeClient } from "api/rage";

    import './main.sass' 

    export let viewData;

    let isCrime = false;
    const onSelectType = (_isCrime) => {
        isCrime = _isCrime;
    }


    let orgName = ""

    const onCreate = () => {
        let check = format("createOrg", orgName);
        if (!check.valid) {
            window.notificationAdd(4, 9, check.text, 3000);
            return;
        }

        executeClient('client.org.create.buy', isCrime, orgName)
    }

    const onClose = () => {
        executeClient('client.org.create.close')

    }
    const onKeyUp = (event) => {
        const { keyCode } = event;


        if (keyCode == 13)
            onCreate ()

        if (keyCode == 27)
            onClose ()
    }
</script>

<svelte:window on:keyup={onKeyUp}/>

<div id="fractionscreate">
    <div class="newproject__buttonblock" on:click={onClose}>
        <div class="newproject__button">ESC</div>
        <div>{translateText('player', 'Закрыть')}</div>
    </div>
    <div class="fractionscreate__title">Создание организации</div>
    <div class="fractionscreate__subtitle">В данном меню вы можете выбрать тип своей организации и создать её. Подходите к процессу создания серьезно!</div>
    <div class="fractionscreate__text_title">Тип организации:</div>
    <div class="fractionscreate__element mb-32" class:active={isCrime} on:click={() => onSelectType (true)}>
        <div class="fractionscreate__checkbox">
            <div class="active"></div>
        </div>
        <div class="box-column">
            <div class="box-flex">
                <div class="fractionscreate__icon crime"></div>
                <div class="fractionscreate__element_title">Группировка</div>
            </div>
            <div class="fractionscreate__element_type crime">Криминальная организация</div>
            <div class="fractionscreate__element_text">Ячейка преступного мира, деятельность которой состоит в формировании альянсов с другими криминальными фракциями, войне за влияние, ограблениях, похищениях и стычках с силовыми и юридическими структурами.</div>
        </div>
    </div>
    <div class="fractionscreate__element" class:active={!isCrime} on:click={() => onSelectType (false)}>
        <div class="fractionscreate__checkbox">
            <div class="active"></div>
        </div>
        <div class="box-column">
            <div class="box-flex">
                <div class="fractionscreate__icon gos"></div>
                <div class="fractionscreate__element_title">Сообщество</div>
            </div>
            <div class="fractionscreate__element_type gos">Легальная организация</div>
            <div class="fractionscreate__element_text">Частное предприятие, которое подписывает контракт с правительством для защиты прав и интересов государства. Имеет возможность оказывать влияние на политические и силовые структуры, является частью государственной системы.</div>
        </div>
    </div>
    <div class="fractionscreate__text_title">Название организации:</div>
    <div class="box-flex">
        <input class="fractionscreate__input" placeholder="Введите название.." bind:value={orgName}>
        <div class="box-column">
            <div class="fractionscreate__small">Стоимость создания:</div>
            <div>${format("money", viewData)}</div>
        </div>
    </div>
    <div class="fractionscreate__info">
        Название от 3 и до 30 символов
        <br>
        Запрещено использовать в названии нецензурные выражения, оскорбления или названия существующих организаций.
    </div>
    <div class="fractionscreate__button" on:click={onCreate}>
        Создать организацию
    </div>
</div>