<script>
    import { businessType } from 'json/realEstate.js'
    import { format } from 'api/formatter'
    export let selectData;
    export let buyPrice;
</script>

<div class="rielt__rielt_info">
    <div class="houseicon-house rielt__rielt_house-icon"></div>
    <div class="rielt__rielt_header">Бизнес №{selectData[0]}</div>
    <div class="rielt__gray">Замечательный бизнес по доступной цене</div>
    <div class="rielt__rielt_stat">
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Налоги:</div>
            <div>{selectData[3]}</div>
        </div>
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Тип:</div>
            <div>{businessType [selectData[1]]}</div>
        </div>
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Гос. цена:</div>
            <div>${format("money", selectData[2])}</div>
        </div>
    </div>
    <div class="box-column">
        <div class="rielt__rielt_price">${format("money", buyPrice)}</div>
        <div class="rielt__gray">Цена:</div>
    </div>
</div>