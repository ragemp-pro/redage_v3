<script>
    import { houseType } from 'json/realEstate.js'
    import { format } from 'api/formatter'
    export let selectData;
    export let buyPrice;
</script>
<div class="rielt__rielt_info">
    <div class="houseicon-house rielt__rielt_house-icon"></div>
    <div class="rielt__rielt_header">Дом №{selectData[0]}</div>
    <div class="rielt__gray">Замечательный дом по доступной цене</div>
    <div class="rielt__rielt_stat">
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Налоги:</div>
            <div>{selectData[3]}</div>
        </div>
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Класс:</div>
            <div>{houseType [selectData[1]]}</div>
        </div>
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Гаражных мест:</div>
            <div>{selectData[4]}</div>
        </div>
        <div class="rielt__rielt_element">
            <div class="rielt__gray">Гос. цена:</div>
            <div>${format("money", selectData[2])}</div>
        </div>
    </div>
    <div class="box-column">
        <div class="rielt__rielt_price">${format("money", (selectData[1] + 1) * buyPrice)}</div>
        <div class="rielt__gray">Цена за сведения:</div>
    </div>
</div>