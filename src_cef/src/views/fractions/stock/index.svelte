<script>
    import { translateText } from 'lang'
    import { executeClient } from 'api/rage'
    import './assets/sass/stock.sass'
    export let viewData;

    let title = "",
        style = "",
        index = 0,
        count = [];

	
    $: {
        if (count !== JSON.parse(viewData)) {
            count = JSON.parse(viewData);
            onChange ();
        }
    }

    const leftHandle = () => {
        index--;
        if(index < 0) index = 4;
        onChange ();
    }

    const rightHandle = () => {
        index++;
        if(index > 4) index = 0;
        onChange ();
    }

    const onChange = () => {
        switch (index) {
            case 0:
                style = 'cash';
                title = translateText('fractions', 'Деньги');
                break;
            case 1:
                style = 'healkit';
                title = translateText('fractions', 'Аптечка');
                break;
            case 2:
                style = 'weed';
                title = translateText('fractions', 'Наркотики');
                break;
            case 3:
                style = 'weapons';
                title = translateText('fractions', 'Оружейные материалы');
                break;
            case 4:
                style = 'weaponsstock';
                title = translateText('fractions', 'Оружейный склад');
                break;
        } 
    }

    const takeHandle = () => {
        executeClient ('stockTake', index);
    }

    const putHandle = () => {
        executeClient ('stockPut', index);
    }

    const exitHandle = () => {
        executeClient ('stockExit');
    }
</script>

<div id="stock">
    <div class="stock module_stock">
        <div class="mod">
            <div class="arrows left icon-arrow" on:click={leftHandle} /> 
            <div class="contain">
                <div class="icon-block">
                    <div class={"ic " + style} />
                </div>
                <div class="title">{title}</div>
            </div>
            <div class="arrows icon-arrow" on:click={rightHandle} />
        </div>
        <div class="button">
            <div id="take" class="btn green" on:click={takeHandle}>{translateText('fractions', 'Взять')}</div>
            <div id="put" class="btn red" on:click={putHandle}>{translateText('fractions', 'Положить')}</div>
        </div>
        <div class="count">
            <div class="gray">{translateText('fractions', 'Всего')}:</div>
            <div class="white">{count[index]}</div>
        </div>
        <div class="button bottom">
            <div id="exit" class="exit btn red" on:click={exitHandle}>{translateText('fractions', 'Выход')}</div>
        </div>
    </div>
</div>